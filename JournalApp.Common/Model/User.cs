using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home.Journal.Common.Model
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string HashedPassword { get; set; }
    }
}
