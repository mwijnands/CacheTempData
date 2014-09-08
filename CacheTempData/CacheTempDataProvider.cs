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

        private readonly HttpContextBase _httpContext;
        private readonly ObjectCache _cache;
        private readonly object _lock;

        public CacheTempDataProvider(HttpContextBase httpContext, ObjectCache cache)
        {
            this._httpContext = httpContext;
            this._cache = cache;
            this._lock = new object();
        }

        public IDictionary<string, object> LoadTempData(ControllerContext controllerContext)
        {
            var cacheKey = GetTempDataCacheKey();

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
            if (values != null && values.Any())
            {
                var item = new CacheTempDataItem
                {
                    Data = values
                };

                var cacheKey = GetTempDataCacheKey();

                _cache.Set(cacheKey, item, null);
            }
        }
  
        private string GetTempDataCacheKey()
        {
            return string.Format("[{0}].[{1}]", TempDataSessionIdCookieName, GetTempDataSessionId());
        }

        private string GetTempDataSessionId()
        {
            if (_httpContext.Response.Cookies.AllKeys.Contains(TempDataSessionIdCookieName))
            {
                return _httpContext.Response.Cookies.Get(TempDataSessionIdCookieName).Value;
            }

            var cookie = _httpContext.Request.Cookies.Get(TempDataSessionIdCookieName);
            if (cookie != null)
            {
                return cookie.Value;
            }

            lock (_lock)
            {
                if (_httpContext.Response.Cookies.AllKeys.Contains(TempDataSessionIdCookieName))
                {
                    return _httpContext.Response.Cookies.Get(TempDataSessionIdCookieName).Value;
                }

                var newCookie = new HttpCookie(TempDataSessionIdCookieName, Guid.NewGuid().ToString())
                {
                    HttpOnly = true,
                    Path = _httpContext.Request.ApplicationPath,
                    Secure = IsSecureConnection()
                };

                _httpContext.Response.Cookies.Add(newCookie);
                return newCookie.Value;
            }
        }
  
        private bool IsSecureConnection()
        {
            try
            {
                return _httpContext.Request.IsSecureConnection;
            }
            catch (Exception)
            {
                return _httpContext.Request.Url.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase);
            }

        }
    }
}
