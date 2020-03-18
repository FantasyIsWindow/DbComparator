using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using System.Collections.ObjectModel;

namespace DbComparator.App.Services
{
    public class FieldsEqualizer : IFieldsEqualizer
    {
        public void CollectionsEquation(ObservableCollection<DtoFullField> primaryCol, ObservableCollection<DtoFullField> secondaryCol)
        {
            if (primaryCol == null || secondaryCol == null)
            {
                return;
            }

            if (primaryCol.Count > secondaryCol.Count)
            {
                EqualizerCollections(primaryCol, secondaryCol);
            }
            else
            {
                EqualizerCollections(secondaryCol, primaryCol);
            }
        }

        private void EqualizerCollections(ObservableCollection<DtoFullField> primaryCol, ObservableCollection<DtoFullField> secondaryCol)
        {
            int size = primaryCol.Count - secondaryCol.Count;

            for (int i = 0; i < size; i++)
            {
                var field = new DtoFullField()
                {
                    ConstraintKeys = "",
                    ConstraintType = "",
                    ConstraintName = "",
                    FieldName = "",
                    IsNullable = "",
                    OnDelete = "",
                    OnUpdate = "",
                    References = "",
                    Size = "",
                    TypeName = ""
                };
                secondaryCol.Add(field);
            }
        }
    }
}
