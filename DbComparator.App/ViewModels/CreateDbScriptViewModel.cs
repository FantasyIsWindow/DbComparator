using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Models;
using DbComparator.FileWorker;
using DbConectionInfoRepository.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbComparator.App.ViewModels
{
    public enum CreatorMode { ScriptCreator, ScriptMerger, ScriptConverter }   
                

    public class CreateDbScriptViewModel : ModelBase
    {
        public event NotifyDelegate CloseHandler;

        private readonly FileCreator _fileCreator;

        private ScriptCollector _collector;

        private IRepository _dbRepository;

        private string _title;

        private readonly RepositoryFactory _repositoryFactory;

        private List<Passage> _dbList;

        public List<Passage> DbList => _dbList;

        private bool _isCheckedTables;

        private bool _isCheckedProcedures;

        private bool _isCheckedTriggers;

        private bool _isChecked;

        private CreatorMode _openMode;

        private Provider _selectedProvider;

        private string[] _providerList;

        private string _scriptDbName;
                     
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

        public bool IsChecked
        {
            get => _isChecked; 
            set => SetProperty(ref _isChecked, value, "IsChecked");
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value, "Title"); 
        }

        public CreatorMode OpenMode
        {
            get => _openMode; 
            set => SetProperty(ref _openMode, value, "OpenMode");
        }

        public Provider SelectedProvider
        {
            get => _selectedProvider;
            set => SetProperty(ref _selectedProvider, value, "SelectedProvider");
        }

        public string[] ProviderList
        {
            get => _providerList;
        }

        public string ScriptDbName
        {
            get => _scriptDbName;
            set => SetProperty(ref _scriptDbName, value, "ScriptDbName");
        }

        public CreateDbScriptViewModel(RepositoryFactory repositoryFactory, List<Passage> info, CreatorMode openMode)
        {
            _repositoryFactory = repositoryFactory;
            _dbList = info;
            _openMode = openMode;
            _fileCreator = new FileCreator();
            _collector = new ScriptCollector();
            InitializeData();
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
                        GettingAndSavingScript();
                        ResetParameters();
                        CloseHandler?.Invoke();
                    }));
            }
        }
               

        public RellayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ??
                    (_cancelCommand = new RellayCommand(obj =>
                    {
                        ResetParameters();
                        CloseHandler?.Invoke();
                    }));
            }
        }

        public RellayCommand CheckedCommand =>
            _checkedCommand = new RellayCommand( (obj) => 
                ProcessingMarkedControl(obj));

        /// <summary>
        /// Processing the marked control
        /// </summary>
        /// <param name="obj">Marked object</param>
        private void ProcessingMarkedControl(object obj)
        {
            if (_openMode == CreatorMode.ScriptCreator)
            {
                var item = (obj as Passage).Info;
                InitialRepository(item);
                IsChecked = true;
                ScriptDbName = item.DbName;
            }
            else if (_openMode == CreatorMode.ScriptMerger)
            {
                IsChecked = false;
                ScriptDbName = "";
                foreach (var db in _dbList)
                {
                    if (db.IsChecked)
                    {
                        IsChecked = true;
                        ScriptDbName += db.Info.DbName + "_";
                    }
                }
            }
        }

        /// <summary>
        /// Getting and saving a script
        /// </summary>
        private void GettingAndSavingScript()
        {
            StringBuilder result = new StringBuilder();
            if (_openMode == CreatorMode.ScriptCreator)
            {
                result.Append(CreateScript());
            }
            else if (_openMode == CreatorMode.ScriptMerger)
            {
                List<DbInfoModel> models = new List<DbInfoModel>();
                foreach (var db in _dbList)
                {
                    if (db.IsChecked)
                    {
                        models.Add(db.Info);
                    }
                }

                foreach (var model in models)
                {
                    InitialRepository(model);
                    result.Append(CreateScript());
                }
            }

            _fileCreator.SaveFile(ScriptDbName, result.ToString());
        }

        /// <summary>
        /// Data initialization
        /// </summary>
        private void InitializeData()
        {
            if (_openMode == CreatorMode.ScriptCreator)
            {
                Title = "Create db script"; 
            }
            else if (_openMode == CreatorMode.ScriptMerger)
            {
                Title = "Merge db scripts";
            }

            int size = Enum.GetValues(typeof(Provider)).Length - 1;
            _providerList = new string[size];
            int i = 0;
            foreach (Provider provider in Enum.GetValues(typeof(Provider)))
            {
                if (provider != Provider.All)
                {
                    _providerList[i++] = provider.ToString();
                }
            }
        }

        /// <summary>
        /// Repository initialization
        /// </summary>
        /// <param name="model">Data to connect to the database</param>
        private void InitialRepository(DbInfoModel model)
        {
            _dbRepository = _repositoryFactory.GetRepository(model.DbType);
            _dbRepository.CreateConnectionString(model.DataSource, model.ServerName, model.DbName, model.Login, model.Password);
            _collector.SetRepository(_dbRepository);
        }

        /// <summary>
        /// Getting the script for the selected element
        /// </summary>
        private string CreateScript()
        {
            string result = "";

            if (IsCheckedTables && IsCheckedProcedures && IsCheckedTriggers)
            {
                result = _collector.GetDbScript(SelectedProvider);
                ScriptDbName = "db" + ScriptDbName;
            }
            else if (IsCheckedTables)
            {
                result = _collector.GetTablesScript(SelectedProvider);
                ScriptDbName = "tables_" + ScriptDbName;
            }
            else if (IsCheckedProcedures)
            {
                result = _collector.GetProceduresScript(SelectedProvider);
                ScriptDbName = "procedures_" + ScriptDbName;
            }
            else if (IsCheckedTriggers)
            {
                result = _collector.GetTriggersScript(SelectedProvider);
                ScriptDbName = "triggers_" + ScriptDbName;
            }

            return result;
        }

        /// <summary>
        /// Resetting variable parameters
        /// </summary>
        private void ResetParameters()
        {
            IsCheckedTables = false;
            IsCheckedProcedures = false;
            IsCheckedTriggers = false;
            IsChecked = false;
        }
    }
}
