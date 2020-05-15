using System.Collections.Generic;
using System.Text;

namespace Comparator.Repositories.Parsers.MySql
{
    internal class MySqlDbScriptDesigner : IDatabaseScriptDesigner
    {
        public string CreateFullDbScript(List<string> tables, List<string> constraints, List<string> procedures, List<string> triggers, string dbName)
        {
            StringBuilder dbScript = new StringBuilder();

            dbScript.Append(CreateTablesScript(tables, constraints) + "\n");
            dbScript.Append(CreateProceduresOrTriggersScript(procedures));
            dbScript.Append(CreateProceduresOrTriggersScript(triggers));

            return dbScript.ToString();
        }

        public string CreateTablesScript(List<string> tables, List<string> constraints)
        {
            StringBuilder dbScript = new StringBuilder("SET foreign_key_checks = 0;\n\n");

            foreach (var table in tables)
            {
                dbScript.Append(table + "\n\n");
            }

            foreach (var constraint in constraints)
            {
                dbScript.Append(constraint + "\n");
            }

            dbScript.Append("SET foreign_key_checks = 1;\n\n");
            return dbScript.ToString();
        }

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
