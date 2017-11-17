using CustomRoutes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ark.Controllers
{
    public class ContentController : Controller , ICustomRouteErrorHandler
    {
        [CustomRoute("{*page?}",1)]
        public ActionResult Index(string page)
        {
            if (string.IsNullOrEmpty(page))
                return View("Index");

            return View(page);
        }

        public ActionResult ErrorPage(int errorCode)
        {
            return View("Errors/"+errorCode);
        }

        public void Error(Exception ex)
        {

        }
	}
}
