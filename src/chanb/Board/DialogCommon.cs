using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using chanb.Settings;

namespace chanb.Board
{
    public static class DialogCommon
    {
        public static StringBuilder GetDialogTemplate()
        {
            StringBuilder sb = new StringBuilder(TemplateProvider.DialogsTemplate);

            sb.Replace("{BoardTitle}", ApplicationSettings.BoardTitle);
            sb.Replace("{WebRoot}", Paths.WebRoot);
            sb.Replace("{FooterText}", ApplicationSettings.FooterText);

            return sb;
        }
    }
}