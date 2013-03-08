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
using System.IO;
using System.Net;
using System.Net.Sockets;
using VoltDB.Data.Client.Properties;
using VoltDB.Net.Sockets;
using VoltDB.ThirdParty.Mono;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Wraps around a read/write stream to provide basic protocol support for:
    ///  - Validation of raw message format and protocol version.
    ///  - Consistency and optimal performance of data transfers.
    /// </summary>
    internal class VoltProtocolStream
    {
        /// <summary>
        /// Buffer size for In IO
        /// </summary>
        public const int ReceiveBufferSize = 8192;
        /// <summary>
        /// Buffer size for Out IO
        /// </summary>
        public const int SendBufferSize = 8192;

        /// <summary>
        /// Protocol version supported by the library.
        /// </summary>
        private const byte PROTOCOL_VERSION = 0;

        /// <summary>
        /// Maximum message length.
        /// </summary>
        private const int MAX_MESSAGE_LENGTH = 20971520;

        /// <summary>
        /// Underlying networkstream used to interact with the server.
        /// </summary>
        private Stream BaseStream;

        /// <summary>
        /// Incoming data stream.
        /// </summary>
        private Stream In;

        /// <summary>
        /// Outgoing data stream.
        /// </summary>
        private Stream Out;

#if PROTOCOL_BUFFERING
        // Deal with buffered writes
        long lastWrite = DateTime.MaxValue.Ticks;
        System.Threading.Timer flushTimer;
        readonly byte[] lenBuffer = new byte[5];
        Exception outException = null;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="VoltProtocolStream"/> class.
        /// </summary>
        /// <param name="baseStream">The underlying stream to consume.</param>
        public VoltProtocolStream(Stream baseStream)
        {
            this.BaseStream = baseStream;

#if PROTOCOL_BUFFERING
            this.In = new BufferedStream(this.BaseStream, 64 * 1024);
            this.Out = new BufferedStream(this.BaseStream, 4096);
            flushTimer = new System.Threading.Timer(_ =>
            {
                var x = System.Threading.Interlocked.Read(ref lastWrite);
                if (new TimeSpan(DateTime.UtcNow.Ticks - x).TotalMilliseconds > 1)
                {
                    lock (this.Out)
                    {
                         if (this.outException != null) return;
                         x = System.Threading.Interlocked.Read(ref lastWrite);
                         if (new TimeSpan(DateTime.UtcNow.Ticks - x).TotalMilliseconds > 1)
                         {
                             try
                             {
                                 this.Out.Flush();
                             }
                             catch (Exception ex)
                             {
                                 this.outException = ex;
                             }
                             lastWrite = DateTime.MaxValue.Ticks;
                         }
                    }
                }
            });
            flushTimer.Change(10, 10);
#else
            this.In = new BufferedStream(this.BaseStream, 8 * ReceiveBufferSize); // 8K socket receive is 64K buffer
            this.Out = new BufferedStream(this.BaseStream, SendBufferSize);
#endif
        }

        /// <summary>
        /// Creates a socket-based Volt protocol stream.
        /// </summary>
        /// <param name="endPoint">IP end-point to which the socket will be bound.</param>
        /// <param name="timeout">Connection timeout.</param>
        /// <returns>A stream through which messages can be exchanged with a VoltDB server.</returns>
        public VoltProtocolStream(IPEndPoint endPoint, int timeout)
            : this(VoltProtocolStream.CreateSocketStream(endPoint, timeout))
        {
            this.ResetTimeout(timeout);
        }

        /// <summary>
        /// Close the stream and all owned underlying streams.
        /// </summary>
        public void Close()
        {
            this.Out.Close();
            this.Out.Dispose();
            this.In.Close();
            this.In.Dispose();
            this.BaseStream.Close();
            this.BaseStream.Dispose();
        }

        /// <summary>
        /// Resets the timeout value on the underlying stream.
        /// </summary>
        /// <param name="timeout">Timeout (in ms) to set on the underlying streams.</param>
        public void ResetTimeout(int timeout)
        {
            this.BaseStream.ReadTimeout = timeout;
            this.BaseStream.WriteTimeout = timeout;
        }

        /// <summary>
        /// Fully blocking read on the underlying stream: will not return until the given number of bytes has been
        /// fully read.  Will throw EndOfStreamException if not all bytes can be read.
        /// </summary>
        /// <param name="buffer"> Array of <see cref="Byte"/> to store the data read from the stream.</param>
        /// <param name="offset">The offset in buffer at which to begin storing the data read from the current
        /// stream.</param>
        /// <param name="count">Number of bytes to read.</param>
        private void ReadFully(byte[] buffer, int offset, int count)
        {
            int numRead = 0;
            int numToRead = count;
            while (numToRead > 0)
            {
                int read = this.In.Read(buffer, offset + numRead, numToRead);
                if (read == 0)
                    throw new EndOfStreamException();
                numRead += read;
                numToRead -= read;
            }
        }

        /// <summary>
        /// Fully blocking read on the underlying stream: will not return until the given number of bytes has been
        /// fully read.  Will throw EndOfStreamException if not all bytes can be read.
        /// </summary>
        /// <param name="buffer"> Array of <see cref="Byte"/> to store the data read from the stream.</param>
        private void ReadFully(byte[] buffer)
        {
            this.ReadFully(buffer, 0, buffer.Length);
        }

        private readonly byte[] _readMessage_Header = new byte[5]; // Avoid extra allocs
        /// <summary>
        /// Reads a full message from the underlying stream and return the binary data for the message body.
        /// </summary>
        /// <returns>Binary data for the message received.</returns>
        public byte[] ReadMessage()
        {
            // Request next message header.
            this.ReadFully(_readMessage_Header, 0, 5);

            // Retrieve length.
            int length = DataConverter.BigEndian.GetInt32(_readMessage_Header, 0) - 1;

            // Validate Protocol Version.
            if (_readMessage_Header[4] != PROTOCOL_VERSION)
                throw new VoltProtocolException(Resources.ProtocolVersionMismatch, PROTOCOL_VERSION, _readMessage_Header[4]);

            // Validate message length.
            if (length > MAX_MESSAGE_LENGTH)
                throw new VoltProtocolException(Resources.ProtocolMessageLengthInvalid, length, MAX_MESSAGE_LENGTH);

            // And now get the message body.
            byte[] message = new byte[length];
            this.ReadFully(message, 0, length);
            return message;
        }

        /// <summary>
        /// Writes a message to the underlying stream, pre-pending the necessary message header.
        /// </summary>
        /// <param name="message">The binary data of the message to send.</param>
        /// <param name="offset">Offset in bytes.</param>
        /// <param name="length">Number of bytes to send.</param>
        public void WriteMessage(byte[] message, int offset, int length)
        {
#if PROTOCOL_BUFFERING
            lock (this.Out)
            {
                if (this.outException != null)
                {
                    throw this.outException;
                }
                DataConverter.BigEndian.PutBytes(lenBuffer, 0, (int)(length + 1));
                lenBuffer[4] = PROTOCOL_VERSION;
                this.Out.Write(lenBuffer, 0, 5);
                this.Out.Write(message, offset, length);
                this.lastWrite = DateTime.UtcNow.Ticks;
            }
#else
            this.Out.Write(DataConverter.BigEndian.GetBytes((int)(length + 1)), 0, 4);
            this.Out.WriteByte(PROTOCOL_VERSION);
            this.Out.Write(message, offset, length);
            // Perf: Flush is because we use a bufferedstream to coalesce these 3 writes into one write to the socket
            this.Out.Flush();
#endif
        }

        /// <summary>
        /// Creates a socket-based stream for the connection.
        /// </summary>
        /// <param name="endPoint">IP end-point to which the socket will be bound.</param>
        /// <param name="timeout">Connection timeout.</param>
        /// <returns>A stream through which messages can be exchanged with a VoltDB server.</returns>
        private static Stream CreateSocketStream(IPEndPoint endPoint, int timeout)
        {
            // Prepare the socket
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = false;
            //Really a bad idea to hard code these, either make configurable with sane default (8k is not sane)
            //or take the OS configured default. Vista and later will try to autotune the receive buffer size. I am just going
            //to pray that actually works well (brief testing says it does) and choose a medium to large send buffer size
//            socket.ReceiveBufferSize = ReceiveBufferSize;
            socket.SendBufferSize = 1024 * 1024 * 256;
            socket.Blocking = true;
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            // Attempt to open asynchronously.
            IAsyncResult ias = socket.BeginConnect(endPoint, null, null);
            if (!ias.AsyncWaitHandle.WaitOne(timeout, false))
            {
                socket.Close();
                throw new VoltConnectionException(Resources.ConnectionTimeout, endPoint);
            }

            // Finalize connection.
            try
            {
                socket.EndConnect(ias);
            }
            catch (Exception x)
            {
                socket.Close();
                throw new VoltConnectionException(Resources.ConnectionFailure, x, endPoint);
            }

            ManagedNetworkStream stream = new ManagedNetworkStream(socket, true);
            return stream;
        }
    }
}