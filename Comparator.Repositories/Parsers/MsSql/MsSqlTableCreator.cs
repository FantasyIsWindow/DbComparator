using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Text;

namespace Comparator.Repositories.Parsers.MsSql
{
    internal class MsSqlTableCreator : ITableCreator
    {
        private StringBuilder _foreignKeys;

        /// <summary>
        /// Get a table script based on the received data
        /// </summary>
        /// <param name="info">A list with the metadata of the table</param>
        /// <param name="tableName">Table name</param>
        /// <param name="foreignKeys">Script foreign keys</param>
        /// <returns>Table script</returns>
        public string GetTableScript(List<DtoFullField> info, string tableName, StringBuilder foreignKeys)
        {
            _foreignKeys = foreignKeys;
            return ScriptCreate(info, tableName);
        }

        /// <summary>
        /// The function build table script
        /// </summary>
        /// <param name="info">A list with the metadata of the table</param>
        /// <param name="tableName">Table name</param>
        /// <returns>The generated script</returns>
        private string ScriptCreate(List<DtoFullField> info, string tableName)
        {
            StringBuilder builder = new StringBuilder($"CREATE TABLE {tableName} (");
            StringBuilder constaintsBuilder = new StringBuilder();

            for (int i = 0; i < info.Count; i++)
            {
                string fieldName = info[i].FieldName != "" ? $"{info[i].FieldName}" : "";
                string typeName = info[i].TypeName != "" ? $" {info[i].TypeName.ToUpper()}" : "";
                string size = info[i].Size != "" ? $" ({info[i].Size})" : "";
                string isNull = info[i].IsNullable != "" ? info[i].IsNullable != "NO" ? " NULL" : " NOT NULL" : "";
                string constType = info[i].ConstraintType != "" ? " " + info[i].ConstraintType.Replace("(", "").Replace(")", "").Replace("-", "").ToUpper() : "";
                string constName = info[i].ConstraintName != "" ? $" {info[i].ConstraintName}" : "";
                string constKeys = info[i].ConstraintKeys != "" ? $" {info[i].ConstraintKeys.Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "")}" : "";
                string referenced = info[i].Referenced != "" ? $" {info[i].Referenced}" : "";
                string onUpdate = info[i].OnUpdate != "" && info[i].OnUpdate != "(n/a)" ? $" ON UPDATE {info[i].OnUpdate.ToUpper()}" : "";
                string onDelete = info[i].OnDelete != "" && info[i].OnDelete != "(n/a)" ? $" ON DELETE {info[i].OnDelete.ToUpper()}" : "";

                if (fieldName != "")
                {
                    builder.Append($"\n\t{fieldName}{typeName}{size}{isNull},");
                }
                else
                {
                    fieldName = FindFieldName(info, i);
                }

                if (constType != "")
                {
                    if (constType.Contains("FOREIGN"))
                    {
                        _foreignKeys.Append($"\nALTER TABLE {tableName} ADD CONSTRAINT{constName}{constType} ({fieldName}) REFERENCES{referenced}{onUpdate}{onDelete};");
                    }
                    else if (info[i].ConstraintType == "DEFAULT")
                    {
                        builder.Remove(builder.Length - 1, 1);
                        builder.Append($" CONSTRAINT{constName}{constType}{IsDigit(constKeys)},");
                    }
                    else if (info[i].ConstraintType == "CHECK")
                    {
                        constaintsBuilder.Append($"\n\tCONSTRAINT{constName}{constType} {IsDigit(constKeys)},");
                    }
                    else
                    {
                        constaintsBuilder.Append($"\n\tCONSTRAINT{constName}{constType} ({fieldName}),");
                    }
                }
            }

            builder.Append(constaintsBuilder.ToString());

            builder.Remove(builder.Length - 1, 1);

            builder.Append($"\n);");

            return builder.ToString();
        }

        /// <summary>
        /// Searches for the field name
        /// </summary>
        /// <param name="fieldsInfo">A list with the metadata of the table</param>
        /// <param name="currentIndex">Current Index</param>
        /// <returns>Field name</returns>
        private string FindFieldName(List<DtoFullField> fieldsInfo, int currentIndex)
        {
            if (currentIndex == 0)
            {
                return "";
            }

            return fieldsInfo[currentIndex].FieldName != "" ? fieldsInfo[currentIndex].FieldName : FindFieldName(fieldsInfo, currentIndex - 1);
        }

        /// <summary>
        /// Formatting the resulting string depending on the type of data contained
        /// </summary>
        /// <param name="str"></param>
        /// <returns>A formatted string</returns>
        private string IsDigit(string str)
        {
            MsSqlReservedKeyword reservedKeyword = new MsSqlReservedKeyword();
            int x;
            if (int.TryParse(str, out x) || reservedKeyword.IsExists(str))
            {
                return $"{str}";
            }
            else
            {
                return $"({str.Trim()})";
            }
        }
    }
}
