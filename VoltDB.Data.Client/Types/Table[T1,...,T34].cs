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
    /// Defines a table wrapper for a table with 34 columns.
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
    /// <typeparam name="T16">Type of column 16 of the underlying table.</typeparam>
    /// <typeparam name="T17">Type of column 17 of the underlying table.</typeparam>
    /// <typeparam name="T18">Type of column 18 of the underlying table.</typeparam>
    /// <typeparam name="T19">Type of column 19 of the underlying table.</typeparam>
    /// <typeparam name="T20">Type of column 20 of the underlying table.</typeparam>
    /// <typeparam name="T21">Type of column 21 of the underlying table.</typeparam>
    /// <typeparam name="T22">Type of column 22 of the underlying table.</typeparam>
    /// <typeparam name="T23">Type of column 23 of the underlying table.</typeparam>
    /// <typeparam name="T24">Type of column 24 of the underlying table.</typeparam>
    /// <typeparam name="T25">Type of column 25 of the underlying table.</typeparam>
    /// <typeparam name="T26">Type of column 26 of the underlying table.</typeparam>
    /// <typeparam name="T27">Type of column 27 of the underlying table.</typeparam>
    /// <typeparam name="T28">Type of column 28 of the underlying table.</typeparam>
    /// <typeparam name="T29">Type of column 29 of the underlying table.</typeparam>
    /// <typeparam name="T30">Type of column 30 of the underlying table.</typeparam>
    /// <typeparam name="T31">Type of column 31 of the underlying table.</typeparam>
    /// <typeparam name="T32">Type of column 32 of the underlying table.</typeparam>
    /// <typeparam name="T33">Type of column 33 of the underlying table.</typeparam>
    /// <typeparam name="T34">Type of column 34 of the underlying table.</typeparam>
    public class Table<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>
    {
        /// <summary>
        /// Table from which the wrapper feeds.
        /// </summary>
        private Table RawTable;

        /// <summary>
        /// Internal storage for the Row collection.
        /// </summary>
        private readonly RowCollection<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> _Rows;

        /// <summary>
        /// Provides an enumerable collection of records on the table, allowing for full LINQ support.
        /// </summary>
        public RowCollection<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> Rows
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
            if (table.ColumnCount != 34)
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

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(5)) == (typeof(T6))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(5)).ToString()
                                                  , typeof(T6).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(6)) == (typeof(T7))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(6)).ToString()
                                                  , typeof(T7).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(7)) == (typeof(T8))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(7)).ToString()
                                                  , typeof(T8).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(8)) == (typeof(T9))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(8)).ToString()
                                                  , typeof(T9).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(9)) == (typeof(T10))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(9)).ToString()
                                                  , typeof(T10).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(10)) == (typeof(T11))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(10)).ToString()
                                                  , typeof(T11).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(11)) == (typeof(T12))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(11)).ToString()
                                                  , typeof(T12).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(12)) == (typeof(T13))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(12)).ToString()
                                                  , typeof(T13).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(13)) == (typeof(T14))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(13)).ToString()
                                                  , typeof(T14).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(14)) == (typeof(T15))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(14)).ToString()
                                                  , typeof(T15).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(15)) == (typeof(T16))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(15)).ToString()
                                                  , typeof(T16).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(16)) == (typeof(T17))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(16)).ToString()
                                                  , typeof(T17).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(17)) == (typeof(T18))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(17)).ToString()
                                                  , typeof(T18).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(18)) == (typeof(T19))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(18)).ToString()
                                                  , typeof(T19).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(19)) == (typeof(T20))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(19)).ToString()
                                                  , typeof(T20).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(20)) == (typeof(T21))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(20)).ToString()
                                                  , typeof(T21).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(21)) == (typeof(T22))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(21)).ToString()
                                                  , typeof(T22).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(22)) == (typeof(T23))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(22)).ToString()
                                                  , typeof(T23).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(23)) == (typeof(T24))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(23)).ToString()
                                                  , typeof(T24).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(24)) == (typeof(T25))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(24)).ToString()
                                                  , typeof(T25).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(25)) == (typeof(T26))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(25)).ToString()
                                                  , typeof(T26).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(26)) == (typeof(T27))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(26)).ToString()
                                                  , typeof(T27).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(27)) == (typeof(T28))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(27)).ToString()
                                                  , typeof(T28).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(28)) == (typeof(T29))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(28)).ToString()
                                                  , typeof(T29).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(29)) == (typeof(T30))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(29)).ToString()
                                                  , typeof(T30).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(30)) == (typeof(T31))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(30)).ToString()
                                                  , typeof(T31).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(31)) == (typeof(T32))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(31)).ToString()
                                                  , typeof(T32).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(32)) == (typeof(T33))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(32)).ToString()
                                                  , typeof(T33).ToString()
                                                  );

            if (!(VoltType.ToDefaultNetType(table.GetColumnDBType(33)) == (typeof(T34))))
                throw new VoltInvalidCastException(
                                                    Resources.InvalidCastException
                                                  , VoltType.ToDefaultNetType(table.GetColumnDBType(33)).ToString()
                                                  , typeof(T34).ToString()
                                                  );

            // Validation complete, keep a reference to the raw table.
            this.RawTable = table;

            // Attach the enumerable row collection.
            this._Rows = new RowCollection<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(this);
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
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T6[] Column6()
        {
            return this.RawTable.GetColumnData<T6>(5);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T6 Column6(int rowIndex)
        {
            return this.RawTable.GetValue<T6>(5, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T7[] Column7()
        {
            return this.RawTable.GetColumnData<T7>(6);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T7 Column7(int rowIndex)
        {
            return this.RawTable.GetValue<T7>(6, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T8[] Column8()
        {
            return this.RawTable.GetColumnData<T8>(7);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T8 Column8(int rowIndex)
        {
            return this.RawTable.GetValue<T8>(7, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T9[] Column9()
        {
            return this.RawTable.GetColumnData<T9>(8);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T9 Column9(int rowIndex)
        {
            return this.RawTable.GetValue<T9>(8, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T10[] Column10()
        {
            return this.RawTable.GetColumnData<T10>(9);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T10 Column10(int rowIndex)
        {
            return this.RawTable.GetValue<T10>(9, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T11[] Column11()
        {
            return this.RawTable.GetColumnData<T11>(10);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T11 Column11(int rowIndex)
        {
            return this.RawTable.GetValue<T11>(10, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T12[] Column12()
        {
            return this.RawTable.GetColumnData<T12>(11);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T12 Column12(int rowIndex)
        {
            return this.RawTable.GetValue<T12>(11, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T13[] Column13()
        {
            return this.RawTable.GetColumnData<T13>(12);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T13 Column13(int rowIndex)
        {
            return this.RawTable.GetValue<T13>(12, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T14[] Column14()
        {
            return this.RawTable.GetColumnData<T14>(13);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T14 Column14(int rowIndex)
        {
            return this.RawTable.GetValue<T14>(13, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T15[] Column15()
        {
            return this.RawTable.GetColumnData<T15>(14);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T15 Column15(int rowIndex)
        {
            return this.RawTable.GetValue<T15>(14, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T16[] Column16()
        {
            return this.RawTable.GetColumnData<T16>(15);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T16 Column16(int rowIndex)
        {
            return this.RawTable.GetValue<T16>(15, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T17[] Column17()
        {
            return this.RawTable.GetColumnData<T17>(16);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T17 Column17(int rowIndex)
        {
            return this.RawTable.GetValue<T17>(16, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T18[] Column18()
        {
            return this.RawTable.GetColumnData<T18>(17);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T18 Column18(int rowIndex)
        {
            return this.RawTable.GetValue<T18>(17, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T19[] Column19()
        {
            return this.RawTable.GetColumnData<T19>(18);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T19 Column19(int rowIndex)
        {
            return this.RawTable.GetValue<T19>(18, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T20[] Column20()
        {
            return this.RawTable.GetColumnData<T20>(19);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T20 Column20(int rowIndex)
        {
            return this.RawTable.GetValue<T20>(19, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T21[] Column21()
        {
            return this.RawTable.GetColumnData<T21>(20);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T21 Column21(int rowIndex)
        {
            return this.RawTable.GetValue<T21>(20, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T22[] Column22()
        {
            return this.RawTable.GetColumnData<T22>(21);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T22 Column22(int rowIndex)
        {
            return this.RawTable.GetValue<T22>(21, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T23[] Column23()
        {
            return this.RawTable.GetColumnData<T23>(22);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T23 Column23(int rowIndex)
        {
            return this.RawTable.GetValue<T23>(22, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T24[] Column24()
        {
            return this.RawTable.GetColumnData<T24>(23);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T24 Column24(int rowIndex)
        {
            return this.RawTable.GetValue<T24>(23, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T25[] Column25()
        {
            return this.RawTable.GetColumnData<T25>(24);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T25 Column25(int rowIndex)
        {
            return this.RawTable.GetValue<T25>(24, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T26[] Column26()
        {
            return this.RawTable.GetColumnData<T26>(25);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T26 Column26(int rowIndex)
        {
            return this.RawTable.GetValue<T26>(25, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T27[] Column27()
        {
            return this.RawTable.GetColumnData<T27>(26);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T27 Column27(int rowIndex)
        {
            return this.RawTable.GetValue<T27>(26, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T28[] Column28()
        {
            return this.RawTable.GetColumnData<T28>(27);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T28 Column28(int rowIndex)
        {
            return this.RawTable.GetValue<T28>(27, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T29[] Column29()
        {
            return this.RawTable.GetColumnData<T29>(28);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T29 Column29(int rowIndex)
        {
            return this.RawTable.GetValue<T29>(28, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T30[] Column30()
        {
            return this.RawTable.GetColumnData<T30>(29);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T30 Column30(int rowIndex)
        {
            return this.RawTable.GetValue<T30>(29, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T31[] Column31()
        {
            return this.RawTable.GetColumnData<T31>(30);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T31 Column31(int rowIndex)
        {
            return this.RawTable.GetValue<T31>(30, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T32[] Column32()
        {
            return this.RawTable.GetColumnData<T32>(31);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T32 Column32(int rowIndex)
        {
            return this.RawTable.GetValue<T32>(31, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T33[] Column33()
        {
            return this.RawTable.GetColumnData<T33>(32);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T33 Column33(int rowIndex)
        {
            return this.RawTable.GetValue<T33>(32, rowIndex);
        }
        /// <summary>
        /// Returns a strongly-typed raw data array for the column.
        /// </summary>
        /// <returns>Raw data array of the column content.</returns>
        public T34[] Column34()
        {
            return this.RawTable.GetColumnData<T34>(33);
        }

        /// <summary>
        /// Returns a specific element in the column, at the given row index.
        /// This method is provided for full-coverage, however, if you find yourself iterating through the column
        /// records this way, you are likely better off grabbing the raw data and iterating on a strongly-typed array.
        /// </summary>
        /// <param name="rowIndex">Row index of the element to retrieve.</param>
        /// <returns>The element (field) value.</returns>
        public T34 Column34(int rowIndex)
        {
            return this.RawTable.GetValue<T34>(33, rowIndex);
        }
    }
}
