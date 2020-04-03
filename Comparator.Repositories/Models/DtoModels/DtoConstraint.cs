namespace Comparator.Repositories.Models.DtoModels
{
    public class DtoConstraint
    {
        public string FieldName { get; set; }

        public string ConstraintType { get; set; }

        public string ConstraintName { get; set; }

        public string ConstraintKeys { get; set; }

        public string Referenced { get; set; }

        public string OnUpdate { get; set; }

        public string OnDelete { get; set; }
    }
}
