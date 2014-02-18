using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace chanb.Board
{
    public static class ImageManipulator
    {
        public static Image ResizeImage(Image original)
        {
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters;

            if (Settings.ApplicationSettings.ProgressiveThumbGeneration)
            {
                encoderParameters = new EncoderParameters(2);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 10L);
                encoderParameters.Param[1] = new EncoderParameter(Encoder.Quality, 85L);
            }
            else
            {
                encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85L);
            }

            int newImageWidth = 250;
            int newImageHeight = 250;

            double ratioX = 250d / (double)original.Width;
            double ratioY = 250d / (double)original.Height;

            double ratio = ratioX < ratioY ? ratioX : ratioY;

            newImageHeight = Convert.ToInt32(original.Height * ratio);

            newImageWidth = Convert.ToInt32(original.Width * ratio);

            Bitmap thumbnail = new Bitmap(newImageWidth, newImageHeight);

            using (var graphic = Graphics.FromImage(thumbnail))
            {
                graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphic.SmoothingMode = SmoothingMode.HighQuality;
                graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphic.CompositingQuality = CompositingQuality.HighQuality;

                graphic.DrawImage(original, 0, 0, newImageWidth, newImageHeight);
            }

            return thumbnail;
        }

    }
}