using Home.Journal.Common.Model;
using Markdig;
using System.Text;

namespace Home.Journal.Common.Composers
{
    internal class MarkdownContentComposer : IPageSectionComposer
    {
        private static MarkdownPipeline _pipeline;
        static MarkdownContentComposer()
        {
            _pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        }

        public string[] AdditionalClasses => new string[0];

        public string GetHtml(Page page, PageSection section, User currentUser, string outputTagIdSuffix)
        {
            StringBuilder blr = new StringBuilder();
            blr.Append(Markdown.ToHtml(section.Data, _pipeline));
            string content = blr.ToString();
            if (section.Properties != null)
            {
                foreach (var k in section.Properties.Keys)
                    content = content.Replace("%%" + k + "%%", section.Properties[k]);
            }

            return content;
        }
    }
}
