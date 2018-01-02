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

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Lists possible responses statuses provided by the server in return of a login request.
    /// </summary>
    public enum LoginResponseStatus : sbyte
    {
        /// <summary>
        /// Indicates a successful login request that yielded a valid connection.
        /// </summary>
        Connected = 0,

        /// <summary>
        /// Indicates invalid credentials where provided.  The server closed the connection.
        /// </summary>
        InvalidCredentials = -1,

        /// <summary>
        /// Indicates the server is too busy to accept further connections.
        /// </summary>
        ServerTooBusy = 1,

        /// <summary>
        /// Indicates a failure to timely exchange credentials within the allowed window.
        /// Server closed the connection.
        /// </summary>
        ConnectionHandshakeTimeout = 2,

        /// <summary>
        /// Indicates the server closed the connection after receiving a corrupted login message.
        /// </summary>
        CorruptedHandshake = 3,

        /// <summary>
        /// Any other failure case for which the connection could not be established.
        /// </summary>
        Unknown = -128
    }
}