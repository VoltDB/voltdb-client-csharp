/* This file is part of VoltDB.
 * Copyright (C) 2008-2015 VoltDB Inc.
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

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Wrapper for all non-trivial exceptions thrown by the Client library (trivial as in: ArgumentException, etc.)
    /// VoltExceptions are exceptions that are specific to library failures and are essentially re-packaged 'expected'
    /// or 'expectable' exceptions (i.e.: using a data type VoltDB doesn't support for data storage, network protocol
    /// message corruption or version mismatch, serialization or deserialization error, etc.)
    /// 
    /// There is a closed-chain inheritance on the VoltException: all final members are sealed classes and intermediary
    /// members (or this root class), should not be derived outside of the library.
    /// </summary>
    [Serializable]
    public class VoltException : Exception
    {
        internal VoltException(string msg)
            : base(msg) { }

        internal VoltException(string msg, Exception inner)
            : base(msg, inner) { }

        internal VoltException(string format, params object[] parameters)
            : base(string.Format(format, parameters)) { }

        internal VoltException(string format, Exception inner, params object[] parameters)
            : base(string.Format(format, parameters), inner) { }
    }
}