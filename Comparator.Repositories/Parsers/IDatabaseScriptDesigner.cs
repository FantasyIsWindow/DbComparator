using System.Collections.Generic;

namespace Comparator.Repositories.Parsers
{
    public interface IDatabaseScriptDesigner
    {
        string CreateFullDbScript(string tables, string procedures, string triggers, string dbName);

        string CreateTablesScript(List<string> tables, List<string> constraints);

        string CreateProceduresOrTriggersScript(List<string> scripts);
    }
}
