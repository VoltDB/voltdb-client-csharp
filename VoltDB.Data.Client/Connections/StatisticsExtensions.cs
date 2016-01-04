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

using System.Collections.Generic;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provide extension methods for IEnumerable&lt;Statistics&gt;, allowing direct summarization (shorthand for
    /// using the static method on the Statistics class).
    /// </summary>
    public static class StatisticsExtensions
    {
        /// <summary>
        /// Aggregate a collection of staticstics counters.
        /// </summary>
        /// <param name="counters">Collection to aggregate.</param>
        /// <returns>Summary statistics.</returns>
        public static Statistics Summarize(this IEnumerable<Statistics> counters)
        {
            return Statistics.Summarize(counters);
        }

        /// <summary>
        /// Aggregate a collection of staticstics counters.
        /// </summary>
        /// <param name="counters">Collection to aggregate.</param>
        /// <returns>Summary statistics.</returns>
        public static Statistics Summarize(this IList<Statistics> counters)
        {
            return Statistics.Summarize(counters);
        }

        /// <summary>
        /// Aggregate a collection of staticstics counters.
        /// </summary>
        /// <param name="counters">Collection to aggregate.</param>
        /// <returns>Summary statistics.</returns>
        public static Statistics Summarize(this ICollection<Statistics> counters)
        {
            return Statistics.Summarize(counters);
        }
    }
}