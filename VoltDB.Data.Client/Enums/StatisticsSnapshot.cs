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

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Lists available types of statistics snapshot commands for the connection's GetStatistics methods.
    /// </summary>
    public enum StatisticsSnapshot : byte
    {
        /// <summary>
        /// Returns a snapshot of the current statistics, since the connection was opened (or the last reset).
        /// </summary>
        SnapshotOnly = 1,

        /// <summary>
        /// Returns a snapshot of the current statistics and resets counters.  If there are pending executions in the
        /// connection's execution cache, their statistics will not be tracked: only new execution requests will be
        /// tracked.
        /// </summary>
        SnapshotAndResetWithIgnorePendingExecutions = 2,

        /// <summary>
        /// Returns a snapshot of the current statistics and resets counters.  If there are pending executions in the
        /// connection's execution cache, their statistics will be tracked along with any new execution requests.
        /// </summary>
        SnapshotAndResetAndTrackPendingExecutions = 3
    }
}