using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comparator.Repositories.Repositories
{
    public interface IRepository
    {
        string DbType { get; }

        string DbName { get; }

        /// <summary>
        /// Creates a database connection string
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="server">Server name</param>
        /// <param name="dbName">Data base name</param>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        void CreateConnectionString(string source, string server, string dbName, string login, string password);

        /// <summary>
        /// Returns a collection of table fields
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Collection of table fields</returns>
        IEnumerable<DtoFullField> GetFieldsInfo(string tableName);

        /// <summary>
        /// Returns a list of procedures
        /// </summary>
        /// <returns>List of procedures</returns>
        IEnumerable<string> GetProcedures();

        /// <summary>
        /// Returns the procedure script
        /// </summary>
        /// <param name="procedureName">Procedure name</param>
        /// <returns>Procedure script</returns>
        string GetProcedureSqript(string procedureName);

        /// <summary>
        /// Returns the trigger script
        /// </summary>
        /// <param name="triggerName">Tragger name</param>
        /// <returns>Trigger script</returns>
        string GetTriggerSqript(string triggerName);

        /// <summary>
        /// Returns a list of tables
        /// </summary>
        /// <returns>List of tables</returns>
        IEnumerable<string> GetTables();

        /// <summary>
        /// Returns a list of triggers
        /// </summary>
        /// <returns>List of triggers</returns>
        IEnumerable<string> GetTriggers();

        /// <summary>
        /// Get the script for the entire database
        /// </summary>
        /// <returns></returns>
        string GetDbScript();

        /// <summary>
        /// Get the script for all tables from the database
        /// </summary>
        /// <returns></returns>
        string GetTablesScript();

        /// <summary>
        /// Get the script for all procedures from the database
        /// </summary>
        /// <returns></returns>
        string GetProceduresScript();

        /// <summary>
        /// Get the script for all triggers from the database
        /// </summary>
        /// <returns></returns>
        string GetTriggersScript();

        /// <summary>
        /// Asynchronous database connection verification
        /// </summary>
        /// <returns>Test result</returns>
        Task<bool> IsConnectionAsync();
    }
}
 