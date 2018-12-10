using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace SimpleDMS.Database
{
    public class Connection
    {
        //private readonly string folder = "/archive/db/";
        private SQLiteConnection connection;
        private string dbPath;
        private string archiveName;

        public Connection(string archivePath, string archiveName)
        {
            dbPath = Path.Combine(archivePath, "db-" + archiveName, archiveName + ".sqlite");

            this.archiveName = archiveName;

            //if (File.Exists(dbPath))
            //    _connect();
        }

        public void Connect()
        {
            _connect();
        }

        private void _connect()
        {
            if(connection == null || connection.State == ConnectionState.Closed) { 
                connection = new SQLiteConnection();
                connection.ConnectionString = "Data Source=" + dbPath + "; pooling = true;";
                connection.Open();
           }
        }

        private void _disconnect(bool close = true)
        {
            if (connection.State == ConnectionState.Open && close)
            {
                //connection.Close();
                //connection.Dispose();
            }  
        }

        public List<T> ExecuteQuery<T>(string sql, Dictionary<string, object> parameters = null, bool closeConnection = true)
        {
            _connect();
            try
            {
                if (parameters != null)
                {
                    DynamicParameters dp = new DynamicParameters();

                    foreach(var param  in parameters) {
                        dp.Add(param.Key, param.Value);
                    }

                    if (connection.State == ConnectionState.Closed)
                        _connect();

                    return Dapper.SqlMapper.Query<T>(connection, sql, dp).ToList();
                }  
                else
                    return Dapper.SqlMapper.Query<T>(connection, sql).ToList();
            }
            finally
            {
                _disconnect(closeConnection);
            }
        }
    }
}
