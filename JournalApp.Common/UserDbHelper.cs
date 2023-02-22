using Home.Journal.Common.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Journal.Common
{
    public static class UserDbHelper
    {
        public static User GetUserFromPin(string username, string encodedPinCode)
        {
            var collection = MongoDbHelper.GetClient<User>();
            var lst = collection.Find(x => x.Id.Equals(username)
                        && x.HashedPincode.Equals(encodedPinCode))
                .FirstOrDefault();
            return lst;
        }

    }
}
