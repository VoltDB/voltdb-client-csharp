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

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides base support for the core VoltDB Table data type (Note partial implementation: several deserialization
    /// methods are auto-generated.  See TableBase.StaticMethods.tt)
    /// </summary>
    public partial class TableBase
    {
        /// <summary>
        /// Custom user-status code for the table.
        /// </summary>
        public readonly sbyte Status;

        /// <summary>
        /// Number of columns in the table.
        /// </summary>
        public readonly short ColumnCount;

        /// <summary>
        /// Number of records in the table.
        /// </summary>
        public readonly int RowCount;

        /// <summary>
        /// Convenience flag indicating the table actually has data (RowCount > 0).
        /// </summary>
        public bool HasData
        {
            get
            {
                return this.RowCount > 0;
            }
        }

        /// <summary>
        /// For internal usage only: Core VoltType data type of each column in the table.
        /// </summary>
        protected readonly DBType[] ColumnType;

        /// <summary>
        /// Perform lazy deserialization on column names. It isn't much, but gains 5-10% throughput.
        /// </summary>
        private byte[] ColumnNameData;

        /// <summary>
        /// Internal column name map for column queries by name (Note: for small map counts, List is more efficient
        /// than Dictionary).
        /// </summary>
        private List<string> _ColumnNameMap = null;

        /// <summary>
        /// For internal usage only: actual data storage for the table data, by column (for type-support).
        /// </summary>
        protected internal readonly object[] Column;

        /// <summary>
        /// Return the name of a column.
        /// </summary>
        /// <param name="columnIndex">Index of the column for which to retrieve the name.</param>
        /// <returns>Name of the column at the specified index.</returns>
        public string GetColumnName(short columnIndex)
        {
            return this.ColumnNameMap[columnIndex];
        }

        /// <summary>
        /// Return the index of a column.
        /// </summary>
        /// <param name="columnName">Name of the column for which to retrieve the index.</param>
        /// <returns>Index of the column with the specified name.</returns>
        public short GetColumnIndex(string columnName)
        {
            return (short)this.ColumnNameMap.FindIndex(i => i.Equals(
                                                                      columnName
                                                                    , StringComparison.InvariantCultureIgnoreCase
                                                                    )
                                                      );
        }

        /// <summary>
        /// .NET data type of a column's elements (the column itself being an array of elements of such Type).
        /// </summary>
        /// <param name="columnIndex">Index of the column for which to retrieve the .NET Type.</param>
        /// <returns>.NET Type of the column at the specified index.</returns>
        public Type GetColumnType(short columnIndex)
        {
            return VoltType.ToDefaultNetType(this.ColumnType[columnIndex]);
        }

        /// <summary>
        /// .NET data type of a column's elements (the column itself being an array of elements of such Type).
        /// </summary>
        /// <param name="columnName">Name of the column for which to retrieve the .NET Type.</param>
        /// <returns>.NET Type of the column with the specified name.</returns>
        public Type GetColumnType(string columnName)
        {
            return this.GetColumnType(this.GetColumnIndex(columnName));
        }

        /// <summary>
        /// VoltDB data type of a column's elements (the column itself being an array of elements of such Type).
        /// </summary>
        /// <param name="columnIndex">Index of the column for which to retrieve the VoltDB Type.</param>
        /// <returns>VoltDB Type of the column at the specified index.</returns>
        public DBType GetColumnDBType(short columnIndex)
        {
            return this.ColumnType[columnIndex];
        }

        /// <summary>
        /// VoltDB data type of a column's elements (the column itself being an array of elements of such Type).
        /// </summary>
        /// <param name="columnName">Name of the column for which to retrieve the VoltDB Type.</param>
        /// <returns>VoltDB Type of the column with the specified name.</returns>
        public DBType GetColumnDBType(string columnName)
        {
            return this.GetColumnDBType(this.GetColumnIndex(columnName));
        }
    }
}