using Nancy.ModelBinding;
using SimpleDMS.Client.Models;
using SimpleDMS.Client.ViewModels;
using System.Collections.Generic;

namespace SimpleDMS.Client.Controllers
{
    public class Archive : Nancy.NancyModule
    {
        public Archive()
        {
            Get["/Archive"] = _ => View["Archive/index.cshtml"];
            Get["/Archive/{guid}"] = _ =>   View["Archive/index.cshtml"];

            Get["/Archive/GetItemsByParentId"] = _ =>
            {
                string id = Request.Query["id"];

                List<TreeNode> docs = DMS.Session.GetDocuments<TreeNode>(id);

                foreach (var doc in docs)
                {
                    bool isFolder = doc.Type < 254 || doc.Type == 9999;
                    bool hasChildren = doc.ChildCount > 0;
                    string icon = string.Format("/Content/images/tree-icons/icon-{0}{1}.ico", doc.Type, hasChildren || !isFolder ? "" : "_disabled");

                    doc.Text = doc.Name;
                    doc.Icon = string.Format("/Content/images/tree-icons/icon-{0}{1}.ico", doc.Type, hasChildren || !isFolder ? "" : "_disabled");
                    doc.Children = doc.ChildCount > 0;
                    doc.Li_attr = new Dictionary<string, object>() {
                        { "objid", doc.Id },
                        { "type", doc.Type },
                        { "isFolder", isFolder },
                        { "guid", doc.Guid }
                    };
                    doc.Id = "ajson" + doc.Id;
                }

                return docs;
            };

            Get["/Archive/GetPathAsArray"] = _ =>
            {
                string id = Request.Query["id"];
                string[] pathArr = DMS.Session.GetPathAsArray(id);

                return pathArr;
            };

            Get["/Archive/AddNewFolder"] = _ =>
            {
                string parentId = Request.Query["selectedNode"];
                string objshort = Request.Query["objshort"];
                int type = int.Parse(Request.Query["type"]);

                Item item = new Item
                {
                    Type = type,
                    Name = objshort,
                    ParentId = parentId,
                    Metadata = new Mask
                    {
                        Maskno = 1
                    }
                };

                return DMS.Session.InsertIntoArchive(item, false);
            };


            Get["/Archive/AddDocumentToFulltext"] = _ =>
            {
                string id = Request.Query["id"];
                string name = Request.Query["name"];

                DMS.Session.AddDocumentToFulltext(id, name);

                return true;
            };

            Get["/Archive/RemoveDocumentFromFulltext"] = _ =>
            {
                string id = Request.Query["id"];
                new OCR().RemoveDocumentFromFulltext(id);

                return true;
            };

            Post["/Archive/MoveDocument"] = _ =>
            {
                var data = this.Bind<Item>();

                DMS.Session.MoveDocument(data);

                return true;
            };

            Post["/Archive/MergeDocuments"] = _ =>
            {
                var files = this.Bind<List<string>>();
                return DocumentExtensionMethods.MergePDFs(files);
            };

            Post["/Archive/SplitDocuments"] = _ =>
            {
                var files = this.Bind<List<string>>();
                return DocumentExtensionMethods.SplitPDFs(files);
            };

            Post["/Archive/DeleteDocuments"] = _ =>
            {
                var files = this.Bind<List<string>>();
                return DMS.Session.DeleteDocuments(files);
            };

            Get["/Archive/GetMaskAndIndexFields"] = _ =>
            {
                int id = int.Parse(Request.Query["id"]);
                var data = DMS.Session.GetMaskAndIndexFields(id);

                return data;
            };

            Get["/Archive/LoadDynKwl"] = _ =>
            {
                string fieldName = Request.Query["fieldName"];
                string searchVal = Request.Query["searchVal"];

                var data = DMS.Session.LoadDynKwl(fieldName, searchVal);

                return data;
            };
        }
    }
}
