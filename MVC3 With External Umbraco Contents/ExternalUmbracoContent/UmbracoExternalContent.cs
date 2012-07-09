using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Collections;


namespace ExternalUmbracoContent
{
    public class UmbracoContent
    {
        private string _umbracoUrl, _umbracoRootPath, _localRootPath;
        private bool _allowCache;
        public UmbracoContent(string umbracoUrl, string umbracoRootPath="", string localRootPath="", bool allowCache=false)
        {
            _umbracoUrl = umbracoUrl;
            _umbracoRootPath = umbracoRootPath;
            _localRootPath = localRootPath;
            _allowCache = allowCache;
        }

        public T CachedFunc<T>(Func<T> func, string cacheKey = "", bool useCache = true)
        {
            if (useCache)
            {
                var result = (T)WebCache.Get(cacheKey);
                if (result == null)
                {
                    result = func();
                    if (result!=null)
                        WebCache.Set(cacheKey, result);
                }
                return result;
            }
            else
            {
                return func();
            }
        }

        public DynamicJsonObject GetNode(int nodeId, bool getChildNodes = false)
        {
            var key = "GetNode " + nodeId + " " + getChildNodes;
            return CachedFunc(() =>
                              requestToJson(_umbracoUrl + "/base/content/node/" + nodeId, getChildNodes), key,
                              _allowCache);
        }
        public DynamicJsonObject GetNode(string path, bool getChildNodes = false)
        {
            if (path.Length > 0 && !path.StartsWith("/")) path = "/" + path;
            path = _umbracoRootPath + path;
            //if (path.StartsWith("/")) path = path.Substring(1); 
            var key = "GetNode " + path + " " + getChildNodes;
            return CachedFunc(() =>
                              requestToJson(_umbracoUrl + "/base/content/nodeByUrl/?url=" + path.Replace("/", "%2F"), getChildNodes), key,
                              _allowCache);

        }
        private DynamicJsonObject requestToJson(string url, bool getChildNodes)
        {
            var request = WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            string text;
            var response = (HttpWebResponse)request.GetResponse();

            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                text = sr.ReadToEnd();
            }

            var node = Json.Decode(text);
            if (node != null)
            {

                if (getChildNodes)
                {
                    var children = new List<DynamicJsonObject>();
                    foreach (var childId in node.ChildIds)
                    {
                        var child = GetNode(childId, false);
                        children.Add(child);
                    }
                    node.Children = children;
                }
                if (node.ParentId != -1)
                {
                    node.Parent = GetNode(node.ParentId, false);
                }

                if (node.Url.Length >= _umbracoRootPath.Length)
                {
                    var remoteUrlBasedOnRoot = node.Url.Substring(_umbracoRootPath.Length);

                    if (remoteUrlBasedOnRoot.Length > 0 && !remoteUrlBasedOnRoot.StartsWith("/"))
                        remoteUrlBasedOnRoot = "/" + remoteUrlBasedOnRoot;

                    var localUrl = _localRootPath + remoteUrlBasedOnRoot;
                    node.LocalUrl = localUrl;
                }
                return node;
            }
            return null;

        }

    }
}