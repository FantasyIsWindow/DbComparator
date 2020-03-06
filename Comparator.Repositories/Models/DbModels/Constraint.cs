namespace Comparator.Repositories.Models.DbModels
{
    public class Constraint
    {
        public string constraint_type { get; set; }

        public string constraint_name { get; set; }

        public string constraint_keys { get; set; }

        public string update_action { get; set; }

        public string delete_action { get; set; }
    }
}
