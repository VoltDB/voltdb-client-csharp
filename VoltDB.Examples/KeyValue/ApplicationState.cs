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
using System.Text;

namespace VoltDB.Examples.KeyValue
{
    /// <summary>
    /// Holds the parameters and state of the application into a single object
    /// </summary>
    internal class ApplicationState
    {
        /// <summary>
        /// Enumeration of available value data encodings
        /// </summary>
        public enum DataEncoding : byte
        {
            Raw = 1,
            Gzip = 3,
            Deflate = 4
        }

        // Parameters
        public readonly int KeySize;
        public readonly int MinValueSize;
        public readonly int MaxValueSize;
        public readonly int InitialDatasetSize;
        public readonly double PercentGet;
        public readonly DataEncoding ValueEncoding;
        public readonly int BenchmarkDuration;
        public readonly double ValueTargetCompressionRatio;

        // Counters
        public long PutCount = 0;
        public long PutBytesUncompressed = 0;
        public long PutBytesCompressed = 0;
        public long GetCount = 0;
        public long GetBytesUncompressed = 0;
        public long GetBytesCompressed = 0;

        // Support variables
        public readonly byte[] BaseValue;
        public readonly Random Rand = new Random();

        /// <summary>
        /// Initializes the application state object from the provided program parameters
        /// </summary>
        /// <param name="keySize">Key Size</param>
        /// <param name="minValueSize">Minimum value size</param>
        /// <param name="maxValueSize">Maximum value size</param>
        /// <param name="initialDatasetSize">Initial number of key/value pairs</param>
        /// <param name="percentGet">Percent GETs (vs PUTs) for the benchmark</param>
        /// <param name="dataEncoding">Data encoding</param>
        /// <param name="benchmarkDuration">Benchmark duration</param>
        public ApplicationState(int keySize, int minValueSize, int maxValueSize, int initialDatasetSize, double percentGet, DataEncoding valueEncoding, double valueTargetCompressionRatio, int benchmarkDuration)
        {
            this.KeySize = keySize;
            this.MinValueSize = minValueSize;
            this.MaxValueSize = maxValueSize;
            this.InitialDatasetSize = initialDatasetSize;
            this.PercentGet = percentGet;
            this.ValueEncoding = valueEncoding;
            this.ValueTargetCompressionRatio = valueTargetCompressionRatio;
            this.BenchmarkDuration = benchmarkDuration;

            // Define raw data
            if (valueEncoding == DataEncoding.Raw)
                this.BaseValue = Encoding.UTF8.GetBytes(string.Empty.PadLeft(this.MaxValueSize, 'v'));
            else
            {
                // Build a base value that will provide (approximately) the desired compression rate
                for (int i = 1; i < this.MaxValueSize + 1; i = i + this.MaxValueSize / 1000)
                {
                    this.BaseValue = new byte[this.MaxValueSize];
                    byte[] pattern = new byte[i];
                    this.Rand.NextBytes(pattern);
                    Buffer.BlockCopy(pattern, 0, this.BaseValue, 0, pattern.Length);
                    int compressedSize = (
                                           valueEncoding == DataEncoding.Deflate
                                         ? this.BaseValue.Deflate()
                                         : this.BaseValue.Gzip()
                                         ).Length;
                    if (1.0 - ((double)compressedSize / (double)this.MaxValueSize) < valueTargetCompressionRatio)
                        break;
                }
            }
        }
    }
}