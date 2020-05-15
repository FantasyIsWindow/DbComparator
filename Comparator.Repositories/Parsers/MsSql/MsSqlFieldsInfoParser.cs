using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Parsers.MsSql
{
    internal class MsSqlFieldsInfoParser
    {
        /// <summary>
        /// Returns a collection of table fields
        /// </summary>
        /// <param name="fields">List of raw fields</param>
        /// <param name="constraints">List of raw constraints</param>
        /// <returns>The filtered list of fields of the table</returns>
        public IEnumerable<DtoFullField> GetFieldsCollection(IEnumerable<Fields> fields, IEnumerable<DtoConstraint> constraints)
        {
            List<DtoConstraint> consts = ConstraintsCollectionPreparation(constraints);
            List<DtoFullField> resultFullFieldsCollection = BuildingCollectionOfFields(fields, consts);           
            NullValueToEmptyString(resultFullFieldsCollection);

            return resultFullFieldsCollection;
        }

        /// <summary>
        /// Returns a prepared collection of restrictions
        /// </summary>
        /// <param name="constraints">List of raw constraints</param>
        /// <returns>The filtered list of constraints of the table</returns>
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
                    last.Referenced = constraint.ConstraintKeys.Replace("REFERENCES ", "");
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

        /// <summary>
        /// Returns a list of filtered fields
        /// </summary>
        /// <param name="fields">List of raw fields</param>
        /// <param name="consts">List of raw constraints</param>
        /// <returns>Building Collection Of Fields</returns>
        private List<DtoFullField> BuildingCollectionOfFields(IEnumerable<Fields> fields, List<DtoConstraint> consts)
        {
            List<DtoFullField> tempResultCollection = new List<DtoFullField>();
            foreach (var field in fields)
            {
                DtoFullField temp = new DtoFullField()
                {
                    FieldName = field.COLUMN_NAME,
                    TypeName = field.TYPE_NAME, //------------------
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
                        temp.Referenced = con.Referenced;
                        temp.OnDelete = con.OnDelete;
                        temp.OnUpdate = con.OnUpdate;
                        flag = true;
                        added = true;
                        tempResultCollection.Add(temp);
                    }
                    else if (con.FieldName == field.COLUMN_NAME && flag)
                    {
                        DtoFullField qwe = new DtoFullField()
                        {
                            ConstraintType = con.ConstraintType,
                            ConstraintName = con.ConstraintName,
                            ConstraintKeys = con.ConstraintKeys,
                            Referenced = con.Referenced
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
