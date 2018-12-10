using SimpleDMS.Client.Models;
using System.Collections.Generic;

namespace SimpleDMS.Client.Controllers
{
    public class Search : Nancy.NancyModule
    {
        public Search()
        {
            Get["/Search"] = _ => View["Search/index.cshtml"];

            
            Get["/Search/Execute"] = _ =>
            {
                string val = Request.Query["val"];
                List<Item> docs = DMS.Session.FindDocuments(val);
                return docs;
            };
        }
    }
}
