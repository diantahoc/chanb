using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace chanb.Common
{
    public static class Misc
    {
        private static System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

        public static string MD5(string data)
        {
            byte[] hash_data = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
            return byte_to_string(hash_data);
        }

        public static string MD5(System.IO.Stream data)
        {
            data.Seek(0, System.IO.SeekOrigin.Begin);
            byte[] hash_data = md5.ComputeHash(data);
            return byte_to_string(hash_data);
        }

        private static string byte_to_string(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in data)
            {
                sb.Append(b.ToString("X2"));
            }
            return sb.ToString().ToLower();
        }

        private static readonly DateTime unix_epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static int GetUnixTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - unix_epoch;
            return Convert.ToInt32(ts.TotalSeconds);
        }

    }
}