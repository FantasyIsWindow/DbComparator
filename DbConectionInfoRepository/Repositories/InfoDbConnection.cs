using DbConectionInfoRepository.DbRequests;
using DbConectionInfoRepository.Models;
using System.Collections.Generic;

namespace DbConectionInfoRepository.Repositories
{
    /// <summary>
    /// Specifies whether the database is a reference database
    /// </summary>
    public enum IsReference { Yes, No }


    public class InfoDbConnection : IInfoDbRepository
    {
        private readonly DbRepository _db;

        private readonly SQLiteRequests _request;

        public InfoDbConnection()
        {
            _db = new DbRepository();
            _request = new SQLiteRequests();
        }

        /// <summary>
        /// Returns a collection of database providers available in the table
        /// </summary>
        /// <returns>Collection of database providers available in the table</returns>
        public IEnumerable<string> GetAllTypes() =>
            _db.ExecuteQuery<string>(_request.GetAllTypesRequest(_db.TableName));

        /// <summary>
        /// Returns a list of all databases from the required provider
        /// </summary>
        /// <param name="dbType">Provider Db type</param>
        /// <param name="reference">Is the requested database a reference database</param>
        /// <returns>Collection of all databases from the required provider</returns>
        public IEnumerable<DbInfoModel> GetAllDbByType(string dbType, IsReference reference)
        {
            string str = reference.ToString();
            return _db.ExecuteQuery<DbInfoModel>(_request.GetAllDbByTypeRequest(_db.TableName, dbType, str));
        }

        /// <summary>
        /// Adding a new entry to the database
        /// </summary>
        /// <param name="model">Entity to add</param>
        public void AddNewRecord(DbInfoModel model) =>
            _db.ExecuteCommand(_request.AddNewRecordRequest(_db.TableName), model);

        /// <summary>
        /// Updating an existing record in the database
        /// </summary>
        /// <param name="model">Entity to update</param>
        public void UpdateDbInfo(DbInfoModel model) =>
            _db.ExecuteCommand(_request.UpdateDbInfoRequest(_db.TableName), model);

        /// <summary>
        /// Deleting an existing record in the database
        /// </summary>
        /// <param name="model">Entity to delete</param>
        public void DeleteDbInfo(DbInfoModel model) =>
            _db.ExecuteCommand(_request.DeleteDbInfoRequest(_db.TableName), model);
    }
}
