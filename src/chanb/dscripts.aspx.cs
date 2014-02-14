using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace chanb
{
    public partial class dscripts : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Response.Cache.SetExpires(DateTime.Now.AddDays(7));
            this.Response.ContentType = "application/javascript";
            this.Response.ContentEncoding = System.Text.Encoding.UTF8;


            if (Cache["dscript"] != null)
            {
                this.Response.Write(Cache["dscript"].ToString());
            }
            else 
            {
                string data = get_content();
                Cache.Insert("dscript", data);
                this.Response.Write(data);
            }

        }

        private string get_content() 
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Web.Configuration.HttpRuntimeSection htc = new System.Web.Configuration.HttpRuntimeSection();

            sb.AppendFormat("var {0} = '{1}';", "webroot", HttpUtility.JavaScriptStringEncode(Settings.Paths.WebRoot));

            sb.AppendFormat("var {0} = {1};", "maxHttpLength", htc.MaxRequestLength * 1024);

            sb.AppendFormat("var {0} = {1};", "maxFileLength", Settings.ApplicationSettings.MaximumFileSize);
          
            return sb.ToString();
        }
    }
}