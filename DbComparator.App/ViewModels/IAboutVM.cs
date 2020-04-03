using DbComparator.App.Infrastructure.Delegates;

namespace DbComparator.App.ViewModels
{
    public interface IAboutVM
    {
        /// <summary>
        /// Handler for pressing the Close or Chancel button
        /// </summary>
        event NotifyDelegate CloseHandler;
    }
}
