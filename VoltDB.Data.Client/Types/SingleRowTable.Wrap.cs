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

namespace VoltDB.Data.Client
{
    /// <summary>
    /// General implementation of a generic Table.
    /// </summary>
    public partial class SingleRowTable
    {

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 2 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2> Wrap<T1, T2>()
        {
            return new SingleRowTable<T1, T2>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 3 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3> Wrap<T1, T2, T3>()
        {
            return new SingleRowTable<T1, T2, T3>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 4 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4> Wrap<T1, T2, T3, T4>()
        {
            return new SingleRowTable<T1, T2, T3, T4>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 5 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5> Wrap<T1, T2, T3, T4, T5>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 6 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6> Wrap<T1, T2, T3, T4, T5, T6>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 7 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7> Wrap<T1, T2, T3, T4, T5, T6, T7>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 8 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8> Wrap<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 9 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 10 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 11 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 12 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 13 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 14 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 15 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 16 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 17 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 18 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 19 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 20 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 21 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 22 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 23 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 24 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 25 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 26 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 27 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
        /// <typeparam name="T27">Type of column 27 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 28 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
        /// <typeparam name="T27">Type of column 27 of the underlying table.</typeparam>
        /// <typeparam name="T28">Type of column 28 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 29 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
        /// <typeparam name="T27">Type of column 27 of the underlying table.</typeparam>
        /// <typeparam name="T28">Type of column 28 of the underlying table.</typeparam>
        /// <typeparam name="T29">Type of column 29 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 30 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
        /// <typeparam name="T27">Type of column 27 of the underlying table.</typeparam>
        /// <typeparam name="T28">Type of column 28 of the underlying table.</typeparam>
        /// <typeparam name="T29">Type of column 29 of the underlying table.</typeparam>
        /// <typeparam name="T30">Type of column 30 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 31 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
        /// <typeparam name="T27">Type of column 27 of the underlying table.</typeparam>
        /// <typeparam name="T28">Type of column 28 of the underlying table.</typeparam>
        /// <typeparam name="T29">Type of column 29 of the underlying table.</typeparam>
        /// <typeparam name="T30">Type of column 30 of the underlying table.</typeparam>
        /// <typeparam name="T31">Type of column 31 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 32 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
        /// <typeparam name="T27">Type of column 27 of the underlying table.</typeparam>
        /// <typeparam name="T28">Type of column 28 of the underlying table.</typeparam>
        /// <typeparam name="T29">Type of column 29 of the underlying table.</typeparam>
        /// <typeparam name="T30">Type of column 30 of the underlying table.</typeparam>
        /// <typeparam name="T31">Type of column 31 of the underlying table.</typeparam>
        /// <typeparam name="T32">Type of column 32 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 33 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
        /// <typeparam name="T27">Type of column 27 of the underlying table.</typeparam>
        /// <typeparam name="T28">Type of column 28 of the underlying table.</typeparam>
        /// <typeparam name="T29">Type of column 29 of the underlying table.</typeparam>
        /// <typeparam name="T30">Type of column 30 of the underlying table.</typeparam>
        /// <typeparam name="T31">Type of column 31 of the underlying table.</typeparam>
        /// <typeparam name="T32">Type of column 32 of the underlying table.</typeparam>
        /// <typeparam name="T33">Type of column 33 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 34 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
        /// <typeparam name="T27">Type of column 27 of the underlying table.</typeparam>
        /// <typeparam name="T28">Type of column 28 of the underlying table.</typeparam>
        /// <typeparam name="T29">Type of column 29 of the underlying table.</typeparam>
        /// <typeparam name="T30">Type of column 30 of the underlying table.</typeparam>
        /// <typeparam name="T31">Type of column 31 of the underlying table.</typeparam>
        /// <typeparam name="T32">Type of column 32 of the underlying table.</typeparam>
        /// <typeparam name="T33">Type of column 33 of the underlying table.</typeparam>
        /// <typeparam name="T34">Type of column 34 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(this);
        }

        /// <summary>
        /// Returns a strongly-typed wrapper around the table instance.
        /// Use for a single-row table with 35 columns.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
        /// <typeparam name="T27">Type of column 27 of the underlying table.</typeparam>
        /// <typeparam name="T28">Type of column 28 of the underlying table.</typeparam>
        /// <typeparam name="T29">Type of column 29 of the underlying table.</typeparam>
        /// <typeparam name="T30">Type of column 30 of the underlying table.</typeparam>
        /// <typeparam name="T31">Type of column 31 of the underlying table.</typeparam>
        /// <typeparam name="T32">Type of column 32 of the underlying table.</typeparam>
        /// <typeparam name="T33">Type of column 33 of the underlying table.</typeparam>
        /// <typeparam name="T34">Type of column 34 of the underlying table.</typeparam>
        /// <typeparam name="T35">Type of column 35 of the underlying table.</typeparam>
        /// <returns>A strongly-typed table wrapper for the current table instance.</returns>
        public SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> Wrap<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>()
        {
            return new SingleRowTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(this);
        }
    }
}
