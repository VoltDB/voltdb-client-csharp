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

using System.Diagnostics;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides a Trace hookup for VoltDB events (when Logging is turned on the connection).
    /// </summary>
    public static class VoltTrace
    {
        /// <summary>
        /// Trace source to which events are posted.
        /// </summary>
        private static TraceSource source = new TraceSource("voltdb");

        /// <summary>
        /// Static class initializer.
        /// </summary>
        static VoltTrace() {}

        /// <summary>
        /// Listeners that will receive Trace events.
        /// </summary>
        public static TraceListenerCollection Listeners
        {
            get { return source.Listeners; }
        }
        
        /// <summary>
        /// Source switch allowing redefinition of the granularity of tracing output.
        /// </summary>
        public static SourceSwitch Switch
        {
            get { return source.Switch; }
            set { source.Switch = value; }
        }

        /// <summary>
        /// Returns a reference to the static trace source to which events are posted.
        /// </summary>
        internal static TraceSource Source
        {
            get { return source; }
        }

        /// <summary>
        /// Logs an informational message (outside of the scope of natively traced events).
        /// </summary>
        /// <param name="id">Message Id</param>
        /// <param name="msg">Message to submit</param>
        internal static void LogInformation(int id, string msg)
        {
            Source.TraceEvent(TraceEventType.Information, id, msg, VoltTraceEventType.Message, -1);
            Trace.TraceInformation(msg);
        }

        /// <summary>
        /// Logs a warning message (outside of the scope of natively traced events).
        /// </summary>
        /// <param name="id">Message Id</param>
        /// <param name="msg">Message to submit</param>
        internal static void LogWarning(int id, string msg)
        {
            Source.TraceEvent(TraceEventType.Warning, id, msg, VoltTraceEventType.Message, -1);
            Trace.TraceWarning(msg);
        }

        /// <summary>
        /// Logs an error message (outside of the scope of natively traced events).
        /// </summary>
        /// <param name="id">Message Id</param>
        /// <param name="msg">Message to submit</param>
        internal static void LogError(int id, string msg)
        {
            Source.TraceEvent(TraceEventType.Error, id, msg, VoltTraceEventType.Message, -1);
            Trace.TraceError(msg);
        }

        /// <summary>
        /// Trace an event.
        /// </summary>
        /// <param name="eventType">Type of event (standard definition).</param>
        /// <param name="voltdbEventType">VoltDB-specific event type.</param>
        /// <param name="msgFormat">String template for the message to post.</param>
        /// <param name="args">Arguments to pass along (will be formatted in the provided string template).</param>
        internal static void TraceEvent(
                                         TraceEventType eventType
                                       , VoltTraceEventType voltdbEventType
                                       , string msgFormat
                                       , params object[] args
                                       )
        {
            Source.TraceEvent(eventType, (int)voltdbEventType, msgFormat, args);
        }
    }
}
