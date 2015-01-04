/* This file is part of VoltDB.
 * Copyright (C) 2008-2015 VoltDB Inc.
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
    /// Indicates the status of a <see cref="VoltConnection"/>
    /// </summary>
    public enum ConnectionStatus : byte
    {
        /// <summary>
        /// The connection is currently performing initialization operations (connecting to the server and initializing
        /// internal data structures).
        /// </summary>
        Connecting = 1,

        /// <summary>
        /// The connection is open and ready to receive client execution requests.
        /// </summary>
        Connected = 2,

        /// <summary>
        /// The connection is currently being drained (execution requests are no longer accepted until the internal
        /// execution cache has been emptied of all pending requests).
        /// </summary>
        Draining = 3,

        /// <summary>
        /// The connection is currently closing (closing the socket to the server, clearing up internal data structures
        /// and snapshoting final statistics, when aplicable).
        /// </summary>
        Closing = 4,

        /// <summary>
        /// The connection is closed and will reject any execution request.
        /// </summary>
        Closed = 5
    }
}