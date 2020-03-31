using DbComparator.App.Infrastructure.Delegates;

namespace DbComparator.App.ViewModels
{
    public interface IMessagerVM
    {
        event NotifyDelegate OkHandler;

        event NotifyDelegate CloseHandler;

        void ShowMessageBox(string title, object package, MbShowDialog state);
    }
}
