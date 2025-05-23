using System.Linq;
using System.Reflection;
using Api.TypeScript.Objects;
using Microsoft.AspNetCore.Mvc;

namespace Api.TypeScript
{
    public static class URLBuilder
    {
        public static string BuildUrl(ControllerMethod method)
        {
            string url = ("/" + method.RequestUrl).ToLower();

            var needsQueryMarker = true;

            // all [FromRoute]
            foreach (var param in method.WebSafeParams)
            {
                if (param.GetCustomAttribute<FromRouteAttribute>() is not null)
                {
                    url = url.Replace($"{{{param.Name!.ToLower()}}}", "' + " + param.Name + " +'");
                }
            }

            var count = 0;
            foreach (var param in method.WebSafeParams)
            {
                if (param.GetCustomAttribute<FromQueryAttribute>() is not null)
                {
                    if (needsQueryMarker)
                    {
                        needsQueryMarker = false;
                        url += '?';
                    }

                    if (count != 0)
                    {
                        url += "&";
                    }

                    var name = TypeScriptService.LcFirst(param.Name);

                    url += name + "=" + $"' + {name} + '";

                    count++;
                }
            }

            if (method.RequiresIncludes)
            {
                url +=
                    "' + (Array.isArray(includes) ? '" + (needsQueryMarker ? "?" : "&") + "includes=' + includes.join(',') : '') + '";
            }

            url = url.Replace("+''", "").Replace("//", "/");

            return url;
        }
    }
}