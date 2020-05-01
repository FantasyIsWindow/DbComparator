using System.Collections.Generic;
using System.Text;

namespace Comparator.Repositories.Parsers.MySql
{
    internal class MySqlDbScriptCreator
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

            dbScript.Append(tables);
            dbScript.Append("DELIMITER //");
            dbScript.Append(procedures);
            dbScript.Append(triggers);
            dbScript.Append("\n//");

            return dbScript.ToString();
        }

        /// <summary>
        /// Get the script for the entire tables
        /// </summary>
        /// <param name="tables">A dictionary containing a list of table data</param>
        /// <returns>Tables script</returns>
        public string CreateTablesScript(Dictionary<string, string> tables)
        {
            StringBuilder dbScript = new StringBuilder("SET foreign_key_checks = 0;\n\n");

            foreach (var table in tables)
            {
                dbScript.Append($"DROP TABLE IF EXISTS `{table.Key}`;\n");
                dbScript.Append($"{table.Value};\n\n");
            }

            dbScript.Append("SET foreign_key_checks = 1;\n\n");
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
                dbScript.Append("DELIMITER //\n\n" + procedure + "\n\nDELIMITER;\n\n");
            }
            return dbScript.ToString();
        }
    }
}
