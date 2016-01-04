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
 
using System;
using System.Threading;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Defines a command that take 35 parameters.
    /// </summary>
    /// <typeparam name="TResult">Return type of the procedure.</typeparam>
    /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
    /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
    /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
    /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
    /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
    /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
    /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
    /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
    /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
    /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
    /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
    /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
    /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
    /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
    /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
    /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
    /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
    /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
    /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
    /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
    /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
    /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
    /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
    /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
    /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
    /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
    /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
    /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
    /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
    /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
    /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
    /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
    /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
    /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
    /// <typeparam name="T35">Type of parameter 35 of the procedure.</typeparam>
    public class ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> : ProcedureWrapper
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
        /// <param name="parameter1">Value of parameter 1.</param>
        /// <param name="parameter2">Value of parameter 2.</param>
        /// <param name="parameter3">Value of parameter 3.</param>
        /// <param name="parameter4">Value of parameter 4.</param>
        /// <param name="parameter5">Value of parameter 5.</param>
        /// <param name="parameter6">Value of parameter 6.</param>
        /// <param name="parameter7">Value of parameter 7.</param>
        /// <param name="parameter8">Value of parameter 8.</param>
        /// <param name="parameter9">Value of parameter 9.</param>
        /// <param name="parameter10">Value of parameter 10.</param>
        /// <param name="parameter11">Value of parameter 11.</param>
        /// <param name="parameter12">Value of parameter 12.</param>
        /// <param name="parameter13">Value of parameter 13.</param>
        /// <param name="parameter14">Value of parameter 14.</param>
        /// <param name="parameter15">Value of parameter 15.</param>
        /// <param name="parameter16">Value of parameter 16.</param>
        /// <param name="parameter17">Value of parameter 17.</param>
        /// <param name="parameter18">Value of parameter 18.</param>
        /// <param name="parameter19">Value of parameter 19.</param>
        /// <param name="parameter20">Value of parameter 20.</param>
        /// <param name="parameter21">Value of parameter 21.</param>
        /// <param name="parameter22">Value of parameter 22.</param>
        /// <param name="parameter23">Value of parameter 23.</param>
        /// <param name="parameter24">Value of parameter 24.</param>
        /// <param name="parameter25">Value of parameter 25.</param>
        /// <param name="parameter26">Value of parameter 26.</param>
        /// <param name="parameter27">Value of parameter 27.</param>
        /// <param name="parameter28">Value of parameter 28.</param>
        /// <param name="parameter29">Value of parameter 29.</param>
        /// <param name="parameter30">Value of parameter 30.</param>
        /// <param name="parameter31">Value of parameter 31.</param>
        /// <param name="parameter32">Value of parameter 32.</param>
        /// <param name="parameter33">Value of parameter 33.</param>
        /// <param name="parameter34">Value of parameter 34.</param>
        /// <param name="parameter35">Value of parameter 35.</param>
        /// <returns>Result of the procedure call</returns>
        public Response<TResult> Execute(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8, T9 parameter9, T10 parameter10, T11 parameter11, T12 parameter12, T13 parameter13, T14 parameter14, T15 parameter15, T16 parameter16, T17 parameter17, T18 parameter18, T19 parameter19, T20 parameter20, T21 parameter21, T22 parameter22, T23 parameter23, T24 parameter24, T25 parameter25, T26 parameter26, T27 parameter27, T28 parameter28, T29 parameter29, T30 parameter30, T31 parameter31, T32 parameter32, T33 parameter33, T34 parameter34, T35 parameter35)
        {
            return this.Executor.Execute<TResult>(this.CommandTimeout, this.Name, this.NameUtf8Bytes, VoltType.CoalesceNull(parameter1), VoltType.CoalesceNull(parameter2), VoltType.CoalesceNull(parameter3), VoltType.CoalesceNull(parameter4), VoltType.CoalesceNull(parameter5), VoltType.CoalesceNull(parameter6), VoltType.CoalesceNull(parameter7), VoltType.CoalesceNull(parameter8), VoltType.CoalesceNull(parameter9), VoltType.CoalesceNull(parameter10), VoltType.CoalesceNull(parameter11), VoltType.CoalesceNull(parameter12), VoltType.CoalesceNull(parameter13), VoltType.CoalesceNull(parameter14), VoltType.CoalesceNull(parameter15), VoltType.CoalesceNull(parameter16), VoltType.CoalesceNull(parameter17), VoltType.CoalesceNull(parameter18), VoltType.CoalesceNull(parameter19), VoltType.CoalesceNull(parameter20), VoltType.CoalesceNull(parameter21), VoltType.CoalesceNull(parameter22), VoltType.CoalesceNull(parameter23), VoltType.CoalesceNull(parameter24), VoltType.CoalesceNull(parameter25), VoltType.CoalesceNull(parameter26), VoltType.CoalesceNull(parameter27), VoltType.CoalesceNull(parameter28), VoltType.CoalesceNull(parameter29), VoltType.CoalesceNull(parameter30), VoltType.CoalesceNull(parameter31), VoltType.CoalesceNull(parameter32), VoltType.CoalesceNull(parameter33), VoltType.CoalesceNull(parameter34), VoltType.CoalesceNull(parameter35));
        }

        /// <summary>
        /// Execute the procedure asynchronously and return an AsyncResponse that can be used to abort execution
        /// before it completes (using the Cancel method on the object itself).  This call will use the connection's
        /// DefaultCommandTimeout to control execution.  Use overloaded methods to specify a custom timeout value.
        /// Upon completion of the call (Success, failure, timeout or cancellation), the wrapper's Callback method will
        /// be called with the resulting Response object.
        /// </summary>
        /// <param name="parameter1">Value of parameter 1.</param>
        /// <param name="parameter2">Value of parameter 2.</param>
        /// <param name="parameter3">Value of parameter 3.</param>
        /// <param name="parameter4">Value of parameter 4.</param>
        /// <param name="parameter5">Value of parameter 5.</param>
        /// <param name="parameter6">Value of parameter 6.</param>
        /// <param name="parameter7">Value of parameter 7.</param>
        /// <param name="parameter8">Value of parameter 8.</param>
        /// <param name="parameter9">Value of parameter 9.</param>
        /// <param name="parameter10">Value of parameter 10.</param>
        /// <param name="parameter11">Value of parameter 11.</param>
        /// <param name="parameter12">Value of parameter 12.</param>
        /// <param name="parameter13">Value of parameter 13.</param>
        /// <param name="parameter14">Value of parameter 14.</param>
        /// <param name="parameter15">Value of parameter 15.</param>
        /// <param name="parameter16">Value of parameter 16.</param>
        /// <param name="parameter17">Value of parameter 17.</param>
        /// <param name="parameter18">Value of parameter 18.</param>
        /// <param name="parameter19">Value of parameter 19.</param>
        /// <param name="parameter20">Value of parameter 20.</param>
        /// <param name="parameter21">Value of parameter 21.</param>
        /// <param name="parameter22">Value of parameter 22.</param>
        /// <param name="parameter23">Value of parameter 23.</param>
        /// <param name="parameter24">Value of parameter 24.</param>
        /// <param name="parameter25">Value of parameter 25.</param>
        /// <param name="parameter26">Value of parameter 26.</param>
        /// <param name="parameter27">Value of parameter 27.</param>
        /// <param name="parameter28">Value of parameter 28.</param>
        /// <param name="parameter29">Value of parameter 29.</param>
        /// <param name="parameter30">Value of parameter 30.</param>
        /// <param name="parameter31">Value of parameter 31.</param>
        /// <param name="parameter32">Value of parameter 32.</param>
        /// <param name="parameter33">Value of parameter 33.</param>
        /// <param name="parameter34">Value of parameter 34.</param>
        /// <param name="parameter35">Value of parameter 35.</param>
        /// <returns>AsyncResponse with which the caller may later cancel the execution</returns>
        public IAsyncResult BeginExecute(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8, T9 parameter9, T10 parameter10, T11 parameter11, T12 parameter12, T13 parameter13, T14 parameter14, T15 parameter15, T16 parameter16, T17 parameter17, T18 parameter18, T19 parameter19, T20 parameter20, T21 parameter21, T22 parameter22, T23 parameter23, T24 parameter24, T25 parameter25, T26 parameter26, T27 parameter27, T28 parameter28, T29 parameter29, T30 parameter30, T31 parameter31, T32 parameter32, T33 parameter33, T34 parameter34, T35 parameter35)
        {
            return this.Executor.BeginExecute<TResult>(this.Callback, null, this.CommandTimeout, this.Name, this.NameUtf8Bytes, VoltType.CoalesceNull(parameter1), VoltType.CoalesceNull(parameter2), VoltType.CoalesceNull(parameter3), VoltType.CoalesceNull(parameter4), VoltType.CoalesceNull(parameter5), VoltType.CoalesceNull(parameter6), VoltType.CoalesceNull(parameter7), VoltType.CoalesceNull(parameter8), VoltType.CoalesceNull(parameter9), VoltType.CoalesceNull(parameter10), VoltType.CoalesceNull(parameter11), VoltType.CoalesceNull(parameter12), VoltType.CoalesceNull(parameter13), VoltType.CoalesceNull(parameter14), VoltType.CoalesceNull(parameter15), VoltType.CoalesceNull(parameter16), VoltType.CoalesceNull(parameter17), VoltType.CoalesceNull(parameter18), VoltType.CoalesceNull(parameter19), VoltType.CoalesceNull(parameter20), VoltType.CoalesceNull(parameter21), VoltType.CoalesceNull(parameter22), VoltType.CoalesceNull(parameter23), VoltType.CoalesceNull(parameter24), VoltType.CoalesceNull(parameter25), VoltType.CoalesceNull(parameter26), VoltType.CoalesceNull(parameter27), VoltType.CoalesceNull(parameter28), VoltType.CoalesceNull(parameter29), VoltType.CoalesceNull(parameter30), VoltType.CoalesceNull(parameter31), VoltType.CoalesceNull(parameter32), VoltType.CoalesceNull(parameter33), VoltType.CoalesceNull(parameter34), VoltType.CoalesceNull(parameter35));
        }
        
        /// <summary>
        /// Execute the procedure asynchronously and return an AsyncResponse that can be used to abort execution
        /// before it completes (using the Cancel method on the object itself).  This call will use the connection's
        /// DefaultCommandTimeout to control execution.  Use overloaded methods to specify a custom timeout value.
        /// Upon completion of the call (Success, failure, timeout or cancellation), the wrapper's Callback method will
        /// be called with the resulting Response object.
        /// </summary>
        /// <param name="parameter1">Value of parameter 1.</param>
        /// <param name="parameter2">Value of parameter 2.</param>
        /// <param name="parameter3">Value of parameter 3.</param>
        /// <param name="parameter4">Value of parameter 4.</param>
        /// <param name="parameter5">Value of parameter 5.</param>
        /// <param name="parameter6">Value of parameter 6.</param>
        /// <param name="parameter7">Value of parameter 7.</param>
        /// <param name="parameter8">Value of parameter 8.</param>
        /// <param name="parameter9">Value of parameter 9.</param>
        /// <param name="parameter10">Value of parameter 10.</param>
        /// <param name="parameter11">Value of parameter 11.</param>
        /// <param name="parameter12">Value of parameter 12.</param>
        /// <param name="parameter13">Value of parameter 13.</param>
        /// <param name="parameter14">Value of parameter 14.</param>
        /// <param name="parameter15">Value of parameter 15.</param>
        /// <param name="parameter16">Value of parameter 16.</param>
        /// <param name="parameter17">Value of parameter 17.</param>
        /// <param name="parameter18">Value of parameter 18.</param>
        /// <param name="parameter19">Value of parameter 19.</param>
        /// <param name="parameter20">Value of parameter 20.</param>
        /// <param name="parameter21">Value of parameter 21.</param>
        /// <param name="parameter22">Value of parameter 22.</param>
        /// <param name="parameter23">Value of parameter 23.</param>
        /// <param name="parameter24">Value of parameter 24.</param>
        /// <param name="parameter25">Value of parameter 25.</param>
        /// <param name="parameter26">Value of parameter 26.</param>
        /// <param name="parameter27">Value of parameter 27.</param>
        /// <param name="parameter28">Value of parameter 28.</param>
        /// <param name="parameter29">Value of parameter 29.</param>
        /// <param name="parameter30">Value of parameter 30.</param>
        /// <param name="parameter31">Value of parameter 31.</param>
        /// <param name="parameter32">Value of parameter 32.</param>
        /// <param name="parameter33">Value of parameter 33.</param>
        /// <param name="parameter34">Value of parameter 34.</param>
        /// <param name="parameter35">Value of parameter 35.</param>
        /// <param name="state">A user-defined state object to be passed to your callback through the Response's
        /// .AsyncState property</param>
        /// <returns>AsyncResponse with which the caller may later cancel the execution</returns>
        public IAsyncResult BeginExecute(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8, T9 parameter9, T10 parameter10, T11 parameter11, T12 parameter12, T13 parameter13, T14 parameter14, T15 parameter15, T16 parameter16, T17 parameter17, T18 parameter18, T19 parameter19, T20 parameter20, T21 parameter21, T22 parameter22, T23 parameter23, T24 parameter24, T25 parameter25, T26 parameter26, T27 parameter27, T28 parameter28, T29 parameter29, T30 parameter30, T31 parameter31, T32 parameter32, T33 parameter33, T34 parameter34, T35 parameter35, object state)
        {
            return this.Executor.BeginExecute<TResult>(this.Callback, state, this.CommandTimeout, this.Name, this.NameUtf8Bytes, VoltType.CoalesceNull(parameter1), VoltType.CoalesceNull(parameter2), VoltType.CoalesceNull(parameter3), VoltType.CoalesceNull(parameter4), VoltType.CoalesceNull(parameter5), VoltType.CoalesceNull(parameter6), VoltType.CoalesceNull(parameter7), VoltType.CoalesceNull(parameter8), VoltType.CoalesceNull(parameter9), VoltType.CoalesceNull(parameter10), VoltType.CoalesceNull(parameter11), VoltType.CoalesceNull(parameter12), VoltType.CoalesceNull(parameter13), VoltType.CoalesceNull(parameter14), VoltType.CoalesceNull(parameter15), VoltType.CoalesceNull(parameter16), VoltType.CoalesceNull(parameter17), VoltType.CoalesceNull(parameter18), VoltType.CoalesceNull(parameter19), VoltType.CoalesceNull(parameter20), VoltType.CoalesceNull(parameter21), VoltType.CoalesceNull(parameter22), VoltType.CoalesceNull(parameter23), VoltType.CoalesceNull(parameter24), VoltType.CoalesceNull(parameter25), VoltType.CoalesceNull(parameter26), VoltType.CoalesceNull(parameter27), VoltType.CoalesceNull(parameter28), VoltType.CoalesceNull(parameter29), VoltType.CoalesceNull(parameter30), VoltType.CoalesceNull(parameter31), VoltType.CoalesceNull(parameter32), VoltType.CoalesceNull(parameter33), VoltType.CoalesceNull(parameter34), VoltType.CoalesceNull(parameter35));
        }
        
        /// <summary>
        /// Execute the procedure asynchronously and return an AsyncResponse that can be used to abort execution
        /// before it completes (using the Cancel method on the object itself).  This call will use the connection's
        /// DefaultCommandTimeout to control execution.  Use overloaded methods to specify a custom timeout value.
        /// Upon completion of the call (Success, failure, timeout or cancellation), the wrapper's Callback method will
        /// be called with the resulting Response object.
        /// </summary>
        /// <param name="parameter1">Value of parameter 1.</param>
        /// <param name="parameter2">Value of parameter 2.</param>
        /// <param name="parameter3">Value of parameter 3.</param>
        /// <param name="parameter4">Value of parameter 4.</param>
        /// <param name="parameter5">Value of parameter 5.</param>
        /// <param name="parameter6">Value of parameter 6.</param>
        /// <param name="parameter7">Value of parameter 7.</param>
        /// <param name="parameter8">Value of parameter 8.</param>
        /// <param name="parameter9">Value of parameter 9.</param>
        /// <param name="parameter10">Value of parameter 10.</param>
        /// <param name="parameter11">Value of parameter 11.</param>
        /// <param name="parameter12">Value of parameter 12.</param>
        /// <param name="parameter13">Value of parameter 13.</param>
        /// <param name="parameter14">Value of parameter 14.</param>
        /// <param name="parameter15">Value of parameter 15.</param>
        /// <param name="parameter16">Value of parameter 16.</param>
        /// <param name="parameter17">Value of parameter 17.</param>
        /// <param name="parameter18">Value of parameter 18.</param>
        /// <param name="parameter19">Value of parameter 19.</param>
        /// <param name="parameter20">Value of parameter 20.</param>
        /// <param name="parameter21">Value of parameter 21.</param>
        /// <param name="parameter22">Value of parameter 22.</param>
        /// <param name="parameter23">Value of parameter 23.</param>
        /// <param name="parameter24">Value of parameter 24.</param>
        /// <param name="parameter25">Value of parameter 25.</param>
        /// <param name="parameter26">Value of parameter 26.</param>
        /// <param name="parameter27">Value of parameter 27.</param>
        /// <param name="parameter28">Value of parameter 28.</param>
        /// <param name="parameter29">Value of parameter 29.</param>
        /// <param name="parameter30">Value of parameter 30.</param>
        /// <param name="parameter31">Value of parameter 31.</param>
        /// <param name="parameter32">Value of parameter 32.</param>
        /// <param name="parameter33">Value of parameter 33.</param>
        /// <param name="parameter34">Value of parameter 34.</param>
        /// <param name="parameter35">Value of parameter 35.</param>
        /// <param name="callback">Execution callback (overrides any default provided when the wraper was
        /// created).</param>
        /// <param name="state">A user-defined state object to be passed to your callback through the Response's
        /// .AsyncState property</param>
        /// <returns>AsyncResponse with which the caller may later cancel the execution</returns>
        public IAsyncResult BeginExecute(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8, T9 parameter9, T10 parameter10, T11 parameter11, T12 parameter12, T13 parameter13, T14 parameter14, T15 parameter15, T16 parameter16, T17 parameter17, T18 parameter18, T19 parameter19, T20 parameter20, T21 parameter21, T22 parameter22, T23 parameter23, T24 parameter24, T25 parameter25, T26 parameter26, T27 parameter27, T28 parameter28, T29 parameter29, T30 parameter30, T31 parameter31, T32 parameter32, T33 parameter33, T34 parameter34, T35 parameter35, ExecuteAsyncCallback<TResult> callback, object state)
        {
            return this.Executor.BeginExecute<TResult>(callback, state, this.CommandTimeout, this.Name, this.NameUtf8Bytes, VoltType.CoalesceNull(parameter1), VoltType.CoalesceNull(parameter2), VoltType.CoalesceNull(parameter3), VoltType.CoalesceNull(parameter4), VoltType.CoalesceNull(parameter5), VoltType.CoalesceNull(parameter6), VoltType.CoalesceNull(parameter7), VoltType.CoalesceNull(parameter8), VoltType.CoalesceNull(parameter9), VoltType.CoalesceNull(parameter10), VoltType.CoalesceNull(parameter11), VoltType.CoalesceNull(parameter12), VoltType.CoalesceNull(parameter13), VoltType.CoalesceNull(parameter14), VoltType.CoalesceNull(parameter15), VoltType.CoalesceNull(parameter16), VoltType.CoalesceNull(parameter17), VoltType.CoalesceNull(parameter18), VoltType.CoalesceNull(parameter19), VoltType.CoalesceNull(parameter20), VoltType.CoalesceNull(parameter21), VoltType.CoalesceNull(parameter22), VoltType.CoalesceNull(parameter23), VoltType.CoalesceNull(parameter24), VoltType.CoalesceNull(parameter25), VoltType.CoalesceNull(parameter26), VoltType.CoalesceNull(parameter27), VoltType.CoalesceNull(parameter28), VoltType.CoalesceNull(parameter29), VoltType.CoalesceNull(parameter30), VoltType.CoalesceNull(parameter31), VoltType.CoalesceNull(parameter32), VoltType.CoalesceNull(parameter33), VoltType.CoalesceNull(parameter34), VoltType.CoalesceNull(parameter35));
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
        /// <param name="parameter1">Value of parameter 1.</param>
        /// <param name="parameter2">Value of parameter 2.</param>
        /// <param name="parameter3">Value of parameter 3.</param>
        /// <param name="parameter4">Value of parameter 4.</param>
        /// <param name="parameter5">Value of parameter 5.</param>
        /// <param name="parameter6">Value of parameter 6.</param>
        /// <param name="parameter7">Value of parameter 7.</param>
        /// <param name="parameter8">Value of parameter 8.</param>
        /// <param name="parameter9">Value of parameter 9.</param>
        /// <param name="parameter10">Value of parameter 10.</param>
        /// <param name="parameter11">Value of parameter 11.</param>
        /// <param name="parameter12">Value of parameter 12.</param>
        /// <param name="parameter13">Value of parameter 13.</param>
        /// <param name="parameter14">Value of parameter 14.</param>
        /// <param name="parameter15">Value of parameter 15.</param>
        /// <param name="parameter16">Value of parameter 16.</param>
        /// <param name="parameter17">Value of parameter 17.</param>
        /// <param name="parameter18">Value of parameter 18.</param>
        /// <param name="parameter19">Value of parameter 19.</param>
        /// <param name="parameter20">Value of parameter 20.</param>
        /// <param name="parameter21">Value of parameter 21.</param>
        /// <param name="parameter22">Value of parameter 22.</param>
        /// <param name="parameter23">Value of parameter 23.</param>
        /// <param name="parameter24">Value of parameter 24.</param>
        /// <param name="parameter25">Value of parameter 25.</param>
        /// <param name="parameter26">Value of parameter 26.</param>
        /// <param name="parameter27">Value of parameter 27.</param>
        /// <param name="parameter28">Value of parameter 28.</param>
        /// <param name="parameter29">Value of parameter 29.</param>
        /// <param name="parameter30">Value of parameter 30.</param>
        /// <param name="parameter31">Value of parameter 31.</param>
        /// <param name="parameter32">Value of parameter 32.</param>
        /// <param name="parameter33">Value of parameter 33.</param>
        /// <param name="parameter34">Value of parameter 34.</param>
        /// <param name="parameter35">Value of parameter 35.</param>
        /// <param name="response">Result of the procedure call</param>
        /// <returns>True if the execution was successfully posted, false otherwise (will only happen if the connection died on you)</returns>
        public bool TryExecute(T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4, T5 parameter5, T6 parameter6, T7 parameter7, T8 parameter8, T9 parameter9, T10 parameter10, T11 parameter11, T12 parameter12, T13 parameter13, T14 parameter14, T15 parameter15, T16 parameter16, T17 parameter17, T18 parameter18, T19 parameter19, T20 parameter20, T21 parameter21, T22 parameter22, T23 parameter23, T24 parameter24, T25 parameter25, T26 parameter26, T27 parameter27, T28 parameter28, T29 parameter29, T30 parameter30, T31 parameter31, T32 parameter32, T33 parameter33, T34 parameter34, T35 parameter35, out Response<TResult> response)
        {
            try
            {
                response = this.Executor.Execute<TResult>(this.CommandTimeout, this.Name, this.NameUtf8Bytes, VoltType.CoalesceNull(parameter1), VoltType.CoalesceNull(parameter2), VoltType.CoalesceNull(parameter3), VoltType.CoalesceNull(parameter4), VoltType.CoalesceNull(parameter5), VoltType.CoalesceNull(parameter6), VoltType.CoalesceNull(parameter7), VoltType.CoalesceNull(parameter8), VoltType.CoalesceNull(parameter9), VoltType.CoalesceNull(parameter10), VoltType.CoalesceNull(parameter11), VoltType.CoalesceNull(parameter12), VoltType.CoalesceNull(parameter13), VoltType.CoalesceNull(parameter14), VoltType.CoalesceNull(parameter15), VoltType.CoalesceNull(parameter16), VoltType.CoalesceNull(parameter17), VoltType.CoalesceNull(parameter18), VoltType.CoalesceNull(parameter19), VoltType.CoalesceNull(parameter20), VoltType.CoalesceNull(parameter21), VoltType.CoalesceNull(parameter22), VoltType.CoalesceNull(parameter23), VoltType.CoalesceNull(parameter24), VoltType.CoalesceNull(parameter25), VoltType.CoalesceNull(parameter26), VoltType.CoalesceNull(parameter27), VoltType.CoalesceNull(parameter28), VoltType.CoalesceNull(parameter29), VoltType.CoalesceNull(parameter30), VoltType.CoalesceNull(parameter31), VoltType.CoalesceNull(parameter32), VoltType.CoalesceNull(parameter33), VoltType.CoalesceNull(parameter34), VoltType.CoalesceNull(parameter35));
                return true;
            }
            catch { response = null; }
            return false;
        }
    }
}
