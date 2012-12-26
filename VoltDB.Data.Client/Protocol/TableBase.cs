/* This file is part of VoltDB.
 * Copyright (C) 2008-2013 VoltDB Inc.
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

using System.Collections.Generic;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides base support for the core VoltDB Table data type (note partial implementation: several deserialization
    /// methods are auto-generated.  See TableBase.StaticMethods.tt).
    /// </summary>
    public partial class TableBase
    {
        /// <summary>
        /// For internal usage only: Map of Names to column index, using lazy-parsing for optimal performance.
        /// </summary>
        protected List<string> ColumnNameMap
        {
            get
            {
                if (this._ColumnNameMap == null)
                {
                    this._ColumnNameMap = new List<string>(this.ColumnCount);
                    Deserializer input = new Deserializer(ColumnNameData);
                    for (var c = 0; c < this.ColumnCount; c++)
                        this._ColumnNameMap.Add(input.ReadString());
                }
                return this._ColumnNameMap;
            }
        }

        /// <summary>
        /// Instantiate a Table by directly deserializing byte data from the given Deserializer.
        /// </summary>
        /// <param name="input">The Deserializer providing the data for the table.</param>
        internal TableBase(Deserializer input)
        {
            // Total byte length of the table data (ignored).
            int tableLength = input.ReadInt();

            // Total byte length of the Table metadata.
            int tableMetadataLength = input.ReadInt();

            // Status code (custom user-set value).
            this.Status = input.ReadSByte();
            // Column Count.
            this.ColumnCount = input.ReadShort();

            // Initialize column-driven data store.
            ColumnType = new DBType[this.ColumnCount];
            Column = new object[this.ColumnCount];

            // Read column data types.
            for (short c = 0; c < this.ColumnCount; c++)
                ColumnType[c] = (DBType)input.ReadSByte();

            // Read column names.
            this.ColumnNameData = input.ReadRaw(tableMetadataLength - 3 - this.ColumnCount);

            // Row count.
            this.RowCount = input.ReadInt();
        }
    }
}