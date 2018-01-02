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
using System.Net.Sockets;

namespace VoltDB.Net.Sockets
{
    /// <summary>
    /// Wraps around a NetworkStream to provide:
    /// 
    ///  - Proper tagging of Timeout exceptions as such (instead of IOExceptions)
    ///  - Workaround and retry logic for invalid WSAEWOULDBLOCK errors
    ///    (see http://tinyurl.com/lhgpyf for more information)
    ///
    /// </summary>
    internal class ManagedNetworkStream : NetworkStream
    {
        /// <summary>
        /// Maximum number of retries for an IO operation on the underlying socket.
        /// </summary>
        private const int MaxRetryCount = 2;

        /// <summary>
        /// Creates a new NetworkStream around the given socket.
        /// </summary>
        /// <param name="socket">The socket around which the stream is wrapped.</param>
        /// <param name="ownsSocket">Whether the stream owns the socket (and will close it when it is itself
        /// closed).</param>
        public ManagedNetworkStream(Socket socket, bool ownsSocket) : base(socket, ownsSocket) { }

        /// <summary>
        /// Handles or rethrow underlying stream exception:
        ///  - Socket timeout exceptions are wrapped so they appear as TimeoutExceptions.
        ///  - WSAEWOULDBLOCK errors are given a chance for retry after resetting the Blocking property on the
        ///    underlying socket.
        ///  - All other exceptions are rethrown as-is.
        /// </summary>
        /// <param name="source">The <see cref="Exception"/> to analyze.</param>
        private void ManageException(Exception source)
        {
            // Navigate the stack trace until we find a relevant exception
            Exception current = source;
            do
            {
                if (current is SocketException)
                {
                    switch ((current as SocketException).SocketErrorCode)
                    {
                        // Workaround for WSAEWOULDBLOCK
                        case SocketError.WouldBlock:
                            this.Socket.Blocking = true;
                            return;

                        // Return Timeout Exception (instead of IOException)
                        case SocketError.TimedOut:
                            throw new TimeoutException((current as SocketException).Message, source);
                    }
                }
                current = current.InnerException;
            }
            while (current != null);
            
            // If we get here, this is yet another type of Exception - just rethrow it.
            throw (source);
        }

        /// <summary>
        /// Reads data from the NetworkStream.
        /// </summary>
        /// <param name="buffer">An array of type <see cref="Byte"/> that is the location in memory to store data read
        /// from the <see cref="NetworkStream"/>.</param>
        /// <param name="offset">The location in buffer to begin storing the data to.</param>
        /// <param name="count">The number of bytes to read from the <see cref="NetworkStream"/>.</param>
        /// <returns>The number of bytes read from the <see cref="NetworkStream"/>.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            // Perform retries around WSAEWOULDBLOCK errors
            int retry = 0;
            Exception exception = null;
            do
            {
                try
                {
                    return base.Read(buffer, offset, count);
                }
                catch (Exception x)
                {
                    exception = x;
                    ManageException(x);
                }
            }
            while (++retry < MaxRetryCount);
            
            // If we get here, the underlying method ultimately failed - rethrow the exception for that failure.
            // Note: for socket timeout operations, this will have been converted into an actual TimeoutException
            // wrapping the source Exception.
            throw exception;
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at
        /// the end of the stream.
        /// </summary>
        /// <returns>The unsigned byte cast to an Int32, or -1 if at the end of the stream.</returns>
        public override int ReadByte()
        {
            // Perform retries around WSAEWOULDBLOCK errors
            int retry = 0;
            Exception exception = null;
            do
            {
                try
                {
                    return base.ReadByte();
                }
                catch (Exception x)
                {
                    exception = x;
                    ManageException(x);
                }
            }
            while (++retry < MaxRetryCount);
            
            // If we get here, the underlying method ultimately failed - rethrow the exception for that failure.
            // Note: for socket timeout operations, this will have been converted into an actual TimeoutException
            // wrapping the source Exception.
            throw exception;
        }

        /// <summary>
        /// Writes data to the <see cref="NetworkStream"/>.
        /// </summary>
        /// <param name="buffer">An array of type <see cref="Byte"/> that contains the data to write to the
        ///  <see cref="NetworkStream"/>.</param>
        /// <param name="offset">The location in buffer from which to start writing data.</param>
        /// <param name="count">The number of bytes to write to the <see cref="NetworkStream"/>.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            // Perform retries around WSAEWOULDBLOCK errors
            int retry = 0;
            Exception exception = null;
            do
            {
                try
                {
                    base.Write(buffer, offset, count);
                    return;
                }
                catch (Exception x)
                {
                    exception = x;
                    ManageException(x);
                }
            }
            while (++retry < MaxRetryCount);
            
            // If we get here, the underlying method ultimately failed - rethrow the exception for that failure.
            // Note: for socket timeout operations, this will have been converted into an actual TimeoutException
            // wrapping the source Exception.
            throw exception;
        }

        /// <summary>
        /// Flushes data from the stream. This method is reserved for future use.
        /// </summary>
        public override void Flush()
        {
            // Perform retries around WSAEWOULDBLOCK errors
            int retry = 0;
            Exception exception = null;
            do
            {
                try
                {
                    base.Flush();
                    return;
                }
                catch (Exception x)
                {
                    exception = x;
                    ManageException(x);
                }
            }
            while (++retry < MaxRetryCount);
            
            // If we get here, the underlying method ultimately failed - rethrow the exception for that failure.
            // Note: for socket timeout operations, this will have been converted into an actual TimeoutException
            // wrapping the source Exception.
            throw exception;
        }

    }
}
