﻿using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.EventsArgs;
using DbComparator.App.Models;
using DbConectionInfoRepository.Models;
using DbConectionInfoRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DbComparator.App.ViewModels
{
    public class MainWindowViewModel : ModelBase
    {
        public string MyProperty { get; }

        private readonly RepositoryFactory _repositoryFactory;

        private readonly IInfoDbRepository _connectionDb;

        private IRepository _dbRepository;

        private IGeneralDbInfoVM _infoViewModel;

        private readonly IMessagerVM _showMessage;

        private readonly IDbInfoCreatorVM _addNewDb;

        private readonly IAboutVM _aboutVM;

        private object _currentPageContent;

        private Provider _selectedDbType;

        private DbInfo _selectedRefModel;

        private DbInfo _selectedNotRefModel;

        private ObservableCollection<DbInfo> _referenceInfoDbs;

        private ObservableCollection<DbInfo> _notReferenceInfoDbs;

        private bool _loadViewVisibility;

        private string _statusBarMessage;

        public IGeneralDbInfoVM InfoViewModel
        {
            get => _infoViewModel;
            set => SetProperty(ref _infoViewModel, value, "InfoViewModel");
        }

        public object CurrentPageContent
        {
            get => _currentPageContent;
            set => SetProperty(ref _currentPageContent, value, "CurrentPageContent");
        }

        public Provider SelectedDbType
        {
            get => _selectedDbType;
            set => SetProperty(ref _selectedDbType, value, "SelectedDbType");
        }

        public DbInfo SelectedRefModel
        {
            get => _selectedRefModel;
            set => SetProperty(ref _selectedRefModel, value, "SelectedRefModel");
        }

        public DbInfo SelectedNotRefModel
        {
            get => _selectedNotRefModel;
            set => SetProperty(ref _selectedNotRefModel, value, "SelectedNotRefModel");
        }

        public ObservableCollection<DbInfo> ReferenceInfoDbs
        {
            get => _referenceInfoDbs;
            set => _referenceInfoDbs = value;
        }

        public ObservableCollection<DbInfo> NotReferenceInfoDbs
        {
            get => _notReferenceInfoDbs;
            set => _notReferenceInfoDbs = value;
        }

        public bool LoadViewVisibility
        {
            get => _loadViewVisibility;
            set => SetProperty(ref _loadViewVisibility, value, "LoadViewVisibility");
        }

        public string StatusBarMessage
        {
            get => _statusBarMessage;
            set => SetProperty(ref _statusBarMessage, value, "StatusBarMessage");
        }

        public MainWindowViewModel(IInfoDbRepository connectionDb,
                                   IGeneralDbInfoVM generalDbInfo,
                                   IMessagerVM messager,
                                   IDbInfoCreatorVM dbInfoManager,
                                   IAboutVM aboutVM)
        {
            _repositoryFactory = new RepositoryFactory();
            _connectionDb = connectionDb;
            _referenceInfoDbs = new ObservableCollection<DbInfo>();
            _notReferenceInfoDbs = new ObservableCollection<DbInfo>();
            _infoViewModel = generalDbInfo;
            _showMessage = messager;
            _addNewDb = dbInfoManager;
            _aboutVM = aboutVM;
            Subscriptions();
        }

        /// <summary>
        /// The implementation of subscriptions
        /// </summary>
        private void Subscriptions()
        {
            _infoViewModel.MessageHandler += ((sender, e) => { GetMessage("Comparison result", sender, e); });
            _infoViewModel.CurrentEntitiesMessageHandler += ((sender, e) =>
            {
                var message = (MessageEventArgs)e;
                StatusBarMessage = (string)message.Message;
            });

            _showMessage.CloseHandler += (() => { CurrentPageContent = null; });

            _addNewDb.CloseHandler += (() => { CurrentPageContent = null; });
            _addNewDb.OkHandler += UpdateTypes;
            _addNewDb.MessageHandler += ((sender, e) => { GetMessage("Warning", sender, e); });
            _aboutVM.CloseHandler += (() => { CurrentPageContent = null; });
        }

        private RellayCommand _addNewDbInfoCommand;

        private RellayCommand _updateNotReferenceRecordCommand;

        private RellayCommand _updateReferenceRecordCommand;

        private RellayCommand _removeNotReferenceRecordCommand;

        private RellayCommand _removeReferenceRecordCommand;

        private RellayCommand _choiseDbTypeCommand;

        private RellayCommand _aboutViewShowCommand;

        private RellayCommand _openCreateDbScriptCommand;

        private RellayCommand _mergeDbScriptCommand;

        public RellayCommand AddNewDbInfoCommand
        {
            get
            {
                return _addNewDbInfoCommand ??
                    (_addNewDbInfoCommand = new RellayCommand(obj =>
                    {
                        if (obj is string reference)
                        {
                            DbInfoModel model = new DbInfoModel { Reference = reference, DbType = SelectedDbType.ToString() };
                            _addNewDb.ShowManagerWindow(OpenStatus.Add, model);
                        }
                        else
                        {
                            _addNewDb.ShowManagerWindow(OpenStatus.Add);
                        }
                        CurrentPageContent = _addNewDb;
                    }));
            }
        }

        public RellayCommand UpdateNotReferenceRecordCommand
        {
            get
            {
                return _updateNotReferenceRecordCommand ??
                    (_updateNotReferenceRecordCommand = new RellayCommand(obj =>
                    {
                        UpdateRecord(SelectedNotRefModel.DataBase);
                    },
                        (obj) => obj != null ? SelectedNotRefModel != null && (bool)obj : false
                    ));
            }
        }

        public RellayCommand UpdateReferenceRecordCommand
        {
            get
            {
                return _updateReferenceRecordCommand ??
                    (_updateReferenceRecordCommand = new RellayCommand(obj =>
                    {
                        UpdateRecord(SelectedRefModel.DataBase);
                    },
                        (obj) => obj != null ? SelectedRefModel != null && (bool)obj : false
                    ));
            }
        }

        public RellayCommand RemoveNotReferenceRecordCommand
        {
            get
            {
                return _removeNotReferenceRecordCommand ??
                    (_removeNotReferenceRecordCommand = new RellayCommand(obj =>
                    {
                        ShowQuastionMessage(SelectedNotRefModel.DataBase);
                    },
                        (obj) => obj != null ? SelectedNotRefModel != null && (bool)obj : false
                    ));
            }
        }

        public RellayCommand RemoveReferenceRecordCommand
        {
            get
            {
                return _removeReferenceRecordCommand ??
                    (_removeReferenceRecordCommand = new RellayCommand(obj =>
                    {
                        ShowQuastionMessage(SelectedRefModel.DataBase);
                    },
                        (obj) => obj != null ? SelectedRefModel != null && (bool)obj : false
                    ));
            }
        }

        public RellayCommand ChoiseDbTypeCommand
        {
            get
            {
                return _choiseDbTypeCommand ??
                    (_choiseDbTypeCommand = new RellayCommand(obj =>
                    {
                        _infoViewModel.ClearInfoObjects(DepthOfCleaning.High);
                        ChoiseType();
                    }));
            }
        }

        public RellayCommand AboutViewShowCommand =>
            _aboutViewShowCommand = new RellayCommand((c) => { CurrentPageContent = _aboutVM; });

        public RellayCommand OpenCreateDbScriptCommand
        {
            get
            {
                return _openCreateDbScriptCommand ??
                   (_openCreateDbScriptCommand = new RellayCommand(obj =>
                   {
                       OpenScriptCreator(CreatorMode.ScriptCreator);
                   },
                        (obj) => _dbRepository != null
                   ));
            }
        }

        public RellayCommand MergeDbScriptCommand
        {
            get
            {
                return _mergeDbScriptCommand ??
                    (_mergeDbScriptCommand = new RellayCommand(obj =>
                    {
                        OpenScriptCreator(CreatorMode.ScriptMerger);
                    },
                        (obj) => _dbRepository != null
                    ));
            }
        }

        /// <summary>
        /// Preparing and transmitting data for the script Creator window
        /// </summary>
        /// <param name="mode">Creator Mode</param>
        private void OpenScriptCreator(CreatorMode mode)
        {
            List<DbInfo> tempCollection = new List<DbInfo>();
            tempCollection.AddRange(_referenceInfoDbs.Where(s => s.IsConnect == true && s.DataBase != null));
            tempCollection.AddRange(_notReferenceInfoDbs.Where(s => s.IsConnect == true && s.DataBase != null));

            List<Passage> resultCollection = new List<Passage>();
            foreach (Provider item in Enum.GetValues(typeof(Provider)))
            {
                if (item != Provider.All)
                {
                    foreach (var db in tempCollection)
                    {
                        if (db.DataBase.DbType == item.ToString() && db.DataBase.DbName != null)
                        {
                            resultCollection.Add(new Passage() { Info = db.DataBase, IsChecked = false });
                        }
                    }
                }
            }

            CreateDbScriptViewModel createDbScriptVM = new CreateDbScriptViewModel(_repositoryFactory, resultCollection, mode);
            createDbScriptVM.MessageHandler += ((sender, e) => { GetMessage("Warning", sender, e); });
            createDbScriptVM.CloseHandler += (() => { CurrentPageContent = null; });
            CurrentPageContent = createDbScriptVM;
        }

        /// <summary>
        /// Updating a database entry
        /// </summary>
        /// <param name="dbInfo">Updated record</param>
        private void UpdateRecord(DbInfoModel dbInfo)
        {
            try
            {
                _addNewDb.ShowManagerWindow(OpenStatus.Update, dbInfo);
                CurrentPageContent = _addNewDb;
            }
            catch (Exception ex)
            {
                ShowMessage("Warning", ex.Message, MbShowDialog.OkState);
            }
        }

        /// <summary>
        /// Output of the notification window
        /// </summary>
        /// <param name="model">Record</param>
        private void ShowQuastionMessage(DbInfoModel model)
        {
            ShowMessage("Warning", "Are you Sure?", MbShowDialog.OkCancelState);
            _showMessage.OkHandler += (() => { RemoveRecord(model); });
        }

        /// <summary>
        /// Deleting an existing record
        /// </summary>
        /// <param name="dbInfo">Delete records</param>
        private void RemoveRecord(DbInfoModel dbInfo)
        {
            InfoDbConnection _repository = new InfoDbConnection();
            _repository.DeleteDbInfo(dbInfo);
            UpdateTypes(null, null);
        }

        /// <summary>
        /// Updating information about providers
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void UpdateTypes(object sender, EventArgs e)
        {
            if (e != null)
            {
                var type = (MessageEventArgs)e;
                SelectedDbType = _repositoryFactory.StringToProvider((string)type.Message);
            }

            ReferenceInfoDbs.Clear();
            NotReferenceInfoDbs.Clear();

            ChoiseType();
            CurrentPageContent = null;
        }

        /// <summary>
        /// Choosing a provider
        /// </summary>
        private async void ChoiseType()
        {
            LoadViewVisibility = true;
            ReferenceInfoDbs.Clear();
            NotReferenceInfoDbs.Clear();
            await GetDbInfo(ReferenceInfoDbs, IsReference.Yes);
            await GetDbInfo(NotReferenceInfoDbs, IsReference.No);
            LoadViewVisibility = false;
        }

        /// <summary>
        /// Get basic information about the database
        /// </summary>
        /// <param name="collection">Collection to fill</param>
        /// <param name="reference">Is the database a reference database</param>
        /// <returns>Task</returns>
        private async Task GetDbInfo(ObservableCollection<DbInfo> collection, IsReference reference)
        {
            IEnumerable<DbInfoModel> result = null;
            if (SelectedDbType == Provider.All)
            {
                foreach (Provider item in Enum.GetValues(typeof(Provider)))
                {
                    if (item == Provider.All)
                    {
                        continue;
                    }

                    _dbRepository = _repositoryFactory.GetRepository(item);
                    result = _connectionDb.GetAllDbByType(item.ToString(), reference);
                    await GetInfo(result, collection);
                }
            }
            else
            {
                _dbRepository = _repositoryFactory.GetRepository(SelectedDbType);
                result = _connectionDb.GetAllDbByType(SelectedDbType.ToString(), reference);
                await GetInfo(result, collection);
            }
        }

        /// <summary>
        /// Collecting information from the resulting list of databases
        /// </summary>
        /// <param name="models">Data to connect to the database</param>
        /// <param name="target">Collection to fill</param>
        /// <returns>Task</returns>
        private async Task GetInfo(IEnumerable<DbInfoModel> models, ObservableCollection<DbInfo> target)
        {
            if (SelectedDbType == Provider.All)
            {
                target.Add(new DbInfo()
                {
                    DataBase = new DbInfoModel()
                    {
                        DbType = _dbRepository.DbType
                    }
                });
            }

            foreach (var item in models)
            {
                _dbRepository.CreateConnectionString(item.DataSource, item.ServerName, item.DbName, item.Login, item.Password);
                var conn = await _dbRepository.IsConnectionAsync();
                DbInfo db = new DbInfo()
                {
                    DataBase = item,
                    IsConnect = conn
                };
                target.Add(db);
            }

            if (target.Count >= 0 && SelectedDbType != Provider.All)
            {
                target.Add(new DbInfo()
                {
                    DataBase = null
                });
            }
        }

        /// <summary>
        /// Receiving messages and opening the message window
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event</param>
        private void GetMessage(string title, object sender, EventArgs e)
        {
            var message = (MessageEventArgs)e;
            ShowMessage(title, message.Message, MbShowDialog.OkState);
        }

        /// <summary>
        /// The call of the message box
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="message">Message with data to display</param>
        /// <param name="state">The display settings window</param>
        private void ShowMessage(string title, object message, MbShowDialog state)
        {
            _showMessage.ShowMessageBox(title, message, state);
            CurrentPageContent = _showMessage;
        }
    }
}
