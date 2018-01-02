/* This file is part of VoltDB.
 * Copyright (C) 2008-2018 VoltDB Inc.
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
    /// Lists possible responses statuses provided by the server in return of a query execution request.
    /// </summary>
    public enum ResponseServerStatus : sbyte
    {
        /// <summary>
        /// Indicates the query execution was successful.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Indicates query execution was aborted on the server by the procedure's business logic: VoltAbortException
        /// was thrown during execution.
        /// </summary>
        UserAbort = -1,

        /// <summary>
        /// Indicates a controled execution failure such as a constraint violation or SQL parsing error.
        /// </summary>
        GracefulFailure = -2,

        /// <summary>
        /// Indicates an unexpected transaction failure such as a core SQL execution error or any unexpected runtime
        /// error in the procedure.
        /// </summary>
        UnexpectedFailure = -3,

        /// <summary>
        /// Indicates a failure due to a lost connection.
        /// </summary>
        ConnectionLost = -4,

        /// <summary>
        /// Indicates a failure due to loss of server availability.
        /// </summary>
        ServerUnavailable = -5
    }
}
