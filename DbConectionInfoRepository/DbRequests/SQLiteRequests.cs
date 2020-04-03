namespace DbConectionInfoRepository.DbRequests
{
    internal class SQLiteRequests
    {
        /// <summary>
        /// Returns a request to get all providers available in the specified table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Request to get all providers available in the specified table</returns>
        public string GetAllTypesRequest(string tableName) => 
            $"SELECT DbType FROM {tableName} GROUP BY DbType";

        /// <summary>
        /// Returns a request to select all databases from the required provider
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="type">Provider Db type</param>
        /// <param name="isReference">Is the requested database a reference database</param>
        /// <returns>Request to select all databases from the required provider</returns>
        public string GetAllDbByTypeRequest(string tableName, string type, string isReference) =>
            $"SELECT * FROM {tableName} WHERE DbType = '{type}' AND Reference = '{isReference}'";

        /// <summary>
        /// Returns a request to add a new entry to the table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Request to add a new entry to the table</returns>
        public string AddNewRecordRequest(string tableName) => 
            $"INSERT INTO {tableName} (DataSource, ServerName, DbName, Login, Password, DbType, Reference) " +
            $"VALUES (@DataSource, @ServerName, @DbName, @Login, @Password, @DbType, @Reference)";

        /// <summary>
        /// Returns a request to update an existing record in the table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Request to update an existing record in the table</returns>
        public string UpdateDbInfoRequest(string tableName) =>
            $"UPDATE {tableName} SET DataSource = @DataSource, ServerName = @ServerName, DbName = @DbName, " +
            $"Login = @Login, Password = @Password, DbType = @DbType, Reference = @Reference WHERE Id = @Id";

        /// <summary>
        /// Returns a request to delete an existing record from the table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Request to delete an existing record from the table</returns>
        public string DeleteDbInfoRequest(string tableName) => 
            $"DELETE FROM {tableName} WHERE Id = @Id";
    }
}
