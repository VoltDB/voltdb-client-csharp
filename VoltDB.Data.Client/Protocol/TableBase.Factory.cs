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
            short columnCount = input.Skip(9).ReadInt16();

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
            int rowCount = input.SkipString().ReadInt32();

            // Load data.
            switch (VoltType.ToNetType(TResult))
            {
                case VoltType.NetType.Byte:
                    byte[] dataByte = new byte[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataByte[r] = input.Skip(4).ReadByte();
                    return dataByte;

                case VoltType.NetType.ByteN:
                    byte?[] dataByteN = new byte?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataByteN[r] = input.Skip(4).ReadByteN();
                    return dataByteN;

                case VoltType.NetType.SByte:
                    sbyte[] dataSByte = new sbyte[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataSByte[r] = input.Skip(4).ReadSByte();
                    return dataSByte;

                case VoltType.NetType.SByteN:
                    sbyte?[] dataSByteN = new sbyte?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataSByteN[r] = input.Skip(4).ReadSByteN();
                    return dataSByteN;

                case VoltType.NetType.Int16:
                    short[] dataInt16 = new short[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataInt16[r] = input.Skip(4).ReadInt16();
                    return dataInt16;

                case VoltType.NetType.Int16N:
                    short?[] dataInt16N = new short?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataInt16N[r] = input.Skip(4).ReadInt16N();
                    return dataInt16N;

                case VoltType.NetType.Int32:
                    int[] dataInt32 = new int[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataInt32[r] = input.Skip(4).ReadInt32();
                    return dataInt32;

                case VoltType.NetType.Int32N:
                    int?[] dataInt32N = new int?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataInt32N[r] = input.Skip(4).ReadInt32N();
                    return dataInt32N;

                case VoltType.NetType.Int64:
                    long[] dataInt64 = new long[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataInt64[r] = input.Skip(4).ReadInt64();
                    return dataInt64;

                case VoltType.NetType.Int64N:
                    long?[] dataInt64N = new long?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataInt64N[r] = input.Skip(4).ReadInt64N();
                    return dataInt64N;

                case VoltType.NetType.Double:
                    double[] dataDouble = new double[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataDouble[r] = input.Skip(4).ReadDouble();
                    return dataDouble;

                case VoltType.NetType.DoubleN:
                    double?[] dataDoubleN = new double?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataDoubleN[r] = input.Skip(4).ReadDoubleN();
                    return dataDoubleN;

                case VoltType.NetType.DateTime:
                    DateTime[] dataDateTime = new DateTime[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataDateTime[r] = input.Skip(4).ReadDateTime();
                    return dataDateTime;

                case VoltType.NetType.DateTimeN:
                    DateTime?[] dataDateTimeN = new DateTime?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataDateTimeN[r] = input.Skip(4).ReadDateTimeN();
                    return dataDateTimeN;

                case VoltType.NetType.String:
                    string[] dataString = new string[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataString[r] = input.Skip(4).ReadString();
                    return dataString;

                case VoltType.NetType.VoltDecimal:
                    VoltDecimal[] dataVoltDecimal = new VoltDecimal[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataVoltDecimal[r] = input.Skip(4).ReadVoltDecimal();
                    return dataVoltDecimal;

                case VoltType.NetType.VoltDecimalN:
                    VoltDecimal?[] dataVoltDecimalN = new VoltDecimal?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataVoltDecimalN[r] = input.Skip(4).ReadVoltDecimalN();
                    return dataVoltDecimalN;

                case VoltType.NetType.Decimal:
                    Decimal[] dataDecimal = new Decimal[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataDecimal[r] = input.Skip(4).ReadDecimal();
                    return dataDecimal;

                case VoltType.NetType.DecimalN:
                    Decimal?[] dataDecimalN = new Decimal?[rowCount];
                    for (int r = 0; r < rowCount; r++)
                        dataDecimalN[r] = input.Skip(4).ReadDecimalN();
                    return dataDecimalN;

                case VoltType.NetType.ByteArray:
                    input.Skip(4);
                    return input.ReadByteArray();

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
            short columnCount = input.Skip(9).ReadInt16();

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
            int rowCount = input.SkipString().ReadInt32();
            
            // Validate there is indeed only one column and row.
            if ((columnCount != 1) || (rowCount != 1))
                throw new VoltInvalidDataException(Resources.InvalidRowAndColumnCount, rowCount, columnCount);

            // Load data (skip row length and load first value) - unfortunately, we do have to box this.
            switch (VoltType.ToNetType(typeof(TResult)))
            {
                case VoltType.NetType.Byte:
                    return (TResult)(object)input.Skip(4).ReadByte();

                case VoltType.NetType.ByteN:
                    return (TResult)(object)input.Skip(4).ReadByteN();

                case VoltType.NetType.SByte:
                    return (TResult)(object)input.Skip(4).ReadSByte();

                case VoltType.NetType.SByteN:
                    return (TResult)(object)input.Skip(4).ReadSByteN();

                case VoltType.NetType.Int16:
                    return (TResult)(object)input.Skip(4).ReadInt16();

                case VoltType.NetType.Int16N:
                    return (TResult)(object)input.Skip(4).ReadInt16N();

                case VoltType.NetType.Int32:
                    return (TResult)(object)input.Skip(4).ReadInt32();

                case VoltType.NetType.Int32N:
                    return (TResult)(object)input.Skip(4).ReadInt32N();

                case VoltType.NetType.Int64:
                    return (TResult)(object)input.Skip(4).ReadInt64();

                case VoltType.NetType.Int64N:
                    return (TResult)(object)input.Skip(4).ReadInt64N();

                case VoltType.NetType.Double:
                    return (TResult)(object)input.Skip(4).ReadDouble();

                case VoltType.NetType.DoubleN:
                    return (TResult)(object)input.Skip(4).ReadDoubleN();

                case VoltType.NetType.DateTime:
                    return (TResult)(object)input.Skip(4).ReadDateTime();

                case VoltType.NetType.DateTimeN:
                    return (TResult)(object)input.Skip(4).ReadDateTimeN();

                case VoltType.NetType.String:
                    return (TResult)(object)input.Skip(4).ReadString();

                case VoltType.NetType.VoltDecimal:
                    return (TResult)(object)input.Skip(4).ReadVoltDecimal();

                case VoltType.NetType.VoltDecimalN:
                    return (TResult)(object)input.Skip(4).ReadVoltDecimalN();

                case VoltType.NetType.Decimal:
                    return (TResult)(object)input.Skip(4).ReadDecimal();

                case VoltType.NetType.DecimalN:
                    return (TResult)(object)input.Skip(4).ReadDecimalN();

                default:
                    throw new VoltUnsupportedTypeException(
                                                            Resources.UnsupportedParameterNETType
                                                          , typeof(TResult).ToString()
                                                          );
            }
        }
    }
}
