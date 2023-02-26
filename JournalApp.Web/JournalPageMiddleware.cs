using Home.Journal.Common;
using Home.Journal.Common.Model;
using Home.Journal.Web.Properties;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Resources;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Home.Journal.Web
{
    public class JournalPageMiddleware
    {
        private readonly RequestDelegate _next;
        private static string _template = null;
        private static string _loginPart = null;

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
                    if (httpContext.User != null
                        && httpContext.User.Identity != null
                        && httpContext.User.Identity.IsAuthenticated)
                    {
                        var username = httpContext.User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
                        page = PageDbHelper.GetUserPageByUrl(url, username);
                    }
                    else
                    {
                        var loginpart = GetLoginPart();
                        httpContext.Response.StatusCode = 301;
                        httpContext.Response.ContentType = "text/html";
                        httpContext.Response.Headers.CacheControl = "no-cache";
                        template = template.Replace("%%TITLE%%", "maNoir JournalApp");
                        template = template.Replace("%%CONTENT%%", loginpart);
                        template = ReplaceResources(template);
                        using (var wri = new StreamWriter(httpContext.Response.Body))
                            wri.Write(template);

                        return Task.CompletedTask;
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

                if (!page.IsPublic)
                    httpContext.Response.Headers.CacheControl = "no-cache";
                else
                    httpContext.Response.Headers.CacheControl = "private; max-age=900";

                var content = PageComposer.GetHtml(page, null);

                template = template.Replace("%%TITLE%%", page.Title);
                template = template.Replace("%%CONTENT%%", content);

                template = ReplaceResources(template);

                using (var wri = new StreamWriter(httpContext.Response.Body))
                    wri.Write(template);

                return Task.CompletedTask;
            }

            return _next(httpContext);
        }

        private static string ReplaceResources(string template)
        {
            while (template.IndexOf("%%RES:") > -1)
            {
                int start = template.IndexOf("%%RES:");
                var replacable = template.Substring(start, template.IndexOf("%%", start + 3) - start + 2);
                var resname = replacable.Substring(6, replacable.Length - 2 - 6);
                var val = GetResourceString(resname);
                template = template.Replace(replacable, val);

            }

            return template;
        }

        private static string GetResourceString(string key)
        {
          return Resources.ResourceManager.GetString(key);  
        }

        private static string GetTemplate()
        {
            if (_template != null)
                return _template;

            // on prévoit des version avec +sieurs langues
            if (ManifestResourceExists("content.pagetemplate.html"))
            {
                using (var st = GetManifestResource("content.pagetemplate.html"))
                using (var rdr = new StreamReader(st))
                {
                    _template = rdr.ReadToEnd();
                }
            }

            return _template;
        }

        private static string GetLoginPart()
        {
            if (_loginPart != null)
                return _loginPart;

            // on prévoit des version avec +sieurs langues
            if (ManifestResourceExists("content.login-part.html"))
            {
                using (var st = GetManifestResource("content.login-part.html"))
                using (var rdr = new StreamReader(st))
                {
                    _loginPart = rdr.ReadToEnd();
                }
            }
            return _loginPart;
        }

        private static Stream GetManifestResource(string fileName)
        {
            return typeof(JournalPageMiddleware).Assembly.GetManifestResourceStream("Home.Journal.Web." + fileName);
        }

        private static bool ManifestResourceExists(string fileName)
        {
            try
            {
                var t = typeof(JournalPageMiddleware).Assembly.GetManifestResourceInfo("Home.Journal.Web." + fileName);
                if (t != null)
                    return true;
            }
            catch (Exception ex)
            {
            }
            return false;
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
