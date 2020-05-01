using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Linq;

namespace Comparator.Repositories.Parsers.MySql
{
    internal class MySqlFieldsInfoParser
    {
        /// <summary>
        /// Returns a collection of table fields
        /// </summary>
        /// <param name="fields">List of raw fields</param>
        /// <param name="constraints">List of raw constraints</param>
        /// <param name="cascadeOptions">List of raw cascade options</param>
        /// <returns>The filtered list of fields of the table<</returns>
        public IEnumerable<DtoFullField> GetFieldsCollection(IEnumerable<MySqlFields> fields, IEnumerable<MySqlConstaintsModel> constraints, IEnumerable<MySqlCascadeOption> cascadeOptions)
        {
            List<DtoConstraint> consts = ConstraintsCollectionPreparation(constraints, cascadeOptions);
            List<DtoFullField> resultFullFieldsCollection = BuildingCollectionOfFields(fields, consts);
            NullValueToEmptyString(resultFullFieldsCollection);

            return  resultFullFieldsCollection;
        }

        /// <summary>
        /// Returns a prepared collection of restrictions
        /// </summary>
        /// <param name="constraints">List of raw constraints</param>
        /// <param name="options">List of raw cascade options</param>
        /// <returns>The filtered list of constraints of the table</returns>
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
                    Referenced = ReferencedStringDecoration(cons.REFERENCED_TABLE_NAME, cons.REFERENCED_COLUMN_NAME)                   
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

        /// <summary>
        /// Returns a formatted reference to the table
        /// </summary>
        /// <param name="tName">Table name</param>
        /// <param name="fName">Field name</param>
        /// <returns>Formatted reference to the table</returns>
        private string ReferencedStringDecoration(string tName, string fName) =>
            fName != null ? tName + " (" + fName + ')' : tName;


        /// <summary>
        /// Returns a list of filtered fields
        /// </summary>
        /// <param name="fields">List of raw fields</param>
        /// <param name="consts">List of raw constraints</param>
        /// <returns>Building Collection Of Fields</returns>
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
                        temp.Referenced = con.Referenced;
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
                            Referenced = con.Referenced
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
                item.FieldName = item.FieldName ?? "";
                item.IsNullable = item.IsNullable ?? "";
                item.OnDelete = item.OnDelete ?? "";
                item.OnUpdate = item.OnUpdate ?? "";
                item.Referenced = item.Referenced ?? "";
                item.Size = item.Size ?? "";
                item.TypeName = item.TypeName ?? "";
            }
        }               
    }
}
