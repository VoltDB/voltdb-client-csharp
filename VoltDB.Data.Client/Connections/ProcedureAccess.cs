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
using System.Text.RegularExpressions;
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Dedicated connection plugin class for procedure wrapper creation.
    /// </summary>
    public class ProcedureAccess
    {
        /// <summary>
        /// Internal reference to the connection/executor that will run the requests.
        /// </summary>
        private VoltConnection Executor;

        /// <summary>
        /// Creates a new instance of this class using the provided executor.  Internal usage only: the executor
        /// itselt will validate it has adhoc access before hooking up this class.
        /// </summary>
        /// <param name="executor">Connection that will execute the requests.</param>
        internal ProcedureAccess(VoltConnection executor)
        {
            this.Executor = executor;
        }

        /// <summary>
        /// Regular expression used to validate tokens (procedure names only for now) (will make sure the name doesn't
        /// have any @ sign, or other invalid character for that matter).
        /// </summary>
        private Regex TokenValidator = new Regex("^[a-zA-Z0-9_]*$");
        
        /// <summary>
        /// Validate the procedure name (alpahnumeric only, no @)
        /// </summary>
        /// <param name="procedureName">Name of the procedure</param>
        private void ValidateProcedureName(string procedureName)
        {
            // Validate system procedure access
            if (!TokenValidator.IsMatch(procedureName))
                throw new VoltPermissionException(Resources.InvalidProcedureName, procedureName);
        }    
            
        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 0 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult> Wrap<TResult>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 0 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult> Wrap<TResult>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 0 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult> Wrap<TResult>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 0 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult> Wrap<TResult>(string procedureName, int timeout)
        {
            return this.Wrap<TResult>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 0 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult> Wrap<TResult>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 0 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult> Wrap<TResult>(string procedureName)
        {
            return this.Wrap<TResult>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 1 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1> Wrap<TResult, T1>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 1 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1> Wrap<TResult, T1>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 1 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1> Wrap<TResult, T1>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 1 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1> Wrap<TResult, T1>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 1 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1> Wrap<TResult, T1>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 1 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1> Wrap<TResult, T1>(string procedureName)
        {
            return this.Wrap<TResult, T1>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 2 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2> Wrap<TResult, T1, T2>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 2 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2> Wrap<TResult, T1, T2>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 2 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2> Wrap<TResult, T1, T2>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 2 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2> Wrap<TResult, T1, T2>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 2 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2> Wrap<TResult, T1, T2>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 2 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2> Wrap<TResult, T1, T2>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 3 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3> Wrap<TResult, T1, T2, T3>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 3 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3> Wrap<TResult, T1, T2, T3>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 3 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3> Wrap<TResult, T1, T2, T3>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 3 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3> Wrap<TResult, T1, T2, T3>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 3 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3> Wrap<TResult, T1, T2, T3>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 3 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3> Wrap<TResult, T1, T2, T3>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 4 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4> Wrap<TResult, T1, T2, T3, T4>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 4 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4> Wrap<TResult, T1, T2, T3, T4>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 4 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4> Wrap<TResult, T1, T2, T3, T4>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 4 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4> Wrap<TResult, T1, T2, T3, T4>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 4 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4> Wrap<TResult, T1, T2, T3, T4>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 4 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4> Wrap<TResult, T1, T2, T3, T4>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 5 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5> Wrap<TResult, T1, T2, T3, T4, T5>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 5 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5> Wrap<TResult, T1, T2, T3, T4, T5>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 5 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5> Wrap<TResult, T1, T2, T3, T4, T5>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 5 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5> Wrap<TResult, T1, T2, T3, T4, T5>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 5 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5> Wrap<TResult, T1, T2, T3, T4, T5>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 5 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5> Wrap<TResult, T1, T2, T3, T4, T5>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 6 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6> Wrap<TResult, T1, T2, T3, T4, T5, T6>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 6 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6> Wrap<TResult, T1, T2, T3, T4, T5, T6>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 6 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6> Wrap<TResult, T1, T2, T3, T4, T5, T6>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 6 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6> Wrap<TResult, T1, T2, T3, T4, T5, T6>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 6 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6> Wrap<TResult, T1, T2, T3, T4, T5, T6>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 6 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6> Wrap<TResult, T1, T2, T3, T4, T5, T6>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 7 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 7 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 7 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 7 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 7 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 7 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 8 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 8 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 8 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 8 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 8 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 8 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 9 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 9 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 9 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 9 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 9 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 9 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 10 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 10 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 10 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 10 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 10 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 10 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 11 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 11 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 11 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 11 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 11 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 11 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 12 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 12 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 12 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 12 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 12 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 12 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 13 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 13 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 13 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 13 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 13 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 13 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 14 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 14 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 14 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 14 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 14 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 14 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 15 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 15 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 15 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 15 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 15 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 15 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 16 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 16 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 16 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 16 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 16 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 16 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 17 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 17 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 17 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 17 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 17 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 17 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 18 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 18 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 18 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 18 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 18 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 18 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 19 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 19 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 19 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 19 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 19 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 19 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 20 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 20 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 20 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 20 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 20 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 20 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 21 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 21 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 21 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 21 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 21 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 21 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 22 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 22 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 22 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 22 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 22 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 22 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 23 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 23 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 23 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 23 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 23 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 23 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 24 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 24 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 24 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 24 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 24 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 24 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 25 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 25 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 25 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 25 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 25 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 25 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 26 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 26 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 26 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 26 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 26 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 26 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 27 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 27 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 27 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 27 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 27 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 27 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 28 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 28 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 28 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 28 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 28 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 28 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 29 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 29 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 29 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 29 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 29 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 29 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 30 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 30 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 30 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 30 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 30 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 30 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 31 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 31 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 31 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 31 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 31 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 31 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 32 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 32 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 32 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 32 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 32 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 32 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 33 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 33 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 33 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 33 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 33 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 33 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 34 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 34 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 34 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 34 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 34 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 34 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34>(procedureName, 0, null);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 35 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <typeparam name="T35">Type of parameter 35 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(string procedureName, int timeout, ExecuteAsyncCallback<TResult> callback)
        {
            this.ValidateProcedureName(procedureName);
            return new ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(this.Executor, procedureName, timeout, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 35 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <typeparam name="T35">Type of parameter 35 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <param name="timeout">Procedure-specific default command timeout in milliseconds or -1 or Timeout.Infinite
        /// for infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(string procedureName, TimeSpan timeout, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(procedureName, (int)timeout.TotalMilliseconds, callback);
        }

        /// <summary>
        /// Creates a procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's procedures.
        /// This method creates a wrapper for a procedure that takes 35 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <typeparam name="T35">Type of parameter 35 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="callback">Callback to execute upon completion of the procedure execution (for Async calls).
        ///  To use different callbacks, create multiple procedure wrappers for the same procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(string procedureName, ExecuteAsyncCallback<TResult> callback)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(procedureName, 0, callback);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 35 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <typeparam name="T35">Type of parameter 35 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(string procedureName, int timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(procedureName, timeout, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 35 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <typeparam name="T35">Type of parameter 35 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <param name="timeout">Procedure-specific command timeout in milliseconds or -1 or Timeout.Infinite for
        /// infinite timeout; 0 for connection's default command timeout.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(string procedureName, TimeSpan timeout)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(procedureName, (int)timeout.TotalMilliseconds, null);
        }

        /// <summary>
        /// Creates a callback-less procedure wrapper providing compact, strongly-typed access to a VoltDB catalog's
        /// procedures.  The AsyncResponse will have to be picked up by calling the wrapper's .EndExecute(IAsyncResult)
        /// method with the handle returned by one of the .BeginExecute(...) calls.
        /// This method creates a wrapper for a procedure that takes 35 arguments.
        /// The first type parameter defines the type of the result (for instance Table[] or Table or SingleRowTable,
        /// etc...), the other type parameters define the type of the procedure's parameters themselves.
        /// </summary>
        /// <typeparam name="TResult">Return type of the procedure.</typeparam>
        /// <typeparam name="T1">Type of parameter 1 of the procedure.</typeparam>
        /// <typeparam name="T2">Type of parameter 2 of the procedure.</typeparam>
        /// <typeparam name="T3">Type of parameter 3 of the procedure.</typeparam>
        /// <typeparam name="T4">Type of parameter 4 of the procedure.</typeparam>
        /// <typeparam name="T5">Type of parameter 5 of the procedure.</typeparam>
        /// <typeparam name="T6">Type of parameter 6 of the procedure.</typeparam>
        /// <typeparam name="T7">Type of parameter 7 of the procedure.</typeparam>
        /// <typeparam name="T8">Type of parameter 8 of the procedure.</typeparam>
        /// <typeparam name="T9">Type of parameter 9 of the procedure.</typeparam>
        /// <typeparam name="T10">Type of parameter 10 of the procedure.</typeparam>
        /// <typeparam name="T11">Type of parameter 11 of the procedure.</typeparam>
        /// <typeparam name="T12">Type of parameter 12 of the procedure.</typeparam>
        /// <typeparam name="T13">Type of parameter 13 of the procedure.</typeparam>
        /// <typeparam name="T14">Type of parameter 14 of the procedure.</typeparam>
        /// <typeparam name="T15">Type of parameter 15 of the procedure.</typeparam>
        /// <typeparam name="T16">Type of parameter 16 of the procedure.</typeparam>
        /// <typeparam name="T17">Type of parameter 17 of the procedure.</typeparam>
        /// <typeparam name="T18">Type of parameter 18 of the procedure.</typeparam>
        /// <typeparam name="T19">Type of parameter 19 of the procedure.</typeparam>
        /// <typeparam name="T20">Type of parameter 20 of the procedure.</typeparam>
        /// <typeparam name="T21">Type of parameter 21 of the procedure.</typeparam>
        /// <typeparam name="T22">Type of parameter 22 of the procedure.</typeparam>
        /// <typeparam name="T23">Type of parameter 23 of the procedure.</typeparam>
        /// <typeparam name="T24">Type of parameter 24 of the procedure.</typeparam>
        /// <typeparam name="T25">Type of parameter 25 of the procedure.</typeparam>
        /// <typeparam name="T26">Type of parameter 26 of the procedure.</typeparam>
        /// <typeparam name="T27">Type of parameter 27 of the procedure.</typeparam>
        /// <typeparam name="T28">Type of parameter 28 of the procedure.</typeparam>
        /// <typeparam name="T29">Type of parameter 29 of the procedure.</typeparam>
        /// <typeparam name="T30">Type of parameter 30 of the procedure.</typeparam>
        /// <typeparam name="T31">Type of parameter 31 of the procedure.</typeparam>
        /// <typeparam name="T32">Type of parameter 32 of the procedure.</typeparam>
        /// <typeparam name="T33">Type of parameter 33 of the procedure.</typeparam>
        /// <typeparam name="T34">Type of parameter 34 of the procedure.</typeparam>
        /// <typeparam name="T35">Type of parameter 35 of the procedure.</typeparam>
        /// <param name="procedureName">Name of the procedure.</param>
        /// <returns>A strongly-typed procedure wrapper that can be executed against this connection.</returns>
        public ProcedureWrapper<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35> Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(string procedureName)
        {
            return this.Wrap<TResult, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32, T33, T34, T35>(procedureName, 0, null);
        }

    }
}
