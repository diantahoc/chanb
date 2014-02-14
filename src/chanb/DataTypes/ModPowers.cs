using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.DataTypes
{
    public class ModPowers
    {
        //Ban, Delete, Toggle sticky, Toggle Locked, Edit post.

        public bool CanBan { get; set; }

        public bool CanDeletePosts { get; set; }

        public bool CanStickyThreads { get; set; }

        public bool CanLockThreads { get; set; }

        public bool CanEditPost { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4}", this.CanBan, this.CanDeletePosts, this.CanStickyThreads, this.CanLockThreads, this.CanEditPost);
        }

        public static ModPowers FromString(string data) 
        {
            string[] a = data.Split(',');
            return new ModPowers()
            {
                CanBan = Convert.ToBoolean(a[0]),
                CanDeletePosts = Convert.ToBoolean(a[1]),
                CanStickyThreads = Convert.ToBoolean(a[2]),
                CanLockThreads = Convert.ToBoolean(a[3]),
                CanEditPost = Convert.ToBoolean(a[4])
            };
        }

    }
}