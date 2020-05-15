using System.Collections.Generic;

namespace Comparator.Repositories.Parsers
{
    public interface IDatabaseScriptDesigner
    {
        /// <summary>
        /// Get the script for the entire database
        /// </summary>
        /// <param name="tables">Tables script</param>
        /// <param name="procedures">Procedures script</param>
        /// <param name="triggers">Triggers script</param>
        /// <param name="dbName">Database name</param>
        /// <returns>Database script</returns>
        string CreateFullDbScript(List<string> tables, List<string> constraints, List<string> procedures, List<string> triggers, string dbNam);

        /// <summary>
        /// Get the script for the entire tables
        /// </summary>
        /// <param name="tables">A dictionary containing a list of table data</param>
        /// <returns>Tables script</returns>
        string CreateTablesScript(List<string> tables, List<string> constraints);
        
        /// <summary>
        /// Get the script for the entire procedures or triggers
        /// </summary>
        /// <param name="scripts">A list containing scripts for procedures or triggers</param>
        /// <returns>Procedures or triggers script</returns>
        string CreateProceduresOrTriggersScript(List<string> scripts);
    }
}
