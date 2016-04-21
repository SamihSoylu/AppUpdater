using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Principal;
using System.IO;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
             Console.Title = "Dark Times Launcher";

             WindowsPrincipal myPrincipal = new WindowsPrincipal (WindowsIdentity .GetCurrent());
             if (myPrincipal.IsInRole(WindowsBuiltInRole .Administrator) == false )
             {
              //show messagebox - displaying a messange to the user that rights are missing
                 Console.WriteLine("\n ## ADMINISTRATOR RIGHTS REQUIRED ##\n  You need to run the application using the \"run as administrator\" option.");
              Console.ReadKey();
                 return;
             }

            Update update = new Update();

            update.executeInstantly = true;

            Console.WriteLine("");

            if (!File.Exists("updater.bin")) {

                update.setup();
                
            } else {

                update.initialization(); // reads updater.bin
                
                update.process(); // downloads latest update

                update.finalization(); // extracts latest updates, re-writes updater.bin

                try
                {
                    System.Diagnostics.Process.Start(update.EXE); // opens up exe file after update
                }
                catch
                {
                    //Console.WriteLine("\nCOULD NOT FIND [" + update.EXE + "]");
                }

                Thread.Sleep(3000);

                Environment.Exit(0);
                
            }
        }
    }
}
