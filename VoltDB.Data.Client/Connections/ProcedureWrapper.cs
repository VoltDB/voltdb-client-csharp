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
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides a wrapper around VoltDB procedures, allowing the definition of strongly-typed execution methods.
    /// </summary>
    public abstract class ProcedureWrapper
    {
        /// <summary>
        /// Internal reference to the procedure executor this wraper was created with (a single connection of a pooled
        /// client).
        /// </summary>
        internal VoltConnection Executor;

        /// <summary>
        /// Name of the VoltDB procedure this wrapper refers to.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Utf8Bytes of the procedure name string.
        /// </summary>
        protected readonly byte[] NameUtf8Bytes;
            
        /// <summary>
        /// Internal storage for command timeout value.
        /// </summary>
        private int _CommandTimeout = 0;
        /// <summary>
        /// Timeout for this procedure wrapper's execution in milliseconds or -1 or Timeout.Infinite for infinite
        /// timeout; 0 for connection's default command timeout.
        /// </summary>
        public int CommandTimeout
        {
            get
            {
                return this._CommandTimeout;
            }
            set
            {
                if (value < -1)
                    throw new ArgumentOutOfRangeException(string.Format(Resources.InvalidTimeoutValue, value));
                this._CommandTimeout = value;
            }
        }

        /// <summary>
        /// Creates a new wrapper.
        /// </summary>
        /// <param name="executor">The connection to refer to when calling for execution.</param>
        /// <param name="name">The name of the VoltDB procedure the wrapper refers to.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        internal ProcedureWrapper(VoltConnection executor, string name, int timeout)
        {
            this.Executor = executor;
            this.Name = name;
            this.NameUtf8Bytes = System.Text.Encoding.UTF8.GetBytes(this.Name);
            this.CommandTimeout = timeout;
        }

        /// <summary>
        /// Changes the connection against which the wrapper will execute (allows for wrapper re-use accross
        /// multiple connections - posted executions in progress will not be impacted by the change as the
        /// response will be managed on the original connection).
        /// </summary>
        /// <param name="connection">The new connection to which the wrapper should be tied.</param>
        public void SetConnection(VoltConnection connection)
        {
            this.Executor = connection;
        }
    }
}