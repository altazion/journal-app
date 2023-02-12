using Home.Journal.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Journal.Common
{
    public interface IPageSectionComposer
    {
        string GetHtml(Page page, PageSection section, User currentUser, string outputTagIdSuffix);
    }
}
