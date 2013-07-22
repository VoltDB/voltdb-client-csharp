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
using System.Linq;
using System.Net;
using System.Threading;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides the core functionalities for a VoltDB connection to a single host.
    /// </summary>
    public sealed partial class VoltNodeConnection : VoltConnection
    {
        /// <summary>
        /// Internal flag indicating this connection owns the callback executor and should stop it when it terminates.
        /// </summary>
        private bool OwnsCallbackExecutor = false;

        /// <summary>
        /// Internal map of pending responses expected from the server.  Used to recall callbacks.
        /// </summary>
        private ExecutionCache ExecutionCache;

        /// <summary>
        /// Internal tracking execution request id.  Allows the connection to match back server responses to original
        /// requests for callback recall.
        /// </summary>
        private long ExecutionId = 0;

        /// <summary>
        /// Internal dictionary of performance statistics, by procedure.
        /// </summary>
        private Dictionary<string, Statistics> Stats = null;

        /// <summary>
        /// Internal global statistics for the connection.
        /// </summary>
        private Statistics LifetimeStats = null;

        /// <summary>
        /// Internal global statistics for the connection's "past lifetimes": if a connection goes through multiple
        /// Open/Close cycles, we keep track of all past global statistics to provide a full trail.  We only do this
        /// for un-resetable lifetime stats though: it would get really convoluted (and confusing for the user in the
        /// end) to keep track of procedure-level data and keep that in tabs with reset requests (or not - which would
        /// the caller want?)
        /// </summary>
        private Statistics PastLifetimeStats = null;

        /// <summary>
        /// Internal reference of the last execution id prior to a statistics reset - used to filter out responses when
        /// the user requests it.
        /// </summary>
        private long StatisticsResetExecutionId = 0;

        /// <summary>
        /// Creates a new single-node connection.  Internal usage only (Factory pattern on the base class).
        /// </summary>
        /// <param name="settings">Connection settings to use to connect to the node.</param>
        /// <param name="callbackExecutor">The callback executor provided by the factory creator that will execute all
        /// callbacks</param>
        internal VoltNodeConnection(ConnectionSettings settings, CallbackExecutor callbackExecutor)
            : base(settings)
        {
            this.ExecutionCache = new ExecutionCache(settings.MaxOutstandingTxns);
            // Reference to the global callback executor
            this.CallbackExecutor = callbackExecutor;
        }

        /// <summary>
        /// Creates a new single-node connection.  Internal usage only (Factory pattern on the base class).
        /// </summary>
        /// <param name="settings">Connection settings to use to connect to the node.</param>
        /// <param name="callbackExecutor">The callback executor provided by the factory creator that will execute all
        /// callbacks</param>
        /// <param name="ownsCallbackExecutor">Flag indicating this connection owns the callback executor and should
        /// stop it when it terminates.</param>
        internal VoltNodeConnection(ConnectionSettings settings, CallbackExecutor callbackExecutor, bool ownsCallbackExecutor)
            : this(settings, callbackExecutor)
        {
            this.OwnsCallbackExecutor = ownsCallbackExecutor;
        }

        /// <summary>
        /// Synchronously execute a procedure against the VoltDB server and block execution of the calling thread.
        /// </summary>
        /// <typeparam name="T">Data type of the expected response result for the call (Table[], Table,
        /// SingleRowTable[], SingleRowTable, T[][], T[] or T, with T one of the supported value types).</typeparam>
        /// <param name="timeout">Timeout value (overrides connection settings DefaultCommandTimeout). Use
        /// Timeout.Infinite or -1 for infinite timeout.</param>
        /// <param name="procedure">The name of the procedure to call.</param>
        /// <param name="procedureUtf8">UTF-8 bytes of the procedure name.</param>
        /// <param name="parameters">List of parameters to pass to the procedure.</param>
        /// <returns>The reponse from the server, containing (if available) the result of the procedure execution
        /// (check .IsError prior to using the result).</returns>
        /// <remarks>Synchronous operations will limit the throughput of your aplication, as each of your request waits
        /// for completion before submitting a new call.  For maximum parallelization and throughput, asynchronous
        /// calls should be made (see BeginExecute).</remarks>
        public override Response<T> Execute<T>(int timeout, string procedure, byte[] procedureUtf8, params object[] parameters)
        {
            // Call the asynchronous method and immediately turn around to call .EndExecute.
            return VoltConnection.EndExecute<T>(this.BeginExecute<T>(null, null, timeout, procedure, procedureUtf8, parameters));
        }

        /// <summary>
        /// Asynchronously execute a procedure against a VoltDB database, returning immediately to the calling thread.
        /// The provided callback will be fired upon completion.
        /// </summary>
        /// <typeparam name="T">Data type of the expected response result for the call (Table[], Table,
        /// SingleRowTable[], SingleRowTable, T[][], T[] or T, with T one of the supported value types).</typeparam>
        /// <param name="callback">The callback method to call upon completion.</param>
        /// <param name="state">A user-defined state object to be passed to your callback through the Response's
        /// .AsyncState property</param>
        /// <param name="timeout">Timeout value (overrides connection settings DefaultCommandTimeout). Use
        /// Timeout.Infinite or -1 for infinite timeout.</param>
        /// <param name="procedure">The name of the procedure to call.</param>
        /// <param name="procedureUtf8">The UTF-8 bytes of the procedure name.</param>
        /// <param name="parameters">List of parameters to pass to the procedure.</param>
        /// <returns>The execution handle for the request.</returns>
        public override AsyncResponse<T> BeginExecute<T>(
                                                                     ExecuteAsyncCallback<T> callback
                                                                   , object state
                                                                   , int timeout
                                                                   , string procedure
                                                                   , byte[] procedureUtf8
                                                                   , params object[] parameters
                                                                   )
        {
            // Correct default timeout usage.
            if (timeout == 0)
                timeout = this.Settings_DefaultCommandTimeout;
            else if (timeout < -1)
                throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidTimeoutValue, timeout));

            // Validate connection status.
            if (this.Status != ConnectionStatus.Connected)
            {
                if (this.TerminalException != null)
                    throw this.TerminalException;
                else
                    throw new InvalidOperationException(string.Format(Resources.InvalidConnectionStatus, this.Status));
            }

            // Assign execution id.
            long executionId = Interlocked.Increment(ref this.ExecutionId);

            // Prepare the response object.
            AsyncResponse<T> response = new AsyncResponse<T>(this, executionId, timeout, callback, state, procedure, parameters);

            // Attempt asynchronouse execution - any failure will trigger a synchronous failure.
            try
            {
                // This might fail (invalid parameters / exceeding max string length, for instance) - the equivalent of an
                // ArgumentException, so we leave it outside of the try/catch block related to protecting ourselves against
                // connectivity issues.
                if (parameters.Length == 1 && parameters[0] is Array)
                {
                    parameters = (object[]) parameters[0];
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parameters[i] = VoltType.CoalesceNull(parameters[i]);
                    }
                }


                var message = GetProcedureCallMessage(executionId, procedureUtf8, parameters);

                // Block if we reached queue capacity.
                while (this.ExecutionCache.Size >= this.Settings_MaxOutstandingTxns)
                {
                    Thread.Sleep(1);
                }

                // The request appears to be properly formatted and ready to ship - push it in the queue.
                this.ExecutionCache.AddItem(response);

                // Write out execution request to protocol stream - lock out access for thread safety around the underlying
                // stream.
                lock (this.SyncRoot)
                {
                    try
                    {
                        this.BaseStream.WriteMessage(message.Array, message.Offset, message.Count);
                    }
                    catch (Exception x)
                    {
                        // We will only get here in case of a network/connection failure.

                        // Swallow any exception: the background processing thread will pick up a network failure as well
                        // and we will let it initiate termination and kick out the abort on the request just posted.

                        // We still set the Terminal exception so we report the *first* exception encountered (failing to
                        // write on the stream here, in case the background thread picks things up first, failing to read).
                        this.TerminalException = new VoltExecutionException(Resources.ConnectionClosedDuringAWrite, x);

                        // But, as stated, we do not re-throw: connection termination will ensure that all pending requests
                        // are rejected with an "abort" exception.  The next execution call will fail directly on a
                        // "connection closed" error, or be blocked until recovery is complete.
                    }
                }

                // Track statistics as needed.
                TrackStatisticsOpenRequest(procedure, message.Count);

                // Trace as needed.
                if (this.Settings_TraceEnabled)
                    VoltTrace.TraceEvent(
                                          TraceEventType.Information
                                        , VoltTraceEventType.ExecutionStarted
                                        , Resources.TraceExecutionStarted
                                        , this.ServerHostId
                                        , this.ConnectionId
                                        , executionId
                                        , procedure
                                        );
            }
            catch (Exception x)
            {
                response.OnExecutionRequestFailed(new VoltExecutionException(Resources.RequestExecutionFailure, x));
            }
            // Return execution handle to caller.
            return response;
        }

        /// <summary>
        /// Request a client-side abort of a given transaction.
        /// </summary>
        /// <param name="handle">The execution handle of the request to cancel.</param>
        /// <returns>True if the abort succeeded, false if it was already too late (and the request completed or timed
        /// out before the abort could be processed).</returns>
        internal bool ExecuteCancelAsync<T>(AsyncResponse<T> handle)
        {
            // Validate connection status.
            // Note: We're not throwing anything, just returning immediately if the connection was closed (failure
            // already caused the abort callback)
            if (this.Status != ConnectionStatus.Connected)
                return false;

            // Get the response from the stack.
            Response response = this.ExecutionCache.BeginRemoveItem(handle.ExecutionId);

            // If the response wasn't already dealt with, finalize processing.
            if (response != null)
            {
                // Trigger the callback.  The call is non-blocking: the callback is queued for execution in the
                // ThreadPool.
                response.OnExecutionAbort();
                // Call completion handler for classes that provide additional tracking.
                this.OnExecutionComplete(
                                          handle.ExecutionId
                                        , response.Procedure
                                        , response.ExecutionDuration
                                        , ResponseStatus.Aborted
                                        , 0 // Not accurate: the response might come later, but since we'll drop it...
                                        );

                // Return true to indicate the request was aborted
                return true;
            }

            // Return false to indicate it was already too late to abort the request.
            return false;
        }

        /// <summary>
        /// Provides a hookup on internal completetion of procedure calls (used by derived classes for performance
        /// monitoring and tracing.
        /// </summary>
        /// <param name="executionId">The execution id of the request that was just completed.</param>
        /// <param name="procedure">The name of the procedure that was run.</param>
        /// <param name="executionDuration">The duration of the execution that was just completed.</param>
        /// <param name="status">The status of the execution request's response.</param>
        /// <param name="bytesReceived">Size of the response, in bytes, for network I/O tracking.</param>
        internal void OnExecutionComplete(
                                           long executionId
                                         , string procedure
                                         , int executionDuration
                                         , ResponseStatus status
                                         , long bytesReceived
                                         )
        {
            // TODO: Revise network tracking so that dropped (timeout or cancel) responses are still tracked adequately
            // Track statistics as needed.
            TrackStatisticsCloseRequest(executionId, procedure, executionDuration, status, bytesReceived);

            // Trace as needed.
            if (this.Settings_TraceEnabled)
                VoltTrace.TraceEvent(
                                      status != ResponseStatus.Success
                                      ? TraceEventType.Error
                                      : TraceEventType.Information

                                    , status == ResponseStatus.Timedout
                                      ? VoltTraceEventType.ExecutionTimedout
                                      : status == ResponseStatus.Failed
                                        ? VoltTraceEventType.ExecutionFailed
                                        : status == ResponseStatus.Aborted
                                          ? VoltTraceEventType.ExecutionAborted
                                          : VoltTraceEventType.ExecutionCompleted

                                    , status == ResponseStatus.Timedout
                                      ? Resources.TraceExecutionTimedout
                                      : status == ResponseStatus.Failed
                                        ? Resources.TraceExecutionFailed
                                        : status == ResponseStatus.Aborted
                                          ? Resources.TraceExecutionAborted
                                          : Resources.TraceExecutionCompleted

                                    , this.ServerHostId
                                    , this.ConnectionId
                                    , executionId
                                    , procedure
                                    , executionDuration
                                    );

            // Confirm the item as removed in the execution cache.
            this.ExecutionCache.EndRemoveItem();
        }

        /// <summary>
        /// Blocks the calling thread until the queue of pending responses is fully exhausted (all responses received
        /// by the server).
        /// </summary>
        /// <returns>A reference to the current connection instance for action chaining.</returns>
        public override VoltConnection Drain()
        {
            // Synchronize access - this will take another lock if the call originates from "Close", but it actually
            // won't: the lock's owning thread will fly through since it owns the lock - everybody else will be stuck.
            lock (this.SyncRoot)
            {
                // Validate connection status.
                if (!((this.Status == ConnectionStatus.Connected) || (this.Status == ConnectionStatus.Closing)))
                    throw new InvalidOperationException(string.Format(Resources.InvalidConnectionStatus, this.Status));

                // Trace as needed.
                if (this.Settings_TraceEnabled)
                    VoltTrace.TraceEvent(
                                          TraceEventType.Information
                                        , VoltTraceEventType.DrainingStarted
                                        , Resources.TraceConnectionDrainingStarted
                                        , this.ServerHostId
                                        , this.ConnectionId
                                        );

                // Cache current status to resore later.
                ConnectionStatus previousStatus = this.Status;

                //Change status and perform draining.
                this.Status = ConnectionStatus.Draining;
                while (this.ExecutionCache.Size > 0)
                    Thread.Sleep(100);
                // Restore status.
                this.Status = previousStatus;

                // Trace as needed.
                if (this.Settings_TraceEnabled)
                    VoltTrace.TraceEvent(
                                          TraceEventType.Information
                                        , VoltTraceEventType.DrainingCompleted
                                        , Resources.TraceConnectionDrainingCompleted
                                        , this.ServerHostId
                                        , this.ConnectionId
                                        );
            }
            return this;
        }

        /// <summary>
        /// Closes the connection, optionally waiting for all pending responses to have been received.
        /// </summary>
        /// <param name="drain">True/false: whether to wait for all responses to be processed before terminating
        /// the connection.</param>
        /// <returns>A reference to the current connection instance for action chaining.</returns>
        public override VoltConnection Close(bool drain)
        {
            // Synchronize access.
            lock (this.SyncRoot)
            {
                // Validate connection status.
                if (this.Status != ConnectionStatus.Connected)
                    throw new InvalidOperationException(string.Format(Resources.InvalidConnectionStatus, this.Status));

                // Trace as needed.
                if (this.Settings_TraceEnabled)
                    VoltTrace.TraceEvent(
                                          TraceEventType.Information
                                        , VoltTraceEventType.ConnectionClosing
                                        , Resources.TraceConnectionClosing
                                        , this.ServerHostId
                                        , this.ConnectionId
                                        );

                // Set status
                this.Status = ConnectionStatus.Closing;

                // Drain first if requested.
                if (drain)
                    this.Drain();

                // Terminate the connection (trace event for final closure will be posted there).
                this.Terminate();
            }
            return this;
        }


        /// <summary>
        /// Track execution startup statistics.
        /// </summary>
        /// <param name="procedure">Name of the procedure whose execution just started.</param>
        /// <param name="bytesSent">Size of the request, in bytes, for network I/O tracking.</param>
        private void TrackStatisticsOpenRequest(string procedure, long bytesSent)
        {
            // Only valid if statistics are on!
            if (this.Settings_StatisticsEnabled)
            {

                // Track periodical procedure-specific statistics.
                Statistics stats;
                
                // Need to lock the dictionary to get/set the counter - only (not to update its internal properties).
                lock ((this.Stats as IDictionary).SyncRoot)
                {
                    if (!this.Stats.TryGetValue(procedure, out stats))
                    {
                        stats = new Statistics();
                        this.Stats.Add(procedure, stats);
                    }
                }
                // Note: Yes, performed outside the lock - all internal statements use Interlocked for perfect
                // consistency - there is no point locking the dictionary itself anymore, once the new key has been
                // added (when necessary), causing useless contention.
                stats.OpenRequest(bytesSent);

                // Track lifetime statistics
                this.LifetimeStats.OpenRequest(bytesSent);
            }
        }

        /// <summary>
        /// Track execution completion statistics.
        /// </summary>
        /// <param name="executionId">Id of the execution that completed (used to filter out responses after a
        /// statistics reset).</param>
        /// <param name="procedure">Name of the procedure for the completed execution.</param>
        /// <param name="executionDuration">Execution duration (as reported by the server).</param>
        /// <param name="status">Response status (Succes, failure, etc).</param>
        /// <param name="bytesReceived">Size of the response, in bytes, for network I/O tracking.</param>
        private void TrackStatisticsCloseRequest(
                                                  long executionId
                                                , string procedure
                                                , int executionDuration
                                                , ResponseStatus status
                                                , long bytesReceived
                                                )
        {
            // Do not track if this disabled or the transaction dates from before the last reset-and-forget call.
            if (!this.Settings_StatisticsEnabled || (executionId < this.StatisticsResetExecutionId))
                return;

            // Track periodical procedure-specific statistics.
            Statistics stats;
            // Need to lock the dictionary to get/set the counter - only (not to update its internal properties).
            lock ((this.Stats as IDictionary).SyncRoot)
            {
                if (!this.Stats.TryGetValue(procedure, out stats))
                {
                    stats = new Statistics();
                    this.Stats.Add(procedure, stats);
                }
            }
            // Note: Yes, performed outside the lock - all internal statements use Interlocked for perfect
            // consistency - there is no point locking the dictionary itself anymore, once the new key has been
            // added (when necessary), causing useless contention.
            stats.CloseRequest(executionDuration, status, bytesReceived);

            // Track lifetime statistics.
            this.LifetimeStats.CloseRequest(executionDuration, status, bytesReceived);
        }

        /// <summary>
        /// Return statistics for a given procedure. (null if there are no available statistics).
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <param name="procedure">The procedure for which you want statistics.</param>
        /// <returns>A Statistics snapshot object.</returns>
        internal override Statistics GetStatistics(StatisticsSnapshot command, string procedure)
        {
            lock ((this.Stats as IDictionary).SyncRoot)
            {
                Statistics result = null;
                if (this.Stats.TryGetValue(procedure, out result))
                {
                    result = result.Snapshot();
                    if (command != StatisticsSnapshot.SnapshotOnly)
                        this.Stats.Remove(procedure);
                }

                return result;
            }
        }

        /// <summary>
        /// Return statistics for each procedure.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A dictionary containing Statistics snapshot objects for each procedure that was called on the
        /// connection.</returns>
        internal override Dictionary<string, Statistics> GetStatistics(StatisticsSnapshot command)
        {
            lock ((this.Stats as IDictionary).SyncRoot)
            {
                Dictionary<string, Statistics> result = this.Stats.ToDictionary(s => s.Key, s => s.Value.Snapshot(), StringComparer.OrdinalIgnoreCase);
                if (command != StatisticsSnapshot.SnapshotOnly)
                    this.Stats = new Dictionary<string, Statistics>(StringComparer.OrdinalIgnoreCase);
                if (command == StatisticsSnapshot.SnapshotAndResetWithIgnorePendingExecutions)
                    this.StatisticsResetExecutionId = this.ExecutionId;

                return result;
            }
        }

        /// <summary>
        /// Return a statistics summary that aggregates all statistics for every called procedure.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A Statistics snapshot object representing a summary of all available statistics for the
        /// connection.</returns>
        internal override Statistics GetStatisticsSummary(StatisticsSnapshot command)
        {
            lock ((this.Stats as IDictionary).SyncRoot)
            {
                Statistics result = this.Stats.Select(i => i.Value).Summarize();
                if (command != StatisticsSnapshot.SnapshotOnly)
                {
                    this.Stats = new Dictionary<string, Statistics>(StringComparer.OrdinalIgnoreCase);
                    if (command == StatisticsSnapshot.SnapshotAndResetWithIgnorePendingExecutions)
                        this.StatisticsResetExecutionId = this.ExecutionId;
                }
                return result;
            }
        }

        /// <summary>
        /// Return lifetime statistics for the connection (global summary - not by procedure).
        /// Those statistics are aggregated throughout the life of the connection and not affected by resets.
        /// </summary>
        /// <returns>Global lifetime statistics summary for the connection.</returns>
        internal override Statistics GetLifetimeStatistics()
        {
            // Snapshot current lifetime statistics and summarize with past lifetime statistics, if any (for
            // connections that have been opened and closed multiple times).
            return this.LifetimeStats.Snapshot().SummarizeWith(this.PastLifetimeStats);
        }

        /// <summary>
        /// Resets the internal statistics for the connection.  Optionally, you can decide to ignore pending executions
        /// that have not yet completed at the time this method is called.
        /// There are pros and cons to each choice:
        ///  - If you do ignore pending executions, you have no visibility on their latency statistics: when the calls
        ///    finally return, this information is simply ignored.
        ///  - If to do not ignore pending executions, you get biased transaction numbers: increased "In" count and TPS
        ///    throughput, and this possibly a positive execution balance on your next snapshot.
        /// </summary>
        /// <param name="ignorePendingExecutions">Whether pending executions should be ignored when they return after
        /// the reset.</param>
        internal override void ResetStatistics(bool ignorePendingExecutions)
        {
            // Wipe out internal dictionary.
            lock ((this.Stats as IDictionary).SyncRoot)
                this.Stats = new Dictionary<string, Statistics>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Return statistics for a given procedure, grouped by node. (an empty dictionary (not null!) if no node has
        /// any statistics for the given procedure).
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters (for this
        /// procedure only).</param>
        /// <param name="procedure">The procedure for which you want statistics.</param>
        /// <returns>A Statistics snapshot object.</returns>
        internal override Dictionary<IPEndPoint, Statistics> GetStatisticsByNode(
                                                                                StatisticsSnapshot command
                                                                              , string procedure
                                                                              )
        {
            Statistics result = this.GetStatistics(command, procedure);
            if (result == null)
                return new Dictionary<IPEndPoint, Statistics>();
            return new Dictionary<IPEndPoint, Statistics>() { { this.IPEndPoint, result } };
        }

        /// <summary>
        /// Return statistics for each procedure, grouped by node
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters</param>
        /// <returns>A dictionary containing Statistics snapshot objects for each procedure that was called on the
        /// connection.</returns>
        internal override Dictionary<IPEndPoint, Dictionary<string, Statistics>>
            GetStatisticsByNode(StatisticsSnapshot command)
        {
            return new Dictionary<IPEndPoint, Dictionary<string, Statistics>>()
                   {
                       { this.IPEndPoint, this.GetStatistics(command) }
                   };
        }

        /// <summary>
        /// Return a statistics summary that aggregates all statistics for every called procedure, and accross all
        /// connections.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters</param>
        /// <returns>A Statistics snapshot object representing a summary of all available statistics for the
        /// connection.</returns>
        internal override Dictionary<IPEndPoint, Statistics> GetStatisticsSummaryByNode(StatisticsSnapshot command)
        {
            return new Dictionary<IPEndPoint, Statistics>()
                   {
                       { this.IPEndPoint, this.GetStatisticsSummary(command) }
                   };
        }

        /// <summary>
        /// Return lifetime statistics for the connection (global summary - not by procedure), grouped by node.
        /// Those statistics are aggregated throughout the life of the connection and not affected by resets.
        /// </summary>
        /// <returns>Global lifetime statistics summary for the connection.</returns>
        internal override Dictionary<IPEndPoint, Statistics> GetLifetimeStatisticsByNode()
        {
            return new Dictionary<IPEndPoint, Statistics>() { { this.IPEndPoint, this.GetLifetimeStatistics() } };
        }

    }
}