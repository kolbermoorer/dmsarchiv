using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDMS.Client.Models
{
    public class SordType
    {
        public FileData disabledIcon { get; set; }
        public string[] extensions { get; set; }
        public FileData icon { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public FileData workflowIcon { get; set; }
        public static List<SordType> GetSordTypes()
        {
            var file = Path.Combine(SimpleServerExtensionMethods.GetRootPath(), "Content", "data", "sordtypes.json");
            var json = System.IO.File.ReadAllText(file);

            var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SordType>>(json);

            return list;
        }
    }

    public class FileData
    {
        public string contentType { get; set; }
        public byte[] data { get; set; }
    }
}
