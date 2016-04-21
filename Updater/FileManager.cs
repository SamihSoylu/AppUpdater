using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Updater
{
    class FileManager
    {
        // Initalizing file writer and To binary converter
        private FileStream file = null;
        private BinaryFormatter bf = new BinaryFormatter();
        private SimpleAES crypt = new SimpleAES();

        public string ReadConfig()
        {
            string data = "";

            try
            {
                // File read
                file = new FileStream("updater.bin", FileMode.Open, FileAccess.Read);

                // Deserializing file text
                data = (string)bf.Deserialize(file);

                // Decryption
                data = crypt.Decrypt(data);

                // Returns raw data
                return data;
            }
            catch (SerializationException e)
            {
                Console.WriteLine("ERR: UPDATER.BIN IS NOT READABLE");
                return null;
            }
            catch (IOException e)
            {
                Console.WriteLine("ERR: COULD NOT READ FILE, FILYOE DOES NOT EXIST");
                return null;
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                    file.Close();
                }
            }
        }

        public bool CreateConfig(string JSON)
        {
            try
            {
                // FileCreation
                file = new FileStream("updater.bin", FileMode.CreateNew, FileAccess.Write);

                // Encrypts settings
                JSON = crypt.Encrypt(JSON);

                // Writes to file
                bf.Serialize(file, JSON);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("---Serialization Exception: {0}", e); // THIS SHOULD NEVER BE TRUE
                return false;
            }
            catch (IOException e)
            {
                Console.WriteLine("ERR: FILE ALREADY EXISTS");
                return false;
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                    file.Close();
                }
            }
            return true;
        }

        public bool UpdateConfig(string JSON)
        {
            try
            {
                file = new FileStream("updater.bin", FileMode.Create, FileAccess.Write);

                JSON = crypt.Encrypt(JSON);

                bf.Serialize(file, JSON);
            }
            catch (SerializationException e)
            {
                Console.WriteLine("---Serialization Exception: {0}", e); // SHOULD NEVER BE TRUE
                return false;
            }
            catch (IOException e)
            {
                Console.WriteLine("ERR: UPDATER.BIN IS CURRENTLY IN USE, WRITING TO FILE FAILED");
                return false;
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                    file.Close();
                }
            }
            return true;
        }

        public void errLog(string e)
        {
            File.AppendAllText("update-error.log", "\n\nError :: " + e + "\n\n");
        }
    }
}
