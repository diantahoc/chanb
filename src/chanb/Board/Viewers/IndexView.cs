using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.Common;
using System.IO;

using chanb.DataTypes;
using chanb.Language;

namespace chanb.Board
{
    public static class IndexView
    {
        public static string GenerateIndexPage(HttpContext context, bool isArchive)
        {
            StringBuilder pageHTML = new StringBuilder(BoardCommon.GetGenericPageHTML());

            pageHTML.Replace("{PostFormMode}", "thread");
            pageHTML.Replace("{PostFormTID}", "0");
            pageHTML.Replace("{AddNewFilesButtonHTML}", "");
            pageHTML.Replace("{DesktopReturnHTML}", "");
            pageHTML.Replace("{MobileReturnHTML}", "");
            pageHTML.Replace("{lang:postaction}", Lang.newthread);

            int page = 1;

            if (!string.IsNullOrEmpty(context.Request["pn"])) { page = Convert.ToInt32(context.Request["pn"]); }

            if (page < 0) { page = 1; }

            int thread_count = 0;

            using (DbConnection dc = Database.DatabaseEngine.GetDBConnection())
            {
                dc.Open();

                thread_count = BoardCommon.GetThreadsCount(isArchive, dc);

                int[] threads = BoardCommon.GetPageInts(page, isArchive, dc);

                StringBuilder documentBody = new StringBuilder();

                foreach (int threadID in threads)
                {
                    documentBody.Append(GetIndexThreadHTML(threadID, dc));
                }

                pageHTML.Replace("{DocumentBody}", documentBody.ToString());
            }

            pageHTML.Replace("{PagesList}", get_pagelist(thread_count, page));

            return pageHTML.ToString();
        }

        private static string GetIndexThreadHTML(int id, DbConnection con)
        {
            if (Settings.ApplicationSettings.CacheIndexView)
            {
                string content_path = Path.Combine(Settings.Paths.IndexThreadBodiesFolder, id.ToString());

                if (File.Exists(content_path))
                {
                    return File.ReadAllText(content_path);
                }
                else
                {
                    string data = generate_index_thread_html(id, con);
                    File.WriteAllText(content_path, data);
                    return data;
                }
            }
            else 
            {
                return generate_index_thread_html(id, con);
            }
        }

        public static void UpdateThreadIndex(int id, DbConnection con)
        {
            string content_path = Path.Combine(Settings.Paths.IndexThreadBodiesFolder, id.ToString());
            string data = generate_index_thread_html(id, con);
            File.WriteAllText(content_path, data);
        }

        private static string generate_index_thread_html(int id, DbConnection con)
        {
            WPost OP = BoardCommon.GetPostData(id, con);

            if (OP == null)
            {
                return "";
            }
            else
            {
                List<WPost> posts = new List<WPost>();

                posts.Add(OP);

                ThreadReplies tr = BoardCommon.GetThreadReplies(OP, con);

                if (Settings.ApplicationSettings.TrailPostsCount > 0 && tr.TotalReplies > 0)
                {
                    posts.AddRange(BoardCommon.GetLastReplies(OP, con));
                }

                StringBuilder thread = new StringBuilder(TemplateProvider.Thread);

                thread.Replace("{id}", posts[0].PostID.ToString());

                thread.Replace("{OP}", posts[0].ToString());

                StringBuilder replies = new StringBuilder();

                int with_image = 0;

                for (int i = 1; i < posts.Count; i++)
                {
                    replies.Append(posts[i].ToString());
                    if (posts[i].FileCount > 0) { with_image++; }
                }

                if (tr.TotalReplies > 0)
                {
                    thread.Replace("{op:replycount}", string.Format("(<b>{0} {1}</b>)", tr.TotalReplies, Lang.replies));

                    int omitted_text_post_count = tr.TextReplies - (posts.Count - 1 - with_image);

                    int omitted_image_post_count = tr.ImageReplies - with_image;

                    string summary = "";

                    if (omitted_image_post_count > 0 & omitted_text_post_count <= 0)
                    {
                        //image only.
                        summary = Lang.summaryIonly;
                    }
                    else if (omitted_text_post_count > 0 & omitted_image_post_count <= 0)
                    {
                        //text only
                        summary = Lang.summaryPonly;
                    }
                    else if (omitted_image_post_count > 0 & omitted_text_post_count > 0)
                    {
                        //image and text
                        summary = Lang.summaryPandI;
                    }

                    summary = summary.Replace("{i}", omitted_image_post_count.ToString()).Replace("{p}", omitted_text_post_count.ToString());

                    thread.Replace("{desktop:summary}", string.Format("<span class=\"summary desktop\">{0}</span>", summary));

                    thread.Replace("{mobile:summary}", string.Format("<span class=\"info\">{0}</span><br />", summary));
                }
                else
                {
                    thread.Replace("{op:replycount}", "");
                    thread.Replace("{desktop:summary}", "");
                    thread.Replace("{mobile:summary}", "");
                }

                thread.Replace("{postlink}", string.Format("{0}{1}.aspx?id={2}", Settings.Paths.WebRoot, posts[0].IsArchived ? "archive" : "default", posts[0].PostID));

                thread.Replace("{Replies}", replies.ToString());

                return thread.ToString();
            }
        }

        private static string get_pagelist(int threadCount, int page)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<div align=\"center\" class=\"pagelist\">");

            int page_count = Convert.ToInt32(Convert.ToDouble(threadCount) / Convert.ToDouble(Settings.ApplicationSettings.ThreadPerPage));

            if (page > 1)
            {
                //add a previous button
                sb.Append(string.Format("<div><a class='button' href='?pn={0}'>{1}</a></div>", page - 1, Lang.previous));
            }

            sb.Append("<div class='pages'>");

            for (int i = 1; i <= page_count; i++)
            {
                if (page == i)
                {
                    sb.Append(string.Format("<strong><a class='button' href='?pn={0}'>{0}</a></strong>", page));
                }
                else
                {
                    sb.Append(string.Format("<a class='button' href='?pn={0}'>{0}</a>", i));
                }
            }

            sb.Append("</div>");

            if (page < page_count)
            {
                //add next button
                sb.Append(string.Format("<div><a class='button' href='?pn={0}'>{1}</a></div>", page + 1, Lang.next));
            }
            //
            sb.Append("</div>");

            return sb.ToString();

        }

    }
}