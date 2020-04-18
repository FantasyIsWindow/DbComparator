using Comparator.Repositories.Extensions;
using Comparator.Repositories.Models.DbModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Comparator.Repositories.Parsers
{
    internal class ScriptParser
    {
        private readonly ReservedKeyword _findWord;

        private bool isBeginAdded;

        private bool isBegin;

        public ScriptParser()
        {
            _findWord = new ReservedKeyword();
        }

        /// <summary>
        /// Returns the procedure or trigger script
        /// </summary>
        /// <param name="str">Collection of procedure or trigger strings</param>
        /// <returns>Procedure or trigger script</returns>
        public string GetProcedureSquript(IEnumerable<string> str) =>
            str.NotNull() ? ParseScript(str.EnumerableToString()) : null;

        /// <summary>
        /// Returns the procedure or trigger script
        /// </summary>
        /// <param name="str">Procedure or trigger string</param>
        /// <returns>Procedure or trigger script</returns>
        public string GetProcedureSquript(string str) =>
            str != null ? ParseScript(str) : null;

        /// <summary>
        /// Returns the trigger script
        /// </summary>
        /// <param name="triggers">Trigger string</param>
        /// <returns>Procedure or trigger script</returns>
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

        /// <summary>
        /// Returns a formatted script
        /// </summary>
        /// <param name="script">Row string</param>
        /// <returns>Formatted script</returns>
        private string ParseScript(string script)
        {
            var arr = StringToArr(script);
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
                    newScript.Append(word + " \nEND\n\t");
                }
                else if (word.StartsWith("@") && isBeginAdded)
                {
                    newScript.Append("\t" + word + ' ');
                }
                else if (word == ";")
                {
                    newScript.Append(word + "\n\t");
                }
                else
                {
                    newScript.Append(word + ' ');
                }
            }
            return newScript.ToString();
        }

        /// <summary>
        /// Returns an array of words divided by a string of words
        /// </summary>
        /// <param name="script">Row string</param>
        /// <returns>Broken according to the line</returns>
        private string[] StringToArr(string script)
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

        /// <summary>
        /// Returns the processed reserved word
        /// </summary>
        /// <param name="arr">Array of words divided by a string of words</param>
        /// <param name="index">The current index of the array</param>
        /// <returns>Processed reserved word</returns>
        private string ProcessingReservedWord(string[] arr, int index)
        {
            string word = arr[index].ToUpper();

            if (word == "AS" && arr[index + 1].ToUpper() != "BEGIN")
            {
                isBegin = true;
                isBeginAdded = true;
                return word + " \nBEGIN\n\t";
            }
            else if (word == "BEGIN" || word == "END")
            {
                isBegin = true;
                return "\n" + word + "\n\t";
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
                    return "\n\t" + word + ' ';
                }
            }
            else
            {
                return word + ' ';
            }
        }
    }
}
