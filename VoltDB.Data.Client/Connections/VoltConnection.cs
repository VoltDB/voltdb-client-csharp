/*

 This file is part of VoltDB.
 Copyright (C) 2008-2011 VoltDB Inc.

 Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
 rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
 persons to whom the Software is furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
 Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS BE
 LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Abstract base class for single-host VoltConnections and Pooled VoltClusterConnetions.
    /// </summary>
    public abstract partial class VoltConnection : IDisposable
    {
        /// <summary>
        /// Reference to the global connection callback executor (only 1 executor per connection, regardless as to
        /// whether this is a Node or Cluster connection)
        /// </summary>
        internal CallbackExecutor CallbackExecutor;

        /// <summary>
        /// Internal synchronization rot for thread-safe operations.
        /// </summary>
        protected object SyncRoot = new object();

        /// <summary>
        /// Connection status
        /// </summary>
        public ConnectionStatus Status { get; protected set; }

        /// <summary>
        /// Date/Time at which the VoltDB cluster to which this connection points was started.
        /// </summary>
        protected internal DateTime ClusterStartTimeStamp;

        /// <summary>
        /// IP address of the leader node of the VltDB cluster to which this connection points.
        /// </summary>
        protected internal IPEndPoint LeaderIPEndPoint;

        /// <summary>
        /// Build string of the server's VoltDB engine, containing the Version details.
        /// </summary>
        protected internal string BuildString;

        /// <summary>
        /// Id of the server to which this connection is pointing (only for Node connections).
        /// </summary>
        protected internal int ServerHostId = -1;

        /// <summary>
        /// Server-assigned Id of this connection (only for Node connections).
        /// </summary>
        protected internal long ConnectionId = -1;

        /// <summary>
        /// Settigns for the connection.
        /// </summary>
        protected internal readonly ConnectionSettings Settings;

        // Perf: Thse properties are checked a lot. Doing a dictionary lookup each time slows things down.
        /// <summary>
        /// Cached value of Settings.TraceEnabled.
        /// </summary>
        protected readonly bool Settings_TraceEnabled;
        /// <summary>
        /// Cached value of Settings.StatisticsEnabled.
        /// </summary>
        protected readonly bool Settings_StatisticsEnabled;
        /// <summary>
        /// Cached value of Settings.MaxOutstandingTxns.
        /// </summary>
        protected readonly int Settings_MaxOutstandingTxns;

        /// <summary>
        /// Cached value of Settings.DefaultCommandTimeout.
        /// </summary>
        protected readonly int Settings_DefaultCommandTimeout;

        /// <summary>
        /// Reference to the SystemAccess object
        /// </summary>
        private SystemAccess _System = null;
        /// <summary>
        /// Hold a reference to a "SystemAccess" object that provides visibility to system procedure calls.
        /// </summary>
        public SystemAccess System
        {
            get
            {
                if (_System == null)
                {
                    if (this.Settings.AllowSystemCalls)
                        this._System = new SystemAccess(this);
                    else
                        throw new VoltPermissionException(Resources.SystemCallNotAllowed);
                }
                return this._System;
            }
        }

        /// <summary>
        /// Reference to the AdhocAccess object
        /// </summary>
        private AdhocAccess _Adhoc = null;
        /// <summary>
        /// Hold a reference to a "AdhocAccess" object that provides visibility to ad-hoc queries.
        /// </summary>
        public AdhocAccess Adhoc
        {
            get
            {
                if (_Adhoc == null)
                {
                    if (this.Settings.AllowAdhocQueries)
                        this._Adhoc = new AdhocAccess(this);
                    else
                        throw new VoltPermissionException(Resources.AdhocQueriesNotAllowed);
                }
                return this._Adhoc;
            }
        }

        /// <summary>
        /// Reference to the StatisticsAccess object
        /// </summary>
        private StatisticsAccess _Statistics = null;
        /// <summary>
        /// Hold a reference to a "StatisticsAccess" object that provides visibility on performance statistics
        /// (client-side tracking).
        /// </summary>
        public StatisticsAccess Statistics
        {
            get
            {
                if (_Statistics == null)
                {
                    if (this.Settings.StatisticsEnabled)
                        this._Statistics = new StatisticsAccess(this);
                    else
                        throw new VoltPermissionException(Resources.StatisticsDisabled);
                }
                return this._Statistics;
            }
        }

        /// <summary>
        /// Reference to the InfoAccess object
        /// </summary>
        private InfoAccess _Info = null;
        /// <summary>
        /// Hold a reference to a "InfoAccess" object that provides visibility on cluster/conection information.
        /// </summary>
        public InfoAccess Info
        {
            get
            {
                if (_Info == null)
                {
                    if (this.Status == ConnectionStatus.Connected)
                        this._Info = new InfoAccess(this);
                    else
                        throw new InvalidOperationException(string.Format(Resources.InvalidConnectionStatus, this.Status));
                }
                // Even if we got disconnected, the info cached the first time will still be valid (unless there's a
                // server configuration swap underneath us, but then the info will get updated, and for cluster
                // connections, this is impossible anyways, since we validate cross-connection consistency).
                return this._Info;
            }
        }

        /// <summary>
        /// Reference to the ProcedureAccess object
        /// </summary>
        private ProcedureAccess _Procedures = null;
        /// <summary>
        /// Hold a reference to a "ProcedureAccess" object that provides the ability to create strongly-typed wrappers
        /// for procedure calls.
        /// </summary>
        public ProcedureAccess Procedures
        {
            get
            {
                if (_Procedures == null)
                {
                    // No status validation: the wrappers will be valid, they just won't be able to run until the
                    // connection is opened.
                    this._Procedures = new ProcedureAccess(this);
                }
                return this._Procedures;
            }
        }

        /// <summary>
        /// Return the endpoint associated to this connection (whether connected or not).
        /// </summary>
        protected internal IPEndPoint IPEndPoint
        {
            get
            {
                return this.Settings.DefaultIPEndPoint;
            }
        }

        /// <summary>
        /// Creates a new connection.
        /// Internal usage only (Factory pattern on the base class).
        /// </summary>
        /// <param name="settings">Connection settings to use to connect to the node or cluster.</param>
        internal VoltConnection(ConnectionSettings settings)
        {
            // Only accept 'Database' for now
            if (settings.ServiceType != ServiceType.Database)
                throw new NotSupportedException(
                                                 string.Format(
                                                                Resources.UnsupportedServiceType
                                                              , settings.ServiceType.ToString()
                                                              )
                                               );

            this.Settings = settings;
            this.Settings_StatisticsEnabled = settings.StatisticsEnabled;
            this.Settings_TraceEnabled = settings.TraceEnabled;
            this.Settings_MaxOutstandingTxns = settings.MaxOutstandingTxns;
            this.Settings_DefaultCommandTimeout = settings.DefaultCommandTimeout;
            this.Status = ConnectionStatus.Closed;
        }

        /// <summary>
        /// Open the connection.
        /// </summary>
        /// <returns>A reference to the current connection instance for action chaining.</returns>
        public abstract VoltConnection Open();

        /// <summary>
        /// Blocks the calling thread until the queue of pending responses is fully exhausted (all responses received
        /// by the server).
        /// </summary>
        /// <returns>A reference to the current connection instance for action chaining.</returns>
        public abstract VoltConnection Drain();

        /// <summary>
        /// Close the connection after it has been fully drained (all pending responses executed and returned).
        /// </summary>
        /// <returns>A reference to the current connection instance for action chaining.</returns>
        public VoltConnection Close() { this.Close(true); return this; }

        /// <summary>
        /// Closes the connection, optionally waiting for all pending responses to have been received.
        /// </summary>
        /// <param name="drain">True/false: whether to wait for all responses to be processed before terminating the
        /// connection.</param>
        /// <returns>A reference to the current connection instance for action chaining.</returns>
        public abstract VoltConnection Close(bool drain);

        /// <summary>
        /// Return statistics for a given procedure. (null if there are no available statistics).
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <param name="procedure">The procedure for which you want statistics.</param>
        /// <returns>A Statistics snapshot object.</returns>
        internal abstract Statistics GetStatistics(StatisticsSnapshot command, string procedure);

        /// <summary>
        /// Return statistics for each procedure.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A dictionary containing Statistics snapshot objects for each procedure that was called on the
        /// connection.</returns>
        internal abstract Dictionary<string, Statistics> GetStatistics(StatisticsSnapshot command);

        /// <summary>
        /// Return a statistics summary that aggregates all statistics for every called procedure.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A Statistics snapshot object representing a summary of all available statistics for the
        /// connection.</returns>
        internal abstract Statistics GetStatisticsSummary(StatisticsSnapshot command);

        /// <summary>
        /// Return lifetime statistics for the connection (global summary - not by procedure)
        /// Those statistics are aggregated throughout the life of the connection and not affected by resets
        /// </summary>
        /// <returns>Global lifetime statistics summary for the connection.</returns>
        internal abstract Statistics GetLifetimeStatistics();

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
        internal abstract void ResetStatistics(bool ignorePendingExecutions);

        /// <summary>
        /// Return statistics for a given procedure, grouped by node. (an empty dictionary (not null!) if no node has
        /// any statistics for the given procedure).
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters (for this
        /// procedure only).</param>
        /// <param name="procedure">The procedure for which you want statistics.</param>
        /// <returns>A Statistics snapshot object.</returns>
        internal abstract Dictionary<IPEndPoint, Statistics> GetStatisticsByNode(
                                                                                StatisticsSnapshot command
                                                                              , string procedure
                                                                              );

        /// <summary>
        /// Return statistics for each procedure, grouped by node.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A dictionary containing Statistics snapshot objects for each procedure that was called on the
        /// connection.</returns>
        internal abstract Dictionary<IPEndPoint, Dictionary<string, Statistics>>
            GetStatisticsByNode(StatisticsSnapshot command);

        /// <summary>
        /// Return a statistics summary that aggregates all statistics for every called procedure, and accross all
        /// connections.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A Statistics snapshot object representing a summary of all available statistics for the
        /// connection.</returns>
        internal abstract Dictionary<IPEndPoint, Statistics> GetStatisticsSummaryByNode(StatisticsSnapshot command);

        /// <summary>
        /// Return lifetime statistics for the connection (global summary - not by procedure), grouped by node.
        /// Those statistics are aggregated throughout the life of the connection and not affected by resets.
        /// </summary>
        /// <returns>Global lifetime statistics summary for the connection.</returns>
        internal abstract Dictionary<IPEndPoint, Statistics> GetLifetimeStatisticsByNode();

        /// <summary>
        /// Synchronously execute a procedure against the VoltDB server and block execution of the calling thread.
        /// </summary>
        /// <typeparam name="T">Data type of the expected response result for the call (Table[], Table,
        /// SingleRowTable[], SingleRowTable, T[][], T[] or T, with T one of the supported value types).</typeparam>
        /// <param name="timeout">Timeout value (overrides connection settings DefaultCommandTimeout). Use
        /// Timeout.Infinite or -1 for infinite timeout.</param>
        /// <param name="procedure">The name of the procedure to call.</param>
        /// <param name="procedureUtf8">UTF-8 bytes of procedure name.</param>
        /// <param name="parameters">List of parameters to pass to the procedure.</param>
        /// <returns>The reponse from the server, containing (if available) the result of the procedure execution
        /// (check .IsError prior to using the result).</returns>
        /// <remarks>Synchronous operations will limit the throughput of your aplication, as each of your request waits
        /// for completion before submitting a new call.  For maximum parallelization and throughput, asynchronous
        /// calls should be made (see BeginExecute).</remarks>
        protected internal abstract Response<T> Execute<T>(int timeout, string procedure, byte[] procedureUtf8, params object[] parameters);

        /// <summary>
        /// Wrapper for Execute that converts the procedure name into UTF8 bytes. Use only for Adhoc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="timeout"></param>
        /// <param name="procedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected internal Response<T> Execute<T>(int timeout, string procedure, params object[] parameters)
        {
            return Execute<T>(timeout, procedure, global::System.Text.Encoding.UTF8.GetBytes(procedure), parameters);
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
        /// <param name="procedureUtf8">UTF-8 bytes of procedure name.</param>
        /// <param name="parameters">List of parameters to pass to the procedure.</param>
        /// <returns>The execution handle for the request.</returns>
        protected internal abstract AsyncResponse<T> BeginExecute<T>(
                                                                     ExecuteAsyncCallback<T> callback
                                                                   , object state
                                                                   , int timeout
                                                                   , string procedure
                                                                   , byte[] procedureUtf8
                                                                   , params object[] parameters
                                                                   );
        /// <summary>
        /// Wrapper for BeginExecute to convert string to bytes. Use only for AdHoc.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <param name="timeout"></param>
        /// <param name="procedure"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected internal AsyncResponse<T> BeginExecute<T>(
                                                                     ExecuteAsyncCallback<T> callback
                                                                   , object state
                                                                   , int timeout
                                                                   , string procedure
                                                                   , params object[] parameters
                                                                   )
        {
            return BeginExecute<T>(callback, state, timeout, procedure, global::System.Text.Encoding.UTF8.GetBytes(procedure), parameters);
        }

        /// <summary>
        /// Wait for completion of the request and return the result (or throw an exception if there was a failure).
        /// </summary>
        /// <typeparam name="T">Data type of the expected response result for the call (Table[], Table,
        /// SingleRowTable[], SingleRowTable, T[][], T[] or T, with T one of the supported value types).</typeparam>
        /// <param name="asyncResponse">The execution handle for the request</param>
        /// <returns>The request's response</returns>
        internal static AsyncResponse<T> EndExecute<T>(AsyncResponse<T> asyncResponse)
        {
            // Wait until completion (Note: timeout control is already managed by the background thread, so we do not
            // specify a(nother!) timeout on the WaitHandle).
            if (!asyncResponse.IsCompleted)
                asyncResponse.AsyncWaitHandle.WaitOne();

            // If there was an error, throw it right out.
            if (asyncResponse.Status != ResponseStatus.Success)
                throw asyncResponse.Exception;

            // Return the response.
            return asyncResponse;
        }

        /// <summary>
        /// Builds a connection for the given settings.  If the settings point to a single server, a VoltNodeConnection
        /// is returned.  If the settings point to multiple servers, a VoltClusterConnection is returned.  Both
        /// connection types offer identical signatures, tagged on the base VoltConnection class.  The only difference
        /// is that the VoltClusterConnection will embed automatic validation that all connected nodes belong to the
        /// same cluster, pointing to the same catalog, and perform load balancing between the different servers.
        /// Transparently, the Cluster connection will aggregate statistics accross all connections, defer Trace events
        /// to each sub-Node connection and perform Round-Robin load balancing (for now this is the only algorithm)
        /// between them for actual execution.  The cluster connection offers additional control over its behavior upon
        /// a node failure (should the conection be re-established, etc.) through the connection settings.
        /// </summary>
        /// <param name="settings">Connection settings to use to establish the connection.</param>
        /// <returns>A VoltConnection object (either a single-node VoltNodeConnection or pooled
        /// VoltClusterConnection.</returns>
        public static VoltConnection Create(ConnectionSettings settings)
        {
            CallbackExecutor callbackExecutor = new CallbackExecutor();

            switch (settings.HostAddresses.Length)
            {
                case 0:
                    throw new ArgumentException(
                                                 string.Format(
                                                                Resources.InvalidConnectionStringHostList
                                                              , settings.HostList
                                                              )
                                               );
                case 1:
                    return new VoltNodeConnection(settings, callbackExecutor, true);
                default:
                    return new VoltClusterConnection(settings, callbackExecutor);
            }
        }

        #region IDisposable Members
        /// <summary>
        /// Internal flag tracking whether the object was disposed.
        /// </summary>
		private bool IsDisposed = false;
        /// <summary>
        /// Ensures connected resources are disposed.
        /// </summary>
		~VoltConnection()
		{
			this.Dispose();
		}
        /// <summary>
        /// Ensures connected resources are disposed.
        /// </summary>
        public void Dispose()
        {
			if (this.IsDisposed)
				return;
            try
            {
                this.Close(true);
            }
            catch { }
			this.IsDisposed = true;
			GC.SuppressFinalize(this);
        }
        #endregion
    }
}