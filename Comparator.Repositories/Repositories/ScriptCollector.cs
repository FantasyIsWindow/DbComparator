using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Parsers;
using Comparator.Repositories.Parsers.MsSql;
using Comparator.Repositories.Parsers.MySql;
using Comparator.Repositories.Parsers.SyBase;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Repositories
{
    /// <summary>
    /// Data base data entities
    /// </summary>
    public enum DataEntities { db, tables, procedures, triggers }

    public class ScriptCollector
    {
        private IDatabaseScriptDesigner _scriptDesigner;

        private ITableCreator _tableCreator;

        /// <summary>
        /// Get the full database script
        /// </summary>
        /// <param name="name">Returned data base name</param>
        /// <param name="reps">Repositories collection</param>
        /// <param name="dType">Data base data entities</param>
        /// <param name="provider">Returned provider</param>
        /// <returns></returns>
        public string GetScript(string name, List<IRepository> reps, DataEntities dType, Provider provider)
        {
            if (CanConvert(reps, provider))
            {
                GetDesigner(provider);
                GetCreator(provider);

                if (dType == DataEntities.db)
                {
                    return CollectorScriptDb(name, reps);
                }
                else if (dType == DataEntities.tables)
                {
                    return CollectorScriptTables(reps);
                }
                else if (dType == DataEntities.procedures)
                {
                    return CollectorScriptProcedures(reps);
                }
                else if (dType == DataEntities.triggers)
                {
                    return CollectorScriptTriggers(reps);
                }
                return "";
            }
            else
            {
                throw new Exception("     It is very likely that the ability to convert from and " +
                                    "to a MySql database will be implemented in the next updates.");
            }            
        }

        /// <summary>
        /// Stub function for the time when converting from and to a mysql database is implemented
        /// </summary>
        /// <param name="reps">Repositories collection</param>
        /// <param name="provider">Returned provider</param>
        /// <returns>Result of checking whether the operation can be performed</returns>
        private bool CanConvert(List<IRepository> reps, Provider provider)
        {
            if (reps.Count() == 1)
            {
                if (reps[0].DbType == provider.ToString() || 
                    (reps[0].DbType != Provider.MySql.ToString() && provider != Provider.MySql))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var mySql = reps.Exists(r => r.DbType == Provider.MySql.ToString());

                var other = reps.Exists(r => 
                    r.DbType == Provider.MicrosoftSql.ToString() || 
                    r.DbType == Provider.SyBase.ToString());

                if (mySql && !other && provider == Provider.MySql || 
                    !mySql && other && provider != Provider.MySql)
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            }           
        }

        /// <summary>
        /// Building a database script
        /// </summary>
        /// <param name="name">Returned data base name</param>
        /// <param name="reps">Repositories collection</param>
        /// <returns>Full data base script</returns>
        private string CollectorScriptDb(string name, List<IRepository> reps)
        {
            List<string> tables = new List<string>();
            List<string> constraints = new List<string>();
            List<string> procedures = new List<string>();
            List<string> triggers = new List<string>();

            foreach (var rep in reps)
            {
                GetTablesScript(rep, tables, constraints);
                procedures.AddRange(GetProceduresScript(rep));
                triggers.AddRange(GetTriggersScript(rep));
            }

            return _scriptDesigner.CreateFullDbScript(tables, constraints, procedures, triggers, name);
        }

        /// <summary>
        /// Building all tables script
        /// </summary>
        /// <param name="reps">Repositories collection</param>
        /// <returns>All table script</returns>
        private string CollectorScriptTables(List<IRepository> reps)
        {
            List<string> tables = new List<string>();
            List<string> constraints = new List<string>();

            foreach (var rep in reps)
            {
                GetTablesScript(rep, tables, constraints);
            }

            return _scriptDesigner.CreateTablesScript(tables, constraints);
        }

        /// <summary>
        /// Get the script for all tables from the database
        /// </summary>
        /// <param name="rep">Processed provider</param>
        /// <returns>The generated script</returns>
        private void GetTablesScript(IRepository rep, List<string> tables, List<string> constraints)
        {
            var tablesNames = rep.GetTables();

            foreach (var tableName in tablesNames)
            {
                List<DtoFullField> tempFICol = rep.GetFieldsInfo(tableName).ToList();

                var result = _tableCreator.GetTableScript(tempFICol, tableName).Split('$');

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
        }

        /// <summary>
        /// Building a procedures script
        /// </summary>
        /// <param name="reps">Repositories collection</param>
        /// <returns>All procedures script</returns>
        private string CollectorScriptProcedures(List<IRepository> reps)
        {
            List<string> scripts = new List<string>();

            foreach (var rep in reps)
            {
                scripts.AddRange(GetProceduresScript(rep));
            }

            return _scriptDesigner.CreateProceduresOrTriggersScript(scripts);
        }

        /// <summary>
        /// Get the script for all procedures from the database
        /// </summary>
        /// <param name="rep">Processed provider</param>
        /// <returns>The generated script</returns>
        private List<string> GetProceduresScript(IRepository rep)
        {
            var proceduresNames = rep.GetProcedures();
            List<string> procedures = new List<string>();

            foreach (var procedureName in proceduresNames)
            {
                string script = rep.GetProcedureSqript(procedureName);
                procedures.Add(script);
            }

            return procedures;
        }

        /// <summary>
        /// Building a triggers script
        /// </summary>
        /// <param name="reps"></param>
        /// <returns>All triggers script</returns>
        private string CollectorScriptTriggers(List<IRepository> reps)
        {
            List<string> scripts = new List<string>();

            foreach (var rep in reps)
            {
                scripts.AddRange(GetTriggersScript(rep));
            }

            return _scriptDesigner.CreateProceduresOrTriggersScript(scripts);
        }

        /// <summary>
        /// Get the script for all triggers from the database
        /// </summary>
        /// <param name="rep">Processed provider</param>
        /// <returns>The generated script</returns>
        private List<string> GetTriggersScript(IRepository rep)
        {
            var triggersNames = rep.GetTriggers();
            List<string> triggers = new List<string>();

            foreach (var triggerName in triggersNames)
            {
                string script = rep.GetTriggerSqript(triggerName);
                triggers.Add(script);
            }

            return triggers;
        }

        /// <summary>
        /// Returns the object of the collector tables
        /// </summary>
        /// <param name="rep">Processed provider</param>
        private void GetCreator(Provider rep)
        {
            if (rep == Provider.MicrosoftSql)
            {
                _tableCreator = new MsSqlTableCreator();
            }
            else if (rep == Provider.MySql)
            {
                _tableCreator = new MySqlTableCreator();
            }
            else if (rep == Provider.SyBase)
            {
                _tableCreator = new SyBaseTableCreator();
            }
        }

        /// <summary>
        /// Returns the script designer object
        /// </summary>
        /// <param name="rep">Processed provider</param>
        private void GetDesigner(Provider rep)
        {
            if (rep == Provider.MicrosoftSql)
            {
                _scriptDesigner = new MsSqlDbScriptDesigner();
            }
            else if (rep == Provider.MySql)
            {
                _scriptDesigner = new MySqlDbScriptDesigner();
            }
            else if (rep == Provider.SyBase)
            {
                _scriptDesigner = new SyBaseDbScriptDesigner();
            }
        }
    }
}
