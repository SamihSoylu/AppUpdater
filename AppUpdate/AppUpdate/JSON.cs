using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization; // reference system.web.extension

namespace AppUpdate
{
    internal class JSON
    {
        public string updateUrl;
        public int updateVersion;
        public string whenFinishedLaunch;

        public string Encode(string url, string file, int vers = 0)
        {
            var obj = new JSON
            {
                updateUrl = url,
                updateVersion = vers,
                whenFinishedLaunch = file
            };
            var json = new JavaScriptSerializer().Serialize(obj);
            return json;
        }

        public bool Decode(string data)
        {
            try
            {
                var d = new JsonDeserializer(data);

                // Note to self JsonDeserializer is a class at the bottom of this file

                this.updateUrl = d.GetString("updateUrl");
                this.whenFinishedLaunch = d.GetString("whenFinishedLaunch");
                this.updateVersion = Convert.ToInt32(d.GetInt("updateVersion"));

                d = null;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class JsonDeserializer
    {
        private IDictionary<string, object> jsonData { get; set; }

        public JsonDeserializer(string json)
        {
            var json_serializer = new JavaScriptSerializer();

            jsonData = (IDictionary<string, object>)json_serializer.DeserializeObject(json);
        }

        public string GetString(string path)
        {
            return (string)GetObject(path);
        }

        public int? GetInt(string path)
        {
            int? result = null;

            object o = GetObject(path);
            if (o == null)
            {
                return result;
            }

            if (o is string)
            {
                result = Int32.Parse((string)o);
            }
            else
            {
                result = (Int32)o;
            }

            return result;
        }

        public object GetObject(string path)
        {
            object result = null;

            var curr = jsonData;
            var paths = path.Split('.');
            var pathCount = paths.Count();

            try
            {
                for (int i = 0; i < pathCount; i++)
                {
                    var key = paths[i];
                    if (i == (pathCount - 1))
                    {
                        result = curr[key];
                    }
                    else
                    {
                        curr = (IDictionary<string, object>)curr[key];
                    }
                }
            }
            catch
            {
                // Probably means an invalid path (ie object doesn't exist)
            }

            return result;
        }
    }
}

/**
    EXAMPLES:

     TO JSON:
         var obj = new Lad
            {
                firstName = "Markoff",
                lastName = "Chaney",
                dateOfBirth = new MyDate
                {
                    year = 1901,
                    month = 4,
                    day = 30
                }
            };

**/