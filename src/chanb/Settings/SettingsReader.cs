using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace chanb.Settings
{
    public class SettingsReader
    {
        private string settings_file_path;

        private Dictionary<string, object> data;

        public SettingsReader(string settings_file)
        {
            settings_file_path = settings_file;
            if (File.Exists(settings_file_path))
            {
                //parse and load
                load(settings_file_path);
            }
            else
            {
                //blank mode
                data = new Dictionary<string, object>();
            }
        }


        private void load(string path)
        {
            string text = File.ReadAllText(path);

            if (string.IsNullOrEmpty(text) | string.IsNullOrWhiteSpace(text))
            {
                data = new Dictionary<string, object>();
            }
            else
            {
                data = (Dictionary<string, object>)JsonConvert.DeserializeObject(text, typeof(Dictionary<string, object>));
            }
        }

        private void save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.None));
        }

        public object GetValue(string key)
        {
            if (this.data != null)
            {
                if (this.data.ContainsKey(key))
                {
                    return this.data[key];
                }
                else { return null; }
            }
            else { return null; }
        }

        public void SetValue(string key, object value)
        {
            if (this.data != null)
            {
                if (this.data.ContainsKey(key))
                {
                    this.data[key] = value;
                }
                else
                {
                    this.data.Add(key, value);
                }
                save(settings_file_path);
            }
            else { throw new Exception("Cannot set values, memory store is null"); }
        }

        public bool ContainKey(string key) { if (this.data != null) { return this.data.ContainsKey(key); } else { return false; } }

    }
}