using DbConectionInfoRepository.Models;

namespace DbComparator.App.Models
{
    public class Passage
    {
        public DbInfoModel Info { get; set; }

        public bool IsChecked { get; set; }
    }
}
