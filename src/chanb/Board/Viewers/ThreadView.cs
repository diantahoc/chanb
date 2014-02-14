using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.Common;
using System.IO;

using chanb.DataTypes;
using chanb.Language;
using chanb.Settings;

namespace chanb.Board
{
    public static class ThreadView
    {
        public static string GenerateThreadPage(int id)
        {
            StringBuilder pageHTML = new StringBuilder(BoardCommon.GetGenericPageHTML());

            pageHTML.Replace("{lang:postaction}", Lang.reply);

            pageHTML.Replace("{DesktopReturnHTML}", TemplateProvider.html_desktop_return);
            pageHTML.Replace("{MobileReturnHTML}", TemplateProvider.html_mobile_return);

            pageHTML.Replace("{PostFormMode}", "reply");

            pageHTML.Replace("{PostFormTID}", id.ToString());

            pageHTML.Replace("{AddNewFilesButtonHTML}", TemplateProvider.html_add_new_files);

            pageHTML.Replace("{PagesList}", "");

            pageHTML.Replace("{DocumentBody}", get_thread_body(id));

            return pageHTML.ToString();
        }

        private static string get_thread_body(int id)
        {
            if (Settings.ApplicationSettings.CacheThreadView)
            {
                string thread_body_file = Path.Combine(Paths.ThreadBodiesFolder, id.ToString());
                if (File.Exists(thread_body_file))
                {
                    return File.ReadAllText(thread_body_file);
                }
                else
                {
                    using (DbConnection dc = Database.DatabaseEngine.GetDBConnection())
                    {
                        dc.Open();
                        string body = generate_thread_body(id, dc);
                        File.WriteAllText(thread_body_file, body);
                        return body;
                    }
                }
            }
            else
            {
                using (DbConnection dc = Database.DatabaseEngine.GetDBConnection())
                {
                    dc.Open();
                    return generate_thread_body(id, dc);
                }
            }
        }

        private static string generate_thread_body(int id, DbConnection con)
        {
            WPost[] posts = BoardCommon.GetThreadData(id, con);

            StringBuilder body = new StringBuilder();

            body.Append(string.Format("<div class=\"thread\" id=\"t{0}\">", id));

            //op post
            body.Append(posts[0].ToString());
            body.Replace("{op:replycount}", "");

            for (int i = 1; i < posts.Length; i++)
            {
                body.Append(posts[i].ToString());
            }

            body.Append("</div><hr/>");

            return body.ToString();
        }

        public static void UpdateThreadBody(int id, DbConnection con)
        {
            string thread_body_file = Path.Combine(Paths.ThreadBodiesFolder, id.ToString());
            string body = generate_thread_body(id, con);
            File.WriteAllText(thread_body_file, body);
        }
    }

}