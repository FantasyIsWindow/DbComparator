using Dapper;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Comparator.Repositories.Repositories
{
    internal class DbRepository
    {
        private readonly string _connectionString;
        private readonly DbProviderFactory _factory;

        public DbRepository(string connectionString, string provider)
        {
            _connectionString = connectionString;
            _factory = DbProviderFactories.GetFactory(provider);
        }

        /// <summary>
        /// Getting data from a database
        /// </summary>
        /// <typeparam name="T">Class to fill in with the received data</typeparam>
        /// <param name="query">Select query</param>
        /// <returns>Collection with data received during the request</returns>
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
            catch 
            {
                return null;
            }            
        }

        /// <summary>
        /// Getting data from the database by field name
        /// </summary>
        /// <param name="query">Select query</param>
        /// <param name="key">Field name</param>
        /// <returns></returns>
        public string SelectForKey(string query, string key) 
        {
            try
            {
                using (DbConnection connector = _factory.CreateConnection())
                {
                    connector.ConnectionString = _connectionString;
                    connector.Open();
                    var result = connector.Query(query);
                    foreach (IDictionary<string, object> row in result)
                    {
                        foreach (var pair in row)
                        {
                            if (pair.Key == key)
                            {
                                return pair.Value.ToString();
                            }
                        }
                    }

                }
            }
            catch 
            {
                return null;
            }
            return "";
        }

        /// <summary>
        /// Checking database availability
        /// </summary>
        /// <returns>Returns true if the database is available</returns>
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
            catch 
            {
                return false;
            }            
        }
    }
}
