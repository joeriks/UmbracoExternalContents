##This is a work in progress / test approach to get editable content for a vanilla MVC 3 application.

As I like the content editing in Umbraco very much I wanted to use that for my application. However I wanted the CMS to stay out of the way as much as possible, and in particular I wanted to handle routing and rendering myself - so I did not want a standard Umbraco installation on top of my application.

I'm sure there are other ways to solve this, but this is working Ok for me and I wanted to publish it if someone else is interested, and also as a part of the discussion "how we like to see Umbraco in the future". I would like to have a setting "disable Umbraco front end routing and rendering" or something like that.

##Two parts
The solution consists of two parts

1. The json content api on a standard Umbraco site. Simply a .cs to add to App_Code, which makes all content nodes available on a simple urls (/base/content/node/{id} and /base/content/nodeByUrl/?url=someurl

2. The MVC3 helper and abstract controller. Those takes care of getting the content from the Umbraco site and renders the content on the MVC3 site. They can handle tree structured content . So you can have an Umbraco structure with root and descendants and get the same structure ported to the MVC3 site.

##Simplest usage:

Setup a controller to get the data like this:

    // Get root node from mysite.com
    _externalUmbracoContent = new UmbracoContent("http://mySite.com/", allowCache: useCache);

    // Add node to ViewBag (for simplicity)
    ViewBag.ExternalContent = _externalUmbracoContent.GetNode("/");

Use the data in your view like this:

    <h2>ViewBag.ExternalContent.Name</h2>
    @Html.Raw(ViewBag.ExternalContent.Properties.BodyText)
    
    
Usage to keep tree structure:

Add a controller that inherits from the abstract controller - specify remote site and paths:

    public class InfoController : UmbracoExternalContentController
    {
        public InfoController() : base(umbracoUrl:"http://mySite.com", umbracoRootPath:"/", localRootPath:"/info", allowCache:false)
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
            