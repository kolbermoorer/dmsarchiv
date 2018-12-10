using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDMS.LuceneSearch
{
    public class ArchiveDocument
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Fulltext { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
