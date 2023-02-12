using Home.Journal.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
                httpContext.Response.StatusCode = 200;
                httpContext.Response.ContentType = "text/html";

                var template = GetTemplate();
                var page = PageDbHelper.GetPageByUrl(url, false);
                if(page==null)
                    return _next(httpContext);

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
