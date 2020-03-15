using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Infrastructure.Enums;

namespace DbComparator.App.ViewModels
{
    public interface IMessagerVM
    {
        event NotifyDelegate OkHandler;

        event NotifyDelegate CloseHandler;

        void ShowMessageBox(string title, string message, MbShowDialog state);
    }
}
