using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Comparator.Repositories.Repositories
{
    public class DbRepository
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _factory;

        public DbRepository(string connectionString, string provider)
        {
            _connectionString = connectionString;
            _factory = DbProviderFactories.GetFactory(provider);
        }

        public IEnumerable<T> Select<T>(string query) where T : class
        {
            try
            {
                using (DbConnection connector = _factory.CreateConnection())
                {
                    connector.ConnectionString = _connectionString;
                    connector.Open();
                    var result = connector.Query<T>(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }            
        }

        public async Task<bool> CheckConectionAsync()
        {
            try
            {
                using (DbConnection connector = _factory.CreateConnection())
                {
                    connector.ConnectionString = _connectionString;
                    await connector.OpenAsync();                    
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            
        }
    }
}
