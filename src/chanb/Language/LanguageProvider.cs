using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using chanb.Settings;

namespace chanb.Language
{
    public static class LanguageProvider
    {

        private static Dictionary<string, object> lang_data;

        public static Enums.TextOrientation TextOrientation { get; private set; }

        static LanguageProvider()
        {
            SettingsReader sr = new SettingsReader(Paths.LanguageSettingFilePath);

            if (sr.ContainKey("lang"))
            {
                string file_path = Path.Combine(Paths.LanguagesFolder, sr.GetValue("lang") + ".json");
                if (File.Exists(file_path))
                {
                    lang_data = (Dictionary<string, object>)Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(file_path), typeof(Dictionary<string, object>));
                }
                else
                {
                    throw new ArgumentException("The language file specified does not exist");
                }
            }
            else
            {
                //Fallback to english
                string file_path = Path.Combine(Paths.LanguagesFolder, "en-US.json");
                if (File.Exists(file_path))
                {
                    lang_data = (Dictionary<string, object>)Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(file_path), typeof(Dictionary<string, object>));
                }
                else
                {
                    throw new ArgumentException("The language file specified does not exist");
                }
            }

            if (sr.ContainKey("tor"))
            {

            }
            else { TextOrientation = Enums.TextOrientation.LeftToRight; }
        }

        public static KeyValuePair<string, string>[] GetAllTokens() 
        {
            if (lang_data != null)
            {
                List<KeyValuePair<string, string>> l = new List<KeyValuePair<string, string>>();

                foreach (KeyValuePair<string, object> data in lang_data)
                {
                    l.Add(new KeyValuePair<string, string>(data.Key, data.Value.ToString()));
                }

                return l.ToArray();
            }
            else { return new KeyValuePair<string, string>[]{}; }
        }
    }
}