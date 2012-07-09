using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExternalUmbracoContent;
namespace MVC3_With_External_Umbraco_Contents.Controllers
{
    public class InfoController : UmbracoExternalContentController
    {
        //
        // GET: /Info/

        public InfoController() : base(umbracoUrl:"http://mySite.com", umbracoRootPath:"/", localRootPath:"/info", allowCache:false)
        {
        }

        // Do not need this, the abstract class has it already 

        //public ActionResult Index()
        //{
        //    return View();
        //}

    }
}
