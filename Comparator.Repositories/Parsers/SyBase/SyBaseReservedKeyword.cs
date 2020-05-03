using System.Collections.Generic;

namespace Comparator.Repositories.Parsers.SyBase
{
    internal class SyBaseReservedKeyword : ReservedKeywordLibrary
    {
        public SyBaseReservedKeyword()
        {
            _reservedWords = new HashSet<string>()
            {
                "CREATE",
                "PROCEDURE",
                "AS",
                "BEGIN",
                "END",
                "SET",
                "NOCOUNT",
                "ON",
                "SELECT",
                "LEFT",
                "RIGHT",
                "OUTER",
                "CASE",
                "WHEN",
                "THEN",
                "ELSE",
                "AVAILABILITY",
                "FROM",
                "INNER",
                "JOIN",
                "WHERE",
                "ORDER",
                "BY",
                "LIKE",
                "ASC",
                "IF",
                "INSERT",
                "VALUES",
                "INTO",
                "RESULT",
                "EXCEPTION",
                "RESUME",
                "AND",
                "DECLARE",
                "FOR",
                "SQLSTATE",
                "VALUE",
                "DYNAMEC",
                "SCROLL",
                "CURSOR",
                "GROUP",
                "LOOP",
                "CLOSE",
                "OUT",
                "OUTER",
                "SUM",
                "INNER",
                "JOIN",
                "OPEN",
                "IN",
                "CAST",
            };

            _tab = new HashSet<string>()
            {
                "SET",
                "CASE",
                "WHEN",
                "ELSE",
                "LEFT",
                "RIGHT",
                "GROUP",
                "ORDER",
                "VALUES",
                "HAVING",
                "DECLARE",
                "WHERE",
                "INNER",
                "INSERT",
                "IF",
                "RETURNS",
                "DELETE",
                "RETURN"
            };

            _newLine = new HashSet<string>()
            {
                "CREATE"
            };
        }
    }
}