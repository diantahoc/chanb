using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using chanb.DataTypes;
using chanb.Settings;
using chanb.Board;
using chanb.Language;

namespace chanb.Board.Formatters
{
    public static class FileFormatter
    {
        public static string FormatPostFiles(WPost post)
        {
            if (post.FileCount > 0)
            {

                if (post.FileCount > 1)
                {
                    //Load rotator template, and process files

                    StringBuilder rotator = new StringBuilder(TemplateProvider.FilesRotator);

                    StringBuilder script_items = new StringBuilder();
                    StringBuilder no_script_items = new StringBuilder();

                    bool is_next = false;

                    foreach (WPostFile file in post.Files)
                    {
                        script_items.Append(get_file_html(file, true, is_next));

                        no_script_items.Append(get_file_html_noscript(file));

                        is_next = true;
                    }

                    rotator.Replace("{rotator:filecount}", post.FileCount.ToString());

                    rotator.Replace("{post:id}", post.PostID.ToString());

                    rotator.Replace("{lang:images}", Lang.images);

                    rotator.Replace("{lang:first}", Lang.first);
                    rotator.Replace("{lang:previous}", Lang.previous);
                    rotator.Replace("{lang:next}", Lang.next);
                    rotator.Replace("{lang:last}", Lang.last);
                    rotator.Replace("{WebRoot}", Paths.WebRoot);


                    rotator.Replace("{rotator:items}", script_items.ToString());

                    rotator.Replace("{rotator:noscriptitems}", no_script_items.ToString());

                    return rotator.ToString();
                }
                else
                {
                    return get_file_html(post.Files[0], false, false);
                }
            }
            else { return ""; }
        }

        private static string get_file_html(WPostFile file, bool isInRotator, bool isNext)
        {
            StringBuilder s;

            switch (file.Extension.ToLower())
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "bmp":
                case "gif":
                case "apng":
                case "svg":
                case "pdf":
                    s = get_imagefile_html(file);
                    break;
                case "webm":
                    s = get_videofile_html(file);
                    break;
                case "ogg":
                case "mp3":
                    s = get_audiofile_html(file);
                    break;
                default:
                    //check custom files
                    return "";
            }

            if (isInRotator)
            {
                s.Replace("{html:filec}", "");

                if (isNext)
                {
                    s.Replace("{rotator:isnext}", "hide");
                }
                else
                {
                    s.Replace("{rotator:isnext}", "file");
                }
            }
            else
            {
                s.Replace("{html:filec}", "file");
                s.Replace("{rotator:isnext}", "");
            }

            return s.ToString();
        }

        private static string get_file_html_noscript(WPostFile file)
        {
            StringBuilder sb = new StringBuilder(TemplateProvider.NoScriptFile);
            sb.Replace("{file:link}", file.ImageWebPath);
            sb.Replace("{file:thumblink}", file.ImageThumbnailWebPath);
            sb.Replace("{file:name}", file.ChanbName);
            return sb.ToString();
        }

        private static StringBuilder get_imagefile_html(WPostFile pf)
        {
            StringBuilder image_template = new StringBuilder(TemplateProvider.ImageFile);

            image_template.Replace("{file:id}", pf.PostID.ToString());

            image_template.Replace("{file:dl-link}", pf.ImageWebPath); // HACK

            image_template.Replace("{file:link}", pf.ImageWebPath);
            image_template.Replace("{file:thumblink}", pf.ImageThumbnailWebPath);

            image_template.Replace("{file:name}", pf.RealName);
            image_template.Replace("{file:size}", format_size_string(pf.Size));
            image_template.Replace("{file:dimensions}", pf.Dimensions);

            image_template.Replace("{file:ext}", pf.Extension.ToUpper());

            image_template.Replace("{file:selinks}", "");

            return image_template;
        }

        private static StringBuilder get_videofile_html(WPostFile pf) 
        {
            StringBuilder video_temp = new StringBuilder(TemplateProvider.VideoFile);

            video_temp.Replace("{file:id}", pf.PostID.ToString());

            video_temp.Replace("{file:dl-link}", pf.ImageWebPath); // HACK

            video_temp.Replace("{file:link}", pf.ImageWebPath);
            video_temp.Replace("{file:thumblink}", pf.ImageThumbnailWebPath);

            video_temp.Replace("{file:name}", pf.RealName);
            video_temp.Replace("{file:size}", format_size_string(pf.Size));
            video_temp.Replace("{file:dimensions}", pf.Dimensions);

            video_temp.Replace("{mime}", get_mime(pf.Extension));
            
            video_temp.Replace("{lang:novideosupport}", Lang.novideosupport);

            return video_temp;
        }

        private static string get_mime(string ext) 
        {
            switch (ext.ToLower()) 
            {
                case "webm":
                    return "webm";
                case "mp3":
                    return "mpeg";
                case "ogg":
                    return "ogg";
                default:
                    return "";
            }
        }

        private static StringBuilder get_audiofile_html(WPostFile pf)
        {
            StringBuilder audio_temp = new StringBuilder(TemplateProvider.AudioFile);

            audio_temp.Replace("{file:id}", pf.PostID.ToString());

            audio_temp.Replace("{file:dl-link}", pf.ImageWebPath); // HACK

            audio_temp.Replace("{file:link}", pf.ImageWebPath);

            audio_temp.Replace("{file:name}", pf.RealName);
            audio_temp.Replace("{file:size}", format_size_string(pf.Size));
            audio_temp.Replace("{file:dimensions}", pf.Dimensions);

            audio_temp.Replace("{mime}", get_mime(pf.Extension));

            audio_temp.Replace("{lang:noaudiosupport}", Lang.noaudiosupport);

            return audio_temp;
        }

        private static string format_size_string(double size)
        {
            double KB = 1024;
            double MB = 1048576;
            double GB = 1073741824;
            if (size < KB)
            {
                return size.ToString() + " B";
            }
            else if (size > KB & size < MB)
            {
                return Math.Round(size / KB, 2).ToString() + " KB";
            }
            else if (size > MB & size < GB)
            {
                return Math.Round(size / MB, 2).ToString() + " MB";
            }
            else if (size > GB)
            {
                return Math.Round(size / GB, 2).ToString() + " GB";
            }
            else
            {
                return Convert.ToString(size);
            }
        }

    }
}