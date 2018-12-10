//using SimpleDMS.Server.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimpleDMS.Client.Models
{
    public static class SimpleServerExtensionMethods
    {
        public static string GetFilePath(string objid, string ext = "pdf", string scope = "basis")
        {
            return Path.Combine(SimpleServerExtensionMethods.GetArchivePath("DMSArchiv"), scope, GetHexFolder(objid), GetHexValue(objid) + "." + ext);
        }

        public static string GetHexValue(string objid)
        {
            int id = int.Parse(objid);
            return id.ToString("X").PadLeft(8, '0');
        }

        public static string GetHexFolder(string objid)
        {
            return "UPR" + GetHexValue(objid).Substring(0, 6);
        }
        public static string GetIntrayPath(string archive)
        {
            return Path.Combine(@"D:\SimpleDMS\intray", archive);
        }

        public static string GetDataPath(string archive)
        {
            return @"D:\SimpleDMS\data";
        }

        public static string GetArchivePath(string archive)
        {
            return Path.Combine(@"D:\SimpleDMS\archive", archive);
        }

        public static string GetRootPath()
        {
            string rootPath = Assembly.GetExecutingAssembly().Location;
            rootPath = rootPath.LeftOfRightmostOf("\\").LeftOfRightmostOf("\\").LeftOfRightmostOf("\\");

            return rootPath;
        }

        public static string GetWebsitePath()
        {
            string websitePath = Assembly.GetExecutingAssembly().Location;
            websitePath = websitePath.LeftOfRightmostOf("\\").LeftOfRightmostOf("\\").LeftOfRightmostOf("\\") + "\\Views";

            return websitePath;
        }

        public static T ParseURI<T>(Dictionary<string, object> parms, string key, string type = null)
        {
            string s = HttpUtility.UrlDecode(parms[key].ToString());
            return ConvertTo<T>(s);
        }

        public static T ConvertTo<T>(object value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static string LeftOfRightmostOf(this String src, string s)
        {
            string ret = src;
            int idx = src.IndexOf(s);
            int idx2 = idx;

            while (idx2 != -1)
            {
                idx2 = src.IndexOf(s, idx + s.Length);

                if (idx2 != -1)
                {
                    idx = idx2;
                }
            }

            if (idx != -1)
            {
                ret = src.Substring(0, idx);
            }

            return ret;
        }

    }
}
