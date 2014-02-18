using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Common;

using chanb.Settings;
using chanb.DataTypes;
using chanb.Database;
using chanb.Board;

namespace chanb
{
    public partial class post : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ApplicationSettings.PostingEnabled)
            {
                Response.StatusCode = 403;
                this.Response.Write(Language.Lang.postingisdisabled);
                this.Response.End();
            }

            if (string.IsNullOrEmpty(Request["mode"]))
            {
                Response.StatusCode = 403;
                Response.Write("403");
                Response.End();
            }

            using (DbConnection con = DatabaseEngine.GetDBConnection())
            {
                con.Open();

                //check bans
                if (Board.BanHandler.IsIPBanned(Request.UserHostAddress, con))
                {
                    Response.Redirect(Paths.WebRoot + "banned.aspx", true);
                }

                //bool is_admin = false;
                //bool is_mod = false;

                bool all_ok = true;

                //check flood

                //check captcha
                if (!CaptchaProvider.Verifiy(this.Context)) 
                {
                    this.Response.Write(Language.Lang.wrongcaptcha);
                    this.Response.End();
                }

                //check file sizes
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFile file = Request.Files[i];
                    if (file.ContentLength > ApplicationSettings.MaximumFileSize)
                    {
                        Response.Write(string.Format("The file '{0}' is larger than the allowed limit {1}.", file.FileName, ApplicationSettings.MaximumFileSize));
                        all_ok = false;
                        break;
                    }
                }

                if (all_ok)
                {
                    switch (Request["mode"])
                    {
                        case "thread":
                            if (Request.Files.Count == 0 | Request.Files["ufile"].ContentLength == 0)
                            {
                                Response.Write("You need a file to start a thread");
                            }
                            else
                            {

                                OPData op_data = new DataTypes.OPData()
                                {
                                    Comment = Request["comment"],
                                    Email = Request["email"],
                                    Name = Request["name"],
                                    Subject = Request["subject"],
                                    Password = Request["password"],
                                    HasFile = true,
                                    IP = Request.UserHostAddress,
                                    UserAgent = Request.UserAgent,
                                    Time = DateTime.UtcNow
                                };

                                int thread_id = -1;

                                try
                                {
                                    thread_id = Board.BoardCommon.MakeThread(op_data, Request.Files["ufile"], con);
                                    Response.Redirect(Paths.WebRoot + "default.aspx?id=" + thread_id.ToString(), true);
                                }
                                catch (Exception ex)
                                {
                                    Response.Write(ex.Message);
                                }
                            }
                            break;
                        case "reply":

                            if (string.IsNullOrEmpty(Request["threadid"]))
                            {
                                Response.Write("Thread id is not specified");
                            }
                            else
                            {
                                int thread_id = -1;

                                try
                                {
                                    thread_id = Convert.ToInt32(Request["threadid"]);

                                    if (thread_id <= 0)
                                    {
                                        Response.Write("Invalid thread id");
                                        Response.End();
                                    }
                                }
                                catch (Exception)
                                {
                                    Response.Write("Invalid thread id");
                                    Response.End();
                                }

                                ThreadInfo t_info = BoardCommon.GetThreadInfo(thread_id, con);

                                if (t_info.isGone)
                                {
                                    Response.Write("Thread does not exist.");
                                    Response.End();
                                }

                                if (t_info.isLocked)
                                {
                                    Response.Write("Thread is locked.");
                                    Response.End();
                                }

                                if (t_info.isArchived)
                                {
                                    Response.Write("Thread is archived.");
                                    Response.End();
                                }

                                if (ApplicationSettings.EnableImpresonationProtection)
                                {
                                    //do stuffs
                                }

                                List<HttpPostedFile> proper_files = new List<HttpPostedFile>();

                                //Discard any empty file field
                                for (int i = 0; i < Request.Files.Count; i++)
                                {
                                    HttpPostedFile file = Request.Files[i];
                                    if (file.ContentLength > 0)
                                    {
                                        proper_files.Add(file);
                                    }
                                }

                                bool file_in_each_post = (Request["finp"] == "yes");
                                bool count_files = (Request["countf"] == "yes");

                                bool sage = (Request["email"] == "sage");

                                OPData op_data = new OPData()
                                {
                                    Comment = Request["comment"],
                                    Email = sage ? "" : Request["email"],
                                    Name = Request["name"],
                                    Subject = Request["subject"],
                                    Password = Request["password"],
                                    IP = Request.UserHostAddress,
                                    UserAgent = Request.UserAgent,
                                    Time = DateTime.UtcNow
                                };

                                int reply_id = -1;

                                try
                                {
                                    reply_id = BoardCommon.ReplyTo(op_data, thread_id, proper_files.ToArray(), file_in_each_post, count_files, con);
                                    if (reply_id > 0)
                                    {
                                        //Update thread body
                                        if (ApplicationSettings.CacheIndexView)
                                        {
                                            IndexView.UpdateThreadIndex(thread_id, con);
                                        }
                                        if (ApplicationSettings.CacheThreadView)
                                        {
                                            ThreadView.UpdateThreadBody(thread_id, con);
                                        }
                                        if (!sage)
                                        {
                                            BoardCommon.BumpThread(thread_id, con);
                                        }
                                        Response.Redirect(Paths.WebRoot + string.Format("default.aspx?id={0}#p{1}", thread_id, reply_id));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Response.Write(ex.Message);
                                }

                            }


                            break;
                        default:
                            Response.Write(string.Format("Invalid posting mode '{0}'", Request["mode"]));
                            break;
                    } //mode switch block
                } // if all ok block
            } // database connection using block

        }//page load void
    }
}