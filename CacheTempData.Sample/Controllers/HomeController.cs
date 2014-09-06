using System;
using System.Linq;
using System.Web.Mvc;
using XperiCode.CacheTempData.Sample.Models;

namespace XperiCode.CacheTempData.Sample.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            var model = new HomeIndexModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Post(HomeIndexModel model)
        {
            if (!ModelState.IsValid)
	        {
                return View("Index", model);
	        }

            TempData["Text"] = model.Text;

            return RedirectToAction("Index");
        }
    }
}
