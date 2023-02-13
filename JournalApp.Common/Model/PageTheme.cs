using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Journal.Common.Model
{
    public class PageTheme
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public bool IsPublic { get; set; }

        public string UserId { get; set; }

        public string BackgroundColor { get; set; }
        public string MainTextColor { get; set; }

        public string AccentColor { get; set; }

        public string ImageUrl { get; set; }

        public List<string> AdditionalClasses { get; set; }
    }
}
