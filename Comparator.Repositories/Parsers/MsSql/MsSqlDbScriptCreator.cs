using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Text;

namespace Comparator.Repositories.Parsers.MsSql
{
    internal class MsSqlDbScriptCreator
    {
        /// <summary>
        /// Get the script for the entire database
        /// </summary>
        /// <param name="tables">Tables script</param>
        /// <param name="procedures">Procedures script</param>
        /// <param name="triggers">Triggers script</param>
        /// <param name="dbName">Database name</param>
        /// <returns>Database script</returns>
        public string CreateFullDbScript(string tables, string procedures, string triggers, string dbName)
        {
            StringBuilder dbScript = new StringBuilder();

            dbScript.Append($"CREATE DATABASE {dbName};\nGO\n\nUSE {dbName};\n\n");

            dbScript.Append(tables);
            dbScript.Append(procedures);
            dbScript.Append(triggers);

            return dbScript.ToString();
        }

        /// <summary>
        /// Get the script for the entire tables
        /// </summary>
        /// <param name="tables">A dictionary containing a list of table data</param>
        /// <returns>Tables script</returns>
        public string CreateTablesScript(Dictionary<string, List<DtoFullField>> tables)
        {
            MsSqlTableCreator tableCreator = new MsSqlTableCreator();

            StringBuilder dbScript = new StringBuilder();
            StringBuilder foreignKeys = new StringBuilder();

            foreach (var table in tables)
            {
                var tableScript = tableCreator.GetTableScript(table.Value, table.Key, foreignKeys);
                dbScript.Append(tableScript + "\n\n");
            }
            dbScript.Append((foreignKeys));
            return dbScript.ToString();
        }

        /// <summary>
        /// Get the script for the entire procedures or triggers
        /// </summary>
        /// <param name="scripts">A list containing scripts for procedures or triggers</param>
        /// <returns>Procedures or triggers script</returns>
        public string CreateProceduresOrTriggersScript(List<string> scripts)
        {
            StringBuilder dbScript = new StringBuilder();

            foreach (var procedure in scripts)
            {
                dbScript.Append(procedure + "\n\nGO\n\n");
            }
            return dbScript.ToString();
        }
    }
}
