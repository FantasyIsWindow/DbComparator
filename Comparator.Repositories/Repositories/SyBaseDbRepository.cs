using Comparator.Repositories.DbRequests;
using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Parsers.SyBase;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Comparator.Repositories.Repositories
{
    internal class SyBaseDbRepository : IRepository
    {
        private const string _provider = "Sap.Data.SQLAnywhere";

        private readonly SyBaseFieldsInfoParser _fieldsParser;

        private readonly SyBaseScriptParser _scriptParser;

        private readonly SyBaseTriggerScriptParser _triggerScriptParser;

        private readonly SyBaseDbScriptCreator _dbScriptCreator;

        private readonly SyBaseRequests _request;

        private string _connectionString;

        private DbRepository _db;

        private string _dbOwner;

        private string _dbName;
        
        
        public string DbName => _dbName;

        public string DbType => (Provider.SyBase).ToString();

        public SyBaseDbRepository()
        {
            _fieldsParser = new SyBaseFieldsInfoParser();
            _scriptParser = new SyBaseScriptParser();
            _triggerScriptParser = new SyBaseTriggerScriptParser();
            _dbScriptCreator = new SyBaseDbScriptCreator();
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

        public IEnumerable<string> GetTables()
        {
            var result = _db.Select<Table>(_request.GetTablesRequest(_dbOwner));
            return Mapper.Map.Map<IEnumerable<Table>, IEnumerable<string>>(result);
        }

        public IEnumerable<string> GetTriggers() => 
            _db.Select<string>(_request.GetTreggersRequest());

        public async Task<bool> IsConnectionAsync() =>
            await _db.CheckConectionAsync();

        public string GetProcedureSqript(string procedureName)
        {
            var sqript = _db.Select<string>(_request.GetSqriptRequest(procedureName));
            return _scriptParser.GetSquript(sqript);
        }

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

        public string GetTriggerSqript(string triggerName)
        {
            var sqript = _db.Select<string>(_request.GetSqriptRequest(triggerName));
            return _triggerScriptParser.GetTriggerParser(sqript);
        }
    }
}
