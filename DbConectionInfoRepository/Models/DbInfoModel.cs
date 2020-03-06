
namespace DbConectionInfoRepository.Models
{
    public class DbInfoModel
    {
        public int Id { get; set; }

        public string DataSource { get; set; }

        public string ServerName { get; set; }

        public string DbName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string DbType { get; set; }

        public string Reference { get; set; }
    }
}
