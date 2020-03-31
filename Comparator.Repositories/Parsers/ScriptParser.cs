using Comparator.Repositories.Extensions;
using Comparator.Repositories.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comparator.Repositories.Parsers
{
    internal class ScriptParser
    {
        private ReservedKeyword _findWord;

        private bool isBeginAdded;

        private bool isBegin;

        public ScriptParser()
        {
            _findWord = new ReservedKeyword();
        }

        public string GetProcedureSquript(IEnumerable<string> str) =>
            str.NotNull() ? ParseScript(str.EnamerableToString()) : null;

        public string GetProcedureSquript(string str) =>
            str != null ? ParseScript(str) : null;

        public string GetTriggerSqript(IEnumerable<MySqlTriggerSqript> triggers)
        {
            foreach (var trigger in triggers)
            {
                string str = $"CREATE TRIGGER '{trigger.TRIGGER_NAME}' {trigger.ACTION_TIMING} {trigger.EVENT_MANIPULATION} ON '{trigger.EVENT_OBJECT_TABLE}'" +
                    $"FOR EACH {trigger.EVENT_OBJECT_TABLE} {trigger.ACTION_ORIENTATION } {trigger.ACTION_STATEMENT}";
                return ParseScript(str);
            }
            return null;
        }

        private string ParseScript(string script)
        {
            var arr = StringArrGenerate(script);
            isBeginAdded = false;
            isBegin = false;

            StringBuilder newScript = new StringBuilder();

            for (int i = 0; i < arr.Length; i++)
            {
                string word = arr[i];

                if (_findWord.IsExists(word))
                {
                    var resultWord = ProcessingReservedWord(arr, i);
                    newScript.Append(resultWord);
                }
                else if (i == arr.Length - 1 && word != "END" && isBeginAdded)
                {
                    newScript.Append(word + " \r\nEND\r\n\t");
                }
                else if (word.StartsWith("@") && isBeginAdded)
                {
                    newScript.Append("\t" + word + ' ');
                }
                else if (word == ";")
                {
                    newScript.Append(word + "\r\n\t");
                }
                else
                {
                    newScript.Append(word + ' ');
                }
            }
            return newScript.ToString();
        }

        private string[] StringArrGenerate(string script)
        {
            StringBuilder builder1 = new StringBuilder();

            for (int i = 0; i < script.Length; i++)
            {
                if (script[i] == '(' || script[i] == ')' || script[i] == ';')
                {
                    builder1.Append(" " + script[i] + " ");
                }
                else
                {
                    builder1.Append(script[i]);
                }
            }

            var tempStr = builder1.ToString();
            return tempStr.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private string ProcessingReservedWord(string[] arr, int index)
        {
            string word = arr[index].ToUpper();

            if (word == "AS" && arr[index + 1].ToUpper() != "BEGIN")
            {
                isBegin = true;
                isBeginAdded = true;
                return word + " \r\nBEGIN\r\n\t";
            }
            else if (word == "BEGIN" || word == "END")
            {
                isBegin = true;
                return "\r\n" + word + "\r\n\t";
            }
            else if (_findWord.IsTab(word) && arr[index - 1] != "(")
            {
                if (isBegin)
                {
                    isBegin = false;
                    return word + ' ';
                }
                else
                {
                    return "\r\n\t" + word + ' ';
                }
            }
            else
            {
                return word + ' ';
            }
        }
    }
}
