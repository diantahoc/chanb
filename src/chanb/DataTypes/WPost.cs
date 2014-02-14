using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using chanb.Board;
using chanb.Settings;
using chanb.Language;

namespace chanb.DataTypes
{
    public class WPost
    {
        public WPost()
        {
            this.Type = Enums.PostType.Unknown;
        }

        public int PostID { get; set; }

        public string Name { get; set; }
        public string PosterID { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        public string Trip { get; set; }
        public string Comment { get; set; }
        public DateTime Time { get; set; }
        public string Password { get; set; }

        public WPostFile[] Files { get; set; }

        //public bool HasFile { get { return this.Files.Count() > 0; } }

        public int FileCount
        {
            get
            {
                if (this.Files != null)
                {
                    return this.Files.Count();
                }
                else
                {
                    return 0;
                }
            }
        }

        public int Parent { get; set; }

        public bool IsSticky { get; set; }
        public bool IsLocked { get; set; }
        public bool IsArchived { get; set; }
        public ThreadReplies ReplyCount { get; set; }

        public string IP { get; set; }
        public string UserAgent { get; set; }

        public chanb.Enums.PostType Type { get; set; }

        public override string ToString()
        {
            StringBuilder template = null;

            switch (this.Type)
            {
                case Enums.PostType.Thread:
                    template = new StringBuilder(TemplateProvider.OPPost);

                    template.Replace("{op:sticky}", this.IsSticky ? string.Format("<img src='{0}res/sticky.png' title='{1}' />", Paths.WebRoot, Lang.sticky) : "");
                    template.Replace("{op:locked}", this.IsLocked ? string.Format("<img src='{0}res/locked.png' title='{1}' />", Paths.WebRoot, Lang.locked) : "");

                    this.Parent = this.PostID; // HACK
                    break;
                case Enums.PostType.Reply:
                    template = new StringBuilder(TemplateProvider.ReplyPost);

                    template.Replace("{wpost:parent}", this.Parent.ToString());
                    break;
                default:
                    return "";
            }

            //template.Replace("{lang:highlightthispost}", Lang.highlightthispost);

            template.Replace("{lang:quotethispost}", Lang.quotethispost);

            template.Replace("{lang:report}", Lang.report);

            template.Replace("{lang:delete}", Lang.delete);

            template.Replace("{WebRoot}", Paths.WebRoot);

            template.Replace("{wpost:id}", this.PostID.ToString());

            if (this.IsArchived)
            {
                template.Replace("{postLink}", string.Format("{0}archive.aspx?id={1}#p{2}", Paths.WebRoot, this.Parent, this.PostID));
            }
            else
            {
                template.Replace("{postLink}", string.Format("{0}default.aspx?id={1}#p{2}", Paths.WebRoot, this.Parent, this.PostID));
            }

            //Post subject
            if (string.IsNullOrEmpty(this.Subject))
            {
                template.Replace("{wpost:subject}", "");
            }
            else
            {
                template.Replace("{wpost:subject}", string.Format("<span class='subject'>{0}</span>", this.Subject));
            }

            //Poster ID.
            if (string.IsNullOrEmpty(this.PosterID))
            {
                template.Replace("{wpost:posterId}", "");
            }
            else
            {
                template.Replace("{wpost:posterId}", string.Format("(<span class='posterid'>{0}</span>)", this.PosterID));
            }

            //Post tripcode
            if (string.IsNullOrEmpty(this.Trip))
            {
                template.Replace("{wpost:trip}", "");
            }
            else
            {
                template.Replace("{wpost:trip}", string.Format("<span class='trip'>{0}</span>", this.Trip));
            }

            if (string.IsNullOrEmpty(this.Email))
            {
                template.Replace("{wpost:name}", this.Name);
            }
            else
            {
                template.Replace("{wpost:name}", string.Format("<a href=\"mailto:{0}\">{1}</a>", this.Email, this.Name));
            }

            template.Replace("{wpost:time}", System.Xml.XmlConvert.ToString(this.Time, "yyyy-MM-dd HH:mm:ss"));

            if (this.FileCount > 0)
            {
                template.Replace("{wpost:files}", Board.Formatters.FileFormatter.FormatPostFiles(this));
            }
            else
            {
                template.Replace("{wpost:files}", "");
            }

            if (string.IsNullOrEmpty(this.Comment))
            {
                template.Replace("{wpost:comment}", "");
            }
            else { template.Replace("{wpost:comment}", string.Format("<blockquote class=\"postMessage\">{0}</blockquote>", Board.Formatters.CommentFormatter.ProcessComment(this))); }

            return template.ToString();
        }
    }
}