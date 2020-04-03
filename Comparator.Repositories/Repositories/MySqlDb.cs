using Comparator.Repositories.DbRequests;
using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Parsers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comparator.Repositories.Repositories
{
    internal class MySqlDb : IRepository
    {
        private const string _provider = "MySql.Data.MySqlClient";

        private readonly MySqlFieldsInfoParser _fieldsParser;

        private readonly ScriptParser _scriptParser;

        private readonly MySqlRequests _request;

        private string _connectionString;        

        private DbRepository _db;

        private string _dbName;


        public string DbName => _dbName;

        public string DbType => (Provider.MySql).ToString();


        public MySqlDb()
        {
            _request = new MySqlRequests();
            _scriptParser = new ScriptParser();
            _fieldsParser = new MySqlFieldsInfoParser();
        }

        public void CreateConnectionString(string source, string server, string dbName, string login, string password)
        {
            _connectionString = $"server={server};database={dbName};user={login};password={password};";
            _dbName = dbName;
            _db = new DbRepository(_connectionString, _provider);
        }

        public IEnumerable<DtoFullField> GetFieldsInfo(string tableName)
        {
            var fields = GetFields(tableName);
            var constraints = GetConstraints(tableName);
            var cascadeOptions = GetCascadeOptions(tableName);
            return _fieldsParser.GetFieldsCollection(fields, constraints, cascadeOptions);
        }

        private IEnumerable<MySqlCascadeOption> GetCascadeOptions(string tableName) => 
            _db.Select<MySqlCascadeOption>(_request.GetCascadeOptions(_dbName, tableName));

        /// <summary>
        /// Returns a raw collection of table fields
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Raw collection of table fields</returns>
        private IEnumerable<MySqlFields> GetFields(string tableName) =>
            _db.Select<MySqlFields>(_request.GetFieldsRequest(_dbName, tableName ));

        /// <summary>
        /// Returns a raw collection of table constraints
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Raw collection of table constraints</returns>
        private IEnumerable<MySqlConstaintsModel> GetConstraints(string tableName) => 
            _db.Select<MySqlConstaintsModel>(_request.GetConstraintsRequest(_dbName, tableName));

        public IEnumerable<string> GetProcedures()
        {
            var procedures = _db.Select<MySqlProcedure>(_request.GetProceduresRequest(_dbName));
            return Mapper.Map.Map<IEnumerable<MySqlProcedure>, IEnumerable<string>>(procedures);
        }

        public string GetProcedureSqript(string procedureName)
        {
            var sqript = _db.SelectForKey(_request.GetProcedureSqriptRequest(procedureName), "Create Procedure");
            return _scriptParser.GetProcedureSquript(sqript);
        }

        public IEnumerable<string> GetTables() =>
            _db.Select<string>(_request.GetTablesRequest(_dbName));

        public IEnumerable<string> GetTriggers()
        {
            var triggers = _db.Select<MySqlTrigger>(_request.GetTreggersRequest(_dbName));
            return Mapper.Map.Map<IEnumerable<MySqlTrigger>, IEnumerable<string>>(triggers);
        }

        public string GetTriggerSqript(string triggerName)
        {
            var sqript = _db.Select<MySqlTriggerSqript>(_request.GetTreggersSqriptRequest(_dbName, triggerName));
            return _scriptParser.GetTriggerSqript(sqript);
        }

        public async Task<bool> IsConnectionAsync() =>
            await _db.CheckConectionAsync();
    }
}
