using DbConectionInfoRepository.Models;

namespace DbComparator.App.Models
{
    public class DbInfo
    {
        public bool IsConnect { get; set; }

        public DbInfoModel DataBase { get; set; }
    }
}
