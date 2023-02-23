using Home.Journal.Common;
using Home.Journal.Common.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Home.Journal.Web.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [Route("login")]
        public User Login(string username, string pincode)
        {
            if (pincode != null)
            {
                pincode += username;

                byte[] hashedBytes;

                var blr = new StringBuilder();

                using (SHA256 hash = SHA256Managed.Create())
                {
                    Encoding enc = Encoding.UTF8;
                    hashedBytes = hash.ComputeHash(enc.GetBytes(pincode));
                }

                foreach (var b in hashedBytes)
                    blr.Append(b.ToString("x2"));
                pincode = blr.ToString();
            }

            var user = UserDbHelper.GetUserFromPin(username, pincode);

            if (user != null)
            {
                var claims = new List<Claim>()
                 {
                     new Claim(ClaimTypes.NameIdentifier, user.Id),
                     new Claim(ClaimTypes.Name, user.Name + " " + user.FirstName),
                     //new Claim(ClaimTypes.Email, user.MainEmail),
                 };

                var authProperties = new AuthenticationProperties()
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.Now.AddHours(1)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties).Wait();
            }

            return user;
        }

        public class AllPages
        {
            public List<PageHierarchy> PublicPages { get; set; } = new List<PageHierarchy>();
            public List<PageHierarchy> UserPages { get; set; } = new List<PageHierarchy>();
        }

        public class PageHierarchy
        {
            public PageHierarchy(Page page)
            {
                this.Page = page;
            }

            public Page Page { get; set; }

            public List<PageHierarchy> SubPages { get; set; } = new List<PageHierarchy>();
        }



        [Route("pages")]
        public AllPages GetAllPages()
        {
            this.HttpContext.ChallengeAsync("Cookies").Wait();

            var ret = new AllPages();

            var allPublic = PageDbHelper.GetAllPublicPages();
            var rootPages = (from z in allPublic
                             where string.IsNullOrEmpty(z.ParentId)
                             select z).ToList();
            MergePages(ret.PublicPages, allPublic, rootPages);

            if (this.HttpContext.User != null
                && this.HttpContext.User.Identity != null
                && this.HttpContext.User.Identity.IsAuthenticated)
            {
                var username = this.HttpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
                rootPages = PageDbHelper.GetAllUserPages(username);
                MergePages(ret.UserPages, allPublic, rootPages);
            }



            return ret;
        }

        private void MergePages(List<PageHierarchy> ret, List<Page> allPages, List<Page> levelPages)
        {
            foreach (var r in levelPages)
            {
                // TODO : s'assurer qu'il n'y a pas de boucles dans les pages / sous pages
                var toadd = new PageHierarchy(r);

                var subPages = (from z in allPages
                                where !string.IsNullOrEmpty(z.ParentId)
                                && z.ParentId.Equals(r.Id)
                                select z).ToList();
                MergePages(toadd.SubPages, allPages, subPages);
                ret.Add(toadd);
            }
        }
    }
}
