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
    /// Enumeration of available system catalog elements that can be queried by the @SystemCatalog system procedure.
    /// </summary>
    public enum SystemCatalogComponent : byte
    {
        /// <summary>
        /// Table columns.
        /// </summary>
        COLUMNS = 1,

        /// <summary>
        /// Index information.
        /// </summary>
        INDEXINFO = 2,

        /// <summary>
        /// Primary Keys.
        /// </summary>
        PRIMARYKEYS = 3,

        /// <summary>
        /// Procedure columns
        /// </summary>
        PROCEDURECOLUMNS = 4,

        /// <summary>
        /// Procedures.
        /// </summary>
        PROCEDURES = 5,

        /// <summary>
        /// Tables.
        /// </summary>
        TABLES = 6,
    }
}