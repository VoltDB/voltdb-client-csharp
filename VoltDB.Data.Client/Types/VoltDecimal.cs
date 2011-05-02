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
using VoltDB.Data.Client.Properties;
using VoltDB.ThirdParty.Math;

namespace VoltDB.Data.Client
{
    /// <summary>
    /// Define the Volt-specific decimal(38,12) structure over the standard Java-like implementation of BigDecimal.
    /// Full operator support provided.  Some methods removed, consistent with the fixed-sclae behavior.
    /// </summary>
    /// <remarks>All conversion and mathematical operations maintain the fixed scale at 12 digits -> some operations
    /// might lose precision or throw overflow exceptions.</remarks>
    public struct VoltDecimal
    {
        /// <summary>
        /// Fixed scale definition
        /// </summary>
        private static int FixedScale = 12;

        /// <summary>
        /// MinValue definition
        /// </summary>
        public static BigDecimal MinValue = new BigDecimal("-99999999999999999999999999.999999999999");

        /// <summary>
        /// MaxValue definition
        /// </summary>
        public static BigDecimal MaxValue = new BigDecimal("99999999999999999999999999.999999999999");

        /// <summary>
        /// NullValue definition
        /// </summary>
        public static BigDecimal NullValue = new BigDecimal("-170141183460469231731687303.715884105728");

        /// <summary>
        /// Returns whether the variable corresponds to a null VoltDB value.
        /// </summary>
        /// <returns>True if the value held by the variable converts to a null value in a VoltDB database.</returns>
        public bool IsVoltDBNull()
        {
            return this.Value == VoltDecimal.NullValue;
        }

        /// <summary>
        /// Returns whether a given BigDecimal value fits within the restrictions of the VoltDecimal type.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>True if the value either represents 'NULL' or fits within the Volt decimal boundaries.  False otherwise.</returns>
        private static bool IsValidValue(BigDecimal value)
        {
            if ((value == VoltDecimal.NullValue)
                || ((value >= VoltDecimal.MinValue) && (value <= VoltDecimal.MaxValue)))
                return true;
            return false;
        }

        /// <summary>
        /// Checks a value and, in case of validation failure throws an exception.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        private void ValidOrThrow(BigDecimal value)
        {
            if (!IsValidValue(value))
                throw new ArgumentOutOfRangeException(string.Format(Resources.VoltDecimalOutsideOfRange, value));
        }

        /// <summary>
        /// Underlying BigDecimal value that this instance wraps.
        /// </summary>
        private BigDecimal Value;

        /// <summary>
        /// Creates a new instance of the BigDecimal data type, from the deserialization data.
        /// </summary>
        /// <param name="data">Binary data as provided by the server's response.</param>
        public VoltDecimal(byte[] data)
        {
            this.Value = BigDecimal.FromBytes(data, 12);
        }

        /// <summary>
        /// Returns the bytes for thie BigDecimal instance, for serialization.
        /// </summary>
        /// <returns>The binary data for this BigDecimal instance, ready to feed to a serialized server
        /// request.</returns>
        public byte[] ToBytes()
        {
            return this.Value.ToBytes(12);
        }

        /// <summary>
        /// Creates a new decimal from a generic BigDecimal value.
        /// </summary>
        /// <param name="num">Value used for initialization.</param>
        public VoltDecimal(BigDecimal num)
        {
            this.Value = num;
            this.Value.setScale(FixedScale);
            ValidOrThrow(this.Value);
        }

        /// <summary>
        /// Creates a new decimal from a long value.
        /// </summary>
        /// <param name="num">Value used for initialization.</param>
        public VoltDecimal(long num)
        {
            this.Value = new BigDecimal(num);
            this.Value.setScale(FixedScale);
            ValidOrThrow(this.Value);
        }

        /// <summary>
        /// Creates a new decimal from a ulong value.
        /// </summary>
        /// <param name="num">Value used for initialization.</param>
        public VoltDecimal(ulong num)
        {
            this.Value = new BigDecimal(num);
            this.Value.setScale(FixedScale);
            ValidOrThrow(this.Value);
        }

        /// <summary>
        /// Creates a new decimal from a double value.
        /// </summary>
        /// <param name="num">Value used for initialization.</param>
        public VoltDecimal(double num)
        {
            this.Value = new BigDecimal(num);
            this.Value.setScale(FixedScale);
            ValidOrThrow(this.Value);
        }

        /// <summary>
        /// Creates a new decimal from a BigInteger value.
        /// </summary>
        /// <param name="num">Value used for initialization.</param>
        public VoltDecimal(BigInteger num)
        {
            this.Value = new BigDecimal(num);
            this.Value.setScale(FixedScale);
            ValidOrThrow(this.Value);
        }

        /// <summary>
        /// Creates a new decimal from a BigInteger value and optional scale (will be redefined to FixedScale after original initialization).
        /// </summary>
        /// <param name="num">Value used for initialization.</param>
        /// <param name="scale">Scale of the decimal value to create.</param>
        public VoltDecimal(BigInteger num, int scale)
        {
            this.Value = new BigDecimal(num, scale);
            this.Value.setScale(FixedScale);
            ValidOrThrow(this.Value);
        }

        /// <summary>
        /// Creates a new decimal from a string value.
        /// </summary>
        /// <param name="num">Value used for initialization.</param>
        public VoltDecimal(string num)
        {
            this.Value = new BigDecimal(num);
            this.Value.setScale(FixedScale);
            ValidOrThrow(this.Value);
        }

        /// <summary>
        /// Returns the unscaled value for this decimal.
        /// </summary>
        public BigInteger UnscaledValue
        {
            get
            {
                return this.Value.unscaledValue;
            }
        }

        /// <summary>
        /// Operator definition: binary +
        /// </summary>
        /// <param name="bd1">First operand.</param>
        /// <param name="bd2">Second operand.</param>
        /// <returns>Result of the operation</returns>
        public static VoltDecimal operator +(VoltDecimal bd1, VoltDecimal bd2)
        {
            return new VoltDecimal(bd1.Value + bd2.Value);
        }

        /// <summary>
        /// Operator definition: unary -
        /// </summary>
        /// <param name="bd">Operand.</param>
        /// <returns>Result of the operation.</returns>
        public static VoltDecimal operator -(VoltDecimal bd)
        {
            return new VoltDecimal(-bd.Value);
        }

        /// <summary>
        /// Operator definition: binary +
        /// </summary>
        /// <param name="bd1">First operand.</param>
        /// <param name="bd2">Second operand.</param>
        /// <returns>Result of the operation</returns>
        public static VoltDecimal operator -(VoltDecimal bd1, VoltDecimal bd2)
        {
            return new VoltDecimal(bd1.Value - bd2.Value);
        }

        /// <summary>
        /// Operator definition: binary *
        /// </summary>
        /// <param name="bd1">First operand.</param>
        /// <param name="bd2">Second operand.</param>
        /// <returns>Result of the operation</returns>
        public static VoltDecimal operator *(VoltDecimal bd1, VoltDecimal bd2)
        {
            return new VoltDecimal(bd1.Value * bd2.Value);
        }

        /// <summary>
        /// Operator definition: binary &lt;
        /// </summary>
        /// <param name="bd1">First operand.</param>
        /// <param name="bd2">Second operand.</param>
        /// <returns>Result of the operation</returns>
        public static bool operator <(VoltDecimal bd1, VoltDecimal bd2)
        {
            return bd1.Value < bd2.Value;
        }

        /// <summary>
        /// Operator definition: binary &gt;
        /// </summary>
        /// <param name="bd1">First operand.</param>
        /// <param name="bd2">Second operand.</param>
        /// <returns>Result of the operation</returns>
        public static bool operator >(VoltDecimal bd1, VoltDecimal bd2)
        {
            return bd1.Value > bd2.Value;
        }

        /// <summary>
        /// Operator definition: binary &lt;=
        /// </summary>
        /// <param name="bd1">First operand.</param>
        /// <param name="bd2">Second operand.</param>
        /// <returns>Result of the operation</returns>
        public static bool operator <=(VoltDecimal bd1, VoltDecimal bd2)
        {
            return !(bd1 > bd2);
        }

        /// <summary>
        /// Operator definition: binary &gt;=
        /// </summary>
        /// <param name="bd1">First operand.</param>
        /// <param name="bd2">Second operand.</param>
        /// <returns>Result of the operation</returns>
        public static bool operator >=(VoltDecimal bd1, VoltDecimal bd2)
        {
            return !(bd1 < bd2);
        }

        /// <summary>
        /// Operator definition: binary ==
        /// </summary>
        /// <param name="bd1">First operand.</param>
        /// <param name="bd2">Second operand.</param>
        /// <returns>Result of the operation</returns>
        public static bool operator ==(VoltDecimal bd1, VoltDecimal bd2)
        {
            return bd1.Value == bd2.Value;
        }

        /// <summary>
        /// Operator definition: binary !=
        /// </summary>
        /// <param name="bd1">First operand.</param>
        /// <param name="bd2">Second operand.</param>
        /// <returns>Result of the operation</returns>
        public static bool operator !=(VoltDecimal bd1, VoltDecimal bd2)
        {
            return !(bd1 == bd2);
        }

        /// <summary>
        /// Operator definition: Conversion to (BigDecimal)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static explicit operator BigDecimal(VoltDecimal val)
        {
            return val.Value;
        }

        /// <summary>
        /// Operator definition: Conversion to (BigInteger)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static explicit operator BigInteger(VoltDecimal val)
        {
            return (BigInteger)val.Value;
        }

        /// <summary>
        /// Operator definition: Conversion to (long)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static explicit operator long(VoltDecimal val)
        {
            return (long)val.Value;
        }

        /// <summary>
        /// Operator definition: Conversion to (float)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static explicit operator float(VoltDecimal val)
        {
            return (float)val.Value;
        }

        /// <summary>
        /// Operator definition: Conversion to (double)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static explicit operator double(VoltDecimal val)
        {
            return (double)val.Value;
        }

        /// <summary>
        /// Operator definition: Conversion to (decimal)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static explicit operator decimal(VoltDecimal val)
        {
            return (decimal)val.Value;
        }

        /// <summary>
        /// Operator definition: Conversion from (BigDecimal)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator VoltDecimal(BigDecimal val)
        {
            return new VoltDecimal(val);
        }

        /// <summary>
        /// Operator definition: Conversion from (long)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator VoltDecimal(long val)
        {
            return (new VoltDecimal(val));
        }

        /// <summary>
        /// Operator definition: Conversion from (ulong)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator VoltDecimal(ulong val)
        {
            return (new VoltDecimal(val));
        }

        /// <summary>
        /// Operator definition: Conversion from (int)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator VoltDecimal(int val)
        {
            return (new VoltDecimal(val));
        }

        /// <summary>
        /// Operator definition: Conversion from (uint)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator VoltDecimal(uint val)
        {
            return (new VoltDecimal(val));
        }

        /// <summary>
        /// Operator definition: Conversion from (double)
        /// </summary>
        /// <param name="val">Value to convert.</param>
        public static implicit operator VoltDecimal(double val)
        {
            return (new VoltDecimal(val));
        }

        /// <summary>
        /// Moves the decimal point to the left (essentially: divide by 10^n)
        /// </summary>
        /// <param name="n">Number of digits to shift by.</param>
        /// <returns></returns>
        public VoltDecimal MovePointLeft(int n)
        {
            return new VoltDecimal(this.Value.MovePointLeft(n));
        }

        /// <summary>
        /// Moves the decimal point to the left (essentially: multiply by 10^n)
        /// </summary>
        /// <param name="n">Number of digits to shift by.</param>
        /// <returns></returns>
        public VoltDecimal MovePointRight(int n)
        {
            return new VoltDecimal(this.Value.MovePointRight(n));
        }

        /// <summary>
        /// Override ToString operator to provide a string representation of the number.
        /// </summary>
        /// <returns>String representation of the number</returns>
        public override string ToString()
        {
            if (this.IsVoltDBNull())
                return "null";
            else
                return this.Value.ToString();
        }

        /// <summary>
        /// Overrides Equals operator
        /// </summary>
        /// <param name="o">Object to compare this instance with.</param>
        /// <returns>True if objects are equal.</returns>
        public override bool Equals(object o)
        {
            return this == (VoltDecimal)o;
        }

        /// <summary>
        /// Returns a hashcode for the current instance.
        /// </summary>
        /// <returns>Hashcode for the instance (support for equality comparison)</returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

    }
}