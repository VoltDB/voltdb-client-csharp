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
using VoltDB.Data.Client;

namespace VoltDB.Examples.HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Read hosts from the command or use defaults
                string hosts = "10.10.180.176";
                if (args.Length > 0)
                    hosts = string.Join(",", string.Join(",", args).Split(' ', ','));

                // Create a connection and open it immediately.
                // Notice the "using" block that will ensure the connection is closed and disposed of at the end.
                using (var connection = VoltConnection.Create("hosts=" + hosts).Open())
                {
                    // Define the procedure wrappers we will use
                    var Insert = connection.Procedures.Wrap<Null, string, string, string>("Insert");
                    var Select = connection.Procedures.Wrap<SingleRowTable, string>("Select");

                    // Load the database - we initialize once only, so consecutive runs won't fail.
                    if (!Select.Execute("English").Result.HasData)
                    {
                        Insert.Execute("Hello", "World", "English");
                        Insert.Execute("Bonjour", "Monde", "French");
                        Insert.Execute("Hola", "Mundo", "Spanish");
                        Insert.Execute("Hej", "Verden", "Danish");
                        Insert.Execute("Ciao", "Mondo", "Italian");
                    }

                    // Get the result and wrap into a strongly-typed single-row table
                    var result = Select.Execute("Spanish").Result.Wrap<string, string>();

                    // Print out the answer
                    if (result.HasData)
                        Console.WriteLine("{0}, {1}", result.Column1, result.Column2);
                    else
                        Console.WriteLine("I can't say Hello in that language!");
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }

        }
    }
}
