﻿using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Parsers
{
    internal class MicrosoftFieldsInfoParser
    {       
        public IEnumerable<FullField> GetFieldsCollection(IEnumerable<Fields> fields, IEnumerable<DtoConstraint> constraints)
        {
            List<DtoConstraint> consts = ConstraintsCollectionPreparation(constraints);
            List<FullField> resultFullFieldsCollection = BuildingCollectionOfFields(fields, consts);           
            NullValueToEmptyString(resultFullFieldsCollection);

            return resultFullFieldsCollection;
        }

        private List<DtoConstraint> ConstraintsCollectionPreparation(IEnumerable<DtoConstraint> constraints)
        {
            List<DtoConstraint> consts = new List<DtoConstraint>();

            foreach (var constraint in constraints)
            {
                DtoConstraint tempConsts = new DtoConstraint();

                var type = constraint.ConstraintType.Split(' ');
                tempConsts.ConstraintName = constraint.ConstraintName;
                tempConsts.OnDelete = constraint.OnDelete;
                tempConsts.OnUpdate = constraint.OnUpdate;

                if (type[0] == "CHECK" || type[0] == "DEFAULT")
                {
                    tempConsts.ConstraintType = type[0];
                    tempConsts.FieldName = type[type.Length - 1];
                    tempConsts.ConstraintKeys = constraint.ConstraintKeys;
                }
                else if (type[0] == "PRIMARY" || type[0] == "FOREIGN")
                {
                    tempConsts.ConstraintType = constraint.ConstraintType;
                    tempConsts.FieldName = constraint.ConstraintKeys;
                }
                else if (constraint.ConstraintKeys.Split(' ')[0] == "REFERENCES")
                {
                    var last = consts.Last();
                    last.References = constraint.ConstraintKeys.Replace("REFERENCES ", "");
                    continue;
                }
                else
                {
                    tempConsts.ConstraintType = constraint.ConstraintType;
                    tempConsts.FieldName = constraint.ConstraintKeys;
                }
                consts.Add(tempConsts);
            }
            return consts;
        }

        private List<FullField> BuildingCollectionOfFields(IEnumerable<Fields> fields, List<DtoConstraint> consts)
        {
            List<FullField> tempResultCollection = new List<FullField>();
            foreach (var field in fields)
            {
                FullField temp = new FullField()
                {
                    FieldName = field.COLUMN_NAME,
                    TypeName = field.TYPE_NAME,
                    Size = field.CHAR_OCTET_LENGTH,
                    IsNullable = field.IS_NULLABLE
                };

                bool added = false;
                bool flag = false;
                foreach (var con in consts)
                {
                    if (con.FieldName == field.COLUMN_NAME && !flag)
                    {
                        temp.ConstraintType = con.ConstraintType;
                        temp.ConstraintName = con.ConstraintName;
                        temp.ConstraintKeys = con.ConstraintKeys;
                        temp.References = con.References;
                        temp.OnDelete = con.OnDelete;
                        temp.OnUpdate = con.OnUpdate;
                        flag = true;
                        added = true;
                        tempResultCollection.Add(temp);
                    }
                    else if (con.FieldName == field.COLUMN_NAME && flag)
                    {
                        FullField qwe = new FullField()
                        {
                            ConstraintType = con.ConstraintType,
                            ConstraintName = con.ConstraintName,
                            ConstraintKeys = con.ConstraintKeys,
                            References = con.References
                        };
                        tempResultCollection.Add(qwe);
                    }
                }
                if (!added)
                {
                    tempResultCollection.Add(temp);
                }
            }
            return tempResultCollection;
        }

        private void NullValueToEmptyString(List<FullField> collection)
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
                item.TypeName        = item.TypeName      ?? "";
            }
        }
    }
}
