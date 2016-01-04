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
using System.Linq;
using System.Collections.Generic;

namespace VoltDB
{
    /// <summary>
    /// Defines extension methods to convert byte size values to user-friendly string formats.
    /// </summary>
    public static class FormatExtensions
    {
        /// <summary>
        /// Scales for byte size conversions.
        /// </summary>
        private static string[] ByteSizeScales = new string[] { " B", " KB", " MB", " GB", " TB", " PB", " EB", " ZB", " YB" };

        /// <summary>
        /// Default precision to use when formatting ByteSize strings
        /// </summary>
        private static int ByteSizeDefaultPrecision = 2;

        /// <summary>
        /// Converts a byte count provided as a [short] to a user-friendly string representation: x.xx KB, MB, etc.
        /// Uses the default precision: 2
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this short value) { return ToByteSizeString(value, ByteSizeDefaultPrecision); }

        /// <summary>
        /// Converts a byte count provided as a [short] to a user-friendly string representation: x.xx KB, MB, etc.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="precision">Desired precision.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this short value, int precision)
        {
            double pow = Math.Min(Math.Floor((value > 0 ? Math.Log(value) : 0) / Math.Log(1024)), ByteSizeScales.Length - 1);
            return ((double)value / Math.Pow(1024, pow)).ToString(pow == 0 ? "F0" : "F" + precision) + ByteSizeScales[(int)pow];
        }

        /// <summary>
        /// Converts a byte count provided as a [ushort] to a user-friendly string representation: x.xx KB, MB, etc.
        /// Uses the default precision: 2
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this ushort value) { return ToByteSizeString(value, ByteSizeDefaultPrecision); }

        /// <summary>
        /// Converts a byte count provided as a [ushort] to a user-friendly string representation: x.xx KB, MB, etc.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="precision">Desired precision.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this ushort value, int precision)
        {
            double pow = Math.Min(Math.Floor((value > 0 ? Math.Log(value) : 0) / Math.Log(1024)), ByteSizeScales.Length - 1);
            return ((double)value / Math.Pow(1024, pow)).ToString(pow == 0 ? "F0" : "F" + precision) + ByteSizeScales[(int)pow];
        }

        /// <summary>
        /// Converts a byte count provided as a [int] to a user-friendly string representation: x.xx KB, MB, etc.
        /// Uses the default precision: 2
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this int value) { return ToByteSizeString(value, ByteSizeDefaultPrecision); }

        /// <summary>
        /// Converts a byte count provided as a [int] to a user-friendly string representation: x.xx KB, MB, etc.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="precision">Desired precision.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this int value, int precision)
        {
            double pow = Math.Min(Math.Floor((value > 0 ? Math.Log(value) : 0) / Math.Log(1024)), ByteSizeScales.Length - 1);
            return ((double)value / Math.Pow(1024, pow)).ToString(pow == 0 ? "F0" : "F" + precision) + ByteSizeScales[(int)pow];
        }

        /// <summary>
        /// Converts a byte count provided as a [uint] to a user-friendly string representation: x.xx KB, MB, etc.
        /// Uses the default precision: 2
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this uint value) { return ToByteSizeString(value, ByteSizeDefaultPrecision); }

        /// <summary>
        /// Converts a byte count provided as a [uint] to a user-friendly string representation: x.xx KB, MB, etc.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="precision">Desired precision.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this uint value, int precision)
        {
            double pow = Math.Min(Math.Floor((value > 0 ? Math.Log(value) : 0) / Math.Log(1024)), ByteSizeScales.Length - 1);
            return ((double)value / Math.Pow(1024, pow)).ToString(pow == 0 ? "F0" : "F" + precision) + ByteSizeScales[(int)pow];
        }

        /// <summary>
        /// Converts a byte count provided as a [long] to a user-friendly string representation: x.xx KB, MB, etc.
        /// Uses the default precision: 2
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this long value) { return ToByteSizeString(value, ByteSizeDefaultPrecision); }

        /// <summary>
        /// Converts a byte count provided as a [long] to a user-friendly string representation: x.xx KB, MB, etc.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="precision">Desired precision.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this long value, int precision)
        {
            double pow = Math.Min(Math.Floor((value > 0 ? Math.Log(value) : 0) / Math.Log(1024)), ByteSizeScales.Length - 1);
            return ((double)value / Math.Pow(1024, pow)).ToString(pow == 0 ? "F0" : "F" + precision) + ByteSizeScales[(int)pow];
        }

        /// <summary>
        /// Converts a byte count provided as a [ulong] to a user-friendly string representation: x.xx KB, MB, etc.
        /// Uses the default precision: 2
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this ulong value) { return ToByteSizeString(value, ByteSizeDefaultPrecision); }

        /// <summary>
        /// Converts a byte count provided as a [ulong] to a user-friendly string representation: x.xx KB, MB, etc.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="precision">Desired precision.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this ulong value, int precision)
        {
            double pow = Math.Min(Math.Floor((value > 0 ? Math.Log(value) : 0) / Math.Log(1024)), ByteSizeScales.Length - 1);
            return ((double)value / Math.Pow(1024, pow)).ToString(pow == 0 ? "F0" : "F" + precision) + ByteSizeScales[(int)pow];
        }

        /// <summary>
        /// Converts a byte count provided as a [double] to a user-friendly string representation: x.xx KB, MB, etc.
        /// Uses the default precision: 2
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this double value) { return ToByteSizeString(value, ByteSizeDefaultPrecision); }

        /// <summary>
        /// Converts a byte count provided as a [double] to a user-friendly string representation: x.xx KB, MB, etc.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="precision">Desired precision.</param>
        /// <returns>Converted value.</returns>
        public static string ToByteSizeString(this double value, int precision)
        {
            double pow = Math.Min(Math.Floor((value > 0 ? Math.Log(value) : 0) / Math.Log(1024)), ByteSizeScales.Length - 1);
            return ((double)value / Math.Pow(1024, pow)).ToString(pow == 0 ? "F0" : "F" + precision) + ByteSizeScales[(int)pow];
        }

        /// <summary>
        /// Converts a list into a string representation.
        /// </summary>
        /// <typeparam name="T">Type of the enumerable to convert to a string representation (implicit from the type of
        /// the enumeration upon which the method is called)</typeparam>
        /// <param name="value">Enumeration upon which to perform the conversion.</param>
        /// <param name="format">Format string to use for each element in the collection.</param>
        /// <param name="separator">Separtor between elements.</param>
        /// <returns>String representation of the collection.</returns>
        /// <example>The following line prints out a byte buffer into a readable representation:
        /// <code>
        /// byte[] buffer;
        /// // Code initializing the buffer...
        /// // Would print something like: ff a0 d3 00 ...
        /// Console.WriteLine(buffer.ToSeparatedString("x2"," "));
        /// </code>
        /// </example>
        public static string ToSeparatedString<T>(this IEnumerable<T> value, string format, string separator)
        {
            format = "{0:" + format + "}";
            return string.Join(separator, value.Select(b => string.Format(format,b)).ToArray());
        }
    }
}