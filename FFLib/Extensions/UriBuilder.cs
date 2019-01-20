using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib.Extensions
{
    public static class UriBuilderExtension
    {
        public static void SetQueryParam(this UriBuilder uriBuilder, string key, string value)
        {
            var qparams = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
            qparams.Set(key, value);
            var kvpairs = new List<string>(100);
            foreach (var k in qparams.AllKeys)
                kvpairs.Add(k + "=" + System.Web.HttpUtility.UrlEncode(qparams[k]));
            uriBuilder.Query = string.Join("&", kvpairs);
        }
    }
}
