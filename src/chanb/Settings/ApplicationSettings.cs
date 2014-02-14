using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace chanb.Settings
{
    public static class ApplicationSettings
    {

        private static Settings.SettingsReader sr;

        static ApplicationSettings()
        {
            sr = new SettingsReader(Paths.SettingsFilePath);
        }

        public static string BoardTitle
        {
            get
            {
                if (sr.ContainKey("BoardTitle"))
                {
                    return sr.GetValue("BoardTitle").ToString();
                }
                else
                {
                    sr.SetValue("BoardTitle", "Channel Board");
                    return BoardTitle;
                }
            }
            set
            {
                sr.SetValue("BoardTitle", value);
            }
        }

        public static string BoardDescription
        {
            get
            {
                if (sr.ContainKey("BoardDescription"))
                {
                    return sr.GetValue("BoardDescription").ToString();
                }
                else
                {
                    sr.SetValue("BoardDescription", "ASP.NET Imageboard");
                    return BoardDescription;
                }
            }
            set
            {
                sr.SetValue("BoardDescription", value);
            }
        }

        public static string FooterText
        {
            get
            {
                if (sr.ContainKey("FooterText"))
                {
                    return sr.GetValue("FooterText").ToString();
                }
                else
                {
                    sr.SetValue("FooterText", "<a href='https://github.com/diantahoc/chanb' target='_blank'>ChanB</a> ASP.NET board.");
                    return FooterText;
                }
            }
            set
            {
                sr.SetValue("FooterText", value);
            }
        }

        public static int FloodInterval
        {
            get
            {
                if (sr.ContainKey("FloodInterval"))
                {
                    return Convert.ToInt32(sr.GetValue("FloodInterval"));
                }
                else
                {
                    sr.SetValue("FloodInterval", 10);
                    return FloodInterval;
                }
            }
            set
            {
                sr.SetValue("FloodInterval", value);
            }
        }

        public static int MaximumFileSize
        {
            get
            {
                if (sr.ContainKey("MaximumFileSize"))
                {
                    return Convert.ToInt32(sr.GetValue("MaximumFileSize"));
                }
                else
                {
                    sr.SetValue("MaximumFileSize", 4194304);
                    return MaximumFileSize;
                }
            }
            set
            {
                sr.SetValue("MaximumFileSize", value);
            }
        }

        public static int ThreadPerPage
        {
            get
            {
                if (sr.ContainKey("ThreadPerPage"))
                {
                    return Convert.ToInt32(sr.GetValue("ThreadPerPage"));
                }
                else
                {
                    sr.SetValue("ThreadPerPage", 10);
                    return ThreadPerPage;
                }
            }
            set
            {
                sr.SetValue("ThreadPerPage", value);
            }
        }

        public static int MaximumPages
        {
            get
            {
                if (sr.ContainKey("MaximumPages"))
                {
                    return Convert.ToInt32(sr.GetValue("MaximumPages"));
                }
                else
                {
                    sr.SetValue("MaximumPages", 10);
                    return MaximumPages;
                }
            }
            set
            {
                sr.SetValue("MaximumPages", value);
            }
        }

        public static int TrailPostsCount
        {
            get
            {
                if (sr.ContainKey("TrailPostsCount"))
                {
                    return Convert.ToInt32(sr.GetValue("TrailPostsCount"));
                }
                else
                {
                    sr.SetValue("TrailPostsCount", 5);
                    return TrailPostsCount;
                }
            }
            set
            {
                sr.SetValue("TrailPostsCount", value);
            }
        }

        public static int BumpLimit
        {
            get
            {
                if (sr.ContainKey("BumpLimit"))
                {
                    return Convert.ToInt32(sr.GetValue("BumpLimit"));
                }
                else
                {
                    sr.SetValue("BumpLimit", 200);
                    return BumpLimit;
                }
            }
            set
            {
                sr.SetValue("BumpLimit", value);
            }
        }

        public static int ResizeMethod
        {
            get
            {
                if (sr.ContainKey("ResizeMethod"))
                {
                    return Convert.ToInt32(sr.GetValue("ResizeMethod"));
                }
                else
                {
                    sr.SetValue("ResizeMethod", 1);
                    return ResizeMethod;
                }
            }
            set
            {
                sr.SetValue("ResizeMethod", value);
            }
        }

        public static bool PostingEnabled
        {
            get
            {
                if (sr.ContainKey("PostingEnabled"))
                {
                    return Convert.ToBoolean(sr.GetValue("PostingEnabled"));
                }
                else
                {
                    sr.SetValue("PostingEnabled", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("PostingEnabled", value);
            }
        }

        public static bool CacheThreadView
        {
            get
            {
                if (sr.ContainKey("CacheThreadView"))
                {
                    return Convert.ToBoolean(sr.GetValue("CacheThreadView"));
                }
                else
                {
                    sr.SetValue("CacheThreadView", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("CacheThreadView", value);
            }
        }

        public static bool CacheIndexView
        {
            get
            {
                if (sr.ContainKey("CacheIndexView"))
                {
                    return Convert.ToBoolean(sr.GetValue("CacheIndexView"));
                }
                else
                {
                    sr.SetValue("CacheIndexView", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("CacheIndexView", value);
            }
        }

        public static bool AutoDeleteFiles
        {
            get
            {
                if (sr.ContainKey("AutoDeleteFiles"))
                {
                    return Convert.ToBoolean(sr.GetValue("AutoDeleteFiles"));
                }
                else
                {
                    sr.SetValue("AutoDeleteFiles", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("AutoDeleteFiles", value);
            }
        }

        public static bool AllowDuplicatesFiles
        {
            get
            {
                if (sr.ContainKey("AllowDuplicatesFiles"))
                {
                    return Convert.ToBoolean(sr.GetValue("AllowDuplicatesFiles"));
                }
                else
                {
                    sr.SetValue("AllowDuplicatesFiles", false);
                    return false;
                }
            }
            set
            {
                sr.SetValue("AllowDuplicatesFiles", value);
            }
        }

        public static bool SmartLinkDuplicateImages
        {
            get
            {
                if (sr.ContainKey("SmartLinkDuplicateImages"))
                {
                    return Convert.ToBoolean(sr.GetValue("SmartLinkDuplicateImages"));
                }
                else
                {
                    sr.SetValue("SmartLinkDuplicateImages", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("SmartLinkDuplicateImages", value);
            }
        }

        public static bool EnableUserID
        {
            get
            {
                if (sr.ContainKey("EnableUserID"))
                {
                    return Convert.ToBoolean(sr.GetValue("EnableUserID"));
                }
                else
                {
                    sr.SetValue("EnableUserID", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("EnableUserID", value);
            }
        }

        public static bool EnableArchive
        {
            get
            {
                if (sr.ContainKey("EnableArchive"))
                {
                    return Convert.ToBoolean(sr.GetValue("EnableArchive"));
                }
                else
                {
                    sr.SetValue("EnableArchive", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("EnableArchive", value);
            }
        }

        public static bool EnableSmilies
        {
            get
            {
                if (sr.ContainKey("EnableSmilies"))
                {
                    return Convert.ToBoolean(sr.GetValue("EnableSmilies"));
                }
                else
                {
                    sr.SetValue("EnableSmilies", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("EnableSmilies", value);
            }
        }

        public static bool EnableCaptcha
        {
            get
            {
                if (sr.ContainKey("EnableCaptcha"))
                {
                    return Convert.ToBoolean(sr.GetValue("EnableCaptcha"));
                }
                else
                {
                    sr.SetValue("EnableCaptcha", false);
                    return false;
                }
            }
            set
            {
                sr.SetValue("EnableCaptcha", value);
            }
        }

        //public static bool RemoveEXIFData
        //{
        //    get
        //    {
        //        if (sr.ContainKey("RemoveEXIFData"))
        //        {
        //            return Convert.ToBoolean(sr.GetValue("RemoveEXIFData"));
        //        }
        //        else
        //        {
        //            sr.SetValue("RemoveEXIFData", true);
        //            return true;
        //        }
        //    }
        //    set
        //    {
        //        sr.SetValue("RemoveEXIFData", value);
        //    }
        //}

        public static bool EnableImpresonationProtection
        {
            get
            {
                if (sr.ContainKey("EnableImpresonationProtection"))
                {
                    return Convert.ToBoolean(sr.GetValue("EnableImpresonationProtection"));
                }
                else
                {
                    sr.SetValue("EnableImpresonationProtection", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("EnableImpresonationProtection", value);
            }
        }

        public static bool CheckExifOrientation
        {
            get
            {
                if (sr.ContainKey("CheckExifOrientation"))
                {
                    return Convert.ToBoolean(sr.GetValue("CheckExifOrientation"));
                }
                else
                {
                    sr.SetValue("CheckExifOrientation", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("CheckExifOrientation", value);
            }
        }

        public static bool ShowThreadRepliesCount
        {
            get
            {
                if (sr.ContainKey("ShowThreadRepliesCount"))
                {
                    return Convert.ToBoolean(sr.GetValue("ShowThreadRepliesCount"));
                }
                else
                {
                    sr.SetValue("ShowThreadRepliesCount", true);
                    return true;
                }
            }
            set
            {
                sr.SetValue("ShowThreadRepliesCount", value);
            }
        }

        public static bool ShowPostSuccessfulMessage
        {
            get
            {
                if (sr.ContainKey("ShowPostSuccessfulMessage"))
                {
                    return Convert.ToBoolean(sr.GetValue("ShowPostSuccessfulMessage"));
                }
                else
                {
                    sr.SetValue("ShowPostSuccessfulMessage", false);
                    return false;
                }
            }
            set
            {
                sr.SetValue("ShowPostSuccessfulMessage", value);
            }
        }

        /// <summary>
        ///  Detect transparency in images files and issue a transparent thumbnail as well.
        /// </summary>
        public static bool EnchancedThumbGeneration
        {
            get
            {
                if (sr.ContainKey("EnchancedThumbGeneration"))
                {
                    return Convert.ToBoolean(sr.GetValue("EnchancedThumbGeneration"));
                }
                else
                {
                    sr.SetValue("EnchancedThumbGeneration", false);
                    return false;
                }
            }
            set
            {
                sr.SetValue("EnchancedThumbGeneration", value);
            }
        }
    }
}