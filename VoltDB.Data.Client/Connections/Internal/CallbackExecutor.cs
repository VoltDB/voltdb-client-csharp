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
 
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// A very simplistic, managed thread pool providing support for callback execution and offering the best
    /// performance/CPU footprint compromise.  An equivalent method would be to use the ThreadPool (as the first
    /// version of the library did), but forcing MaxWorkerThreads to MinWorkerThread, however, this would also
    /// impact all client aplications since the ThreadPool is unfortunally a shared object.
    /// </summary>
    public class CallbackExecutor
    {
        /// <summary>
        /// Simple workItem object holding a reference to a callback and the state object with which it will be called.
        /// </summary>
        internal class CallbackWorkItem
        {
            /// <summary>
            /// Callback to execute.
            /// </summary>
            public readonly WaitCallback Callback;

            /// <summary>
            /// State object to pass for execution.
            /// </summary>
            public readonly object State;

            /// <summary>
            /// Creates a new WorkItem instance
            /// </summary>
            /// <param name="callback">Callback to execute.</param>
            /// <param name="state">State object to pass for execution.</param>
            public CallbackWorkItem(WaitCallback callback, object state)
            {
                this.Callback = callback;
                this.State = state;
            }
        }

        /// <summary>
        /// Internal queue of workitems (pushed in by connection threads) that need to be executed
        /// </summary>
        private Queue<CallbackWorkItem> Queue = new Queue<CallbackWorkItem>();

        /// <summary>
        /// SyncRoot for locking around the internal workitem queue.
        /// </summary>
        private object SyncRoot = new object();

        /// <summary>
        /// Array of processing threads performing callback execution.
        /// </summary>
        private Thread[] Threads;

        /// <summary>
        /// Internal flag indicating whether the executor was started (so we don't incorrectly spawn more threads!)
        /// </summary>
        private bool Started = false;

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public CallbackExecutor() { }

        /// <summary>
        /// Start the executor, initiating the processing threads
        /// </summary>
        public void Start()
        {
            // If another connection already started the executor (case of cluster connections where the first
            // succesfully opened connection will call this method), return immediately.
            // We make sure the instance is locked first to avoid race conditions that could cause multiple executions
            // of this method!
            lock (this.SyncRoot)
            {
                if (this.Started)
                    return;

                // Calculate an optimal number of threads: 2 is the best CPU/performance compromise in most cases, however
                // if there are more CPUs on the machine, we can push this further.  We calculate this based on the global
                // .NET ThreadPool's own calculation (MinWorkerThread is the .NET-estimated number of usable CPUs: we use
                // this instead of our own calculation, with the intent to agree with the ThreadPool's understanding of
                // whether HyperThreading/Cores have an influence on optimal threading, instead of doing our own, different
                // calculation)
                int workerThreads;
                int completionPortThreads;
                ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
                int processingThreadCount = Math.Max(workerThreads - 3, 2); // -3 is for: 1 Client thread + at least 2 connection threads.
                Threads = new Thread[processingThreadCount];

                // Start the threads.
                for (int i = 0; i < processingThreadCount; i++)
                {
                    this.Threads[i] = new Thread(this.BackgroundThreadRun)
                    {
                        IsBackground = true,
                        Priority = ThreadPriority.Normal
                    };
                    this.Threads[i].Start();
                }

                this.Started = true;
            }
        }

        /// <summary>
        /// Stop the executor (upon connection closure).  This works a bit like the "Drain" method: we ensure all
        /// callbacks are triggered (just as with the old ThreadPool model) before effectively terminating all the
        /// threads.
        /// </summary>
        public void Stop()
        {
            // If another connection already terminated the executor, there's nothing to do anymore.  I'm just being
            // defensive: coding is such that in a cluster connection, the Cluster connection itself will call this,
            // only, so there should never be more than one call.
            if (!this.Started)
                return;

            // Wait until the queue has been entirely consumed
            int count = 1;
            while (count > 0)
            {
                count = 0;
                lock (this.SyncRoot)
                    count = this.Queue.Count;
                if (count > 0)
                    Thread.Sleep(100);
            }
            for (int i = 0; i < this.Threads.Length; i++)
            {
                this.Threads[i].Abort();
            }

        }
        /// <summary>
        /// Queue a callback for execution
        /// </summary>
        /// <param name="callback">The callback to execute.</param>
        /// <param name="state">The state object to pass for execution.</param>
        public void QueueWorkItem(WaitCallback callback, object state)
        {
            lock (this.SyncRoot)
                this.Queue.Enqueue(new CallbackWorkItem(callback, state));
        }

        /// <summary>
        /// Running process for the background thread that monitors time-out conditions and triggers callbacks.
        /// </summary>
        private void BackgroundThreadRun()
        {
            try
            {
                CallbackWorkItem item;
                while (true)
                {
                    item = null;
                    lock (this.SyncRoot)
                        if (this.Queue.Count > 0)
                            item = this.Queue.Dequeue();

                    // Sleep a while if we have nothing to do.  This might cause some delays in triggering of the
                    // callbacks, but at the benefit of lowering vastly the CPU footprint (and associated context
                    // switching)
                    if (item == null)
                        Thread.Sleep(100);
                    else
                    {
                        try
                        {
                            item.Callback(item.State);
                        }
                        catch { }
                    }
                }
            }
            // ThreadAborts will occur when the connection is closed or the application terminates - swallow them.
            // Everything else is protected (indeed twice since the AsyncCallback itself will have a try/finalize
            // block, so no other case of failure will lead us there (except things like out of memory exceptions
            // which will terminate the application.  The point: we don't need to provide any logic to restart those
            // threads because termination will be... final!)
            catch { }
        }
    }
}