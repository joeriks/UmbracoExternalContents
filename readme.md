##This is a work in progress / test approach to use Umbraco as a content editor for a plain MVC 3 application.

Problem : having MVC 3 apps that needs nice content editing - but without all the fuzz of a full Umbraco CMS.

Solution : use an Umbraco site externally and just retrieve the content from the MVC 3 app.

We like the content editing in Umbraco, and wanted to use that for an existing MVC 3 application. However we wanted the CMS to stay out of the way as much as possible, and in particular we wanted to handle routing and rendering completely ourselves (or rather the plain MVC3 way) - so the standard Umbraco installation was not something we wanted to have.

I know there are other ways to solve this, but this is simple and does seems to work ok (still early though) and I wanted to publish it if someone else is interested + also as part of the disussion of how we would like to see Umbraco in the future (I'd vote for an option to be able to disable Umbraco front-end routing and rendering).

##Two parts
The solution consists of two simple parts

1. The json content api on a standard Umbraco site. Simply add the file content.cs https://github.com/joeriks/UmbracoExternalContents/blob/master/Umbraco-Site/App_Code/content.cs to App_Code. It will make all content nodes available on url GETs (/base/content/node/{id} and /base/content/nodeByUrl/?url=someurl

2. The MVC3 UmbracoContent.cs helper and an abstract controller. Those takes care of getting the content from the Umbraco site and renders the content on the MVC3 site. The controller in combination with a route can handle structured content, so you can have an Umbraco structure with root and descendants on UmbracoContentSite.com/somepath and get the same structure ported to the MVC3 site MySite.com/Information/somepath

##Simplest usage:

(Sample at https://github.com/joeriks/UmbracoExternalContents/tree/master/MVC3%20With%20External%20Umbraco%20Contents)

Get the data from within the controller like this:

    // Get root node from mysite.com
    _externalUmbracoContent = new UmbracoContent("http://UmbracoContentSite.com/", allowCache: useCache);

    // Add node to ViewBag (for simplicity)
    ViewBag.ExternalContent = _externalUmbracoContent.GetNode("/");
    // Or
    // ViewBag.ExternalContent = _externalUmbracoContent.GetNode(1100);
    

Use the data in your view like this:

    <h2>ViewBag.ExternalContent.Name</h2>
    @Html.Raw(ViewBag.ExternalContent.Properties.BodyText)
    
    
###Usage to keep tree structure:

Add a controller that inherits from the abstract controller - specify remote site and paths:

    public class InfoController : UmbracoExternalContentController
    {
        public InfoController() : base(umbracoUrl:"http://UmbracoContentSite.com", umbracoRootPath:"/", localRootPath:"/info", allowCache:false)
        {
        }

        // Do not need this, the abstract class has it already 

        //public ActionResult Index()
        //{
        //    return View();
        //}

    }
    
Sample view:

    @Html.Raw(Model.Properties.BodyText)

    <p>Sample links to children</p>
    @foreach(var child in Model.Children)
    {
        <a href="@child.LocalUrl">@child.Name</a><br/>
    }

Add route with "catch all":

    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        //
        // Routes external content pages from Umbraco
        //
        routes.MapRoute(
            "Info", // Route name
            "info/{*url}", // URL with parameters
                new { controller = "Info", action = "Index" } // Parameter defaults
            );
            
##Additional information
You will need to setup timer based "ping" to the outside of your umbraco content site. (A call to /base won't fire up the site if application is down because of no visits.)

Use directory url's.

Setup tinymce to store full image urls ("/media/9999/foo.png" wont work from the other site).

The caching is pretty much untested.

##What does not work
Pretty much anything non-basic-node-content, like:
Macros (could be useful, I will probably add it later on)
Umbraco templates (we don't want them here anyway)
Fancy stuff that uses it's own .ashx
