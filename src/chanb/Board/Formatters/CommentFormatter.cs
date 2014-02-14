using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using chanb.DataTypes;

namespace chanb.Board.Formatters
{
    public static class CommentFormatter
    {
        static Regex quote_matcher = new Regex(@"&gt;&gt;\d+", RegexOptions.Compiled);

        static SimpleBBCode[] bb_codes;

        //public static string BBCodeRegexp { get { return @"\[{0}]((((\r?)\n)+)?(.)+(((\r?)\n)+)?)+\[/{0}]"; } }
        public static string BBCodeRegexp { get { return @"\[{0}].+\[/{0}]"; } }

        static CommentFormatter()
        {
            bb_codes = new SimpleBBCode[] { new SpolierBBCode(), new QuoteBBCode(), new CodeBBCode(), new MarkDownBBCode() };
        }

        public static string ProcessComment(WPost post)
        {
            if (string.IsNullOrEmpty(post.Comment))
            {
                return "";
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (string line in post.Comment.Split('\n'))
                {
                    if (line.StartsWith("&gt;") & !line.StartsWith("&gt;&gt;"))
                    {
                        sb.AppendFormat("<span class=\"quote\">{0}</span>", line);
                    }
                    else
                    {
                        sb.Append(line);
                    }
                    sb.Append("<br/>");
                }

                foreach (Match m in quote_matcher.Matches(post.Comment))
                {
                    sb.Replace(m.Value,
                        string.Format("<a class='backlink' href='{0}{1}.aspx?id={2}#p{3}'>{4}</a>",
                        Settings.Paths.WebRoot,
                        post.IsArchived ? "archive" : "default",
                        post.Parent, m.Value.Replace("&gt;", ""),
                        m.Value));
                }

                for (int i = 0; i < bb_codes.Length; i++)
                {
                    SimpleBBCode bbcode = bb_codes[i];
                    if (post.Comment.Contains("[" + bbcode.TagName + "]"))
                    {
                        MatchCollection cl = bbcode.RegexPattren.Matches(post.Comment);
                        foreach (Match m in cl)
                        {
                            string sb_value = m.Value.Replace("\n", "<br/>");
                            sb.Replace(sb_value, bbcode.Format(m.Value.Replace("[" + bbcode.TagName + "]", "")
                                                            .Replace("[/" + bbcode.TagName + "]", "")));
                        }
                    }
                }

                return sb.ToString();
            }
        }
    }

    interface SimpleBBCode
    {
        Regex RegexPattren { get; }
        string Format(string input);
        string TagName { get; }
    }

    public class SpolierBBCode : SimpleBBCode
    {
        public SpolierBBCode()
        {
            this.RegexPattren = new Regex(string.Format(CommentFormatter.BBCodeRegexp, this.TagName), RegexOptions.Compiled | RegexOptions.Singleline);
        }

        public string TagName { get { return "spoiler"; } }
        public Regex RegexPattren { get; private set; }
        public string ContentFormat { get { return "<s>{0}</s>"; } }

        public string Format(string input)
        {
            return string.Format(this.ContentFormat, input);
        }
    }

    public class MarkDownBBCode : SimpleBBCode
    {
        MarkdownSharp.Markdown w = new MarkdownSharp.Markdown();

        public MarkDownBBCode()
        {
            this.RegexPattren = new Regex(string.Format(CommentFormatter.BBCodeRegexp, this.TagName), RegexOptions.Compiled | RegexOptions.Singleline);
        }

        public string TagName { get { return "md"; } }
        public Regex RegexPattren { get; private set; }
        public string ContentFormat { get { return "<div class='md'>{0}</div>"; } }

        public string Format(string input)
        {
            return string.Format(this.ContentFormat, w.Transform(input));
        }

    }

    public class QuoteBBCode : SimpleBBCode
    {
        public QuoteBBCode()
        {
            this.RegexPattren = new Regex(string.Format(CommentFormatter.BBCodeRegexp, this.TagName), RegexOptions.Compiled | RegexOptions.Singleline);
        }

        public string TagName { get { return "q"; } }
        public Regex RegexPattren { get; private set; }
        public string ContentFormat { get { return "<span class='tt'>{0}</span>"; } }

        public string Format(string input)
        {
            return string.Format(this.ContentFormat, input);
        }
    }

    public class CodeBBCode : SimpleBBCode
    {
        public CodeBBCode()
        {
            this.RegexPattren = new Regex(string.Format(CommentFormatter.BBCodeRegexp, this.TagName), RegexOptions.Compiled | RegexOptions.Singleline);
        }

        public string TagName { get { return "code"; } }
        public Regex RegexPattren { get; private set; }
        public string ContentFormat { get { return "<pre><code>{0}</code></pre>"; } }

        public string Format(string input)
        {
            return string.Format(this.ContentFormat, input);
        }
    }
}