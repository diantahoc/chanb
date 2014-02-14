using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

using chanb.DataTypes;

namespace chanb.UnmanagedWrappers
{
    public static class FFMpegWrapper
    {
        private static string ffmpeg_path;

        static bool binary_exist = false;

        static FFMpegWrapper()
        {
            ffmpeg_path = Path.Combine(Settings.Paths.DllFolder, "ffmpeg.exe");

            binary_exist = File.Exists(ffmpeg_path);
        }

        public static Image GenerateWEBMThumb(string path)
        {
            if (binary_exist)
            {
                Image im = null;

                if (File.Exists(path))
                {
                    ProcessStartInfo psr = new ProcessStartInfo(ffmpeg_path);
                    psr.CreateNoWindow = true;
                    psr.WorkingDirectory = Settings.Paths.DllFolder;

                    //psr.Arguments = string.Format("-i \"{0}\" -f image2 -r 1 -t 00:00:03 -vcodec png -", path);
                    psr.Arguments = string.Format("-i \"{0}\" -r 1 -f image2 -c:v png -", path);
                    psr.UseShellExecute = false;

                    psr.RedirectStandardOutput = true;

                    MemoryStream memio = new MemoryStream();

                    using (Process proc = Process.Start(psr)) 
                    {
                        proc.WaitForExit(5000);

                        if (!proc.HasExited) { proc.Kill(); }
                        proc.StandardOutput.BaseStream.CopyTo(memio);

                    }

                    try
                    {
                        if (memio.Length > 0)
                        {
                            im = Image.FromStream(memio);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch (Exception)
                    {
                        //im = new Bitmap(128, 128);

                        //using (Graphics g = Graphics.FromImage(im))
                        //{
                        //    g.DrawString("Unable to\ngenerate\npreview", new Font(FontFamily.GenericMonospace, 15, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Black, new PointF(0, 0));
                        //}
                        return null;
                    }

                }
                return im;
            }
            else
            {
                return null;
                //Image i = new Bitmap(128, 128);
                //using (Graphics g = Graphics.FromImage(i))
                //{
                //    g.DrawString("FFMpeg binary\ndoes not\nexist", new Font(FontFamily.GenericMonospace, 15, FontStyle.Regular, GraphicsUnit.Pixel), Brushes.Black, new PointF(0, 0));
                //}
                //return i;
            }
        }
    }
}