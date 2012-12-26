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

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Enumeration of custom Tracing event types posted to a <see cref="VoltTrace"/>
    /// </summary>
    public enum VoltTraceEventType : int
    {
        /// <summary>
        /// The traced connection iscurrently being opened.
        /// </summary>
        ConnectionOpening = 1,

        /// <summary>
        /// The connection was opened successfully.
        /// </summary>
        ConnectionOpened,

        /// <summary>
        /// The connection is currently being closed.
        /// </summary>
        ConnectionClosing,

        /// <summary>
        /// The connection has been closed.
        /// </summary>
        ConnectionClosed,

        /// <summary>
        /// An execution request has started.
        /// </summary>
        ExecutionStarted,

        /// <summary>
        /// An execution request was completed.
        /// </summary>
        ExecutionCompleted,

        /// <summary>
        /// An execution request was aborted (on the client-side, through a call to ExecuteCancelAsync).
        /// </summary>
        ExecutionAborted,

        /// <summary>
        /// An execution request failed.
        /// </summary>
        ExecutionFailed,

        /// <summary>
        /// And execution request timed out (on the client-side, based on requested command-timeout).
        /// </summary>
        ExecutionTimedout,

        /// <summary>
        /// The connection initiated the draining process and will refuse all execution requests until that process has
        /// been completed.
        /// </summary>
        DrainingStarted,

        /// <summary>
        /// The draining process was completed: all pending executions on the connection have concluded.
        /// </summary>
        DrainingCompleted,

        /// <summary>
        /// General event flag for non-native messages posted to the trace.
        /// </summary>
        Message
    }
}