using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Models;
using System.Collections.Generic;

namespace DbComparator.App.ViewModels
{
    /// <summary>
    /// Sets parameters for displaying the message window
    /// </summary>
    public enum MbShowDialog { OkState, OkCancelState }


    public class MessagerViewModel : ModelBase, IMessagerVM
    {
        public event NotifyDelegate OkHandler;

        public event NotifyDelegate CloseHandler;

        private string _title;

        private string _message;

        private List<CompareResult> _compareResults;

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
        
        public List<CompareResult> CompareResults
        {
            get => _compareResults; 
            set => SetProperty(ref _compareResults, value, "CompareResults"); 
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
                    CompareResults = null;
                    OkHandler?.Invoke();
                    CloseHandler?.Invoke();
                })
            );

        public RellayCommand CloseCommand => _closeCommand ??
            (
                _closeCommand = new RellayCommand(obj =>
                {
                    CloseHandler?.Invoke();
                    OkHandler = null;
                })
            );

        /// <summary>
        /// Getting data to display
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="package">Package containing the message</param>
        /// <param name="state">Display status of the output window</param>
        public void ShowMessageBox(string title, object package, MbShowDialog state)
        {
            Title = title;
            CurrentState = state;

            switch (package)
            {
                case string str: Message = str; break;
                case List<CompareResult> result: CompareResults = result; break;
            }
        }
    }
}
