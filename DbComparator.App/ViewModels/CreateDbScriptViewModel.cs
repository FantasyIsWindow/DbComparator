using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Infrastructure.EventsArgs;
using DbComparator.App.Models;
using DbComparator.FileWorker;
using DbConectionInfoRepository.Models;
using System;
using System.Collections.Generic;

namespace DbComparator.App.ViewModels
{
    /// <summary>
    /// Sets the window opening model
    /// </summary>
    public enum CreatorMode { ScriptCreator, ScriptMerger }

    public class CreateDbScriptViewModel : ModelBase
    {
        public event EventHandler MessageHandler;

        public event NotifyDelegate CloseHandler;

        private readonly FileCreator _fileCreator;

        private readonly ScriptCollector _collector;

        private IRepository _dbRepository;

        private string _title;

        private readonly RepositoryFactory _repositoryFactory;

        private readonly List<Passage> _dbList;

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

        public List<Passage> DbList => _dbList;

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
                        try
                        {
                            string result = GettingAndSavingScript();
                            _fileCreator.SaveFile(ScriptDbName, result);
                            ResetParameters();
                            CloseHandler?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            SendMessage(MessageHandler, ex.Message);
                        }
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

        public RellayCommand CheckedCommand
        {
            get
            {
                return _checkedCommand ??
                    (_checkedCommand = new RellayCommand(obj =>
                    {
                        ProcessingMarkedControl();
                    }));
            }
        }

        /// <summary>
        /// Processing the marked control
        /// </summary>
        private void ProcessingMarkedControl()
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

        /// <summary>
        /// Getting and saving a script
        /// </summary>
        private string GettingAndSavingScript()
        {
            List<IRepository> reps = new List<IRepository>();

            foreach (var db in _dbList)
            {
                if (db.IsChecked)
                {
                    reps.Add(InitialRepository(db.Info));
                }
            }

            return CreateScript(reps);
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
        private IRepository InitialRepository(DbInfoModel model)
        {
            _dbRepository = _repositoryFactory.GetRepository(model.DbType);
            _dbRepository.CreateConnectionString(model.DataSource, model.ServerName, model.DbName, model.Login, model.Password);
            return _dbRepository;
        }

        /// <summary>
        /// Getting the script for the selected element
        /// </summary>
        private string CreateScript(List<IRepository> reps)
        {
            string result = "";

            if (IsCheckedTables && IsCheckedProcedures && IsCheckedTriggers)
            {
                ScriptDbName = "db" + ScriptDbName;
                result = _collector.GetScript(ScriptDbName, reps, DataEntities.db, SelectedProvider);
            }
            else if (IsCheckedTables)
            {
                ScriptDbName = "tables_" + ScriptDbName;
                result = _collector.GetScript(ScriptDbName, reps, DataEntities.tables, SelectedProvider);
            }
            else if (IsCheckedProcedures)
            {
                ScriptDbName = "procedures_" + ScriptDbName;
                result = _collector.GetScript(ScriptDbName, reps, DataEntities.procedures, SelectedProvider);
            }
            else if (IsCheckedTriggers)
            {
                ScriptDbName = "triggers_" + ScriptDbName;
                result = _collector.GetScript(ScriptDbName, reps, DataEntities.triggers, SelectedProvider);
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

        /// <summary>
        /// Sending a message
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="message">Message</param>
        private void SendMessage(EventHandler handler, string message)
        {
            if (handler != null)
            {
                MessageEventArgs eventArgs = new MessageEventArgs();
                eventArgs.Message = message;
                handler?.Invoke(this, eventArgs);
            }
        }
    }
}
