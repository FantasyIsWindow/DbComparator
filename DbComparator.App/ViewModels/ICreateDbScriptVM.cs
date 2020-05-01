using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Models;
using System.Collections.ObjectModel;

namespace DbComparator.App.ViewModels
{
    public interface ICreateDbScriptVM
    {
        event NotifyDelegate CloseHandler;

        void SetDbInfo(ObservableCollection<Passage> info, IRepository _dbRepository);

    }
}
