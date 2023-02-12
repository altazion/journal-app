using Home.Journal.Common.Model;
using Markdig;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Journal.Common.Composers
{
    internal class MarkdownContentComposer : IPageSectionComposer
    {
        private static MarkdownPipeline _pipeline;
        static MarkdownContentComposer()
        {
            _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        }
            
        public string GetHtml(Page page, PageSection section, User currentUser, string outputTagIdSuffix)
        {
            StringBuilder blr = new StringBuilder();
            blr.Append(Markdown.ToHtml(section.Data, _pipeline));
            return blr.ToString();
        }
    }
}
