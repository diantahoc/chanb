using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.DataTypes
{
    public class BanData
    {
        public int ID { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool Permanant { get; set; }
        public int PostID { get; set; }
        
        /// <summary>
        /// Ban reason
        /// </summary>
        public string Comment { get; set; }

        public string IP { get; set; }

        public bool CanBrowse { get; set; }

        public string ModeratorName { get; set; }

        public DateTime BannedOn { get; set; }

        public bool IsBanEffective { get; set; }

    }
}