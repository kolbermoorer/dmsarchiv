using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace SimpleDMS.LuceneSearch
{
    public static class LuceneEngine
    {
        private const string LUCENE_IDENTIFIER = "Id";

        private static string _luceneDir = Path.Combine(@"D:\SimpleDMS\data\search-DMSArchiv", "search_index");
        private static FSDirectory _directoryTemp;
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        /**************
         * Search Documents
         **************/

        public static IEnumerable<ArchiveDocument> Search(string input, string fieldName = "")
        {
            if (string.IsNullOrEmpty(input)) return new List<ArchiveDocument>();

            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);

            return _search(input, fieldName);
        }

        public static IEnumerable<ArchiveDocument> SearchDefault(string input, string fieldName = "")
        {
            return string.IsNullOrEmpty(input) ? new List<ArchiveDocument>() : _search(input, fieldName);
        }

        public static IEnumerable<ArchiveDocument> GetAllIndexRecords()
        {
            // validate search index
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<ArchiveDocument>();

            // set up lucene searcher
            var searcher = new IndexSearcher(_directory, false);
            var reader = IndexReader.Open(_directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            while (term.Next()) docs.Add(searcher.Doc(term.Doc));
            reader.Dispose();
            searcher.Dispose();
            return _mapLuceneToDataList(docs);
        }

        private static IEnumerable<ArchiveDocument> _search(string searchQuery, string searchField = "")
        {
            // validation
            if (string.IsNullOrEmpty(searchQuery.Replace("*", "").Replace("?", ""))) return new List<ArchiveDocument>();

            // set up lucene searcher
            using (var searcher = new IndexSearcher(_directory, false))
            {
                var hits_limit = 1000;
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);

                // search by single field
                if (!string.IsNullOrEmpty(searchField))
                {
                    var parser = new QueryParser(Version.LUCENE_30, searchField, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search(query, hits_limit).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
                // search by multiple fields (ordered by RELEVANCE)
                else
                {
                    var parser = new MultiFieldQueryParser
                        (Version.LUCENE_30, new[] { "Id", "Name", "Fulltext" }, analyzer);
                    var query = parseQuery(searchQuery, parser);
                    var hits = searcher.Search
                    (query, null, hits_limit, Sort.RELEVANCE).ScoreDocs;
                    var results = _mapLuceneToDataList(hits, searcher);
                    analyzer.Close();
                    searcher.Dispose();
                    return results;
                }
            }
        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }

        /**************
         * Add Document(s)
         **************/

        public static void AddSingleDocumentToIndex(ArchiveDocument doc)
        {

            AddMultipleDocumentsToIndex(new List<ArchiveDocument> { doc });
        }

        public static void AddMultipleDocumentsToIndex(List<ArchiveDocument> docs)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                // add data to lucene search index (replaces older entry if any)
                foreach (var doc in docs) _addToLuceneIndex(doc, writer);

                // close handles
                analyzer.Close();
                writer.Dispose();
            }
        }

        private static void _addToLuceneIndex(ArchiveDocument document, IndexWriter writer)
        {
            _removeFromLuceneIndex(document, writer);

            // add new index entry
            var doc = new Document();
            doc.Add(new Field("Id", document.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name", document.Name, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Fulltext", document.Fulltext, Field.Store.YES, Field.Index.ANALYZED));

            writer.AddDocument(doc);
        }

        /**************
         * Remove Document(s)
         **************/

        public static void RemoveSingleDocumentFromIndex(ArchiveDocument doc)
        {
            RemoveMultipleDocumentsFromIndex(new List<ArchiveDocument> { doc });
        }

        public static void RemoveMultipleDocumentsFromIndex(List<ArchiveDocument> docs)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var doc in docs)
                    _removeFromLuceneIndex(doc, writer);

                analyzer.Close();
                writer.Dispose();
            }
        }

        private static void _removeFromLuceneIndex(ArchiveDocument document, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term(LUCENE_IDENTIFIER, document.Id.ToString()));
            writer.DeleteDocuments(searchQuery);
        }

        /**************
         * Clear Index
         **************/


        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    writer.DeleteAll();
                    analyzer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /**************
         * Optimize Index
         **************/

        public static void ReindexDocuments()
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Dispose();
            }
        }


        /**************
         * Map Document To Index
         **************/

        private static ArchiveDocument _mapLuceneDocumentToData(Document doc)
        {
            return new ArchiveDocument
            {
                Id = doc.Get("Id"),
                Name = doc.Get("Name"),
                Fulltext = doc.Get("Fulltext")
            };
        }

        private static IEnumerable<ArchiveDocument> _mapLuceneToDataList(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData).ToList();
        }
        private static IEnumerable<ArchiveDocument> _mapLuceneToDataList(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData(searcher.Doc(hit.Doc))).ToList();
        }

    }
}
