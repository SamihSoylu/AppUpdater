/*
 * Author: Samih Soylu.
 */

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

        // Used in downloadprogress to determine where to reset the cursor
        private int left, top;

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
                /*
                 * Saves current cursor position
                 *
                 * Added it here otherwise pointer would move to start of console.
                 * This only happenend when there was a small zip file to download
                 * and the Draw Progress Bar method was never accessed.
                 */
                this.left = Console.CursorLeft;
                this.top = Console.CursorTop;

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

                DrawProgressBar(e.ProgressPercentage, 100, 25, '=', e);
            }
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                Console.WriteLine("DOWNLOAD CANCELLED");
                this._completed = false;
            }
            else if (this.HasError == false)
            {
                Console.SetCursorPosition(this.left, this.top);
                Console.WriteLine("DOWNLOAD COMPLETE\t\t\t\t\t");
                this._completed = true;
            }
        }

        private void DrawProgressBar(int complete, int maxVal, int barSize, char progressCharacter, DownloadProgressChangedEventArgs e)
        {
            Console.CursorVisible = false;

            // Saves current cursor position
            this.left = Console.CursorLeft;
            this.top = Console.CursorTop;

            decimal perc = (decimal)complete / (decimal)maxVal;
            int chars = (int)Math.Floor(perc / ((decimal)1 / (decimal)barSize));
            string p1 = String.Empty, p2 = String.Empty;

            for (int i = 0; i < chars; i++)
            {
                if (i == (chars - 1))
                {
                    p1 += '>';
                }
                else
                {
                    p1 += progressCharacter;
                }
            }
            for (int i = 0; i < barSize - chars; i++) p2 += progressCharacter;

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(p1);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(p2);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("]");

            Console.ResetColor();
            Console.Write(" ({0}%)", e.ProgressPercentage);

            Console.Write(" "
                              + ((e.BytesReceived / 1024f) / 1024f).ToString("#0.##") + "MB"
                              + " of "
                              + ((e.TotalBytesToReceive / 1024f) / 1024f).ToString("#0.##") + "MB \t\t"
                );

            // Resets cursor position
            Console.SetCursorPosition(this.left, this.top);
        }
    }
}