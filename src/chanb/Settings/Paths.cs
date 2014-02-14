using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace chanb.Settings
{
    public static class Paths
    {

        //NOTE: ALL WEB PATHS END WITH A TRAILING SLASH '/'

        public static string WebRoot
        {
            get
            {
                string prefix = "http://";

                HttpRequest request = HttpContext.Current.Request;

                if (request.ServerVariables["HTTPS"] == "ON")
                {
                    prefix = "https://";
                }

                if (request.ApplicationPath == "/")
                {
                    return prefix + request.ServerVariables["HTTP_HOST"] + request.ApplicationPath;
                }
                else
                {
                    return prefix + request.ServerVariables["HTTP_HOST"] + request.ApplicationPath + "/";
                }

                //return System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

                //return HttpRuntime.AppDomainAppVirtualPath;
            }
        }

        #region File storage paths

        private static string files_folder_name = "files";

        public static string PhysicalFilesStorageFolder
        {
            get
            {
                HttpRequest request = HttpContext.Current.Request;
                return Path.Combine(request.PhysicalApplicationPath, files_folder_name);
            }
        }

        public static string VirtualFilesStorageFolder
        {
            get
            {
                return WebRoot + files_folder_name + "/";
            }
        }

        #endregion

        #region Thumbnail folder paths

        private static string thumb_folder_name = "thumbs";

        public static string PhysicalThumbStorageFolder
        {
            get
            {
                HttpRequest request = HttpContext.Current.Request;
                return Path.Combine(request.PhysicalApplicationPath, thumb_folder_name);
            }
        }

        public static string VirtualThumbStorageFolder
        {
            get { return WebRoot + thumb_folder_name + "/"; }
        }

        #endregion

        public static string DllFolder
        {
            get { return HttpRuntime.BinDirectory; }
        }

        //public static string TempFolder
        //{
        //    get { return Path.Combine(HttpRuntime.BinDirectory, "temp"); ; }
        //}

        public static string SettingsFolder
        {
            get { return Path.Combine(DllFolder, "settings"); }
        }

        public static string LanguagesFolder
        {
            get { return Path.Combine(DllFolder, "langs"); }
        }


        public static string TemplatesFolder
        {
            get { return Path.Combine(DllFolder, "templates"); }
        }

        public static string ThreadBodiesFolder
        {
            get { return Path.Combine(DllFolder, "threadb"); }
        }

        public static string IndexThreadBodiesFolder
        {
            get { return Path.Combine(DllFolder, "threadb-index"); }
        }

        public static string WordFilterFilePath
        {
            get { return Path.Combine(SettingsFolder, "wordfilter.json"); }
        }

        public static string SmiliesFilePath
        {
            get { return Path.Combine(SettingsFolder, "smilies.json"); }
        }

        public static string SettingsFilePath
        {
            get { return Path.Combine(SettingsFolder, "settings.json"); }
        }

        public static string ReportReasonsFile
        {
            get { return Path.Combine(SettingsFolder, "report-reasons.json"); }
        }

        public static string DatabaseSettingsFilePath
        {
            get { return Path.Combine(SettingsFolder, "database.json"); }
        }

        static Paths()
        {
            //Check folders and make them when necessary
            CheckDirectory(SettingsFolder);
            CheckDirectory(TemplatesFolder);
            CheckDirectory(ThreadBodiesFolder);
            CheckDirectory(IndexThreadBodiesFolder);
            //Common.FileSystemHelper.CheckDirectory(TempFolder);

            //Common.FileSystemHelper.CheckDirectory(LanguagesFolder);

            CheckDirectory(PhysicalFilesStorageFolder);
            CheckDirectory(PhysicalThumbStorageFolder);
        }

        public static void CheckDirectory(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
    }
}