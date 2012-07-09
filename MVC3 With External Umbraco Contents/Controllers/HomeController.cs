using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExternalUmbracoContent;

namespace MVC3_With_External_Umbraco_Contents.Controllers
{
    public class HomeController : Controller
    {
        private UmbracoContent _externalUmbracoContent;
        public HomeController()
        {
#if DEBUG
            const bool useCache = false;
#else
            const bool useCache = true;
#endif

            // Get root node from mysite.com
            _externalUmbracoContent = new UmbracoContent("http://mySite.com/", allowCache: useCache);

            // Add node to ViewBag (for simplicity)
            ViewBag.ExternalContent = _externalUmbracoContent.GetNode("/");

            // Use it in the view as:
            // <h1>@ViewBag.ExternalContent.Name</h1>
            // @Html.Raw(ViewBag.ExternalContent.Properties.BodyText)

            // All custom properties for the node are under (Properties. )

        }
        public ActionResult Index()
        {
            return View();
        }

    }
}
