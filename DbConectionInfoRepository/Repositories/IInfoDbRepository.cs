using DbConectionInfoRepository.Enums;
using DbConectionInfoRepository.Models;
using System.Collections.Generic;

namespace DbConectionInfoRepository.Repositories
{
    public interface IInfoDbRepository
    {
        IEnumerable<string> GetAllTypes();

        IEnumerable<DbInfoModel> GetAllReferenceDbByType(string dbType, IsReference reference);

        void AddNewRecord(DbInfoModel model);

        void UpdateDbInfo(DbInfoModel model);

        void DeleteDbInfo(DbInfoModel model);
    }
}
