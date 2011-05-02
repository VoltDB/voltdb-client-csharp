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
namespace VoltDB.Data.Client
{
    /// <summary>
    /// General implementation of a generic Table.
    /// </summary>
    public partial class Table : TableBase
    {
        /// <summary>
        /// Internal storage for the Row collection.
        /// </summary>
        private readonly RowCollection _Rows;

        /// <summary>
        /// Provides an enumerable collection of records on the table, allowing for full LINQ support.
        /// </summary>
        public RowCollection Rows
        {
            get
            {
                return this._Rows;
            }
        }

        /// <summary>
        /// Returns a single column from the table.
        /// </summary>
        /// <param name="columnIndex">Index of the column to retrieve.</param>
        /// <returns>The requested column.</returns>
        public Column GetColumn(short columnIndex)
        {
            return new Column(this, columnIndex);
        }

        /// <summary>
        /// Returns a single column from the table.
        /// </summary>
        /// <param name="columnName">Name of the column to retrieve.</param>
        /// <returns>The requested column.</returns>
        public Column GetColumn(string columnName)
        {
            return new Column(this, this.GetColumnIndex(columnName));
        }

        /// <summary>
        /// Returns a strongly-type raw data array for the column.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T">.NET data type of the elements.</typeparam>
        /// <param name="columnIndex">Name of the column for which to retrieve the raw data.</param>
        /// <returns>Raw data array of the column content.</returns>
        public T[] GetColumnData<T>(short columnIndex)
        {
            return (T[])this.Column[columnIndex];
        }

        /// <summary>
        /// Returns a strongly-type raw data array for the column.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T">.NET data type of the elements.</typeparam>
        /// <param name="columnName">Name of the column for which to retrieve the raw data.</param>
        /// <returns>Raw data array of the column content.</returns>
        public T[] GetColumnData<T>(string columnName)
        {
            return (T[])this.Column[this.GetColumnIndex(columnName)];
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T">Type of the element to retrieve.</typeparam>
        /// <param name="columnIndex">Column index of the element to retrieve.</param>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T GetValue<T>(short columnIndex, int rowIndex)
        {
            return ((T[])this.Column[columnIndex])[rowIndex];
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// Conversion is not possible, thus you MUST request the exact nullable type corresponding to the underlying
        /// data, or this call will fail.
        /// Valid types are: sbyte?, short?, int?, long?, double?, DateTime?, string, BigDecimal
        /// </summary>
        /// <typeparam name="T">Type of the element to retrieve.</typeparam>
        /// <param name="columnName">Column name of the element to retrieve.</param>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T GetValue<T>(string columnName, int rowIndex)
        {
            return ((T[])this.Column[this.GetColumnIndex(columnName)])[rowIndex];
        }
    }
}