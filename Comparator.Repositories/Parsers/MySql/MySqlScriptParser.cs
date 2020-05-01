using System;
using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Parsers.MySql
{
    internal class MySqlScriptParser : ScriptParser
    {
        public MySqlScriptParser()
            : base(new MySqlReservedKeyword()) { }

        protected override string ParseScript(string script)
        {
            var fragmentedQuery = StringToArr(script);
            bool asIs = false;
            bool isAdd = false;
            int count = 0;

            for (int i = 0; i < fragmentedQuery.Count; i++)
            {
                if (!isAdd && !asIs && fragmentedQuery[i] == "(" || fragmentedQuery[i] == ")")
                {
                    count += fragmentedQuery[i] == "(" ? 1 : -1;

                    if (count == 0 && fragmentedQuery[i + 1].ToUpper() != "BEGIN")
                    {
                        fragmentedQuery.Insert(i + 1, "\nBEGIN\n");
                        isAdd = true;
                    }
                    else if (fragmentedQuery[i + 1].ToUpper() == "BEGIN")
                    {
                        asIs = true;
                    }
                    continue;
                }

                if (_findWord.IsExists(fragmentedQuery[i]))
                {
                    string word = fragmentedQuery[i].ToUpper();

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

            if (isAdd)
            {
                fragmentedQuery.Add("\nEND;");
            }
            else if (!fragmentedQuery.Last().EndsWith(";"))
            {
                fragmentedQuery[fragmentedQuery.Count - 1] = fragmentedQuery.Last() + ";";
            }

            return string.Join(" ", fragmentedQuery);
        }

        protected override List<string> StringToArr(string script)
        {
            string preparedLine = script.Replace(",", ", ").Replace(";", " ; ").Replace("(", " ( ").Replace(")", " ) ").Replace("`", "").Replace("'", " ");

            var result = preparedLine.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].Contains("DEFINER"))
                {
                    result.RemoveAt(i);
                }
            }

            return result;
        }

        public string GetScript(string script)
        {
            return "DELIMITER //\n" + script + "//\n DELIMITER;\n\n";
        }
    }
}


/*
 
CREATE TABLE test (
`id` INT( 11 ) UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
`content` TEXT NOT NULL
) ENGINE = InnoDB;


CREATE TABLE log (
`id` INT( 11 ) UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY,
`msg` VARCHAR( 255 ) NOT NULL,
`time` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
`row_id` INT( 11 ) NOT NULL
) ENGINE = InnoDB;

DELIMITER |


DELIMITER //

CREATE TRIGGER `update_test` AFTER INSERT ON `test`
FOR EACH ROW BEGIN
   INSERT INTO log Set msg = 'insert', row_id = NEW.id;
END; 
DELIMITER

     */
