using Comparator.Repositories.DbRequests;
using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Parsers;
using System.Collections.Generic;

namespace Comparator.Repositories.Repositories
{
    public class MicrosoftDb : IRepository
    {
        private const string _provider = "System.Data.SqlClient";

        private MicrosoftFieldsInfoParser _fieldsParser;

        private ProcedureScriptParser _scriptParser;

        private string _connectionString;

        private MicrosoftDbRequests _requests;

        private DbRepository _db;

        public void CreateConnectionString(string source, string server, string dbName, string login, string password)
        {
            _fieldsParser = new MicrosoftFieldsInfoParser();
            _scriptParser = new ProcedureScriptParser();
            _connectionString = $"server={server};Trusted_Connection=True;Database={dbName};Connect Timeout=5;";
            _requests = new MicrosoftDbRequests();
            _db = new DbRepository(_connectionString, _provider);
        }

        public IEnumerable<FullField> GetFieldsInfo(string tableName)
        {
            var fields = GetFields(tableName);
            var constraints = GetConstraints(tableName);
            return _fieldsParser.GetFieldsCollection(fields, constraints);
        }

        private IEnumerable<Fields> GetFields(string tableName) =>
            _db.Select<Fields>(_requests.GetFieldsRequest(tableName));

        private IEnumerable<DtoConstraint> GetConstraints(string tableName)
        {
            var constraints = _db.Select<Constraint>(_requests.GetConstraintsRequest(tableName));
            return Mapper.Map.Map<IEnumerable<Constraint>, IEnumerable<DtoConstraint>>(constraints);
        }

        public IEnumerable<string> GetProcedures()
        {
            var procedures = _db.Select<Procedure>(_requests.GetProceduresRequest("dbo"));
            ProcessingCollection(procedures);
            return Mapper.Map.Map<IEnumerable<Procedure>, IEnumerable<string>>(procedures);
        }

        public string GetProcedureSqript(string procedureName)
        {
            var sqript = _db.Select<string>(_requests.GetProcedureSqriptRequest(procedureName));
            return _scriptParser.GetProcedureSquript(sqript);
        }
               
        public IEnumerable<string> GetTables()
        {
            var tables = _db.Select<Table>(_requests.GetTablesRequest("dbo"));
            return Mapper.Map.Map<IEnumerable<Table>, IEnumerable<string>>(tables);
        }

        public bool IsConnection() =>
            _db.CheckConection();

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

