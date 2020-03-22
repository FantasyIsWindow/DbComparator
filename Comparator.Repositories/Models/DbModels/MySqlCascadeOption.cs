namespace Comparator.Repositories.Models.DbModels
{
    public class MySqlCascadeOption
    {
        public string CONSTRAINT_NAME { get; set; }

        public string UPDATE_RULE { get; set; }

        public string DELETE_RULE { get; set; }
    }
}
