using Comparator.Repositories.Models.DbModels;
using System.Collections.ObjectModel;

namespace DbComparator.App.Services
{
    public class FieldsEqualizer : IFieldsEqualizer
    {
        public void CollectionsEquation(ObservableCollection<FullField> primaryCol, ObservableCollection<FullField> secondaryCol)
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

        private void EqualizerCollections(ObservableCollection<FullField> primaryCol, ObservableCollection<FullField> secondaryCol)
        {
            int size = primaryCol.Count - secondaryCol.Count;

            for (int i = 0; i < size; i++)
            {
                var field = new FullField()
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
