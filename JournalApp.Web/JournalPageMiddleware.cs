using Home.Journal.Common;
using Home.Journal.Common.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Home.Journal.Web
{
    public class JournalPageMiddleware
    {
        private readonly RequestDelegate _next;
        private static string _template = null;

        public JournalPageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var url = httpContext.Request.Path.Value.ToLowerInvariant();
            if (url.EndsWith("/"))
                url = url + "index.html";

            if (Path.GetExtension(url).Equals(".html"))
            {
                var template = GetTemplate();

                Page page = null;
                
                if (url.StartsWith("/user/"))
                {
                    httpContext.ChallengeAsync("Cookies").Wait();
                    if(httpContext.User !=null 
                        && httpContext.User.Identity !=null
                        && httpContext.User.Identity.IsAuthenticated)
                    {
                        var username = httpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
                        page = PageDbHelper.GetUserPageByUrl(url, username);
                    }
                }
                else
                {
                    page = PageDbHelper.GetPublicPageByUrl(url);
                }

                if (page == null)
                    return _next(httpContext);

                httpContext.Response.StatusCode = 200;
                httpContext.Response.ContentType = "text/html";

                var content = PageComposer.GetHtml(page, null);

                template = template.Replace("%%TITLE%%", page.Title);
                template = template.Replace("%%CONTENT%%", content);

                using (var wri = new StreamWriter(httpContext.Response.Body))
                    wri.Write(template);

                return Task.CompletedTask;
            }

            return _next(httpContext);
        }

        private static string GetTemplate()
        {
            if (_template != null)
                return _template;

            using (var st = typeof(JournalPageMiddleware).Assembly.GetManifestResourceStream("Home.Journal.Web.content.pagetemplate.html"))
            using(var rdr = new StreamReader(st))
            {
                _template = rdr.ReadToEnd();
            }
            return _template;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class JournalPageMiddlewareExtensions
    {
        public static IApplicationBuilder UseJournalPageMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JournalPageMiddleware>();
        }
    }
}
