using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Infrastructure.Enums;
using DbConectionInfoRepository.Models;
using System;

namespace DbComparator.App.ViewModels
{
    public interface IDbInfoCreatorVM
    {
        event NotifyDelegate OkHandler;

        event NotifyDelegate CloseHandler;

        event EventHandler MessageHandler;

        void ShowManagerWindow(OpenStatus status, string isReference, DbInfoModel dbInfo = null);
    }
}
