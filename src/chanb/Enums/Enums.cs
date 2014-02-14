using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.Enums
{
    public enum PostType
    {
        Unknown = -1,
        Thread = 0,
        Reply = 1
    }

    public enum DatabaseType 
    {
        MsSQL, MySQL
    }

    public enum ChanbAccountType 
    {
        Administrator,
        Moderator,
        Janitor
    }

    public enum TextOrientation { LeftToRight, RightToLeft }

    public enum ReportSeverity 
    {
        Low,
        Normal,
        High
    }
}