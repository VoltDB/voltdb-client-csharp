/* This file is part of VoltDB.
 * Copyright (C) 2008-2018 VoltDB Inc.
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
    /// Defines a table wrapper for a table with 5 columns.
    /// </summary>
    /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
    /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
    /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
    /// <typeparam name="T4">Type of column 4 of the underlying table.</typeparam>
    /// <typeparam name="T5">Type of column 5 of the underlying table.</typeparam>
    public class Table<T1, T2, T3, T4, T5>
    {
        /// <summary>
        /// Table from which the wrapper feeds.
        /// </summary>
        private Table RawTable;

        /// <summary>
        /// Internal storage for the Row collection.
        /// </summary>
        private readonly RowCollection<T1, T2, T3, T4, T5> _Rows;

        /// <summary>
        /// Provides an enumerable collection of records on the table, allowing for full LINQ support.
        /// </summary>
        public RowCollection<T1, T2, T3, T4, T5> Rows
        {
            get
            {
                return this._Rows;
            }
        }
        
        /// <summary>
        /// Number of records in the table.
        /// </summary>
        public int RowCount
        {
            get
            {
                return this.RawTable.RowCount;
            }
        }

        /// <summary>
        /// Convenience flag indicating the table actually has data (RowCount > 0).
        /// </summary>
        public bool HasData
        {
            get
            {
                return this.RawTable.RowCount > 0;
            }
        }

        /// <summary>
        /// Instantiate a strongly-typed Table Wrapper around the given generic Table.
        /// </summary>
        /// <param name="table">The table to build the wrapper for.</param>
        internal Table(Table table)
        {
            // Validate column count.
            if (table.ColumnCount != 5)
                throw new VoltInvalidDataException(Resources.InvalidColumnCount, table.ColumnCount);

            // Validate column data types.
            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(0)) == (typeof(T1))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(0)).ToString()
                                                  , typeof(T1).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(1)) == (typeof(T2))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(1)).ToString()
                                                  , typeof(T2).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(2)) == (typeof(T3))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(2)).ToString()
                                                  , typeof(T3).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(3)) == (typeof(T4))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(3)).ToString()
                                                  , typeof(T4).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(4)) == (typeof(T5))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(4)).ToString()
                                                  , typeof(T5).ToString()
                                                  );

            // Validation complete, keep a reference to the raw table.
            this.RawTable = table;

            // Attach the enumerable row collection.
            this._Rows = new RowCollection<T1, T2, T3, T4, T5>(this);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T1[] Column1()
        {
            return this.RawTable.GetColumnData<T1>(0);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T1 Column1(int rowIndex)
        {
            return this.RawTable.GetValue<T1>(0, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T2[] Column2()
        {
            return this.RawTable.GetColumnData<T2>(1);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T2 Column2(int rowIndex)
        {
            return this.RawTable.GetValue<T2>(1, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T3[] Column3()
        {
            return this.RawTable.GetColumnData<T3>(2);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T3 Column3(int rowIndex)
        {
            return this.RawTable.GetValue<T3>(2, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T4[] Column4()
        {
            return this.RawTable.GetColumnData<T4>(3);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T4 Column4(int rowIndex)
        {
            return this.RawTable.GetValue<T4>(3, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T5[] Column5()
        {
            return this.RawTable.GetColumnData<T5>(4);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T5 Column5(int rowIndex)
        {
            return this.RawTable.GetValue<T5>(4, rowIndex);
        }
    }
}
