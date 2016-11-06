/*
 * Created by Samih Soylu
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppUpdate
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "App update";

            /*
             * Checking administrative privileges
             */
            WindowsPrincipal myPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            if (myPrincipal.IsInRole(WindowsBuiltInRole.Administrator) == false)
            {
                Console.WriteLine("\n ## ADMINISTRATOR RIGHTS REQUIRED ##\n  You need to run the application using the \"run as administrator\" option.");
                Console.ReadKey();
                return;
            }

            /*
             * Checking updates
             */
            Update Update = new Update();

            Console.WriteLine(" ");

            // Checks for existing configuration file
            if (!File.Exists("update.bin")) { Console.WriteLine("ERROR: 01"); }

            // Starts update process if configuration file exists
            else
            {
                /* Begining of update process */

                Update.ReadBinaryFile();

                Update.StartDownloadingLatestUpdate();

                Update.ExtractLatestUpdate();

                /* End of update process */

                // Opens file after update process.
                try
                {
                    System.Diagnostics.Process.Start(Update.PathToFileWhichLoadsAfterUpdate);
                }
                catch
                {
                    //Console.WriteLine("\nCOULD NOT FIND [" + update.EXE + "]");
                }

                // Gives the user a chance to read before it quits.
                Thread.Sleep(3000);

                Environment.Exit(0);
            }

            Console.ReadKey();
        }
    }
}