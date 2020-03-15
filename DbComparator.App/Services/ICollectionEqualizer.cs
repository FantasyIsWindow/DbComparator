using System.Collections.Generic;

namespace DbComparator.App.Services
{
    public interface ICollectionEqualizer
    {
        void CollectionsEquation(List<string> primaryCol, List<string> secondaryCol);
    }
}
