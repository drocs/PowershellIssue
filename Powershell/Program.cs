// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Management.Automation;

namespace Application.Test
{
    public class Program
    {
        /// <summary>
        /// Managed entry point shim, which starts the actual program
        /// </summary>
        public static int Main(string[] args)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                ps.Streams.Verbose.DataAdded += VerboseOnDataAdded;
                ps.Streams.Error.DataAdded += ErrorOnDataAdded;

                Console.WriteLine("\nEvaluating 'Get-Command Write-Output' in PS Core Runspace\n");
                var results = ps.AddScript("Get-Command Write-Output").Invoke();
                Console.WriteLine(results[0].ToString());

                ps.Commands.Clear();

                ps.AddScript("Get-PackageProvider nuget").Invoke();
                //Console.WriteLine(results[0].ToString());

                ps.Commands.Clear();

                Console.WriteLine("\nEvaluating '([S.M.A.ActionPreference], [S.M.A.AliasAttribute]).FullName' in PS Core Runspace\n");
                results = ps.AddScript("([System.Management.Automation.ActionPreference], [System.Management.Automation.AliasAttribute]).FullName").Invoke();
                foreach (dynamic result in results)
                {
                    Console.WriteLine(result.ToString());
                }
            }

            Console.ReadKey();
            return 0;
        }

        private static void ErrorOnDataAdded(object sender, DataAddedEventArgs e)
        {
            var err = ((PSDataCollection<ErrorRecord>)sender)[e.Index];
            Console.WriteLine(err.ErrorDetails?.Message??err.Exception.Message);
        }

        private static void VerboseOnDataAdded(object sender, DataAddedEventArgs e)
        {
            var rec = ((PSDataCollection<VerboseRecord>)sender)[e.Index];
            Console.WriteLine(rec.Message);
        }
    }
}