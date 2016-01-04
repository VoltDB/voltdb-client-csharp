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

using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides a data structure for a single-record Volt Table.
    /// </summary>
    public partial class SingleRowTable : TableBase
    {
        /// <summary>
        /// Instantiate a Table by directly deserializing byte data from the given Deserializer.
        /// </summary>
        /// <param name="input">The Deserializer providing the data for the table.</param>
        internal SingleRowTable(Deserializer input)
            : base(input)
        {
            // Validate row count.
            if (this.RowCount > 1)
                throw new VoltInvalidDataException(Resources.InvalidRowCount, this.RowCount);
            else if (this.RowCount == 0)
                return;

            // Total byte length of the row (ignored).
            int rowLength = input.ReadInt32();

            // Read data and push in storage.
            for (short c = 0; c < this.ColumnCount; c++)
            {
                switch (ColumnType[c])
                {
                    case DBType.TINYINT:
                        Column.SetValue(input.ReadSByteN(), c);
                        break;
                    case DBType.SMALLINT:
                        Column.SetValue(input.ReadInt16N(), c);
                        break;
                    case DBType.INTEGER:
                        Column.SetValue(input.ReadInt32N(), c);
                        break;
                    case DBType.BIGINT:
                        Column.SetValue(input.ReadInt64N(), c);
                        break;
                    case DBType.FLOAT:
                        Column.SetValue(input.ReadDoubleN(), c);
                        break;
                    case DBType.DECIMAL:
                        Column.SetValue(input.ReadVoltDecimalN(), c);
                        break;
                    case DBType.TIMESTAMP:
                        Column.SetValue(input.ReadDateTimeN(), c);
                        break;
                    case DBType.VARBINARY:
                        Column.SetValue(input.ReadByteArray(), c);
                        break;
                    default:
                        Column.SetValue(input.ReadString(), c);
                        break;
                }
            }
        }
    }
}