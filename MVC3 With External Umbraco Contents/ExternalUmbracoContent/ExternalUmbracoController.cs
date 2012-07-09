using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExternalUmbracoContent
{
    public abstract class UmbracoExternalContentController : Controller
    {
        private readonly UmbracoContent _umbracoContent;

        public UmbracoExternalContentController(string umbracoUrl, string umbracoRootPath, string localRootPath, bool allowCache)
        {
            _umbracoContent = new UmbracoContent(umbracoUrl, umbracoRootPath, localRootPath, allowCache);
        }
        public virtual ActionResult Index(string url)
        {
            return View(GetNode(url, true));
        }
        public object GetNode(int nodeId, bool populateChildNodes = false)
        {
            return _umbracoContent.GetNode(nodeId, populateChildNodes);
        }
        public object GetNode(string url, bool populateChildNodes = false)
        {
            return _umbracoContent.GetNode(url ?? "", populateChildNodes);
        }

    }

}