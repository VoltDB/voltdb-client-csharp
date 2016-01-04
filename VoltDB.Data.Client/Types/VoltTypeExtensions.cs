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

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides extension methods on core .NET data type for easy data checking and conversions with the native VoltDB
    /// data types.
    /// </summary>
    public static class VoltDBTypeExtensions
    {
        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this sbyte value)
        {
            return value == VoltType.NULL_TINYINT;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this sbyte? value)
        {
            return !value.HasValue || value == VoltType.NULL_TINYINT;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this byte value)
        {
            return value == unchecked((byte)VoltType.NULL_TINYINT);
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this byte? value)
        {
            return !value.HasValue || value == unchecked((byte)VoltType.NULL_TINYINT);
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this short value)
        {
            return value == VoltType.NULL_SMALLINT;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this short? value)
        {
            return !value.HasValue || value == VoltType.NULL_SMALLINT;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this int value)
        {
            return value == VoltType.NULL_INTEGER;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this int? value)
        {
            return !value.HasValue || value == VoltType.NULL_INTEGER;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this long value)
        {
            return value == VoltType.NULL_BIGINT;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this long? value)
        {
            return !value.HasValue || value == VoltType.NULL_BIGINT;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this double value)
        {
            return value == VoltType.NULL_FLOAT;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this double? value)
        {
            return !value.HasValue || value == VoltType.NULL_FLOAT;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this DateTime value)
        {
            return value.ToVoltDBLong() == VoltType.NULL_BIGINT;
        }

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public static bool IsVoltDBNull(this DateTime? value)
        {
            return !value.HasValue || value.ToVoltDBLong() == VoltType.NULL_BIGINT;
        }

        /// <summary>
        /// Converts a DateTime to a VoltDB long microsecond value.
        /// </summary>
        /// <param name="value">The date/time to convert.</param>
        /// <returns>The long value as used for storage in a VoltDB database.</returns>
        public static long ToVoltDBLong(this DateTime value)
        {
            return (((DateTime)value).ToUniversalTime().Ticks - VoltType.TIMESTAMP_ORIGIN) / 10L;
        }

        /// <summary>
        /// Converts a DateTime to a VoltDB long microsecond value.
        /// </summary>
        /// <param name="value">The date/time to convert.</param>
        /// <returns>The long value as used for storage in a VoltDB database.</returns>
        public static long ToVoltDBLong(this DateTime? value)
        {
            return (value == null)
                   ? VoltType.NULL_BIGINT
                   : (((DateTime)value).ToUniversalTime().Ticks - VoltType.TIMESTAMP_ORIGIN) / 10L;
        }

        /// <summary>
        /// Converts a long into a DateTime (assuming the long itself represents a VoltDB timestamp, as stored in a
        /// long format, in microseconds since the UNIX Epoch).
        /// </summary>
        /// <param name="value">The long value to convert.</param>
        /// <returns>The .NET-friendly DateTime object corresponding to the VoltDB time-stamp.</returns>
        public static DateTime? ToVoltDDateTime(this long value)
        {
            return new DateTime(value * 10 + VoltType.TIMESTAMP_ORIGIN, DateTimeKind.Utc);
        }
    }
}