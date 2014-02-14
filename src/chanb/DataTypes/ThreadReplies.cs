using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.DataTypes
{
    public class ThreadReplies
    {
        public int TextReplies { get; set; }
        public int ImageReplies { get; set; }

        public int TotalReplies { get { return this.TextReplies + this.ImageReplies; } }
    }
}