using System.IO;

namespace System.Web.Mvc
{
    public static partial class Extention
    {
        public static string Scrpit(this UrlHelper helper, string scriptVirtualPath)
        {
            if(scriptVirtualPath== "~/Scripts/util/util.js")
            {
                string tmp = string.Empty;
            }
            string filePath = helper.RequestContext.HttpContext.Server.MapPath(scriptVirtualPath);
            FileInfo fileInfo = new FileInfo(filePath);
            var lastTime = fileInfo.LastWriteTime.GetHashCode();
            return helper.Content($"{scriptVirtualPath}?_v={lastTime}");
        }
    }
}