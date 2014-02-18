using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;

using chanb.DataTypes;

namespace chanb.Board
{
    public static class BanHandler
    {
        public static bool IsIPBanned(string ip, DbConnection connection)
        {
            BanData data = get_ban_data(ip);

            if (data != null)
            {
                if (data.Permanant)
                {
                    return true;
                }
                else
                {
                    if (data.ExpirationDate < DateTime.Now)
                    {
                        //drop ban
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else { return false; }

        }

        private static BanData get_ban_data(string ip)
        {
            return null;
        }

    }
}