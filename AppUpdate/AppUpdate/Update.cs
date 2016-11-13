/*
 * Author: Samih Soylu.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppUpdate
{
    [Serializable]
    internal class Update
    {
        // Initializes all classes
        private JSON JSON = new JSON();

        private FileManager FileManager = new FileManager();
        private FileDownloader FileDownloader = new FileDownloader();
        private ArchiveManager ArchiveManager = new ArchiveManager();

        // Initalized variables
        private string UpdateURL;

        public string PathToFileWhichLoadsAfterUpdate;
        private int LOCAL_VERSION; // max: 2,147,483,647
        private int SERVER_VERSION;
        private bool downloadable, extractable;

        // Sets variable values from JSON Class
        public void reDefineVariables()
        {
            this.LOCAL_VERSION = JSON.updateVersion;
            this.UpdateURL = JSON.updateUrl;
            this.PathToFileWhichLoadsAfterUpdate = JSON.whenFinishedLaunch;
        }

        /*
         *  ReadBinaryFile() - reads a binary file, based on information collected
         *                     the method starts downloading the latest update.
         */

        public void ReadBinaryFile()
        {
            string TheFileThatHasBeenRead;
            bool BinaryFileWasNotRead = true;

            Console.WriteLine("INITIALIZING");

            /*
             * The file to read has been hard coded in to the file manager
             * and also JSON.cs as well.
             */

            TheFileThatHasBeenRead = FileManager.ReadFile();

            if (TheFileThatHasBeenRead != null)
            {
                if (JSON.Decode(TheFileThatHasBeenRead))
                {
                    reDefineVariables();
                    BinaryFileWasNotRead = false;
                }
            }

            if (BinaryFileWasNotRead) { downloadable = false; return; } // errors are thrown by the methods

            Console.WriteLine("CONNECTING TO SERVER");

            try
            {
                HttpWebRequest conn = (HttpWebRequest)WebRequest.Create(UpdateURL + "version.txt");
                HttpWebResponse response = (HttpWebResponse)conn.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());
                this.SERVER_VERSION = Convert.ToInt32(reader.ReadToEnd());

                reader.Close();
                response.Close();

                Console.WriteLine("CONNECTED TO SERVER");

                Console.WriteLine("");
                Console.WriteLine("LOCAL VERSION:  [{0}]", LOCAL_VERSION);
                Console.WriteLine("SERVER VERSION: [{0}]", SERVER_VERSION);
                Console.WriteLine("");

                if (LOCAL_VERSION < SERVER_VERSION)
                {
                    Console.WriteLine("PROCEEDING TO UPDATE..");
                }
                else
                {
                    Console.WriteLine("LATEST UPDATE IS PRESENT");
                    this.downloadable = false;
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("CONNECTION TO SERVER FAILED");
                FileManager.LogError(e.ToString());
                this.downloadable = false; return;
            }

            Console.WriteLine(""); // just to add a space

            this.downloadable = true; // must be true at end
        }

        /*
         * StartDownloadingLatestUpdate() - as soon as ReadBinaryFile() approves that
         *    there is a newer update, this method starts downloading the new update.
         */

        public void StartDownloadingLatestUpdate()
        {
            if (!downloadable) { extractable = false; return; }

            Console.WriteLine("STARTING DOWNLOAD");

            /*
             * Important note: Do not get latest.zip and latest#.zip mixed up
             *                 latest.zip (on the server) is always downloaded
             *                 and then it is renamed to the version number of
             *                 the server.
             */
            for (int i = LOCAL_VERSION; i < SERVER_VERSION; i++)
            {
                int z = i + 1;
                string changeToFileName = "update" + z + ".zip";

                Console.WriteLine("DOWNLOADING: " + changeToFileName);

                FileDownloader.Download(UpdateURL + "version" + z + ".zip", changeToFileName);
            }

            // loop above this if statement so else is acceptable

            if (!FileDownloader.HasError) { this.extractable = true; }

            if (extractable)
            {
                Console.WriteLine("SUCCESSFULLY DOWNLOADED");
            }
            else
            {
                Console.WriteLine("FAILED DOWNLOAD");
            }
        }

        /*
         * ExtractLatestUpdate() - Starts extracting the zip file downloaded but ONLY
         *          if the zip file has been downloaded successfully.
         */

        public void ExtractLatestUpdate()
        {
            if (!extractable)
                return;

            Console.WriteLine("");

            Console.WriteLine("\nSTARTING EXTRACTION\n\n");

            bool extractedWithNoErrors = true;

            for (int i = LOCAL_VERSION; i < SERVER_VERSION; i++)
            {
                int z = i + 1;
                string fileName = "update" + z + ".zip";

                Console.WriteLine("EXTRACTING: " + fileName);

                if (ArchiveManager.decompress(fileName))
                {
                    Console.WriteLine("CLEAN UP: " + fileName + " [DELETED]");
                    File.Delete(fileName);

                    this.LOCAL_VERSION = z;

                    Console.WriteLine("\n");
                }
                else
                {
                    extractedWithNoErrors = false;
                }
            }

            Console.WriteLine("FINISHED EXTRACTION");

            if (extractedWithNoErrors)
            {
                FileManager.UpdateFile(JSON.Encode(UpdateURL, this.PathToFileWhichLoadsAfterUpdate, LOCAL_VERSION));
                Console.WriteLine("\nSUCCESSFULLY UPDATED");
            }
            else
            {
                Console.WriteLine("\nUPDATE FAILED");
                Console.WriteLine("PLEASE RE-INSTALL APP");
                Thread.Sleep(5000);
            }
        }
    }
}