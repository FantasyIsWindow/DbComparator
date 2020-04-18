using Comparator.Repositories.Repositories;
using DbComparator.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbComparator.App.Services
{
    internal class ScriptBuilder
    {
        private StringBuilder _script;

        public ScriptBuilder()
        {
            _script = new StringBuilder();
        }

        public void CreateScript(IRepository repository)
        {
            var proceduresScripts = GetProceduresScript(repository);
            var triggersScripts = GetTriggersScript(repository);

            GluingScripts(proceduresScripts);
            GluingScripts(triggersScripts);
        }

        private List<string> GetProceduresScript(IRepository repository)
        {
            var proceduresNames = repository.GetProcedures().ToList();
            List<string> proceduresScripts = new List<string>();

            foreach (var name in proceduresNames)
            {
                string procedureScript = repository.GetProcedureSqript(name);
                proceduresScripts.Add(procedureScript);
            }
            return proceduresScripts;
        }

        private List<string> GetTriggersScript(IRepository repository)
        {
            var triggersNames = repository.GetTriggers().ToList();
            List<string> triggersScripts = new List<string>();
            foreach (var name in triggersNames)
            {
                string triggerScript = repository.GetTriggerSqript(name);
                triggersScripts.Add(triggerScript);
            }
            return triggersScripts;
        }

        private void GluingScripts(List<string> scripts)
        {
            foreach (var script in scripts)
            {
                _script.Append(script + ";\n");
            }
        }
    }
}
