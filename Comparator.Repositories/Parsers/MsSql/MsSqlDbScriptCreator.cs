using System.Collections.Generic;
using System.Text;

namespace Comparator.Repositories.Parsers.MsSql
{
    internal class MsSqlDbScriptCreator : IDatabaseScriptDesigner
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
        public string CreateTablesScript(List<string> tables, List<string> constraints)
        {
            StringBuilder dbScript = new StringBuilder();
            foreach (var table in tables)
            {
                dbScript.Append(table + "\n\n");
            }

            foreach (var constraint in constraints)
            {
                dbScript.Append(constraint + "\n");
            }

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
