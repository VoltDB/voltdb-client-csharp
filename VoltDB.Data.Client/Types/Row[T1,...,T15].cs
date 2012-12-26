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

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Defines a table row on a table with 15 columns.
    /// </summary>
    /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
    /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
    /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
    /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
    /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
    /// <typeparam name="T6">Type of column 6 of the underlying table.</typeparam>
    /// <typeparam name="T7">Type of column 7 of the underlying table.</typeparam>
    /// <typeparam name="T8">Type of column 8 of the underlying table.</typeparam>
    /// <typeparam name="T9">Type of column 9 of the underlying table.</typeparam>
    /// <typeparam name="T10">Type of column 10 of the underlying table.</typeparam>
    /// <typeparam name="T11">Type of column 11 of the underlying table.</typeparam>
    /// <typeparam name="T12">Type of column 12 of the underlying table.</typeparam>
    /// <typeparam name="T13">Type of column 13 of the underlying table.</typeparam>
    /// <typeparam name="T14">Type of column 14 of the underlying table.</typeparam>
    /// <typeparam name="T15">Type of column 15 of the underlying table.</typeparam>
    public class Row<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
        /// <summary>
        /// The table (wrapper) owning this row.
        /// </summary>
        private Table<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Table;
        
        /// <summary>
        /// Index of the row in the underlying table (wrapper).
        /// </summary>
        private int Index = -1;
        
        /// <summary>
        /// Protected constructor - for the client API, table rows can only be spawned from an existing table.
        /// </summary>
        /// <param name="tableWrapper">The table (wrapper) the row belongs to.</param>
        /// <param name="rowIndex">Index of the row in the table's row collection.</param>
        protected internal Row(Table<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> tableWrapper, int rowIndex)
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

        /// <summary>
        /// Returns the (strongly-typed) value of column 3 for this row.
        /// </summary>
        public T3 Column3
        {
            get
            {
                return this.Table.Column3(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 4 for this row.
        /// </summary>
        public T4 Column4
        {
            get
            {
                return this.Table.Column4(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 5 for this row.
        /// </summary>
        public T5 Column5
        {
            get
            {
                return this.Table.Column5(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 6 for this row.
        /// </summary>
        public T6 Column6
        {
            get
            {
                return this.Table.Column6(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 7 for this row.
        /// </summary>
        public T7 Column7
        {
            get
            {
                return this.Table.Column7(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 8 for this row.
        /// </summary>
        public T8 Column8
        {
            get
            {
                return this.Table.Column8(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 9 for this row.
        /// </summary>
        public T9 Column9
        {
            get
            {
                return this.Table.Column9(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 10 for this row.
        /// </summary>
        public T10 Column10
        {
            get
            {
                return this.Table.Column10(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 11 for this row.
        /// </summary>
        public T11 Column11
        {
            get
            {
                return this.Table.Column11(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 12 for this row.
        /// </summary>
        public T12 Column12
        {
            get
            {
                return this.Table.Column12(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 13 for this row.
        /// </summary>
        public T13 Column13
        {
            get
            {
                return this.Table.Column13(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 14 for this row.
        /// </summary>
        public T14 Column14
        {
            get
            {
                return this.Table.Column14(this.Index);
            }
        }

        /// <summary>
        /// Returns the (strongly-typed) value of column 15 for this row.
        /// </summary>
        public T15 Column15
        {
            get
            {
                return this.Table.Column15(this.Index);
            }
        }
    }
}
