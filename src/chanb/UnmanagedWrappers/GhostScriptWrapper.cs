using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace chanb.UnmanagedWrappers
{
    public static class GhostScriptWrapper
    {

        private static string gs_path;

        static bool binary_exist = false;

        static GhostScriptWrapper()
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    gs_path = "/usr/bin/gs";
                    break;
                default:
                    gs_path = Path.Combine(Settings.Paths.DllFolder, "gswin32c.exe");
                    break;
            }

            binary_exist = File.Exists(gs_path);
        }

        public static Image GeneratePDFThumb(string path)
        {
            if (binary_exist)
            {
                Image im = null;

                if (File.Exists(path))
                {
                    ProcessStartInfo psr = new ProcessStartInfo(gs_path);
                    psr.CreateNoWindow = true;
                    psr.WorkingDirectory = Settings.Paths.DllFolder;
                    psr.WindowStyle = ProcessWindowStyle.Hidden;

                    StringBuilder param = new StringBuilder();
                    param.Append(" -sDEVICE=jpeg ");
                    param.Append(" -q -sOutputFile=-  "); //output to stdout
                    param.Append(" -dFirstPage=1  "); //only the first page
                    param.Append(" -dLastPage=1  ");
                    param.Append(" -dJPEGQ=75 "); // set JPEG quality to 75%
                    param.AppendFormat(" -dNumRenderingThreads={0} ", Environment.ProcessorCount * 2); // use multiple threads 
                    param.Append(" -dBATCH -dNOPAUSE ");
                    param.Append(" -dGrayDetection=false "); //speed up
                    param.Append(" -dPDFFitPage "); //for aspect ratio
                    param.Append(" -dShowAnnots=false "); //speed up
                    param.Append(" -r18 "); //to generate small images (this factor depends on page size).

                    param.AppendFormat("\"{0}\"", path);

                    psr.Arguments = param.ToString();

                    psr.UseShellExecute = false;

                    psr.RedirectStandardOutput = true;
                    //psr.RedirectStandardError = true;

                    Process proc = Process.Start(psr);

                    proc.WaitForExit(15000); // wait 15 seconds 


                    if (!proc.HasExited) { proc.Kill(); }

                    MemoryStream memio = new MemoryStream();

                    proc.StandardOutput.BaseStream.CopyTo(memio);

                    proc.Dispose();

                    try
                    {
                        im = Image.FromStream(memio);
                    }
                    catch (Exception)
                    {
                        //im = new Bitmap(128, 128);
                        //using (Graphics g = Graphics.FromImage(im))
                        //{
                        //    g.Clear(Color.White);
                        //    g.DrawString("Unable to\ngenerate\npreview", new Font(FontFamily.GenericMonospace, 15, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Black, new PointF(0, 0));
                        //}

                        //return null
                    }
                }
                return im;
            }
            else
            {
                //Image i = new Bitmap(128, 128);
                //using (Graphics g = Graphics.FromImage(i))
                //{
                //    g.Clear(Color.White);
                //    g.DrawString("GhostScript\n binary\ndoes not\nexist", new Font(FontFamily.GenericMonospace, 15, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Black, new PointF(0, 0));
                //}
                //return i;
                return null;
            }
        }
    }
}