using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Common;

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
                    var qwe = connector.Query(query);
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }            
        }

        public bool CheckConection()
        {
            try
            {
                using (DbConnection connector = _factory.CreateConnection())
                {
                    connector.ConnectionString = _connectionString;
                    connector.Open();                    
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
