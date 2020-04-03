using DbComparator.App.Infrastructure.Delegates;
using DbConectionInfoRepository.Models;
using System;

namespace DbComparator.App.ViewModels
{
    public interface IDbInfoCreatorVM
    {
        /// <summary>
        /// Handler for pressing the Ok button
        /// </summary>
        event EventHandler OkHandler;

        /// <summary>
        /// Handler for pressing the Close or Chancel button
        /// </summary>
        event NotifyDelegate CloseHandler;

        /// <summary>
        /// Handler for the message delivery
        /// </summary>
        event EventHandler MessageHandler;

        /// <summary>
        /// Opens a dialog box to fill in the data about the database
        /// </summary>
        /// <param name="status">The status window is open</param>
        /// <param name="dbInfo">Data base information</param>
        void ShowManagerWindow(OpenStatus status, DbInfoModel dbInfo = null);
    }
}
