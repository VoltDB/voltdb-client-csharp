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

using System;
using System.Collections;
using System.Collections.Generic;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides a shalow enumerable collection of rows that can easily be queried against using LINQ.
    /// </summary>
    public class RowCollection : IEnumerable<Row>
    {
        /// <summary>
        /// Table this row collection belongs to.
        /// </summary>
        private Table Table;

        /// <summary>
        /// Internal constructor - row collections can only be spawned from an existing table.
        /// </summary>
        /// <param name="table">The table the row collection belongs to.</param>
        internal RowCollection(Table table)
        {
            this.Table = table;
        }

        /// <summary>
        /// Returns a strongly-typed enumerator for the collection.
        /// </summary>
        /// <returns>The collection enumerator.</returns>
        public IEnumerator<Row> GetEnumerator()
        {
            return new RowEnumerator(this.Table);
        }
        /// <summary>
        /// Returns a general enumerator for the collection.
        /// </summary>
        /// <returns>The collection enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new RowEnumerator(this.Table);
        }

        /// <summary>
        /// Enumerator class for the row collection.
        /// </summary>
        public class RowEnumerator : IEnumerator<Row>
        {
            /// <summary>
            /// Table the parent row collection belongs to.
            /// </summary>
            private Table Table;

            /// <summary>
            /// Position of the enumerator in the underlying collection.
            /// </summary>
            private int Position = -1;

            /// <summary>
            /// Protected constructor - instantiation is only allowed as part of the Table support framework.
            /// </summary>
            /// <param name="table">The Table upon which the enumerator operates.</param>
            protected internal RowEnumerator(Table table)
            {
                this.Table = table;
            }

            /// <summary>
            /// Returns the (strongly-typed) element the enumerator currently points to.
            /// </summary>
            public Row Current
            {
                get
                {
                    if ((this.Position > -1) && (this.Position < this.Table.RowCount))
                        return new Row(this.Table, this.Position);
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
                        return new Row(this.Table, this.Position);
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