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
    /// Enumeration of core VoltDB storage data types (with their identifying value in the wire protocol).
    /// </summary>
    public enum DBType : sbyte
    {
        /// <summary>
        /// TINYINT type: corresponds by default to sbyte? (Nullable SByte (Signed-Byte)).
        /// Interchangeable with .NET data types: sbyte?, sbyte, byte?, byte.
        /// </summary>
        TINYINT = 3,

        /// <summary>
        /// SMALLINT type: corresponds by default to short? (Nullable Int16/short).
        /// Interchangeable with .NET data types: short?, short.
        /// </summary>
        SMALLINT = 4,

        /// <summary>
        /// INTEGER type: corresponds by default to int? (Nullable Int32/int).
        /// Interchangeable with .NET data types: int?, int.
        /// </summary>
        INTEGER = 5,

        /// <summary>
        /// BIGINT type: corresponds by default to long? (Nullable Int64/long).
        /// Interchangeable with .NET data types: long?, long.
        /// </summary>
        BIGINT = 6,

        /// <summary>
        /// FLOAT type: corresponds by default to double? (Nullable Double).
        /// Interchangeable with .NET data types: double?, double.
        /// </summary>
        FLOAT = 8,

        /// <summary>
        /// TIMESTAMP type: corresponds by default to DateTime? (Nullable DateTime).
        /// Interchangeable with .NET data types: DateTime?, DateTime.
        /// </summary>
        TIMESTAMP = 11,

        /// <summary>
        /// STRING/VARCHAR type: corresponds to .NET datatype: string.
        /// </summary>
        STRING = 9,

        /// <summary>
        /// DECIMAL type: corresponds to custom .NET data type BigDecimal
        /// (currently unsupported in .NET - will resolve to byte[16] upon which no manipulation is available).
        /// </summary>
        DECIMAL = 22,

        /// <summary>
        /// VARBINARY type: corresponds to .NET datatype: byte[].
        /// </summary>
        VARBINARY = 25
    }
}