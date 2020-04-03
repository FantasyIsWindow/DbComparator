namespace Comparator.Repositories.DbRequests
{
    internal class MicrosoftDbRequests
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
            $"EXEC sp_columns @table_name = '{tableName}'";

        /// <summary>
        /// Returns the text of a request to get a list of constraints
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Request to get a list of table constraints</returns>
        public string GetConstraintsRequest(string tableName) =>
            $"EXEC sp_helpconstraint {tableName}, nomsg";

        /// <summary>
        /// Returns the text of a request to get a list of procedures
        /// </summary>
        /// <param name="ownerName">Data base owner</param>
        /// <returns>Returns the text of a request to get a list of procedures</returns>
        public string GetProceduresRequest(string ownerName) =>
            $"EXEC sp_stored_procedures @sp_owner = '{ownerName}'";

        /// <summary>
        /// Returns the procedure or trigger script
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Request to get a procedure script or trigger</returns>
        public string GetSqriptRequest(string name) =>
            $"EXEC sp_helptext {name}";

        /// <summary>
        /// Returns the text of a request to get a list of triggers
        /// </summary>
        /// <returns>Request to get a list of database triggers</returns>
        public string GetTreggersRequest() =>
            $"SELECT name FROM sys.triggers";
    }
}
