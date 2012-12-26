/* This file is part of VoltDB.
 * Copyright (C) 2008-2013 VoltDB Inc.
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
 * OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using VoltDB.Data.Client.Properties;
using VoltDB.ThirdParty.Mono;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides the core functionalities for a VoltDB connection to a single host.
    /// </summary>
    public sealed partial class VoltNodeConnection : VoltConnection
    {
        /// <summary>
        /// Underlying Volt Protocol stream to/from which data is written/read.
        /// </summary>
        private VoltProtocolStream BaseStream;

        /// <summary>
        /// The background network thread that will read the server responses and dispatch callbacks.
        /// </summary>
        private Thread BackgroundNetworkThread;

        /// <summary>
        /// The background timeout control thread that will monitor pending requests for possible timeout
        /// interruptions.
        /// </summary>
        private Thread BackgroundTimeoutThread;

        /// <summary>
        /// Tracking of terminal exceptions: we don't let background threads throw, but we report at the top-level on
        /// the next call.
        /// </summary>
        private VoltExecutionException TerminalException = null;

        /// <summary>
        /// Open the connection.
        /// </summary>
        /// <returns>A reference to the current connection instance for action chaining.</returns>
        public override VoltConnection Open()
        {
            // Synchronize access.
            lock (this.SyncRoot)
            {
                // Validate connection status.
                if (this.Status != ConnectionStatus.Closed)
                    throw new InvalidOperationException(string.Format(Resources.InvalidConnectionStatus, this.Status));

                // Set status to "connecting".
                this.Status = ConnectionStatus.Connecting;

                // Connect to the default endpoint (there should only be one anyways if this is a managed pool
                // connection, otherwise, you're on your own - this is a connection, not a Pool!).
                IPEndPoint endPoint = this.Settings.DefaultIPEndPoint;
                try
                {
                    // Create new socket stream and wrap a core protocol stream manager around it.
                    this.BaseStream = new VoltProtocolStream(endPoint, this.Settings.ConnectionTimeout);

                    // Now send login message.
                    using (Serializer serializer = new Serializer())
                    {
                        var msg = serializer
                                  .Write(this.Settings.ServiceType.ToString().ToLowerInvariant())
                                  .Write(this.Settings.UserID)
                                  .WriteRaw(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(this.Settings.Password)))
                                  .GetBytes();
                        this.BaseStream.WriteMessage(msg.Array, msg.Offset, msg.Count);
                    }

                    // Receive and process login response message.
                    var deserializer = new Deserializer(this.BaseStream.ReadMessage());
                    LoginResponseStatus status = (LoginResponseStatus)deserializer.ReadSByte();
                    if (status != LoginResponseStatus.Connected)
                    {
                        // Re-package server response in an appropriate exception.
                        switch (status)
                        {
                            case LoginResponseStatus.ConnectionHandshakeTimeout:
                                throw new VoltConnectionException(
                                                                   Resources.LRS_ConnectionHandshakeTimeout
                                                                 , endPoint
                                                                 , status
                                                                 );

                            case LoginResponseStatus.CorruptedHandshake:
                                throw new VoltConnectionException(
                                                                   Resources.LRS_CorruptedHandshake
                                                                 , endPoint
                                                                 , status
                                                                 );

                            case LoginResponseStatus.InvalidCredentials:
                                throw new VoltPermissionException(
                                                                   Resources.LRS_InvalidCredentials
                                                                 , endPoint
                                                                 , status
                                                                 );

                            case LoginResponseStatus.ServerTooBusy:
                                throw new VoltConnectionException(
                                                                   Resources.LRS_ServerTooBusy
                                                                 , endPoint
                                                                 , status
                                                                 );

                            default:
                                throw new VoltConnectionException(Resources.LRS_Unknown, endPoint, status);
                        }
                    }
                    // Parse the rest of the response the get core cluster information.
                    try
                    {
                        this.ServerHostId = deserializer.ReadInt();
                        this.ConnectionId = deserializer.ReadLong();
                        this.ClusterStartTimeStamp = deserializer.ReadDateTimeFromMilliseconds();
                        this.LeaderIPEndPoint = new IPEndPoint(
                                                                new IPAddress(deserializer.ReadRaw(4))
                                                              , endPoint.Port
                                                              );
                        this.BuildString = deserializer.ReadString();
                    }
                    catch (Exception x)
                    {
                        throw new VoltConnectionException(Resources.LR_FailedParsingResponse, x, endPoint);
                    }

                    // Now that we are successfully connected, set the socket timeout to infinite (we are constantly
                    // listening for new messages).
                    this.BaseStream.ResetTimeout(Timeout.Infinite);

                    // Create background threads.
                    this.BackgroundNetworkThread = new Thread(this.BackgroundNetworkThreadRun)
                                                    {
                                                        IsBackground = true,
                                                        Priority = ThreadPriority.AboveNormal
                                                    };
                    this.BackgroundTimeoutThread = new Thread(this.BackgroundTimeoutThreadRun)
                                                    {
                                                        IsBackground = true
                                                    };

                    // Starting background processing threads.
                    this.BackgroundNetworkThread.Start();
                    this.BackgroundTimeoutThread.Start();

                    // Start Callback executor
                    this.CallbackExecutor.Start();

                    // Ensure terminal exception is reset
                    this.TerminalException = null;

                    // Connection is now ready.
                    this.Status = ConnectionStatus.Connected;

                    // Initialize statistics as needed.
                    if (this.Settings.StatisticsEnabled)
                    {
                        this.Stats = new Dictionary<string, Statistics>(StringComparer.OrdinalIgnoreCase);
                        // For lifetime statistics, keep track of previous connection cycles, if any.
                        if (this.LifetimeStats != null)
                        {
                            if (this.PastLifetimeStats != null)
                                this.PastLifetimeStats.SummarizeWith(this.LifetimeStats);
                            else
                                this.PastLifetimeStats = this.LifetimeStats;
                        }
                        this.LifetimeStats = new Statistics();
                    }

                    // Trace as needed
                    if (this.Settings.TraceEnabled)
                        VoltTrace.TraceEvent(
                                              TraceEventType.Information
                                            , VoltTraceEventType.ConnectionOpened
                                            , Resources.TraceConnectionOpened
                                            , this.ServerHostId
                                            , this.ConnectionId
                                            , endPoint
                                            , this.LeaderIPEndPoint
                                            , this.ClusterStartTimeStamp
                                            , this.BuildString
                                            );
                }
                catch (Exception x)
                {
                    try
                    {
                        // In case of failure, terminate everything immediately (will correct status).
                        this.Terminate();
                    }
                    catch { }
                    // Re-throw exception, wrapping if needed.
                    if (x is VoltException)
                        throw new VoltConnectionException(Resources.ConnectionFailure, x, endPoint);
                    else
                        throw x;
                }
            }
            return this;
        }

        /// <summary>
        /// Running process for the background thread that reads and parses out server messages and triggers callbacks.
        /// </summary>
        private void BackgroundNetworkThreadRun()
        {
            try
            {
                byte[] message;
                while ((message = this.BaseStream.ReadMessage()) != null)
                {
                    // Read the execution id from the server message to pull the corresponding pending response from
                    // the internal queue.
                    long executionId = DataConverter.BigEndian.GetInt64(message, 0);

                    // Get the response from the stack
                    Response response = this.ExecutionCache.BeginRemoveItem(executionId);

                    // If the response wasn't already dealt with, finalize processing.
                    if (response != null)
                    {
                        // Trigger the callback.  The call is non-blocking: the callback is queued for execution in the
                        // ThreadPool.
                        response.OnExecutionComplete(message);
                        // Call completion handler for classes that provide additional tracking
                        this.OnExecutionComplete(
                                                  executionId
                                                , response.Procedure
                                                , response.ExecutionDuration
                                                , response.Status
                                                , message.LongLength
                                                );
                    }
                    // else: message is discarded: the request timed-out or was cancelled by the client and wasn't
                    // hoped for anymore.
                }
            }
            // ThreadAborts will occur when the connection is closed or the application terminates - swallow them.
            catch (ThreadAbortException) { }
            catch (Exception x)
            {
                // If this thread dies, we have no alternative than to kill the connection entirely.
                this.Terminate();
                if (this.TerminalException == null)
                    this.TerminalException = new VoltExecutionException(Resources.BackgroundNetworkThreadDied, x);
            }
        }

        /// <summary>
        /// Running process for the background thread that monitors time-out conditions and triggers callbacks.
        /// </summary>
        private void BackgroundTimeoutThreadRun()
        {
            try
            {
                while (true)
                {
                    var expiredExecutionIds = this.ExecutionCache.GetExpiredItems();

                    foreach (long expiredExecutionId in expiredExecutionIds)
                    {

                        // Get the response from the stack.
                        Response response = this.ExecutionCache.BeginRemoveItem(expiredExecutionId);

                        // If the response wasn't already dealt with, finalize processing
                        if (response != null)
                        {
                            // Trigger the callback.  The call is non-blocking: the callback is queued for execution
                            // in the ThreadPool.
                            response.OnExecutionTimeout();
                            // Call completion handler for classes that provide additional tracking.
                            this.OnExecutionComplete(
                                                      expiredExecutionId
                                                    , response.Procedure
                                                    , response.ExecutionDuration
                                                    , response.Status
                                                    , 0 // Not accurate: the response might come later...
                                                    );
                        }
                    }
                    // Sleep a while, just to prevent CPU 100%.  Note tat this means there is a 1ms+ inprecision on
                    // triggerring the callback.
                    // Thread.Sleep(1);

                    // PERF: So, we should have GetExpiredItems tell us when the next expiration date is
                    // It's a sort of long function (iterate over entire dictionary), and locks it for adds/removes
                    // PERF: sebc - Not sure I love that 100ms sleep when there is nothing in the queue.  With a bit of
                    // ill timing, this will introduce an artificial 100ms additional latency on the first procedure
                    // call that hits after a period of quietness - on low-volume application where the submission rate
                    // is slower than (100ms + avg-procedure-execution-duration), all client requests would be
                    // artificially delayed.  This said, we're talking about triggering timeout callbacks (not as
                    // triggering the calback for successful executions) and the gain in CPU footprint is major (20%).
                    // Might need to refine again in the future.
                    if (expiredExecutionIds.Count > 0)
                    {
                        Thread.Sleep(10);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            // ThreadAborts will occur when the connection is closed or the application terminates - swallow them.
            catch (ThreadAbortException) { }
            catch (Exception x)
            {
                // If this thread dies, we have no alternative than to kill the connection entirely.
                this.Terminate();
                if (this.TerminalException == null)
                    this.TerminalException = new VoltExecutionException(Resources.BackgroundTimeoutThreadDied, x);
            }
        }

        /// <summary>
        /// For internal use only: serializes a procedure call into a message ready to be sent to the server.
        /// </summary>
        /// <param name="executionId">The reference id for this procedure call.</param>
        /// <param name="procedureUtf8">The procedure to call.</param>
        /// <param name="parameters">The list of parameters to pass to the procedure.</param>
        /// <returns>Serialized procedure call message, ready to post to the server.</returns>
        static ArraySegment<byte> GetProcedureCallMessage(long executionId, byte[] procedureUtf8, params object[] parameters)
        {
            // Format and return the message
            using (Serializer serializer = new Serializer())
                return serializer.WriteStringInternal(procedureUtf8)
                                 .Write(executionId)
                                 .WriteParameters(parameters)
                                 .GetBytes();
        }

        /// <summary>
        /// Terminates the connection and closes all resources.
        /// </summary>
        internal void Terminate()
        {
            // Mark connection as closed
            this.Status = ConnectionStatus.Closed;

            // Stop background threads - swallow any undue exception...
            try
            {
                // Kill the threads, the stream.
                try
                {
                    if (this.OwnsCallbackExecutor)
                        this.CallbackExecutor.Stop();
                    this.BackgroundNetworkThread.Abort();
                    this.BackgroundTimeoutThread.Abort();
                    this.BaseStream.Close();
                }
                catch { }

                // Trigger all callbacks on executions we will not be able to fulfill.
                try
                {
                    long[] killList = this.ExecutionCache.GetCurrentItems();
                    while (killList.Length > 0)
                    {
                        foreach (long expiredExecutionId in killList)
                        {
                            // Get the response from the stack
                            Response response = this.ExecutionCache.BeginRemoveItem(expiredExecutionId);

                            // If the response wasn't already dealt with, finalize processing.
                            if (response != null)
                            {
                                // Trigger the callback.  The call is non-blocking: the callback is queued for
                                // execution in the ThreadPool.
                                response.OnExecutionAbort();
                                // Call completion handler for classes that provide additional tracking.
                                this.OnExecutionComplete(
                                                          expiredExecutionId
                                                        , response.Procedure
                                                        , response.ExecutionDuration
                                                        , response.Status
                                                        , 0 // Not accurate: the response might come later...
                                                        );
                            }
                        }
                        Thread.Sleep(10);
                        killList = this.ExecutionCache.GetCurrentItems();
                    }
                }
                catch { }

                // Freeze statistics so the elapsed time doesn't report nonsensical figures.
                if (this.Settings.StatisticsEnabled)
                {
                    lock ((this.Stats as IDictionary).SyncRoot)
                        foreach (KeyValuePair<string, Statistics> pair in this.Stats)
                            pair.Value.Freeze();
                    this.LifetimeStats.Freeze();
                }

                // Trace as needed.
                if (this.Settings.TraceEnabled)
                    VoltTrace.TraceEvent(
                                          TraceEventType.Information
                                        , VoltTraceEventType.ConnectionClosed
                                        , Resources.TraceConnectionClosed
                                        , this.ServerHostId
                                        , this.ConnectionId
                                        );
            }
            catch { }
        }
    }
}