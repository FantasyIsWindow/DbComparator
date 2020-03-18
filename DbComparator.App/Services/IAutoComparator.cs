using Comparator.Repositories.Repositories;
using DbComparator.App.Models;
using System.Collections.Generic;

namespace DbComparator.App.Services
{
    public interface IAutoComparator
    {
        List<CompareResult> Compare(IRepository primaryRep, IRepository secondarRep);
    }
}
