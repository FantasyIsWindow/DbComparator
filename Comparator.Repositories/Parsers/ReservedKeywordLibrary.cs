using System.Collections.Generic;

namespace Comparator.Repositories.Parsers
{
    internal class ReservedKeywordLibrary
    {
        protected HashSet<string> _reservedWords;

        protected HashSet<string> _tab;

        protected HashSet<string> _newLine;

        /// <summary>
        /// Checks whether such a word exists in the list of reserved words
        /// </summary>
        /// <param name="word">Search word</param>
        /// <returns>True if there is a word in the reference list</returns>
        public bool IsExists(string word) =>
            _reservedWords.Contains(word.ToUpper());

        /// <summary>
        /// Checks whether such a word exists in the list of reserved tab words
        /// </summary>
        /// <param name="word">Search word</param>
        /// <returns>True if there is a word in the reference list</returns>
        public bool IsTab(string word) =>
            _tab.Contains(word);

        /// <summary>
        /// Checks whether such a word exists in the list of reserved newLine words
        /// </summary>
        /// <param name="word">Search word</param>
        /// <returns>True if there is a word in the reference list</returns>
        public bool IsNewLine(string word) =>
            _newLine.Contains(word);
    }
}
