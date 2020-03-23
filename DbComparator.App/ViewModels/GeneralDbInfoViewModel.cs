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
using System.Linq;

namespace DbComparator.App.ViewModels
{
    public enum DepthOfCleaning { Low, Medium, High }


    public class GeneralDbInfoViewModel : ModelBase, IGeneralDbInfoVM
    {
        public Action<Property> GetAction => ((x) => ItemSelection(x));

        public event EventHandler MessageHandler;



        private CollectionEqualizer _collectionEqualizer;

        private FieldsEqualizer _fieldsEqualizer;

        private AutoComparator _autoComparator;

        private IRepository _lsRepository; 

        private IRepository _rsRepository;

        private DbInfo _lsReceiver; 

        private DbInfo _rsReceiver;

        private ObservableCollection<GeneralDbInfo> _lsGeneralInfo; 

        private ObservableCollection<GeneralDbInfo> _rsGeneralInfo;

        private ObservableCollection<DtoFullField> _lsTableFields;

        private ObservableCollection<DtoFullField> _rsTableFields;

        private string _lsSqript;

        private string _rsSqript;

        private string _leftCompared;

        private string _rightCompared;



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

        public string LsSqript
        {
            get => _lsSqript;
            set => SetProperty(ref _lsSqript, value, "LsSqript");
        }

        public string RsSqript
        {
            get => _rsSqript;
            set => SetProperty(ref _rsSqript, value, "RsSqript");
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

        public GeneralDbInfoViewModel()
        {
            _lsGeneralInfo = new ObservableCollection<GeneralDbInfo>();
            _rsGeneralInfo = new ObservableCollection<GeneralDbInfo>();
            _collectionEqualizer = new CollectionEqualizer();
            _fieldsEqualizer = new FieldsEqualizer();
            _autoComparator = new AutoComparator();
        }



        private RellayCommand _backCommand;

        private RellayCommand _autoCompareCommand;

        private RellayCommand _compareCommand;


        public RellayCommand BackCommand =>
            _backCommand = new RellayCommand((c) => { ClearInfoObjects(DepthOfCleaning.Low); });

        public RellayCommand AutoCompareCommand
        {
            get
            {
                return _autoCompareCommand ??
                    (_autoCompareCommand = new RellayCommand(obj =>
                    {
                        var result = _autoComparator.Compare(_lsRepository, _rsRepository);
                        SendMessage(result);
                    }));
            }
        }

        public RellayCommand CompareCommand =>
            _compareCommand = new RellayCommand(CompareGeneralDbInfo);

        private void ItemSelection(Property property)
        {
            if (property != null && property.Name != "null")
            {
                switch (property.PropertyType)
                {
                    case "Table": { FetchTableFields(property.Name); break; }
                    case "Procedure": { FetchProcedureSqript(property.Name); break; }
                    case "Trigger": { FetchTriggerSqript(property.Name); break; }
                }
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
                        LsGeneralInfo.ClearIfNotEmpty();
                        RsGeneralInfo.ClearIfNotEmpty();
                        goto case DepthOfCleaning.Low;
                    }
                case DepthOfCleaning.Low:
                    {
                        LsTableFields = null;
                        RsTableFields = null;
                        LsSqript = null;
                        RsSqript = null;
                        break;
                    }
            }
        }

        private void CompareGeneralDbInfo(object obj)
        {
            var leftDbTables = _lsRepository.GetTables().ToList();
            var rightDbTables = _rsRepository.GetTables().ToList();
            _collectionEqualizer.CollectionsEquation(leftDbTables, rightDbTables);

            var leftDbProcedures = _lsRepository.GetProcedures().ToList();
            var rightDbProcedures = _rsRepository.GetProcedures().ToList();
            _collectionEqualizer.CollectionsEquation(leftDbProcedures, rightDbProcedures);

            var leftDbTriggers = _lsRepository.GetTriggers().ToList();
            var rightDbTriggers = _rsRepository.GetTriggers().ToList();
            _collectionEqualizer.CollectionsEquation(leftDbTriggers, rightDbTriggers);

            FillColection(_lsGeneralInfo, leftDbTables, leftDbProcedures, leftDbTriggers, _lsReceiver);
            FillColection(_rsGeneralInfo, rightDbTables, rightDbProcedures, rightDbTriggers, _rsReceiver);
        }

        private void FillColection(ObservableCollection<GeneralDbInfo> collection, List<string> tables, List<string> procedures, List<string> triggers, DbInfo db)
        {
            var result = DesignOfCollection(tables, procedures, triggers, db.DataBase.DbName);
            collection.ClearIfNotEmpty();
            collection.Add(result);
        }

        private void FetchTableFields(string name)
        {
            LsTableFields = GetFieldsInfo(_lsRepository, name);
            RsTableFields = GetFieldsInfo(_rsRepository, name);

            _fieldsEqualizer.CollectionsEquation(LsTableFields, RsTableFields);
        }

        private ObservableCollection<DtoFullField> GetFieldsInfo(IRepository repository, string tableName) =>
            tableName != "null" ? repository.GetFieldsInfo(tableName).ToObservableCollection() : null;

        private void FetchProcedureSqript(string name)
        {
            GetSqript(_lsRepository.GetProcedureSqript, _rsRepository.GetProcedureSqript, name);
            LsSqript = LeftCompared;
            RsSqript = RightCompared;
        }

        private void FetchTriggerSqript(string name)
        {
            GetSqript(_lsRepository.GetTriggerSqript, _rsRepository.GetTriggerSqript, name);
            LsSqript = LeftCompared;
            RsSqript = RightCompared;
        }

        private void GetSqript(Func<string, string> left, Func<string, string> right, string name)
        {
            LeftCompared = left(name) ?? " ";
            RightCompared = right(name) ?? " ";
        }

        private void DataContextChanged(DbInfo db, ref IRepository repository)
        {
            if (repository == null || db.DataBase.DbType != repository.DbType)
            {
                repository = RepositoryFactory.GetRepository(db.DataBase.DbType);
            }
            ClearInfoObjects(DepthOfCleaning.Medium);
            repository.CreateConnectionString(db.DataBase.DataSource, 
                db.DataBase.ServerName, db.DataBase.DbName, db.DataBase.Login, db.DataBase.Password);
        }

        private GeneralDbInfo DesignOfCollection(List<string> tables, List<string> procedures, List<string> triggers, string dbName)
        {
            var tempTables = GetProperties(tables, "Table");
            var tempProcedures = GetProperties(procedures, "Procedure");
            var tempTriggers = GetProperties(triggers, "Trigger");

            GeneralDbInfo dataBase = new GeneralDbInfo()
            {
                Name = dbName,
                Entitys = new List<Entity>()
                {
                    new Entity() { Name = "Tables", Properties = tempTables },
                    new Entity() { Name = "Procedures", Properties = tempProcedures },
                    new Entity() { Name = "Triggers", Properties = tempTriggers }
                }
            };

            return dataBase;
        }

        private List<Property> GetProperties(List<string> collection, string propertyName) =>
            (from c in collection select new Property { Name = c, PropertyType = propertyName }).ToList();


        private void SendMessage(object package)
        {
            if (MessageHandler != null)
            {
                MessageEventArgs eventArgs = new MessageEventArgs();
                eventArgs.Message = package;
                MessageHandler(this, eventArgs);
            }
        }
    }
}
