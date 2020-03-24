using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Infrastructure.Enums;
using DbConectionInfoRepository.Models;
using System;

namespace DbComparator.App.ViewModels
{
    public interface IDbInfoCreatorVM
    {
        event EventHandler OkHandler;

        event NotifyDelegate CloseHandler;

        event EventHandler MessageHandler;

        void ShowManagerWindow(OpenStatus status, DbInfoModel dbInfo = null);
    }
}
