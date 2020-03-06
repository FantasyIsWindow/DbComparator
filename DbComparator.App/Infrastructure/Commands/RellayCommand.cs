using System;
using System.Windows.Input;

namespace DbComparator.App.Infrastructure.Commands
{
    public class RellayCommand : ICommand
    {
        private Action<object> _execute;
        private Predicate<object> _canExecute;


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RellayCommand(Action<object> execute) : this(execute, null)
        {
            _execute = execute;
        }

        public RellayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new Exception("Execute is null!");
            }
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => 
            _canExecute?.Invoke(parameter) ?? true;
        

        public void Execute(object parameter) => 
            _execute?.Invoke(parameter);        
    }
}