using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;

namespace XperiCode.CacheTempData
{
    public class CacheTempDataProvider : ITempDataProvider
    {
        private const string TempDataSessionIdCookieName = "XperiCode.CacheTempData.SessionId";

        private readonly ObjectCache _cache;
        private readonly object _lock;

        public CacheTempDataProvider(ObjectCache cache)
        {
            this._cache = cache;
            this._lock = new object();
        }

        public IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
        {
            var cacheKey = GetTempDataCacheKey(controllerContext.HttpContext);

            var tempData = _cache.Get(cacheKey) as CacheTempDataItem;
            if (tempData != null)
            {
                _cache.Remove(cacheKey);

                if (tempData.Data != null)
                {
                    return tempData.Data;
                }
            }

            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public void SaveTempData(ControllerContext controllerContext, IDictionary<string, object> values)
        {
            var cacheKey = GetTempDataCacheKey(controllerContext.HttpContext);

            if (values != null && values.Any())
            {
                var item = new CacheTempDataItem
                {
                    Data = values
                };

                _cache.Set(cacheKey, item, null);
            }
            else
            {
                _cache.Remove(cacheKey);
            }
        }

        internal string GetTempDataCacheKey(HttpContextBase httpContext)
        {
            return string.Format("[{0}].[{1}]", TempDataSessionIdCookieName, GetTempDataSessionId(httpContext));
        }

        internal string GetTempDataSessionId(HttpContextBase httpContext)
        {
            if (httpContext.Response.Cookies.AllKeys.Contains(TempDataSessionIdCookieName))
            {
                return httpContext.Response.Cookies.Get(TempDataSessionIdCookieName).Value;
            }

            var cookie = httpContext.Request.Cookies.Get(TempDataSessionIdCookieName);
            if (cookie != null)
            {
                return cookie.Value;
            }

            lock (_lock)
            {
                if (httpContext.Response.Cookies.AllKeys.Contains(TempDataSessionIdCookieName))
                {
                    return httpContext.Response.Cookies.Get(TempDataSessionIdCookieName).Value;
                }

                var newCookie = new HttpCookie(TempDataSessionIdCookieName, Guid.NewGuid().ToString())
                {
                    HttpOnly = true,
                    Path = httpContext.Request.ApplicationPath,
                    Secure = IsSecureConnection(httpContext)
                };

                httpContext.Response.Cookies.Add(newCookie);
                return newCookie.Value;
            }
        }
  
        internal bool IsSecureConnection(HttpContextBase httpContext)
        {
            try
            {
                return httpContext.Request.IsSecureConnection;
            }
            catch (Exception)
            {
                return httpContext.Request.Url.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase);
            }

        }
    }
}
