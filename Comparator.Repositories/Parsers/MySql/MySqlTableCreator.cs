using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Text;

namespace Comparator.Repositories.Parsers.MySql
{
    internal class MySqlTableCreator : ITableCreator
    {
        /// <summary>
        /// Get a table script based on the received data
        /// </summary>
        /// <param name="info">A list with the metadata of the table</param>
        /// <param name="tableName">Table name</param>
        /// <param name="foreignKeys">Script foreign keys</param>
        /// <returns>Table script</returns>
        public string GetTableScript(List<DtoFullField> info, string tableName, StringBuilder foreignKeys)
        {
            return ScriptCreate(info, tableName);
        }

        private string ScriptCreate(List<DtoFullField> info, string tableName)
        {
            StringBuilder builder = new StringBuilder("SET foreign_key_checks = 0;\n\n");

            builder.Append($"DROP TABLE IF EXISTS `{tableName}`;");

            for (int i = 0; i < info.Count; i++)
            {

            }

            return "";
        }

        private string IsDigit(string str)
        {
            MySqlReservedKeyword reservedKeyword = new MySqlReservedKeyword();
            int x;
            if(int.TryParse(str, out x))
            {
                return $"DEFAULT {str}";
            }
            else if(reservedKeyword.IsExists(str))
            {
                return $"DEFAULT {str}";
            }
            else
            {
                return $"DEFAULT '{str}'";
            }
        }

    }
}

