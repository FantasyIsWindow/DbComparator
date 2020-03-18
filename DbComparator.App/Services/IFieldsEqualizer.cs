using Comparator.Repositories.Models.DtoModels;
using System.Collections.ObjectModel;

namespace DbComparator.App.Services
{
    public interface IFieldsEqualizer
    {
        void CollectionsEquation(ObservableCollection<DtoFullField> primaryCol, ObservableCollection<DtoFullField> secondaryCol);
    }
}
