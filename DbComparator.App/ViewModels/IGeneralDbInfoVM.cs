using System;

namespace DbComparator.App.ViewModels
{
    public interface IGeneralDbInfoVM
    {
        event EventHandler MessageHandler;

        void ClearAll();
    }
}
