using Comparator.Repositories.DbRequests;
using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Parsers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comparator.Repositories.Repositories
{
    internal class SyBaseDb : IRepository
    {
        private const string _provider = "Sap.Data.SQLAnywhere";

        private readonly SyBaseFieldsInfoParser _fieldsParser;

        private readonly ScriptParser _scriptParser;

        private readonly SyBaseRequests _request;

        private string _connectionString;

        private DbRepository _db;

        private string _dbOwner;

        private string _dbName;


        public string DbName => _dbName;

        public string DbType => (Provider.SyBase).ToString();

        public SyBaseDb()
        {
            _fieldsParser = new SyBaseFieldsInfoParser();
            _scriptParser = new ScriptParser();
            _request = new SyBaseRequests();
        }

        public void CreateConnectionString(string source, string server, string dbName, string login, string password)
        {
            _connectionString = $"Data Source=SQL Anywhere 17 Demo;Server={server};DatabaseName={dbName};Uid={login};Password={password};";
            _db = new DbRepository(_connectionString, _provider);
            _dbOwner = login;
            _dbName = dbName;
        }

        public IEnumerable<DtoFullField> GetFieldsInfo(string tableName)
        {
            var fields = GetFields(tableName);
            var constraints = GetConstraints(tableName);
            return _fieldsParser.GetFieldsCollection(fields, constraints);
        }

        /// <summary>
        /// Returns a raw collection of table constraints
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Raw collection of table constraints</returns>
        private IEnumerable<DtoSyBaseConstaintsModel> GetConstraints(string tableName)
        {
            var result = _db.Select<SyBaseConstaintsModel>(_request.GetConstraintsRequest(tableName));
            return Mapper.Map.Map<IEnumerable<SyBaseConstaintsModel>, IEnumerable<DtoSyBaseConstaintsModel>>(result);
        }

        /// <summary>
        /// Returns a raw collection of table fields
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Raw collection of table fields</returns>
        private IEnumerable<SyBaseFieldsModel> GetFields(string tableName) => 
            _db.Select<SyBaseFieldsModel>(_request.GetFieldsRequest(tableName));

        public IEnumerable<string> GetProcedures()
        {
            var result = _db.Select<Procedure>(_request.GetProceduresRequest(_dbOwner));
            return Mapper.Map.Map<IEnumerable<Procedure>, IEnumerable<string>>(result);
        }

        public string GetProcedureSqript(string procedureName) =>
            GetSqript(procedureName);

        public IEnumerable<string> GetTables()
        {
            var result = _db.Select<Table>(_request.GetTablesRequest(_dbOwner));
            return Mapper.Map.Map<IEnumerable<Table>, IEnumerable<string>>(result);
        }

        public IEnumerable<string> GetTriggers() => 
            _db.Select<string>(_request.GetTreggersRequest());

        public async Task<bool> IsConnectionAsync() =>
            await _db.CheckConectionAsync();

        public string GetTriggerSqript(string triggerName) => 
            GetSqript(triggerName);

        /// <summary>
        /// Returns the procedure or trigger script
        /// </summary>
        /// <param name="name">Procedure or trigger name</param>
        /// <returns>Procedure or trigger script</returns>
        private string GetSqript(string name)
        {
            var sqript = _db.Select<string>(_request.GetSqriptRequest(name));
            return _scriptParser.GetProcedureSquript(sqript);
        }
    }
}
