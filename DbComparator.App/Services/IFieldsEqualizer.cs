using Comparator.Repositories.Models.DbModels;
using System.Collections.ObjectModel;

namespace DbComparator.App.Services
{
    public interface IFieldsEqualizer
    {
        void CollectionsEquation(ObservableCollection<FullField> primaryCol, ObservableCollection<FullField> secondaryCol);
    }
}
