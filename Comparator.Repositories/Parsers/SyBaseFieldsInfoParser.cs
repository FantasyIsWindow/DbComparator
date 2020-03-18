using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System;

namespace Comparator.Repositories.Parsers
{
    internal class SyBaseFieldsInfoParser
    {
        public IEnumerable<DtoFullField> GetFieldsCollection(IEnumerable<SyBaseFieldsModel> fields, IEnumerable<DtoSyBaseConstaintsModel> constraints)
        {
            List<DtoFullField> tempFieldsCollection = FieldsCollectionPreparation(fields);
            List<DtoFullField> tempConstraints = ConstraintsCollectionPreparation(tempFieldsCollection, constraints);
            List<DtoFullField> resultFullFieldsCollection = BuildingCollectionOfFields(tempFieldsCollection, tempConstraints);

            NullValueToEmptyString(resultFullFieldsCollection);
            return resultFullFieldsCollection;
        }

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
                field.References = GetReference(item.OtherTable, item.OtherColumns);

                tempConstraints.Add(field);
            }
            return tempConstraints;
        }

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
                        fullField.References = con.References;
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
                            References = con.References
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

            return digit != null ? digit : null;
        }

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

            return temp != null ? temp : null;
        }

        private string GetReference(string tableName, string tableField)
        {
            if (!string.IsNullOrEmpty(tableName))
            {
                return tableName + " (" + tableField + ")";
            }
            return null;
        }

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
                item.References     = item.References     ?? "";
                item.Size           = item.Size           ?? "";
                item.TypeName       = item.TypeName       ?? "";
            }
        }
    }
}
