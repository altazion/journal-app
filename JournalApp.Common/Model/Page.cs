using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Journal.Common.Model
{
    public class Page
    {
        public string Id { get; set; }

        public string Path { get; set; }
        public string ParentId { get; set; }

        public List<string> UserIds { get; set; } = new List<string>();

        public DateTimeOffset? DateDeleted { get; set; }

        public string Title { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTimeOffset? LastModifiedDate { get; set; } = DateTimeOffset.Now;
    }
}
