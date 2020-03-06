using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using Dapper;

namespace DbConectionInfoRepository.Repositories
{
    public class DbRepository
    {
        private readonly string _fullPath;
        private readonly string _connectionString;
        private string _dbName;
        private readonly string _tableName;

        public string TableName
            => _tableName; 
        
        public DbRepository()
        {
            _dbName = "dataDb.db";
            _fullPath = Path.Combine(Environment.CurrentDirectory, _dbName);
            _connectionString = $"Data Source={_fullPath};Version=3;";
            _tableName = "ConnectionInfo";
            CreateDb();
        }

        private void CreateDb()
        {
            if (!File.Exists(_dbName))
            {
                var file = File.Create(_fullPath);
                file.Close();
                string mainTable = $"CREATE TABLE IF NOT EXISTS [{_tableName}] (" +
                                   "[Id] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                   "[DataSource] NVARCHAR(256) NULL, " +
                                   "[ServerName] NVARCHAR(128) NOT NULL, " +
                                   "[DbName] NVARCHAR(128) NOT NULL, " +
                                   "[Login] NVARCHAR(128) NULL, " +
                                   "[Password] NVARCHAR(128) NULL, " +
                                   "[DbType] NVARCHAR(256) NOT NULL, " +
                                   "[Reference] NVARCHAR(26) NOT NULL)";

                ExecuteCommand(mainTable);
            }
        }

        public void ExecuteCommand(string command, object obj = null)
        {
            try
            {
                using (SQLiteConnection _connection = new SQLiteConnection(_connectionString))
                {
                    _connection.Open();
                    _connection.Execute(command, obj);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IEnumerable<T> ExecuteQuery<T>(string query) where T : class
        {
            try
            {
                using (SQLiteConnection _connection = new SQLiteConnection(_connectionString))
                {
                    _connection.ConnectionString = _connectionString;
                    _connection.Open();
                    var result = _connection.Query<T>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
