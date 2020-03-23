using Comparator.Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comparator.Repositories.Parsers
{
    internal class ProcedureScriptParser
    {
        private ReservedKeyword _findWord;

        public ProcedureScriptParser()
        {
            _findWord = new ReservedKeyword();
        }

        public string GetProcedureSquript(IEnumerable<string> str)
        {
            if (str.IsNull())
            {
                var result = ArrayToString(str);
                return GeSquript(ArrayToString(str));
            }
            return null;
        }

        public string GetProcedureSquript(string str) =>
            str != null ? GeSquript(str) : null;        
        
        public string GeSquript(string squript)
        {
            StringBuilder builder1 = new StringBuilder();

            for (int i = 0; i < squript.Length; i++)
            {
                if (squript[i] == '(' || squript[i] == ')')
                {
                    builder1.Append(" " + squript[i] + " ");
                }
                else
                {
                    builder1.Append(squript[i]);
                }
            }

            squript = builder1.ToString();

            var arr = squript.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder builder = new StringBuilder();
            bool flag = false;
            bool nst = false;

            for (int i = 0; i < arr.Length; i++)
            {
                string word = arr[i];

                if (_findWord.IsExists(word.ToUpper()))
                {
                    string reserved = arr[i].ToUpper();
                    if (reserved == "AS" && arr[i + 1].ToUpper() != "BEGIN")
                    {
                        builder.Append(reserved + " \r\nBEGIN\r\n\t");
                        nst = true;
                        flag = true;
                    }
                    else if (reserved == "BEGIN" || reserved == "END")
                    {
                        builder.Append("\r\n" + reserved + "\r\n\t");
                        nst = true;
                    }
                    else if (_findWord.IsTab(word))
                    {
                        if (nst)
                        {
                            builder.Append(reserved + ' ');
                            nst = false;
                        }
                        else
                        {
                            builder.Append("\r\n\t" + reserved + ' ');
                        }
                    }
                    else
                    {
                        builder.Append(reserved + ' ');
                    }
                }
                else if (i == arr.Length - 1 && word != "END" && flag)
                {
                    builder.Append(word + " \r\nEND\r\n\t");
                }
                else if (word.StartsWith("@") && flag)
                {
                    builder.Append("\t" + word + ' ');
                }
                else
                {
                    builder.Append(word + ' ');
                }
            }

            return builder.ToString();
        }

        private string ArrayToString(IEnumerable<string> str) =>
            string.Join(" ", str.ToArray());
    }
}