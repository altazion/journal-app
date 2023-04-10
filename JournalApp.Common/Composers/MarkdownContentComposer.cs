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

            string content = section.Data;
            if (section.Properties != null)
            {
                foreach (var k in section.Properties.Keys)
                    content = content.Replace("%%" + k + "%%", section.Properties[k]);
            }

            blr.Append(Markdown.ToHtml(content, _pipeline));
            return blr.ToString();
        }
    }
}
