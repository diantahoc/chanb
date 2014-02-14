using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.Settings
{
    public static class DatabaseSettings
    {
        static DatabaseSettings()
        {
            SettingsReader sr = new SettingsReader(Paths.DatabaseSettingsFilePath);

            if (sr.GetValue("type") != null) 
            {
                switch (Convert.ToInt32(sr.GetValue("type"))) 
                {
                    case 0:
                        DbType = Enums.DatabaseType.MsSQL;
                        break;
                    case 1:
                        DbType = Enums.DatabaseType.MySQL;
                        break;
                    default:
                        throw new Exception("Invalid database type. Must be 0 for MS SQL and 1 for My SQL");
                }
            }

            if (sr.GetValue("con") != null) 
            {
                ConnectionString = sr.GetValue("con").ToString();

                if (string.IsNullOrEmpty(ConnectionString) | string.IsNullOrWhiteSpace(ConnectionString)) 
                {
                    throw new ArgumentNullException("Invalid connection string");
                }

            }
        }

        public static Enums.DatabaseType DbType { get; private set; }
        public static string ConnectionString { get; private set; }
    }
}