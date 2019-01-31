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
 
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides a thread-safe connection statistics tracking structure.  Allows monitoring of procedure call latency
    /// (or globally for an entire connection).
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// Marker for the time of the last reset (used to calculate elapsed time).
        /// </summary>
        private long _StartTick;

        /// <summary>
        /// Marker for the time of the last reset (used to calculate elapsed time).
        /// </summary>
        private long _EndTick;

        /// <summary>
        /// Number of requests posted to the cluster.
        /// </summary>
        private long _RequestCount;

        /// <summary>
        /// Number of responses received from the server.
        /// </summary>
        private long _ResponseCount;

        /// <summary>
        /// Number of failed responses (actual errors).
        /// </summary>
        private long _FailureCount;

        /// <summary>
        /// Number of timeout responses.
        /// </summary>
        private long _TimeoutCount;

        /// <summary>
        /// Number of aborted execution (client-side aborts only - aborted transaction on the server side will register
        /// as failures).
        /// </summary>
        private long _AbortCount;

        /// <summary>
        /// Minimum execution latency.
        /// </summary>
        private long _MinimumLatency;

        /// <summary>
        /// Maximum execution latency.
        /// </summary>
        private long _MaximumLatency;

        /// <summary>
        /// Total execution duration (used to calculate average latency).
        /// </summary>
        private long _TotalExecutionDuration;

        /// <summary>
        /// Latency bucket counters.
        /// Split in 9 values, the counters provide counts of executions where the latency is in the following buckets:
        ///  -   0 - 25ms
        ///  -  25 - 50ms
        ///  -  50 - 75ms
        ///  -  75 - 100ms
        ///  - 100 - 125ms
        ///  - 125 - 150ms
        ///  - 150 - 175ms
        ///  - 175 - 200ms
        ///  - 200ms and up
        /// </summary>
        private long[] _LatencyBucketCount;

        /// <summary>
        /// Number of bytes sent.
        /// </summary>
        private long _BytesSent;

        /// <summary>
        /// Number of bytes received.
        /// </summary>
        private long _BytesReceived;

        /// <summary>
        /// Number of threads currently capturing a snapshot of the statistics.
        /// </summary>
        private long SnapshotLock;

        /// <summary>
        /// Count of threads currently modifying the statistics.
        /// </summary>
        private long WriteLock;

        /// <summary>
        /// Initialize a new conection statistics structure.
        /// </summary>
        public Statistics()
        {
            this._StartTick = Stopwatch.GetTimestamp();
            this._RequestCount = 0;
            this._ResponseCount = 0;
            this._FailureCount = 0;
            this._TimeoutCount = 0;
            this._AbortCount = 0;
            this._MinimumLatency = long.MaxValue;
            this._MaximumLatency = long.MinValue;
            this._TotalExecutionDuration = 0;
            this._LatencyBucketCount = new long[9];
            this._BytesSent = 0;
            this._BytesReceived = 0;
            this._EndTick = 0;
            this.SnapshotLock = 0;
            this.WriteLock = 0;
        }

        /// <summary>
        /// Resets the counters to their original startup state.
        /// </summary>
        internal void Reset()
        {
            // Wait for snapshot to complete.
            while (this.SnapshotLock > 0) ;

            try
            {
                // Increment the write lock so the snapshot process knows to wait.
                Interlocked.Increment(ref this.WriteLock);

                Interlocked.Exchange(ref this._StartTick, Stopwatch.GetTimestamp());
                Interlocked.Exchange(ref this._EndTick, 0);
                Interlocked.Exchange(ref this._RequestCount, 0);
                Interlocked.Exchange(ref this._ResponseCount, 0);
                Interlocked.Exchange(ref this._FailureCount, 0);
                Interlocked.Exchange(ref this._TimeoutCount, 0);
                Interlocked.Exchange(ref this._AbortCount, 0);
                Interlocked.Exchange(ref this._MinimumLatency, long.MaxValue);
                Interlocked.Exchange(ref this._MaximumLatency, long.MinValue);
                Interlocked.Exchange(ref this._TotalExecutionDuration, 0);
                for (byte i = 0; i < 9; i++)
                    Interlocked.Exchange(ref this._LatencyBucketCount[i], 0);
                Interlocked.Exchange(ref this._BytesSent, 0);
                Interlocked.Exchange(ref this._BytesReceived, 0);
            }
            finally
            {
                // Decrement the write lock.
                Interlocked.Decrement(ref this.WriteLock);
            }
        }

        /// <summary>
        /// Number of requests posted to the cluster.
        /// </summary>
        public long RequestCount
        {
            get
            {
                return Interlocked.Read(ref this._RequestCount);
            }
        }

        /// <summary>
        /// Number of responses received from the server.
        /// </summary>
        public long ResponseCount
        {
            get
            {
                return Interlocked.Read(ref this._ResponseCount);
            }
        }

        /// <summary>
        /// Number of failed responses (actual errors).
        /// </summary>
        public long FailureCount
        {
            get
            {
                return Interlocked.Read(ref this._FailureCount);
            }
        }

        /// <summary>
        /// Number of timeout responses.
        /// </summary>
        public long TimeoutCount
        {
            get
            {
                return Interlocked.Read(ref this._TimeoutCount);
            }
        }

        /// <summary>
        /// Number of aborted execution (client-side aborts only - aborted transaction on the server side will register
        /// as failures).
        /// </summary>
        public long AbortCount
        {
            get
            {
                return Interlocked.Read(ref this._AbortCount);
            }
        }

        /// <summary>
        /// Number of pending responses (count of recevied responses - count of sent requests).
        /// </summary>
        public long PendingResponseCount
        {
            get
            {
                return Interlocked.Read(ref this._RequestCount) - Interlocked.Read(ref this._ResponseCount);
            }
        }

        /// <summary>
        /// Minimum execution latency.
        /// </summary>
        public long MinimumLatency
        {
            get
            {
                return Interlocked.Read(ref this._MinimumLatency);
            }
        }

        /// <summary>
        /// Maximum execution latency.
        /// </summary>
        public long MaximumLatency
        {
            get
            {
                return Interlocked.Read(ref this._MaximumLatency);
            }
        }

        /// <summary>
        /// Returns the average execution latency.
        /// </summary>
        public double AverageLatency
        {
            get
            {
                long responseCount = Interlocked.Read(ref this._ResponseCount);
                return (double)Interlocked.Read(ref this._TotalExecutionDuration) / Math.Max(1d, (double)responseCount);
            }
        }

        /// <summary>
        /// Total execution duration (used to calculate average latency)
        /// </summary>
        public long TotalExecutionDuration
        {
            get
            {
                return Interlocked.Read(ref this._TotalExecutionDuration);
            }
        }

        /// <summary>
        /// Elapsed duration since the last reset (Stopwatch Ticks).  This will vary in time, except if this is a
        /// "Snapshot" instance for which the _EndTick property was set at the time of snapshot.
        /// </summary>
        public long ElapsedDurationTicks
        {
            get
            {

                return (this._EndTick == 0 ? Stopwatch.GetTimestamp() : this._EndTick)
                    - Interlocked.Read(ref this._StartTick);
            }
        }

        /// <summary>
        /// Elapsed duration since the last reset (ms).
        /// </summary>
        public long ElapsedDuration
        {
            get
            {
                return this.ElapsedDurationTicks * 1000 / Stopwatch.Frequency;
            }
        }

        /// <summary>
        /// Transactions per second.
        /// </summary>
        public double TPS
        {
            get
            {
                return (double)(this.ResponseCount * Stopwatch.Frequency)
                    / Math.Max(1d, (double)this.ElapsedDurationTicks);
            }
        }

        /// <summary>
        /// Latency bucket counters.
        /// Split in 9 values, the counters provide counts of executions where the latency is in the following buckets:
        ///  -   0 - 25ms
        ///  -  25 - 50ms
        ///  -  50 - 75ms
        ///  -  75 - 100ms
        ///  - 100 - 125ms
        ///  - 125 - 150ms
        ///  - 150 - 175ms
        ///  - 175 - 200ms
        ///  - 200ms and up.
        /// </summary>
        public long[] LatencyBucketCount
        {
            get
            {
                long[] latencyBucketCount = new long[9];
                for (byte i = 0; i < 9; i++)
                    latencyBucketCount[i] = Interlocked.Read(ref this._LatencyBucketCount[i]);
                return latencyBucketCount;
            }
        }

        /// <summary>
        /// Number of bytes sent.
        /// </summary>
        public long BytesSent
        {
            get
            {
                return Interlocked.Read(ref this._BytesSent);
            }
        }

        /// <summary>
        /// Number of bytes received.
        /// </summary>
        public long BytesReceived
        {
            get
            {
                return Interlocked.Read(ref this._BytesReceived);
            }
        }

        /// <summary>
        /// Increment the request count upon a new request being posted.
        /// </summary>
        internal void OpenRequest(long bytesSent)
        {
            // Ignore request if the counter has been frozen/snapshoted.
            if (this._EndTick > 0)
                return;

            // Wait for snapshot to complete
            while (this.SnapshotLock > 0) ;

            try
            {
                // Increment the write lock so the snapshot process knows to wait.
                Interlocked.Increment(ref this.WriteLock);

                // Increment request count.
                Interlocked.Increment(ref this._RequestCount);

                // Increment bytes sent.
                Interlocked.Add(ref this._BytesSent, bytesSent);
            }
            finally
            {
                // Decrement the write lock.
                Interlocked.Decrement(ref this.WriteLock);
            }
        }

        /// <summary>
        /// Update internal counters upon a response being received.
        /// </summary>
        /// <param name="executionDuration">Execution duration as reported by the server (or timeout value in case the
        /// execution timedout).</param>
        /// <param name="status">The status of the execution request's response.</param>
        /// <param name="bytesReceived">Size of the response, in bytes, for network I/O tracking.</param>
        internal void CloseRequest(int executionDuration, ResponseStatus status, long bytesReceived)
        {
            // Hmm... The server will sometimes report negative durations for very fast execution!?! Correct that.
            if (executionDuration < 0)
                executionDuration = 0;

            // Ignore request if the counter has been frozen/snapshoted.
            if (this._EndTick > 0)
                return;

            // Wait for snapshot to complete.
            while (this.SnapshotLock > 0) ;

            try
            {

                // Increment the write lock so the snapshot process knows to wait.
                Interlocked.Increment(ref this.WriteLock);

                // Increment response count.
                Interlocked.Increment(ref this._ResponseCount);

                // Increment appropriate counter for non-success executions.
                switch (status)
                {
                    case ResponseStatus.Failed:
                        Interlocked.Increment(ref this._FailureCount);
                        break;
                    case ResponseStatus.Timedout:
                        Interlocked.Increment(ref this._TimeoutCount);
                        break;
                    case ResponseStatus.Aborted:
                        Interlocked.Increment(ref this._AbortCount);
                        break;
                }

                // Update minimum latency.
                long initialMinimumLatency, newMinimumLatency;
                do
                {
                    initialMinimumLatency = Interlocked.Read(ref this._MinimumLatency);
                    newMinimumLatency = Math.Min(initialMinimumLatency, executionDuration);
                }
                while (initialMinimumLatency != Interlocked.CompareExchange(ref this._MinimumLatency, newMinimumLatency, initialMinimumLatency));

                // Update maximum latency.
                long initialMaximumLatency, newMaximumLatency;
                do
                {
                    initialMaximumLatency = Interlocked.Read(ref this._MaximumLatency);
                    newMaximumLatency = Math.Max(initialMaximumLatency, executionDuration);
                }
                while (initialMaximumLatency != Interlocked.CompareExchange(ref this._MaximumLatency, newMaximumLatency, initialMaximumLatency));

                // Increment total execution duration.
                Interlocked.Add(ref this._TotalExecutionDuration, executionDuration);

                // Increment latency bucket count.
                Interlocked.Increment(ref this._LatencyBucketCount[Math.Min(executionDuration / 25L, 8)]);

                // Increment bytes sent.
                Interlocked.Add(ref this._BytesReceived, bytesReceived);
            }
            finally
            {
                // Decrement the write lock.
                Interlocked.Decrement(ref this.WriteLock);
            }
        }

        /// <summary>
        /// Returns a string representation of the counter.
        /// </summary>
        /// <returns>Formatted string detailing the counter data.</returns>
        public override string ToString()
        {
            return this.ToString(Resources.DefaultStatisticsFormat);
        }

        /// <summary>
        /// Returns a string representation of the counter using one of the defined default formats.
        /// </summary>
        /// <param name="format">One of the standard formats defined in the library.</param>
        /// <returns>Formatted string detailing the counter data.</returns>
        public string ToString(StatisticsFormat format)
        {
            return this.ToString(
                                  format == StatisticsFormat.Default
                                ? Resources.DefaultStatisticsFormat
                                : Resources.DefaultStatisticsShortFormat
                                );
        }

        /// <summary>
        /// Provides a basic mean to custom-format the statistics counter (no fancy IFormatter, sorry!)
        /// 
        /// Parameter list:
        /// 
        ///  {0}  :: ElapsedDuration (as a TimeSpan)
        ///  {1}  :: RequestCount
        ///  {2}  :: ResponseCount
        ///  {3}  :: FailureCount
        ///  {4}  :: TimeoutCount
        ///  {5}  :: AbortCount
        ///  {6}  :: PendingResponseCount
        ///  {7}  :: TPS
        ///  {8}  :: MinimumLatency
        ///  {9}  :: AverageLatency
        ///  {10} :: MaximumLatency
        ///  {11} :: LatencyBucketCount[0]
        ///  {12} :: LatencyBucketCount[1]
        ///  {13} :: LatencyBucketCount[2]
        ///  {14} :: LatencyBucketCount[3]
        ///  {15} :: LatencyBucketCount[4]
        ///  {16} :: LatencyBucketCount[5]
        ///  {17} :: LatencyBucketCount[6]
        ///  {18} :: LatencyBucketCount[7]
        ///  {19} :: LatencyBucketCount[8]
        ///  {20} :: BytesSent (B, KB, MB, etc. - converted to friendly string representation)
        ///  {21} :: BytesReceived (B, KB, MB, etc. - converted to friendly string representation)
        ///  
        /// </summary>
        /// <param name="format">Custom format string to use for the string.Format call.</param>
        /// <returns>Formatted string detailing the counter data.</returns>
        public string ToString(string format)
        {
            return string.Format(
                                  format
                                , new TimeSpan(this.ElapsedDuration * 10000)
                                , this.RequestCount
                                , this.ResponseCount
                                , this.FailureCount
                                , this.TimeoutCount
                                , this.AbortCount
                                , this.PendingResponseCount
                                , this.TPS
                                , this.MinimumLatency
                                , this.AverageLatency
                                , this.MaximumLatency
                                , this.LatencyBucketCount[0]
                                , this.LatencyBucketCount[1]
                                , this.LatencyBucketCount[2]
                                , this.LatencyBucketCount[3]
                                , this.LatencyBucketCount[4]
                                , this.LatencyBucketCount[5]
                                , this.LatencyBucketCount[6]
                                , this.LatencyBucketCount[7]
                                , this.LatencyBucketCount[8]
                                , this.BytesSent.ToByteSizeString()
                                , this.BytesReceived.ToByteSizeString()
                                );
        }

        /// <summary>
        /// Creates a frozen snapshot of the statistics counter - this is the state under which counters will be
        /// provided by the connection.
        /// </summary>
        /// <returns>Deep copy of the source.</returns>
        internal Statistics Snapshot()
        {
            // The future copy
            Statistics copy = null;

            // Wait for writers to complete
            while (this.WriteLock > 0) ;

            // Wait for other snapshots to complete
            while (this.SnapshotLock > 0) ;

            try
            {
                // Increment the snapshot lock so writers know to wait
                Interlocked.Increment(ref this.SnapshotLock);

                // Make the copy - there are no writers on the object right now, so the snapshot should be consistent
                // (don't really need interlocked access either as it is, since we're in snapshot lock, but it doesn't
                // hurt to stay safe...)
                copy = new Statistics();
                copy._StartTick = Interlocked.Read(ref this._StartTick);
                copy._EndTick = copy._EndTick == 0 ? Stopwatch.GetTimestamp() : Interlocked.Read(ref this._EndTick);
                copy._RequestCount = Interlocked.Read(ref this._RequestCount);
                copy._ResponseCount = Interlocked.Read(ref this._ResponseCount);
                copy._FailureCount = Interlocked.Read(ref this._FailureCount);
                copy._TimeoutCount = Interlocked.Read(ref this._TimeoutCount);
                copy._AbortCount = Interlocked.Read(ref this._AbortCount);
                copy._MinimumLatency = Interlocked.Read(ref this._MinimumLatency);
                copy._MaximumLatency = Interlocked.Read(ref this._MaximumLatency);
                copy._TotalExecutionDuration = Interlocked.Read(ref this._TotalExecutionDuration);
                for (byte i = 0; i < 9; i++)
                    copy._LatencyBucketCount[i] = Interlocked.Read(ref this._LatencyBucketCount[i]);
                copy._BytesSent = Interlocked.Read(ref this._BytesSent);
                copy._BytesReceived = Interlocked.Read(ref this._BytesReceived);

                // If the counter was never updated, those will have startup values - fix that.
                if (copy._MinimumLatency == long.MaxValue) copy._MinimumLatency = 0;
                if (copy._MaximumLatency == long.MinValue) copy._MaximumLatency = 0;
            }
            finally
            {
                // Decrement the snapshot lock.
                Interlocked.Decrement(ref this.SnapshotLock);
            }
            return copy;
        }

        /// <summary>
        /// Aggregates the current statistics with another statistics counter.
        /// </summary>
        /// <param name="other">The other counter to aggregate with the current instance.</param>
        /// <returns>Current instance, summarized with the given statistics.</returns>
        public Statistics SummarizeWith(Statistics other)
        {
            if (other != null)
            {
                this._StartTick = Math.Min(this._StartTick, other._StartTick);
                this._EndTick = Math.Max(this._EndTick, other._EndTick);
                this._RequestCount += other._RequestCount;
                this._ResponseCount += other._ResponseCount;
                this._FailureCount += other._FailureCount;
                this._TimeoutCount += other._TimeoutCount;
                this._AbortCount += other._AbortCount;
                this._MinimumLatency = Math.Min(this._MinimumLatency, other._MinimumLatency);
                this._MaximumLatency = Math.Max(this._MaximumLatency, other._MaximumLatency);
                this._TotalExecutionDuration += other._TotalExecutionDuration;
                for (byte i = 0; i < 9; i++)
                    this._LatencyBucketCount[i] += other._LatencyBucketCount[i];
                this._BytesSent += other._BytesSent;
                this._BytesReceived += other._BytesReceived;
            }
            return this;
        }

        /// <summary>
        /// Aggregate multiple counters with the current instance.
        /// </summary>
        /// <param name="others">List of counters to aggregate to the current instance.</param>
        /// <returns>Current instance, summarized with the given statistics.</returns>
        public Statistics SummarizeWith(IEnumerable<Statistics> others)
        {
            return this.SummarizeWith(Statistics.Summarize(others));
        }

        /// <summary>
        /// Aggregates multiple counters together to provide a summary counter.
        /// </summary>
        /// <param name="counters">Enumeration of counters to aggregate.</param>
        /// <returns>Summary statistics.</returns>
        public static Statistics Summarize(IEnumerable<Statistics> counters)
        {
            Statistics result = null;
            foreach (Statistics other in counters)
                if (result == null)
                {
                    if (other != null)
                        result = other.Snapshot();
                }
                else
                    result.SummarizeWith(other);
            return result;
        }

        /// <summary>
        /// Freezes this statistics counter: further calls will be ignored and the elapsed time will have a finalized
        /// value (instead of a running time).
        /// </summary>
        internal void Freeze()
        {
            this._EndTick = Stopwatch.GetTimestamp();

            // If the counter was never updated, those will have startup values - fix that.
            if (this._MinimumLatency == long.MaxValue) this._MinimumLatency = 0;
            if (this._MaximumLatency == long.MinValue) this._MaximumLatency = 0;
        }
    }
}