using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using chanb.Settings;


namespace chanb.DataTypes
{
    public class WPostFile
    {
        public string ChanbName { get; set; }

        /// <summary>
        /// With Extension
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// In bytes
        /// </summary>
        public int Size { get; set; }

        ///// <summary>
        ///// [0] Height , [1] Width
        ///// </summary>
        //public int[] Dimensions { get; set; }
        public string Dimensions { get; set; }
        public string Hash { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }

        /// <summary>
        /// File owner
        /// </summary>
        public int PostID { get; set; }

        public string ImageWebPath
        {
            get
            {
                return Paths.VirtualFilesStorageFolder + this.ChanbName;
            }
        }

        public string ImageThumbnailWebPath
        {
            get
            {
                string cc = this.ChanbName.Split('.')[0];

                string png_thumb = System.IO.Path.Combine(Settings.Paths.PhysicalThumbStorageFolder, cc + ".png");
                string jpg_thumb = System.IO.Path.Combine(Settings.Paths.PhysicalThumbStorageFolder, cc + ".jpg");

                if (System.IO.File.Exists(jpg_thumb))
                {
                    return Paths.VirtualThumbStorageFolder + cc + ".jpg";
                }
                else if (System.IO.File.Exists(png_thumb)) 
                {
                    return Paths.VirtualThumbStorageFolder + cc + ".png";
                }
                {
                    return Paths.VirtualThumbStorageFolder + "n.gif"; // no preview thumbnail
                }
            }
        }

    }
}