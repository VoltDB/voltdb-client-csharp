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

using System;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Defines a shallow Table Column structure wrapping around a deserialized table for easy data access.
    /// </summary>
    public class Column
    {
        /// <summary>
        /// Table this column belongs to.
        /// </summary>
        private Table Table;

        /// <summary>
        /// Index of the column in the table's column collection.
        /// </summary>
        public readonly short Index = -1;

        /// <summary>
        /// Internal constructor - columns can only be spawned from an existing table.
        /// </summary>
        /// <param name="table">The table the column belongs to.</param>
        /// <param name="columnIndex">Index of the column in the table's column collection.</param>
        internal Column(Table table, short columnIndex)
        {
            this.Table = table;
            this.Index = columnIndex;
        }

        /// <summary>
        /// Name of the column.
        /// </summary>
        public string Name
        {
            get
            {
                return this.Table.GetColumnName(this.Index);
            }
        }

        /// <summary>
        /// .NET data type of the column elements (the column itself being an array of elements of such Type).
        /// </summary>
        public Type Type
        {
            get
            {
                return this.Table.GetColumnType(this.Index);
            }
        }

        /// <summary>
        /// VoltDB data type of the column elements (the column itself being an array of elements of such Type).
        /// </summary>
        public DBType DBType
        {
            get
            {
                return this.Table.GetColumnDBType(this.Index);
            }
        }

        /// <summary>
        /// Returns a strongly-type raw data array for the column.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T">.NET data type of the elements.</typeparam>
        /// <returns>Raw data array of the column content.</returns>
        public T[] GetData<T>()
        {
            return this.Table.GetColumnData<T>(this.Index);
        }

        /// <summary>
        /// Returns a strongly-type raw data array for the column.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public object[] GetData()
        {
            return this.Table.GetColumnData(this.Index);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself ierating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T">Type f the element to retrieve.</typeparam>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T GetValue<T>(int rowIndex)
        {
            return this.Table.GetValue<T>(this.Index, rowIndex);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself ierating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public object GetValue(int rowIndex)
        {
            return this.Table.GetValue(this.Index, rowIndex);
        }

        /// <summary>
        /// Returns the row count / number of elements in the column - all columns will obvioulsy return the same
        /// number and the property is merely provided for full coverage.
        /// </summary>
        public int RowCount
        {
            get
            {
                return this.Table.RowCount;
            }
        }
    }
}