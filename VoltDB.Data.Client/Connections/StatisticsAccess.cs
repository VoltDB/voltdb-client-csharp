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

using System.Collections.Generic;
using System.Net;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Dedicated connection plugin class for statistics activities.
    /// </summary>
    public sealed class StatisticsAccess
    {
        /// <summary>
        /// Internal reference to the connection/executor that will run the requests.
        /// </summary>
        private VoltConnection Executor;

        /// <summary>
        /// Creates a new instance of this class using the provided executor.  Internal usage only: the executor
        /// itselt will validate it has statistics access before hooking up this class.
        /// </summary>
        /// <param name="executor">Connection that will execute the requests.</param>
        internal StatisticsAccess(VoltConnection executor)
        {
            this.Executor = executor;
        }

        /// <summary>
        /// Return statistics for a given procedure. (null if there are no available statistics).
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <param name="procedure">The procedure for which you want statistics.</param>
        /// <returns>A Statistics snapshot object.</returns>
        public Statistics GetStatistics(StatisticsSnapshot command, string procedure)
        {
            return this.Executor.GetStatistics(command, procedure);
        }

        /// <summary>
        /// Return statistics for each procedure.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A dictionary containing Statistics snapshot objects for each procedure that was called on the
        /// connection.</returns>
        public Dictionary<string, Statistics> GetStatistics(StatisticsSnapshot command)
        {
            return this.Executor.GetStatistics(command);
        }

        /// <summary>
        /// Return a statistics summary that aggregates all statistics for every called procedure.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A Statistics snapshot object representing a summary of all available statistics for the
        /// connection.</returns>
        public Statistics GetStatisticsSummary(StatisticsSnapshot command)
        {
            return this.Executor.GetStatisticsSummary(command);
        }

        /// <summary>
        /// Return lifetime statistics for the connection (global summary - not by procedure)
        /// Those statistics are aggregated throughout the life of the connection and not affected by resets
        /// </summary>
        /// <returns>Global lifetime statistics summary for the connection.</returns>
        public Statistics GetLifetimeStatistics()
        {
            return this.Executor.GetLifetimeStatistics();
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
        /// <returns>A reference to the current instance for action chaining.</returns>
        public StatisticsAccess ResetStatistics(bool ignorePendingExecutions)
        {
            this.Executor.ResetStatistics(ignorePendingExecutions);
            return this;
        }

        /// <summary>
        /// Return statistics for a given procedure, grouped by node. (an empty dictionary (not null!) if no node has
        /// any statistics for the given procedure).
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters (for this
        /// procedure only).</param>
        /// <param name="procedure">The procedure for which you want statistics.</param>
        /// <returns>A Statistics snapshot object.</returns>
        public Dictionary<IPEndPoint, Statistics> GetStatisticsByNode(
                                                                                StatisticsSnapshot command
                                                                              , string procedure
                                                                              )
        {
            return this.Executor.GetStatisticsByNode(command, procedure);
        }

        /// <summary>
        /// Return statistics for each procedure, grouped by node.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A dictionary containing Statistics snapshot objects for each procedure that was called on the
        /// connection.</returns>
        public Dictionary<IPEndPoint, Dictionary<string, Statistics>>
            GetStatisticsByNode(StatisticsSnapshot command)
        {
            return this.Executor.GetStatisticsByNode(command);
        }

        /// <summary>
        /// Return a statistics summary that aggregates all statistics for every called procedure, and accross all
        /// connections.
        /// </summary>
        /// <param name="command">Whether to snapshot only or also reset the internal statistics counters.</param>
        /// <returns>A Statistics snapshot object representing a summary of all available statistics for the
        /// connection.</returns>
        public Dictionary<IPEndPoint, Statistics> GetStatisticsSummaryByNode(StatisticsSnapshot command)
        {
            return this.Executor.GetStatisticsSummaryByNode(command);
        }

        /// <summary>
        /// Return lifetime statistics for the connection (global summary - not by procedure), grouped by node.
        /// Those statistics are aggregated throughout the life of the connection and not affected by resets.
        /// </summary>
        /// <returns>Global lifetime statistics summary for the connection.</returns>
        public Dictionary<IPEndPoint, Statistics> GetLifetimeStatisticsByNode()
        {
            return this.Executor.GetLifetimeStatisticsByNode();
        }
    }
}