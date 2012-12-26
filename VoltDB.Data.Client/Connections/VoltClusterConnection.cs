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
    /// Provides the core functionalities for a VoltDB connection to a list of hosts.
    /// </summary>
    public sealed class VoltClusterConnection : VoltConnection
    {
        /// <summary>
        /// Internal pool of node connections
        /// </summary>
        internal readonly List<VoltNodeConnection> ConnectionPool = new List<VoltNodeConnection>();

        /// <summary>
        /// Count of connections - we need this because of the additional de-referencing layer used for resilience
        /// support.
        /// </summary>
        internal long ConnectionCount = 0;

        /// <summary>
        /// Indexes of the connections that are alive in the connection pool: in order to keep statistics, dead
        /// connections are left in, but obviously we don't want to send traffic there.
        /// </summary>
        private int[] LiveConnectionIndexList;

        /// <summary>
        /// Execution Id - not really used for a cluster connection, other than to provide support for load-balancing.
        /// </summary>
        private long ExecutionId = 0;

        /// <summary>
        /// Size of batches for load balancing, by default 100: the first 100 executions are sent to the first node,
        /// the next 100 to the second, etc.  Works a bit better than changing with every execution and still prevents
        /// fire-hosing.  This is a configurable connection setting.
        /// </summary>
        private long LoadBalancingBatchSize;

        /// <summary>
        /// Internal list of exceptions gathered while trying to open sub-connections.
        /// </summary>
        private List<Exception> ConnectionExceptionList;

        /// <summary>
        /// Internal count of connection failures (only those that matter: cluster inconsistencies or when in
        /// "ConnectToAllOrNone" connection mode.
        /// </summary>
        private long ConnectionExceptionCount;

        /// <summary>
        /// Creates a new cluster connection (load-balanced multi-node connection).
        /// Internal usage only (Factory pattern on the base class).
        /// </summary>
        /// <param name="settings">Connection settings to use to connect to the cluster.</param>
        /// <param name="callbackExecutor">Reference to the executor that will execute all callbacks.</param>
        internal VoltClusterConnection(ConnectionSettings settings, CallbackExecutor callbackExecutor)
            : base(settings)
        {
            // Faster than looking up the settings property all the time
            this.LoadBalancingBatchSize = this.Settings.LoadBalancingBatchSize;
            // Reference to the global callback executor
            this.CallbackExecutor = callbackExecutor;
        }

        /// <summary>
        /// Delegate used for async connection open operations.
        /// </summary>
        /// <param name="settings"></param>
        private delegate void OpenChildConnectionDelegate(ConnectionSettings settings);

        /// <summary>
        /// Async sub-connection opening - open a child connection and add to the pool when successful.
        /// </summary>
        /// <param name="settings">Settings to use for the child connection to open.</param>
        private void OpenChildConnection(ConnectionSettings settings)
        {
            // Return immediately if there was a terminal connection failure on any of the parallel attempts.
            if (Interlocked.Read(ref this.ConnectionExceptionCount) > 0)
                return;

            try
            {
                // Try to open the node connection.
                VoltNodeConnection connection = new VoltNodeConnection(settings, this.CallbackExecutor);
                connection.Open();

                // Lock the connection pool for validation and addition.
                lock ((this.ConnectionPool as IList).SyncRoot)
                {
                    // Validate new node against previous connections: we will refuse any connection to an inconsistent
                    // cluster (different versions, different application, etc).
                    if (this.ConnectionPool.Count > 0)
                    {
                        if (
                            (this.BuildString != connection.BuildString)
                            || (!this.LeaderIPEndPoint.Equals(connection.LeaderIPEndPoint))
                            || (this.ClusterStartTimeStamp != connection.ClusterStartTimeStamp)
                           )
                        {
                            // This node doesn't belong to the cluster - terminate the connection and throw out!
                            try
                            {
                                connection.Terminate();
                            }
                            catch {}

                            throw new VoltClusterConnectionException(
                                  Resources.InconsistentClusterTarget

                                , string.Join("\r\n"
                                    , this.ConnectionPool.Select(i =>
                                                    string.Format(
                                                                   Resources.InconsistentClusterTargetListing
                                                                 , i.ServerHostId
                                                                 , i.ConnectionId
                                                                 , i.IPEndPoint
                                                                 , i.LeaderIPEndPoint
                                                                 , i.ClusterStartTimeStamp
                                                                 , i.BuildString
                                                                 )
                                        ).ToArray()
                                             )

                                , string.Format(
                                                 Resources.InconsistentClusterTargetListing
                                               , connection.ServerHostId
                                               , connection.ConnectionId
                                               , connection.IPEndPoint
                                               , connection.LeaderIPEndPoint
                                               , connection.ClusterStartTimeStamp
                                               , connection.BuildString
                                               )

                                                                    );
                        }
                    }
                    else
                    {
                        // Grab cluster details after the first connection to compare future connections against.
                        this.BuildString = connection.BuildString;
                        this.ClusterStartTimeStamp = connection.ClusterStartTimeStamp;
                        this.LeaderIPEndPoint = connection.LeaderIPEndPoint;
                    }
                    // Connection is valid, add it to the pool
                    this.ConnectionPool.Add(connection);
                }
            }
            catch (Exception x)
            {
                // Flag terminal exceptions: in this case we do NOT want to proceed no matter if a single node ever
                // replied.
                if ((x is VoltClusterConnectionException) || this.Settings.ConnectToAllOrNone)
                    Interlocked.Increment(ref this.ConnectionExceptionCount);

                // Add the exception to the list for later reporting.
                lock ((this.ConnectionExceptionList as IList).SyncRoot)
                    this.ConnectionExceptionList.Add(x);
            }
        }

        /// <summary>
        /// Provides a background thread to reconnect failed node connections.
        /// </summary>
        /// <param name="state">The connection to re-open</param>
        private void ReconnectChildConnection(object state)
        {
            // Get connection object from the state object passed by the thread pool.
            VoltNodeConnection connection = state as VoltNodeConnection;

            // Try to reconnect until the connection is actively closed by the user or terminated (when all child
            // connections died and none could be re-opened)
            while (this.Status == ConnectionStatus.Connected)
            {
                try
                {
                    // Attempt to open the connection
                    connection.Open();

                    // If somebody tried to swap out the node from under us to start a different cluster, return
                    // immediately: we will not reconnect there!
                    if (
                        (this.BuildString != connection.BuildString)
                        || (!this.LeaderIPEndPoint.Equals(connection.LeaderIPEndPoint))
                        || (this.ClusterStartTimeStamp != connection.ClusterStartTimeStamp)
                       )
                    {
                        connection.Terminate();
                        return;
                    }

                    // Refresh pool status to re-enlist the connection into active duty.
                    this.RefreshConnectionPoolStatus();

                    // End the thread: connection successfully re-added to the pool.
                    return;
                }
                catch
                {
                    // Sleep a while to give the node time to rejoin if needed.
                    Thread.Sleep(this.Settings.ConnectionTimeout);
                }
            }
        }

        /// <summary>
        /// Open the cluster connections: the provided settings are analyzed to deploy a full list of IP Endpoints
        /// (IP:port) of nodes to which we should connect.  Child connections are then opened in parallel to all those
        /// endpoints.  Depending on your settings and the cluster configuration, a failure can mean the "Open" method
        /// will fail (and throw details about the errors encountered), or move forth (all connections opened, of
        /// course; but also in cases where the configuration indicates that a partial connection is valid, the
        /// connection will be allowed to stay available).
        /// </summary>
        /// <returns>A reference to the current connection instance for action chaining.</returns>
        public override VoltConnection Open()
        {
            lock (this.SyncRoot)
            {
                // Validate connection status.
                if (this.Status != ConnectionStatus.Closed)
                    throw new InvalidOperationException(string.Format(Resources.InvalidConnectionStatus, this.Status));

                // Set status to "connecting".
                this.Status = ConnectionStatus.Connecting;

                try
                {
                    // Deploy the connection settings into individual IPEndPoint connections.
                    ConnectionSettings[] clusterSettings = this.Settings.ClusterConnectionSettings;

                    // Create delegate for invokation.
                    OpenChildConnectionDelegate handler = this.OpenChildConnection;

                    // Empty the connection list if this is happening after a close event.
                    this.ConnectionPool.Clear();
                    this.ConnectionCount = 0;

                    // Reset connection monitoring flags.
                    this.ConnectionExceptionList = new List<Exception>();
                    this.ConnectionExceptionCount = 0;

                    // Prepare async operation - we open connections in parallel (as much as the system will allow,
                    // that is by blocks of 64 maximum - the size limit for a WaitHandle[] that WaitAll will accept).
                    for (int batchStart = 0; batchStart < clusterSettings.Length; batchStart += 64)
                    {
                        int batchLength = Math.Min(clusterSettings.Length - batchStart, 64);

                        // Create a block of wait handles.
                        WaitHandle[] waitHandles = new WaitHandle[batchLength];

                        // Kick off connection.Open requests - finalization will be processed in parallel as well.
                        // We will check the results after al threads have returned or a timeout.
                        // Now technically, since we're doing this in batches of 64, if we have 65 connections, it
                        // means we will wait for the first 64 connections to be open (or fail to open) before trying
                        // out for the 65th.  Not ideal, but probably not a case to seriously worry about.
                        for (int sourceIndex = batchStart; sourceIndex < batchLength + batchStart; sourceIndex++)
                        {
                            IAsyncResult ar = handler.BeginInvoke(clusterSettings[sourceIndex], null, handler);
                            waitHandles[sourceIndex - batchStart] = ar.AsyncWaitHandle;
                        }

                        // Now wait for all connections to complete before proceeding to check the results.
                        if (!WaitHandle.WaitAll(
                                                 waitHandles
                                               , this.Settings.ConnectionTimeout == Timeout.Infinite
                                                 ? Timeout.Infinite
                                                 : this.Settings.ConnectionTimeout * batchLength)
                                               )
                        {
                            // At least one of the connections timeout.  If we're in "ConnectToAllOrNone" mode, trigger
                            // termination.
                            if (this.Settings.ConnectToAllOrNone)
                                lock ((this.ConnectionPool as IList).SyncRoot)
                                    throw new VoltClusterConnectionException(
                                          Resources.ClusterConnectionTimeout
                                        , string.Join("\r\n"
                                            , clusterSettings.Select(r =>
                                                string.Format(
                                                      Resources.ClusterConnectionSummaryRow
                                                    , r.DefaultIPEndPoint
                                                    , this.ConnectionPool
                                                        .Exists(c => c.IPEndPoint.Equals(r.DefaultIPEndPoint))
                                                             )
                                                                    ).ToArray()
                                                     )
                                                                            );
                        }

                        // Check the results.
                        if (this.ConnectionExceptionCount > 0)
                        {
                            // Aggregate all the exceptions to feed to the user.
                            lock ((this.ConnectionExceptionList as IList).SyncRoot)
                                throw new VoltClusterConnectionException(
                                      Resources.ClusterConnectionFailure
                                    , string.Join("\r\n\r\n"
                                        , this.ConnectionExceptionList.Select(x => x.ToString()).ToArray()
                                                 )
                                                                        );
                        }
                    }

                    // If we get here, all connections were successfully open (or in the case we're not in "All or
                    // nothing", some did - or did they?  Make sure we have at least one!
                    if (this.ConnectionPool.Count < 1)
                        throw new VoltClusterConnectionException(Resources.ClusterConnectionFailureNoSingleHost
                          , string.Join("\r\n\r\n", this.ConnectionExceptionList.Select(x => x.ToString()).ToArray()));

                    // Record connection count for multiplexing.
                    this.ConnectionCount = this.ConnectionPool.Count;

                    // Initialize map of live connections - at the beginning, obviously, they are all alive.
                    // This array contains the list of Indexes of the live connections.  For instance, if connection
                    // #2 where to die on a cluster of 3 connections , the array would be modified to: [1,3].
                    // Multiplexing will then load balance between those two, while the dead connection is left in the
                    // pool for possible recovery and statistical history support.
                    this.LiveConnectionIndexList = new int[this.ConnectionCount];
                    for (int i = 0; i < this.ConnectionCount; i++)
                        this.LiveConnectionIndexList[i] = i;


                    // Looking good... Set status to "connected".
                    this.Status = ConnectionStatus.Connected;

                    // Trace as needed.
                    if (this.Settings.TraceEnabled)
                        VoltTrace.TraceEvent(
                                              TraceEventType.Information
                                            , VoltTraceEventType.ConnectionOpened
                                            , Resources.TraceClusterConnectionOpened
                                            , this.LeaderIPEndPoint
                                            , this.ClusterStartTimeStamp
                                            , this.BuildString
                                            , string.Join("\r\n"
                                                , clusterSettings.Select(r =>
                                                    string.Format(Resources.ClusterConnectionSummaryRow
                                                        , r.DefaultIPEndPoint
                                                        , this.ConnectionPool
                                                            .Exists(c => c.IPEndPoint.Equals(r.DefaultIPEndPoint))
                                                        )
                                                                        ).ToArray()
                                                         )
                                            );
                }
                catch (Exception x)
                {
                    try
                    {
                        // In case of failure, terminate everything immediately (will correct status)
                        try
                        {
                            // No waiting, no nothing: it's over - Terminate sub-connections (Terminate swallows any
                            // exception, so this is safe without try/catch).
                            foreach (VoltNodeConnection connection in this.ConnectionPool)
                                connection.Terminate();
                            this.ConnectionPool.Clear();

                            // Trace as needed
                            if (this.Settings.TraceEnabled)
                                VoltTrace.TraceEvent(
                                                      TraceEventType.Information
                                                    , VoltTraceEventType.ConnectionClosed
                                                    , Resources.TraceClusterConnectionClosed
                                                    );
                        }
                        finally
                        {
                            this.ConnectionPool.Clear();
                            this.Status = ConnectionStatus.Closed;
                        }
                    }
                    catch { }
                    // Re-throw exception, wrapping if needed.
                    if (x is VoltException)
                        throw new VoltConnectionException(Resources.ClusterConnectionUnknownFailure, x);
                    else
                        throw x;
                }
            }
            return this;
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
                if (this.Settings.TraceEnabled)
                    VoltTrace.TraceEvent(
                                          TraceEventType.Information
                                        , VoltTraceEventType.DrainingStarted
                                        , Resources.TraceClusterConnectionDrainingStarted
                                        );

                // Cache current status to resore later.
                ConnectionStatus previousStatus = this.Status;

                //Change status and perform draining.
                this.Status = ConnectionStatus.Draining;


                // In concept, this should really be parallelized but who really cares?  The cluster will refuse all
                // connections, so that by not submitting more work we ARE draining all the children anyways.
                // Thus the wait will still be = Max-Drain-Duration-for-slowest-host.
                foreach (VoltNodeConnection connection in this.ConnectionPool)
                {
                    // Sub-connection might have died, in which case there won't be anything to do, but we need to make
                    // sure we won't die on the exception it will raise!
                    try
                    {
                        connection.Drain();
                    }
                    catch { }
                }

                // Restore status.
                this.Status = previousStatus;

                // Trace as needed.
                if (this.Settings.TraceEnabled)
                    VoltTrace.TraceEvent(
                                          TraceEventType.Information
                                        , VoltTraceEventType.DrainingCompleted
                                        , Resources.TraceClusterConnectionDrainingCompleted
                                        );
            }
            return this;
        }

        /// <summary>
        /// Closes the connection, optionally waiting for all pending responses to have been received.
        /// </summary>
        /// <param name="drain">True/false: whether to wait for all responses to be processed before terminating the
        /// connection.</param>
        /// <returns>A reference to the current connection instance for action chaining.</returns>
        public override VoltConnection Close(bool drain)
        {
            // Synchronize access.
            lock (this.SyncRoot)
            {
                try
                {
                    // Validate connection status.
                    if (this.Status != ConnectionStatus.Connected)
                        throw new InvalidOperationException(
                                                             string.Format(
                                                                            Resources.InvalidConnectionStatus
                                                                          , this.Status
                                                                          )
                                                           );

                    // Trace as needed.
                    if (this.Settings.TraceEnabled)
                        VoltTrace.TraceEvent(
                                              TraceEventType.Information
                                            , VoltTraceEventType.ConnectionClosing
                                            , Resources.TraceClusterConnectionClosing
                                            );

                    // Set status.
                    this.Status = ConnectionStatus.Closing;

                    // Drain first if requested (we do not call the child's CLose(drain) method because we want to make
                    // sure tracing at the top level is consistent).
                    if (drain)
                        this.Drain();

                    // Close connections
                    foreach (VoltNodeConnection connection in this.ConnectionPool)
                    {
                        // Sub-connection might have died, in which case there won't be anything to do, but we need to
                        // make sure we won't die on the exception it will raise!
                        try
                        {
                            connection.Close(false);
                        }
                        catch { }
                    }

                    // Stop the callback executor's threads.  This will also drain callback execution, ensuring every
                    // triggered callback actually gets executed before the executor is shut down.
                    this.CallbackExecutor.Stop();

                    // Trace as needed.
                    if (this.Settings.TraceEnabled)
                        VoltTrace.TraceEvent(
                                              TraceEventType.Information
                                            , VoltTraceEventType.ConnectionClosed
                                            , Resources.TraceClusterConnectionClosed
                                            );

                }
                finally
                {
                    // Mark connection as closed
                    this.Status = ConnectionStatus.Closed;
                }
            }
            return this;
        }

        /// <summary>
        /// Return statistics for a given procedure, summarized for all connections. (null if there are no available
        /// statistics).
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters (for this
        /// procedure only).</param>
        /// <param name="procedure">The procedure for which you want statistics.</param>
        /// <returns>A Statistics snapshot object.</returns>
        internal override Statistics GetStatistics(StatisticsSnapshot command, string procedure)
        {
            // Summarize from each node connection.
            return this.ConnectionPool.Select(c => c.GetStatistics(command, procedure)).Summarize();
        }

        /// <summary>
        /// Return statistics for each procedure, summarized for all connections.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A dictionary containing Statistics snapshot objects for each procedure that was called on the
        /// connection.</returns>
        internal override Dictionary<string, Statistics> GetStatistics(StatisticsSnapshot command)
        {
            // Grab from every node then regroup by procedure name and summarize for all nodes by procedure
            return this.ConnectionPool.SelectMany(c => c.GetStatistics(command))
                                      .GroupBy(r => r.Key, r => r.Value)
                                      .ToDictionary(g => g.Key, g => g.Summarize(), StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Return a statistics summary that aggregates all statistics for every called procedure, and accross all
        /// connections.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A Statistics snapshot object representing a summary of all available statistics for the
        /// connection.</returns>
        internal override Statistics GetStatisticsSummary(StatisticsSnapshot command)
        {
            // Summarize from each node connection
            return this.ConnectionPool.Select(c => c.GetStatisticsSummary(command)).Summarize();
        }

        /// <summary>
        /// Return lifetime statistics for the connection (global summary - not by procedure), summarized for all
        /// connections.  Those statistics are aggregated throughout the life of the connection and not affected by
        /// resets.
        /// </summary>
        /// <returns>Global lifetime statistics summary for the connection.</returns>
        internal override Statistics GetLifetimeStatistics()
        {
            // Summarize from each node connection
            return this.ConnectionPool.Select(c => c.GetLifetimeStatistics()).Summarize();
        }

        /// <summary>
        /// Resets the internal statistics for all connections.  Optionally, you can decide to ignore pending
        /// executions that have not yet completed at the time this method is called.
        /// There are pros and cons to each choice:
        ///  - If you do ignore pending executions, you have no visibility on their latency statistics: when the calls
        ///    finally return, this information is simply ignored.
        ///  - If to do not ignore pending executions, you get biased transaction numbers: increased "In" count and TPS
        ///    throughput, and this possibly a positive execution balance on your next snapshot.
        /// </summary>
        /// <param name="ignorePendingExecutions">Whether pending executions should be ignored when they return after
        /// the reset.</param>
        /// <returns>A reference to the current connection instance for action chaining.</returns>
        internal override void ResetStatistics(bool ignorePendingExecutions)
        {
            // Issue reset to each node
            this.ConnectionPool.ForEach(c => c.ResetStatistics(ignorePendingExecutions));
        }

        /// <summary>
        /// Return statistics for a given procedure, grouped by node. (an empty dictionary (not null!) if no node has
        /// any statistics for the given procedure).
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters (for this
        /// procedure only).</param>
        /// <param name="procedure">The procedure for which you want statistics</param>
        /// <returns>A Statistics snapshot object</returns>
        internal override Dictionary<IPEndPoint, Statistics> GetStatisticsByNode(
                                                                                StatisticsSnapshot command
                                                                              , string procedure
                                                                              )
        {
            // Summarize from each node connection.
            return this.ConnectionPool.ToDictionary(c => c.IPEndPoint, c => c.GetStatistics(command, procedure))
                                      .Where(r => r.Value != null)
                                      .ToDictionary(r => r.Key, r => r.Value);
        }

        /// <summary>
        /// Return statistics for each procedure, grouped by node.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A dictionary containing Statistics snapshot objects for each procedure that was called on the
        /// connection.</returns>
        internal override Dictionary<IPEndPoint, Dictionary<string, Statistics>>
            GetStatisticsByNode(StatisticsSnapshot command)
        {
            // Simply gather everything together
            return this.ConnectionPool.ToDictionary(c => c.IPEndPoint, c => c.GetStatistics(command));
        }

        /// <summary>
        /// Return a statistics summary that aggregates all statistics for every called procedure, and accross all
        /// connections.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A Statistics snapshot object representing a summary of all available statistics for the
        /// connection.</returns>
        internal override Dictionary<IPEndPoint, Statistics> GetStatisticsSummaryByNode(StatisticsSnapshot command)
        {
            // Summarize from each node connection
            return this.ConnectionPool.ToDictionary(c => c.IPEndPoint, c => c.GetStatisticsSummary(command));
        }

        /// <summary>
        /// Return lifetime statistics for the connection (global summary - not by procedure), grouped by node.
        /// Those statistics are aggregated throughout the life of the connection and not affected by resets
        /// </summary>
        /// <returns>Global lifetime statistics summary for the connection.</returns>
        internal override Dictionary<IPEndPoint, Statistics> GetLifetimeStatisticsByNode()
        {
            // Summarize from each node connection
            return this.ConnectionPool.ToDictionary(c => c.IPEndPoint, c => c.GetLifetimeStatistics());
        }

        /// <summary>
        /// Synchronously execute a procedure against the VoltDB server and block execution of the calling thread.
        /// </summary>
        /// <typeparam name="T">Data type of the expected response result for the call (Table[], Table,
        /// SingleRowTable[], SingleRowTable, T[][], T[] or T, with T one of the supported value types).</typeparam>
        /// <param name="timeout">Timeout value (overrides connection settings DefaultCommandTimeout). Use
        /// Timeout.Infinite or -1 for infinite timeout.</param>
        /// <param name="procedure">The name of the procedure to call.</param>
        /// <param name="procedureUtf8">The UTF-8 bytes of the procedure name.</param>
        /// <param name="parameters">List of parameters to pass to the procedure.</param>
        /// <returns>The reponse from the server, containing (if available) the result of the procedure execution
        /// (check .IsError prior to using the result).</returns>
        /// <remarks>Synchronous operations will limit the throughput of your aplication, as each of your request waits
        /// for completion before submitting a new call.  For maximum parallelization and throughput, asynchronous
        /// calls should be made (see BeginExecute).</remarks>
        protected internal override Response<T> Execute<T>(int timeout, string procedure, byte[] procedureUtf8, params object[] parameters)
        {
            // Validate connection status.
            if (this.Status != ConnectionStatus.Connected)
                throw new InvalidOperationException(string.Format(Resources.InvalidConnectionStatus, this.Status));

            // Grab the index of the connection we'll use - this will lock onto the connection for a moment to ensure
            // we get a valid connection, and throw an exception if none can be returned (connection closed).
            int index = this.GetConnectionIndex();

            // Call underlying executor.
            return this.ConnectionPool[index].Execute<T>(timeout, procedure, procedureUtf8, parameters);

            // The previous call will not throw an exception (except if truly justified: bad argument or disaster)
            // What might happen however is that the connection failed and was terminated instantly: all pending
            // executions will trigger into client abort.
            // The next execution request will hit a dead connection, which will trigger a re-evaluation of the pool
            // status.
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
        protected internal override AsyncResponse<T> BeginExecute<T>(
                                                                     ExecuteAsyncCallback<T> callback
                                                                   , object state
                                                                   , int timeout
                                                                   , string procedure
                                                                   , byte[] procedureUtf8
                                                                   , params object[] parameters
                                                                   )
        {
            // Grab the index of the connection we'll use - this will lock onto the connection for a moment to ensure
            // we get a valid connection, and throw an exception if none can be returned (connection closed).
            int index = this.GetConnectionIndex();

            // Call underlying executor.
            return this.ConnectionPool[index].BeginExecute<T>(callback, state, timeout, procedure, procedureUtf8, parameters);

            // The previous call will not throw an exception (except if truly justified: bad argument or disaster)
            // What might happen however is that the connection failed and was terminated instantly: all pending
            // executions will trigger into client abort.
            // The next execution request will hit a dead connection, which will trigger a re-evaluation of the pool
            // status.
        }

        /// <summary>
        /// Return the index of the live connection to which the next query should be posted.
        /// </summary>
        /// <returns>Index of the connection in the pool to send the next query to.</returns>
        private int GetConnectionIndex()
        {
            // Get a lock to ensure we're getting a valid connection
            lock (this.SyncRoot)
            {
                int index = -1;
                while (index == -1 && this.Status == ConnectionStatus.Connected)
                {
                    index = this.LiveConnectionIndexList[(int)(
                                                                  (
                                                                    Interlocked.Increment(ref this.ExecutionId)
                                                                  / this.LoadBalancingBatchSize
                                                                  )
                                                              % this.ConnectionCount
                                                              )];

                    // Ensure sub-connection is valid.
                    if (this.ConnectionPool[index].Status != ConnectionStatus.Connected)
                    {
                        // We lost someone - refresh the pool status.
                        RefreshConnectionPoolStatus();

                        // Push a background thread to attempt reconnection
                        ThreadPool.QueueUserWorkItem(ReconnectChildConnection, this.ConnectionPool[index]);

                        // Can't pick up this connection.
                        index = -1;
                    }
                }
                // When we get here, we either have a valid index, or the connection has been shut down.

                // Validate connection status.
                if (this.Status != ConnectionStatus.Connected)
                    throw new InvalidOperationException(string.Format(Resources.InvalidConnectionStatus, this.Status));

                // Return the index.
                return index;
            }
        }

        /// <summary>
        /// Refresh the status of the connection pool by validating every connection and pulling out any connection
        /// that isn't in "Opened" status.
        /// </summary>
        private void RefreshConnectionPoolStatus()
        {
            // Lock out the connection while the rebuilt is in progress.
            lock (this.SyncRoot)
            {
                // Go through each connection and check the status to rebuild the array as needed.
                List<int> newlist = new List<int>();
                for (int i = 0; i < this.ConnectionPool.Count; i++)
                    if (this.ConnectionPool[i].Status == ConnectionStatus.Connected)
                        newlist.Add(i);

                // If all the connections died, we are dead as well, we ensure the connection status is set
                // properly and return immediately.
                if (newlist.Count == 0)
                {
                    try { this.Close(false); }
                    catch { }
                    return;
                }

                // Otherwise, we're still in business.
                this.LiveConnectionIndexList = newlist.ToArray();
                this.ConnectionCount = this.LiveConnectionIndexList.Length;
            }
        }

    }
}