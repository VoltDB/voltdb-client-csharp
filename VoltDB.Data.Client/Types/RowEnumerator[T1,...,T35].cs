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
using System.Collections;
using System.Collections.Generic;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides a shalow enumerable collection of strongly-typed rows that can easily be queried against using LINQ.
    /// This enumerator operates on a Table wrapper created for a table with 35 columns.
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
    /// <typeparam name="T35">Type of column 35 of the underlying table.</typeparam>
    public class RowCollection<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> : IEnumerable<Row<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>>
    {
        /// <summary>
        /// Table this row collection belongs to.
        /// </summary>
        private Table<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> Table;

        /// <summary>
        /// Internal constructor - row collections can only be spawned from an existing table (wrapper).
        /// </summary>
        /// <param name="tableWrapper">The table (wrapper) the row collection belongs to.</param>
        internal RowCollection(Table<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> tableWrapper)
        {
            this.Table = tableWrapper;
        }

        /// <summary>
        /// Returns a strongly-typed enumerator for the collection.
        /// </summary>
        /// <returns>The collection enumerator.</returns>
        public IEnumerator<Row<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>> GetEnumerator()
        {
            return new RowEnumerator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(this.Table);
        }
        /// <summary>
        /// Returns a general enumerator for the collection.
        /// </summary>
        /// <returns>The collection enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new RowEnumerator<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(this.Table);
        }

        /// <summary>
        /// Enumerator class for the row collection.
        /// </summary>
        /// <typeparam name="E1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="E2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="E3">Type of column 3 of the underlying table.</typeparam>
        /// <typeparam name="E4">Type of column 4 of the underlying table.</typeparam>
        /// <typeparam name="E5">Type of column 5 of the underlying table.</typeparam>
        /// <typeparam name="E6">Type of column 6 of the underlying table.</typeparam>
        /// <typeparam name="E7">Type of column 7 of the underlying table.</typeparam>
        /// <typeparam name="E8">Type of column 8 of the underlying table.</typeparam>
        /// <typeparam name="E9">Type of column 9 of the underlying table.</typeparam>
        /// <typeparam name="E10">Type of column 10 of the underlying table.</typeparam>
        /// <typeparam name="E11">Type of column 11 of the underlying table.</typeparam>
        /// <typeparam name="E12">Type of column 12 of the underlying table.</typeparam>
        /// <typeparam name="E13">Type of column 13 of the underlying table.</typeparam>
        /// <typeparam name="E14">Type of column 14 of the underlying table.</typeparam>
        /// <typeparam name="E15">Type of column 15 of the underlying table.</typeparam>
        /// <typeparam name="E16">Type of column 16 of the underlying table.</typeparam>
        /// <typeparam name="E17">Type of column 17 of the underlying table.</typeparam>
        /// <typeparam name="E18">Type of column 18 of the underlying table.</typeparam>
        /// <typeparam name="E19">Type of column 19 of the underlying table.</typeparam>
        /// <typeparam name="E20">Type of column 20 of the underlying table.</typeparam>
        /// <typeparam name="E21">Type of column 21 of the underlying table.</typeparam>
        /// <typeparam name="E22">Type of column 22 of the underlying table.</typeparam>
        /// <typeparam name="E23">Type of column 23 of the underlying table.</typeparam>
        /// <typeparam name="E24">Type of column 24 of the underlying table.</typeparam>
        /// <typeparam name="E25">Type of column 25 of the underlying table.</typeparam>
        /// <typeparam name="E26">Type of column 26 of the underlying table.</typeparam>
        /// <typeparam name="E27">Type of column 27 of the underlying table.</typeparam>
        /// <typeparam name="E28">Type of column 28 of the underlying table.</typeparam>
        /// <typeparam name="E29">Type of column 29 of the underlying table.</typeparam>
        /// <typeparam name="E30">Type of column 30 of the underlying table.</typeparam>
        /// <typeparam name="E31">Type of column 31 of the underlying table.</typeparam>
        /// <typeparam name="E32">Type of column 32 of the underlying table.</typeparam>
        /// <typeparam name="E33">Type of column 33 of the underlying table.</typeparam>
        /// <typeparam name="E34">Type of column 34 of the underlying table.</typeparam>
        /// <typeparam name="E35">Type of column 35 of the underlying table.</typeparam>
        public class RowEnumerator<E1, E2, E3, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13, E14, E15, E16, E17, E18, E19, E20, E21, E22, E23, E24, E25, E26, E27, E28, E29, E30, E31, E32, E33, E34, E35> : IEnumerator<Row<E1, E2, E3, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13, E14, E15, E16, E17, E18, E19, E20, E21, E22, E23, E24, E25, E26, E27, E28, E29, E30, E31, E32, E33, E34, E35>>
        {
            /// <summary>
            /// Table (wrapper) the parent row collection belongs to.
            /// </summary>
            private Table<E1, E2, E3, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13, E14, E15, E16, E17, E18, E19, E20, E21, E22, E23, E24, E25, E26, E27, E28, E29, E30, E31, E32, E33, E34, E35> Table;

            /// <summary>
            /// Position of the enumerator in the underlying collection.
            /// </summary>
            private int Position = -1;

            /// <summary>
            /// Protected constructor - instantiation is only allowed as part of the Table support framework.
            /// </summary>
            /// <param name="tableWrapper">The Table (wrapper) upon which the enumerator operates.</param>
            protected internal RowEnumerator(Table<E1, E2, E3, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13, E14, E15, E16, E17, E18, E19, E20, E21, E22, E23, E24, E25, E26, E27, E28, E29, E30, E31, E32, E33, E34, E35> tableWrapper)
            {
                this.Table = tableWrapper;
            }

            /// <summary>
            /// Returns the (strongly-typed) element the enumerator currently points to.
            /// </summary>
            public Row<E1, E2, E3, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13, E14, E15, E16, E17, E18, E19, E20, E21, E22, E23, E24, E25, E26, E27, E28, E29, E30, E31, E32, E33, E34, E35> Current
            {
                get
                {
                    if ((this.Position > -1) && (this.Position < this.Table.RowCount))
                        return new Row<E1, E2, E3, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13, E14, E15, E16, E17, E18, E19, E20, E21, E22, E23, E24, E25, E26, E27, E28, E29, E30, E31, E32, E33, E34, E35>(this.Table, this.Position);
                    else
                        throw new InvalidOperationException();
                }
            }

            /// <summary>
            /// Moves the enumerator to the next element in the collection.
            /// </summary>
            /// <returns>true if the enumerator was successfully advanced to the next row; false if the enumerator has
            /// passed the end of the collection.</returns>
            public bool MoveNext()
            {
                this.Position++;
                return this.Position < this.Table.RowCount;
            }

            /// <summary>
            /// Rewinds the enumerator to the beginning.
            /// </summary>
            public void Reset()
            {
                this.Position = -1;
            }

            /// <summary>
            /// General interface implementation: Returns the (boxed) element the enumerator currently points to.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if ((this.Position > 0) && (this.Position < this.Table.RowCount))
                        return new Row<E1, E2, E3, E4, E5, E6, E7, E8, E9, E10, E11, E12, E13, E14, E15, E16, E17, E18, E19, E20, E21, E22, E23, E24, E25, E26, E27, E28, E29, E30, E31, E32, E33, E34, E35>(this.Table, this.Position);
                    else
                        throw new InvalidOperationException();
                }
            }

            /// <summary>
            /// Dispose of used resources.
            /// </summary>
            public void Dispose() { }
        }
    }
}
