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
using VoltDB.Data.Client.Properties;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Provides the basic support for the Response object, used to parse out return messages from the server and
    /// deserialize their payload into usable .NET objects and values.
    /// </summary>
    public abstract partial class Response
    {
        /// <summary>
        /// Internal enumeration used to determined which of the status fields is available from the server response,
        /// to drive the binary parsing.
        /// </summary>
        [Flags]
        private enum HasFields : byte
        {
            /// <summary>
            /// Flag indicating the response payload has a Server Status string.
            /// </summary>
            Status = 0x20,

            /// <summary>
            /// Flag indicating the response payload has a Server Exception string.
            /// </summary>
            Exception = 0x40,

            /// <summary>
            /// Flag indicating the response payload has a Server Application Status string.
            /// </summary>
            ApplicationStatus = 0x80
        }

        /// <summary>
        /// Flags indicating which status fields are on in the received server response.
        /// </summary>
        private HasFields _HasFields;

        /// <summary>
        /// Status of the response, as provided by the server (details of the server error when the response's status
        /// is set to 'Failed'.
        /// </summary>
        public ResponseServerStatus ServerStatus { get; private set; }

        /// <summary>
        /// Detailed status for the response (in case of failure).
        /// </summary>
        public string ServerStatusString { get; private set; }

        /// <summary>
        /// Application status (custom-set by the procedure on the server-side).
        /// </summary>
        public sbyte ServerApplicationStatus { get; private set; }

        /// <summary>
        /// Detailed application status (custom-set by the procedure on the server-side).
        /// </summary>
        public string ServerApplicationStatusString { get; private set; }

        /// <summary>
        /// Duration of the execution request (ms).
        /// </summary>
        public int ExecutionDuration { get; private set; }

        // <summary>
        // Exception string/binary data (not used in protocol version 0 where it duplicates the content of the server
        // status string, and ignored).  Listed here for information only.
        // </summary>
        // public string ServerExceptionString { get; private set; }

        /// <summary>
        /// Exception data for any deserialization issue, to be provided to the caller as part of the response.
        /// </summary>
        public Exception Exception { get; protected set; }

        /// <summary>
        /// Abstract method: deserialize the payload into a strongly-typed, specific data type.
        /// </summary>
        /// <param name="message">The byte buffer containing the server response message.</param>
        internal abstract void ParseResponse(byte[] message);

        /// <summary>
        /// Parses the returned server message to read properties.
        /// </summary>
        /// <param name="input">The deserializer holding the data form the server message.</param>
        internal void ParseHeader(Deserializer input)
        {
            try
            {
                // Skip the client executionId: the caller (background network thread) already verified a match and
                // we recorded it at the start.
                input.Skip(8);

                // Check field map for status header parsing.
                this._HasFields = (HasFields)input.ReadByte();

                // Read the Status (and optional associated message).
                this.ServerStatus = (ResponseServerStatus)input.ReadSByte();
                if ((this._HasFields & HasFields.Status) == HasFields.Status)
                    this.ServerStatusString = input.ReadString();

                // Read the Application Status (and optional associated message).
                this.ServerApplicationStatus = input.ReadSByte();
                if ((this._HasFields & HasFields.ApplicationStatus) == HasFields.ApplicationStatus)
                    this.ServerApplicationStatusString = input.ReadString();

                // Track query duration (ms).
                this.ExecutionDuration = input.ReadInt32();

                // Skip exception data (currently not used and initialized to the same value as the Status string!).
                if ((this._HasFields & HasFields.Exception) == HasFields.Exception)
                    //this.ServerExceptionString = input.ReadString();
                    input.SkipString();

                // Figure out whether to move forward or kill over - note that we don't *really* kill over, but simply
                // report the Exception in the response's Exception field - the caller should validate that the
                // response is in good status before proceeding to read the results.
                switch (this.ServerStatus)
                {
                    case ResponseServerStatus.Success:
                        break;

                    case ResponseServerStatus.UserAbort:
                        this.Exception = new VoltAbortException(
                                                                 Resources.RequestAborted
                                                               , this.Status
                                                               , this.ServerStatusString
                                                               );
                        break;

                    case ResponseServerStatus.GracefulFailure:
                        this.Exception = new VoltRequestFailureException(
                                                                          Resources.RequestFailure
                                                                        , this.Status
                                                                        , this.ServerStatusString
                                                                        );
                        break;

                    case ResponseServerStatus.UnexpectedFailure:
                        this.Exception = new VoltCriticalRequestFailureException(
                                                                                  Resources.RequestFailure
                                                                                , this.Status
                                                                                , this.ServerStatusString
                                                                                );
                        break;

                    default:
                        this.Exception = new VoltCriticalRequestFailureException(
                                                                                  Resources.ServerFailure
                                                                                , this.Status
                                                                                , this.ServerStatusString
                                                                                );
                        break;
                }
            }
            catch (Exception x)
            {
                this.Exception = new VoltSerializationException(Resources.ResponseDeserializationFailure, x);
            }
        }
    }
}