using DbConectionInfoRepository.Models;
using System.Collections.Generic;

namespace DbConectionInfoRepository.Repositories
{
    public interface IInfoDbRepository
    {
        /// <summary>
        /// Returns a collection of databases
        /// </summary>
        /// <returns>Returns a collection of databases</returns>
        IEnumerable<string> GetAllTypes();

        /// <summary>
        /// Returns a collection of databases by the requested type
        /// </summary>
        /// <param name="dbType">Data base type</param>
        /// <param name="reference">Reference</param>
        /// <returns>Collection of databases by the requested type</returns>
        IEnumerable<DbInfoModel> GetAllDbByType(string dbType, IsReference reference);

        /// <summary>
        /// Adding a new object to the database
        /// </summary>
        /// <param name="model">Object to add</param>
        void AddNewRecord(DbInfoModel model);

        /// <summary>
        /// Updating database data
        /// </summary>
        /// <param name="model">Update object</param>
        void UpdateDbInfo(DbInfoModel model);

        /// <summary>
        /// Deleting an object from the database
        /// </summary>
        /// <param name="model">Object to delete from the database</param>
        void DeleteDbInfo(DbInfoModel model);
    }
}
