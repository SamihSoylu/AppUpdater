/*
 * Created by Samih Soylu
 *
 * Notes:
 *      - files must be name version1.zip, version2.zip and onwards.
 *      - update.bin has been hard coded as the binary file that contains all the settings.
 *
 * Error Codes:
 *      01 - Missing file
 *           update.bin is missing, you must put it in to the same diretory as AppUpdate.exe
 *
 *      23 - Corrupted file
 *           FileManager.cs cannot read the file. It is corrupted. Update.bin has most likely been tampered
 *
 *      24 - File does not exist
 *           FileManager.cs cannot open file or does not have permissions to access it.
 *
 *      25 - File exists
 *           Filemanager.cs cannot create a new file because it exists
 *
 *      26 - File in use
 *           FileManager.cs cannot write to file because it is currently in use and has no access.
 *
 *
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