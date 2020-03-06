namespace Comparator.Repositories.Models.DbModels
{
    public class FullField
    {
        public string FieldName { get; set; }

        public string TypeName { get; set; }

        public string Size { get; set; }

        public string IsNullable { get; set; }

        public string ConstraintType { get; set; }

        public string ConstraintName { get; set; }

        public string ConstraintKeys { get; set; }

        public string References { get; set; }

        public string OnUpdate { get; set; }

        public string OnDelete { get; set; }       
    }
}
