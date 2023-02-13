using Home.Journal.Common.Model;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Journal.Common
{
    public static class PageDbHelper
    {
        public static List<PageSection> GetSectionsForPageByUrl(string pageUri, bool isAuthenticated)
        {
            var pg = GetPageByUrl(pageUri, isAuthenticated);
            if (pg == null)
                return new List<PageSection>();
            return GetSectionsForPageById(pg.Id);
        }
        public static List<PageSection> GetSectionsForPageById(string pageId)
        {
            pageId = pageId.ToLowerInvariant();
            var collection = MongoDbHelper.GetClient<PageSection>();
            var lst = collection.Find(x => x.PageId.Equals(pageId)).SortBy(x => x.Order);
            return lst.ToList();
        }

        public static Page GetPageByUrl(string pageUrl, bool isAuthenticated)
        {
            var collection = MongoDbHelper.GetClient<Page>();
            var lst = collection.Find(x => x.Path.Equals(pageUrl)
                    && (!x.DateDeleted.HasValue)
                    && x.IsPublic == !isAuthenticated)
                .FirstOrDefault();
            return lst;
        }

        public static void TestCreate()
        {
            var pth = Environment.CurrentDirectory;
            pth = Path.Combine(pth, "..", "sample-data");
            // c'est crade, mais c'est fait juste pour les premiers tests
            var pages = Newtonsoft.Json.JsonConvert.DeserializeObject<Page[]>(
                File.ReadAllText(Path.Combine(pth, "pages.json")));
            var cPage = MongoDbHelper.GetClient<Page>();
            foreach (var p in pages)
            {
                var t = cPage.Find(x => x.Id.Equals(p.Id)).FirstOrDefault();
                if (t == null)
                    cPage.InsertOne(p);
                else
                    cPage.ReplaceOne(x => x.Id.Equals(p.Id), p);
            }
            var sections = Newtonsoft.Json.JsonConvert.DeserializeObject<PageSection[]>(
                File.ReadAllText(Path.Combine(pth, "page-sections.json")));
            var cPageSec = MongoDbHelper.GetClient<PageSection>();
            foreach (var s in sections)
            {
                var t = cPageSec.Find(x => x.Id.Equals(s.Id)).FirstOrDefault();
                if (t == null)
                    cPageSec.InsertOne(s);
                else
                    cPageSec.ReplaceOne(x => x.Id.Equals(s.Id), s);
            }
        }

    }
}
