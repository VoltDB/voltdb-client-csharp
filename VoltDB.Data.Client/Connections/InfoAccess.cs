/* This file is part of VoltDB.
 * Copyright (C) 2008-2017 VoltDB Inc.
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
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Dedicated connection plugin class for info details.
    /// </summary>
    public sealed class InfoAccess
    {
        /// <summary>
        /// Internal reference to the connection/executor that will run the requests.
        /// </summary>
        private VoltConnection Executor;

        /// <summary>
        /// Creates a new instance of this class using the provided executor.  Internal usage only: the executor
        /// itselt will validate access before hooking up this class.
        /// </summary>
        /// <param name="executor">Connection that will execute the requests.</param>
        internal InfoAccess(VoltConnection executor)
        {
            this.Executor = executor;
        }

        /// <summary>
        /// Return the endpoint associated to this connection (whether connected or not).  For a cluster connection,
        /// this is merely the first IP in the complete list of IPs available to the cluster.
        /// </summary>
        public IPEndPoint IPEndPoint
        {
            get
            {
                return this.Executor.IPEndPoint;
            }
        }

        /// <summary>
        /// Date/Time at which the VoltDB cluster to which this connection points was started.
        /// </summary>
        public DateTime ClusterStartTimeStamp
        {
            get
            {
                return this.Executor.ClusterStartTimeStamp;
            }
        }

        /// <summary>
        /// IP address of the leader node of the VltDB cluster to which this connection points.
        /// </summary>
        public IPEndPoint LeaderIPEndPoint
        {
            get
            {
                return this.Executor.LeaderIPEndPoint;
            }
        }

        /// <summary>
        /// Build string of the server's VoltDB engine, containing the Version details.
        /// </summary>
        public string BuildString
        {
            get
            {
                return this.Executor.BuildString;
            }
        }

        /// <summary>
        /// Id of the server to which this connection is pointing (only for Node connections).
        /// </summary>
        public int ServerHostId
        {
            get
            {
                return this.Executor.ServerHostId;
            }
        }

        /// <summary>
        /// Server-assigned Id of this connection (only for Node connections).
        /// </summary>
        public long ConnectionId
        {
            get
            {
                return this.Executor.ConnectionId;
            }
        }

        /// <summary>
        /// Replicate the connection's Status property - useful when querying from the NodeInfoList.
        /// </summary>
        public ConnectionStatus Status
        {
            get
            {
                return this.Executor.Status;
            }
        }

        /// <summary>
        /// Returns whether the current connection is a Cluster connection to multiple nodes.
        /// </summary>
        public bool IsClusterConnection
        {
            get
            {
                return this.Executor is VoltClusterConnection;
            }
        }

        /// <summary>
        /// Returns a list of InfoAccess object for every sub-connection within this connection (for a Node connection,
        /// itself, for a cluster connection, all child connections).
        /// </summary>
        public IEnumerable<InfoAccess> NodeInfoList
        {
            get
            {
                if (this.IsClusterConnection)
                    return (this.Executor as VoltClusterConnection).ConnectionPool.Select(c => c.Info);
                else
                    return new List<InfoAccess>() { this }.AsEnumerable();
            }
        }

        /// <summary>
        /// Return the number of sub-connections (1 for node connections, more for cluster connections).
        /// </summary>
        public int ConnectionCount
        {
            get
            {
                if (this.IsClusterConnection)
                    return (this.Executor as VoltClusterConnection).ConnectionPool.Count;
                else
                    return 1;
            }
        }

        /// <summary>
        /// Return the number of live connections (0 or 1 for node connections, more but less than or equal to
        /// ConnectionCount for cluster connections).
        /// </summary>
        public int LiveConnectionCount
        {
            get
            {
                if (this.IsClusterConnection)
                    return (int)(this.Executor as VoltClusterConnection).ConnectionCount;
                else
                    return this.Executor.Status == ConnectionStatus.Connected ? 1 : 0;
            }
        }

        /// <summary>
        /// Returns the UserId for the connection
        /// </summary>
        public string UserID
        {
            get
            {
                return this.Executor.Settings.UserID;
            }
        }

    }
}