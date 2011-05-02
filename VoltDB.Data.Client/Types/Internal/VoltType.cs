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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides enumerations and conversion support between core VoltDB data types and basic .NET data types.
    /// </summary>
    internal static class VoltType
    {
        /// <summary>
        /// Tickcount timestamp origin for convertion (timestamps are stored as long/BIGINT).
        /// </summary>
        public const long TIMESTAMP_ORIGIN = 621355968000000000L;

        /// <summary>
        /// Indicator for null string in Table serializations.
        /// </summary>
        public const int NULL_STRING_INDICATOR = -1;

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
        /// List of .NET data types usable as data storage types in VoltDB - Enumeration of core .NET types as
        /// platform-insensitive (32/64bit) hashcodes (see method .GetTypeHash()).
        /// </summary>
        public enum NETType : int
        {
            Byte = 788535831,                 // System.Byte
            NullableByte = -1774142966,       // System.Nullable`1[System.Byte]
            SByte = 1323817117,               // System.SByte
            NullableSByte = 288254159,        // System.Nullable`1[System.SByte]
            Short = -2087907454,              // System.Int16
            NullableShort = 1585391270,       // System.Nullable`1[System.Int16]
            Int = -1689782088,                // System.Int32
            NullableInt = 1585653412,         // System.Nullable`1[System.Int32]
            Long = -904160537,                // System.Int64
            NullableLong = 1585260201,        // System.Nullable`1[System.Int64]
            Double = 248904818,               // System.Double
            NullableDouble = -299215054,      // System.Nullable`1[System.Double]
            DateTime = -902198056,            // System.DateTime
            NullableDateTime = -2002782677,   // System.Nullable`1[System.DateTime]
            String = 1501690144,              // System.String
            VoltDecimal = -611454561,         // VoltDB.Data.Client.VoltDecimal
            NullableVoltDecimal = 846517016   // System.Nullable`1[VoltDB.Data.Client.VoltDecimal]
        }

        /// <summary>
        /// List of .NET data types usable as parameter types in VoltDB - Enumeration of core .NET types as
        /// platform-insensitive (32/64bit) hashcodes (see method .GetTypeHash()
        /// </summary>
        public enum ParameterNETType : int
        {
            Byte = 788535831,                    // System.Byte
            ByteArray = -1273902537,             // System.Byte[]
            NullableByte = -1774142966,          // System.Nullable`1[System.Byte]
            NullableByteArray = 452265083,       // System.Nullable`1[System.Byte][]
            SByte = 1323817117,                  // System.SByte
            SByteArray = -485781331,             // System.SByte[]
            NullableSByte = 288254159,           // System.Nullable`1[System.SByte]
            NullableSByteArray = 812348791,      // System.Nullable`1[System.SByte][]
            Short = -2087907454,                 // System.Int16
            ShortArray = 363250082,              // System.Int16[]
            NullableShort = 1585391270,          // System.Nullable`1[System.Int16]
            NullableShortArray = -1415566668,    // System.Nullable`1[System.Int16][]
            Int = -1689782088,                   // System.Int32
            IntArray = 761375448,                // System.Int32[]
            NullableInt = 1585653412,            // System.Nullable`1[System.Int32]
            NullableIntArray = -1406915846,      // System.Nullable`1[System.Int32][]
            Long = -904160537,                   // System.Int64
            LongArray = 1546996999,              // System.Int64[]
            NullableLong = 1585260201,           // System.Nullable`1[System.Int64]
            NullableLongArray = -1419891951,     // System.Nullable`1[System.Int64][]
            Double = 248904818,                  // System.Double
            DoubleArray = -1608819823,           // System.Double[]
            NullableDouble = -299215054,         // System.Nullable`1[System.Double]
            NullableDoubleArray = -1988401784,   // System.Nullable`1[System.Double][]
            DateTime = -902198056,               // System.DateTime
            DateTimeArray = 130399106,           // System.DateTime[]
            NullableDateTime = -2002782677,      // System.Nullable`1[System.DateTime]
            NullableDateTimeArray = -1688737366, // System.Nullable`1[System.DateTime][]
            String = 1501690144,                 // System.String
            StringArray = 390706205,             // System.String[]
            VoltDecimal = -611454561,            // VoltDB.Data.Client.VoltDecimal
            VoltDecimalArray = 711446513,        // VoltDB.Data.Client.VoltDecimal[]
            NullableVoltDecimal = 846517016,     // System.Nullable`1[VoltDB.Data.Client.VoltDecimal]
            NullableVoltDecimalArray = 613378077 // System.Nullable`1[VoltDB.Data.Client.VoltDecimal][]
        }

        /// <summary>
        /// Maps .NET data types to VoltDB storage data types.
        /// </summary>
        public enum NETTypeToDBType : sbyte
        {
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
            NullableVoltDecimal = 22
        }

        static VoltType()
        {
            _NetTypeTypes = new Type[] {
                typeof(System.Byte),
                typeof(System.Nullable<System.Byte>),
                typeof(System.SByte),
                typeof(System.Nullable<System.SByte>),
                typeof(System.Int16),
                typeof(System.Nullable<System.Int16>),
                typeof(System.Int32),
                typeof(System.Nullable<System.Int32>),
                typeof(System.Int64),
                typeof(System.Nullable<System.Int64>),
                typeof(System.Double),
                typeof(System.Nullable<System.Double>),
                typeof(System.DateTime),
                typeof(System.Nullable<System.DateTime>),
                typeof(System.String),
                typeof(VoltDB.Data.Client.VoltDecimal),
                typeof(System.Nullable<VoltDB.Data.Client.VoltDecimal>),
            };

            _toNETTypeCache =
               _NetTypeTypes
               .Select(t => new { K = t, V = _ToNETTypeInternal(t) })
               .ToDictionary(x => x.K, x => x.V);
            _toDbTypeCache =
                _NetTypeTypes
                .Select(t => new { K = t, V = _ToDBTypeInternal(t) })
                .ToDictionary(x => x.K, x => x.V);
        }

        static readonly Type[] _NetTypeTypes;
        static readonly System.Collections.Generic.Dictionary<Type, DBType> _toDbTypeCache;
        static readonly Dictionary<Type, NETType> _toNETTypeCache;

        /// <summary>
        /// Validates the given .NET data type as corresponding to a valid VoltDB storage data type.
        /// </summary>
        /// <param name="t">The .NET type to validate.</param>
        /// <returns>The NETType enumeration value corresponding to this .NET data type.</returns>
        public static NETType ToNETType(Type t)
        {
            NETType res;
            if (_toNETTypeCache.TryGetValue(t, out res)) return res;
            throw new VoltUnsupportedTypeException(Resources.UnsupportedNETTypeToDBType, t.ToString());
        }

        public static NETType _ToNETTypeInternal(Type t)
        {
            int hash = GetTypeHash(t);
            if (!Enum.IsDefined(typeof(NETType), hash))
                throw new VoltUnsupportedTypeException(Resources.UnsupportedNETTypeToDBType, t.ToString());
            return (NETType)hash;
        }
        
        /// <summary>
        /// Validates the given .NET data type as corresponding to a valid VoltDB storage data type, and return the
        /// enumeration value of the underlying VoltDB storage data type.
        /// </summary>
        /// <param name="t">The .NET type to validate.</param>
        /// <returns>The DBType enumeration value corresponding to this .NET data type.</returns>
        public static DBType ToDBType(Type t)
        {
            DBType res;
            if (_toDbTypeCache.TryGetValue(t, out res)) return res;
            throw new VoltUnsupportedTypeException(Resources.UnsupportedNETTypeToDBType, t.ToString());
        }

        public static DBType _ToDBTypeInternal(Type t)
        {
            return (DBType)Enum.Parse(typeof(NETTypeToDBType), ((NETType)GetTypeHash(t)).ToString());
        }

        /// <summary>
        /// Returns the default .NET Type (as an actual Type object) for a given VoltDB storage data type. Multiple
        /// types might be compatible with a given VoltDB storage type, for instance, a SMALLINT can be converted as a
        /// short? (nullable short, the default), or a short.
        /// </summary>
        /// <param name="dbType">The VoltDB storage data type.</param>
        /// <returns>The default .NET data type.</returns>
        public static Type ToDefaultNetType(DBType dbType)
        {
            switch (dbType)
            {
                case DBType.TINYINT: return typeof(sbyte?);
                case DBType.SMALLINT: return typeof(short?);
                case DBType.INTEGER: return typeof(int?);
                case DBType.BIGINT: return typeof(long?);
                case DBType.FLOAT: return typeof(double?);
                case DBType.TIMESTAMP: return typeof(DateTime?);
                case DBType.STRING: return typeof(string);
                case DBType.DECIMAL: return typeof(VoltDecimal?);
                default: throw new VoltUnsupportedTypeException(Resources.UnsupportedDBType, dbType);
            }
        }

        /// <summary>
        /// Returns the default .NET array type (as an actual Type object) for a given VoltDB storage data type.
        /// Multiple types might be compatible with a given VoltDB storage type, for instance, a SMALLINT can be
        /// converted as a short? (nullable short, the default), or a short.
        /// </summary>
        /// <param name="dbType">The VoltDB storage data type.</param>
        /// <returns>The default .NET array data type.</returns>
        public static Type ToDefaultNetArrayType(DBType dbType)
        {
            switch (dbType)
            {
                case DBType.TINYINT: return typeof(sbyte?[]);
                case DBType.SMALLINT: return typeof(short?[]);
                case DBType.INTEGER: return typeof(int?[]);
                case DBType.BIGINT: return typeof(long?[]);
                case DBType.FLOAT: return typeof(double?[]);
                case DBType.TIMESTAMP: return typeof(DateTime?[]);
                case DBType.STRING: return typeof(string[]);
                case DBType.DECIMAL: return typeof(VoltDecimal?[]);
                default: throw new VoltUnsupportedTypeException(Resources.UnsupportedDBType, dbType);
            }
        }

        /// <summary>
        /// Returns a platform-insensitive hashcode for a data type.  This method uses the fast, 'unsafe' algorithm for
        /// string HashCode generation (with a specific fix to make the hash value platform insensitive (32/64bits),
        /// by specifically using an Int32* instead of an InPtr for which registry size varies, as in the default
        /// framework implementation.  Hash value comparison in switch/case statement is approximately 5-10% faster
        /// than constant string comparison (itself faster than an if/else type-check statement), justifying such
        /// usage.
        /// </summary>
        /// <param name="t">The type object for which we want a has value.</param>
        /// <returns>A platform-insensitive integer hash for the type.</returns>
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        private static unsafe int GetTypeHash(Type t)
        {
            string s = t.ToString();
            fixed (char* str = s)
            {
                char* chPtr = str;
                int num = 352654597;
                int num2 = num;
                Int32* numPtr = (Int32*)chPtr;
                for (int i = s.Length; i > 0; i -= 4)
                {
                    num = (((num << 5) + num) + (num >> 27)) ^ numPtr[0];
                    if (i <= 2)
                    {
                        break;
                    }
                    num2 = (((num2 << 5) + num2) + (num2 >> 27)) ^ numPtr[1];
                    numPtr += 2;
                }
                return (num + (num2 * 1566083941));
            }
        }
    }
}