using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Infrastructure.Enums;

namespace DbComparator.App.ViewModels
{
    public class MessagerViewModel : ModelBase, IMessagerVM
    {
        public event NotifyDelegate OkHandler;

        public event NotifyDelegate CloseHandler;

        private string _title;

        private string _message;

        private MbShowDialog _currentState;


        public string Title
        {
            get => _title; 
            set => SetProperty(ref _title, value, "Title"); 
        }

        public string Message
        {
            get => _message; 
            set => SetProperty(ref _message, value, "Message"); 
        }

        public MbShowDialog CurrentState
        {
            get => _currentState;
            set => SetProperty(ref _currentState, value, "CurrentState");
        }
          

        private RellayCommand _okCommand;

        private RellayCommand _closeCommand;



        public RellayCommand OkCommand => _okCommand ??
            (
                _okCommand = new RellayCommand(obj =>
                {
                    OkHandler?.Invoke();
                    CloseHandler?.Invoke();
                })
            );

        public RellayCommand CloseCommand =>
            _closeCommand = new RellayCommand((c) => CloseHandler?.Invoke());  
        
        public void ShowMessageBox(string title, string message, MbShowDialog state)
        {
            Title = title;
            Message = message;
            CurrentState = state;
        }
    }
}
