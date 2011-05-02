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
using System.Threading;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Defines a command that take 0 parameters.
    /// </summary>
    /// <typeparam name="TResult">Return type of the procedure.</typeparam>
    public class ProcedureWrapper<TResult> : ProcedureWrapper
    {
        /// <summary>
        /// Internal callback reference for asynchronous calls.
        /// </summary>
        private ExecuteAsyncCallback<TResult> Callback;

        /// <summary>
        /// Internal constructor: factory protocol only.
        /// </summary>
        /// <param name="executor">Connection that will execute requests for this procedure wrapper.</param>
        /// <param name="name">Name of the procedure.</param>
        /// <param name="timeout">Wrapper-specific command timeout.</param>
        /// <param name="callback">Execution callback.</param>
        internal ProcedureWrapper(VoltConnection executor, string name, int timeout, ExecuteAsyncCallback<TResult> callback)
            : base(executor, name, timeout)
        {
            this.Callback = callback;
        }

        /// <summary>
        /// Execute the procedure synchronously and return the result.
        /// This call will use the connection's DefaultCommandTimeout to control execution.  Use overloaded methods to
        /// specify a custom timeout value.
        /// </summary>
        /// <returns>Result of the procedure call</returns>
        public Response<TResult> Execute()
        {
            return this.Executor.Execute<TResult>(this.CommandTimeout, this.Name, this.NameUtf8Bytes);
        }

        /// <summary>
        /// Execute the procedure asynchronously and return an AsyncResponse that can be used to abort execution
        /// before it completes (using the Cancel method on the object itself).  This call will use the connection's
        /// DefaultCommandTimeout to control execution.  Use overloaded methods to specify a custom timeout value.
        /// Upon completion of the call (Success, failure, timeout or cancellation), the wrapper's Callback method will
        /// be called with the resulting Response object.
        /// </summary>
        /// <returns>AsyncResponse with which the caller may later cancel the execution</returns>
        public IAsyncResult BeginExecute()
        {
            return this.Executor.BeginExecute<TResult>(this.Callback, null, this.CommandTimeout, this.Name, this.NameUtf8Bytes);
        }
        
        /// <summary>
        /// Execute the procedure asynchronously and return an AsyncResponse that can be used to abort execution
        /// before it completes (using the Cancel method on the object itself).  This call will use the connection's
        /// DefaultCommandTimeout to control execution.  Use overloaded methods to specify a custom timeout value.
        /// Upon completion of the call (Success, failure, timeout or cancellation), the wrapper's Callback method will
        /// be called with the resulting Response object.
        /// </summary>
        /// <param name="state">A user-defined state object to be passed to your callback through the Response's
        /// .AsyncState property</param>
        /// <returns>AsyncResponse with which the caller may later cancel the execution</returns>
        public IAsyncResult BeginExecute(object state)
        {
            return this.Executor.BeginExecute<TResult>(this.Callback, state, this.CommandTimeout, this.Name, this.NameUtf8Bytes);
        }
        
        /// <summary>
        /// Execute the procedure asynchronously and return an AsyncResponse that can be used to abort execution
        /// before it completes (using the Cancel method on the object itself).  This call will use the connection's
        /// DefaultCommandTimeout to control execution.  Use overloaded methods to specify a custom timeout value.
        /// Upon completion of the call (Success, failure, timeout or cancellation), the wrapper's Callback method will
        /// be called with the resulting Response object.
        /// </summary>
        /// <param name="callback">Execution callback (overrides any default provided when the wraper was
        /// created).</param>
        /// <param name="state">A user-defined state object to be passed to your callback through the Response's
        /// .AsyncState property</param>
        /// <returns>AsyncResponse with which the caller may later cancel the execution</returns>
        public IAsyncResult BeginExecute(ExecuteAsyncCallback<TResult> callback, object state)
        {
            return this.Executor.BeginExecute<TResult>(callback, state, this.CommandTimeout, this.Name, this.NameUtf8Bytes);
        }

        /// <summary>
        /// Wait for completion of the request and return the result (or throw an exception if there was a failure).
        /// </summary>
        /// <param name="asyncResponse">The execution handle for the request</param>
        /// <returns>The request's response</returns>
        private AsyncResponse<TResult> EndExecute(AsyncResponse<TResult> asyncResponse)
        {
            if (asyncResponse.Procedure != this.Name)
                throw new ArgumentException(Resources.AsyncResponseMismatch);
            return VoltConnection.EndExecute<TResult>(asyncResponse);
        }

        /// <summary>
        /// Wait for completion of the request and return the result (or throw an exception if there was a failure).
        /// </summary>
        /// <param name="asyncResult">The IAsyncResult for the request</param>
        /// <returns>The request's response</returns>
        public AsyncResponse<TResult> EndExecute(IAsyncResult asyncResult)
        {
            return this.EndExecute(asyncResult as AsyncResponse<TResult>);
        }
        
        /// <summary>
        /// Cancels the request associated to this execution, causing triggerring of the Asynchronous callback with a
        /// VoltClientAbortException result.  Understand that this process merely lets you decide to "forget" about
        /// the request, however, the execution has been posted to the server and will be completed all the same.
        /// </summary>
        /// <param name="asyncResponse">The execution handle for the request</param>
        private void ExecuteCancelAsync(AsyncResponse<TResult> asyncResponse)
        {
            asyncResponse.Cancel();
        }

        /// <summary>
        /// Cancels the request associated to this execution, causing triggerring of the Asynchronous callback with a
        /// VoltClientAbortException result.  Understand that this process merely lets you decide to "forget" about
        /// the request, however, the execution has been posted to the server and will be completed all the same.
        /// </summary>
        /// <param name="asyncResult">The IAsyncResult for the request</param>
        public void ExecuteCancelAsync(IAsyncResult asyncResult)
        {
            this.ExecuteCancelAsync(asyncResult as AsyncResponse<TResult>);
        }

        /// <summary>
        /// Execute the procedure synchronously and return the result.
        /// This call will use the connection's DefaultCommandTimeout to control execution.  Use overloaded methods to
        /// specify a custom timeout value.  Convenience TryMethod-style execution: the method wraps the underlying
        /// call into a try/catch block so you don't have to and will return false in case of failure.
        /// </summary>
        /// <remarks>You will ONLY get an exception in case the connection dies on you.  Actual execution errors (such
        /// as a constraint violation while executing a procedure on the server) are packaged into the response object.
        /// While you may prefer this method to writing your own try/catch block, you should keep in mind that a
        /// failure prior to actually shipping out the request to the server usually means your application is in no
        /// suitable shape to function; your catch or if(!proc.Exec(...)) block will have to adequately deal with the
        /// situation.
        /// </remarks>
        /// <param name="response">Result of the procedure call</param>
        /// <returns>True if the execution was successfully posted, false otherwise (will only happen if the connection died on you)</returns>
        public bool TryExecute(out Response<TResult> response)
        {
            try
            {
                response = this.Executor.Execute<TResult>(this.CommandTimeout, this.Name, this.NameUtf8Bytes);
                return true;
            }
            catch { response = null; }
            return false;
        }
    }
}
