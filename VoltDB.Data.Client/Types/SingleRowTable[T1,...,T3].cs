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
    /// Defines a table wrapper for a table with 3 columns.
    /// </summary>
    /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
    /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
    /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
    public class SingleRowTable<T1, T2, T3>
    {
        /// <summary>
        /// Table from which the wrapper feeds.
        /// </summary>
        private SingleRowTable RawTable;

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
        /// <param name="table">The single-row table to build the wrapper for.</param>
        internal SingleRowTable(SingleRowTable table)
        {
            // Validate column count.
            if (table.ColumnCount != 3)
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

            // Validation complete, keep a reference to the raw table.
            this.RawTable = table;
        }
        /// <summary>
        /// Returns the value for Column1.
        /// </summary>
        /// <returns>The element (field) value.</returns>
        public T1 Column1
        {
            get
            {
                return this.RawTable.GetValue<T1>(0);
            }
        }
        /// <summary>
        /// Returns the value for Column2.
        /// </summary>
        /// <returns>The element (field) value.</returns>
        public T2 Column2
        {
            get
            {
                return this.RawTable.GetValue<T2>(1);
            }
        }
        /// <summary>
        /// Returns the value for Column3.
        /// </summary>
        /// <returns>The element (field) value.</returns>
        public T3 Column3
        {
            get
            {
                return this.RawTable.GetValue<T3>(2);
            }
        }
    }
}
