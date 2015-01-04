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
using System.IO;
using System.Text;
using VoltDB.Data.Client.Properties;
using VoltDB.ThirdParty.Mono;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Encapsulates all basic serialization methods for the VoltDB native wire protocol.
    /// For easy command chaining/ease of coding, all methods return the writing instance, such that the caller can
    /// write a full message through:
    /// 
    ///     byte[] message;
    ///     using(Serializer s = new Serializer())
    ///         message = s.Write("SomeProcedure")
    ///                    .WriteParameters(Parameter1, Parameter2)
    ///                    .GetBytes();
    ///                
    /// </summary>
    internal class Serializer : IDisposable
    {
        /// <summary>
        /// Internal BigEndian data converter.
        /// </summary>
        private DataConverter Cnv = DataConverter.BigEndian;

        /// <summary>
        /// Internal buffer for the serialization message being constraucted.
        /// </summary>
        private MemoryStream writer;

        /// <summary>
        /// Buffer to avoid allocating lots of little arrays.
        /// </summary>
        private byte[] _tempBuffer = new byte[8];

        /// <summary>
        /// Instantiate a new instance of the Serializer class.
        /// </summary>
        public Serializer()
        {
            writer = new MemoryStream(64); // TODO: Determine best starting size, or perhaps allow caller to specify
        }

        /// <summary>
        /// Finalize the buffer and returns a byte array containing all the data written by the caller.
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> GetBytes()
        {
            writer.Flush();
            writer.Position = 0;
            var buf = writer.GetBuffer();
            return new ArraySegment<byte>(writer.GetBuffer(), 0, (int)writer.Length);
        }

        /// <summary>
        /// Writes out a list of procedure parameters to the stream.
        /// </summary>
        /// <remarks>The type-checking is sub-optimal, however, the nature of the encapsulation, despite use of
        /// generics, required it.</remarks>
        /// <param name="parameters">Procedure parameters to serialize to the message.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer WriteParameters(params object[] parameters)
        {
            Cnv.PutBytes(_tempBuffer, 0, (short)parameters.Length);
            writer.Write(_tempBuffer, 0, 2);
            // PERF: Just use direct typechecks for now.
            // For real perf, we'll use LCG to create custom serialization methods
            // for each procedure.
            foreach (object value in parameters)
            {
                if (value is byte)
                    this.Write(DBType.TINYINT).Write((byte)value);
                else if (value is byte[])
                    this.Write(VoltType.ARRAY).Write((byte[])value);
                else if (value is byte?)
                    this.Write(DBType.TINYINT).Write((byte?)value);
                else if (value is byte?[])
                    this.Write(VoltType.ARRAY).Write((byte?[])value);
                else if (value is sbyte)
                    this.Write(DBType.TINYINT).Write((sbyte)value);
                else if (value is sbyte[])
                    this.Write(VoltType.ARRAY).Write((sbyte[])value);
                else if (value is sbyte?)
                    this.Write(DBType.TINYINT).Write((sbyte?)value);
                else if (value is sbyte?[])
                    this.Write(VoltType.ARRAY).Write((sbyte?[])value);
                else if (value is short)
                    this.Write(DBType.SMALLINT).Write((short)value);
                else if (value is short[])
                    this.Write(VoltType.ARRAY).Write((short[])value);
                else if (value is short?)
                    this.Write(DBType.SMALLINT).Write((short?)value);
                else if (value is short?[])
                    this.Write(VoltType.ARRAY).Write((short?[])value);
                else if (value is int)
                    this.Write(DBType.INTEGER).Write((int)value);
                else if (value is int[])
                    this.Write(VoltType.ARRAY).Write((int[])value);
                else if (value is int?)
                    this.Write(DBType.INTEGER).Write((int?)value);
                else if (value is int?[])
                    this.Write(VoltType.ARRAY).Write((int?[])value);
                else if (value is long)
                    this.Write(DBType.BIGINT).Write((long)value);
                else if (value is long[])
                    this.Write(VoltType.ARRAY).Write((long[])value);
                else if (value is long?)
                    this.Write(DBType.BIGINT).Write((long?)value);
                else if (value is long?[])
                    this.Write(VoltType.ARRAY).Write((long?[])value);
                else if (value is double)
                    this.Write(DBType.FLOAT).Write((double)value);
                else if (value is double[])
                    this.Write(VoltType.ARRAY).Write((double[])value);
                else if (value is double?)
                    this.Write(DBType.FLOAT).Write((double?)value);
                else if (value is double?[])
                    this.Write(VoltType.ARRAY).Write((double?[])value);
                else if (value is DateTime)
                    this.Write(DBType.TIMESTAMP).Write((DateTime)value);
                else if (value is DateTime[])
                    this.Write(VoltType.ARRAY).Write((DateTime[])value);
                else if (value is DateTime?)
                    this.Write(DBType.TIMESTAMP).Write((DateTime?)value);
                else if (value is DateTime?[])
                    this.Write(VoltType.ARRAY).Write((DateTime?[])value);
                else if (value is string)
                    this.Write(DBType.STRING).Write((string)value);
                else if (value is string[])
                    this.Write(VoltType.ARRAY).Write((string[])value);
                else if (value is VoltDecimal?)
                    this.Write(DBType.DECIMAL).Write((VoltDecimal?)value);
                else if (value is VoltDecimal)
                    this.Write(DBType.DECIMAL).Write((VoltDecimal)value);
                else if (value is VoltDecimal[])
                    this.Write(VoltType.ARRAY).Write((VoltDecimal[])value);
                else if (value is VoltDecimal?[])
                    this.Write(VoltType.ARRAY).Write((VoltDecimal?[])value);
                else if (value is decimal?)
                    this.Write(DBType.DECIMAL).Write((decimal?)value);
                else if (value is decimal)
                    this.Write(DBType.DECIMAL).Write((decimal)value);
                else if (value is decimal[])
                    this.Write(VoltType.ARRAY).Write((decimal[])value);
                else if (value is decimal?[])
                    this.Write(VoltType.ARRAY).Write((decimal?[])value);
                else if (value is VoltType.DateTimeDBNull)
                    this.Write(DBType.TIMESTAMP).Write(VoltType.NULL_BIGINT);
                else if (value is VoltType.StringDBNull)
                    this.Write(DBType.STRING).Write((string)null);
                else if (value is VoltType.ByteArrayDBNull)
                    this.Write(DBType.VARBINARY).Write((string)null);
                else
                    throw new VoltUnsupportedTypeException(
                                                            Resources.UnsupportedParameterNETType
                                                          , value.GetType().ToString()
                                                          );
                //}
            }
            return this;
        }

        /// <summary>
        /// Writes a DBType value (indicating the type of the next byte sequence).
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(DBType value)
        {
            writer.WriteByte(unchecked((byte)value));
            return this;
        }

        /// <summary>
        /// Writes a RAW (as-is) byte array to the stream.
        /// </summary>
        /// <param name="value">The value to write</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer WriteRaw(byte[] value)
        {
            writer.Write(value, 0, value.Length);
            return this;
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(byte value)
        {
            writer.WriteByte(value);
            return this;
        }

        /// <summary>
        /// Writes a byte array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(byte[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.TINYINT));
            if (value == null) {
                Cnv.PutBytes(_tempBuffer, 0, -1);
                writer.Write(_tempBuffer, 0, 4);
            } else {
                Cnv.PutBytes(_tempBuffer, 0, value.Length);
                writer.Write(_tempBuffer, 0, 4);
                writer.Write(value, 0, value.Length);
            }
            return this;
        }

        /// <summary>
        /// Writes a Nullable byte.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(byte? value)
        {
            writer.WriteByte((value == null) ? unchecked((byte)VoltType.NULL_TINYINT) : (byte)value);
            return this;
        }

        /// <summary>
        /// Writes a Nullable byte array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(byte?[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.TINYINT));
            Cnv.PutBytes(_tempBuffer, 0, value.Length);
            writer.Write(_tempBuffer, 0, 4);
            for (int i = 0; i < value.Length; i++)
                writer.WriteByte((value[i] == null) ? unchecked((byte)VoltType.NULL_TINYINT) : (byte)value[i]);
            return this;
        }

        /// <summary>
        /// Writes a signed-byte.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(sbyte value)
        {
            writer.WriteByte(unchecked((byte)value));
            return this;
        }

        /// <summary>
        /// Writes a signed-byte array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(sbyte[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.TINYINT));
            Cnv.PutBytes(_tempBuffer, 0, value.Length);
            writer.Write(_tempBuffer, 0, 4);
            for (int i = 0; i < value.Length; i++)
                writer.WriteByte(unchecked((byte)value[i]));
            return this;
        }

        /// <summary>
        /// Writes a Nullable signed-byte.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(sbyte? value)
        {
            writer.WriteByte((value == null) ? unchecked((byte)VoltType.NULL_TINYINT) : unchecked((byte)value));
            return this;
        }

        /// <summary>
        /// Writes a Nullable signed-byte array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(sbyte?[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.TINYINT));
            Cnv.PutBytes(_tempBuffer, 0, value.Length);
            writer.Write(_tempBuffer, 0, 4);
            for (int i = 0; i < value.Length; i++)
                writer.WriteByte(
                                  value[i] == null
                                ? unchecked((byte)VoltType.NULL_TINYINT)
                                : unchecked((byte)value[i])
                                );
            return this;
        }

        /// <summary>
        /// Writes a short (Int16).
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(short value)
        {
            Cnv.PutBytes(_tempBuffer, 0, value);
            writer.Write(_tempBuffer, 0, 2);
            return this;
        }

        /// <summary>
        /// Writes a short (Int16) array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(short[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.SMALLINT));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
            {
                Cnv.PutBytes(_tempBuffer, 0, value[i]);
                writer.Write(_tempBuffer, 0, 2);
            }
            return this;
        }

        /// <summary>
        /// Writes a Nullable short (Int16).
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(short? value)
        {
            Cnv.PutBytes(_tempBuffer, 0, (value == null) ? VoltType.NULL_SMALLINT : (short)value);
            writer.Write(_tempBuffer, 0, 2);
            return this;
        }

        /// <summary>
        /// Writes a Nullable short (Int16) array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(short?[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.SMALLINT));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
            {
                Cnv.PutBytes(_tempBuffer, 0, (value[i] == null) ? VoltType.NULL_SMALLINT : (short)value[i]);
                writer.Write(_tempBuffer, 0, 2);
            }
            return this;
        }

        /// <summary>
        /// Writes an int (Int32).
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(int value)
        {
            Cnv.PutBytes(_tempBuffer, 0, value);
            writer.Write(_tempBuffer, 0, 4);
            return this;
        }

        /// <summary>
        /// Writes an int (Int32) array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(int[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.INTEGER));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
            {
                Cnv.PutBytes(_tempBuffer, 0, value[i]);
                writer.Write(_tempBuffer, 0, 4);
            }
            return this;
        }

        /// <summary>
        /// Writes a Nullable int (Int32).
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(int? value)
        {
            Cnv.PutBytes(_tempBuffer, 0, (value == null) ? VoltType.NULL_INTEGER : (int)value);
            writer.Write(_tempBuffer, 0, 4);
            return this;
        }

        /// <summary>
        /// Writes a Nullable int (Int32) array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(int?[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.INTEGER));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
            {
                Cnv.PutBytes(_tempBuffer, 0, (value[i] == null) ? VoltType.NULL_INTEGER : (int)value[i]);
                writer.Write(_tempBuffer, 0, 4);
            }
            return this;
        }

        /// <summary>
        /// Writes a long (Int64).
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(long value)
        {
            Cnv.PutBytes(_tempBuffer, 0, value);
            writer.Write(_tempBuffer, 0, 8);
            return this;
        }

        /// <summary>
        /// Writes a long (Int64) array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(long[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.BIGINT));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
            {
                Cnv.PutBytes(_tempBuffer, 0, value[i]);
                writer.Write(_tempBuffer, 0, 8);
            }
            return this;
        }

        /// <summary>
        /// Writes a Nullable long (Int64).
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(long? value)
        {
            Cnv.PutBytes(_tempBuffer, 0, (value == null) ? VoltType.NULL_BIGINT : (long)value);
            writer.Write(_tempBuffer, 0, 8);
            return this;
        }

        /// <summary>
        /// Writes a Nullable long (Int64) array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(long?[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.BIGINT));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
            {
                Cnv.PutBytes(_tempBuffer, 0, (value[i] == null) ? VoltType.NULL_BIGINT : (long)value[i]);
                writer.Write(_tempBuffer, 0, 8);
            }
            return this;
        }

        /// <summary>
        /// Writes a double.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(double value)
        {
            Cnv.PutBytes(_tempBuffer, 0, value);
            writer.Write(_tempBuffer, 0, 8);
            return this;
        }

        /// <summary>
        /// Writes a double array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(double[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.FLOAT));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
            {
                Cnv.PutBytes(_tempBuffer, 0, value[i]);
                writer.Write(_tempBuffer, 0, 8);
            }
            return this;
        }

        /// <summary>
        /// Writes a Nullable double.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(double? value)
        {
            Cnv.PutBytes(_tempBuffer, 0, (value == null) ? VoltType.NULL_FLOAT : (double)value);
            writer.Write(_tempBuffer, 0, 8);
            return this;
        }

        /// <summary>
        /// Writes a Nullable double array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(double?[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.FLOAT));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
            {
                Cnv.PutBytes(_tempBuffer, 0, (value[i] == null) ? VoltType.NULL_FLOAT : (double)value[i]);
                writer.Write(_tempBuffer, 0, 8);
            }
            return this;
        }

        /// <summary>
        /// Writes a DateTime.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(DateTime value)
        {
            if (value.Kind == DateTimeKind.Local) value = value.ToUniversalTime();
            Cnv.PutBytes(_tempBuffer, 0, (value.Ticks - VoltType.TIMESTAMP_ORIGIN) / 10L);
            writer.Write(_tempBuffer, 0, 8);
            return this;
        }

        /// <summary>
        /// Writes a DateTime array
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(DateTime[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.TIMESTAMP));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
            {
                Cnv.PutBytes(_tempBuffer, 0, (((DateTime)value[i]).ToUniversalTime().Ticks - VoltType.TIMESTAMP_ORIGIN) / 10L);
                writer.Write(_tempBuffer, 0, 8);
            }
            return this;
        }

        /// <summary>
        /// Writes a Nullable DateTime.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(DateTime? value)
        {
            Cnv.PutBytes(_tempBuffer, 0, value == null ? VoltType.NULL_BIGINT : (((DateTime)value).ToUniversalTime().Ticks - VoltType.TIMESTAMP_ORIGIN) / 10L);
            writer.Write(_tempBuffer, 0, 8);
            return this;
        }

        /// <summary>
        /// Writes a Nullable DateTime array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(DateTime?[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.TIMESTAMP));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
            {
                Cnv.PutBytes(_tempBuffer, 0, value[i] == null ? VoltType.NULL_BIGINT : (((DateTime)value[i]).ToUniversalTime().Ticks - VoltType.TIMESTAMP_ORIGIN) / 10L);
                writer.Write(_tempBuffer, 0, 8);
            }
            return this;
        }

        /// <summary>
        /// Writes a string.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(string value)
        {
            if (value == null) {
                Cnv.PutBytes(_tempBuffer, 0, (int)-1);
                writer.Write(_tempBuffer, 0, 4);
                return this;
            } else {
                return WriteStringInternal(Encoding.UTF8.GetBytes(value));
            }
        }
        internal Serializer WriteStringInternal(byte[] data)
        {
            if (data.Length > VoltType.MAX_VALUE_LENGTH)
                throw new VoltInvalidDataException(
                                                    Resources.MaximumStringLengthViolation
                                                    , data.Length
                                                    , VoltType.MAX_VALUE_LENGTH
                                                    );
            else
            {
                Cnv.PutBytes(_tempBuffer, 0, data.Length);
                writer.Write(_tempBuffer, 0, 4);
                writer.Write(data, 0, data.Length);
            }
            return this;
        }

        /// <summary>
        /// Writes a string array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(string[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.STRING));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
                this.Write(value[i]);
            return this;
        }

        /// <summary>
        /// Writes a BigDecimal.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(VoltDecimal value)
        {
            writer.Write(value.ToBytes(), 0, 16);
            return this;
        }

        /// <summary>
        /// Writes a BigDecimal array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(VoltDecimal[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.DECIMAL));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
                writer.Write(value[i].ToBytes(), 0, 16);
            return this;
        }

        /// <summary>
        /// Writes a Nullable BigDecimal.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(VoltDecimal? value)
        {
            writer.Write((value == null) ? VoltDecimal.NullValueBytes : value.Value.ToBytes(), 0, 16);
            return this;
        }

        /// <summary>
        /// Writes a Nullable BigDecimal array.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <returns>The serializer instance, ready to chain the next command.</returns>
        public Serializer Write(VoltDecimal?[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.DECIMAL));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
                writer.Write((value[i] == null) ? VoltDecimal.NullValueBytes : value[i].Value.ToBytes(), 0, 16);
            return this;
        }

        public Serializer Write(decimal value)
        {
            return Write(new VoltDecimal(value));
        }

        public Serializer Write(decimal[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.DECIMAL));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
                writer.Write(((VoltDecimal)value[i]).ToBytes(), 0, 16);
            return this;
        }

        public Serializer Write(decimal? value)
        {
            return Write((VoltDecimal?)value);
        }

       public Serializer Write(Decimal?[] value)
        {
            writer.WriteByte(unchecked((byte)DBType.DECIMAL));
            Cnv.PutBytes(_tempBuffer, 0, (short)value.Length);
            writer.Write(_tempBuffer, 0, 2);
            for (int i = 0; i < value.Length; i++)
                writer.Write((value[i] == null) ? VoltDecimal.NullValueBytes : ((VoltDecimal)(value[i].Value)).ToBytes(), 0, 16);
            return this;
        }

        #region IDisposable Members
        /// <summary>
        /// Internal flag tracking whether the object was disposed.
        /// </summary>
        private bool IsDisposed = false;

        /// <summary>
        /// Ensures connected resources are disposed.
        /// </summary>
        public void Dispose()
        {
            if (this.IsDisposed)
                return;
            this.IsDisposed = true;
            try
            {
                this.writer.Close();
                this.writer.Dispose();
            }
            catch { }
        }
        #endregion
    }
}
