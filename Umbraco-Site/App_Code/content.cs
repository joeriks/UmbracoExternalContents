using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.presentation.umbracobase;
using System.Web.Helpers;

[RestExtension("content")]
public class ContentClass
{
    [RestExtensionMethod(returnXml = false)]
    public static string node(int nodeId)
    {
        var node = new umbraco.NodeFactory.Node(nodeId);

        // I think getting template name requires a database hit
        /*var template = new umbraco.cms.businesslogic.template.Template(node.template); */
        
        return Json.Encode(
        new
        {
            node.Name,
            node.UrlName,
            node.NodeTypeAlias,
            node.CreatorName,
            node.template,
            /*Template = template.Alias,*/
            Properties = node.PropertiesAsList.Select(p => new { p.Alias, p.Value }).ToDictionary(k => k.Alias, k => k.Value),
            node.CreateDate,
            node.UpdateDate,
            node.SortOrder,
            node.Url,
            ParentId = (node.Parent != null) ? node.Parent.Id : -1,
            ChildIds = node.ChildrenAsList.Select(n => n.Id)
        });
    }

    [RestExtensionMethod(returnXml = false)]
    public static string nodeByUrl()
    {
        var url = System.Web.HttpContext.Current.Request["url"] ?? "";
        try
        {
            var findNode = umbraco.content.Instance.XmlContent.SelectSingleNode(umbraco.requestHandler.CreateXPathQuery(url, true));
            if (findNode != null) return node(Convert.ToInt32(findNode.Attributes["id"].Value));
        }
        catch (Exception)
        {
        }
        return "";
    }
}