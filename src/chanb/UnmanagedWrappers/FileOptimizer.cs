using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Diagnostics;

namespace chanb.UnmanagedWrappers
{
    /// <summary>
    /// An image optimizer for JPEG (JPEGTRAN), PNG (OPTIPNG), GIF(GIFSICLE), WEBM(MKCLEAN)
    /// </summary>
    public static class FileOptimizer
    {
        //  static string gif_prog = "";
        static string jpg_prog = "";
        static string png_prog = "";
        static string webm_prog = "";

        //static bool gif_ok = false;
        static bool jpg_ok = false;
        static bool png_ok = false;
        static bool webm_ok = false;

        static FileOptimizer()
        {
            //gif_prog = Path.Combine(Settings.Paths.DllFolder, "gifsicle.exe");
            //gif_ok = File.Exists(gif_prog);

            jpg_prog = Path.Combine(Settings.Paths.DllFolder, "jpegtran.exe");
            jpg_ok = File.Exists(jpg_prog);

            png_prog = Path.Combine(Settings.Paths.DllFolder, "optipng.exe");
            png_ok = File.Exists(png_prog);

            webm_prog = Path.Combine(Settings.Paths.DllFolder, "mkclean.exe");
            webm_ok = File.Exists(webm_prog);

        }

        public static void OptimizePNG(string path)
        {
            if (png_ok)
            {
                ProcessStartInfo psi = new ProcessStartInfo(png_prog);
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;

                psi.Arguments = string.Format("-o1 \"{0}\"", path); //-o1 for fastest optimization

                using (Process proc = Process.Start(psi))
                {
                    proc.Start();
                    proc.WaitForExit(10000);

                    if (!proc.HasExited) { proc.Kill(); }
                }


            }
        }

        //public static void OptimizeGif(string path)
        //{
        //    if (gif_ok)
        //    {
        //        ProcessStartInfo psi = new ProcessStartInfo(gif_prog);
        //        psi.CreateNoWindow = true;
        //        psi.UseShellExecute = false;

        //        psi.Arguments = string.Format("--batch --optimize=1 \"{0}\"", path); // --optimize=1 for lossless optimization

        //        using (Process proc = Process.Start(psi))
        //        {
        //            proc.Start();
        //            proc.WaitForExit(10000);

        //            if (!proc.HasExited) { proc.Kill(); }
        //        }
        //    }
        //}

        public static void OptimizeJPG(string path)
        {
            if (jpg_ok)
            {
                ProcessStartInfo psi = new ProcessStartInfo(jpg_prog);
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;

                psi.Arguments = string.Format("-optimize \"{0}\" \"{0}\"", path);

                using (Process proc = Process.Start(psi))
                {
                    proc.Start();
                    proc.WaitForExit(10000);

                    if (!proc.HasExited) { proc.Kill(); }
                }
            }
        }

        public static void OptimizeWEBM(string path) 
        {
            if (webm_ok) 
            {
                ProcessStartInfo psi = new ProcessStartInfo(webm_prog);
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;

                psi.Arguments = string.Format("--keep-cues --quiet --doctype 4 --optimize \"{0}\"", path);

                using (Process proc = Process.Start(psi))
                {
                    proc.Start();
                    proc.WaitForExit(5000);
                    if (!proc.HasExited) { proc.Kill(); }
                }
            }
        }
    }
}