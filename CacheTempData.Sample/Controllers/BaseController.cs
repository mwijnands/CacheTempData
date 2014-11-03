using System;
using System.Linq;
using System.Runtime.Caching;
using System.Web.Mvc;

namespace XperiCode.CacheTempData.Sample.Controllers
{
    public abstract class BaseController : Controller
    {
        protected override ITempDataProvider CreateTempDataProvider()
        {
            // A better approach would be to register CacheTempDataProvider as ITempDataProvider using an IoC container.
            return new CacheTempDataProvider(MemoryCache.Default);
        }
    }
}
