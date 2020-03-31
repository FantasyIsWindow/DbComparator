using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.EventsArgs;
using DbComparator.App.Infrastructure.Extensions;
using DbComparator.App.Models;
using DbComparator.App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DbComparator.App.ViewModels
{
    public enum DepthOfCleaning { Low, Medium, High }


    public class GeneralDbInfoViewModel : ModelBase, IGeneralDbInfoVM
    {
        public Action<Property> GetAction => ((x) => ItemSelection(x));

        public event EventHandler MessageHandler;

        public event EventHandler CurrentEntitiesMessageHandler;



        private CollectionEqualizer _collectionEqualizer;

        private RepositoryFactory _repositoryFactory;

        private GeneralComparator _generalComparator;

        private IRepository _lsRepository; 

        private IRepository _rsRepository;

        private DbInfo _lsReceiver; 

        private DbInfo _rsReceiver;

        private ObservableCollection<GeneralDbInfo> _lsGeneralInfo; 

        private ObservableCollection<GeneralDbInfo> _rsGeneralInfo;

        private ObservableCollection<DtoFullField> _lsTableFields;

        private ObservableCollection<DtoFullField> _rsTableFields;

        private string _lsScript;

        private string _rsScript;

        private string _leftCompared;

        private string _rightCompared;

        private bool _isAuto;



        public DbInfo LsReceiver
        {
            get => _lsReceiver;
            set
            {
                if (value != null)
                {
                    DataContextChanged(value, ref _lsRepository);
                }
                SetProperty(ref _lsReceiver, value, "LsReceiver");
            }
        }

        public DbInfo RsReceiver
        {
            get => _rsReceiver;
            set
            {
                if (value != null)
                {
                    DataContextChanged(value, ref _rsRepository);
                }
                SetProperty(ref _rsReceiver, value, "RsReceiver");
            }
        }

        public ObservableCollection<GeneralDbInfo> LsGeneralInfo
        {
            get => _lsGeneralInfo;
            set => SetProperty(ref _lsGeneralInfo, value, "LsGeneralInfo");
        }

        public ObservableCollection<GeneralDbInfo> RsGeneralInfo
        {
            get => _rsGeneralInfo;
            set => SetProperty(ref _rsGeneralInfo, value, "RsGeneralInfo");
        }

        public ObservableCollection<DtoFullField> LsTableFields
        {
            get => _lsTableFields;
            set => SetProperty(ref _lsTableFields, value, "LsTableFields");
        }

        public ObservableCollection<DtoFullField> RsTableFields
        {
            get => _rsTableFields;
            set => SetProperty(ref _rsTableFields, value, "RsTableFields");
        }

        public string LsScript
        {
            get => _lsScript;
            set => SetProperty(ref _lsScript, value, "LsScript");
        }

        public string RsScript
        {
            get => _rsScript;
            set => SetProperty(ref _rsScript, value, "RsScript");
        }

        public string LeftCompared
        {
            get => _leftCompared;
            set => SetProperty(ref _leftCompared, value, "LeftCompared");
        }

        public string RightCompared
        {
            get => _rightCompared;
            set => SetProperty(ref _rightCompared, value, "RightCompared");
        }     
        
        public bool IsAuto
        {
            get => _isAuto;
            set => SetProperty(ref _isAuto, value, "IsAuto");
        }

        public GeneralDbInfoViewModel()
        {
            _lsGeneralInfo = new ObservableCollection<GeneralDbInfo>();
            _rsGeneralInfo = new ObservableCollection<GeneralDbInfo>();
            _repositoryFactory = new RepositoryFactory();
            _generalComparator = new GeneralComparator();
            _collectionEqualizer = new CollectionEqualizer();
            _isAuto = false;
        }



        private RellayCommand _backCommand;

        private RellayCommand _autoCompareCommand;

        private RellayCommand _compareCommand;


        public RellayCommand BackCommand =>
            _backCommand = new RellayCommand((c) => { ClearInfoObjects(DepthOfCleaning.Low); });

        public RellayCommand AutoCompareCommand => 
            _autoCompareCommand = new RellayCommand(AutoCompare);

        public RellayCommand CompareCommand =>
            _compareCommand = new RellayCommand(CompareGeneralDbInfo);

        private void ItemSelection(Property property)
        {
            if (property != null && property.Name != "null")
            {
                string message = "";
                switch (property.PropertyType)
                {
                    case "Table": 
                        {
                            var str = GetFields(property.Name);
                            message = "Tables: " + str;
                            break;
                        }
                    case "Procedure": 
                        {
                            var str = GetScript(_lsRepository.GetProcedureSqript, _rsRepository.GetProcedureSqript, property.Name);
                            message = "Procedures: " + str;
                            break;
                        }
                    case "Trigger":
                        {
                            var str = GetScript(_lsRepository.GetTriggerSqript, _rsRepository.GetTriggerSqript, property.Name);
                            message = "Triggers: " + str;
                            break;
                        }
                }
                SendMessage(CurrentEntitiesMessageHandler, message);
            }
        }

        public void ClearInfoObjects(DepthOfCleaning depth)
        {
            switch (depth)
            {
                case DepthOfCleaning.High:
                    {
                        LsReceiver = null;
                        RsReceiver = null;
                        goto case DepthOfCleaning.Medium;
                    }
                case DepthOfCleaning.Medium:
                    {
                        LsGeneralInfo.Clear();
                        RsGeneralInfo.Clear();
                        goto case DepthOfCleaning.Low;
                    }
                case DepthOfCleaning.Low:
                    {
                        LsTableFields = null;
                        RsTableFields = null;
                        LsScript = null;
                        RsScript = null;
                        break;
                    }
            }
            SendMessage(CurrentEntitiesMessageHandler, "");
        }

        private void AutoCompare(object obj)
        {
            var result = _generalComparator.Colorize(LsGeneralInfo[0], RsGeneralInfo[0]);
            IsAuto = true;
            SendMessage(MessageHandler, result);
        }

        private void CompareGeneralDbInfo(object obj)
        {
            IsAuto = false;

            List<GeneralDbInfo> test_01 = new List<GeneralDbInfo>();
            List<GeneralDbInfo> test_02 = new List<GeneralDbInfo>();

            _generalComparator.CompareGeneralInfo(_lsRepository, _rsRepository, test_01, test_02);

            LsGeneralInfo = new ObservableCollection<GeneralDbInfo>(test_01);
            RsGeneralInfo = new ObservableCollection<GeneralDbInfo>(test_02);
        }

        private string GetFields(string name)
        {
            LsTableFields = _lsRepository.GetFieldsInfo(name).ToObservableCollection();
            RsTableFields = _rsRepository.GetFieldsInfo(name).ToObservableCollection();

            string lS = LsTableFields.Count != 0 ? name : "null";
            string rS = RsTableFields.Count != 0 ? name : "null";

            _collectionEqualizer.FieldsAlignment(LsTableFields, RsTableFields);

            return $"{lS} ↔ {rS}";
        }

        private string GetScript(Func<string, string> lsFunc, Func<string, string> rsFunc, string name)
        {
            LeftCompared = lsFunc(name) ?? " ";
            RightCompared = rsFunc(name) ?? " ";
            LsScript = LeftCompared;
            RsScript = RightCompared;

            string lS = LeftCompared != " " ? name : "null";
            string rS = RightCompared != " " ? name : "null";

            return $"{lS} ↔ {rS}";
        }

        private void DataContextChanged(DbInfo db, ref IRepository repository)
        {
            if (repository == null || db.DataBase.DbType != repository.DbType)
            {
                repository = _repositoryFactory.GetRepository(db.DataBase.DbType);
            }
            ClearInfoObjects(DepthOfCleaning.Medium);
            repository.CreateConnectionString(db.DataBase.DataSource, 
                                              db.DataBase.ServerName, 
                                              db.DataBase.DbName, 
                                              db.DataBase.Login, 
                                              db.DataBase.Password);
        }

        private void SendMessage(EventHandler handler, object package)
        {
            if (handler != null)
            {
                MessageEventArgs eventArgs = new MessageEventArgs();
                eventArgs.Message = package;
                handler(this, eventArgs);
            }
        }
    }
}
