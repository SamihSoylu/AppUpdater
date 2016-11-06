using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppUpdate
{
    internal class FileDownloader
    {
        // Creates web client instance
        private WebClient Client = new WebClient();

        // Instantiates file manager
        private FileManager FileManager = new FileManager();

        // For later to check if download it complete
        private volatile bool _completed;

        // Volatile bool is returned
        private bool DownloadCompleted { get { return _completed; } }

        // Counter used in download progress method
        private int counter;

        // Error boolean used to check if the download has failed.
        public bool HasError = false;

        /*
         * Download() - Downloads file from web address and saves
         *              downloaded file as NewUpdate.zip
         *
         * @param string address - web address of file
         * @param string saveAs  - file name to save as
         */

        public void Download(string address, string saveAs = "NewUpdate.zip")
        {
            this._completed = false;

            Uri Uri = new Uri(address);

            try
            {
                Client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
                Client.DownloadFileAsync(Uri, saveAs);

                while (!_completed)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (WebException e)
            {
                FileManager.LogError("FileDownloader :: " + e.ToString());
                this._completed = true;
                this.HasError = true;

                Console.WriteLine("DOWNLOAD WAS NOT FOUND");
            }
        }

        private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            counter++;

            if (counter % 500 == 0)
            {
                // Displays the operation identifier, and the transfer progress.
                Console.WriteLine(" DOWNLOAD:  "
                              + ((e.BytesReceived / 1024f) / 1024f).ToString("#0.##") + "mb"
                              + " of "
                              + ((e.TotalBytesToReceive / 1024f) / 1024f).ToString("#0.##") + "mb"
                              + " \t(" + e.ProgressPercentage + "%)"
                );
            }
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                Console.WriteLine("DOWNLOAD [CANCELLED]");
                this._completed = false;
            }
            else if (this.HasError == false)
            {
                // Console.WriteLine("ARCHIVE DOWNLOAD: [SUCCESS]");
                this._completed = true;
            }
        }
    }
}