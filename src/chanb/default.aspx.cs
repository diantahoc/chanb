using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using chanb.Board;

namespace chanb
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            
            sw.Start();

            bool thread_mode = !string.IsNullOrEmpty(this.Request["id"]);

            if (thread_mode) 
            {
                Response.Write(ThreadView.GenerateThreadPage(Convert.ToInt32(this.Request["id"])));
            }
            else
            {
                Response.Write(IndexView.GenerateIndexPage(this.Context, false));
            }
            
            sw.Stop();

            Response.Write(string.Format("<!-- Generated in {0} sec -->", sw.Elapsed.TotalSeconds));
        }
    }
}