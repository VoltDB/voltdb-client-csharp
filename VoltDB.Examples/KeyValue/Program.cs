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
using System.Linq;
using System.Text;
using System.Threading;
using VoltDB.Data.Client;

namespace VoltDB.Examples.KeyValue
{
    /// <summary>
    /// Class for the sample "KeyValue Benchmark".
    /// </summary>
    class KeyValueBenchmark
    {
        // Various format strings for console display.
        static string line = "\r\n----------------------------------------------------------------------------------------------------\r\n";
        static string introFormat = "{0}\r\n KeyValue Benchmark\r\n{0}\r\n                         Duration : {1,15}\r\n                         Key Size : {2,15}\r\n               Minimum Value Size : {3,15}\r\n               Maximum Value Size : {4,15}\r\nInitial Number of Key/Value Pairs : {5,15:##,#}\r\n     Percentage of GETS (vs PUTS) : {6,15}\r\n                    Data Encoding : {7,15}{8}\r\n\r\n{0}";
        static string resultFormat = "{0} Operations\r\n                  Calls : {1,10:##,#}\r\n           Uncompressed : {2,10}\r\n             Compressed : {3,10}\r\n       Compression Rate : {4,10:#0.## %}\r\n Avg. Uncompressed Size : {5,10}\r\n   Avg. Compressed Size : {6,10}\r\n";

        /// <summary>
        /// Defines a simple structure for tracking of value data size
        /// </summary>
        struct SizeInfo
        {
            /// <summary>
            /// Compressed size of the value.
            /// </summary>
            public readonly long CompressedSize;
            /// <summary>
            /// Uncompressed size of the value.
            /// </summary>
            public readonly long UncompressedSize;
            /// <summary>
            /// Creates a new instance of the SizeInfo structure.
            /// </summary>
            /// <param name="compressedSize">Compressed size of the value.</param>
            /// <param name="uncompressedSize">Uncompressed size of the value.</param>
            public SizeInfo(long compressedSize, long uncompressedSize)
            {
                this.CompressedSize = compressedSize;
                this.UncompressedSize = uncompressedSize;
            }
        }

        /// <summary>
        /// Tracking callback for PUT operations.
        /// </summary>
        /// <param name="response">The response received from the server.</param>
        public static void PutCallback(AsyncResponse<Null> response)
        {
            // Only track the Put if the request was successful!
            if (response.Status == ResponseStatus.Success)
            {
                // Retrieve the pair that was passed as state object for our callback
                SizeInfo info = (SizeInfo)response.AsyncState;

                // Increment tracking counters
                Interlocked.Increment(ref State.PutCount);
                Interlocked.Add(ref State.PutBytesCompressed, info.CompressedSize);
                Interlocked.Add(ref State.PutBytesUncompressed, info.UncompressedSize);
            }
        }

        /// <summary>
        /// Tracking callback for GET operations.
        /// </summary>
        /// <param name="response">The response received from the server.</param>
        public static void GetCallback(AsyncResponse<Table> response)
        {
            // Only track the Put if the request was successful!
            if (response.Status == ResponseStatus.Success)
            {
                // Unpack result to retrieve compressed and uncompressed value sizes and increment counters.
                byte[] data = response.Result.GetValue<byte[]>(1,0);
                long compressedSize = data.Length; // Encoding.UTF8.GetBytes(response.Result).LongLength;
                long uncompressedSize = 0;
                switch (State.ValueEncoding)
                {
                    case ApplicationState.DataEncoding.Gzip:
                        uncompressedSize = data.UnGzip().LongLength;
                        break;
                    case ApplicationState.DataEncoding.Deflate:
                        uncompressedSize = data.UnDeflate().LongLength;
                        break;
                    default:
                        uncompressedSize = compressedSize;
                        break;
                }
                // Increment tracking counters
                Interlocked.Increment(ref State.GetCount);
                Interlocked.Add(ref State.GetBytesCompressed, compressedSize);
                Interlocked.Add(ref State.GetBytesUncompressed, uncompressedSize);
            }
            else
            {
                Console.WriteLine(response.ServerStatusString);
            }
        }

        /// <summary>
        /// Application states (parameters, counters and support objects driving the application)
        /// </summary>
        static ApplicationState State;

        // Application entry-point
        static void Main(string[] args)
        {
            try
            {
                // Read hosts from the command or use defaults
                string hosts = "10.10.180.176";
                if (args.Length > 0)
                    hosts = string.Join(",", string.Join(",", args).Split(' ', ','));

                // Initialize application state
                State = new ApplicationState(
                          32                                 // Key size (250 maximum)
                        , 1024                               // Minimum Value Size (Max 1048576)
                        , 1024                               // Maximum Value Size (Max 1048576)
                        , 100000                             // Number of Pairs to put in store before benchmark
                        , 0.75                               // % of Get (vs Put) during benchmark
                        , ApplicationState.DataEncoding.Gzip // Type of data compression to use for benchmark
                        , 0.5                                // Value target compression ratio (only for Gzip and
                                                             // Deflate: Raw will not compress, and Base64 will
                                                             // by design cause a +33% payload increase)
                        , 120000                             // Benchmark duration
                        );

                // Print out introduction message
                Console.WriteLine(
                                   introFormat
                                 , line
                                 , new TimeSpan(0, 0, 0, 0, State.BenchmarkDuration)
                                 , State.KeySize
                                 , State.MinValueSize.ToByteSizeString()
                                 , State.MaxValueSize.ToByteSizeString()
                                 , State.InitialDatasetSize
                                 , State.PercentGet * 100
                                 , State.ValueEncoding.ToString()
                                 , (
                                      State.ValueEncoding == ApplicationState.DataEncoding.Deflate
                                   || State.ValueEncoding == ApplicationState.DataEncoding.Gzip
                                   )
                                   ? string.Format("\r\n   Approx. Value Compression Rate : {0,15:#0.## %}", State.ValueTargetCompressionRatio)
                                   : ""
                                 );


                // Create & open connection
                using (VoltConnection voltDB = VoltConnection.Create("hosts=" + hosts + ";statistics=true;adhoc=true;").Open())
                {
                    // Define procedures
                    var Initialize = voltDB.Procedures.Wrap<Null, int, int, string, byte[]>("Initialize", null);
                    var Put = voltDB.Procedures.Wrap<Null, string, byte[]>("Put", PutCallback);
                    var Get = voltDB.Procedures.Wrap<Table, string>("Get", GetCallback);

                    // Process variables
                    string key;
                    byte[] value;
                    byte[] compressedValue;

                    // Initialize the data
                    Console.Write("Initializing data store... ");
                    for (int i = 0; i < State.InitialDatasetSize; i += 1000)
                    {
                        // Generate value
                        if (State.MaxValueSize == State.MinValueSize)
                            value = State.BaseValue;
                        else
                        {
                            value = new byte[State.Rand.Next(State.MinValueSize, State.MaxValueSize)];
                            Buffer.BlockCopy(State.BaseValue, 0, value, 0, value.Length);
                        }

                        // Compress as needed
                        switch (State.ValueEncoding)
                        {
                            case ApplicationState.DataEncoding.Gzip:
                                compressedValue = value.Gzip();
                                break;
                            case ApplicationState.DataEncoding.Deflate:
                                compressedValue = value.Deflate();
                                break;
                            default:
                                compressedValue = value;
                                break;
                        }

                        Initialize.Execute(i, Math.Min(i + 1000, State.InitialDatasetSize), "K%1$#" + (State.KeySize - 1) + "s", value);
                    }
                    Console.WriteLine(" Done.");

                    // Initialize ticker
                    Timer ticker = new Timer(delegate(object state) { Console.WriteLine("{0,24}", string.Join("\r\n", (state as VoltConnection).Statistics.GetStatisticsSummaryByNode(StatisticsSnapshot.SnapshotOnly).Select(r => r.Key.ToString() + " :: " + r.Value.ToString(StatisticsFormat.Short)).ToArray())); }, voltDB, 5000, 5000);

                    // Now benchmark GET/PUT operations.
                    int end = Environment.TickCount + State.BenchmarkDuration;
                    Console.WriteLine("{0}\r\nRunning GET/PUT benchmark\r\n{0}", line);
                    while (Environment.TickCount < end)
                    {
                        // Get a random key to act upon.
                        key = State.Rand.Next(State.InitialDatasetSize).ToString().PadLeft(State.KeySize, 'k');

                        // Decide whether to do a Get/Put
                        if (State.Rand.NextDouble() <= State.PercentGet)
                            Get.BeginExecute(key);
                        else
                        {
                            // Generate value
                            if (State.MaxValueSize == State.MinValueSize)
                                value = State.BaseValue;
                            else
                            {
                                value = new byte[State.Rand.Next(State.MinValueSize, State.MaxValueSize)];
                                Buffer.BlockCopy(State.BaseValue, 0, value, 0, value.Length);
                            }

                            // Compress as needed
                            switch (State.ValueEncoding)
                            {
                                case ApplicationState.DataEncoding.Gzip:
                                    compressedValue = value.Gzip();
                                    break;
                                case ApplicationState.DataEncoding.Deflate:
                                    compressedValue = value.Deflate();
                                    break;
                                default:
                                    compressedValue = value;
                                    break;
                            }

                            // Push PUT call to server.  Note: Tracking ONLY performed upon successful completion of the call
                            Put.BeginExecute(key, compressedValue, new SizeInfo(compressedValue.LongLength, value.LongLength));
                        }
                    }

                    // Drain connection
                    voltDB.Drain();

                    // Dispose of ticket
                    ticker.Dispose();

                    Console.WriteLine("{0}\r\nConnection Statistics by Procedure.\r\n{0}", line);
                    // Print out statistics by procedure
                    Console.WriteLine(string.Join("\r\n", voltDB.Statistics.GetStatistics(StatisticsSnapshot.SnapshotOnly).Select(i => string.Format("{0,8} :: {1}", i.Key, i.Value.ToString(StatisticsFormat.Short))).ToArray()));

                    Console.WriteLine("{0}\r\nConnection Statistics.\r\n{0}", line);
                    // Print out statistics and reset (though pointless, since we'll close the connection next!)
                    Console.WriteLine(voltDB.Statistics.GetStatisticsSummary(StatisticsSnapshot.SnapshotAndResetWithIgnorePendingExecutions).ToString());

                    Console.WriteLine("{0}\r\nApplication Statistics.\r\n{0}", line);
                    // Print out application statistics
                    Console.WriteLine(resultFormat
                               , "GET"
                               , State.GetCount
                               , State.GetBytesUncompressed.ToByteSizeString()
                               , State.GetBytesCompressed.ToByteSizeString()
                               , (double)(State.GetBytesUncompressed - State.GetBytesCompressed) / (double)State.GetBytesUncompressed
                               , ((double)State.GetBytesUncompressed / (double)State.GetCount).ToByteSizeString()
                               , ((double)State.GetBytesCompressed / (double)State.GetCount).ToByteSizeString()
                               );

                    Console.WriteLine(resultFormat
                               , "PUT"
                               , State.PutCount
                               , State.PutBytesUncompressed.ToByteSizeString()
                               , State.PutBytesCompressed.ToByteSizeString()
                               , (double)(State.PutBytesUncompressed - State.PutBytesCompressed) / (double)State.PutBytesUncompressed
                               , ((double)State.PutBytesUncompressed / (double)State.PutCount).ToByteSizeString()
                               , ((double)State.PutBytesCompressed / (double)State.PutCount).ToByteSizeString()
                               );

                }

            }
            catch (Exception x)
            {
                // You'll want to be more elaborate than having a global catch block!
                Console.WriteLine(x.ToString());
            }
        }
    }
}