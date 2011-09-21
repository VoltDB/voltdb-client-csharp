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
using System.Linq;
using System.Threading;
using VoltDB.Data.Client;

namespace VoltDB.Examples.Voter
{
    /// <summary>
    /// Class for the sample "Voter Benchmark" application
    /// Flagged "Novice" for the wealth of explanatory comments which, while useful can make the code harder to read
    /// when you understand things well enough.
    /// </summary>
    class VoterBenchmarkNovice
    {
        // A (long) format string for result display - save a lot of "Console.WriteLine"s!
        static string ResultFormat = @"
----------------------------------------------------------------------------------------------------
Vote Results (Contestant / Number of Votes)
----------------------------------------------------------------------------------------------------
{0}
----------------------------------------------------------------------------------------------------
Transaction Results
----------------------------------------------------------------------------------------------------

     Successful Votes : {1,12:##,#}
 Invalid Contestant # : {2,12:##,#}
     Voter over limit : {3,12:##,#}
  Failed Transactions : {4,12:##,#}

----------------------------------------------------------------------------------------------------
Performance Statistics
----------------------------------------------------------------------------------------------------
{5}
----------------------------------------------------------------------------------------------------
Performance Statistics (By Node)
----------------------------------------------------------------------------------------------------
{6}
----------------------------------------------------------------------------------------------------
Procedure Statistics
----------------------------------------------------------------------------------------------------
{7}
----------------------------------------------------------------------------------------------------
Full Performance Statistics
----------------------------------------------------------------------------------------------------
{8}

";

        // We'll track vote results live (Success, Rejection for abuse (3 votes maximum) or invalid entries (wrong
        // contestant id)
        static long[] VoteResults = new long[4];

        /// <summary>
        /// This is our callback method for the Vote procedure.
        /// Note it is strongly typed to receive a response that carries a single INTEGER
        /// </summary>
        /// <param name="response">The procedure response object</param>
        static void VoterCallback(Response<long> response)
        {
            // The response object carries a lot of information with it:
            //  - The name of the procedure and parameters passed
            //  - Details of the response status as returned by the server
            //  - In case of failure, Exception details about the reason
            //  - Finally, what you really care about, a stringly-typed "Result" property that is, well... the result
            //    of the procedure call
            //
            // It is good practice to check that the procedure was successful before getting the result (if you don't
            // and the response failed, you will then get an exception!
            // Callbacks are ALWAYS triggered.  If there is a failure, you can retrieve its details from the Response
            // object - there will not be an Exception throw without you first having a chance to deal with the matter.
            // You can either you the pattern below, or leverage the .TryGetResult(out result) method also available to
            // you on the Response object.
            //
            // Note that because asynchronous calls are, by nature, multi-threaded, you need to code in a thread-safe
            // manner if you modify shared resources.
            // So here, we make sure we use the Interlocked model to increment our counters, a safe, lightweight
            // framework for updating shared values.
            if (response.Status == ResponseStatus.Success)
                Interlocked.Increment(ref VoteResults[(int)response.Result]);
            else
                Interlocked.Increment(ref VoteResults[3]);
        }

        /// <summary>
        /// This is the entry point for the sample application
        /// </summary>
        /// <param name="args">Command line arguments - you may pass a comma or space-separated list of hosts to be
        /// used (otherwise the application's default will be used)</param>
        static void NoviceMain(string[] args)
        {
            try
            {
                // Default hosts
                string hosts = "192.168.1.203,192.168.1.200";

                // If arguments were given, assume it's a host list. I don't know if you space or comma separated them,
                // so I clean all that up
                if (args.Length > 0)
                    hosts = string.Join(",", string.Join(",", args).Split(' ', ','));

                // Initialize some variables for the process
                Random rand = new Random();
                long phoneNumber = 0;
                sbyte contestantId = 0;
                int maximumAllowedVotesPerPhoneNumber = 3;

                // A little introduction message
                Console.WriteLine("Voter Benchmark (2 minutes)\r\n---------------------------------------------------"
                                 + "-------------------------------------------------");

                // You create a connection by calling the VoltConnection.GetConnection static factory method.
                // You may pass a ConnectionSettings object or a connectionstring - the conversion is implicit and the
                // ConnectionSettings object is fully compatible with the standard ConnectionStringBuilder class from
                // which it actually derives.
                // Through action-chaining (Open, Drain, Close and ResetStatistics all return the acting instance
                // instead of 'void', allowing LINQ-like sequencing of actions), you can also immediately open the
                // connection after getting it, which we do here.
                VoltConnection voltDB = VoltConnection.Create("hosts=" + hosts
                                                                    + ";statistics=true")
                                                      .Open();

                // Create strongly-typed procedure wrappers for each procedure we will use.
                //  - Voter will have a callback to track procedure results asynchronously.
                //  - Initialize and Results don't because we will simply call them synchronously.
                var Vote = voltDB.Procedures.Wrap<long, long, sbyte, int>("Vote", VoterCallback);
                var Initialize = voltDB.Procedures.Wrap<long, int, string>("Initialize", null);
                var Results = voltDB.Procedures.Wrap<Table>("Results", null);

                // Initialize the catalog and request a random number of contestants to vote on.
                // Notice the result is strongly-typed, so we can access it directly!
                int numContestants = (int)Initialize.Execute(
                                                         rand.Next(5, 10)
                                                       , "Edwina Burnam,Tabatha Gehling,Kelly Clauss,Jessie Alloway,Alana Bregman,Jessie Eichman,Allie Rogalski,Nita Coster,Kurt Walser,Ericka Dieter,Loraine Nygren,Tania Mattioli"
                                                       ).Result;

                // Print a startup message.
                Console.WriteLine(
                                   "Voting for {0} Contestants\r\nTracking Live Statistics (Connection Aggregates)\r\n"
                                 + "----------------------------------------------------------------------------------"
                                 + "------------------"
                                 , numContestants);

                // Start a display timer that will "tick" every 5 seconds to print live performance data.  We don't
                // really need a context here, so we'll just use an anonymous delegate for the callback.
                Timer ticker = new Timer(
                                          delegate(object state)
                                          {
                                              Console.WriteLine("{0,24}"
                                                  , string.Join(
                                                            "\r\n"
                                                          , (state as VoltConnection)
                                                                .Statistics
                                                                .GetLifetimeStatisticsByNode()
                                                                .Select(r => r.Key.ToString()
                                                                             + " :: "
                                                                             + r.Value.ToString(StatisticsFormat.Short)
                                                                       )
                                                                .ToArray()
                                                               )
                                                               );
                                          }
                                        , voltDB  // Pass the connection
                                        , 5000    // Start in 5 seconds
                                        , 5000    // Tick every 5 seconds
                                        );

                // Run for 2 minutes.  Use the environment's ticks to count time - not very precise but that's not
                // where it matters, and it's low-cost.
                int end = Environment.TickCount + 120000;
                while (Environment.TickCount < end)
                {
                    // Get a random phone number.
                    phoneNumber = 2000000000L + (long)(rand.NextDouble() * 9999999999L);
                    contestantId = unchecked((sbyte)rand.Next(0, numContestants));

                    // Randomly introduce some bad votes (0.1%)
                    if (rand.Next(10000) < 10) contestantId = 11;

                    // Randomly introduce vote abuse (0.05%) by calling with the same phone number more than the
                    // maximum allowed.
                    // Note how we call the BeginExecute method on our procedure wrapper - this is all it takes.  Our
                    // callback will be executed upon completion.
                    // One final note: this could fail: not because of a failure on the server (this is the role of
                    // exception-packaging in the response), but if you lost connectivity and your connection is now
                    // closed!
                    // This means you should use a try/catch block and adequate handling, or leverage the .TryExecute
                    // & .TryExecuteAsync methods on your Procedure Wrapper.
                    // Here we have a global try/catch block that's good enough for a sample app, but vastly inadequate
                    // for a production client, of course!
                    if (rand.Next(20000) < 10)
                        for (int i = 0; i < 5; i++)
                            Vote.BeginExecute(
                                               phoneNumber
                                             , unchecked((sbyte)rand.Next(0, numContestants))
                                             , maximumAllowedVotesPerPhoneNumber
                                             );
                    else
                        Vote.BeginExecute(
                                           phoneNumber
                                         , contestantId
                                         , maximumAllowedVotesPerPhoneNumber
                                         );
                }

                // Drain the connection until all the work is done.
                voltDB.Drain();

                // Stop and dispose of the ticker - we're done here.
                ticker.Dispose();

                // Get the vote results (who's the winner, etc...).
                Table result = Results.Execute().Result;

                // And close the connection.
                voltDB.Close();

                // Enjoy the fully LINQ-compatible Table object:
                string resultDisplay = string.Join(
                                                    "\r\n"
                                                  , result.Rows
                                                          .Select(r => string.Format(
                                                                                      "{0,21} => {1,12:##,#} votes(s)"
                                                                                    , r.GetValue<string>(0)
                                                                                    , r.GetValue<long?>(2)
                                                                                    )
                                                                 ).ToArray()
                                                  )
                                     + string.Format(
                                                      "\r\n\r\n {0,21} was the winner!\r\n"
                                                    , result.Rows
                                                            .OrderByDescending(r => r.GetValue<long?>(2))
                                                            .First()
                                                            .GetValue<string>(0)
                                                    );

                // Print out the results, performance stats, etc.  A fair bit of fancy LINQ there - not the easiest to
                // read, but it's good learning material :-)
                // You could of course do that with loops and 50 lines, but I prefer simple one-liners...
                Console.Write(
                               ResultFormat
                             , resultDisplay
                             , VoteResults[0]
                             , VoteResults[1]
                             , VoteResults[2]
                             , VoteResults[3]

                               // Get the lifetime statistics for the connection.
                             , string.Format("{0,21} :: {1}", "Connection"
                                            , voltDB.Statistics.GetLifetimeStatistics().ToString(StatisticsFormat.Short))

                               // Get lifetime statistics by node.
                             , string.Join("\r\n"
                                          , voltDB.Statistics.GetLifetimeStatisticsByNode()
                                                  .Select(p => string.Format("{0,21} :: {1}"
                                                                            , p.Key
                                                                            , p.Value.ToString(StatisticsFormat.Short)
                                                                            )
                                                         )
                                                  .ToArray()
                                          )

                               // Statistics by procedure, across all nodes.
                             , string.Join("\r\n"
                                          , voltDB.Statistics.GetStatistics(StatisticsSnapshot.SnapshotOnly)
                                                  .Select(p => string.Format("{0,21} :: {1}"
                                                                            , p.Key
                                                                            , p.Value.ToString(StatisticsFormat.Short)
                                                                            )
                                                         )
                                                  .ToArray()
                                          )

                               // And lifetime statistics again, but with the detailed latency information.
                             , voltDB.Statistics.GetLifetimeStatistics().ToString(StatisticsFormat.Default)
                             );
            }
            catch (Exception x)
            {
                // You'll want to be more elaborate than having a global catch block!
                // Might happen if: failed to connect, lose connection during the test (wrap a try/catch around the
                // Vote.BeginExecute call, or use the Vote.TryExecuteAsync call to test the execution was launched
                // successfully!
                Console.WriteLine(x.ToString());
            }
        }
    }
}