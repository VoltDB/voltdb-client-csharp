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
    /// Provides a handle on a specific execution (used to perform cancellations)
    /// </summary>
    /// <typeparam name="TResult">Expected return type of the procedure call: Table[], Table, etc.</typeparam>
    public sealed class AsyncResponse<TResult> : Response<TResult>, IAsyncResult
    {
        /// <summary>
        /// The callback to execute upon completion of the request.
        /// </summary>
        private readonly ExecuteAsyncCallback<TResult> Callback;

        /// <summary>
        /// Flag indicating whether the response has an associated completion callback.
        /// </summary>
        protected override bool HasCallback
        {
            get
            {
                return this.Callback != null;
            }
        }

        /// <summary>
        /// A user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        private readonly object _AsyncState;

        /// <summary>
        /// A user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        public object AsyncState
        {
            get
            {
                return this._AsyncState;
            }
        }

        /// <summary>
        /// Creates a new instance of the AsyncResponse class, that serves as a state object during execution of the
        /// request and fully documented Request/Response data structure for the callback method.
        /// </summary>
        /// <param name="executor">Reference to the connection that will execute the request.</param>
        /// <param name="executionId">The reference id for the execution request.</param>
        /// <param name="timeout">Execution timeout for the request.</param>
        /// <param name="callback">The callback to execute upon completion.</param>
        /// <param name="state">A user-defined object that qualifies or contains information about an asynchronous
        /// operation.</param>
        /// <param name="procedure">The procedure to execute on the server.</param>
        /// <param name="parameters">The parameters passed to the procedure.</param>
        internal AsyncResponse(
                                VoltNodeConnection executor
                              , long executionId
                              , int timeout
                              , ExecuteAsyncCallback<TResult> callback
                              , object state
                              , string procedure
                              , params object[] parameters
                              )
            : base(executor, executionId, timeout, procedure, parameters)
        {
            this.Callback = callback;
            this._AsyncState = state;
        }

        /// <summary>
        /// Cancels the request associated to this execution, causing triggerring of the Asynchronous callback with a
        /// VoltClientAbortException result.  Understand that this process merely lets you decide to "forget" about
        /// the request, however, the execution has been posted to the server and will be completed all the same.
        /// </summary>
        /// <returns>True if the request was successfully cancelled - false if a timeout or server callback was
        /// triggered before this call was made and the handle is no longer valid.</returns>
        internal bool Cancel()
        {
            return this.Executor.ExecuteCancelAsync(this);
        }

        /// <summary>
        /// Perform manual lambda lifting on this callback
        /// </summary>
        static readonly WaitCallback _onExecCompleteWC = delegate(object s)
        {
            try { ((AsyncResponse<TResult>)s).Callback((AsyncResponse<TResult>)s); }
            finally { ((AsyncResponse<TResult>)s).OnNotifyCompleted(false); }
        };
 
        /// <summary>
        /// Trigger the user callback upon completion, by pushing a new work item to the ThreadPool (thus forking out
        /// the callback and ensuring long-running callbacks do not endanger the stability of the connection).
        /// </summary>
        protected override void OnExecutionComplete()
        {
            // Don't attempt to call anything if this was a fire & forget
            if (this.Callback != null)
            {
                // Attempt to queue the callback into the ThreadPool for deferred processing.  If the system is
                // overloaded, this will raise an exception (and in all likelyhood, the system is about to crash,
                // so we just wrap and rethrow.
                try
                {
                    this.Executor.CallbackExecutor.QueueWorkItem(_onExecCompleteWC, this);
                    //ThreadPool.UnsafeQueueUserWorkItem(_onExecCompleteWC, this);
                    //_onExecCompleteWC(this); //PERF: Test this for single threaded callbacks
                }
                catch (Exception x)
                {
                    throw new VoltExecutionException(Resources.CannotSpawnCallbackProcess, x);
                }
            }
        }

        #region IAsyncResult Members
        /// <summary>
        /// Issues notification that the request has completed, signaling the MRE if necessary.
        /// </summary>
        /// <param name="completedSynchronously">Whether the request completed synchronously.</param>
        protected override void OnNotifyCompleted(bool completedSynchronously)
        {
            // Update the state prior to invoking the callback.
            if (
                 Interlocked.Exchange(
                                       ref CompletionState
                                     , completedSynchronously
                                       ? StateCompletedSynchronously
                                       : StateCompletedAsynchronously
                                     )
                 != StatePending
               )
                throw new InvalidOperationException(Resources.AsyncResultSetAsCompletedAlreadyCalled);

            // If the event exists, set it.
            if (this._AsyncWaitHandle != null) this._AsyncWaitHandle.Set();
        }

        /// <summary>
        /// Manual Reset event to allow waits on the operation - created on-demand only
        /// </summary>
        private ManualResetEvent _AsyncWaitHandle;

        /// <summary>
        /// A WaitHandle that is used to wait for an asynchronous operation to complete.
        /// </summary>
        public WaitHandle AsyncWaitHandle
        {
            get
            {
                // We only create the handle on-demand to mitigate performance cost...
                if (this._AsyncWaitHandle == null)
                {
                    bool done = this.IsCompleted;
                    ManualResetEvent mre = new ManualResetEvent(done);
                    if (Interlocked.CompareExchange(ref this._AsyncWaitHandle, mre, null) != null)
                    {
                        // If another thread created the MRE; dispose of the one we just created.
                        mre.Close();
                    }
                    else
                    {
                        if (!done && this.IsCompleted)
                        {
                            // If the operation wasn't done when we created the event but now is, set the event.
                            this._AsyncWaitHandle.Set();
                        }
                    }
                }
                return this._AsyncWaitHandle;
            }
        }

        /// <summary>
        /// Indicates the IAsyncResult is pending completion
        /// </summary>
        private const int StatePending = 0;

        /// <summary>
        /// Indicates the IAsyncResult was completed synchronously
        /// </summary>
        private const int StateCompletedSynchronously = 1;

        /// <summary>
        /// Indicates the IAsyncResult was completed asynchronously
        /// </summary>
        private const int StateCompletedAsynchronously = 2;

        /// <summary>
        /// Internal flag hoding the status of the IAsyncResult
        /// </summary>
        private int CompletionState = StatePending;

        /// <summary>
        /// A value that indicates whether the asynchronous operation completed synchronously.
        /// </summary>
        public bool CompletedSynchronously
        {
            get
            {
                return Thread.VolatileRead(ref this.CompletionState) == StateCompletedSynchronously;
            }
        }

        /// <summary>
        /// A value that indicates whether the asynchronous operation has completed.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return Thread.VolatileRead(ref this.CompletionState) != StatePending;
            }
        }

        #endregion
    }
}