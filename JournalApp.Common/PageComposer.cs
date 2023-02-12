using Home.Journal.Common.Model;
using Markdig.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Journal.Common
{
    public static class PageComposer
    {
        private static Dictionary<string, IPageSectionComposer> _defaults = new Dictionary<string, IPageSectionComposer>()
        {
            { "default.markdown", new Composers.MarkdownContentComposer() }
        };

        private static Dictionary<string, IPageSectionComposer> _all = new Dictionary<string, IPageSectionComposer>();

        static PageComposer()
        {
            _all = _defaults;
        }


        public static string GetHtml(string pageUrl, User currentUser)
        {
            var page = PageDbHelper.GetPageByUrl(pageUrl, false);

            return GetHtml(page, currentUser);
        }

        public static string GetHtml(Page page, User currentUser)
        {
            StringBuilder blr = new StringBuilder();

            var parts = PageDbHelper.GetSectionsForPageById(page.Id);

            foreach (var part in parts)
            {
                string suffix = Guid.NewGuid().ToString("n").ToLowerInvariant();
                IPageSectionComposer compo;
                if (!_all.TryGetValue(part.Kind, out compo))
                {

                }
                else
                {
                    var st = compo.GetHtml(page, part, currentUser, suffix);
                    if (!string.IsNullOrEmpty(st))
                    {
                        blr.Append("<article id='content");
                        blr.Append(suffix);
                        blr.Append("' class='");
                        blr.Append("kind-");
                        blr.Append(SanitizeCssClass(part.Kind));
                        blr.AppendLine("'>");
                        blr.AppendLine(st);
                        blr.AppendLine("</article>");
                    }
                }
            }

            return blr.ToString();
        }

        private static string SanitizeCssClass(string className)
        {
            StringBuilder blr = new StringBuilder();
            foreach(var c in className)
            {
                if (c.IsAlphaNumeric())
                    blr.Append(c);
                else if (c.Equals('.'))
                    blr.Append("-");
            }
            return blr.ToString();
        }
    }
}
