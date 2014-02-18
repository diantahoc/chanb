using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

using chanb.Settings;
using chanb.Language;

namespace chanb.Board
{
    public static class TemplateProvider
    {
        public static string OPPost { get { return get_template("op_post"); } }

        public static string ReplyPost { get { return get_template("reply_post"); } }

        public static string Thread { get { return get_template("thread"); } }

        public static string ImageFile { get { return get_template("image_file"); } }

        public static string VideoFile { get { return get_template("video_file"); } }

        public static string AudioFile { get { return get_template("audio_file"); } }

        public static string FilesRotator { get { return get_template("files_rotator"); } }

        public static string FullPage { get { return get_template("full_page"); } }

        public static string ReportPage { get { return get_template("report_page"); } }

        public static string DeletePostPage { get { return get_template("delete_post_page"); } }

        public static string ModSBR { get { return get_template("modSBR"); } }

        public static string EditPostPage { get { return get_template("edit_post_page"); } }

        public static string BanPage { get { return get_template("banned_page"); } }

        public static string DeletePostFilePage { get { return get_template("delete_file_page"); } }

        public static string DialogsTemplate { get { return get_template("dialogs_template"); } }

        public static string NoScriptFile { get { return get_template("file_noscript"); } }

        public static string html_desktop_return { get { return string.Format("<a class=\"button\" href=\"{0}default.aspx\">{1}</a>", Paths.WebRoot, Lang.return_); ; } }

        public static string html_mobile_return { get { return string.Format("<span class=\"mobileib buttonm\"><a href=\"{0}default.aspx\">{1}</a></span>", Paths.WebRoot, Lang.return_); ; } }

        public static string html_add_new_files
        {
            get
            {
                return "<input id=\"fr_finp\" type=\"checkbox\" name=\"finp\" value=\"yes\" /><label for=\"fr_finp\">" + Lang.addeachfiletoseperatepost + "</label><br/><input type=\"checkbox\" name=\"countf\" id=\"fr_cfiles\" value=\"yes\" /><label for=\"fr_cfiles\">" + Lang.countfiles + "</label><br/><input type='button' onclick=\"addUf('files')\" class='button' value='" + Lang.addanotherfile + "'/>";
            }

        }

        public static string post_menu_delete_files
        {
            get
            {
                return "<li><a class=\"wdlink\" href=\"" + Paths.WebRoot + "deletefile.aspx?id={wpost:id}\" target=\"_blank\">" + Lang.deletefiles + "</a></li>";
            }
        }

        //public static string CatalogItem { get { return get_template("catalog_item"); } }


        private static string get_template(string templateName)
        {
            return File.ReadAllText(Path.Combine(Settings.Paths.TemplatesFolder, templateName + ".html"));
        }
    }
}