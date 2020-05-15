namespace Comparator.Repositories.DbRequests
{
    internal class SyBaseRequests
    {
        /// <summary>
        /// Returns the text of a request to get a list of table names
        /// </summary>
        /// <param name="ownerName">Data base owner</param>
        /// <returns>Request to get a list of table names in the database</returns>
        public string GetTablesRequest(string ownerName) =>
            $"EXEC sp_tables @table_owner = '{ownerName}'";

        /// <summary>
        /// Returns the text of a request to get a list of table fields
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Request to get a list of fields from a table in the database</returns>
        public string GetFieldsRequest(string tableName) =>
            $"SELECT * FROM systabcol c KEY JOIN systab t on t.table_id=c.table_id WHERE t.table_name = '{tableName}'";

        /// <summary>
        /// Returns the text of a request to get a list of procedures
        /// </summary>
        /// <param name="ownerName">Data base owner</param>
        /// <returns>Request to get a list of database procedures</returns>
        public string GetProceduresRequest(string ownerName) =>
            $"EXEC sp_stored_procedures @sp_owner = '{ownerName}'";

        /// <summary>
        /// Returns the procedure or trigger script
        /// </summary>
        /// <param name="name">Entity name</param>
        /// <returns>Request to get a procedure script or trigger</returns>
        public string GetSqriptRequest(string name) =>
            $"EXEC sp_helptext '{name}'";

        /// <summary>
        /// Returns the text of a request to get a list of constraints
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Request to get a list of table constraints</returns>
        public string GetConstraintsRequest(string tableName) =>
            $"SELECT(SELECT TABLE_NAME FROM SysTable WHERE object_id = SC.\"table_object_id\") " +
            $"AS \"table_name\", " +
            $"SC.constraint_name AS constraint_name, " +
            $"CASE SC.constraint_type " +
                $"WHEN 'C' THEN 'Column Check' " +
                $"WHEN 'T' THEN 'Table Constraint' " +
                $"WHEN 'P' THEN 'Primary Key' " +
                $"WHEN 'F' THEN 'Foreign Key' " +
                $"WHEN 'U' THEN 'Unique Constraint' " +
            $"END AS constraint_type, " +
            $"CASE SC.constraint_type " +
                $"WHEN 'C' THEN (SELECT column_name FROM syscolumn WHERE syscolumn.object_id = SC.ref_object_id) " +
                $"WHEN 'P' THEN(SELECT list (column_name ORDER BY column_id) FROM SysIDX KEY JOIN SysTable KEY JOIN SysColumn " +
                $"WHERE SysIDX.object_id = SC.ref_object_id AND pkey = 'Y') " +
                $"WHEN 'F' THEN(SELECT list(Column_Name) FROM \"SysIdx\" LEFT JOIN SysFKey ON SysIdx.index_id = SysFKey.foreign_index_id AND SysFKey.foreign_table_id = SysIdx.table_id " +
                $"KEY JOIN SysFKCol JOIN SysColumn ON SysFKCol.foreign_table_id = SysColumn.table_id AND SysFKCol.foreign_column_id = SysColumn.column_id WHERE SysIdx.Object_id = SC.ref_object_id) " +
                $"WHEN 'U' THEN (SELECT list (column_name ORDER BY SEQUENCE) FROM SysIDX KEY JOIN SysIXCol KEY JOIN SysColumn WHERE SysIDX.object_id = SC.ref_object_id) " +
            $"END AS fields_name, " +
            $"CASE SC.constraint_type " +
                $"WHEN 'C' THEN (SELECT \"check_defn\" FROM \"SYSCHECK\" WHERE \"check_id\" =  SC.constraint_id) " +
                $"WHEN 'T' THEN(SELECT \"check_defn\" FROM \"SYSCHECK\" " +
                $"WHERE \"check_id\" =  SC.constraint_id) " +
            $"END AS \"constraint_keys\", " +
            $"CASE SC.constraint_type WHEN 'F' THEN(SELECT SysTable.TABLE_NAME FROM \"SysIdx\" id1 LEFT OUTER JOIN SysFKey ON id1.index_id = SysFKey.foreign_index_id AND " +
            $"SysFKey.foreign_table_id = id1.table_id LEFT OUTER JOIN \"SysIdx\" id2 ON id2.index_id = SysFKey.primary_index_id AND id2.table_id = SysFKey.primary_table_id " +
            $"KEY JOIN SysTable WHERE id1.Object_id = SC.ref_object_id) " +
            $"END AS other_table, " +
            $"CASE SC.constraint_type " +
              $"WHEN 'F' THEN (SELECT list(Column_Name) FROM \"SysIdx\" LEFT JOIN SysFKey ON SysIdx.index_id = SysFKey.foreign_index_id AND SysFKey.foreign_table_id = SysIdx.table_id KEY JOIN SysFKCol JOIN " +
              $"SysColumn ON SysFKey.primary_table_id = SysColumn.table_id AND SysFKCol.primary_column_id = SysColumn.column_id WHERE SysIdx.Object_id = SC.ref_object_id) " +
            $"END AS other_columns, " +
            $"CASE SC.constraint_type " +
              $"WHEN 'F' THEN (SELECT \"nulls\" FROM \"SysIdx\" LEFT JOIN SysFKey ON SysIdx.index_id = SysFKey.foreign_index_id AND SysFKey.foreign_table_id = SysIdx.table_id WHERE SysIdx.Object_id = SC.ref_object_id) " +
            $"END AS \"allow_null\", " +
            $"CASE SC.constraint_type " +
                $"WHEN 'F' THEN(CASE (SELECT referential_action FROM \"SysIdx\" LEFT JOIN SysFKey ON SysIdx.index_id = SysFKey.foreign_index_id AND SysFKey.foreign_table_id = SysIdx.table_id " +
                $"LEFT OUTER JOIN \"SYS\".\"SYSTRIGGER\" AS \"UT\" ON \"UT\".\"foreign_table_id\" = SysFKey.\"foreign_table_id\" AND \"UT\".\"foreign_key_id\" = SysFKey.\"foreign_index_id\" " +
                $"AND \"UT\".\"event\" = 'C' WHERE SysIdx.Object_id = SC.ref_object_id) " +
                $"WHEN 'C' THEN 'Cascade' " +
                $"WHEN 'D' THEN 'Set Default' " +
                $"WHEN 'N' THEN 'Set Null' " +
                $"WHEN 'R' THEN 'Restrict' END) " +
            $"END AS \"on_update\", " +
            $"CASE SC.constraint_type " +
              $"WHEN 'F' THEN(" +
            $"CASE (SELECT referential_action FROM \"SysIdx\" LEFT JOIN SysFKey ON SysIdx.index_id = SysFKey.foreign_index_id AND SysFKey.foreign_table_id = SysIdx.table_id LEFT OUTER " +
            $"JOIN \"SYS\".\"SYSTRIGGER\" AS \"UT\" ON \"UT\".\"foreign_table_id\" = SysFKey.\"foreign_table_id\" AND \"UT\".\"foreign_key_id\" = SysFKey.\"foreign_index_id\" AND \"UT\".\"event\" = 'D' " +
            $"WHERE SysIdx.Object_id = SC.ref_object_id) " +
                $"WHEN 'C' THEN 'Cascade' " +
                $"WHEN 'D' THEN 'Set Default' " +
                $"WHEN 'N' THEN 'Set Null' " +
                $"WHEN 'R' THEN 'Restrict' END) " +
                $"END AS \"on_delete\" " +
            $"FROM SysTable " +
            $"JOIN \"SYSCONSTRAINT\" SC ON SysTable.object_id = SC.table_object_id " +
            $"LEFT OUTER JOIN \"SYSINDEX\" SIUnique ON SC.ref_object_id = SIUnique.object_id " +
            $"WHERE SysTable.TABLE_NAME = '{tableName}'";

        /// <summary>
        /// Returns the text of a request to get a list of triggers
        /// </summary>
        /// <returns>Request to get a list of database triggers</returns>
        public string GetTreggersRequest() => 
            $"SELECT r.trigger_name FROM sys.systable AS t JOIN sys.systrigger r ON r.table_id = t.table_id";
    }
}
