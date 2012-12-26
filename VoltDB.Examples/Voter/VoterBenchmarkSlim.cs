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
using System.Linq;
using System.Threading;
using VoltDB.Data.Client;

namespace VoltDB.Examples.Voter
{
    /// <summary>
    /// Class for the sample "Voter Benchmark" application / Flagged "Slim" for the lack of comments: you can focus on
    /// the code!
    /// </summary>
    class VoterBenchmarkSlim
    {
        static string line = "\r\n----------------------------------------------------------------------------------------------------\r\n";
        static string ResultFormat = "{9}Vote Results (Contestant / Number of Votes){9}{0}{9}Transaction Results{9}\r\n     Successful Votes : {1,12:##,#}\r\n Invalid Contestant # : {2,12:##,#}\r\n     Voter over limit : {3,12:##,#}\r\n  Failed Transactions : {4,12:##,#}\r\n{9}Performance Statistics{9}{5}{9}Performance Statistics (By Node){9}{6}{9}Procedure Statistics{9}{7}{9}Full Performance Statistics{9}{8}\r\n";

        // Voter procedure callback and counters
        static long[] VoteResults = new long[4];
        static void VoterCallback(Response<long> response)
        {
            if (response.Status == ResponseStatus.Success)
                Interlocked.Increment(ref VoteResults[(int)response.Result]);
            else
                Interlocked.Increment(ref VoteResults[3]);
        }

        // Application entry-point
        static void Main(string[] args)
        {
            try
            {
                // Read hosts from the command or use defaults
                string hosts = "192.168.1.203";
                if (args.Length > 0)
                    hosts = string.Join(",", string.Join(",", args).Split(' ', ','));

                // Initialize some variables for the process
                Random rand = new Random(); long phoneNumber = 0; sbyte contestantId = 0; int maximumAllowedVotesPerPhoneNumber = 3;

                Console.WriteLine("Voter Benchmark (2 minutes)\r\n----------------------------------------------------------------------------------------------------");

                // Create & open connection
                VoltConnection voltDB = VoltConnection.Create("hosts=" + hosts + ";statistics=true;").Open();

                // Create strongly-typed procedure wrappers
                var Vote = voltDB.Procedures.Wrap<long, long, sbyte, int>("Vote", VoterCallback);
                var Initialize = voltDB.Procedures.Wrap<long, int, string>("Initialize", null);
                var Results = voltDB.Procedures.Wrap<Table>("Results", null);

                // Initialize application
                int numContestants = (int)Initialize.Execute(rand.Next(5, 10), "Edwina Burnam,Tabatha Gehling,Kelly Clauss,Jessie Alloway,Alana Bregman,Jessie Eichman,Allie Rogalski,Nita Coster,Kurt Walser,Ericka Dieter,Loraine Nygren,Tania Mattioli").Result;

                Console.WriteLine("Voting for {0} Contestants\r\nTracking Live Statistics (Connection Aggregates)\r\n----------------------------------------------------------------------------------------------------", numContestants);

                // Start display ticker for stats
                Timer ticker = new Timer(delegate(object state) { Console.WriteLine("{0,24}", string.Join("\r\n", (state as VoltConnection).Statistics.GetLifetimeStatisticsByNode().Select(r => r.Key.ToString() + " :: " + r.Value.ToString(StatisticsFormat.Short)).ToArray())); }, voltDB, 5000, 5000);

                /*
                int workerThreads;
                int completionPortThreads;
                ThreadPool.GetMinThreads(out workerThreads, out completionPortThreads);
                ThreadPool.SetMaxThreads(workerThreads*2, completionPortThreads*2);
                */

                // Run for 2 minutes.
                int end = Environment.TickCount + 120000;
                while (Environment.TickCount < end)
                {
                    phoneNumber = 2000000000L + (long)(rand.NextDouble() * 9999999999L);
                    contestantId = unchecked((sbyte)rand.Next(0, numContestants));

                    if (rand.Next(10000) < 10) contestantId = 11;

                    if (rand.Next(20000) < 10)
                        for (int i = 0; i < 5; i++)
                            Vote.BeginExecute(phoneNumber, unchecked((sbyte)rand.Next(0, numContestants)), maximumAllowedVotesPerPhoneNumber);
                    else
                        Vote.BeginExecute(phoneNumber, contestantId, maximumAllowedVotesPerPhoneNumber);
                }

                // Finalize operations and get vote results
                voltDB.Drain();

                ticker.Dispose();
                // Something new compared to the "Novice" version: you don't have to constantly when accessing data:
                // use .GetValue<type>(columnIndex) - simply wrap your table into a strongly-typed accessor and things
                // become a lot easier!
                var resultData = Results.Execute().Result.Wrap<string,int?, long?>();
                voltDB.Close();

                // Compile results & print summary
                string resultDisplay = string.Join("\r\n", resultData.Rows.Select(r => string.Format("{0,21} => {1,12:##,#} votes(s)", r.Column1, r.Column3)).ToArray())
                                     + string.Format("\r\n\r\n {0,21} was the winner!\r\n", resultData.Rows.OrderByDescending(r => r.Column3).First().Column1);
                Console.Write(ResultFormat, resultDisplay, VoteResults[0], VoteResults[1], VoteResults[2], VoteResults[3]
                             , string.Format("{0,21} :: {1}", "Connection", voltDB.Statistics.GetLifetimeStatistics().ToString(StatisticsFormat.Short))
                             , string.Join("\r\n", voltDB.Statistics.GetLifetimeStatisticsByNode().Select(p => string.Format("{0,21} :: {1}", p.Key, p.Value.ToString(StatisticsFormat.Short))).ToArray())
                             , string.Join("\r\n", voltDB.Statistics.GetStatistics(StatisticsSnapshot.SnapshotOnly).Select(p => string.Format("{0,21} :: {1}", p.Key, p.Value.ToString(StatisticsFormat.Short))).ToArray())
                             , voltDB.Statistics.GetLifetimeStatistics().ToString(StatisticsFormat.Default)
                             , line
                             );
            }
            catch (Exception x)
            {
                // You'll want to be more elaborate than having a global catch block!
                Console.WriteLine(x.ToString());
            }
        }
    }
}