using System;

namespace DbComparator.App.ViewModels
{
    public interface IGeneralDbInfoVM
    {
        event EventHandler MessageHandler;

        event EventHandler CurrentEntitiesMessageHandler;

        void ClearInfoObjects(DepthOfCleaning depth);
    }
}
