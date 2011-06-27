/*

 This file is part of VoltDB.
 Copyright (C) 2008-2011 VoltDB Inc.

 Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
 documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
 rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
 persons to whom the Software is furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
 Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS BE
 LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/
using System;
using System.Text;
using VoltDB.Data.Client.Properties;
using VoltDB.ThirdParty.Mono;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Encapsulates all basic deserialization methods for the VoltDB native wire protocol.
    /// For easy command chaining/ease of coding, methods that would not have a return type by their function (such as
    /// skipping forward/backward) return the reading instance.
    /// </summary>
    internal class Deserializer 
    {
        /// <summary>
        /// Internal Bigendian data converter
        /// </summary>
        private DataConverter Cnv = DataConverter.BigEndian;

        /// <summary>
        /// We manage the byte buffer of the message data, as returned from the network stream, internally
        /// (technically, the Deserializer acts as a stream, without specifically implementing the full interface).
        /// </summary>
        private byte[] Input;

        /// <summary>
        /// Current pointer position when reading data out of the byte buffer.
        /// </summary>
        private int Position;

        /// <summary>
        /// Create a new instance of the <see cref="Deserializer"/> class.
        /// </summary>
        /// <param name="input">The byte array to deserialize.</param>
        public Deserializer(byte[] input)
        {
            this.Input = input;
            this.Position = 0;
        }

        /// <summary>
        /// Skip forward (or backward) by the given number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to skip by (negative for rewinding).</param>
        /// <returns>The deserializer instance, ready to chain the next command.</returns>
        public Deserializer Skip(int count)
        {
            this.Position += count;
            return this;
        }

        /// <summary>
        /// Rewinds the virtual stream provided by the deserializer by reseting the Position to the beginning of the
        /// byte array.
        /// </summary>
        /// <returns>The deserializer instance, ready to chain the next command.</returns>
        public Deserializer Rewind()
        {
            this.Position = 0;
            return this;
        }

        /// <summary>
        /// Read a specific number of bytes to be returned as a RAW data array (as-is).
        /// </summary>
        /// <param name="count">Number of bytes to read.</param>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public byte[] ReadRaw(int count)
        {
            byte[] value = new byte[count];
            Buffer.BlockCopy(this.Input, this.Position, value, 0, count);
            this.Position += value.Length;
            return value;
        }

        /// <summary>
        /// Reads a byte/Unsigned-Byte (VoltDB::Tinyint).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public byte ReadByte()
        {
            return this.Input[this.Position++];
        }

        /// <summary>
        /// Reads a Nullable byte/Unsigned-Byte (VoltDB::Tinyint).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public byte? ReadNullableByte()
        {
            byte value = this.Input[this.Position++];
            if (unchecked((sbyte)value) == VoltType.NULL_TINYINT)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Reads a sbyte/Signed-Byte (VoltDB::Tinyint).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public sbyte ReadSByte()
        {
            return unchecked((sbyte)this.Input[this.Position++]);
        }
        /// <summary>
        /// Reads a Nullable sbyte/Signed-Byte (VoltDB::Tinyint).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public sbyte? ReadNullableSByte()
        {
            sbyte value = unchecked((sbyte)this.Input[this.Position++]);
            if (value == VoltType.NULL_TINYINT)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Reads a Short (VoltDB::Smallint).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public short ReadShort()
        {
            short value = Cnv.GetInt16(this.Input, this.Position);
            this.Position += 2;
            return value;
        }
        /// <summary>
        /// Reads a Nullable Short (VoltDB::Smallint).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public short? ReadNullableShort()
        {
            short value = Cnv.GetInt16(this.Input, this.Position);
            this.Position += 2;
            if (value == VoltType.NULL_SMALLINT)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Reads a Int (VoltDB::Integer).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public int ReadInt()
        {
            int value = Cnv.GetInt32(this.Input, this.Position);
            this.Position += 4;
            return value;
        }
        /// <summary>
        /// Reads a Nullable Int (VoltDB::Integer).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public int? ReadNullableInt()
        {
            int value = Cnv.GetInt32(this.Input, this.Position);
            this.Position += 4;
            if (value == VoltType.NULL_INTEGER)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Reads a Long (VoltDB::BigInt).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public long ReadLong()
        {
            long value = Cnv.GetInt64(this.Input, this.Position);
            this.Position += 8;
            return value;
        }
        /// <summary>
        /// Reads a Nullable Long (VoltDB::BigInt).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public long? ReadNullableLong()
        {
            long value = Cnv.GetInt64(this.Input, this.Position);
            this.Position += 8;
            if (value == VoltType.NULL_BIGINT)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Reads a Double (VoltDB::Float).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public double ReadDouble()
        {
            double value = Cnv.GetDouble(this.Input, this.Position);
            this.Position += 8;
            return value;
        }
        /// <summary>
        /// Reads a Nullable Double (VoltDB::Float).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public double? ReadNullableDouble()
        {
            double value = Cnv.GetDouble(this.Input, this.Position);
            this.Position += 8;
            if (value == VoltType.NULL_FLOAT)
                return null;
            else
                return value;
        }

        /// <summary>
        /// Reads a DateTime from a millisecond timestamp (as such in some messaging fields) (VoltDB::Timestamp).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public DateTime ReadDateTimeFromMilliseconds()
        {
            DateTime value = new DateTime(
                                           Cnv.GetInt64(this.Input, this.Position) * 10000 + VoltType.TIMESTAMP_ORIGIN
                                         , DateTimeKind.Utc
                                         );
            this.Position += 8;
            return value;
        }
        /// <summary>
        /// Reads a DateTime from a microsecond long value (VoltDB::Timestamp).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public DateTime ReadDateTime()
        {
            DateTime value = new DateTime(
                                           Cnv.GetInt64(this.Input, this.Position) * 10 + VoltType.TIMESTAMP_ORIGIN
                                         , DateTimeKind.Utc
                                         );
            this.Position += 8;
            return value;
        }
        /// <summary>
        /// Reads a Nullable DateTime (VoltDB::Timestamp).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public DateTime? ReadNullableDateTime()
        {
            long value = Cnv.GetInt64(this.Input, this.Position);
            this.Position += 8;
            if (value == VoltType.NULL_BIGINT)
                return null;
            else
                return new DateTime(value * 10 + VoltType.TIMESTAMP_ORIGIN, DateTimeKind.Utc);
        }

        /// <summary>
        /// Reads a string (VoltDB::Varchar).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public string ReadString()
        {
            int length = Cnv.GetInt32(this.Input, this.Position);
            this.Position += 4;

            if (length == VoltType.NULL_STRING_AND_VARBINARY_INDICATOR)
                return null;

            if (length > VoltType.MAX_VALUE_LENGTH)
                throw new VoltInvalidDataException(
                                                    Resources.MaximumStringLengthViolation
                                                  , length
                                                  , VoltType.MAX_VALUE_LENGTH
                                                  );

            if (length < VoltType.NULL_STRING_AND_VARBINARY_INDICATOR)
                throw new VoltInvalidDataException(Resources.InvalidStringLength, length);

            byte[] buffer = new byte[length];
            Buffer.BlockCopy(this.Input, this.Position, buffer, 0, length);
            this.Position += buffer.Length;
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Reads a varbinary (VoltDB::Varbinary).
        /// </summary>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public byte[] ReadVarbinary()
        {
            int length = Cnv.GetInt32(this.Input, this.Position);
            this.Position += 4;

            if (length == VoltType.NULL_STRING_AND_VARBINARY_INDICATOR)
                return null;

            if (length > VoltType.MAX_VALUE_LENGTH)
                throw new VoltInvalidDataException(
                                                    Resources.MaximumVarbinaryLengthViolation
                                                  , length
                                                  , VoltType.MAX_VALUE_LENGTH
                                                  );

            if (length < VoltType.NULL_STRING_AND_VARBINARY_INDICATOR)
                throw new VoltInvalidDataException(Resources.InvalidVarbinaryLength, length);

            byte[] buffer = new byte[length];
            Buffer.BlockCopy(this.Input, this.Position, buffer, 0, length);
            this.Position += buffer.Length;
            return buffer;
        }

        /// <summary>
        /// Skips a string field (VoltDB::Varchar).
        /// </summary>
        /// <returns>The deserializer iteself, allowing chained calls</returns>
        public Deserializer SkipString()
        {
            int length = Cnv.GetInt32(this.Input, this.Position);
            this.Position += 4;
            if (length != VoltType.NULL_STRING_AND_VARBINARY_INDICATOR)
                this.Position += length;
            return this;
        }

        /// <summary>
        /// Reads a Decimal (VoltDB:Decimal).
        /// </summary>
        /// <remarks>No .NET support for the equivalent BigDecimal data type from Java. At this time, this data type is
        /// not supported by the .NET client library.</remarks>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public VoltDecimal ReadVoltDecimal()
        {
            // Remark: value = (read == -170141183460469231731687303715884105728.) ? null : read;
            byte[] buffer = new byte[16];
            Buffer.BlockCopy(this.Input, this.Position, buffer, 0, 16);
            this.Position += 16;
            return new VoltDecimal(buffer);
        }

        /// <summary>
        /// Reads a Decimal (VoltDB:Decimal).
        /// </summary>
        /// <remarks>No .NET support for the equivalent BigDecimal data type from Java. At this time, this data type is
        /// not supported by the .NET client library.</remarks>
        /// <returns>Value read from the underlying byte buffer.</returns>
        public VoltDecimal? ReadNullableVoltDecimal()
        {
            // Remark: value = (read == -170141183460469231731687303715884105728.) ? null : read;
            byte[] buffer = new byte[16];
            Buffer.BlockCopy(this.Input, this.Position, buffer, 0, 16);
            this.Position += 16;
            VoltDecimal result = new VoltDecimal(buffer);
            if (result.IsVoltDBNull())
                return null;
            return result;
        }

        /// <summary>
        /// Reads an array of Tables (macro data type).
        /// </summary>
        /// <returns>Array of Tables as read from the underlying byte buffer.</returns>
        public Table[] ReadTableArray()
        {
            int count = Cnv.GetInt16(this.Input, this.Position);
            this.Position += 2;
            Table[] result = new Table[count];
            for (int i = 0; i < count; i++)
                result[i] = new Table(this);
            return result;
        }

        /// <summary>
        /// Reads an array of SingleRow-Tables (macro data type).
        /// </summary>
        /// <returns>Array of SingleRow-Tables as read from the underlying byte buffer.</returns>
        public SingleRowTable[] ReadSingleRowTableArray()
        {
            int count = Cnv.GetInt16(this.Input, this.Position);
            this.Position += 2;
            SingleRowTable[] result = new SingleRowTable[count];
            for (int i = 0; i < count; i++)
                result[i] = new SingleRowTable(this);
            return result;
        }

    }
}