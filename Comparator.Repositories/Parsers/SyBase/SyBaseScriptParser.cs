﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Parsers.SyBase
{
    internal class SyBaseScriptParser : ScriptParser
    {
        public SyBaseScriptParser() 
            : base(new SyBaseReservedKeyword()) { }

        protected override string ParseScript(string script)
        {
            bool isAs = false;
            bool isBegin = false;
            bool isBeginD = false;
            var fragmentedQuery = StringToArr(script);

            for (int i = 0; i < fragmentedQuery.Count; i++)
            {
                if (_findWord.IsExists(fragmentedQuery[i]))
                {
                    string word = fragmentedQuery[i].ToUpper();

                    if (!isAs && word == "AS" && !isBeginD)
                    {
                        fragmentedQuery[i] = "\nAS";
                        isAs = true;
                        if (fragmentedQuery[i + 1].ToUpper() != "BEGIN")
                        {
                            isBegin = true;
                            fragmentedQuery.Insert(i + 1, "\nBEGIN");
                        }
                        continue;
                    }

                    if (word == "BEGIN")
                    {
                        fragmentedQuery[i] = "\n" + word;
                        isBeginD = true;
                        continue;
                    }
                    
                    if (word == "END" && i != fragmentedQuery.Count - 1)
                    {
                        fragmentedQuery[i] = !_findWord.IsTab(fragmentedQuery[i + 1]) ? 
                            "\n" + word + "\n" : 
                            "\n" + word;
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

                if(fragmentedQuery[i].EndsWith(";") && !_findWord.IsExists(fragmentedQuery[i + 1]))
                {
                    string qwe = fragmentedQuery[i] + "\n";
                    fragmentedQuery[i] = qwe;
                }
            }

            if (isBegin)
            {
                fragmentedQuery.Add("\nEND;");
            }
            else if (!fragmentedQuery.Last().EndsWith(";"))
            {
                fragmentedQuery[fragmentedQuery.Count - 1] = "\n" + fragmentedQuery.Last() + ";";
            }

            var str = string.Join(" ", fragmentedQuery);

            return str;
        }

        protected override List<string> StringToArr(string script)
        {
            string preparedLine = script.Replace(",", ", ").Replace(";", " ; ").Replace("(", " ( ").Replace(")", " ) ").Replace("\"", "");
            return preparedLine.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
