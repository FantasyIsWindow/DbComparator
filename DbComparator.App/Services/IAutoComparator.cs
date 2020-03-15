using Comparator.Repositories.Repositories;

namespace DbComparator.App.Services
{
    public interface IAutoComparator
    {
        string Compare(IRepository primaryRep, IRepository secondarRep);
    }
}
