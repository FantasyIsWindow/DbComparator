using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Enums;
using DbComparator.App.Infrastructure.Extensions;
using DbComparator.App.Models;
using DbComparator.App.Services;
using DbConectionInfoRepository.Enums;
using DbConectionInfoRepository.Models;
using DbConectionInfoRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DbComparator.App.ViewModels
{
    public class MainWindowViewModel : ModelBase
    {
        private InfoDbConnection _connectionDb;

        private IRepository _dbRepository;

        private object _currentPageContent; 

        private ObservableCollection<string> _dbTypes; 

        private string _selectedDbType; 

        private DbInfo _selectedReferenceModel; 

        private DbInfo _selectedNotReferenceModel; 

        private ObservableCollection<DbInfo> _notReferenceInfoDbs; 

        private ObservableCollection<DbInfo> _referenceInfoDbs; 

        private ShowDbInfoViewModel _infoViewModel; 

        private ShowMessageViewModel _showMessage;

        private AddNewDbInfoViewModel _addNewDb;
 
        public ShowDbInfoViewModel InfoViewModel
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
            set =>_dbTypes = value;
        }

        public string SelectedDbType
        {
            get => _selectedDbType;
            set => SetProperty(ref _selectedDbType, value, "SelectedDbType");
        }

        public DbInfo SelectedReferenceModel
        {
            get => _selectedReferenceModel;
            set => SetProperty(ref _selectedReferenceModel, value, "SelectedReferenceModel");
        }

        public DbInfo SelectedNotReferenceModel
        {
            get => _selectedNotReferenceModel;
            set => SetProperty(ref _selectedNotReferenceModel, value, "SelectedNotReferenceModel");
        }

        public ObservableCollection<DbInfo> NotReferenceInfoDbs
        {
            get => _notReferenceInfoDbs;
            set =>_notReferenceInfoDbs = value;
        }

        public ObservableCollection<DbInfo> ReferenceInfoDbs
        {
            get => _referenceInfoDbs;
            set => _referenceInfoDbs = value;
        }


        public MainWindowViewModel()
        {
            _connectionDb = new InfoDbConnection();

            _dbTypes = new ObservableCollection<string>(_connectionDb.GetAllTypes());

            _referenceInfoDbs = new ObservableCollection<DbInfo>();
            _notReferenceInfoDbs = new ObservableCollection<DbInfo>();

            _infoViewModel = new ShowDbInfoViewModel();
            _infoViewModel.MessageHandler += ((sender, e) => { GetMessage(sender, e); });

            _showMessage = new ShowMessageViewModel();
            _showMessage.CloseHandler += (() => { CurrentPageContent = null; });

            _addNewDb = new AddNewDbInfoViewModel(new InfoDbConnection());
            _addNewDb.CloseHandler += (() => { CurrentPageContent = null; });
            _addNewDb.OkHandler += UpdateTypes;
            _addNewDb.MessageHandler += ((sender, e) => { GetMessage(sender, e); });
        }



        private RellayCommand _addNewDbInfoCommand;

        private RellayCommand _updateNotReferenceRecordCommand;

        private RellayCommand _updateReferenceRecordCommand;

        private RellayCommand _removeNotReferenceRecordCommand;

        private RellayCommand _removeReferenceRecordCommand;

        private RellayCommand _choiseDbTypeCommand;



        public RellayCommand AddNewDbInfoCommand
        {
            get
            {
                return _addNewDbInfoCommand ??
                    (_addNewDbInfoCommand = new RellayCommand(obj =>
                    {
                        var reference = obj as string;
                        _addNewDb.ShowMessage(OpenStatus.Add, reference);
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
                        UpdateRecord(SelectedNotReferenceModel.DataBase);
                    }, 
                        (obj) => obj != null ? SelectedNotReferenceModel != null && (bool)obj : false
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
                        UpdateRecord(SelectedReferenceModel.DataBase);
                    },
                        (obj) => obj != null ? SelectedReferenceModel != null && (bool)obj : false
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
                        ShowQuastionMessage(SelectedNotReferenceModel.DataBase);
                    },
                        (obj) => obj != null ? SelectedNotReferenceModel != null && (bool)obj : false
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
                        ShowQuastionMessage(SelectedReferenceModel.DataBase);
                    },
                        (obj) => obj != null ? SelectedReferenceModel != null && (bool)obj : false
                    ));
            }
        }
                                 
        public RellayCommand ChoiseDbTypeCommand =>
            _choiseDbTypeCommand = new RellayCommand(ChoiseType);

        private void UpdateTypes()
        {
            ChoiseType(null);
            CurrentPageContent = null;
        }

        private void UpdateRecord(DbInfoModel dbInfo)
        {
            try
            {
                _addNewDb.ShowMessage(OpenStatus.Update, null, dbInfo);
                CurrentPageContent = _addNewDb;
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }

        }

        private void ShowQuastionMessage(DbInfoModel model)
        {
            _showMessage.ShowMessageBox("Are you Sure?", MbShowDialog.OkCancelState);
            CurrentPageContent = _showMessage;
            _showMessage.OkHandler += (() => { RemoveRecord(model); });
        }
                       
        private void RemoveRecord(DbInfoModel dbInfo)
        {
            InfoDbConnection _repository = new InfoDbConnection();
            _repository.DeleteDbInfo(dbInfo);
            UpdateTypes();
        }

        private void ChoiseType(object obj)
        {
            _dbRepository = CreateRepository();
            GetDbInfo(ReferenceInfoDbs, IsReference.Yes);
            GetDbInfo(NotReferenceInfoDbs, IsReference.No);
        }

        private IRepository CreateRepository()
        {
            switch (SelectedDbType)
            {
                case "Microsoft Sql": { return new MicrosoftDb(); }
                case "SyBase":        { return new SyBaseDb();    }
            }
            return null;
        }

        private void GetDbInfo(ObservableCollection<DbInfo> collection, IsReference reference)
        {
            collection.ClearIfNotEmpty();
            var result = _connectionDb.GetAllReferenceDbByType(SelectedDbType, reference);
            GetDbTypes(result, collection);
        }

        private async void GetDbTypes(IEnumerable<DbInfoModel> models, ObservableCollection<DbInfo> target)
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
            target.Add(new DbInfo() { DataBase = null });
        }

        private void GetMessage(object sender, EventArgs e)
        {
            var message = (MessageEventArgs)e;
            ShowMessage(message.Message);           
        }

        private void ShowMessage(string message)
        {
            _showMessage.ShowMessageBox(message, MbShowDialog.OkState);
            CurrentPageContent = _showMessage;
        }
    }
}
