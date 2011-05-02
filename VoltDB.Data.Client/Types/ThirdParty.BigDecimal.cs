//************************************************************************************
// BigInteger Class Version 1.03
//
// Copyright (C) 2005-2006 Dmitry S. Kataev
// Copyright (c) 2002 Chew Keong TAN
// All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, provided that the above
// copyright notice(s) and this permission notice appear in all copies of
// the Software and that both the above copyright notice(s) and this
// permission notice appear in supporting documentation.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT
// OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL
// INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING
// FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT,
// NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION
// WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//
//
// Disclaimer
// ----------
// Although reasonable care has been taken to ensure the correctness of this
// implementation, this code should never be used in any application without
// proper verification and testing.  I disclaim all liability and responsibility
// to any person or entity with respect to any loss or damage caused, or alleged
// to be caused, directly or indirectly, by the use of this BigInteger class.
//
// Comments, bugs and suggestions to
// (http://www.codeproject.com/csharp/biginteger.asp)
//
//
// Overloaded Operators +, -, *, /, %, >>, <<, ==, !=, >, <, >=, <=, &, |, ^, ++, --, ~
//
// Features
// --------
// 1) Arithmetic operations involving large signed integers (2's complement).
// 2) Primality test using Fermat little theorm, Rabin Miller's method,
//    Solovay Strassen's method and Lucas strong pseudoprime.
// 3) Modulo exponential with Barrett's reduction.
// 4) Inverse modulo.
// 5) Pseudo prime generation.
// 6) Co-prime generation.
//
//
// Known Problem
// -------------
// This pseudoprime passes my implementation of
// primality test but failed in JDK's isProbablePrime test.
//
//       byte[] pseudoPrime1 = { (byte)0x00,
//             (byte)0x85, (byte)0x84, (byte)0x64, (byte)0xFD, (byte)0x70, (byte)0x6A,
//             (byte)0x9F, (byte)0xF0, (byte)0x94, (byte)0x0C, (byte)0x3E, (byte)0x2C,
//             (byte)0x74, (byte)0x34, (byte)0x05, (byte)0xC9, (byte)0x55, (byte)0xB3,
//             (byte)0x85, (byte)0x32, (byte)0x98, (byte)0x71, (byte)0xF9, (byte)0x41,
//             (byte)0x21, (byte)0x5F, (byte)0x02, (byte)0x9E, (byte)0xEA, (byte)0x56,
//             (byte)0x8D, (byte)0x8C, (byte)0x44, (byte)0xCC, (byte)0xEE, (byte)0xEE,
//             (byte)0x3D, (byte)0x2C, (byte)0x9D, (byte)0x2C, (byte)0x12, (byte)0x41,
//             (byte)0x1E, (byte)0xF1, (byte)0xC5, (byte)0x32, (byte)0xC3, (byte)0xAA,
//             (byte)0x31, (byte)0x4A, (byte)0x52, (byte)0xD8, (byte)0xE8, (byte)0xAF,
//             (byte)0x42, (byte)0xF4, (byte)0x72, (byte)0xA1, (byte)0x2A, (byte)0x0D,
//             (byte)0x97, (byte)0xB1, (byte)0x31, (byte)0xB3,
//       };
//
//
// Change Log
// ----------
// 1) September 23, 2002 (Version 1.03)
//    - Fixed operator- to give correct data length.
//    - Added Lucas sequence generation.
//    - Added Strong Lucas Primality test.
//    - Added integer square root method.
//    - Added setBit/unsetBit methods.
//    - New isProbablePrime() method which do not require the
//      confident parameter.
//
// 2) August 29, 2002 (Version 1.02)
//    - Fixed bug in the exponentiation of negative numbers.
//    - Faster modular exponentiation using Barrett reduction.
//    - Added getBytes() method.
//    - Fixed bug in ToHexString method.
//    - Added overloading of ^ operator.
//    - Faster computation of Jacobi symbol.
//
// 3) August 19, 2002 (Version 1.01)
//    - Big integer is stored and manipulated as unsigned integers (4 bytes) instead of
//      individual bytes this gives significant performance improvement.
//    - Updated Fermat's Little Theorem test to use a^(p-1) mod p = 1
//    - Added isProbablePrime method.
//    - Updated documentation.
//
// 4) August 9, 2002 (Version 1.0)
//    - Initial Release.
//
//
// References
// [1] D. E. Knuth, "Seminumerical Algorithms", The Art of Computer Programming Vol. 2,
//     3rd Edition, Addison-Wesley, 1998.
//
// [2] K. H. Rosen, "Elementary Number Theory and Its Applications", 3rd Ed,
//     Addison-Wesley, 1993.
//
// [3] B. Schneier, "Applied Cryptography", 2nd Ed, John Wiley & Sons, 1996.
//
// [4] A. Menezes, P. van Oorschot, and S. Vanstone, "Handbook of Applied Cryptography",
//     CRC Press, 1996, www.cacr.math.uwaterloo.ca/hac
//
// [5] A. Bosselaers, R. Govaerts, and J. Vandewalle, "Comparison of Three Modular
//     Reduction Functions," Proc. CRYPTO'93, pp.175-186.
//
// [6] R. Baillie and S. S. Wagstaff Jr, "Lucas Pseudoprimes", Mathematics of Computation,
//     Vol. 35, No. 152, Oct 1980, pp. 1391-1417.
//
// [7] H. C. Williams, "�douard Lucas and Primality Testing", Canadian Mathematical
//     Society Series of Monographs and Advance Texts, vol. 22, John Wiley & Sons, New York,
//     NY, 1998.
//
// [8] P. Ribenboim, "The new book of prime number records", 3rd edition, Springer-Verlag,
//     New York, NY, 1995.
//
// [9] M. Joye and J.-J. Quisquater, "Efficient computation of full Lucas sequences",
//     Electronics Letters, 32(6), 1996, pp 537-538.
//
//************************************************************************************

using System;
using System.Globalization;
using System.Text;

namespace VoltDB.ThirdParty.Math
{
#pragma warning disable  1591
    public struct BigDecimal
    {
        private BigInteger biNumber;
        private int iScale;
        private static string DecimalSeparator = CultureInfo.InvariantCulture.NumberFormat.CurrencyDecimalSeparator;
        private static string GroupSeparator = CultureInfo.InvariantCulture.NumberFormat.CurrencyGroupSeparator;

        public BigDecimal(long num)
        {
            biNumber = new BigInteger(num);
            iScale = 0;
        }

        public BigDecimal(ulong num)
        {
            biNumber = new BigInteger(num);
            iScale = 0;
        }

        public BigDecimal(double num)
            : this(num.ToString(CultureInfo.InvariantCulture))
        {
        }

        public BigDecimal(BigInteger num)
            : this(num, 0)
        {
        }

        public BigDecimal(BigInteger num, int scale)
        {
            biNumber = num;
            iScale = scale;
        }

        public BigDecimal(string num)
        {
            string int_part;
            string float_part;
            int s = 0;
            int cur_off = 0;

            // initially iScale = 0 and biNumber is null
            iScale = 0;

            // empty string = 0
            if (num.Trim().Length == 0)
            {
                biNumber = new BigInteger(0);
                return;
            }

            // find the decimal separator and exponent position
            num = num.Replace(GroupSeparator, string.Empty);
            int dot_index = num.IndexOf(DecimalSeparator, cur_off);
            int exp_index = num.IndexOf('e', cur_off);
            if (exp_index < 0)
                exp_index = num.IndexOf('E', cur_off);

            // check number format
            if (num.IndexOf(DecimalSeparator, dot_index + 1) >= 0)
                throw new FormatException();
            int last_exp_index = num.IndexOf('e', exp_index + 1);
            if (last_exp_index < 0)
                last_exp_index = num.IndexOf('E', exp_index + 1);
            if (last_exp_index >= 0)
                throw new FormatException();

            // if this number is integer
            if (dot_index < 0)
            {
                // with exponent
                if (exp_index >= 0)
                {
                    // extract integer part
                    int_part = num.Substring(0, exp_index - cur_off).TrimStart('0');
                    cur_off = exp_index + 1;

                    // extract exponent value
                    if (cur_off < num.Length)
                        s = int.Parse(num.Substring(cur_off), CultureInfo.InvariantCulture);

                    if (int_part.Trim().Length == 0)
                        biNumber = new BigInteger(0);
                    else
                    {
                        biNumber = new BigInteger(int_part,10);
                        if (biNumber != 0)
                            iScale = -s;
                    }
                }
                else
                {
                    // just get integer part
                    int_part = num.TrimStart('0');
                    if (int_part.Trim().Length == 0)
                        biNumber = new BigInteger(0);
                    else
                        biNumber = new BigInteger(int_part, 10);
                }

                return;
            }

            // extract integer part
            int_part = num.Substring(0, dot_index - cur_off).TrimStart('0');
            cur_off = dot_index + 1;

            string float_num = int_part;

            // if exponent doesn't exist just extract float part 
            if (exp_index < 0)
            {
                float_part = num.Substring(cur_off).TrimEnd('0');
                float_num += float_part;
                if (float_num.Trim().Length == 0)
                    biNumber = new BigInteger(0);
                else
                {
                    biNumber = new BigInteger(float_num, 10);
                    if (biNumber != 0)
                        iScale = float_part.Length;
                }
                return;
            }

            // extract float part
            float_part = num.Substring(cur_off, exp_index - cur_off).TrimEnd('0');
            cur_off = exp_index + 1;

            // get exponent value
            if (cur_off < num.Length)
                s = int.Parse(num.Substring(cur_off), CultureInfo.InvariantCulture);

            float_num += float_part;
            if (float_num.Trim().Length == 0)
                biNumber = new BigInteger(0);
            else
            {
                biNumber = new BigInteger(float_num, 10);
                if (biNumber != 0)
                    iScale = float_part.Length - s;
            }
        }

        public int Scale
        {
            get
            {
                return iScale;
            }
        }

        public BigDecimal setScale(int val)
        {
            BigInteger ten = new BigInteger(10);
            BigInteger num = biNumber;
            if (val > iScale)
                for (int i = 0; i < val - iScale; i++)
                    num *= ten;
            else
                for (int i = 0; i < iScale - val; i++)
                    num /= ten;
            return new BigDecimal(num, val);
        }

        public BigInteger unscaledValue
        {
            get
            {
                return biNumber;
            }
        }

        public static BigDecimal operator +(BigDecimal bd1, BigDecimal bd2)
        {
            int scale = (bd1.Scale > bd2.Scale ? bd1.Scale : bd2.Scale);
            return new BigDecimal(bd1.setScale(scale).unscaledValue + bd2.setScale(scale).unscaledValue, scale);
        }

        public static BigDecimal operator -(BigDecimal bd)
        {
            return new BigDecimal(-bd.unscaledValue, bd.Scale);
        }

        public static BigDecimal operator -(BigDecimal bd1, BigDecimal bd2)
        {
            int scale = (bd1.Scale > bd2.Scale ? bd1.Scale : bd2.Scale);
            return new BigDecimal(bd1.setScale(scale).unscaledValue - bd2.setScale(scale).unscaledValue, scale);
        }

        public static BigDecimal operator *(BigDecimal bd1, BigDecimal bd2)
        {
            return new BigDecimal(bd1.unscaledValue * bd2.unscaledValue, bd1.Scale + bd2.Scale);
        }

        public static bool operator <(BigDecimal bd1, BigDecimal bd2)
        {
            int scale = (bd1.Scale > bd2.Scale ? bd1.Scale : bd2.Scale);
            return bd1.setScale(scale).unscaledValue < bd2.setScale(scale).unscaledValue;
        }

        public static bool operator >(BigDecimal bd1, BigDecimal bd2)
        {
            int scale = (bd1.Scale > bd2.Scale ? bd1.Scale : bd2.Scale);
            return bd1.setScale(scale).unscaledValue > bd2.setScale(scale).unscaledValue;
        }

        public static bool operator <=(BigDecimal bd1, BigDecimal bd2)
        {
            return !(bd1 > bd2);
        }

        public static bool operator >=(BigDecimal bd1, BigDecimal bd2)
        {
            return !(bd1 < bd2);
        }

        public static bool operator ==(BigDecimal bd1, BigDecimal bd2)
        {
            if (object.Equals(bd1, null) && !object.Equals(bd2, null)) return false;
            if (!object.Equals(bd1, null) && object.Equals(bd2, null)) return false;
            if (object.Equals(bd1, null) && object.Equals(bd2, null)) return true;
            int scale = (bd1.Scale > bd2.Scale ? bd1.Scale : bd2.Scale);
            return bd1.setScale(scale).unscaledValue == bd2.setScale(scale).unscaledValue;
        }

        public static bool operator !=(BigDecimal bd1, BigDecimal bd2)
        {
            return !(bd1 == bd2);
        }

        public static explicit operator BigInteger(BigDecimal val)
        {
            return val.setScale(0).unscaledValue;
        }

        public static explicit operator long(BigDecimal val)
        {
            return (long)val.setScale(0).unscaledValue;
        }

        public static explicit operator float(BigDecimal val)
        {
            return float.Parse(val.ToString(), CultureInfo.InvariantCulture);
        }

        public static explicit operator double(BigDecimal val)
        {
            return double.Parse(val.ToString(), CultureInfo.InvariantCulture);
        }

        public static explicit operator decimal(BigDecimal val)
        {
            return decimal.Parse(val.ToString(), CultureInfo.InvariantCulture);
        }

        public static implicit operator BigDecimal(long val)
        {
            return (new BigDecimal(val));
        }

        public static implicit operator BigDecimal(ulong val)
        {
            return (new BigDecimal(val));
        }

        public static implicit operator BigDecimal(int val)
        {
            return (new BigDecimal(val));
        }

        public static implicit operator BigDecimal(uint val)
        {
            return (new BigDecimal(val));
        }

        public static implicit operator BigDecimal(double val)
        {
            return (new BigDecimal(val));
        }

        public BigDecimal MovePointLeft(int n)
        {
            if (n >= 0)
                return new BigDecimal(biNumber, iScale + n);
            else
                return MovePointRight(-n);
        }

        public BigDecimal MovePointRight(int n)
        {
            if (n >= 0)
            {
                if (iScale >= n)
                    return new BigDecimal(biNumber, iScale - n);
                else
                {
                    BigInteger ten = new BigInteger(10);
                    BigInteger num = biNumber;
                    for (int i = 0; i < n - iScale; i++)
                        num *= ten;

                    return new BigDecimal(num);
                }
            }
            else
                return MovePointLeft(-n);
        }

        public override string ToString()
        {
            string s_num = biNumber.ToString();
            int s_scale = iScale;
            if (biNumber < 0)
                s_num = s_num.Remove(0, 1);
            if (s_scale < 0)
            {
                s_num = s_num.PadRight(s_num.Length - s_scale, '0');
                s_scale = 0;
            }
            if (s_scale >= s_num.Length)
                s_num = s_num.PadLeft(s_scale + 1, '0');

            s_num = (biNumber >= 0 ? string.Empty : "-") + s_num.Insert(s_num.Length - s_scale, DecimalSeparator);

            if (s_num.EndsWith(DecimalSeparator))
                s_num = s_num.Remove(s_num.Length - DecimalSeparator.Length, DecimalSeparator.Length);

            return s_num;
        }

        public override bool Equals(object o)
        {
            return this == (BigDecimal)o;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static BigDecimal FromBytes(byte[] data, int scale)
        {
            return new BigDecimal(new BigInteger(data), scale);
        }

        public byte[] ToBytes()
        {
            return this.biNumber.GetSerializationBytes();
        }
        public byte[] ToBytes(int scale)
        {
            this.setScale(scale);
            return this.ToBytes();
        }
    }
}
