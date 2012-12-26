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

using System;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides the basic support for the Response object, used to parse out return messages from the server and
    /// deserialize their payload into usable .NET objects and values.
    /// </summary>
    public abstract partial class Response
    {
        /// <summary>
        /// The execution request id (internal tracking for the owning connection).
        /// </summary>
        public readonly long ExecutionId;

        /// <summary>
        /// We cannot use wait handles for asynchronous operations, even using a background thread to perform a
        /// .WaitAny on a given list... Because?: there will be potentially thousands of requests to wait on
        /// (WaithAny is limited to 64).
        /// </summary>
        internal readonly long ExpirationTicks;

        /// <summary>
        /// The procedure to call/called.
        /// </summary>
        public readonly string Procedure;

        /// <summary>
        /// Parameters to pass/passed to the procedure.
        /// </summary>
        public readonly object[] Parameters;

        /// <summary>
        /// Indicates the status of the response (client-side status - for server-side status, see the ServerStatus
        /// property).
        /// </summary>
        private ResponseStatus _Status = ResponseStatus.Pending;

        /// <summary>
        /// Indicates the status of the response (client-side status - for server-side status, see the ServerStatus
        /// property).
        /// </summary>
        public ResponseStatus Status
        {
            get
            {
                return this._Status;
            }
        }

        internal readonly VoltNodeConnection Executor;

        /// <summary>
        /// Creates a new instance of the AsynResponseBase class.
        /// </summary>
        /// <param name="executor">Reference to the connection that will execute the request.</param>
        /// <param name="executionId">The reference id for the execution request.</param>
        /// <param name="timeout">Execution timeout for the request.</param>
        /// <param name="procedure">The procedure to execute on the server.</param>
        /// <param name="parameters">The parameters passed to the procedure.</param>
        internal Response(VoltNodeConnection executor, long executionId, int timeout, string procedure, params object[] parameters)
        {
            this.Executor = executor;
            this.ExecutionId = executionId;
            this.Procedure = procedure;
            this.Parameters = parameters;
            // Initialize execution duration to timeout - this will be the case if the request times out, otherwise
            // the value will be overwritten when the server message is parsed out.
            this.ExecutionDuration = timeout;
            this.ExpirationTicks = (
                                     timeout == -1
                                   ? DateTime.UtcNow.AddDays(1)
                                   : DateTime.UtcNow.AddMilliseconds(timeout)
                                   ).Ticks;
        }

        /// <summary>
        /// Finalize the response by deferring to the strongly-typed sub-classes to trigger the response callback.
        /// </summary>
        protected abstract void OnExecutionComplete();

        /// <summary>
        /// Notify watchers of the WaitHandle that the request has completed (synchronously or not).  Abstract method
        /// for deferred strongly-typed implementation in child AsyncResponse[T] class.
        /// </summary>
        /// <param name="completedSynchronously">Whether the execution completed synchronously (only true if we
        /// encountered a failure during preparation).</param>
        protected abstract void OnNotifyCompleted(bool completedSynchronously);

        /// <summary>
        /// Flag indicating whether the response has an associated completion callback.  Provided as a non-generic hook
        /// for type-less calls.
        /// </summary>
        protected abstract bool HasCallback { get; }

        /// <summary>
        /// Parse the server response and trigger the callback.
        /// </summary>
        /// <param name="message">The byte array of the server response.</param>
        internal void OnExecutionComplete(byte[] message)
        {
            this.ParseResponse(message);
            this._Status = this.Exception == null ? ResponseStatus.Success : ResponseStatus.Failed;
            this.OnExecutionComplete();
            if (!this.HasCallback)
                this.OnNotifyCompleted(false);
        }

        /// <summary>
        /// Flag the response as having timed out and trigger the callback.
        /// </summary>
        internal void OnExecutionTimeout()
        {
            this.Exception = new VoltClientTimeoutException(Resources.ExecutionTimeout, this.ExecutionDuration);
            this._Status = ResponseStatus.Timedout;
            this.OnExecutionComplete();
            if (!this.HasCallback)
                this.OnNotifyCompleted(false);
        }

        /// <summary>
        /// Flag the response as having been aborted and trigger the callback.
        /// </summary>
        internal void OnExecutionAbort()
        {
            this.ExecutionDuration -= (int)(new TimeSpan(this.ExpirationTicks - DateTime.UtcNow.Ticks).TotalMilliseconds);
            this.Exception = new VoltClientAbortException(Resources.ExecutionClientAbort, this.ExecutionDuration);
            this._Status = ResponseStatus.Aborted;
            this.OnExecutionComplete();
            if (!this.HasCallback)
                this.OnNotifyCompleted(false);
        }

        /// <summary>
        /// Flag the response as having failed during the request initiation (for instance, trying to run a query on a
        /// closed connection).
        /// </summary>
        /// <param name="exception">The exception captured during the request preparation.</param>
        internal void OnExecutionRequestFailed(Exception exception)
        {
            this.ExecutionDuration = 0;
            this.Exception = exception;
            this._Status = ResponseStatus.Failed;
            this.OnExecutionComplete();
            this.OnNotifyCompleted(true);
        }
    }
}