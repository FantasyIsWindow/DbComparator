using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Models;
using DbComparator.FileWorker;
using System.Collections.ObjectModel;

namespace DbComparator.App.ViewModels
{
    public class CreateDbScriptViewModel : ModelBase, ICreateDbScriptVM
    {
        public event NotifyDelegate CloseHandler;

        private readonly FileCreator _fileCreator;

        private IRepository _dbRepository;

        private ObservableCollection<Passage> _dbList;

        public ObservableCollection<Passage> DbList => _dbList;

        private bool _isCheckedTables;

        private bool _isCheckedProcedures;

        private bool _isCheckedTriggers;

        private Passage _checkedItem;

        private bool _isChecked;


        public bool IsCheckedTables
        {
            get => _isCheckedTables;
            set => SetProperty(ref _isCheckedTables, value, "IsCheckedTables");
        }

        public bool IsCheckedProcedures
        {
            get => _isCheckedProcedures;
            set => SetProperty(ref _isCheckedProcedures, value, "IsCheckedProcedures");
        }

        public bool IsCheckedTriggers
        {
            get => _isCheckedTriggers;
            set => SetProperty(ref _isCheckedTriggers, value, "IsCheckedTriggers");
        }

        public Passage CheckedItem
        {
            get => _checkedItem;
            set => SetProperty(ref _checkedItem, value, "CheckedItem");
        }

        public bool IsChecked
        {
            get => _isChecked; 
            set => SetProperty(ref _isChecked, value, "IsChecked");
        }




        public CreateDbScriptViewModel()
        {
            _fileCreator = new FileCreator();
            _dbList = new ObservableCollection<Passage>();
        }




        public void SetDbInfo(ObservableCollection<Passage> info, IRepository dbRepository)
        {
            _dbList = info;
            _dbRepository = dbRepository;
        }






        private RellayCommand _okCommand;

        private RellayCommand _cancelCommand;

        private RellayCommand _checkedCommand;



        public RellayCommand OkCommand
        {
            get
            {
                return _okCommand ??
                    (_okCommand = new RellayCommand(obj =>
                    {
                        CreateScript();
                        CloseHandler?.Invoke();
                    }));
            }
        }

        public RellayCommand CancelCommand =>
            _cancelCommand = new RellayCommand(c => CloseHandler?.Invoke());

        public RellayCommand CheckedCommand
        {
            get
            {
                return _checkedCommand ??
                    (_checkedCommand = new RellayCommand(obj =>
                    {
                        var item = obj as Passage;
                        CheckedItem = item;
                        IsChecked = true;
                    }));
            }
        }

        private void CreateScript()
        {
            string result = "";
            string type = "";

            if (IsCheckedTables && IsCheckedProcedures && IsCheckedTriggers)
            {
                result = _dbRepository.GetDbScript();
                type = "db_";
            }
            else if (IsCheckedTables)
            {
                result = _dbRepository.GetTablesScript();
                type = "tables_";
            }
            else if (IsCheckedProcedures)
            {
                result = _dbRepository.GetProceduresScript();
                type = "procedures_";
            }
            else if (IsCheckedTriggers)
            {
                result = _dbRepository.GetTriggersScript();
                type = "triggers_";
            }

            _fileCreator.SaveFile(type + _dbRepository.DbName, result);

            IsCheckedTables = false;
            IsCheckedProcedures = false;
            IsCheckedTriggers = false;
        }
    }
}
