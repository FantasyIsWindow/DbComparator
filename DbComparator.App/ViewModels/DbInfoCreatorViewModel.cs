using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Infrastructure.Enums;
using DbComparator.App.Services;
using DbConectionInfoRepository.Models;
using DbConectionInfoRepository.Repositories;
using System;

namespace DbComparator.App.ViewModels
{ 
    public class DbInfoCreatorViewModel : ModelBase, IDbInfoCreatorVM
    {
        public event NotifyDelegate OkHandler;

        public event NotifyDelegate CloseHandler;

        public event EventHandler MessageHandler;


        private readonly IInfoDbRepository _repository;  
        
        private OpenStatus _openStatus;

        private DbInfoModel _dbInfoModel;

        private string[] _referencesType;

        private string _content;

        private bool _isEnabled;

        private string[] _dbTypes;



        public DbInfoModel DbInfoModel
        {
            get => _dbInfoModel; 
            set => SetProperty(ref _dbInfoModel, value, "DbInfoModel"); 
        }

        public string[] ReferencesType
        {
            get => _referencesType; 
        }

        public string Content
        {
            get => _content;
        }
               
        public bool IsEnabled
        {
            get => _isEnabled; 
        }

        public string[] DbTypes
        {
            get => _dbTypes; 
        }


        public DbInfoCreatorViewModel(IInfoDbRepository repository)
        {
            _repository = repository;
            _referencesType = new string[] { "Yes", "No" };
            _dbTypes = new[] { "Microsoft Sql", "SyBase", "MySql" };
        }
        
        
        private RellayCommand _closeCommand;

        private RellayCommand _okCommand;



        public RellayCommand CloseCommand =>
            _closeCommand = new RellayCommand(c => CloseHandler?.Invoke());

        public RellayCommand OkCommand =>
            _okCommand = new RellayCommand(AddInfoToDb, CanAddInfoToDb);
        
        public void ShowManagerWindow(OpenStatus status, DbInfoModel dbInfo = null)
        {
            _openStatus = status;
            _dbInfoModel = dbInfo ?? new DbInfoModel();
            _content = _openStatus.ToString();
            _isEnabled = (_openStatus == OpenStatus.Update || dbInfo != null) ? false : true;
        }


        private void AddInfoToDb(object obj)
        {
            try
            {
                switch (_openStatus)
                {
                    case OpenStatus.Add: { _repository.AddNewRecord(_dbInfoModel); break; }
                    case OpenStatus.Update: { _repository.UpdateDbInfo(_dbInfoModel); break; }
                }
                OkHandler?.Invoke();
            }
            catch (Exception ex)
            {
                SendMessage(ex.Message);
            }
        }

        private bool CanAddInfoToDb(object obj) =>
            _dbInfoModel.ServerName  != null &&
            _dbInfoModel.DbName      != null &&
            _dbInfoModel.DbType      != null &&
            _dbInfoModel.Reference   != null;       
        
        private void SendMessage(string message)
        {
            if (MessageHandler != null)
            {
                MessageEventArgs eventArgs = new MessageEventArgs();
                eventArgs.Message = message;
                MessageHandler?.Invoke(this, eventArgs);
            }
        }
    }
}
