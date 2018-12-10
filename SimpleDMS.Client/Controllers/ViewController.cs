using Nancy;
using SimpleDMS.Client.Models;
using System.IO;
using System.Web;

namespace SimpleDMS.Client.Controllers
{
    public class View : Nancy.NancyModule
    {
        public View()
        {
            Get["/View/GetConfig"] = _ =>
            {
                var config = DMS.Session.GetConfig();

                return config;
            };

            Get["/View/LoadDocument"] = _ =>
            {
                string path = Request.Query["path"];
                string objid = Request.Query["id"];
                int type = int.Parse(Request.Query["type"]);

                string hexValue = SimpleServerExtensionMethods.GetHexValue(objid);
                string folder = SimpleServerExtensionMethods.GetHexFolder(objid);

                string file = string.Empty;

                if(path != null && path != string.Empty)
                {
                    file = path;
                }
                else if (type == 258) //pdf
                { 
                    file = SimpleServerExtensionMethods.GetFilePath(objid);
                }
                else if (type == 255)
                {
                    string preview = SimpleServerExtensionMethods.GetFilePath(objid, scope: "preview");

                    if (File.Exists(preview))
                        file = preview;
                    else
                        file = new Office("DMSArchiv", SimpleServerExtensionMethods.GetFilePath(objid, "docx"), SimpleServerExtensionMethods.GetHexValue(objid),
                                    SimpleServerExtensionMethods.GetHexFolder(objid)).CreatePDF();
                }


                byte[] byteArrayData = objid == "9999" ? new byte[] { } : File.ReadAllBytes(file);

                string mimeType = MimeMapping.GetMimeMapping(file);

                return Response.FromByteArray(byteArrayData, mimeType);
            };
        }
    }

    public static class Extensions
    {
        public static Response FromByteArray(this IResponseFormatter formatter, byte[] body, string contentType = null)
        {
            return new ByteArrayResponse(body, contentType);
        }
    }

    public class ByteArrayResponse : Response
    {
        /// <summary>
        /// Byte array response
        /// </summary>
        /// <param name="body">Byte array to be the body of the response</param>
        /// <param name="contentType">Content type to use</param>
        public ByteArrayResponse(byte[] body, string contentType = null)
        {
            this.ContentType = contentType ?? "application/octet-stream";

            this.Contents = stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(body);
                }
            };
        }
    }
}
