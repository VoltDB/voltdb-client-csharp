/* This file is part of VoltDB.
 * Copyright (C) 2008-2017 VoltDB Inc.
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
    /// Holds a reference (path and unique-ID) to a specific snapshot.
    /// </summary>
    public struct SnapshotReference
    {
        /// <summary>
        /// Path of the directory containing the snapshot.
        /// </summary>
        public string DirectoryPath;
        /// <summary>
        /// Unique ID of the snapshot.
        /// </summary>
        public string UniqueId;
        /// <summary>
        /// Creates a new instance of the SnapshotReference class.
        /// </summary>
        /// <param name="directoryPath">Path of the directory containing the snapshot.</param>
        /// <param name="uniqueId">Unique ID of the snapshot.</param>
        public SnapshotReference(string directoryPath, string uniqueId)
        {
            this.DirectoryPath = directoryPath;
            this.UniqueId = uniqueId;
        }
    }
}