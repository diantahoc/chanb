using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data.Common;

using chanb.Board;
using chanb.DataTypes;

namespace chanb
{
    public partial class report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //ReportReason[] data = ReportHelper.ReportReasons;

            //for (int i = 0; i < data.Length; i++) 
            //{
            //    this.Response.Write(string.Format(" * Report id :{0} <br/> * Report Description: '{1}' <br/> * Report Severity {2}  <br/> <b>-----------</b> <br/>", i, data[i].Description, data[i].Severity.ToString()));  
            //}
            ReportReason[] report_reasons = ReportHelper.ReportReasons;

            bool do_action = !(string.IsNullOrEmpty(this.Request["id"]) || string.IsNullOrEmpty(this.Request["rr"]));

            if (do_action)
            {
                int post_id = -1;

                Int32.TryParse(this.Request["id"], out post_id);

                int report_reason = -1;

                Int32.TryParse(this.Request["rr"], out report_reason);


                if (post_id > 0)
                {
                    if (report_reason >= 0 & (report_reason <= report_reasons.Length - 1))
                    {

                        using (DbConnection con = Database.DatabaseEngine.GetDBConnection())
                        {
                            con.Open();

                            try
                            {
                                if (BoardCommon.IPAlreadyReportedPost(post_id, this.Request.UserHostAddress, con))
                                {
                                    this.Response.Write("Your IP already reported this post");
                                }
                                else
                                {
                                    BoardCommon.InsertReport(report_reason, report_reasons[report_reason], this.Request.UserHostAddress, post_id, con);
                                    this.Response.Write("Report successful");
                                }
                            }
                            catch (Exception)
                            {
                                //  this.Response.Write("Unable to report post");
                                throw;
                            }
                        }
                    }
                    else
                    {
                        this.Response.Write("Invalid report reason");
                        this.Response.End();
                    }
                }
                else
                {
                    this.Response.Write("Invalid ID specified");
                    this.Response.End();
                }
            }
            else
            {
                //show page
                StringBuilder dialog_page = DialogCommon.GetDialogTemplate();

                dialog_page.Replace("{DialogTitle}", Language.Lang.report);

                //--------------

                StringBuilder report_page = new StringBuilder(TemplateProvider.ReportPage);

                report_page.Replace("{lang:report}", Language.Lang.report)
                    .Replace("{lang:reportreason}", Language.Lang.reportreason)
                    .Replace("{ID}", Convert.ToString(this.Request["id"]))
                    .Replace("{notice:wrongcaptcha}", Request["wc"] == "1" ? string.Format("<span class=\"notice\">{0}</span>", Language.Lang.wrongcaptcha) : "")
                    .Replace("{captcha}", DialogCommon.GetCaptcha_ForDialogs());

                StringBuilder reasons = new StringBuilder();

                for (int i = 0; i < report_reasons.Length; i++)
                {
                    reasons.AppendFormat("<option value=\"{0}\">{1}</option>", i, report_reasons[i].Description);
                }

                report_page.Replace("{ReportReasons}", reasons.ToString());

                //--------------

                dialog_page.Replace("{DialogBody}", report_page.ToString());


                this.Response.Write(dialog_page.ToString());
            }

        }
    }
}