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
using System.Linq;
using System.Runtime.ConstrainedExecution;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client {
    /// <summary>
    /// Provides enumerations and conversion support between core VoltDB data types and basic .NET data types.
    /// </summary>
    internal static class VoltType {
        /// <summary>
        /// Tickcount timestamp origin for convertion (timestamps are stored as long/BIGINT).
        /// </summary>
        public const long TIMESTAMP_ORIGIN = 621355968000000000L;

        /// <summary>
        /// Indicator for null string/varbinary in Table serializations.
        /// </summary>
        public const int NULL_STRING_AND_VARBINARY_INDICATOR = -1;

        /// <summary>
        /// Maximum string value length (or byte array length).
        /// </summary>
        public const int MAX_VALUE_LENGTH = 1048576;

        /// <summary>
        /// Indicator for null TINYINT values.
        /// </summary>
        public const sbyte NULL_TINYINT = sbyte.MinValue;

        /// <summary>
        /// Indicator for null SMALLINT values.
        /// </summary>
        public const short NULL_SMALLINT = short.MinValue;

        /// <summary>
        /// Indicator for null INTEGER values.
        /// </summary>
        public const int NULL_INTEGER = int.MinValue;

        /// <summary>
        /// Indicator for null BIGINT values.
        /// </summary>
        public const long NULL_BIGINT = long.MinValue;

        /// <summary>
        /// Indicator for null FLOAT values.
        /// </summary>
        public const double NULL_FLOAT = -1.7E+308d;

        /// <summary>
        /// Indicator for array data types (not for data storage - only for message serialization).
        /// </summary>
        public const sbyte ARRAY = -99;

        /// <summary>
        /// Maps .NET data types to VoltDB storage data types.
        /// </summary>
        public enum NETTypeToDBType : sbyte {
            Byte = 3,              // TINYINT
            NullableByte = 3,
            SByte = 3,
            NullableSByte = 3,
            Short = -4,            // SMALLINT
            NullableShort = 4,
            Int = 5,               // INTEGER
            NullableInt = 5,
            Long = 6,              // BIGINT
            NullableLong = 6,
            Double = 8,            // FLOAT
            NullableDouble = 8,
            DateTime = 11,         // TIMESTAMP
            NullableDateTime = 11,
            String = 9,            // STRING
            VoltDecimal = 22,      // DECIMAL
            NullableVoltDecimal = 22,
            Varbinary = 25         // VARBINARY
        }

        internal enum NetType {
            Byte, ByteN, SByte, SByteN, Int16, Int16N, Int32, Int32N, Int64, Int64N, Double, DoubleN, DateTime, DateTimeN,
            String, VoltDecimal, VoltDecimalN, Decimal, DecimalN,
            ByteArray
        }

        public class DateTimeDBNull { }
        public class StringDBNull { }
        public class ByteArrayDBNull { }

        static object nullable<T>(T x) where T : struct { return new Nullable<T>(x); }

        static VoltType() {
            _toDbTypeCache = new Dictionary<Type, DBType>(){
                {typeof(Byte),      DBType.TINYINT},
                {typeof(Byte?),     DBType.TINYINT},
                {typeof(SByte),     DBType.TINYINT},
                {typeof(SByte?),    DBType.TINYINT},
                {typeof(Int16),     DBType.SMALLINT},
                {typeof(Int16?),    DBType.SMALLINT},
                {typeof(Int32),     DBType.INTEGER},
                {typeof(Int32?),    DBType.INTEGER},
                {typeof(Int64),     DBType.BIGINT},
                {typeof(Int64?),    DBType.BIGINT},
                {typeof(Double),    DBType.FLOAT},
                {typeof(Double?),   DBType.FLOAT},
                {typeof(DateTime),  DBType.TIMESTAMP},
                {typeof(DateTime?), DBType.TIMESTAMP},
                {typeof(String),    DBType.STRING},
                {typeof(VoltDecimal),   DBType.DECIMAL},
                {typeof(VoltDecimal?),  DBType.DECIMAL},
                {typeof(Decimal),   DBType.DECIMAL},
                {typeof(Decimal?),  DBType.DECIMAL},
                {typeof(byte[]),    DBType.VARBINARY},
            };
            _toNetTypeCache = new Dictionary<Type, NetType>() {
                {typeof(Byte),      NetType.Byte},
                {typeof(Byte?),     NetType.ByteN},
                {typeof(SByte),     NetType.SByte},
                {typeof(SByte?),    NetType.SByteN},
                {typeof(Int16),     NetType.Int16},
                {typeof(Int16?),    NetType.Int16N},
                {typeof(Int32),     NetType.Int32},
                {typeof(Int32?),    NetType.Int32N},
                {typeof(Int64),     NetType.Int64},
                {typeof(Int64?),    NetType.Int64N},
                {typeof(Double),    NetType.Double},
                {typeof(Double?),   NetType.DoubleN},
                {typeof(DateTime),  NetType.DateTime},
                {typeof(DateTime?), NetType.DateTimeN},
                {typeof(String),    NetType.String},
                {typeof(VoltDecimal),   NetType.VoltDecimal},
                {typeof(VoltDecimal?),  NetType.VoltDecimalN},
                {typeof(Decimal),   NetType.Decimal},
                {typeof(Decimal?),  NetType.DecimalN},
                {typeof(byte[]),    NetType.ByteArray},
            };
            _nullValuessCache = new Dictionary<Type, object>() {
                 {typeof(Byte?),     nullable(NULL_TINYINT)},
                 {typeof(SByte?),    nullable(NULL_TINYINT)},
                 {typeof(Int16?),    nullable(NULL_SMALLINT)},
                 {typeof(Int32?),    nullable(NULL_INTEGER)},
                 {typeof(Int64?),    nullable(NULL_BIGINT)},
                 {typeof(Double?),   nullable(NULL_FLOAT)},
                 {typeof(DateTime?), new DateTimeDBNull()},
                 {typeof(String),    new StringDBNull()},
                 {typeof(VoltDecimal?), nullable(VoltDecimal.NullValue)},
                 {typeof(Decimal?),  nullable(VoltDecimal.NullValue)},
                 {typeof(byte[]),    new ByteArrayDBNull()},
            };
        }

        static readonly System.Collections.Generic.Dictionary<Type, DBType> _toDbTypeCache;
        static readonly Dictionary<Type, NetType> _toNetTypeCache;
        static readonly Dictionary<Type, object> _nullValuessCache;

        /// <summary>
        /// Validates the given .NET data type as corresponding to a valid VoltDB storage data type, and return the
        /// enumeration value of the underlying VoltDB storage data type.
        /// </summary>
        /// <param name="t">The .NET type to validate.</param>
        /// <returns>The DBType enumeration value corresponding to this .NET data type.</returns>
        public static DBType ToDBType(Type t) {
            DBType res;
            if (_toDbTypeCache.TryGetValue(t, out res)) return res;
            throw new VoltUnsupportedTypeException(Resources.UnsupportedNETTypeToDBType, t.ToString());
        }

        /// <summary>
        /// Returns the default .NET Type (as an actual Type object) for a given VoltDB storage data type. Multiple
        /// types might be compatible with a given VoltDB storage type, for instance, a SMALLINT can be converted as a
        /// short? (nullable short, the default), or a short.
        /// </summary>
        /// <param name="dbType">The VoltDB storage data type.</param>
        /// <returns>The default .NET data type.</returns>
        public static Type ToDefaultNetType(DBType dbType) {
            switch (dbType) {
                case DBType.TINYINT: return typeof(sbyte?);
                case DBType.SMALLINT: return typeof(short?);
                case DBType.INTEGER: return typeof(int?);
                case DBType.BIGINT: return typeof(long?);
                case DBType.FLOAT: return typeof(double?);
                case DBType.TIMESTAMP: return typeof(DateTime?);
                case DBType.STRING: return typeof(string);
                case DBType.DECIMAL: return typeof(VoltDecimal?);
                case DBType.VARBINARY: return typeof(byte[]);
                default: throw new VoltUnsupportedTypeException(Resources.UnsupportedDBType, dbType);
            }
        }

        public static NetType ToNetType(Type t) {
            NetType res;
            if (_toNetTypeCache.TryGetValue(t, out res)) return res;
            throw new VoltUnsupportedTypeException(Resources.UnsupportedNETTypeToDBType, t.ToString());
        }

        /// <summary>
        /// Returns the default .NET array type (as an actual Type object) for a given VoltDB storage data type.
        /// Multiple types might be compatible with a given VoltDB storage type, for instance, a SMALLINT can be
        /// converted as a short? (nullable short, the default), or a short.
        /// </summary>
        /// <param name="dbType">The VoltDB storage data type.</param>
        /// <returns>The default .NET array data type.</returns>
        public static Type ToDefaultNetArrayType(DBType dbType) {
            switch (dbType) {
                case DBType.TINYINT: return typeof(sbyte?[]);
                case DBType.SMALLINT: return typeof(short?[]);
                case DBType.INTEGER: return typeof(int?[]);
                case DBType.BIGINT: return typeof(long?[]);
                case DBType.FLOAT: return typeof(double?[]);
                case DBType.TIMESTAMP: return typeof(DateTime?[]);
                case DBType.STRING: return typeof(string[]);
                case DBType.DECIMAL: return typeof(VoltDecimal?[]);
                case DBType.VARBINARY: return typeof(byte[][]);
                default: throw new VoltUnsupportedTypeException(Resources.UnsupportedDBType, dbType);
            }
        }

        public static object CoalesceNull<T>(T x) {
            if (x != null) return x;
            object res;
            if (_nullValuessCache.TryGetValue(typeof(T), out res)) return res;
            throw new VoltUnsupportedTypeException(Resources.UnsupportedNETTypeToDBType, typeof(T).ToString());
        }
    }
}