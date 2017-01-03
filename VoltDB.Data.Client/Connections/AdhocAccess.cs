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

using System.Threading;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Dedicated connection plugin class for adhoc activities.  All operations synchronized.
    /// </summary>
    public sealed class AdhocAccess
    {
        /// <summary>
        /// Internal reference to the connection/executor that will run the requests.
        /// </summary>
        private VoltConnection Executor;

        /// <summary>
        /// Creates a new instance of this class using the provided executor.  Internal usage only: the executor
        /// itselt will validate it has adhoc access before hooking up this class.
        /// </summary>
        /// <param name="executor">Connection that will execute the requests.</param>
        internal AdhocAccess(VoltConnection executor)
        {
            this.Executor = executor;
        }

        /// <summary>
        /// Runs an ad-hoc query
        /// </summary>
        /// <typeparam name="TResult">Expected return type: Table, SingleRowTable, basetype[] or basetype (only one
        /// dataset returned).</typeparam>
        /// <param name="query">The query to run.</param>
        /// <returns>Query result.</returns>
        public Response<TResult> Execute<TResult>(string query)
        {
            // Don't let dirty parameter injection through
            if (query.Contains("?"))
                throw new VoltConnectionException(Resources.AdHocParameterQueriesNotAllowed, query);

            return this.Executor.Execute<TResult>(Timeout.Infinite, "@AdHoc", query);
        }
    }
}