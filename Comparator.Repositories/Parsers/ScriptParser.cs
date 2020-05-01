using Comparator.Repositories.Extensions;
using System.Collections.Generic;

namespace Comparator.Repositories.Parsers
{
    internal class ScriptParser
    {
        protected ReservedKeywordLibrary _findWord;

        public ScriptParser(ReservedKeywordLibrary library)
        {
            _findWord = library;
        }

        /// <summary>
        /// Returns the procedure or trigger script
        /// </summary>
        /// <param name="str">Collection of procedure or trigger strings</param>
        /// <returns>Procedure or trigger script</returns>
        public string GetSquript(IEnumerable<string> str) =>
            str.NotNull() ? ParseScript(str.EnumerableToString()) : null;

        /// <summary>
        /// Returns the procedure or trigger script
        /// </summary>
        /// <param name="str">Procedure or trigger string</param>
        /// <returns>Procedure or trigger script</returns>
        public string GetSquript(string str) =>
            str != null ? ParseScript(str) : null;

        /// <summary>
        /// Returns a formatted script
        /// </summary>
        /// <param name="script">Row string</param>
        /// <returns>Formatted script</returns>
        protected virtual string ParseScript(string script) => null;

        /// <summary>
        /// Returns an array of words divided by a string of words
        /// </summary>
        /// <param name="script">Row string</param>
        /// <returns>Broken according to the line</returns>
        protected virtual List<string> StringToArr(string script) => null;
    }
}
