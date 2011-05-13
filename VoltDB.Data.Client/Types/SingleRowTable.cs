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

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides a data structure for a single-record Volt Table.
    /// </summary>
    public partial class SingleRowTable : TableBase
    {
        /// <summary>
        /// Returns the value of an element (field) for the given column index.
        /// </summary>
        /// <typeparam name="T">Type of the element to retrieve.</typeparam>
        /// <param name="columnIndex">Index of the column for the field to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T GetValue<T>(short columnIndex)
        {
            return (T)this.Column[columnIndex];
        }

        /// <summary>
        /// Returns the value of an element (field) for the given column name.
        /// </summary>
        /// <typeparam name="T">Type of the element to retrieve.</typeparam>
        /// <param name="columnName">Name of the column for the field to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T GetValue<T>(string columnName)
        {
            return this.GetValue<T>(this.GetColumnIndex(columnName));
        }

        /// <summary>
        /// Returns the value of an element (field) for the given column index.
        /// </summary>
        /// <param name="columnIndex">Index of the column for the field to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public object GetValue(short columnIndex)
        {
            return this.Column[columnIndex];
        }

        /// <summary>
        /// Returns the value of an element (field) for the given column name.
        /// </summary>
        /// <param name="columnName">Name of the column for the field to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public object GetValue(string columnName)
        {
            return this.GetValue(this.GetColumnIndex(columnName));
        }
    }
}