using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System;

namespace Comparator.Repositories.Parsers
{
    internal class SyBaseFieldsInfoParser
    {
        /// <summary>
        /// Returns a collection of table fields
        /// </summary>
        /// <param name="fields">List of raw fields</param>
        /// <param name="constraints">List of raw constraints</param>
        /// <returns>The filtered list of fields of the table</returns>
        public IEnumerable<DtoFullField> GetFieldsCollection(IEnumerable<SyBaseFieldsModel> fields, IEnumerable<DtoSyBaseConstaintsModel> constraints)
        {
            List<DtoFullField> tempFieldsCollection = FieldsCollectionPreparation(fields);
            List<DtoFullField> tempConstraints = ConstraintsCollectionPreparation(tempFieldsCollection, constraints);
            List<DtoFullField> resultFullFieldsCollection = BuildingCollectionOfFields(tempFieldsCollection, tempConstraints);

            NullValueToEmptyString(resultFullFieldsCollection);
            return resultFullFieldsCollection;
        }

        /// <summary>
        /// Returns a prepared list of fields
        /// </summary>
        /// <param name="fields">List of raw fields</param>
        /// <returns>Prepared list of fields</returns>
        private List<DtoFullField> FieldsCollectionPreparation(IEnumerable<SyBaseFieldsModel> fields)
        {
            List<DtoFullField> tempCollection = new List<DtoFullField>();

            foreach (var item in fields)
            {
                DtoFullField field = new DtoFullField()
                {
                    TypeName = GetType(item.base_type_str),
                    IsNullable = item.nulls,
                    FieldName = item.column_name,
                    Size = GetSize(item.base_type_str)
                };

                tempCollection.Add(field);
            }

            return tempCollection;
        }

        /// <summary>
        /// Returns a prepared collection of restrictions and fields
        /// </summary>
        /// <param name="fields">List of raw fields</param>
        /// <param name="constraints">List of raw constraints</param>
        /// <returns>The filtered list of constraints and fields of the table</returns>
        private List<DtoFullField> ConstraintsCollectionPreparation(List<DtoFullField> fields, IEnumerable<DtoSyBaseConstaintsModel> constraints)
        {
            List<DtoFullField> tempConstraints = new List<DtoFullField>();

            foreach (var item in constraints)
            {
                DtoFullField field = new DtoFullField();

                if (item.ConstraintType == "Table Constraint")
                {
                    field.ConstraintType = GetConstraintType(item.ConstraintKeys);
                    field.ConstraintKeys = GetConstraintKey(item.ConstraintKeys);
                    field.FieldName = GetConstraintField(item.ConstraintKeys, fields);
                }
                else
                {
                    field.ConstraintType = GetConstraintType(item.ConstraintType);
                    field.ConstraintKeys = item.ConstraintKeys;
                    field.FieldName = item.FieldName;
                }

                field.ConstraintName = item.ConstraintName;
                field.OnDelete = item.OnDelete;
                field.OnUpdate = item.OnUpdate;
                field.Referenced = GetReferenced(item.OtherTable, item.OtherColumns);

                tempConstraints.Add(field);
            }
            return tempConstraints;
        }

        /// <summary>
        /// Returns a list of filtered fields
        /// </summary>
        /// <param name="fields">List of raw fields</param>
        /// <param name="consts">List of raw constraints</param>
        /// <returns>Building Collection Of Fields</returns>
        private List<DtoFullField> BuildingCollectionOfFields(List<DtoFullField> fields, List<DtoFullField> consts)
        {
            List<DtoFullField> tempFullFieldsCollection = new List<DtoFullField>();

            foreach (var field in fields)
            {
                DtoFullField fullField = new DtoFullField()
                {
                    FieldName = field.FieldName,
                    TypeName = field.TypeName,
                    Size = field.Size,
                    IsNullable = field.IsNullable
                };

                bool added = false;
                bool flag = false;
                foreach (var con in consts)
                {
                    if (con.FieldName == field.FieldName && !flag)
                    {
                        fullField.ConstraintKeys = con.ConstraintKeys;
                        fullField.ConstraintName = con.ConstraintName;
                        fullField.ConstraintType = con.ConstraintType;
                        fullField.Referenced = con.Referenced;
                        fullField.OnUpdate = con.OnUpdate;
                        fullField.OnDelete = con.OnDelete;
                        flag = true;
                        added = true;
                        tempFullFieldsCollection.Add(fullField);
                    }
                    else if (con.FieldName == field.FieldName && flag)
                    {
                        DtoFullField temp = new DtoFullField()
                        {
                            ConstraintKeys = con.ConstraintKeys,
                            ConstraintName = con.ConstraintName,
                            ConstraintType = con.ConstraintType,
                            OnDelete = con.OnDelete,
                            OnUpdate = con.OnUpdate,
                            Referenced = con.Referenced
                        };
                        tempFullFieldsCollection.Add(temp);
                    }
                }
                if (!added)
                {
                    tempFullFieldsCollection.Add(fullField);
                }
            }
            return tempFullFieldsCollection;
        }

        /// <summary>
        /// Returns a formatted string for the field type name
        /// </summary>
        /// <param name="str">Raw string</param>
        /// <returns>Formatted string for the field type name</returns>
        private string GetType(string str)
        {
            string digit = null;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    break;
                }

                digit += str[i];
            }

            return digit;
        }

        /// <summary>
        /// Returns a formatted string of the field size type
        /// </summary>
        /// <param name="str">Raw string</param>
        /// <returns>Formatted string size field type</returns>
        private string GetSize(string str)
        {
            string digit = null;
            bool flag = false;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ')')
                {
                    flag = false;
                }

                if (flag)
                {
                    digit += str[i];
                }

                if (str[i] == '(')
                {
                    flag = true;
                }
            }

            return digit;
        }

        /// <summary>
        /// Returns a formatted string of the restriction type
        /// </summary>
        /// <param name="constraint">List of raw constraints</param>
        /// <returns>Formatted string type restriction</returns>
        private string GetConstraintType(string constraint)
        {
            if (constraint != null)
            {
                var type = constraint.Split(new char[] { ' ', '(', }, StringSplitOptions.RemoveEmptyEntries);
                if (type[0] == "Primary" || type[0] == "Foreign")
                {
                    return constraint;
                }
                else if (type[0] == "check")
                {
                    return "Check";
                }

                return type[0];
            }

            return null;
        }

        /// <summary>
        /// Returns a formatted string for the restriction field
        /// </summary>
        /// <param name="constraint">List of raw constraints</param>
        /// <param name="fields">List of raw fields</param>
        /// <returns>Formatted string of the restriction field/returns>
        private string GetConstraintField(string constraint, List<DtoFullField> fields)
        {
            if (constraint != null)
            {
                var temp = constraint.Split('"');
                foreach (var item in temp)
                {
                    foreach (var field in fields)
                    {
                        if (field.FieldName == item)
                        {
                            return field.FieldName;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a formatted string of constraint keys
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns>A formatted string of the key restrictions</returns>
        private string GetConstraintKey(string constraint)
        {
            string temp = null;
            bool flag = false;

            if (constraint != null)
            {
                for (int i = 0; i < constraint.Length; i++)
                {
                    if (i == constraint.Length - 1 && constraint[constraint.Length - 1] == ')')
                    {
                        break;
                    }

                    if (flag)
                    {
                        temp += constraint[i];
                    }

                    if (constraint[i] == '(')
                    {
                        flag = true;
                    }
                }
            }

            return temp;
        }

        /// <summary>
        /// Returns a formatted reference to the table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="fieldName">Field name</param>
        /// <returns>Formatted reference to the table</returns>
        private string GetReferenced(string tableName, string fieldName)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                return tableName + " (" + fieldName + ")";
            }
            return null;
        }

        /// <summary>
        /// Null to empty string
        /// </summary>
        /// <param name="collection">List of fields with null</param>
        private void NullValueToEmptyString(List<DtoFullField> collection)
        {
            foreach (var item in collection)
            {
                item.ConstraintKeys = item.ConstraintKeys ?? "";
                item.ConstraintName = item.ConstraintName ?? "";
                item.ConstraintType = item.ConstraintType ?? "";
                item.FieldName      = item.FieldName      ?? "";
                item.IsNullable     = item.IsNullable     ?? "";
                item.OnDelete       = item.OnDelete       ?? "";
                item.OnUpdate       = item.OnUpdate       ?? "";
                item.Referenced     = item.Referenced     ?? "";
                item.Size           = item.Size           ?? "";
                item.TypeName       = item.TypeName       ?? "";
            }
        }
    }
}
