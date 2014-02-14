using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using chanb.DataTypes;
using chanb.Settings;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace chanb.Board
{
    public static class ReportHelper
    {
        static ReportHelper()
        {
            string content = null;

            if (File.Exists(Paths.ReportReasonsFile))
            {
                content = File.ReadAllText(Paths.ReportReasonsFile);
            }

            if (string.IsNullOrEmpty(content))
            {
                ReportReasons = GetAndWriteDefaultReports();
            }
            else
            {
                JArray reasons = (JArray)JsonConvert.DeserializeObject(content, typeof(JArray));

                List<ReportReason> r = new List<ReportReason>();

                for (int i = 0; i < reasons.Count; i++)
                {
                    JObject ob = (JObject)reasons[i];

                    r.Add(new ReportReason() 
                    {
                        Description = Convert.ToString(ob["Description"]),
                        Severity = (Enums.ReportSeverity)Convert.ToInt32(ob["Severity"])
                    });

                }

                ReportReasons = r.ToArray();
            }
        }

        public static ReportReason[] ReportReasons
        {
            get;
            private set;
        }

        private static ReportReason[] GetAndWriteDefaultReports()
        {
            List<ReportReason> r = new List<ReportReason>();

            r.Add(new ReportReason()
            {
                Severity = Enums.ReportSeverity.Normal,
                Description = "Rule Violation"
            });

            r.Add(new ReportReason()
            {
                Severity = Enums.ReportSeverity.High,
                Description = "SPAM/Abuse"
            });

            r.Add(new ReportReason()
            {
                Severity = Enums.ReportSeverity.Low,
                Description = "NSFW material on SFW board"
            });

            r.Add(new ReportReason()
            {
                Severity = Enums.ReportSeverity.High,
                Description = "Illegal Material"
            });

            ReportReason[] data = r.ToArray();

            SaveData(r.ToArray());

            return data;
        }

        private static void SaveData(ReportReason[] data)
        {
            File.WriteAllText(Paths.ReportReasonsFile, JsonConvert.SerializeObject(data, Formatting.Indented));
        }

    }
}