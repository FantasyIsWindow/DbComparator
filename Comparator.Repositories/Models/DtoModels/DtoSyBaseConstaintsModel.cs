namespace Comparator.Repositories.Models.DtoModels
{
    public class DtoSyBaseConstaintsModel
    {
        public string FieldName { get; set; }

        public string ConstraintType { get; set; }

        public string ConstraintName { get; set; }

        public string ConstraintKeys { get; set; }
               
        public string OtherTable { get; set; }

        public string OtherColumns { get; set; }

        public string AllowNull { get; set; }

        public string OnUpdate { get; set; }

        public string OnDelete { get; set; }
    }
}
