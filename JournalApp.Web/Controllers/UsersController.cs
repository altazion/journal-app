using Home.Journal.Common;
using Home.Journal.Common.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
                    AllowRefresh= true,
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

    }
}
