using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.Common
{
    public static class Limits
    {
        /// <summary>
        /// Maximum length for the email, name, subject and password fields
        /// </summary>
        public static int MaximumFieldLength { get { return 1000; } }

        public static int MaximumCommentLength { get { return 2000; } }
    }
}