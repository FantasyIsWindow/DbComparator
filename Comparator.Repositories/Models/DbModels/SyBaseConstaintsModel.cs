namespace Comparator.Repositories.Models.DbModels
{
    internal class SyBaseConstaintsModel
    {
        public string constraint_type { get; set; }

        public string constraint_name { get; set; }

        public string constraint_keys { get; set; }

        public string fields_name { get; set; }

        public string other_table { get; set; }

        public string other_columns { get; set; }

        public string allow_null { get; set; }

        public string on_update { get; set; }

        public string on_delete { get; set; }
    }
}
