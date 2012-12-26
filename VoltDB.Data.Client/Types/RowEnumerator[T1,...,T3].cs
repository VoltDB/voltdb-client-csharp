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
    /// This enumerator operates on a Table wrapper created for a table with 3 columns.
    /// </summary>
    /// <typeparam name="T1">Type of column 1 of the underlying table.</typeparam>
    /// <typeparam name="T2">Type of column 2 of the underlying table.</typeparam>
    /// <typeparam name="T3">Type of column 3 of the underlying table.</typeparam>
    public class RowCollection<T1, T2, T3> : IEnumerable<Row<T1, T2, T3>>
    {
        /// <summary>
        /// Table this row collection belongs to.
        /// </summary>
        private Table<T1, T2, T3> Table;

        /// <summary>
        /// Internal constructor - row collections can only be spawned from an existing table (wrapper).
        /// </summary>
        /// <param name="tableWrapper">The table (wrapper) the row collection belongs to.</param>
        internal RowCollection(Table<T1, T2, T3> tableWrapper)
        {
            this.Table = tableWrapper;
        }

        /// <summary>
        /// Returns a strongly-typed enumerator for the collection.
        /// </summary>
        /// <returns>The collection enumerator.</returns>
        public IEnumerator<Row<T1, T2, T3>> GetEnumerator()
        {
            return new RowEnumerator<T1, T2, T3>(this.Table);
        }
        /// <summary>
        /// Returns a general enumerator for the collection.
        /// </summary>
        /// <returns>The collection enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new RowEnumerator<T1, T2, T3>(this.Table);
        }

        /// <summary>
        /// Enumerator class for the row collection.
        /// </summary>
        /// <typeparam name="E1">Type of column 1 of the underlying table.</typeparam>
        /// <typeparam name="E2">Type of column 2 of the underlying table.</typeparam>
        /// <typeparam name="E3">Type of column 3 of the underlying table.</typeparam>
        public class RowEnumerator<E1, E2, E3> : IEnumerator<Row<E1, E2, E3>>
        {
            /// <summary>
            /// Table (wrapper) the parent row collection belongs to.
            /// </summary>
            private Table<E1, E2, E3> Table;

            /// <summary>
            /// Position of the enumerator in the underlying collection.
            /// </summary>
            private int Position = -1;

            /// <summary>
            /// Protected constructor - instantiation is only allowed as part of the Table support framework.
            /// </summary>
            /// <param name="tableWrapper">The Table (wrapper) upon which the enumerator operates.</param>
            protected internal RowEnumerator(Table<E1, E2, E3> tableWrapper)
            {
                this.Table = tableWrapper;
            }

            /// <summary>
            /// Returns the (strongly-typed) element the enumerator currently points to.
            /// </summary>
            public Row<E1, E2, E3> Current
            {
                get
                {
                    if ((this.Position > -1) && (this.Position < this.Table.RowCount))
                        return new Row<E1, E2, E3>(this.Table, this.Position);
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
                        return new Row<E1, E2, E3>(this.Table, this.Position);
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
