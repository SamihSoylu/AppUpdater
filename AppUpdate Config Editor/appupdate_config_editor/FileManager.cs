using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Versioner
{
    class FileManager
    {
        // Initalizing file writer and To binary converter
        private FileStream file = null;
        private BinaryFormatter bf = new BinaryFormatter();
        private SimpleAES crypt = new SimpleAES();

        public string ReadConfig(string address)
        {
            string data = "";

            try
            {
                // File read
                file = new FileStream(address, FileMode.Open, FileAccess.Read);

                // Deserializing file text
                data = (string)bf.Deserialize(file);

                // Decryption
                data = crypt.Decrypt(data);

                // Returns raw data
                return data;
            }
            catch (Exception e)
            {
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

        public bool CreateConfig(string JSON, string address)
        {
            bool result;

            try
            {
                // FileCreation
                file = new FileStream(address, FileMode.Create, FileAccess.Write);

                // Encrypts settings
                JSON = crypt.Encrypt(JSON);

                // Writes to file
                bf.Serialize(file, JSON);

                result = true;
            }
            catch (Exception e)
            {
                errLog(e.ToString());
                result = false;
            }
            finally
            {
                if (file != null)
                {
                    file.Dispose();
                    file.Close();
                }
            }
            return result;
        }

        public void errLog(string e)
        {
            File.AppendAllText("bin-editor-error.log", "\n\nError :: " + e + "\n\n");
        }
    }
}
