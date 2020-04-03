using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DbComparator.App.ViewModels
{
    public class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Sets a new variable value and activates the update event
        /// </summary>
        /// <typeparam name="T">A generalized class</typeparam>
        /// <param name="storage">Variable receiver</param>
        /// <param name="value">Value</param>
        /// <param name="propertyName">Property name</param>
        protected virtual void SetProperty<T>(ref T storage, T value, string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return;
            }
            storage = value;
            OnPropertyChanged(propertyName);
        }
    }
}
