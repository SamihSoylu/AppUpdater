using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Net;
using System.ComponentModel;


namespace Updater
{
    class FileDownloader
    {
        // Creates web client instance
        WebClient client = new WebClient();
        FileManager filemanager = new FileManager();

        // Variables
        private volatile bool _completed;
        private bool DownloadCompleted { get { return _completed; } }
        private int counter;
        public bool err = false;

        // Begind sdownload process
        public void InitiateDownload(string address, string saveAs="update.zip")
        {
            _completed = false;

            Uri Uri = new Uri(address);

            try
            {
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgress);
                client.DownloadFileAsync(Uri, saveAs);

                while (!_completed)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (WebException e)
            {
                filemanager.errLog(e.ToString());
                _completed = true;
                err = true;

                Console.WriteLine("ARCHIVE DOWNLOAD: [NOT FOUND]");
                
            }

            //filemanager.errLog(e.ToString()
        }

        // Shows download process
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
                Console.WriteLine("ARCHIVE DOWNLOAD: [CANCELLED]");
                _completed = false;
            }
            else if (this.err == false)
            {
                // Console.WriteLine("ARCHIVE DOWNLOAD: [SUCCESS]");
                _completed = true;
            }
        }
    }
}
