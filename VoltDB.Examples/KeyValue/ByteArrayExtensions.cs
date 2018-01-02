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
using System.IO;
using System.IO.Compression;

namespace VoltDB.Examples.KeyValue
{
    /// <summary>
    /// Define extension methods for the different compression types of our array data.
    /// </summary>
    static class ByteArrayExtensions
    {
        /// <summary>
        /// Compresses a byte array through GZip
        /// </summary>
        /// <param name="data">Data to compress</param>
        /// <returns>Compressed data</returns>
        public static byte[] Gzip(this byte[] data)
        {
            MemoryStream buffer = new MemoryStream();
            GZipStream compressor = new GZipStream(buffer, CompressionMode.Compress, true);
            compressor.Write(data, 0, data.Length);
            compressor.Close();
            buffer.Flush();
            buffer.Position = 0;
            byte[] output = new byte[buffer.Length];
            buffer.Read(output, 0, output.Length);
            buffer.Close();
            return output;
        }

        /// <summary>
        /// Decompresses a GZipped byte array
        /// </summary>
        /// <param name="data">Data to decompress</param>
        /// <returns>Uncompressed data</returns>
        public static byte[] UnGzip(this byte[] data)
        {
            MemoryStream result = new MemoryStream();
            using (MemoryStream source = new MemoryStream(data))
            {
                using (GZipStream decompress = new GZipStream(source, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[data.Length];
                    int numRead;
                    while ((numRead = decompress.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        result.Write(buffer, 0, numRead);
                    }
                }
            }
            result.Flush();
            result.Position = 0;
            byte[] output = new byte[result.Length];
            result.Read(output, 0, output.Length);
            result.Close();
            return output;
        }

        /// <summary>
        /// Compresses a byte array through Deflate
        /// </summary>
        /// <param name="data">Data to compress</param>
        /// <returns>Compressed data</returns>
        public static byte[] Deflate(this byte[] data)
        {
            MemoryStream buffer = new MemoryStream();
            DeflateStream compressor = new DeflateStream(buffer, CompressionMode.Compress, true);
            compressor.Write(data, 0, data.Length);
            compressor.Close();
            buffer.Flush();
            buffer.Position = 0;
            byte[] output = new byte[buffer.Length];
            buffer.Read(output, 0, output.Length);
            buffer.Close();
            return output;
        }

        /// <summary>
        /// Decompresses a Deflated byte array
        /// </summary>
        /// <param name="data">Data to decompress</param>
        /// <returns>Uncompressed data</returns>
        public static byte[] UnDeflate(this byte[] data)
        {
            MemoryStream result = new MemoryStream();
            using (MemoryStream source = new MemoryStream(data))
            {
                using (DeflateStream decompress = new DeflateStream(source, CompressionMode.Decompress))
                {
                    byte[] buffer = new byte[data.Length];
                    int numRead;
                    while ((numRead = decompress.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        result.Write(buffer, 0, numRead);
                    }
                }
            }
            result.Flush();
            result.Position = 0;
            byte[] output = new byte[result.Length];
            result.Read(output, 0, output.Length);
            result.Close();
            return output;
        }

        /// <summary>
        /// Converts a byte array to a base64-encoded string
        /// </summary>
        /// <param name="data">Data to encode</param>
        /// <returns>Encoded data</returns>
        public static string ToBase64(this byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// Converts a base64-encoded string to the original byte data it was generated from
        /// </summary>
        /// <param name="data">Data to decode</param>
        /// <returns>Decoded data</returns>
        public static byte[] FromBase64(this string data)
        {
            return Convert.FromBase64String(data);
        }
    }
}