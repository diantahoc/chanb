﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

using chanb.Settings;

namespace chanb.Database
{
    public static class DatabaseEngine
    {
        //public static int ExecuteNonQuery(string queryText)
        //{
        //    switch (DatabaseSettings.DbType)
        //    {
        //        case Enums.DatabaseType.MsSQL:
        //            using (SqlConnection sq = new SqlConnection(DatabaseSettings.ConnectionString))
        //            {
        //                sq.Open();
        //                using (SqlCommand sc = new SqlCommand(queryText, sq))
        //                {
        //                    return sc.ExecuteNonQuery();
        //                }
        //            }
        //        case Enums.DatabaseType.MySQL:
        //            using (MySqlConnection sq = new MySqlConnection(DatabaseSettings.ConnectionString))
        //            {
        //                sq.Open();
        //                using (MySqlCommand sc = new MySqlCommand(queryText, sq))
        //                {
        //                    return sc.ExecuteNonQuery();
        //                }
        //            }
        //        default:
        //            return 0;
        //    }
        //}

        public static int ExecuteNonQuery(DbCommand query, DbConnection dc)
        {
            switch (DatabaseSettings.DbType)
            {
                case Enums.DatabaseType.MsSQL:
                case Enums.DatabaseType.MySQL:
                    using (query)
                    {
                        query.Connection = dc;
                        return query.ExecuteNonQuery();
                    }
                default:
                    return -1;
            }
        }
   
        public static DbCommand GenerateDbCommand()
        {
            switch (DatabaseSettings.DbType)
            {
                case Enums.DatabaseType.MsSQL:
                    SqlConnection sc = new SqlConnection(DatabaseSettings.ConnectionString);
                    sc.Open();
                    return new SqlCommand() { Connection = sc };
                case Enums.DatabaseType.MySQL:
                    MySqlConnection my_sc = new MySqlConnection(DatabaseSettings.ConnectionString);
                    my_sc.Open();
                    return new MySqlCommand() { Connection = my_sc };

                default:
                    return null;
            }
        }

        public static DbCommand GenerateDbCommand(string commandText)
        {
            DbCommand d = GenerateDbCommand();
            d.CommandText = commandText;
            return d;
        }

        public static DbCommand GenerateDbCommand(DbConnection conx)
        {
            DbCommand d = GenerateDbCommand();
            d.Connection = conx;
            return d;
        }

        public static DbCommand GenerateDbCommand(string commandText, DbConnection conx) 
        {
            DbCommand dc = GenerateDbCommand(commandText);
            dc.Connection = conx;
            return dc;
        }

        public static DbParameter MakeParameter(string Name, object value, System.Data.DbType Type)
        {
            switch (DatabaseSettings.DbType)
            {
                case Enums.DatabaseType.MsSQL:
                    return new SqlParameter(Name, Type) { Value = value };
                case Enums.DatabaseType.MySQL:
                    MySqlParameter p = new MySqlParameter(Name, value);
                    switch (Type) 
                    {
                        case System.Data.DbType.String:
                            p.MySqlDbType = MySqlDbType.Text;
                            break;
                        case System.Data.DbType.DateTime:
                            p.MySqlDbType = MySqlDbType.DateTime;
                            break;
                        case System.Data.DbType.Int32:
                            p.MySqlDbType = MySqlDbType.Int32;
                            break;
                        case System.Data.DbType.AnsiString:
                            p.MySqlDbType = MySqlDbType.Text;
                            break;
                        case System.Data.DbType.Boolean:
                            p.MySqlDbType = MySqlDbType.Bit;
                            break;
                        case System.Data.DbType.Int64:
                            p.MySqlDbType = MySqlDbType.Int64;
                            break;
                        default:
                            break;
                    }
                    return p;
                default:
                    return null;
            }
        }

        public static DbConnection GetDBConnection() 
        {
            switch (DatabaseSettings.DbType) 
            {
                case Enums.DatabaseType.MsSQL:
                    return new SqlConnection(DatabaseSettings.ConnectionString);
                case Enums.DatabaseType.MySQL:
                    return new MySqlConnection(DatabaseSettings.ConnectionString);
                default:
                    return null;
            }
        }

    }
}