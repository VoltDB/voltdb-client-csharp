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
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Partial class implementation: Support for Single-Column Table extraction.
    /// </summary>
    public partial class TableBase
    {
        /// <summary>
        /// Returns a straight data array (boxed) of the requested data type from a single-column Table.
        /// </summary>
        /// <param name="input">The deserializer hoding the response data.</param>
        /// <param name="TResult">The desired output data type of the array elements.</param>
        /// <returns>A boxed array of elements of the requested data type.</returns>
        internal static object FromSingleColumn(Deserializer input, Type TResult)
        {
            // Skip table length, metadata length, status, get column count.
            short columnCount = input.Skip(9).ReadShort();

            // Validate there is indeed only one column.
            if (columnCount != 1)
                throw new VoltInvalidDataException(Resources.InvalidColumnCount, columnCount);

            // Read column data type.
            DBType columnType = (DBType)input.ReadSByte();

            // Validate the data type matches the .NET casting request.
            if (VoltType.ToDBType(TResult) != columnType)
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , columnType.ToString()
                                                  , TResult.ToString()
                                                  );

            // Skip column name, get Row count.
            int rowCount = input.SkipString().ReadInt();

            // Load data.
            switch (VoltType.ToNETType(TResult))
            {
                case VoltType.NETType.Byte:
                    byte[] dataByte = new byte[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataByte[r] = input.Skip(4).ReadByte();
                    return dataByte;

                case VoltType.NETType.NullableByte:
                    byte?[] dataNullableByte = new byte?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataNullableByte[r] = input.Skip(4).ReadNullableByte();
                    return dataNullableByte;

                case VoltType.NETType.SByte:
                    sbyte[] dataSByte = new sbyte[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataSByte[r] = input.Skip(4).ReadSByte();
                    return dataSByte;

                case VoltType.NETType.NullableSByte:
                    sbyte?[] dataNullableSByte = new sbyte?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataNullableSByte[r] = input.Skip(4).ReadNullableSByte();
                    return dataNullableSByte;

                case VoltType.NETType.Short:
                    short[] dataShort = new short[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataShort[r] = input.Skip(4).ReadShort();
                    return dataShort;

                case VoltType.NETType.NullableShort:
                    short?[] dataNullableShort = new short?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataNullableShort[r] = input.Skip(4).ReadNullableShort();
                    return dataNullableShort;

                case VoltType.NETType.Int:
                    int[] dataInt = new int[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataInt[r] = input.Skip(4).ReadInt();
                    return dataInt;

                case VoltType.NETType.NullableInt:
                    int?[] dataNullableInt = new int?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataNullableInt[r] = input.Skip(4).ReadNullableInt();
                    return dataNullableInt;

                case VoltType.NETType.Long:
                    long[] dataLong = new long[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataLong[r] = input.Skip(4).ReadLong();
                    return dataLong;

                case VoltType.NETType.NullableLong:
                    long?[] dataNullableLong = new long?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataNullableLong[r] = input.Skip(4).ReadNullableLong();
                    return dataNullableLong;

                case VoltType.NETType.Double:
                    double[] dataDouble = new double[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataDouble[r] = input.Skip(4).ReadDouble();
                    return dataDouble;

                case VoltType.NETType.NullableDouble:
                    double?[] dataNullableDouble = new double?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataNullableDouble[r] = input.Skip(4).ReadNullableDouble();
                    return dataNullableDouble;

                case VoltType.NETType.DateTime:
                    DateTime[] dataDateTime = new DateTime[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataDateTime[r] = input.Skip(4).ReadDateTime();
                    return dataDateTime;

                case VoltType.NETType.NullableDateTime:
                    DateTime?[] dataNullableDateTime = new DateTime?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataNullableDateTime[r] = input.Skip(4).ReadNullableDateTime();
                    return dataNullableDateTime;

                case VoltType.NETType.String:
                    string[] dataString = new string[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataString[r] = input.Skip(4).ReadString();
                    return dataString;

                case VoltType.NETType.VoltDecimal:
                    VoltDecimal[] dataVoltDecimal = new VoltDecimal[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataVoltDecimal[r] = input.Skip(4).ReadVoltDecimal();
                    return dataVoltDecimal;

                case VoltType.NETType.NullableVoltDecimal:
                    VoltDecimal?[] dataNullableVoltDecimal = new VoltDecimal?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataNullableVoltDecimal[r] = input.Skip(4).ReadNullableVoltDecimal();
                    return dataNullableVoltDecimal;

                case VoltType.NETType.Varbinary:
                    byte[][] dataVarbinary = new byte[rowCount][];
                    for (int r = 0; r < rowCount; r++)
                        dataVarbinary[r] = input.Skip(4).ReadVarbinary();
                    return dataVarbinary;

                default:
                    throw new VoltUnsupportedTypeException(Resources.UnsupportedParameterNETType, TResult.ToString());
            }
        }

        /// <summary>
        /// Returns a single value (strongly-type) of the requested data type from a single-row/single-column Table.
        /// </summary>
        /// <typeparam name="TResult">The desired output data type of the returned value.</typeparam>
        /// <param name="input">The deserializer hoding the response data.</param>
        /// <returns>The single cell contained in the table read.</returns>
        internal static TResult FromSingleValue<TResult>(Deserializer input)
        {
            // Skip basic metadata and get Column Count.
            short columnCount = input.Skip(9).ReadShort();

            // Read column data type.
            DBType columnType = (DBType)input.ReadSByte();

            // Validate the data type matches the .NET casting request
            if (VoltType.ToDBType(typeof(TResult)) != columnType)
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , columnType.ToString()
                                                  , typeof(TResult).ToString()
                                                  );

            // Skip column name and get Row count.
            int rowCount = input.SkipString().ReadInt();
            
            // Validate there is indeed only one column and row.
            if ((columnCount != 1) || (rowCount != 1))
                throw new VoltInvalidDataException(Resources.InvalidRowAndColumnCount, rowCount, columnCount);

            // Load data (skip row length and load first value) - unfortunately, we do have to box this.
            switch (VoltType.ToNETType(typeof(TResult)))
            {
                case VoltType.NETType.Byte:
                    return (TResult)(object)input.Skip(4).ReadByte();

                case VoltType.NETType.NullableByte:
                    return (TResult)(object)input.Skip(4).ReadNullableByte();

                case VoltType.NETType.SByte:
                    return (TResult)(object)input.Skip(4).ReadSByte();

                case VoltType.NETType.NullableSByte:
                    return (TResult)(object)input.Skip(4).ReadNullableSByte();

                case VoltType.NETType.Short:
                    return (TResult)(object)input.Skip(4).ReadShort();

                case VoltType.NETType.NullableShort:
                    return (TResult)(object)input.Skip(4).ReadNullableShort();

                case VoltType.NETType.Int:
                    return (TResult)(object)input.Skip(4).ReadInt();

                case VoltType.NETType.NullableInt:
                    return (TResult)(object)input.Skip(4).ReadNullableInt();

                case VoltType.NETType.Long:
                    return (TResult)(object)input.Skip(4).ReadLong();

                case VoltType.NETType.NullableLong:
                    return (TResult)(object)input.Skip(4).ReadNullableLong();

                case VoltType.NETType.Double:
                    return (TResult)(object)input.Skip(4).ReadDouble();

                case VoltType.NETType.NullableDouble:
                    return (TResult)(object)input.Skip(4).ReadNullableDouble();

                case VoltType.NETType.DateTime:
                    return (TResult)(object)input.Skip(4).ReadDateTime();

                case VoltType.NETType.NullableDateTime:
                    return (TResult)(object)input.Skip(4).ReadNullableDateTime();

                case VoltType.NETType.String:
                    return (TResult)(object)input.Skip(4).ReadString();

                case VoltType.NETType.VoltDecimal:
                    return (TResult)(object)input.Skip(4).ReadVoltDecimal();

                case VoltType.NETType.NullableVoltDecimal:
                    return (TResult)(object)input.Skip(4).ReadNullableVoltDecimal();

                case VoltType.NETType.Varbinary:
                    return (TResult)(object)input.Skip(4).ReadVarbinary();

                default:
                    throw new VoltUnsupportedTypeException(
                                                            Resources.UnsupportedParameterNETType
                                                          , typeof(TResult).ToString()
                                                          );
            }
        }
    }
}
