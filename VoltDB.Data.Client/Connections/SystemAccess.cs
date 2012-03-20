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
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Dedicated connection plugin class for system activities.  All operations synchronized.
    /// </summary>
    public sealed class SystemAccess
    {
        /// <summary>
        /// Internal reference to the connection/executor that will run the requests.
        /// </summary>
        private VoltConnection Executor;

        /// <summary>
        /// Creates a new instance of this class using the provided executor.  Internal usage only: the executor
        /// itselt will validate it has root access before hooking up this class.
        /// </summary>
        /// <param name="executor">Connection that will execute the requests.</param>
        internal SystemAccess(VoltConnection executor)
        {
            this.Executor = executor;
        }

        /// <summary>
        /// Waits for all queued export data to be written to the connector.
        /// </summary>
        public void Quiesce()
        {
            this.Executor.Execute<long>(Timeout.Infinite, "@Quiesce");
        }

        /// <summary>
        /// Shuts down the database.
        /// </summary>
        public void Shutdown()
        {
            // We do expect a VoltFailureException when the server drops us, but we'll re-throw anything else.
            try
            {
                this.Executor.Execute<long>(Timeout.Infinite, "@Shutdown");
            }
            catch (VoltConnectionException) { }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Deletes one or more snapshots.
        /// </summary>
        /// <param name="snapshotReferences">List of snapshot references to delete.</param>
        /// <returns>Status of the delete operations.</returns>
        public Response<Table> SnapshotDelete(IEnumerable<SnapshotReference> snapshotReferences)
        {
            return this.Executor.Execute<Table>(
                                                 Timeout.Infinite
                                               , "@SnapshotDelete"
                                               , snapshotReferences.Select(i => i.DirectoryPath).ToArray()
                                               , snapshotReferences.Select(i => i.UniqueId).ToArray()
                                               );
        }

        /// <summary>
        /// Restores a database from disk.
        /// </summary>
        /// <param name="snapshotReference">Key <see cref="SnapshotReference"/> for the snapshot to restore, providing
        /// the path and unique ID.</param>
        /// <returns>Detailed status information on the initiated restore operation.</returns>
        public Response<Table[]> SnapshotRestore(SnapshotReference snapshotReference)
        {
            return this.Executor.Execute<Table[]>(
                                                   Timeout.Infinite
                                                 , "@SnapshotRestore"
                                                 , snapshotReference.DirectoryPath
                                                 , snapshotReference.UniqueId
                                                 );
        }

        /// <summary>
        /// Saves the current database contents to disk.
        /// </summary>
        /// <param name="snapshotReference">Key <see cref="SnapshotReference"/> for the snapshot to save, providing
        /// the path and unique ID.</param>
        /// <param name="blocking">Whether all client transactions should be paused on the server while the snapshot is
        /// being saved (to provide a perfect 'point in time' snapshot), or whether they may run concurrently.</param>
        /// <returns>Detailed status information on the initiated save operation.</returns>
        public Response<Table> SnapshotSave(SnapshotReference snapshotReference, bool blocking)
        {
            return this.Executor.Execute<Table>(
                                                 Timeout.Infinite
                                               , "@SnapshotSave"
                                               , snapshotReference.DirectoryPath
                                               , snapshotReference.UniqueId
                                               , blocking ? 1 : 0
                                               );
        }

        /// <summary>
        /// Lists information about existing snapshots in a given directory path.
        /// </summary>
        /// <param name="directoryPath">Path to the directory to scan.</param>
        /// <returns>Detailed information about the snapshots found in the directory.</returns>
        public Response<Table[]> SnapshotScan(string directoryPath)
        {
            return this.Executor.Execute<Table[]>(Timeout.Infinite, "@SnapshotScan", directoryPath);
        }

        /// <summary>
        /// Lists information about the most recent snapshots created from the current database.
        /// </summary>
        /// <returns>Detailed snapshot status information.</returns>
        public Response<Table[]> SnapshotStatus()
        {
            return this.Executor.Execute<Table[]>(Timeout.Infinite, "@SnapshotStatus");
        }

        /// <summary>
        /// Returns statistics about the usage of the VoltDB database.
        /// </summary>
        /// <param name="component">The specific statistics component to query the system for.</param>
        /// <param name="isInterval">Whether the returned statistics should be aggregated since the time the cluster
        /// was started of since the last interval call.</param>
        /// <returns>Statistics information table set.</returns>
        public Response<Table[]> Statistics(ServerStatistics component, bool isInterval)
        {
            return this.Executor.Execute<Table[]>(
                                                   Timeout.Infinite
                                                 , "@Statistics"
                                                 , component.ToString().ToUpper()
                                                 , isInterval ? 1 : 0
                                                 );
        }

        /// <summary>
        /// Returns system catalog information for the given element.
        /// </summary>
        /// <param name="component">The component to query (TABLES, COLUMNS, etc.)</param>
        /// <returns>Table containing the catalog information for the selected element on the target cluster.</returns>
        public Response<Table[]> SystemCatalog(SystemCatalogComponent component)
        {
            return this.Executor.Execute<Table[]>(Timeout.Infinite, "@SystemCatalog", component.ToString().ToUpper());
        }

        /// <summary>
        /// Returns system information for each node of the database cluster.
        /// </summary>
        /// <returns>Table containing the basic system information for each node in the cluster to which this
        /// connection points.</returns>
        public Response<Table[]> SystemInformation()
        {
            return this.SystemInformation(ServerSysInfoSelector.OVERVIEW);
        }

        /// <summary>
        /// Returns system information for each node of the database cluster.
        /// </summary>
        /// <param name="selector">The System Information item to retrieve (Overview or Deployment)</param>
        /// <returns>Table containing the basic system information for each node in the cluster to which this
        /// connection points.</returns>
        public Response<Table[]> SystemInformation(ServerSysInfoSelector selector)
        {
            return this.Executor.Execute<Table[]>(Timeout.Infinite, "@SystemInformation", selector.ToString().ToUpper());
        }

        /// <summary>
        /// Reconfigures the database by replacing the application catalog currently in use.
        /// </summary>
        /// <param name="catalogPath">Path to the updated catalog's JAR file</param>
        /// <param name="deploymentConfigPath">Path to the updated deploym,ent.xml file</param>
        /// <returns>Status response</returns>
        public Response<long> UpdateApplicationCatalog(string catalogPath, string deploymentConfigPath)
        {
            return this.Executor.Execute<long>(
                                                Timeout.Infinite
                                              , "@UpdateApplicationCatalog"
                                              , catalogPath
                                              , deploymentConfigPath
                                              );
        }

        // Signature change with v1.3 of the engine core.
#if VoltDBVersion12
        /// <summary>
        /// Changes the logging configuration for a running database.
        /// </summary>
        /// <param name="configuration">The XML configuration to ship to the node(s).</param>
        /// <param name="applyToAllNodes">Whether the new configuration is posted to all the nodes in the cluster or
        /// (when false) exclusively to the node this connection is bound to.</param>
        /// <returns>Status response.</returns>
        public Response<long> UpdateLogging(XDocument configuration, bool applyToAllNodes)
        {
            return this.Executor.Execute<long>(
                                                Timeout.Infinite
                                              , "@UpdateLogging"
                                              , configuration.ToString(SaveOptions.DisableFormatting)
                                              , applyToAllNodes ? 1 : 0
                                              );
        }
#else
        /// <summary>
        /// Changes the logging configuration for a running database.
        /// With v1.3, this operation is automatically applied to all nodes.
        /// </summary>
        /// <param name="configuration">The XML configuration to ship to the node(s).</param>
        /// <returns>Status response.</returns>
        public Response<long> UpdateLogging(XDocument configuration)
        {
            return this.Executor.Execute<long>(
                                                Timeout.Infinite
                                              , "@UpdateLogging"
                                              , configuration.ToString(SaveOptions.DisableFormatting)
                                              );
        }
#endif
        /// <summary>
        /// Flag cluster to operate in "Admin Mode" (only connection on admin port accepted).
        /// Connection MUST be made on the Admin Port for this operation to succeed.
        /// </summary>
        public void Pause()
        {
            this.Executor.Execute<long>(Timeout.Infinite, "@Pause");
        }

        /// <summary>
        /// Takes cluster out of the "Admin Mode".
        /// Connection MUST be made on the Admin Port for this operation to succeed.
        /// </summary>
        public void Resume()
        {
            this.Executor.Execute<long>(Timeout.Infinite, "@Resume");
        }
        
        /// <summary>
        /// Promotes the cluster from a replica to a master
        /// The cluster will start accepting write transactions
        /// </summary>
        public void Promote()
        {
            this.Executor.Execute<long>(Timeout.Infinite, "@Promote");
        }

    }
}