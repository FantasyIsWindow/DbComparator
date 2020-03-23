using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Enums;
using DbComparator.App.Infrastructure.EventsArgs;
using DbComparator.App.Infrastructure.Extensions;
using DbComparator.App.Models;
using DbConectionInfoRepository.Enums;
using DbConectionInfoRepository.Models;
using DbConectionInfoRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DbComparator.App.ViewModels
{
    public class MainWindowViewModel : ModelBase
    {
        private IInfoDbRepository _connectionDb;

        private IRepository _dbRepository;

        private IGeneralDbInfoVM _infoViewModel;

        private IMessagerVM _showMessage;

        private IDbInfoCreatorVM _addNewDb;

        private IAboutVM _aboutVM;

        private object _currentPageContent;

        private ObservableCollection<string> _dbTypes;

        private Provider _selectedDbType;

        private DbInfo _selectedRefModel;

        private DbInfo _selectedNotRefModel;

        private ObservableCollection<DbInfo> _referenceInfoDbs;

        private ObservableCollection<DbInfo> _notReferenceInfoDbs;

        private bool _loadViewVisibility;
                     

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

        public ObservableCollection<string> DbTypes
        {
            get => _dbTypes;
            set => _dbTypes = value;
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

        public MainWindowViewModel(IInfoDbRepository connectionDb, 
                                   IGeneralDbInfoVM generalDbInfo,
                                   IMessagerVM messager, 
                                   IDbInfoCreatorVM dbInfoManager, 
                                   IAboutVM aboutVM)
        {
            _connectionDb = connectionDb;

            _dbTypes = new ObservableCollection<string>(_connectionDb.GetAllTypes());
            _referenceInfoDbs = new ObservableCollection<DbInfo>();
            _notReferenceInfoDbs = new ObservableCollection<DbInfo>();
            _infoViewModel = generalDbInfo;
            _showMessage = messager;
            _addNewDb = dbInfoManager;
            _aboutVM = aboutVM;
            Subscriptions();
        }

        private void Subscriptions()
        {
            _infoViewModel.MessageHandler += ((sender, e) => { GetMessage(sender, e); });
            _showMessage.CloseHandler += (() => { CurrentPageContent = null; });
            _addNewDb.CloseHandler += (() => { CurrentPageContent = null; });
            _addNewDb.OkHandler += UpdateTypes;
            _addNewDb.MessageHandler += ((sender, e) => { GetMessage(sender, e); });
            _aboutVM.CloseHandler += (() => { CurrentPageContent = null; });
        }


        private RellayCommand _addNewDbInfoCommand;

        private RellayCommand _updateNotReferenceRecordCommand;

        private RellayCommand _updateReferenceRecordCommand;

        private RellayCommand _removeNotReferenceRecordCommand;

        private RellayCommand _removeReferenceRecordCommand;

        private RellayCommand _choiseDbTypeCommand;

        private RellayCommand _aboutViewShowCommand;


        public RellayCommand AddNewDbInfoCommand
        {
            get
            {
                return _addNewDbInfoCommand ??
                    (_addNewDbInfoCommand = new RellayCommand(obj =>
                    {                       
                        var reference = obj as string;
                        if (reference != null)
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

        private void UpdateTypes()
        {
            ChoiseType();
            CurrentPageContent = null;
        }

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

        private void ShowQuastionMessage(DbInfoModel model)
        {
            ShowMessage("Warning", "Are you Sure?", MbShowDialog.OkCancelState);
            _showMessage.OkHandler += (() => { RemoveRecord(model); });
        }

        private void RemoveRecord(DbInfoModel dbInfo)
        {
            InfoDbConnection _repository = new InfoDbConnection();
            _repository.DeleteDbInfo(dbInfo);
            UpdateTypes();
        }

        private async void ChoiseType()
        {
            LoadViewVisibility = true;
            _dbRepository = RepositoryFactory.GetRepository(SelectedDbType);
            await GetDbInfo(ReferenceInfoDbs, IsReference.Yes);
            await GetDbInfo(NotReferenceInfoDbs, IsReference.No);
            LoadViewVisibility = false;
        }

        private async Task GetDbInfo(ObservableCollection<DbInfo> collection, IsReference reference)
        {
            collection.ClearIfNotEmpty();
            var result = _connectionDb.GetAllReferenceDbByType(SelectedDbType.ToString(), reference);
            await GetDbTypes(result, collection);
        }

        private async Task GetDbTypes(IEnumerable<DbInfoModel> models, ObservableCollection<DbInfo> target)
        {
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
            if (target.Count >= 0)
            {
                target.Add(new DbInfo() { DataBase = null });
            }            
        }

        private void GetMessage(object sender, EventArgs e)
        {
            var message = (MessageEventArgs)e;
            ShowMessage("Warning", message.Message, MbShowDialog.OkState);
        }

        private void ShowMessage(string title, object message, MbShowDialog state)
        {
            _showMessage.ShowMessageBox(title, message, state);
            CurrentPageContent = _showMessage;
        }
    }
}
