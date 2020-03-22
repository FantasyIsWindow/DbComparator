using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Parsers
{
    public class MySqlFieldsInfoParser
    {
        public IEnumerable<DtoFullField> GetFieldsCollection(IEnumerable<MySqlFields> fields, IEnumerable<MySqlConstaintsModel> constraints, IEnumerable<MySqlCascadeOption> options)
        {
            List<DtoConstraint> consts = ConstraintsCollectionPreparation(constraints, options);
            List<DtoFullField> resultFullFieldsCollection = BuildingCollectionOfFields(fields, consts);
            NullValueToEmptyString(resultFullFieldsCollection);

            return  resultFullFieldsCollection;
        }
                          
        private List<DtoConstraint> ConstraintsCollectionPreparation(IEnumerable<MySqlConstaintsModel> constraints, IEnumerable<MySqlCascadeOption> options)
        {
            List<DtoConstraint> dtoConstraints = new List<DtoConstraint>();
            foreach (var cons in constraints)
            {
                DtoConstraint item = new DtoConstraint
                {
                    FieldName = cons.COLUMN_NAME,
                    ConstraintType = cons.CONSTRAINT_TYPE,
                    ConstraintName = cons.CONSTRAINT_NAME,
                    References = ReferenceDecoration(cons.REFERENCED_TABLE_NAME, cons.REFERENCED_COLUMN_NAME)                   
                };
                dtoConstraints.Add(item);
            }
                                 
            foreach (var option in options)
            {
                var result = dtoConstraints.FirstOrDefault(o => o.ConstraintName == option.CONSTRAINT_NAME);
                if (result != null)
                {
                    result.OnUpdate = option.UPDATE_RULE;
                    result.OnDelete = option.DELETE_RULE;
                }
            }
            
            return dtoConstraints;
        }

        private string ReferenceDecoration(string tName, string cName) => 
            cName != null ? tName + " (" + cName + ')' : tName;

        private List<DtoFullField> BuildingCollectionOfFields(IEnumerable<MySqlFields> fields, List<DtoConstraint> consts)
        {
            List<DtoFullField> tempFieldsList = new List<DtoFullField>();

            foreach (var item in fields)
            {
                DtoFullField temp = new DtoFullField
                {
                    FieldName = item.COLUMN_NAME,
                    TypeName = item.DATA_TYPE + ' ' + item.EXTRA,
                    IsNullable = item.IS_NULLABLE,
                    ConstraintKeys = item.COLUMN_DEFAULT,
                    Size = item.CHARACTER_MAXIMUM_LENGTH
                };
                
                bool added = false;
                bool flag = false;

                foreach (var con in consts)
                {
                    if (con.FieldName == item.COLUMN_NAME && !flag)
                    {
                        temp.ConstraintType = con.ConstraintType;
                        temp.ConstraintName = con.ConstraintName;
                        temp.References = con.References;
                        temp.OnDelete = con.OnDelete;
                        temp.OnUpdate = con.OnUpdate;
                        flag = true;
                        added = true;
                        tempFieldsList.Add(temp);
                    }
                    else if (con.FieldName == item.COLUMN_NAME && flag)
                    {
                        DtoFullField qwe = new DtoFullField()
                        {
                            ConstraintType = con.ConstraintType,
                            ConstraintName = con.ConstraintName,
                            References = con.References
                        };
                        tempFieldsList.Add(qwe);
                    }
                }
                if (!added)
                {
                    tempFieldsList.Add(temp);
                }
            }    

            return tempFieldsList;
        }

        private void NullValueToEmptyString(List<DtoFullField> collection)
        {
            foreach (var item in collection)
            {
                item.ConstraintKeys = item.ConstraintKeys ?? "";
                item.ConstraintName = item.ConstraintName ?? "";
                item.ConstraintType = item.ConstraintType ?? "";
                item.FieldName = item.FieldName ?? "";
                item.IsNullable = item.IsNullable ?? "";
                item.OnDelete = item.OnDelete ?? "";
                item.OnUpdate = item.OnUpdate ?? "";
                item.References = item.References ?? "";
                item.Size = item.Size ?? "";
                item.TypeName = item.TypeName ?? "";
            }
        }               
    }
}
