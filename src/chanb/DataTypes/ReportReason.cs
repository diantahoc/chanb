using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.DataTypes
{
    public class ReportReason
    {
        public string Description { get; set; }

        public Enums.ReportSeverity Severity { get; set; }
    }
}