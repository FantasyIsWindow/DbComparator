namespace Comparator.Repositories.Models.DbModels
{
    public class MySqlConstaintsModel
    {
        public string CONSTRAINT_TYPE { get; set; }

        public string CONSTRAINT_NAME { get; set; }

        public string COLUMN_NAME { get; set; }

        public string REFERENCED_TABLE_NAME { get; set; }

        public string REFERENCED_COLUMN_NAME { get; set; }
    }
}
