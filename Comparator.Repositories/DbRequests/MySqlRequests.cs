namespace Comparator.Repositories.DbRequests
{
    internal class MySqlRequests
    {
        /// <summary>
        /// Returns the text of a request to get a list of table names
        /// </summary>
        /// <param name="dbName">Data base name</param>
        /// <returns>Request to get a list of table names in the database</returns>
        public string GetTablesRequest(string dbName) =>
            $"SHOW TABLES FROM {dbName}";

        /// <summary>
        /// Returns the text of a request to get a list of table fields
        /// </summary>
        /// <param name="dbName">Data base name</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Request to get a list of fields from a table in the database</returns>
        public string GetFieldsRequest(string dbName, string tableName) => 
            $"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT, EXTRA, ORDINAL_POSITION," +
            $"CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' " +
            $"AND TABLE_SCHEMA = '{dbName}' ORDER BY ORDINAL_POSITION;";

        /// <summary>
        /// Returns the text of a request to get a list of constraints
        /// </summary>
        /// <param name="dbName">Data base name</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Request to get a list of table constraints</returns>
        public string GetConstraintsRequest(string dbName, string tableName) =>
            $"SELECT DISTINCT i.CONSTRAINT_TYPE, i.CONSTRAINT_NAME, k.COLUMN_NAME, k.REFERENCED_TABLE_NAME, " +
            $"k.REFERENCED_COLUMN_NAME FROM information_schema.TABLE_CONSTRAINTS i LEFT JOIN " +
            $"information_schema.KEY_COLUMN_USAGE k ON i.CONSTRAINT_NAME = k.CONSTRAINT_NAME WHERE " +
            $"i.TABLE_NAME = '{tableName}' AND k.TABLE_NAME = '{tableName}' AND i.TABLE_SCHEMA = '{dbName}';";

        /// <summary>
        /// Returns information about the dependencies of a table
        /// </summary>
        /// <param name="dbName">Data base name</param>
        /// <param name="tableName">Table name</param>
        /// <returns>Request for information about table dependencies</returns>
        public string GetCascadeOptions(string dbName, string tableName) => 
            $"SELECT CONSTRAINT_NAME, UPDATE_RULE, DELETE_RULE FROM " +
            $"information_schema.REFERENTIAL_CONSTRAINTS WHERE TABLE_NAME = '{tableName}'  AND CONSTRAINT_SCHEMA = '{dbName}'";

        /// <summary>
        /// Returns the text of a request to get a list of procedures
        /// </summary>
        /// <param name="dbName">Data base name</param>
        /// <returns>Request to get a list of database procedures</returns>
        public string GetProceduresRequest(string dbName) =>
            $"SHOW PROCEDURE STATUS WHERE Db = '{dbName}'";

        /// <summary>
        /// Returns the procedure script
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <returns>Request to get a procedure script</returns>
        public string GetProcedureSqriptRequest(string procedureName) => 
            $"SHOW CREATE PROCEDURE {procedureName}";

        /// <summary>
        /// Returns the text of a request to get a list of triggers
        /// </summary>
        /// <param name="dbName">Data base name</param>
        /// <returns>Request to get a list of database triggers</returns>
        public string GetTreggersRequest(string dbName) =>
            $"SHOW TRIGGERS FROM {dbName}";

        /// <summary>
        /// Returns the trigger script
        /// </summary>
        /// <param name="dbName">Data base name</param>
        /// <param name="triggerName">Trigger name</param>
        /// <returns>Request to get a trigger script</returns>
        public string GetTreggersSqriptRequest(string dbName, string triggerName) => 
            $"SELECT TRIGGER_NAME, ACTION_TIMING, EVENT_MANIPULATION, EVENT_OBJECT_TABLE, ACTION_ORIENTATION, ACTION_STATEMENT " +
            $"FROM information_schema.triggers WHERE TRIGGER_NAME = '{triggerName}' AND TRIGGER_SCHEMA = '{dbName}'";

        /// <summary>
        /// Returns the table script
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns></returns>
        public string GetTableDDL(string tableName) => 
            $"SHOW CREATE TABLE {tableName}";
    }
}
