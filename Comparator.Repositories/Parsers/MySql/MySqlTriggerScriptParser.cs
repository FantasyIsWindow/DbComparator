using Comparator.Repositories.Models.DbModels;
using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Parsers.MySql
{
    internal class MySqlTriggerScriptParser : MySqlScriptParser
    {
        /// <summary>
        /// Returns the trigger script
        /// </summary>
        /// <param name="triggers">Trigger string</param>
        /// <returns>Procedure or trigger script</returns>
        public string GetTriggerSqript(IEnumerable<MySqlTriggerSqript> triggers)
        {
            foreach (var trigger in triggers)
            {
                string str = $"CREATE TRIGGER `{trigger.TRIGGER_NAME}` {trigger.ACTION_TIMING} {trigger.EVENT_MANIPULATION} ON `{trigger.EVENT_OBJECT_TABLE}`" +
                    $"FOR EACH {trigger.ACTION_ORIENTATION } {trigger.ACTION_STATEMENT}";
                return ParseScript(str);
            }
            return null;
        }

        /// <summary>
        /// Script bulkhead
        /// </summary>
        /// <param name="script">The script being processed</param>
        /// <returns></returns>
        protected override string ParseScript(string script)
        {
            var fragmentedQuery = StringToArr(script);
            bool isBegin = false;

            for (int i = 0; i < fragmentedQuery.Count; i++)
            {

                if (_findWord.IsExists(fragmentedQuery[i]))
                {
                    string word = fragmentedQuery[i].ToUpper();

                    if (!isBegin && word == "ROW" && fragmentedQuery[i - 1].ToUpper() == "EACH" && fragmentedQuery[i + 1].ToUpper() != "BEGIN")
                    {
                        isBegin = true;
                        fragmentedQuery.Insert(i + 1, "\nBEGIN");
                    }

                    if (word == "BEGIN")
                    {
                        fragmentedQuery[i] = "\n" + word;
                        continue;
                    }

                    if (_findWord.IsTab(word))
                    {
                        fragmentedQuery[i] = "\n\t" + word;
                        continue;
                    }

                    if (_findWord.IsNewLine(word) && i != 0)
                    {
                        fragmentedQuery[i] = "\n" + word;
                        continue;
                    }

                    fragmentedQuery[i] = word;
                }

                if (fragmentedQuery[i] == ";")
                {
                    fragmentedQuery[i] = _findWord.IsTab(fragmentedQuery[i + 1]) ?
                        fragmentedQuery[i] :
                        fragmentedQuery[i] + "\n";
                }
            }

            if (isBegin)
            {
                fragmentedQuery.Add("\nEND;");
            }
            else if (!fragmentedQuery.Last().EndsWith(";"))
            {
                fragmentedQuery[fragmentedQuery.Count - 1] = fragmentedQuery.Last() + ";";
            }

            return string.Join(" ", fragmentedQuery);
        }
    }
}
