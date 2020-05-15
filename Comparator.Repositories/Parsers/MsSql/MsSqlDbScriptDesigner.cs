using System.Collections.Generic;
using System.Text;

namespace Comparator.Repositories.Parsers.MsSql
{
    internal class MsSqlDbScriptDesigner : IDatabaseScriptDesigner
    {
        public string CreateFullDbScript(List<string> tables, List<string> constraints, List<string> procedures, List<string> triggers, string dbName)
        {
            StringBuilder dbScript = new StringBuilder();

            dbScript.Append($"CREATE DATABASE {dbName};\nGO\n\nUSE {dbName};\n\n");

            dbScript.Append(CreateTablesScript(tables, constraints) + "\n");
            dbScript.Append(CreateProceduresOrTriggersScript(procedures));
            dbScript.Append(CreateProceduresOrTriggersScript(triggers));

            return dbScript.ToString();
        }

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
