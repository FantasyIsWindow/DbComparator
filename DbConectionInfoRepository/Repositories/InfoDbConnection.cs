using DbConectionInfoRepository.DbRequests;
using DbConectionInfoRepository.Models;
using System.Collections.Generic;

namespace DbConectionInfoRepository.Repositories
{
    public enum IsReference { Yes, No }


    public class InfoDbConnection : IInfoDbRepository
    {
        private DbRepository _db;

        private SQLiteRequests _request;

        public InfoDbConnection()
        {
            _db = new DbRepository();
            _request = new SQLiteRequests();
        }

        public IEnumerable<string> GetAllTypes() =>
            _db.ExecuteQuery<string>(_request.GetAllTypesRequest(_db.TableName));

        public IEnumerable<DbInfoModel> GetAllReferenceDbByType(string dbType, IsReference reference)
        {
            string str = reference == IsReference.Yes ? "Yes" : "No";
            return _db.ExecuteQuery<DbInfoModel>(_request.GetAllDbByTypeRequest(_db.TableName, dbType, str));
        }

        public void AddNewRecord(DbInfoModel model) =>
            _db.ExecuteCommand(_request.AddNewRecordRequest(_db.TableName), model);

        public void UpdateDbInfo(DbInfoModel model) =>
            _db.ExecuteCommand(_request.UpdateDbInfoRequest(_db.TableName), model);

        public void DeleteDbInfo(DbInfoModel model) =>
            _db.ExecuteCommand(_request.DeleteDbInfoRequest(_db.TableName), model);
    }
}
