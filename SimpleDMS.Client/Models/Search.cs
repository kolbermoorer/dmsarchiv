using SimpleDMS.LuceneSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDMS.Client.Models
{
    public class Search
    {
        public IEnumerable<ArchiveDocument> Execute(string val)
        {
            return LuceneEngine.Search(val);
        }

        public void AddToLuceneIndex()
        {

        }
    }
}
