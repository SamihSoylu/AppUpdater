using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Net;
using System.Net.Http;
using System.IO;


namespace Updater
{
    [Serializable]
    class Update
    {

        JSON json = new JSON(); // encodes/decodes bin file
        FileManager filemanager = new FileManager(); // saves/reads bin file
        FileDownloader downloader = new FileDownloader(); // downloads latest update
        ArchiveManager archive = new ArchiveManager(); // extracts latest update

        // Initalized variables
        private string URL;
        public string EXE;
        private int VERSION; // max: 2,147,483,647
        private int SERV_VERSION;
        private bool processable, finalizable;
        public bool executeInstantly;

        // Sets variable values from JSON Class
        public void reDefineVariables()
        {
            this.VERSION = json.updateVersion;
            this.URL = json.updateUrl;
            this.EXE = json.whenFinishedLaunch;
        }

        // ------------- Begin of Setup Method -------------
        public void setup()
        {
            Console.WriteLine("Looks like your the configuartion file is missing.\n");

            bool createNewConfig = true;

            // While there are errors the questions will be asked again
            while (createNewConfig)
            {

                // Asks user if they want to create a new configuration
                Console.Write("\nDo you want to create a new config? (Y / N): ");
                string res = Convert.ToString(Console.ReadLine());

                bool madeMistakes = true;

                // If they want to create a new configuration
                if (res == "Y" || res == "y")
                {

                    Console.WriteLine("\n# Please make sure update.exe is in the same directory #");
                    Console.WriteLine("# as the file that will be updated using this program. #");

                    while (madeMistakes)
                    {

                        // Asks for user update URL
                        Console.Write("\nEnter the URL to check for updates: ");
                        URL = Convert.ToString(Console.ReadLine());

                        // Asks for file name to open after completition
                        Console.Write("Enter the file name that will be opened once updates are complete (Example: client.exe): ");
                        EXE = Convert.ToString(Console.ReadLine());

                        // Ask if user made a mistake above
                        Console.Write("Did you make a mistake above? Press Y to restart or any other key to continue: ");
                        res = Convert.ToString(Console.ReadLine());

                        if (res == "y" || res == "Y")
                        {
                            madeMistakes = true;
                        }
                        else { madeMistakes = false; }
                    }

                    // Goes ahead creating config file
                    Console.WriteLine("--Creating config file.");
                    Thread.Sleep(1000);

                    if (filemanager.CreateConfig(json.Encode(URL, EXE)))
                    {
                        Console.WriteLine("--Writing to config file.");
                        Thread.Sleep(1000);

                        Console.WriteLine("--Complete! Closing terminal..");
                        Thread.Sleep(2000);
                        
                    }

                    createNewConfig = false;
                    Environment.Exit(0);
                   


                }
                else if (res == "n" || res == "N")
                {
                    Environment.Exit(0);
                }
            }
        }
        // ------------- End of Setup Method -------------

        // ------------- Begin of Initalization Method -------------
        public void initialization()
        {
            string readFile;
            bool error = true;

            Console.WriteLine("INITIALIZATION: [START]");
            if (!executeInstantly)
                Thread.Sleep(3000);

            readFile = filemanager.ReadConfig();
            if (readFile != null)
            {
                if (json.Decode(readFile))
                {
                    reDefineVariables();
                    error = false;
                    Console.WriteLine("INITIALIZATION: [END]\n");
                }
            }

            if (error) { processable = false; return; } // errors are thrown by the methods

            Console.WriteLine("CONNECTION: [ATTEMPT]");
            if (!executeInstantly)
                Thread.Sleep(2000);

            try
            {
                HttpWebRequest conn = (HttpWebRequest)WebRequest.Create(URL + "version.txt");
                HttpWebResponse response = (HttpWebResponse)conn.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());
                SERV_VERSION = Convert.ToInt32(reader.ReadToEnd());

                reader.Close();
                response.Close();

                Console.WriteLine("CONNECTION: [SUCCESS]");

                if (!executeInstantly)
                    Thread.Sleep(2000);

                Console.WriteLine("");
                Console.WriteLine("CURRENT VERSION: [{0}]", VERSION);
                Console.WriteLine("SERVER VERSION [{0}]", SERV_VERSION);
                Console.WriteLine("");

                if (!executeInstantly)
                    Thread.Sleep(1000);

                if (VERSION < SERV_VERSION)
                {
                    Console.WriteLine("PROCEEDING TO UPDATE..");
                }
                else
                {
                    Console.WriteLine("LATEST UPDATE IS PRESENT");
                    processable = false;
                    return;
                }

                
            }
            catch (Exception e)
            {
                Console.WriteLine("CONNECTION: [FAILED]");
                filemanager.errLog(e.ToString());
                processable = false; return;
            }

            Console.WriteLine(""); // just to add a space

            processable = true; // must be true at end

        }
        // ------------- End of Initalization Method -------------

        // ------------- Begin of Process Method -------------
        public void process()
        {
            if (!processable) { finalizable = false; return; }

            Console.WriteLine("DOWNLOAD [START]");

            /*
             * Important note: Do not get latest.zip and latest#.zip mixed up
             *                 latest.zip (on the server) is always downloaded 
             *                 and then it is renamed to the version number of 
             *                 the server.
             */
            for (int i = VERSION; i < SERV_VERSION; i++)
            {
                int z = i + 1;
                string changeToFileName = "update" + z + ".zip";

                Console.WriteLine("DOWNLOADING: " + changeToFileName);

                downloader.InitiateDownload(URL + "version"+ z +".zip", changeToFileName);
            
            }

            // loop above this if statement so else is acceptable

            if (!downloader.err) { finalizable = true; }

            if (finalizable)
            {
                Console.WriteLine("DOWNLOAD [SUCCESS]");
                if (!executeInstantly)
                    Thread.Sleep(2000);
            }
            else
            {
                Console.WriteLine("DOWNLOAD [FAIL]");
            }
        }
        
        // ------------- End of Process Method -------------

        // ------------- Start of Finalization Method -------------
        public void finalization()
        {
            if (!finalizable)
                return;

            Console.WriteLine("");

            Console.WriteLine("\nDECOMPRESS [START]\n\n");

            if (!executeInstantly)
                Thread.Sleep(1000);

            bool extractedWithNoErrors = true;

            for (int i = VERSION; i < SERV_VERSION; i++)
            {
                int z = i + 1; 
                string fileName = "update" + z + ".zip";

                Console.WriteLine("DECOMPRESSING: " + fileName);

                if (archive.decompress(fileName))
                {
                    Console.WriteLine("CLEAN UP: " + fileName + " [DELETED]");
                    File.Delete(fileName);

                    VERSION = z;

                    Console.WriteLine("\n");

                }
                else
                {
                    extractedWithNoErrors = false;
                }
            }

            Console.WriteLine("DECOMPRESS: [END]");

            if (extractedWithNoErrors)
            {

                filemanager.UpdateConfig(json.Encode(URL, EXE, VERSION));
                Console.WriteLine("\nUPDATE [SUCCESS]");

            } else {
                Console.WriteLine("\nUPDATE [FAILED]");
                Console.WriteLine("RE INSTALL REQUIRED [END]");
                Thread.Sleep(5000);
            }
        }
        // ------------- End of Finalization Method
    }
}
