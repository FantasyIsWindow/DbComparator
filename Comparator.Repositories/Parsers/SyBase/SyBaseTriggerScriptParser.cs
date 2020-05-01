using System.Collections.Generic;

namespace Comparator.Repositories.Parsers.SyBase
{
    internal class SyBaseTriggerScriptParser : SyBaseScriptParser
    {
        /// <summary>
        /// Get the trigger script
        /// </summary>
        /// <param name="script">Collection with information about the script</param>
        /// <returns>Trigger script</returns>
        public string GetTriggerParser(IEnumerable<string> script) => 
            GetSquript(script);
    }
}
