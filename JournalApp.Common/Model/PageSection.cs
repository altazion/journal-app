using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Journal.Common.Model
{
    public class PageSection
    {
        public string Id { get; set; }

        public string PageId { get; set; }

        public int Order { get; set; }

        public string Kind { get; set; }

        public string Source { get; set; }

        public string Data { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

    }
}
