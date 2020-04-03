using DbComparator.App.Infrastructure.Delegates;

namespace DbComparator.App.ViewModels
{
    public interface IMessagerVM
    {
        /// <summary>
        /// Handler for pressing the Ok button
        /// </summary>
        event NotifyDelegate OkHandler;

        /// <summary>
        /// Handler for pressing the Close or Chancel button
        /// </summary>
        event NotifyDelegate CloseHandler;

        /// <summary>
        /// Displays the message window
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="package">Package with data to display</param>
        /// <param name="state">The display settings window</param>
        void ShowMessageBox(string title, object package, MbShowDialog state);
    }
}
