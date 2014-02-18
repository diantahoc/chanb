using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.Board
{
    public static class CaptchaProvider
    {
        public static bool Verifiy(HttpContext context)
        {
            return true;
        }

        public static string GetCaptchaBody()
        {
            return "";
        }

    }
}