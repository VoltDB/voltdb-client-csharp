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

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Enumeration of available system statistics that can be queried by the @Statistics system procedure.
    /// </summary>
    public enum ServerStatistics : byte
    {
        /// <summary>
        /// Index statistics.
        /// </summary>
        INDEX = 1,

        /// <summary>
        /// Initiator statistics.
        /// </summary>
        INITIATOR = 2,

        /// <summary>
        /// I/O statistics.
        /// </summary>
        IOSTATS = 3,

        /// <summary>
        /// Management statistics.
        /// (multiple resultsets for INDEX, INITIATOR, IOSTATS, MEMORY, PROCEDURE and TABLE statistics)
        /// </summary>
        MANAGEMENT = 4,

        /// <summary>
        /// Memory usage statistics.
        /// </summary>
        MEMORY = 5,

        /// <summary>
        /// Procedure execution statistics.
        /// </summary>
        PROCEDURE = 6,

        /// <summary>
        /// Table statistics.
        /// </summary>
        TABLE = 7,

        /// <summary>
        /// Returns the number of partitions in the VoltDB cluster.
        /// Equal to: [hostCount*partitionCountPerHost]/[kFactor+1]
        /// </summary>
        PARTITIONCOUNT = 8,

        /// <summary>
        /// Cluster starvation details.
        /// </summary>
        STARVATION = 9,

        /// <summary>
        /// Information about currently connected clients.
        /// </summary>
        LIVECLIENTS = 10,

        /// <summary>
        /// Information about replication
        /// </summary>
        DR = 11
    }
}
