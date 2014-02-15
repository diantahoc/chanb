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

            bool bad_password = Request["badpass"] == "1";

            if (bad_password)
            {
                deletefile_page.Replace("{notice:badpassword}", string.Format("<span class=\"notice\">{0}</span>", Language.Lang.badpassword));
            }
            else 
            {
                deletefile_page.Replace("{notice:badpassword}", "");
            }

            deletefile_page.Replace("{captcha}", "");

            deletefile_page.Replace("{ID}", post.PostID.ToString())
                .Replace("{lang:password}", Language.Lang.password)
                .Replace("{lang:deletefile}", Language.Lang.deletefiles);

            StringBuilder files = new StringBuilder();
            foreach (WPostFile file in post.Files)
            {
                files.AppendFormat("<il><input id='{0}' type='checkbox' name='{0}' value='file{0}' /><label for='{0}'><img class='icon' src='{1}'/><span>{2}</span></label></il><br/>", file.Hash, file.ImageThumbnailWebPath, file.RealName);
            }

            deletefile_page.Replace("{Files}", files.ToString());

            dialog_page.Replace("{DialogBody}", deletefile_page.ToString());
            return dialog_page.ToString();
        }

    }
}