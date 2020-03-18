using Comparator.Repositories.DbRequests;
using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Parsers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comparator.Repositories.Repositories
{
    public class MicrosoftDb : IRepository
    {
        private const string _provider = "System.Data.SqlClient";

        private MicrosoftFieldsInfoParser _fieldsParser;

        private ProcedureScriptParser _scriptParser;

        private string _connectionString;

        private MicrosoftDbRequests _request;

        private DbRepository _db;

        public void CreateConnectionString(string source, string server, string dbName, string login, string password)
        {
            _fieldsParser = new MicrosoftFieldsInfoParser();
            _scriptParser = new ProcedureScriptParser();
            _connectionString = $"server={server};Trusted_Connection=True;Database={dbName};Connect Timeout=5;";
            _request = new MicrosoftDbRequests();
            _db = new DbRepository(_connectionString, _provider);
        }

        public IEnumerable<DtoFullField> GetFieldsInfo(string tableName)
        {
            var fields = GetFields(tableName);
            var constraints = GetConstraints(tableName);
            return _fieldsParser.GetFieldsCollection(fields, constraints);
        }

        private IEnumerable<Fields> GetFields(string tableName) =>
            _db.Select<Fields>(_request.GetFieldsRequest(tableName));

        private IEnumerable<DtoConstraint> GetConstraints(string tableName)
        {
            var constraints = _db.Select<Constraint>(_request.GetConstraintsRequest(tableName));
            return Mapper.Map.Map<IEnumerable<Constraint>, IEnumerable<DtoConstraint>>(constraints);
        }

        public IEnumerable<string> GetProcedures()
        {
            var procedures = _db.Select<Procedure>(_request.GetProceduresRequest("dbo"));
            ProcessingCollection(procedures);
            return Mapper.Map.Map<IEnumerable<Procedure>, IEnumerable<string>>(procedures);
        }

        public string GetSqript(string procedureName)
        {
            var sqript = _db.Select<string>(_request.GetSqriptRequest(procedureName));
            return _scriptParser.GetProcedureSquript(sqript);
        }
               
        public IEnumerable<string> GetTables()
        {
            var tables = _db.Select<Table>(_request.GetTablesRequest("dbo"));
            return Mapper.Map.Map<IEnumerable<Table>, IEnumerable<string>>(tables);
        } 
        
        public IEnumerable<string> GetTriggers() => 
            _db.Select<string>(_request.GetTreggersRequest());
        
        public async Task<bool> IsConnectionAsync() => 
            await _db.CheckConectionAsync();

        private void ProcessingCollection(IEnumerable<Procedure> procedures)
        {
            foreach (var procedure in procedures)
            {
                var tempArr = procedure.PROCEDURE_NAME.Split(';');
                procedure.PROCEDURE_NAME = tempArr[0];
            }
        }
    }
}

