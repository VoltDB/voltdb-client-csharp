/* This file is part of VoltDB.
 * Copyright (C) 2008-2016 VoltDB Inc.
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
    /// Provides a strongly-typed Generic wrapper around server responses.
    /// </summary>
    public partial class Response<TResult> : Response
    {
        /// <summary>
        /// Parse the response header (deferred to the based class) and the result Payload.
        /// </summary>
        /// <param name="message">The byte buffer of the binary response data from the server.</param>
        internal override void ParseResponse(byte[] message)
        {
            var input = new Deserializer(message);
            // Parse header information first, deferring to base class
            this.ParseHeader(input);

            // Return immediately if header parsing failed
            if (this.ServerStatus != ResponseServerStatus.Success)
                return;

            // Parse the result
            try
            {
                // Unfortunately this is ugly: have to dynamically check the type to figure out which path to take
                // - no generic implementation can seamlessly deal with T, T[] and T[][]
                Type type = typeof(TResult);

                // Array type - either of:
                //  - Table[] an array of generic Tables
                //  - SingleRowTable[] an array of generic single-row Tables
                //  - T[][] an array of generic single-column Tables
                //  - T[] a single single-column Table
                if (type.IsArray)
                {
                    // Get sub-type within the array
                    Type elementType = type.GetElementType();

                    // We have a Table Array
                    if (elementType == typeof(Table))
                        this._Result = (TResult)(object)input.ReadTableArray();

                    // We have a SingleRowTable aArray
                    else if (elementType == typeof(SingleRowTable))
                        this._Result = (TResult)(object)input.ReadSingleRowTableArray();

                    // We have an array of single-column tables
                    else if (elementType.IsArray)
                    {
                        short count = input.ReadInt16();
                        Array result = Array.CreateInstance(elementType, count);
                        for (short i = 0; i < count; i++)
                            result.SetValue(Table.FromSingleColumn(input, elementType.GetElementType()), i);
                        this._Result = (TResult)(object)result;
                    }

                    // We have a single single-column table
                    else
                    {
                        short count = input.ReadInt16();
                        if (count != 1)
                            throw new VoltInvalidDataException(Resources.InvalidResultsetSize, count);
                        if (elementType == typeof(byte)) elementType = typeof(byte[]);
                        this._Result = (TResult)Table.FromSingleColumn(input, elementType);
                    }
                }
                // Base type - either of:
                //  - Table
                //  - SingleRowTable
                //  - T
                else
                {
                    if (type == typeof(Null))
                        this._Result = (TResult)(object)new Null();
                    else
                    {
                        // Read count: there should be only 1 data set for those calls
                        short count = input.ReadInt16();
                        if (count != 1)
                            throw new VoltInvalidDataException(Resources.InvalidResultsetSize, count);

                        // A single Table
                        if (type == typeof(Table))
                            this._Result = (TResult)(object)new Table(input);

                        // A single single-row Table
                        else if (type == typeof(SingleRowTable))
                            this._Result = (TResult)(object)new SingleRowTable(input);

                        // A single value
                        else
                            this._Result = (TResult)Table.FromSingleValue<TResult>(input);
                    }
                }
            }
            catch (Exception x)
            {
                this.Exception = new VoltSerializationException(Resources.ResponseDeserializationFailure, x);
            }
        }
    }
}