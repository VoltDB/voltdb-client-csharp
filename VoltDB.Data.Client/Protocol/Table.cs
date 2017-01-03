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
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// General implementation of a generic Table.
    /// </summary>
    public partial class Table : TableBase
    {
        /// <summary>
        /// Instantiate a Table by directly deserializing byte data from the given Deserializer.
        /// </summary>
        /// <param name="input">The Deserializer providing the data for the table.</param>
        internal Table(Deserializer input)
            : base(input)
        {
            // Allocate column-based data storage.
            for (short c = 0; c < this.ColumnCount; c++)
            {
                switch (ColumnType[c])
                {
                    case DBType.TINYINT:
                        Column[c] = new sbyte?[this.RowCount];
                        break;
                    case DBType.SMALLINT:
                        Column[c] = new short?[this.RowCount];
                        break;
                    case DBType.INTEGER:
                        Column[c] = new int?[this.RowCount];
                        break;
                    case DBType.BIGINT:
                        Column[c] = new long?[this.RowCount];
                        break;
                    case DBType.FLOAT:
                        Column[c] = new double?[this.RowCount];
                        break;
                    case DBType.DECIMAL:
                        Column[c] = new VoltDecimal?[this.RowCount];
                        break;
                    case DBType.TIMESTAMP:
                        Column[c] = new DateTime?[this.RowCount];
                        break;
                    case DBType.STRING:
                        Column[c] = new string[this.RowCount];
                        break;
                    case DBType.VARBINARY:
                        Column[c] = new byte[this.RowCount][];
                        break;
                    default:
                        throw new VoltUnsupportedTypeException(Resources.UnsupportedDBType, ColumnType[c]);
                }
            }

            // Get data and push to storage.
            for (int r = 0; r < this.RowCount; r++)
            {
                // Total byte length of the row (ignored).
                input.Skip(4);
                // Read data and push in storage.
                for (short c = 0; c < this.ColumnCount; c++)
                {
                    switch (ColumnType[c])
                    {
                        case DBType.TINYINT:
                            (Column[c] as sbyte?[])[r] = input.ReadSByteN();
                            break;
                        case DBType.SMALLINT:
                            (Column[c] as short?[])[r] = input.ReadInt16N();
                            break;
                        case DBType.INTEGER:
                            (Column[c] as int?[])[r] = input.ReadInt32N();
                            break;
                        case DBType.BIGINT:
                            (Column[c] as long?[])[r] = input.ReadInt64N();
                            break;
                        case DBType.FLOAT:
                            (Column[c] as double?[])[r] = input.ReadDoubleN();
                            break;
                        case DBType.DECIMAL:
                            (Column[c] as VoltDecimal?[])[r] = input.ReadVoltDecimalN();
                            break;
                        case DBType.TIMESTAMP:
                            (Column[c] as DateTime?[])[r] = input.ReadDateTimeN();
                            break;
                        case DBType.VARBINARY:
                            (Column[c] as byte[][])[r] = input.ReadByteArray();
                            break;
                        default:
                            (Column[c] as string[])[r] = input.ReadString();
                            break;
                    }
                }
            }

            // Attach the enumerable row collection.
            this._Rows = new RowCollection(this);
        }
    }
}