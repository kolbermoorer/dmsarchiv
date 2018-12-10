
using DocumentFormat.OpenXml.Packaging;
using Newtonsoft.Json;
using OpenXmlPowerTools;
using SimpleDMS.Client.Controllers;
using SimpleDMS.Client.Models;
using SimpleDMS.Database;
using SimpleDMS.LuceneSearch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace SimpleDMS.Client
{
    public class ServerConnection
    {
        /*public static ResponsePacket ConvertDocToHtml(Session session, Dictionary<string, object> parms)
        {
            byte[] byteArray = File.ReadAllBytes(@"D:\SimpleDMS\intray\DMSArchiv\ShowWord.docx");
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(byteArray, 0, byteArray.Length);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(memoryStream, true))
                {
                    HtmlConverterSettings settings = new HtmlConverterSettings()
                    {
                        PageTitle = "My Page Title"
                    };
                    XElement html = HtmlConverter.ConvertToHtml(doc, settings);

                    File.WriteAllText(@"D:\SimpleDMS\intray\DMSArchiv\ShowWord.html", html.ToStringNewLineOnAttributes());
                }
            }

            string json = JsonConvert.SerializeObject(true);

            return new ResponsePacket() { Data = Encoding.UTF8.GetBytes(json), ContentType = "json" };
        }*/


        /*public static ResponsePacket DownloadWord(Session session, Dictionary<string, object> parms)
        {
            var file = Path.Combine(SimpleServerExtensionMethods.GetRootPath(), "Content", "docs", "test.docx");

            var byteArrayData = File.ReadAllBytes(file);

            return new ResponsePacket() { Data = byteArrayData, ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
        }*/

        //public string ErrorHandler(Engine.ServerError error)
        //{
        //    string ret = null;

        //    switch (error)
        //    {
        //        case Engine.ServerError.ExpiredSession:
        //            ret = "/error/expiredSession.html";
        //            break;
        //        case Engine.ServerError.FileNotFound:
        //            ret = "/error/fileNotFound.html";
        //            break;
        //        case Engine.ServerError.NotAuthorized:
        //            ret = "/error/notAuthorized.html";
        //            break;
        //        case Engine.ServerError.PageNotFound:
        //            ret = "/error/pageNotFound.html";
        //            break;
        //        case Engine.ServerError.ServerError:
        //            ret = "/error/serverError.html";
        //            break;
        //        case Engine.ServerError.UnknownType:
        //            ret = "/error/unknownType.html";
        //            break;
        //        case Engine.ServerError.ValidationError:
        //            ret = "/error/validationError.html";
        //            break;
        //    }

        //    return ret;
        //}
    }
}
