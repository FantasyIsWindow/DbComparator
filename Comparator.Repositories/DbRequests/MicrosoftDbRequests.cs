namespace Comparator.Repositories.DbRequests
{
    internal class MicrosoftDbRequests
    {
        public string GetTablesRequest(string ownerName) =>
            $"EXEC sp_tables @table_owner = '{ownerName}'";

        public string GetFieldsRequest(string tableName) =>
            $"EXEC sp_columns @table_name = '{tableName}'";

        public string GetConstraintsRequest(string tableName) =>
            $"EXEC sp_helpconstraint {tableName}, nomsg";

        public string GetProceduresRequest(string ownerName) =>
            $"EXEC sp_stored_procedures @sp_owner = '{ownerName}'";

        public string GetProcedureSqriptRequest(string procedureName) =>
            $"EXEC sp_helptext {procedureName}";
    }
}
