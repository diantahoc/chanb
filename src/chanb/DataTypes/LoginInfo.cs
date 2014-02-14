using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.DataTypes
{
    public class LoginInfo
    {
        public bool IsValid { get; set; }
        public Enums.ChanbAccountType AccountType { get; set; }
        public ModPowers Powers { get; set; }
    }
}