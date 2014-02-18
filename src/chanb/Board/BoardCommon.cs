using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using chanb.Language;
using chanb.Settings;
using chanb.Database;
using chanb.DataTypes;
using chanb.Enums;

using System.Data.Common;
using System.Data;
using System.IO;

namespace chanb.Board
{
    public static class BoardCommon
    {

        private static Random rnd = new Random();
        /// <summary>
        /// Add localisation to the full page template
        /// </summary>
        /// <returns></returns>
        public static string GetGenericPageHTML()
        {
            StringBuilder sb = new StringBuilder(TemplateProvider.FullPage);

            sb.Replace("{WebRoot}", Paths.WebRoot);

            sb.Replace("{BoardTitle}", ApplicationSettings.BoardTitle);
            sb.Replace("{BoardDecs}", ApplicationSettings.BoardTitle);

            sb.Replace("{lang:name}", Lang.name);
            sb.Replace("{lang:email}", Lang.email);
            sb.Replace("{lang:subject}", Lang.subject);
            sb.Replace("{lang:comment}", Lang.comment);
            sb.Replace("{lang:verification}", Lang.verification);
            sb.Replace("{lang:files}", Lang.files);

            sb.Replace("{lang:password}", Lang.password);

            sb.Replace("{lang:forpostremoval}", Lang.forpostremoval);
            sb.Replace("{lang:catalog}", Lang.catalog);
            sb.Replace("{lang:bottom}", Lang.bottom);
            sb.Replace("{lang:archive}", Lang.archive);

            sb.Replace("{lang:top}", Lang.top);

            sb.Replace("{lang:update}", Lang.update);
            sb.Replace("{lang:quickreply}", Lang.quickreply);

            sb.Replace("{lang:dump}", Lang.dump);
            sb.Replace("{lang:countfiles}", Lang.countfiles);
            sb.Replace("{lang:send}", Lang.send);

            sb.Replace("{FooterText}", ApplicationSettings.FooterText);

            return sb.ToString();
        }

        #region Database Functions

        public static WPost GetPostData(int id, DbConnection connection)
        {
            string query = string.Format("SELECT type, time, comment, postername, trip, email, password, parentT, subject, IP, ua, posterID, sticky, locked, mta, hasFile FROM  board  WHERE (id = {0})", id);

            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(query, connection))
            {
                WPost po = null;

                bool has_file = false;

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Convert.IsDBNull(reader[0]))
                        {
                            return null;
                        }
                        else
                        {
                            po = new WPost()
                            {
                                PostID = id,
                                Type = (PostType)reader.GetInt32(0),
                                Time = Convert.ToDateTime(ReadParam(reader[1])),
                                Comment = Convert.ToString(ReadParam(reader[2])),
                                Name = Convert.ToString(ReadParam(reader[3])),
                                Trip = Convert.ToString(ReadParam(reader[4])),
                                Email = Convert.ToString(ReadParam(reader[5])),
                                Password = Convert.ToString(ReadParam(reader[6])),
                                Parent = Convert.ToInt32(ReadParam(reader[7])),
                                Subject = Convert.ToString(ReadParam(reader[8])),
                                IP = Convert.ToString(ReadParam(reader[9])),
                                UserAgent = Convert.ToString(ReadParam(reader[10])),
                                PosterID = Convert.ToString(ReadParam(reader[11])),
                                IsSticky = Convert.ToBoolean(ReadParam(reader[12])),
                                IsLocked = Convert.ToBoolean(ReadParam(reader[13])),
                                IsArchived = Convert.ToBoolean(ReadParam(reader[14]))
                            };
                            has_file = Convert.ToBoolean(reader[15]);
                        }
                    }
                }

                if (has_file)
                {
                    po.Files = GetPostFiles(id, connection);
                }

                return po;
            }
        }

        public static WPost[] GetThreadData(int id, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(string.Format("SELECT ID FROM board WHERE (ID = {0}) OR (parentT = {0}) ORDER BY ID ASC", id.ToString()), con))
            {
                List<int> posts = new List<int>();

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        posts.Add(reader.GetInt32(0));
                    }
                }

                List<WPost> wpos = new List<WPost>();

                foreach (int i in posts)
                {
                    wpos.Add(GetPostData(i, con));
                }

                return wpos.ToArray();
            }
        }

        private static object ReadParam(object r)
        {
            if (Convert.IsDBNull(r)) { return null; } else { return r; }
        }

        private static WPost[] GetPosts(int[] ids, DbConnection dc)
        {
            List<WPost> l = new List<WPost>();

            foreach (int id in ids)
            {
                l.Add(GetPostData(id, dc));
            }

            return l.ToArray();
        }

        private static int[] GetThreadsIds(int startIndex, int count, bool ignoreStikies, bool archive, DbConnection con)
        {
            List<int> l = new List<int>();

            //sticky order is ignored in the archives.
            if (archive)
            {
                using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
                {
                    dc.CommandText = "SELECT ID FROM board WHERE (type = 0) AND (mta = 1) ORDER BY bumplevel DESC";

                    using (DbDataReader reader = dc.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            l.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            else
            {
                //first select stikies, order stickys by time

                using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
                {
                    if (!ignoreStikies)
                    {
                        dc.CommandText = "SELECT ID FROM board WHERE (type = 0) AND (mta = 0) AND (sticky = 1) ORDER BY time DESC";
                        using (DbDataReader reader = dc.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                l.Add(reader.GetInt32(0));
                            }
                        }
                    }

                    dc.CommandText = "SELECT ID FROM board  WHERE (type = 0) AND (sticky = 0) AND (mta = 0) ORDER BY bumplevel DESC";

                    using (DbDataReader reader = dc.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            l.Add(reader.GetInt32(0));
                        }
                    }
                }
            }

            if (l.Count() > 0)
            {
                // return l.GetRange(startIndex, count).ToArray();
                List<int> final = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    if (startIndex + i > l.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        final.Add(l[startIndex + i]);
                    }
                }
                return final.ToArray();
            }
            else { return l.ToArray(); }
        }

        /// <summary>
        /// Return a list of thread ids on a specific page
        /// 
        /// </summary>
        /// <param name="page">Page number, first page is 1</param>
        /// <returns></returns>
        public static int[] GetPageInts(int page, bool archive, DbConnection con)
        {
            int startIndex = page - 1;

            return GetThreadsIds(startIndex * ApplicationSettings.ThreadPerPage, ApplicationSettings.ThreadPerPage, false, archive, con);
        }

        public static int GetThreadsCount(bool archive, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand("SELECT Count(ID) AS T FROM board WHERE (type = 0) AND (mta = @t)", con))
            {
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@t", archive ? 1 : 0, DbType.Int32));

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    int count = 0;
                    while (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                    return count;
                }
            }
        }

        public static ThreadReplies GetThreadReplies(WPost po, DbConnection con)
        {
            int text_replies = 0;
            int image_replies = 0;

            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
            {
                dc.CommandText = "SELECT Count(ID) As T FROM board WHERE (parentT = @id) AND (hasFile = @f) AND (mta = @mta)";

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@mta", po.IsArchived ? 1 : 0, DbType.Int32));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@id", po.PostID, DbType.Int32));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@f", false, DbType.Boolean));

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        text_replies = reader.GetInt32(0);
                    }
                }

                dc.Parameters["@f"].Value = true;

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        image_replies = reader.GetInt32(0);
                    }
                }
            }

            return new ThreadReplies() { ImageReplies = image_replies, TextReplies = text_replies };

        }

        public static WPost[] GetLastReplies(WPost thread, DbConnection con)
        {
            string queryText = "";

            switch (DatabaseSettings.DbType)
            {
                case DatabaseType.MsSQL:
                    queryText = string.Format("SELECT TOP {0} ID FROM board WHERE (parentT = @tid) AND (mta = @mta) ORDER BY ID DESC", ApplicationSettings.TrailPostsCount);
                    break;
                case DatabaseType.MySQL:
                    queryText = string.Format("SELECT ID FROM board WHERE (parentT = @tid) AND (mta = @mta) ORDER BY ID DESC LIMIT 0, {0}", ApplicationSettings.TrailPostsCount - 1);
                    break;
                default:
                    return new WPost[] { };
            }

            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(queryText, con))
            {
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@tid", thread.PostID, System.Data.DbType.Int32));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@mta", thread.IsArchived ? 1 : 0, System.Data.DbType.Int32));

                List<int> posts_ids = new List<int>();

                List<WPost> posts_list = new List<WPost>();

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        posts_ids.Add(reader.GetInt32(0));
                    }
                }

                foreach (int i in posts_ids)
                {
                    posts_list.Add(GetPostData(i, con));
                }

                posts_list.Reverse();

                return posts_list.ToArray();
            }
        }

        public static int MakeThread(OPData data, HttpPostedFile file, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
            {
                dc.CommandText = "INSERT INTO board (type, time, comment, postername, trip, email, password, subject, IP, ua, mta, locked, sticky, hasFile, bumplevel) VALUES " +
                 "(@type, @time, @comment, @postername, @trip, @email, @password, @subject, @IP, @ua, @mta, @locked, @sticky, @hasFile, @bumplevel) ; SELECT ID FROM board WHERE (time = @time) AND (IP = @IP)";

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@type", 0, System.Data.DbType.Int32)); // Mark the post as a thread

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@time", data.Time, System.Data.DbType.DateTime));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@comment", data.Comment, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@postername", data.Name, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@trip", data.Trip, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@email", data.Email, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@password", data.Password, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@subject", data.Subject, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@IP", data.IP, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@ua", data.UserAgent, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@mta", false, System.Data.DbType.Boolean));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@locked", false, System.Data.DbType.Boolean));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@sticky", false, System.Data.DbType.Boolean));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@hasFile", data.HasFile, System.Data.DbType.Boolean));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@bumplevel", DateTime.UtcNow, System.Data.DbType.DateTime));

                int post_id = -1;

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        post_id = reader.GetInt32(0);
                    }
                }

                if (post_id > 0)
                {
                    if (ApplicationSettings.EnableUserID)
                    {
                        dc.Parameters.Clear();
                        dc.CommandText = "UPDATE board SET posterID = @posterID WHERE (ID = @tid)";

                        dc.Parameters.Add(DatabaseEngine.MakeParameter("@posterID", GenerateUserID(post_id, data.IP), System.Data.DbType.String));
                        dc.Parameters.Add(DatabaseEngine.MakeParameter("@tid", post_id, System.Data.DbType.Int32));

                        dc.ExecuteNonQuery();
                    }

                    if (data.HasFile)
                    {
                        try
                        {
                            save_post_file(post_id, file, con);
                        }
                        catch (Exception)
                        {
                            //delete the thread
                            delete_post_from_database(post_id, con);
                            throw;
                        }
                    }
                }

                return post_id;
            }
        }

        public static ThreadInfo GetThreadInfo(int id, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(string.Format("SELECT ID, locked, mta FROM board WHERE (ID = {0})", id), con))
            {
                ThreadInfo info = new ThreadInfo();

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Convert.IsDBNull(reader[0]))
                        {
                            info.isGone = true;
                            break;
                        }
                        else
                        {
                            info.isGone = false;
                            info.isLocked = reader.GetBoolean(1);
                            info.isArchived = reader.GetBoolean(2);
                        }
                    }
                }

                return info;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="thread_id"></param>
        /// <param name="files"></param>
        /// <param name="dump">File in each post</param>
        /// <param name="count_files"></param>
        /// <param name="con"></param>
        /// <returns></returns>
        public static int ReplyTo(OPData data, int thread_id, HttpPostedFile[] files, bool dump, bool count_files, DbConnection con)
        {
            if (files.Length > 1) //multiple files
            {
                data.HasFile = true;
                if (dump) //file in each post
                {
                    int file_count = files.Length;

                    int last_post_id = 0;

                    for (int file_index = 0; file_index < file_count; file_index++)
                    {

                        HttpPostedFile file = files[file_index];

                        if (count_files)
                        {
                            if (file_index == 0)
                            {
                                //first post, keep comment data
                                data.Comment = data.Comment + Environment.NewLine + string.Format("{0}/{1}", file_index + 1, file_count);
                            }
                            else
                            {
                                data.Comment = string.Format("{0}/{1}", file_index + 1, file_count);
                            }
                        }
                        else
                        {
                            //file are not counted, but all posts except the first one have null comment
                            if (file_index != 0)
                            {
                                data.Comment = "";
                            }
                        }

                        int post_id = save_single_post(data, thread_id, con);


                        try
                        {
                            save_post_file(post_id, file, con);
                            last_post_id = post_id;
                        }
                        catch (Exception)
                        {
                            //unable to save the file, so we delete the blank post
                            delete_post_from_database(post_id, con);
                        }

                    }
                    return last_post_id;
                }
                else //single post with multiple files 
                {
                    int post_id = save_single_post(data, thread_id, con);
                    int saved_files = 0;
                    foreach (HttpPostedFile file in files)
                    {
                        try
                        {
                            save_post_file(post_id, file, con);
                            saved_files++;
                        }
                        catch (Exception)
                        { }
                    }

                    if (string.IsNullOrEmpty(data.Comment))
                    {
                        if (saved_files == 0)
                        {
                            delete_post_from_database(post_id, con);
                        }
                    }

                    return post_id;
                }
            }
            else if (files.Length == 1) //single files
            {
                data.HasFile = true;
                int post_id = save_single_post(data, thread_id, con);

                try
                {
                    save_post_file(post_id, files[0], con);
                }
                catch (Exception)
                {
                    delete_post_from_database(post_id, con);
                    throw;
                }

                return post_id;
            }
            else //no files 
            {
                data.HasFile = false;
                return save_single_post(data, thread_id, con);
            }
        }

        private static int save_single_post(OPData data, int thread_id, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
            {
                dc.CommandText = "INSERT INTO board (type, time, comment, postername, trip, email, password, parentT, subject, IP, ua, posterID, mta, locked, sticky, hasFile) VALUES " +
                                "(@type, @time, @comment, @postername, @trip, @email, @password, @parentT, @subject, @IP, @ua, @posterId, @mta, @locked, @sticky, @hasFile) ; SELECT ID FROM board WHERE (time = @time) AND (IP = @IP)";

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@type", 1, System.Data.DbType.Int32)); // Mark the post as a reply

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@parentT", thread_id, System.Data.DbType.Int32));//Set the post owner thread

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@time", data.Time, System.Data.DbType.DateTime));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@comment", data.Comment, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@postername", data.Name, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@trip", data.Trip, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@email", data.Email, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@password", data.Password, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@subject", data.Subject, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@IP", data.IP, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@ua", data.UserAgent, System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@posterId", ApplicationSettings.EnableUserID ? GenerateUserID(thread_id, data.IP) : "", System.Data.DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@mta", false, System.Data.DbType.Boolean));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@locked", false, System.Data.DbType.Boolean));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@sticky", false, System.Data.DbType.Boolean));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@hasFile", data.HasFile, System.Data.DbType.Boolean));

                int post_id = -1;

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        post_id = reader.GetInt32(0);
                    }
                }

                return post_id;
            }
        }

        public static void save_post_file(int postID, HttpPostedFile file, DbConnection con)
        {
            string file_extension = file.FileName.Split('.').Last().ToLower();

            if (Array.IndexOf(ApplicationSettings.DisabledFiles, file_extension) >= 0)
            {
                throw new Exception(string.Format("File type '{0}' is disabled", file_extension));
            }

            string file_hash = Common.Misc.MD5(file.InputStream);

            if (!ApplicationSettings.AllowDuplicatesFiles)
            {
                bool file_exist_indb = FileExistInDatabase(file_hash, -1, con);

                if (file_exist_indb)
                {
                    if (ApplicationSettings.SmartLinkDuplicateImages)
                    {
                        WPostFile wpf = GetFileByHash(file_hash, con);

                        wpf.RealName = file.FileName;
                        wpf.PostID = postID;

                        AddFileToDatabase(wpf, con);
                        return;
                    }
                    else
                    {
                        throw new Exception(string.Format("Duplicate file '{0}' with the hash (md5) '{1}'", file.FileName, file_hash));
                    }
                }

            }  //no need to check for duplicate files if duplicate files are allowed.


            string chanb_name = string.Format("{0}r{1}", Common.Misc.GetUnixTimeStamp(), rnd.Next(0, 1024));

            string file_path = Path.Combine(Paths.PhysicalFilesStorageFolder, chanb_name + "." + file_extension);

            file.SaveAs(file_path);

            string thumb_path = "";

            switch (file_extension)
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "bmp":
                case "gif":
                case "apng":

                    System.Drawing.Image im = null;
                    try
                    {
                        im = System.Drawing.Image.FromStream(file.InputStream);
                    }
                    catch (Exception)
                    {
                        File.Delete(file_path);
                        throw new Exception("Bad image data");
                    }

                    bool is_transparent = (file_extension == "png" || file_extension == "apng");

                    System.Drawing.Imaging.ImageFormat format = null;

                    if (is_transparent)
                    {
                        thumb_path = Path.Combine(Paths.PhysicalThumbStorageFolder, chanb_name + ".png");
                        format = System.Drawing.Imaging.ImageFormat.Png;
                    }
                    else
                    {
                        thumb_path = Path.Combine(Paths.PhysicalThumbStorageFolder, chanb_name + ".jpg");
                        format = System.Drawing.Imaging.ImageFormat.Jpeg;
                    }

                    if ((im.Width * im.Height) <= 62500)
                    {
                        im.Save(thumb_path, format);
                    }
                    else
                    {
                        using (System.Drawing.Image rezized = ImageManipulator.ResizeImage(im))
                        {
                            rezized.Save(thumb_path, format);
                        }
                    }

                    if (format == System.Drawing.Imaging.ImageFormat.Png)
                    {
                        UnmanagedWrappers.FileOptimizer.OptimizePNG(thumb_path);
                    }
                    else
                    {
                        UnmanagedWrappers.FileOptimizer.OptimizeJPG(thumb_path);
                    }

                    WPostFile wpf = new WPostFile()
                    {
                        ChanbName = chanb_name + "." + file_extension,
                        Size = file.ContentLength,
                        Dimensions = string.Format("{0}x{1}", im.Width, im.Height),
                        Extension = file_extension.ToUpper(),
                        RealName = file.FileName,
                        Hash = file_hash,
                        PostID = postID,
                        MimeType = file.ContentType // HACK
                    };

                    im.Dispose();

                    AddFileToDatabase(wpf, con);

                    break;
                case "webm":

                    if (!Validator.ValidateWEBM(file.InputStream))
                    {
                        File.Delete(file_path);
                        throw new Exception("Invalid WEBM file");
                    }

                    UnmanagedWrappers.FileOptimizer.OptimizeWEBM(file_path);

                    string optimized_webm = Path.Combine(Paths.PhysicalFilesStorageFolder, "clean." + chanb_name + "." + file_extension);

                    if (File.Exists(optimized_webm))
                    {
                        File.Delete(file_path); //delete the unoptimized file
                        File.Move(optimized_webm, file_path);// replace the file with the optimized one
                    }

                    thumb_path = Path.Combine(Paths.PhysicalThumbStorageFolder, chanb_name + ".png");

                    System.Drawing.Image thumb = UnmanagedWrappers.FFMpegWrapper.GenerateWEBMThumb(file_path);

                    if (thumb != null)
                    {
                        if (thumb.Width * thumb.Width <= 62500)
                        {
                            thumb.Save(thumb_path, System.Drawing.Imaging.ImageFormat.Png);
                        }
                        else
                        {
                            using (System.Drawing.Image rezized = ImageManipulator.ResizeImage(thumb))
                            {
                                rezized.Save(thumb_path, System.Drawing.Imaging.ImageFormat.Png);
                            }
                        }

                        UnmanagedWrappers.FileOptimizer.OptimizePNG(thumb_path);
                    }

                    WPostFile wpf_webm = new WPostFile()
                    {
                        ChanbName = chanb_name + "." + file_extension,
                        Size = file.ContentLength,
                        Dimensions = string.Format("{0}x{1}", (thumb == null) ? 50 : thumb.Width, (thumb == null) ? 25 : thumb.Height),
                        Extension = file_extension.ToUpper(),
                        RealName = file.FileName,
                        Hash = file_hash,
                        PostID = postID,
                        MimeType = file.ContentType // HACK
                    };

                    if (thumb != null) { thumb.Dispose(); }

                    AddFileToDatabase(wpf_webm, con);

                    break;
                case "pdf":
                    thumb_path = Path.Combine(Paths.PhysicalThumbStorageFolder, chanb_name + ".jpg");

                    System.Drawing.Image thumb_pdf = UnmanagedWrappers.GhostScriptWrapper.GeneratePDFThumb(file_path);

                    if (thumb_pdf != null)
                    {
                        if (thumb_pdf.Width * thumb_pdf.Width <= 62500)
                        {
                            thumb_pdf.Save(thumb_path, System.Drawing.Imaging.ImageFormat.Jpeg);
                        }
                        else
                        {
                            using (System.Drawing.Image rezized = ImageManipulator.ResizeImage(thumb_pdf))
                            {
                                rezized.Save(thumb_path, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }

                        UnmanagedWrappers.FileOptimizer.OptimizeJPG(thumb_path);
                    }

                    WPostFile wpf_pdf = new WPostFile()
                    {
                        ChanbName = chanb_name + "." + file_extension,
                        Size = file.ContentLength,
                        Dimensions = string.Format("{0}x{1}", (thumb_pdf == null) ? 50 : thumb_pdf.Width, (thumb_pdf == null) ? 25 : thumb_pdf.Height),
                        Extension = file_extension.ToUpper(),
                        RealName = file.FileName,
                        Hash = file_hash,
                        PostID = postID,
                        MimeType = file.ContentType // HACK
                    };

                    if (thumb_pdf != null) { thumb_pdf.Dispose(); }

                    AddFileToDatabase(wpf_pdf, con);
                    break;
                default:
                    throw new Exception(string.Format("Unsupported file type '{0}'", file_extension));
            }
        }

        private static void delete_post_from_database(int id, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(string.Format("DELETE FROM board WHERE (ID = {0})", id), con)) { dc.ExecuteNonQuery(); }
        }

        private static string GenerateUserID(int threadid, string ip)
        {
            string hash = Common.Misc.MD5(threadid.ToString() + ip);
            return hash.Substring(0, 8);
        }


        #region PostFiles

        public static void AddFileToDatabase(WPostFile WPF, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
            {
                dc.CommandText = "INSERT INTO files (postID, chanbname, realname, size, md5, extension, mimetype, dimension) " +
                        " VALUES  (@postID, @chanbname, @realname, @size, @md5, @extension, @mimetype, @dimension)";

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@postID", WPF.PostID, DbType.Int32));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@chanbname", WPF.ChanbName, DbType.String));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@realname", WPF.RealName, DbType.String));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@size", WPF.Size, DbType.Int64));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@md5", WPF.Hash, DbType.String));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@extension", WPF.Extension, DbType.String));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@mimetype", WPF.MimeType, DbType.String));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@dimension", WPF.Dimensions, DbType.String));

                dc.ExecuteNonQuery();
            }
        }

        public static WPostFile GetFileByHash(string hash, DbConnection con)
        {
            WPostFile pf = null;

            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
            {
                dc.CommandText = "SELECT ID, postID, chanbname, realname, size, extension, mimetype, dimension FROM files WHERE (md5 = @md5)";
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@md5", hash, DbType.String));

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Convert.IsDBNull(reader[0])) { break; }

                        pf = new WPostFile()
                        {
                            PostID = Convert.ToInt32(ReadParam(reader[1])),
                            ChanbName = Convert.ToString(ReadParam(reader[2])),
                            RealName = Convert.ToString(ReadParam(reader[3])),
                            Size = Convert.ToInt32(ReadParam(reader[4])),
                            Extension = Convert.ToString(ReadParam(reader[5])),
                            MimeType = Convert.ToString(ReadParam(reader[6])),
                            Dimensions = Convert.ToString(ReadParam(reader[7])),
                            Hash = hash
                        };
                    }
                }
            }

            return pf;
        }

        public static WPostFile GetFileByChanbName(string name, DbConnection con)
        {
            WPostFile pf = null;

            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
            {
                dc.CommandText = "SELECT ID, postID, md5, realname, size, extension, mimetype, dimension FROM files WHERE (chanbname = @chanbname)";
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@chanbname", name, DbType.String));

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Convert.IsDBNull(reader[0])) { break; }

                        pf = new WPostFile()
                        {
                            PostID = Convert.ToInt32(ReadParam(reader[1])),
                            Hash = Convert.ToString(ReadParam(reader[2])),
                            ChanbName = name,
                            RealName = Convert.ToString(ReadParam(reader[3])),
                            Size = Convert.ToInt32(ReadParam(reader[4])),
                            Extension = Convert.ToString(ReadParam(reader[5])),
                            MimeType = Convert.ToString(ReadParam(reader[6])),
                            Dimensions = Convert.ToString(ReadParam(reader[7])),
                        };
                    }
                }
            }

            return pf;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="excluded_post">Specify -1 to disable</param>
        /// <returns></returns>
        public static bool FileExistInDatabase(string hash, int excluded_post, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand("SELECT ID FROM files WHERE (md5 = @md5) AND (postId <> @exP)", con))
            {
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@md5", hash, DbType.String));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@exP", excluded_post, DbType.Int32));

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return (!Convert.IsDBNull(reader[0]));
                    }
                }
            }

            return false;
        }

        private static WPostFile[] GetPostFiles(int id, DbConnection connection)
        {
            List<WPostFile> li = new List<WPostFile>();

            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(connection))
            {
                dc.CommandText = "SELECT ID, chanbname, realname, size, md5, extension, mimetype, dimension FROM files WHERE (postID = @postID)";

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@postID", id, DbType.Int32));

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (!Convert.IsDBNull(reader[0]))
                        {
                            li.Add(new WPostFile()
                            {
                                PostID = id,
                                ChanbName = Convert.ToString(ReadParam(reader[1])),
                                RealName = Convert.ToString(ReadParam(reader[2])),
                                Size = Convert.ToInt32(ReadParam(reader[3])),
                                Hash = Convert.ToString(ReadParam(reader[4])),
                                Extension = Convert.ToString(ReadParam(reader[5])),
                                MimeType = Convert.ToString(ReadParam(reader[6])),
                                Dimensions = Convert.ToString(ReadParam(reader[7]))
                            });
                        }
                    }
                }
            }

            return li.ToArray();
        }

        public static void DeleteFileFromDatabase(int id, string[] hashes, DbConnection con)
        {
            using (DbCommand dc = Database.DatabaseEngine.GenerateDbCommand("DELETE FROM files WHERE (postID = @postID) AND (md5 = @md5)", con))
            {
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@postID", id, DbType.Int32));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@md5", "", DbType.String));
                foreach (string file_h in hashes)
                {
                    dc.Parameters["@md5"].Value = file_h;
                    dc.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #endregion


        #region Reporting

        public static bool IPAlreadyReportedPost(int postid, string ip, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
            {
                dc.CommandText = "SELECT ID FROM reports WHERE (postID = @postID) AND (reporterIP = @ip)";

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@postID", postid, DbType.Int32));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@ip", ip, DbType.String));

                using (DbDataReader reader = dc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return !Convert.IsDBNull(reader[0]);
                    }
                }
            }
            return false;
        }

        public static void InsertReport(int reasonid, ReportReason reason, string ip, int postID, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
            {
                dc.CommandText = "INSERT INTO reports (postID, reporterIP, time, comment, reasonID) " +
                                " VALUES (@id, @ip, @time, @comment, @reasonID)";

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@id", postID, DbType.Int32));
                dc.Parameters.Add(DatabaseEngine.MakeParameter("@ip", ip, DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@time", DateTime.UtcNow, DbType.DateTime));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@comment", reason.Description, DbType.String));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@reasonID", reasonid, DbType.Int32));

                dc.ExecuteNonQuery();
            }
        }

        #endregion

        public static void BumpThread(int id, DbConnection con)
        {
            using (DbCommand dc = DatabaseEngine.GenerateDbCommand(con))
            {
                dc.CommandText = "UPDATE board SET bumplevel = @bump WHERE (ID = @id)";

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@id", id, DbType.Int32));

                dc.Parameters.Add(DatabaseEngine.MakeParameter("@bump", DateTime.UtcNow, DbType.DateTime));

                dc.ExecuteNonQuery();
            }
        }

        public static string GetCaptchaFullPageBody()
        {
            return string.Format("<tr><th>{0}</th><td>{1}</td></tr>", Language.Lang.verification, CaptchaProvider.GetCaptchaBody());
        }


    }
}