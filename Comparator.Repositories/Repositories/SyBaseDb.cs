using Comparator.Repositories.DbRequests;
using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Parsers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comparator.Repositories.Repositories
{
    public class SyBaseDb : IRepository
    {
        private const string _provider = "Sap.Data.SQLAnywhere";

        private SyBaseFieldsInfoParser _fieldsParser;

        private ProcedureScriptParser _scriptParser;

        private string _connectionString;

        private SyBaseRequests _request;

        private DbRepository _db;

        private string _dbOwner;

        public void CreateConnectionString(string source, string server, string dbName, string login, string password)
        {
            _fieldsParser = new SyBaseFieldsInfoParser();
            _scriptParser = new ProcedureScriptParser();
            _connectionString = $"Data Source=SQL Anywhere 17 Demo;Server={server};DatabaseName={dbName};Uid={login};Password={password};";
            _request = new SyBaseRequests();
            _db = new DbRepository(_connectionString, _provider);
            _dbOwner = login;
        }
        
        public IEnumerable<DtoFullField> GetFieldsInfo(string tableName)
        {
            var fields = GetFields(tableName);
            var constraints = GetConstraints(tableName);
            return _fieldsParser.GetFieldsCollection(fields, constraints);
        }

        private IEnumerable<DtoSyBaseConstaintsModel> GetConstraints(string tableName)
        {
            var result = _db.Select<SyBaseConstaintsModel>(_request.GetConstraintsRequest(tableName));
            return Mapper.Map.Map<IEnumerable<SyBaseConstaintsModel>, IEnumerable<DtoSyBaseConstaintsModel>>(result);
        }

        private IEnumerable<SyBaseFieldsModel> GetFields(string tableName) => 
            _db.Select<SyBaseFieldsModel>(_request.GetFieldsRequest(tableName));

        public IEnumerable<string> GetProcedures()
        {
            var result = _db.Select<Procedure>(_request.GetProceduresRequest(_dbOwner));
            return Mapper.Map.Map<IEnumerable<Procedure>, IEnumerable<string>>(result);
        }

        public string GetSqript(string procedureName)
        {
            var sqript = _db.Select<string>(_request.GetSqriptRequest(procedureName));
            return _scriptParser.GetProcedureSquript(sqript);
        }        

        public IEnumerable<string> GetTables()
        {
            var result = _db.Select<Table>(_request.GetTablesRequest(_dbOwner));
            return Mapper.Map.Map<IEnumerable<Table>, IEnumerable<string>>(result);
        }

        public IEnumerable<string> GetTriggers() => 
            _db.Select<string>(_request.GetTreggersRequest());
                
        public async Task<bool> IsConnectionAsync() =>
            await _db.CheckConectionAsync();
    }
}
