using Nancy.ModelBinding;
using SimpleDMS.Client.Models;
using System.Collections.Generic;
using System.Linq;

namespace SimpleDMS.Client.Controllers
{
    public class Intray : Nancy.NancyModule
    {
        public Intray()
        {
            Get["/Intray"] = _ => View["Intray/index.cshtml"];

            Get["/Intray/GetFiles"] = _ =>
            {
                List<IntrayItem> files = DMS.Session.GetIntrayFiles();
                return files;
            };

            Post["/Intray/InsertIntoArchive"] = _ =>
            {
                IntrayItem file = this.Bind<IntrayItem>();
                string id = DMS.Session.InsertIntoArchive(file, true);
                return id;
            };

            Post["/Intray/UploadFile"] = _ =>
            {
                Nancy.HttpFile file = Request.Files.FirstOrDefault();
                string page = Request.Form["page"];

                var uploader = new FileUploader();
                uploader.SaveFile(file, page);

                return true;
            };
        }
    }
}
