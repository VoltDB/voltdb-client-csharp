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
using System;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Defines a shallow Table Row structure wrapping around a deserialized table for easy data access.
    /// </summary>
    public class Row
    {
        /// <summary>
        /// Table this row belongs to.
        /// </summary>
        private Table Table;

        /// <summary>
        /// Index of the row in the table's row collection.
        /// </summary>
        public readonly int Index = -1;

        /// <summary>
        /// Protected constructor - for the client API, table rows can only be spawned from an existing table.
        /// </summary>
        /// <param name="table">The table the row belongs to.</param>
        /// <param name="rowIndex">Index of the row in the table's row collection.</param>
        protected internal Row(Table table, int rowIndex)
        {
            this.Table = table;
            this.Index = rowIndex;
        }

        /// <summary>
        /// Returns the value of an element (field) for the given column index.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T">Type of the element to retrieve.</typeparam>
        /// <param name="columnIndex">Index of the column for the field to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T GetValue<T>(short columnIndex)
        {
            return this.Table.GetValue<T>(columnIndex, this.Index);
        }

        /// <summary>
        /// Returns the value of an element (field) for the given column name.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T">Type of the element to retrieve.</typeparam>
        /// <param name="columnName">Name of the column for the field to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T GetValue<T>(string columnName)
        {
            return this.Table.GetValue<T>(columnName, this.Index);
        }

        /// <summary>
        /// Returns the value of an element (field) for the given column index.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <param name="columnIndex">Index of the column for the field to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public object GetValue(short columnIndex)
        {
            return this.Table.GetValue(columnIndex, this.Index);
        }

        /// <summary>
        /// Returns the value of an element (field) for the given column name.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <param name="columnName">Name of the column for the field to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public object GetValue(string columnName)
        {
            return this.Table.GetValue(columnName, this.Index);
        }

        /// <summary>
        /// Return the name of a column.
        /// </summary>
        /// <param name="columnIndex">Index of the column for which to retrieve the name.</param>
        /// <returns>Name of the column at the specified index.</returns>
        public string GetColumnName(short columnIndex)
        {
            return this.Table.GetColumnName(columnIndex);
        }

        /// <summary>
        /// Return the index of a column.
        /// </summary>
        /// <param name="columnName">Name of the column for which to retrieve the index.</param>
        /// <returns>Index of the column with the specified name.</returns>
        public short GetColumnIndex(string columnName)
        {
            return this.Table.GetColumnIndex(columnName);
        }

        /// <summary>
        /// .NET data type of a column's elements (the column itself being an array of elements of such Type).
        /// </summary>
        /// <param name="columnIndex">Index of the column for which to retrieve the .NET Type.</param>
        /// <returns>.NET Type of the column at the specified index.</returns>
        public Type GetColumnType(short columnIndex)
        {
            return this.Table.GetColumnType(columnIndex);
        }

        /// <summary>
        /// .NET data type of a column's elements (the column itself being an array of elements of such Type).
        /// </summary>
        /// <param name="columnName">Name of the column for which to retrieve the .NET Type.</param>
        /// <returns>.NET Type of the column with the specified name.</returns>
        public Type GetColumnType(string columnName)
        {
            return this.Table.GetColumnType(columnName);
        }

        /// <summary>
        /// VoltDB data type of a column's elements (the column itself being an array of elements of such Type).
        /// </summary>
        /// <param name="columnIndex">Index of the column for which to retrieve the VoltDB Type.</param>
        /// <returns>VoltDB Type of the column at the specified index.</returns>
        public DBType GetColumnDBType(short columnIndex)
        {
            return this.Table.GetColumnDBType(columnIndex);
        }

        /// <summary>
        /// VoltDB data type of a column's elements (the column itself being an array of elements of such Type).
        /// </summary>
        /// <param name="columnName">Name of the column for which to retrieve the VoltDB Type.</param>
        /// <returns>VoltDB Type of the column with the specified name.</returns>
        public DBType GetColumnDBType(string columnName)
        {
            return this.Table.GetColumnDBType(columnName);
        }

        /// <summary>
        /// Returns the column count / number of elements in the row - all rows will obvioulsy return the same number
        /// and the property is merely provided for full coverage.
        /// </summary>
        public short ColumnCount
        {
            get
            {
                return this.Table.ColumnCount;
            }
        }
    }
}