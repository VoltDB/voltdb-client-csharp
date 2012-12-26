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
    /// Lists possible client-side response statuses.
    /// </summary>
    public enum ResponseStatus : byte
    {
        /// <summary>
        /// Indicates the query execution was successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// Indicates the query execution was aborted - on the client-side - through a call to ExecuteCancelAsync.
        /// </summary>
        Aborted = 1,

        /// <summary>
        /// Inidicates the query execution timed out - on the client-side - when the results were not returned by the
        /// server prior to the requested command timeout.
        /// </summary>
        Timedout = 2,

        /// <summary>
        /// Inidicates the query execution failed - encapsulates all of the ResponseServerStatus failure cases.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Indicates the response for a query execution is still pending.
        /// </summary>
        Pending = 4
    }
}