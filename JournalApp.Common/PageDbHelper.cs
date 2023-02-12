using Home.Journal.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Journal.Common
{
    public static class PageDbHelper
    {
        public static List<PageSection> GetSectionsForPageByUrl(string pageUri)
        {
            return GetSectionsForPageById("1");
        }
        public static List<PageSection> GetSectionsForPageById(string pageId)
        {
            List<PageSection> ret = new List<PageSection>();

            ret.Add(new PageSection()
            {
                Id = Guid.NewGuid().ToString(),
                Kind = "default.markdown",
                Order = 0,
                PageId = pageId,
                Source = null,
                Data = "# Ceci est un test\r\n## Avec un sous titre\r\n\r\nEt puis du texte"
            });

            return ret;
        }

        public static Page GetPageByUrl(string pageUrl)
        {
            return new Page()
            {
                Id = "1",
                Title = "MyPage",
            };
        }
    }
}
