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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides a synchronized cache of execution processes.
    /// </summary>
    internal class ExecutionCache
    {
        /// <summary>
        /// Internal map of pending responses expected from the server.  Used to recall callbacks.
        /// </summary>
        private readonly Dictionary<long, Response> Cache;

        /// <summary>
        /// Access to the cache's SyncRoot
        /// </summary>
        private readonly object Lock;

        /// <summary>
        /// Internal cache size counter (faster to read/write to through Interlocked call than reading the dictionary).
        /// </summary>
        private long _Size = 0;

        /// <summary>
        /// Initializes a new instance of the ExecutionCache class.
        /// </summary>
        /// <param name="capacity">Initial capacity of the cache (corresponds to setting "MaxOutstandingTxns".</param>
        public ExecutionCache(int capacity)
        {
            this.Cache = new Dictionary<long, Response>(capacity);
            // Marginally faster done this way than doing the property lookup all the time.
            this.Lock = (this.Cache as IDictionary).SyncRoot;
        }

        /// <summary>
        /// Returns the size of the cache (used for connection draining).
        /// </summary>
        public long Size
        {
            get
            {
                return Interlocked.Read(ref this._Size);
            }
        }

        /// <summary>
        /// Push a new request to the internal stack where it will wait until timeout or the server responds.
        /// </summary>
        /// <param name="response">The prepared response state object to push into the stack.</param>
        public void AddItem(Response response)
        {
            Interlocked.Increment(ref this._Size);
            lock(this.Lock)
                this.Cache.Add(response.ExecutionId, response);
        }

        /// <summary>
        /// Removes a completed response from the stack and return it to the caller for finalization.
        /// </summary>
        /// <param name="executionId">The execution id of the response to remove.</param>
        /// <returns>The response object handed out for finalization.</returns>
        public Response BeginRemoveItem(long executionId)
        {
            lock(this.Lock)
            {
                Response response = null;
                if (this.Cache.TryGetValue(executionId, out response))
                        this.Cache.Remove(executionId);
                return response;
            }
        }

        /// <summary>
        /// Confirms the item's execution has been completed and the request can be considered to be over with (used
        /// to ensure that the drain function waits for ALL execution competions).
        /// </summary>
        public void EndRemoveItem()
        {
            Interlocked.Decrement(ref this._Size);
        }

        /// <summary>
        /// Returns a list of exection ids for items that should be kicked out (time out expired).
        /// </summary>
        /// <returns>List of expired execution ids.</returns>
        public List<long> GetExpiredItems()
        {
            var res = new List<long>();
            var now = DateTime.UtcNow.Ticks;
            lock (this.Lock)
            {
                foreach (var kv in this.Cache)
                {
                    var xticks = kv.Value.ExpirationTicks;
                    if (xticks <= now)
                    {
                        res.Add(kv.Key);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Returns a list of all exection ids currently in the cache.
        /// </summary>
        /// <returns>List of current execution ids.</returns>
        public long[] GetCurrentItems()
        {
            lock (this.Lock)
                return this.Cache.Select(r => r.Key).ToArray();
        }

    }
}