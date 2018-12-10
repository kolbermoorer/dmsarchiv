using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using SimpleDMS.Client.ViewModels;
using SimpleDMS.Database;
using SimpleDMS.LuceneSearch;

namespace SimpleDMS.Client.Models
{
    public class DMS
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static DMS Session = new DMS();

        private string archiveName;

        private Connection conn;
        private List<Mask> masks;
        private Dictionary<string, List<masklines>> fields;

        public string ArchiveName { get => archiveName; }
        public Connection Conn { get => conn; }
        public List<Mask> Masks { get => masks; }
        public Dictionary<string, List<masklines>> Fields { get => fields; }

        public List<IntrayItem> GetIntrayFiles()
        {
            var sordTypes = SordType.GetSordTypes();

            sordTypes = sordTypes.Where(x => x.extensions != null).ToList();

            string path = SimpleServerExtensionMethods.GetIntrayPath("DMSArchiv");

            var intrayFiles = new List<IntrayItem>();

            DirectoryInfo d = new DirectoryInfo(path);
            var files = d.GetFiles("*.*").ToList();

            foreach (FileInfo file in files)
            {
                string ext = file.Extension.Substring(1);
                var sordInfo = sordTypes.Where(x => x.extensions.Contains(ext)).FirstOrDefault();

                if (sordInfo == null)
                    sordInfo = sordTypes.Where(x => x.id == 254).First();

                var intrayData = Path.ChangeExtension(file.FullName, ".es8");
                var objshort = Path.GetFileNameWithoutExtension(file.FullName);

                var newIntrayFile = new IntrayItem
                {
                    FilePath = file.FullName,
                    FileName = file.Name,
                    Name = objshort,
                    CreationDate = file.CreationTime,
                    IconBase64 = Convert.ToBase64String(sordInfo.icon.data),
                    Type = sordInfo.id
                };

                intrayFiles.Add(newIntrayFile);
            }

            intrayFiles = intrayFiles.OrderBy(x => x.CreationDate).ToList();

            return intrayFiles;
        }

        public List<Item> FindDocuments(string val)
        {
            var fulltextResults = LuceneSearch.LuceneEngine.Search(val).ToList();

            string[] searchByFulltextIds = fulltextResults.Select(x=>x.Id).ToArray();

            var sql = "SELECT *, (SELECT COUNT(*) FROM item o2 WHERE o2.parentid = o1.id ) AS childcount FROM item o1 WHERE name LIKE @name OR id IN @ids";
            var parameters = new Dictionary<string, object>() {
                ["name"] = "%" + val + "%",
                ["ids"] = searchByFulltextIds
            };

            var docs = conn.ExecuteQuery<Item>(sql, parameters);

            docs.ForEach(x => x.Icon = string.Format("/Content/images/tree-icons/icon-{0}{1}.ico", x.Type, x.ChildCount > 0 || !(x.Type < 254 || x.Type == 9999) ? "" : "_disabled"));

            return docs;
        }

        public Dictionary<string, object> GetConfig()
        {
            Dictionary<string, object> config = new Dictionary<string, object>();
            config["masks"] = masks;
            config["fields"] = fields;

            return config;
        }

        private void _setDocMasks()
        {
            string masksSQL = @"SELECT * FROM mask ORDER BY maskname";
            string fieldsSQL = @"SELECT * FROM masklines ORDER BY mlineno";
            masks = conn.ExecuteQuery<Mask>(masksSQL, closeConnection: false);
            var fields = conn.ExecuteQuery<masklines>(fieldsSQL);

            foreach(var mask in masks)
            {
                var lines = fields.Where(x => x.Maskno == mask.Maskno).ToList();
                this.fields[mask.Maskname.ToLower()] = lines;
            }
        }

        public void Init(string archiveName)
        {
            this.archiveName = archiveName;
            this.conn = new Connection(@"D:\SimpleDMS\data", archiveName);
            this.masks = new List<Mask>();
            this.fields = new Dictionary<string, List<masklines>>();

            _setDocMasks();
        }

        public void AddDocumentToFulltext(string id, string name)
        {
            string fulltext = new OCR().AddDocumentToFulltext(id);

            ArchiveDocument arcDoc = new ArchiveDocument()
            {
                Id = id,
                Name = name, 
                Fulltext = fulltext
            };

            LuceneSearch.LuceneEngine.AddSingleDocumentToIndex(arcDoc);
        }

        public void MoveDocument(Item request)
        {
            var parentid = _createArcPath(request.Metadata);
            var sql = "UPDATE item SET parentid = @parentid WHERE id = @id";
            var parameters = new Dictionary<string, object>() { ["parentid"] = parentid, ["id"] = request.Id };

            conn.ExecuteQuery<bool>(sql, parameters);

            _updateMetadata(request.Id.ToString(), request.Metadata.Fields);
        }

        public void _updateMetadata(string objid, Dictionary<string, object> fields)
        {
            foreach (KeyValuePair<string, object> field in fields)
            {
                var sql = "INSERT OR REPLACE INTO metadata VALUES (@parentid, @fieldkey, @fielddata);";

                //var sql = "UPDATE metadata SET fielddata = @fielddata WHERE parentid = @parentid AND fieldkey = @fieldkey";
                var parameters = new Dictionary<string, object>() { ["parentid"] = objid, ["fieldkey"] = field.Key, ["fielddata"] = field.Value };
                conn.ExecuteQuery<bool>(sql, parameters);
            } 
        }

        public string InsertIntoArchive(Item item, bool isIntray)
        {
            var parentid = _createArcPath(item.Metadata);
            item.ParentId = parentid;
            _addEntry(item);

            string id = _exists(parentid, item.Name);

            _updateMetadata(id, item.Metadata.Fields);

            if (isIntray)
            {
                var intrayItem = item as IntrayItem;
                _insertPhysicalFile(id, intrayItem.FilePath);

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    /* run your code here */
                    AddDocumentToFulltext(id, item.Name);
                }).Start();

            }

            return id;
        }

        private void _insertPhysicalFile(string id, string filepath)
        {
            string hexId = SimpleServerExtensionMethods.GetHexValue(id);
            string hexFolder = SimpleServerExtensionMethods.GetHexFolder(id);

            string intrayPath = SimpleServerExtensionMethods.GetIntrayPath("DMSArchiv");

            string archivePath = SimpleServerExtensionMethods.GetArchivePath("DMSArchiv");

            string ext = Path.GetExtension(filepath);
            string archiveFile = Path.Combine(archivePath, "basis", hexFolder, hexId + ext);

            File.Move(filepath, archiveFile);
        }

        private string _createArcPath(Mask metadata, char delimiter = '¶')
        {
            string maskindex = delimiter + (metadata.Maskindex ?? "");
            if (maskindex == delimiter.ToString())
                maskindex = string.Empty;

            string path = metadata.Maskno.ToString("D2") + " " + metadata.Maskname + maskindex;

            foreach (KeyValuePair<string, object> field in metadata.Fields)
            {
                path = path.Replace(field.Key, field.Value.ToString());
            }

            var arr = path.Split(delimiter);
            string objparent = "1";
            string objid = string.Empty;
            int level = 1;
            foreach (string objshort in arr)
            {
                objid = _exists(objparent, objshort);

                if (objid == null)
                {
                    Item item = new Item
                    {
                        Type = level,
                        ParentId = objparent,
                        Name = objshort,
                        Metadata = new Mask
                        {
                            Maskno = 1
                        }
                    };
                    _addEntry(item);
                    objid = _exists(objparent, objshort);
                }
                objparent = objid;
                level++;
            }
            return objid;
        }

        private bool _addEntry(Item item)
        {
            try
            {
                string sql = @"INSERT INTO item(type, name, parentid, creationdate, documentdate, maskno, guid, editor, version) VALUES(@type, @name, @parentid, @creationdate, @documentdate, @maskno, @guid, @editor, @version)";
                var parameters = new Dictionary<string, object>() {
                    ["type"] = item.Type,
                    ["name"] = item.Name,
                    ["parentid"] = item.ParentId,
                    ["creationdate"] = DateTime.Now,
                    ["documentdate"] = DateTime.Now,
                    ["maskno"] = item.Metadata.Maskno,
                    ["guid"] = Guid.NewGuid().ToString("P"),
                    ["editor"] = _getUsername(),
                    ["version"] = 1
                };

                conn.ExecuteQuery<bool>(sql, parameters);

                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return false;
            }
        }

        private string _exists(string objparent, string objshort)
        {
            string sql = @"SELECT id FROM item o WHERE o.parentid = @parentid AND name = @name";
            var parameters = new Dictionary<string, object>()
            {
                ["parentid"] = objparent,
                ["name"] = objshort
            };

            return conn.ExecuteQuery<string>(sql, parameters).FirstOrDefault();
        }

        private int _getUsername()
        {
            return 2;
        }

        public List<T> GetDocuments<T>(string id)
        {
            var sql = "SELECT o1.*, (SELECT COUNT(*) FROM Item o2 WHERE o2.parentid = o1.id ) AS childcount FROM item o1 WHERE parentid = @parentid";
            var parameters = new Dictionary<string, object>() { ["parentid"] = id };

            var docs = conn.ExecuteQuery<T>(sql, parameters);

            return docs;
        }

        public string[] GetPathAsArray(string id)
        {
            var parameters = new Dictionary<string, object>() { ["id"] = id };
            string sql = @"WITH RECURSIVE path(level, name, parentid) AS (
                                SELECT 0, name, parentid
                                FROM item
                                WHERE id = @id or guid = @id
                                UNION ALL
                                SELECT path.level + 1,
                                       item.name,
                                       item.parentid
                                FROM item
                                JOIN path ON item.id = path.parentid
                            ),
                            path_from_root AS (
                                SELECT parentid
                                FROM path
                                ORDER BY level DESC
                            )
                            SELECT group_concat(parentid, '¶')
                            FROM path_from_root;
                            ";

            string path = conn.ExecuteQuery<string>(sql, parameters).FirstOrDefault() ?? string.Empty;

            return path.Split('¶');
        }

        public List<string> LoadDynKwl(string fieldName, string searchVal)
        {
            string sql = "SELECT DISTINCT fielddata FROM metadata WHERE fieldkey = @fieldkey AND fielddata LIKE @fieldData";
            var parameters = new Dictionary<string, object>() { ["fieldkey"] = fieldName, ["fieldData"] = "%" + searchVal + "%" };

            var data = conn.ExecuteQuery<string>(sql, parameters);

            return data;
        }

        public List<Metadata> GetMaskAndIndexFields(int id)
        {
            string sql = @" SELECT metadata.*, masklines.linetype, (SELECT maskno FROM item WHERE item.id = metadata.parentid) AS maskno 
                            FROM metadata LEFT JOIN
                            masklines ON metadata.fieldkey = masklines.linekey
                            WHERE parentid = @parentid";
            var parameters = new Dictionary<string, object>() { ["parentid"] = id };

            var data = conn.ExecuteQuery<Metadata>(sql, parameters);

            return data;
        }

        public bool DeleteDocuments(List<string> files)
        {
            try
            {
                var isIntray = Regex.IsMatch(files[0], @"^\d+$") == false;

                if(isIntray)
                {
                    foreach (var file in files)
                        File.Delete(file);
                }
                else
                {
                    //TODO
                }

                return true;
            }
            catch(Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.StackTrace);
                return false;
            }
        }
    }
}
