namespace DbConectionInfoRepository.DbRequests
{
    internal class SQLiteRequests
    {
        public string GetAllTypesRequest(string tableName) => 
            $"SELECT DbType FROM {tableName} GROUP BY DbType";     
        
        public string GetAllDbByTypeRequest(string tableName, string type, string isReference) =>
            $"SELECT * FROM {tableName} WHERE DbType = '{type}' AND Reference = '{isReference}'";   

        public string AddNewRecordRequest(string tableName) => 
            $"INSERT INTO {tableName} (DataSource, ServerName, DbName, Login, Password, DbType, Reference) " +
            $"VALUES (@DataSource, @ServerName, @DbName, @Login, @Password, @DbType, @Reference)";   
        
        public string UpdateDbInfoRequest(string tableName) =>
            $"UPDATE {tableName} SET DataSource = @DataSource, ServerName = @ServerName, DbName = @DbName, " +
            $"Login = @Login, Password = @Password, DbType = @DbType, Reference = @Reference WHERE Id = @Id";   
        
        public string DeleteDbInfoRequest(string tableName) => 
            $"DELETE FROM {tableName} WHERE Id = @Id";
    }
}
