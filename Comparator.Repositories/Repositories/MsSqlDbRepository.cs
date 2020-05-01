using Comparator.Repositories.DbRequests;
using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Parsers.MsSql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comparator.Repositories.Repositories
{
    internal class MsSqlDbRepository : IRepository
    {
        private const string _provider = "System.Data.SqlClient";

        private readonly MsSqlFieldsInfoParser _fieldsParser;

        private readonly MsSqlDbScriptCreator _dbScriptCreator;

        private readonly MsSqlScriptParser _procedureScriptParser;

        private readonly MsSqlTriggerScriptParser _triggerScriptParser;

        private readonly MicrosoftDbRequests _request;

        private string _connectionString;

        private DbRepository _db;

        private string _dbName;


        public string DbType => (Provider.MicrosoftSql).ToString();

        public string DbName => _dbName;         


        public MsSqlDbRepository()
        {
            _fieldsParser = new MsSqlFieldsInfoParser();
            _procedureScriptParser = new MsSqlScriptParser();
           // _dbScriptCreator = new DbScriptCreator(new MsSqlTableCreator());
            _dbScriptCreator = new MsSqlDbScriptCreator();
            _triggerScriptParser = new MsSqlTriggerScriptParser();
            _request = new MicrosoftDbRequests();
        }

        public void CreateConnectionString(string source, string server, string dbName, string login, string password)
        {
            _connectionString = $"server={server};Trusted_Connection=True;Database={dbName};Connect Timeout=5;";
            _db = new DbRepository(_connectionString, _provider);
            _dbName = dbName;
        }

        public IEnumerable<DtoFullField> GetFieldsInfo(string tableName)
        {
            var fields = GetFields(tableName);
            var constraints = GetConstraints(tableName);
            return _fieldsParser.GetFieldsCollection(fields, constraints);
        }

        /// <summary>
        /// Returns a raw collection of table fields
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Raw collection of table fields</returns>
        private IEnumerable<Fields> GetFields(string tableName) =>
            _db.Select<Fields>(_request.GetFieldsRequest(tableName));

        /// <summary>
        /// Returns a raw collection of table constraints
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Raw collection of table constraints</returns>
        private IEnumerable<DtoConstraint> GetConstraints(string tableName)
        {
            var constraints = _db.Select<Constraint>(_request.GetConstraintsRequest(tableName));
            return Mapper.Map.Map<IEnumerable<Constraint>, IEnumerable<DtoConstraint>>(constraints);
        }

        public IEnumerable<string> GetProcedures()
        {
            var procedures = _db.Select<Procedure>(_request.GetProceduresRequest("dbo"));
            foreach (var procedure in procedures)
            {
                var tempArr = procedure.PROCEDURE_NAME.Split(';');
                procedure.PROCEDURE_NAME = tempArr[0];
            }
            return Mapper.Map.Map<IEnumerable<Procedure>, IEnumerable<string>>(procedures);
        }

        public string GetProcedureSqript(string procedureName)
        {
            var sqript = _db.Select<string>(_request.GetSqriptRequest(procedureName));
            return _procedureScriptParser.GetSquript(sqript);
        }

        public string GetTriggerSqript(string triggerName)
        {
            var sqript = _db.Select<string>(_request.GetSqriptRequest(triggerName));
            return _triggerScriptParser.GetSquript(sqript);
        }

        public IEnumerable<string> GetTables()
        {
            var tables = _db.Select<Table>(_request.GetTablesRequest("dbo"));
            return Mapper.Map.Map<IEnumerable<Table>, IEnumerable<string>>(tables);
        }

        public IEnumerable<string> GetTriggers() => 
            _db.Select<string>(_request.GetTreggersRequest());

        public string GetDbScript()
        {
            string tables = GetTablesScript();
            string procedures = GetProceduresScript();
            string triggers = GetTriggersScript();

            return _dbScriptCreator.CreateFullDbScript(tables, procedures, triggers, _dbName);
        }

        public string GetTablesScript()
        {
            var tablesNames = GetTables();
            Dictionary<string, List<DtoFullField>> tables = new Dictionary<string, List<DtoFullField>>();

            foreach (var tableName in tablesNames)
            {
                var temp = GetFieldsInfo(tableName).ToList();
                tables.Add(tableName, temp);
            }
            return _dbScriptCreator.CreateTablesScript(tables);
        }

        public string GetProceduresScript()
        {
            var proceduresNames = GetProcedures();
            List<string> procedures = new List<string>();

            foreach (var procedureName in proceduresNames)
            {
                procedures.Add(GetProcedureSqript(procedureName));
            }
            return _dbScriptCreator.CreateProceduresOrTriggersScript(procedures);
        }

        public string GetTriggersScript()
        {
            var triggersNames = GetTriggers();
            List<string> triggers = new List<string>();

            foreach (var triggerName in triggersNames)
            {
                triggers.Add(GetTriggerSqript(triggerName));
            }
            return _dbScriptCreator.CreateProceduresOrTriggersScript(triggers);
        }
                                    
        public async Task<bool> IsConnectionAsync() => 
            await _db.CheckConectionAsync();
    }
}
