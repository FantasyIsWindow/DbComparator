﻿using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Delegates;
using DbComparator.App.Infrastructure.Enums;
using DbComparator.App.Services;
using DbConectionInfoRepository.Models;
using DbConectionInfoRepository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private int? _selectedIndex;

        private string[] _referencesType;

        private string _content;

        private bool _isEnabled;

        private List<string> _dbTypes;



        public DbInfoModel DbInfoModel
        {
            get => _dbInfoModel; 
            set => SetProperty(ref _dbInfoModel, value, "DbInfoModel"); 
        }

        public int? SelectedIndex
        {
            get => _selectedIndex; 
            set => SetProperty(ref _selectedIndex, value, "SelectedIndex"); 
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

        public List<string> DbTypes
        {
            get => _dbTypes; 
            set => SetProperty(ref _dbTypes, value, "DbTypes"); 
        }


        public DbInfoCreatorViewModel(IInfoDbRepository repository)
        {
            _repository = repository;
            _referencesType = new string[] { "Yes", "No" };
        }
        
        
        private RellayCommand _closeCommand;

        private RellayCommand _okCommand;



        public RellayCommand CloseCommand =>
            _closeCommand = new RellayCommand(c => CloseHandler?.Invoke());

        public RellayCommand OkCommand =>
            _okCommand = new RellayCommand(AddInfoToDb, CanAddInfoToDb);
        

        public void ShowManagerWindow(OpenStatus status, string isReference, DbInfoModel dbInfo = null)
        {
            _openStatus = status;
            _dbInfoModel = dbInfo ?? new DbInfoModel();
            _content = _openStatus == OpenStatus.Add ? "Add" : "Update";

            _selectedIndex = _openStatus == OpenStatus.Update || isReference != null ?
                             _dbInfoModel.Reference == "Yes"  || isReference == "Yes" ?
                           0 : 1 : -1;

            _isEnabled = _selectedIndex == -1 || _openStatus == OpenStatus.Update;

            DbTypes = _repository.GetAllTypes().ToList();
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