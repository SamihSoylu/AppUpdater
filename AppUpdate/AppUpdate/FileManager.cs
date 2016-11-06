using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AppUpdate
{
    internal class FileManager
    {
        // Initializes file writer / reader
        private FileStream TheFile = null;

        // Initializes serializer
        private BinaryFormatter bf = new BinaryFormatter();

        // Initializes AES encryption / decryption
        private SimpleAES AES = new SimpleAES();

        /*
         * Read File - Method reads a file that must exist
         * De serializes and decrypts the data and returns a string
         *
         * @param string PATH - path to the file to read.
         */

        public string ReadFile(string PATH = "update.bin")
        {
            string RetrievedData = "";

            try
            {
                // Read file
                TheFile = new FileStream(PATH, FileMode.Open, FileAccess.Read);

                // Deserialize file
                RetrievedData = (string)bf.Deserialize(TheFile);

                // Decrypt file
                RetrievedData = AES.Decrypt(RetrievedData);

                return RetrievedData;
            }
            catch (SerializationException e)
            {   // Corrupted file
                Console.WriteLine("ERROR: 23");
                return null;
            }
            catch (IOException e)
            {   // File does not exist
                Console.WriteLine("ERROR: 24");
                return null;
            }
            finally
            {
                if (TheFile != null)
                {
                    TheFile.Dispose();
                    TheFile.Close();
                }
            }
        }

        /*
         * CreateFile - Method creates a file that allows the user to write a
         * string in to a serialized formatted file.
         *
         * @param string JSON - String in json format that will be written in to the fiel
         * @param string PATH - Path to the file
         */

        public bool CreateFile(string JSON, string PATH = "update.bin")
        {
            try
            {
                // FileCreation
                TheFile = new FileStream(PATH, FileMode.CreateNew, FileAccess.Write);

                // Encrypt File
                JSON = AES.Encrypt(JSON);

                // Serialize File and Write
                bf.Serialize(TheFile, JSON);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("FileManager :: Serialization Exception: {0}", e.Message); // THIS SHOULD NEVER BE TRUE
                return false;
            }
            catch (IOException e)
            {
                Console.WriteLine("ERROR: 25");
                return false;
            }
            finally
            {
                if (TheFile != null)
                {
                    TheFile.Dispose();
                    TheFile.Close();
                }
            }
            return true;
        }

        /*
         * UpdateFile - Writes to a file
         *
         * @param string JSON - String in json format that will be written in to the fiel
         * @param string PATH - Path to the file
         */

        public bool UpdateFile(string JSON, string PATH = "update.bin")
        {
            try
            {
                TheFile = new FileStream(PATH, FileMode.Create, FileAccess.Write);

                JSON = AES.Encrypt(JSON);

                bf.Serialize(TheFile, JSON);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("FileManager :: Serialization Exception: {0}", e.Message); // SHOULD NEVER BE TRUE
                return false;
            }
            catch (IOException e)
            {
                Console.WriteLine("ERROR: 26");
                return false;
            }
            finally
            {
                if (TheFile != null)
                {
                    TheFile.Dispose();
                    TheFile.Close();
                }
            }
            return true;
        }

        public void LogError(string ErrorMessage)
        {
            File.AppendAllText("e.log", ErrorMessage);
        }
    }
}