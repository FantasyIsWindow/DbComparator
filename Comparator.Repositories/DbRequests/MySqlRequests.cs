namespace Comparator.Repositories.DbRequests
{
    public class MySqlRequests
    {
        public string GetTablesRequest(string dbName) =>
            $"SHOW TABLES FROM {dbName}";

        public string GetFieldsRequest(string dbName, string tableName) => 
            $"SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT, EXTRA, ORDINAL_POSITION," +
            $"CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' " +
            $"AND TABLE_SCHEMA = '{dbName}' ORDER BY ORDINAL_POSITION;";

        public string GetConstraintsRequest(string dbName, string tableName) =>
            $"SELECT DISTINCT i.CONSTRAINT_TYPE, i.CONSTRAINT_NAME, k.COLUMN_NAME, k.REFERENCED_TABLE_NAME, " +
            $"k.REFERENCED_COLUMN_NAME FROM information_schema.TABLE_CONSTRAINTS i LEFT JOIN " +
            $"information_schema.KEY_COLUMN_USAGE k ON i.CONSTRAINT_NAME = k.CONSTRAINT_NAME WHERE " +
            $"i.TABLE_NAME = '{tableName}' AND k.TABLE_NAME = '{tableName}' AND i.TABLE_SCHEMA = '{dbName}';";

        public string GetCascadeOptions(string dbName, string tableName) => 
            $"SELECT CONSTRAINT_NAME, UPDATE_RULE, DELETE_RULE FROM " +
            $"information_schema.REFERENTIAL_CONSTRAINTS WHERE TABLE_NAME = '{tableName}'  AND CONSTRAINT_SCHEMA = '{dbName}'";

        public string GetProceduresRequest(string dbName) =>
            $"SHOW PROCEDURE STATUS WHERE Db = '{dbName}'";

        public string GetProcedureSqriptRequest(string procedureName) => 
            $"SHOW CREATE PROCEDURE {procedureName}";

        public string GetTreggersRequest(string dbName) =>
            $"SHOW TRIGGERS FROM {dbName}";

        public string GetTreggersSqriptRequest(string dbName) => 
            $"select action_statement from information_schema.triggers where TRIGGER_NAME = '{ dbName}'";
    }
}
