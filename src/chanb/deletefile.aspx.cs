using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.Common;
using System.Text;
using chanb.DataTypes;
using chanb.Board;
namespace chanb
{
    public partial class deletefile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            bool do_action = (!string.IsNullOrEmpty(Request["id"]) & Request["mode"] == "deletefile");

            if (do_action)
            {
                int id = -1;
                Int32.TryParse(Request["id"], out id);

                if (id <= 0)
                {
                    Response.Write("Invalid post id.");
                    Response.End();
                }

                using (DbConnection dc = Database.DatabaseEngine.GetDBConnection())
                {
                    dc.Open();

                    WPost post = Board.BoardCommon.GetPostData(id, dc);

                    if (post == null)
                    {
                        Response.Write("Post does not exist");
                        Response.End();
                    }
                    else
                    {
                        //first check captcha, then check password, and finally delete files

                        if (CaptchaProvider.Verifiy(this.Context))
                        {
                            if (Request["pwd"] == post.Password) //pwd is the user input password
                            {
                                //We should gather a list of files hashes, and delete them
                                List<string> file_hashes = new List<string>();

                                foreach (string qs in this.Request.Form)
                                {
                                    if (qs.StartsWith("file"))
                                    {
                                        file_hashes.Add(qs.Remove(0, 4));
                                    }
                                }

                                if (file_hashes.Count > 0)
                                {
                                    BoardCommon.DeleteFileFromDatabase(id, file_hashes.ToArray(), dc);


                                    if (Settings.ApplicationSettings.AutoDeleteFiles)
                                    {
                                        foreach (WPostFile file in post.Files)
                                        {
                                            if (file_hashes.Contains(file.Hash))
                                            {
                                                //remove the files physically from the disk
                                                System.IO.File.Delete(System.IO.Path.Combine(Settings.Paths.PhysicalFilesStorageFolder, file.ChanbName + "." + file.Extension));

                                                //delete thumbs as well
                                                System.IO.File.Delete(System.IO.Path.Combine(Settings.Paths.PhysicalThumbStorageFolder, file.ChanbName + ".jpg"));
                                                System.IO.File.Delete(System.IO.Path.Combine(Settings.Paths.PhysicalThumbStorageFolder, file.ChanbName + ".png"));
                                            }
                                        }
                                    }


                                    //update thread page and index.
                                    IndexView.UpdateThreadIndex(id, dc);
                                    ThreadView.UpdateThreadBody(id, dc);
                                    Response.Write(file_hashes.Count + " files deleted successfully");
                                }
                                else
                                {
                                    //No file was selected.  Redirect to the delete file page, with 'no file selected' notice.
                                    Response.Redirect(Settings.Paths.WebRoot + "deletefile.aspx?ns=1&id=" + id.ToString(), true); //ns == no file seleted
                                }


                            }
                            else
                            {
                                //Bad password. Redirect to the delete file page, with 'bad password' notice.
                                Response.Redirect(Settings.Paths.WebRoot + "deletefile.aspx?bp=1&id=" + id.ToString(), true); //bp == bad password
                            }
                        }
                        else
                        {
                            //invalid captcha. Redirect to the delete file page, with 'bad captcha' notice
                            Response.Redirect(Settings.Paths.WebRoot + "deletefile.aspx?wc=1&id=" + id.ToString(), true); //wc == wrong captcha
                        }
                    }
                }

            }
            else
            {
                int id = -1;
                Int32.TryParse(Request["id"], out id);

                if (id <= 0)
                {
                    Response.Write("Invalid post id.");
                    Response.End();
                }

                using (DbConnection dc = Database.DatabaseEngine.GetDBConnection())
                {
                    dc.Open();

                    WPost post = Board.BoardCommon.GetPostData(id, dc);

                    if (post == null)
                    {
                        Response.Write("Post does not exist");
                        Response.End();
                    }
                    else
                    {
                        if (post.FileCount == 0)
                        {
                            Response.Write("Post has no files");
                            Response.End();
                        }
                        else if (post.FileCount == 1)
                        {
                            if (string.IsNullOrEmpty(post.Comment) & post.Type == Enums.PostType.Reply)
                            {
                                Response.Write("Cannot delete this post because it has no comment and only a single file. \n Delete the post instead.");
                                Response.End();
                            }
                            else
                            {
                                //show delete file page
                                Response.Write(generate_page(post));
                            }
                        }
                        else
                        {
                            Response.Write(generate_page(post));
                        }
                    }
                }
            }

        }

        private string generate_page(WPost post)
        {
            StringBuilder dialog_page = DialogCommon.GetDialogTemplate();

            dialog_page.Replace("{DialogTitle}", Language.Lang.deletefiles);

            StringBuilder deletefile_page = new StringBuilder(TemplateProvider.DeletePostFilePage);

            //Bad password notice
            deletefile_page.Replace("{notice:badpassword}", Request["bp"] == "1" ? string.Format("<span class=\"notice\">{0}</span>", Language.Lang.badpassword) : "");

            //No file selected notice
            deletefile_page.Replace("{notice:nofileselected}", Request["ns"] == "1" ? string.Format("<span class=\"notice\">{0}</span>", Language.Lang.nofileselected) : "");

            //Captcha HTML container and notice

            deletefile_page.Replace("{captcha}", DialogCommon.GetCaptcha_ForDialogs())
                           .Replace("{notice:wrongcaptcha}", Request["wc"] == "1" ? string.Format("<span class=\"notice\">{0}</span>", Language.Lang.wrongcaptcha) : "");

            deletefile_page.Replace("{ID}", post.PostID.ToString())
                .Replace("{lang:password}", Language.Lang.password)
                .Replace("{lang:deletefile}", Language.Lang.deletefiles);

            //Files elements

            StringBuilder files = new StringBuilder();
            foreach (WPostFile file in post.Files)
            {
                files.AppendFormat("<il><input id='{0}' type='checkbox' name='file{0}' value='file' /><label for='{0}'><img class='icon' src='{1}'/><span>{2}</span></label></il><br/>", file.Hash, file.ImageThumbnailWebPath, file.RealName);
            }

            deletefile_page.Replace("{Files}", files.ToString());

            dialog_page.Replace("{DialogBody}", deletefile_page.ToString());
            return dialog_page.ToString();
        }

    }
}