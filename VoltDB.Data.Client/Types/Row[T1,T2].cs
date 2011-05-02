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
    /// Defines a table row on a table with 2 columns.
    /// </summary>
    /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
    /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
    public class Row<T1, T2>
    {
        /// <summary>
        /// The table (wrapper) owning this row.
        /// </summary>
        private Table<T1, T2> Table;
        
        /// <summary>
        /// Index of the row in the underlying table (wrapper).
        /// </summary>
        private int Index = -1;
        
        /// <summary>
        /// Protected constructor - for the client API, table rows can only be spawned from an existing table.
        /// </summary>
        /// <param name="tableWrapper">The table (wrapper) the row belongs to.</param>
        /// <param name="rowIndex">Index of the row in the table's row collection.</param>
        protected internal Row(Table<T1, T2> tableWrapper, int rowIndex)
        {
            this.Table = tableWrapper;
            this.Index = rowIndex;
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 1 for this row.
        /// </summary>
        public T1 Column1
        {
            get
            {
                return this.Table.Column1(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 2 for this row.
        /// </summary>
        public T2 Column2
        {
            get
            {
                return this.Table.Column2(this.Index);
            }
        }
    }
}
