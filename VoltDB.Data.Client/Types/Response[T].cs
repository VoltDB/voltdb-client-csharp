/* This file is part of VoltDB.
 * Copyright (C) 2008-2016 VoltDB Inc.
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
    /// Provides a strongly-typed Generic wrapper around server responses.
    /// </summary>
    /// <typeparam name="TResult">Expected return type of the procedure call: Table[], Table, etc.</typeparam>
    public abstract partial class Response<TResult> : Response
    {
        /// <summary>
        /// The strongly-typed query result: a Table[], Table, SingleRowTable[], SingleRowTable, T[][], T[] or T with
        /// T one of the base supported .NET data types mapping to a valid Volt DB type (sbyte, sbyte?, byte, byte?,
        /// short, short?, int, int?, long, long?, double, double?, DateTime, DateTime?, string, BigDecimal).
        /// </summary>
        private TResult _Result;

        /// <summary>
        /// Return the strongly-typed result for the procedure execution.
        /// This property wrapper will re-throw the execution exception if the response is flagged as a failure
        /// response, otherwise, a value value will always be returned.
        /// For access without the overhead of catching an exception, see the method: <seealso cref="TryGetResult"/>.
        /// </summary>
        public TResult Result
        {
            get
            {
                if (this.Status == ResponseStatus.Success)
                    return this._Result;
                throw this.Exception;
            }
        }

        /// <summary>
        /// Safe result accessor: assigns the response result to the out-bound parameter and return true/false to
        /// indicates success.  Will return false (and return default(T) as value) if the execution request failed.
        /// </summary>
        /// <param name="result">The result of the execution call.</param>
        /// <returns>True if the result could be returned, false if the execution failed and the result is not
        /// available (the Exception property will detail the reason).</returns>
        public bool TryGetResult(out TResult result)
        {
            result = this._Result;
            if (this.Status == ResponseStatus.Success)
                return true;
            return false;
        }

        /// <summary>
        /// Creates a new instance of the AsyncResponse class, that serves as a state object during execution of the
        /// request and fully documented Request/Response data structure for the callback method.
        /// </summary>
        /// <param name="executor">Reference to the connection that will execute the request.</param>
        /// <param name="executionId">The reference id for the execution request.</param>
        /// <param name="timeout">Execution timeout for the request.</param>
        /// <param name="procedure">The procedure to execute on the server.</param>
        /// <param name="parameters">The parameters passed to the procedure.</param>
        protected internal Response(
                                     VoltNodeConnection executor
                                   , long executionId
                                   , int timeout
                                   , string procedure
                                   , params object[] parameters
                                   )
            : base(executor, executionId, timeout, procedure, parameters)
        {
        }
    }
}