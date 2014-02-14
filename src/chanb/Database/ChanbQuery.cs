using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;

namespace chanb.Database
{
    public class ChanbQuery
    {
        public DbDataReader Reader { get; set; }
        public DbConnection Connection { get; set; }
    }
}