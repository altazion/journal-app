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
                    && !x.DateDeleted.HasValue
                    /*&& (isAuthenticated ? !x.UserIds.Contains("public") : x.UserIds.Contains("public"))*/)
                .FirstOrDefault();
            return lst;
        }

        public static void TestCreate()
        {
            var pages = Newtonsoft.Json.JsonConvert.DeserializeObject<Page[]>(
                File.ReadAllText(@"c:\temp\pages.json"));
            var cPage = MongoDbHelper.GetClient<Page>();
            cPage.InsertMany(pages);

            var sections = Newtonsoft.Json.JsonConvert.DeserializeObject<PageSection[]>(
                File.ReadAllText(@"c:\temp\page-sections.json"));
            var cPageSec = MongoDbHelper.GetClient<PageSection>();
            cPageSec.InsertMany(sections);
        }

    }
}
