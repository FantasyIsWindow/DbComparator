using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Parsers;
using Comparator.Repositories.Parsers.MsSql;
using Comparator.Repositories.Parsers.MySql;
using Comparator.Repositories.Parsers.SyBase;
using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Repositories
{
    public class ScriptCollector
    {
        private IRepository _repository;

        private IDatabaseScriptDesigner _scriptDesigner;

        private ITableCreator _tableCreator;

        /// <summary>
        /// Initializing the repository
        /// </summary>
        /// <param name="repository"></param>
        public void SetRepository(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Returns the object of the collector tables
        /// </summary>
        /// <param name="provider"></param>
        private void GetCreator(Provider provider)
        {
            if (provider == Provider.MicrosoftSql)
            {
                _tableCreator = new MsSqlTableCreator();
            }
            else if (provider == Provider.MySql)
            {
                _tableCreator = new MySqlTableCreator();
            }
            else if (provider == Provider.SyBase)
            {
                _tableCreator = new SyBaseTableCreator();
            }
        }

        /// <summary>
        /// Returns the script designer object
        /// </summary>
        /// <param name="provider"></param>
        private void GetDesigner(Provider provider)
        {
            if (provider == Provider.MicrosoftSql)
            {
                _scriptDesigner = new MsSqlDbScriptCreator();
            }
            else if (provider == Provider.MySql)
            {
                _scriptDesigner = new MySqlDbScriptCreator();
            }
            else if (provider == Provider.SyBase)
            {
                _scriptDesigner = new SyBaseDbScriptCreator();
            }
        }

        /// <summary>
        /// Get the script for the entire database
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>The generated script</returns>
        public string GetDbScript(Provider provider)
        {
            string tables = GetTablesScript(provider);
            string procedures = GetProceduresScript(provider);
            string triggers = GetTriggersScript(provider);

            return _scriptDesigner.CreateFullDbScript(tables, procedures, triggers, "123");
        }

        /// <summary>
        /// Get the script for all tables from the database
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>The generated script</returns>
        public string GetTablesScript(Provider provider)
        {
            GetCreator(provider);
            GetDesigner(provider);
            var tablesNames = _repository.GetTables();

            List<string> tables = new List<string>();
            List<string> constraints = new List<string>();

            foreach (var tableName in tablesNames)
            {
                List<DtoFullField> temp = _repository.GetFieldsInfo(tableName).ToList();

                var result = _tableCreator.GetTableScript(temp, tableName).Split('$');

                if (result.Length >= 2)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        if (i == 0)
                        {
                            tables.Add(result[i]);
                        }
                        else
                        {
                            constraints.Add(result[i]);
                        }
                    }
                }
                else
                {
                    tables.Add(result[0]);
                }
            }

            return _scriptDesigner.CreateTablesScript(tables, constraints);
        }

        /// <summary>
        /// Get the script for all procedures from the database
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>The generated script</returns>
        public string GetProceduresScript(Provider provider)
        {
            GetDesigner(provider);
            var proceduresNames = _repository.GetProcedures();
            List<string> procedures = new List<string>();

            foreach (var procedureName in proceduresNames)
            {
                string script = _repository.GetProcedureSqript(procedureName);
                procedures.Add(script);
            }

            return _scriptDesigner.CreateProceduresOrTriggersScript(procedures);
        }

        /// <summary>
        /// Get the script for all triggers from the database
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>The generated script</returns>
        public string GetTriggersScript(Provider provider)
        {
            GetDesigner(provider);
            var triggersNames = _repository.GetTriggers();
            List<string> triggers = new List<string>();

            foreach (var triggerName in triggersNames)
            {
                string script = _repository.GetTriggerSqript(triggerName);
                triggers.Add(script);
            }
            return _scriptDesigner.CreateProceduresOrTriggersScript(triggers);
        }
    }
}
