using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Commands;
using DbComparator.App.Infrastructure.Extensions;
using DbComparator.App.Models;
using DbComparator.App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DbComparator.App.ViewModels
{
    public class GeneralDbInfoViewModel : ModelBase, IGeneralDbInfoVM
    {
        public Action<Property> GetAction => ((x) => ItemSelection(x));
        
        public event EventHandler MessageHandler;



        private ICollectionEqualizer _collectionEqualizer;

        private IFieldsEqualizer _fieldsEqualizer;

        private IAutoComparator _autoComparator;

        private IRepository _primaryDbRepository;

        private IRepository _secondaryDbRepository;

        private DbInfo _leftDbInfoReceiver;

        private DbInfo _rightDbInfoReceiver;

        private ObservableCollection<GeneralDbInfo> _generalInfoDbLeft;

        private ObservableCollection<GeneralDbInfo> _generalInfoDbRight;

        private ObservableCollection<DtoFullField> _leftDbFieldsInfo;

        private ObservableCollection<DtoFullField> _rightDbFieldsInfo;

        private string _leftDbProcedureSqript;

        private string _rightDbProcedureSqript;

        private string _leftCompared;

        private string _rightCompared;



        public DbInfo LeftDbInfoReceiver
        {
            get => _leftDbInfoReceiver;
            set
            {
                if (value != null)
                {
                    DataContextChanged(value, ref _primaryDbRepository);
                }
                SetProperty(ref _leftDbInfoReceiver, value, "LeftDbInfoReceiver");
            }
        }

        public DbInfo RightDbInfoReceiver
        {
            get => _rightDbInfoReceiver;
            set
            {
                if (value != null)
                {
                    DataContextChanged(value, ref _secondaryDbRepository);
                }
                SetProperty(ref _rightDbInfoReceiver, value, "RightDbInfoReceiver");
            }
        }


        public ObservableCollection<GeneralDbInfo> GeneralInfoDbLeft
        {
            get => _generalInfoDbLeft;
            set => SetProperty(ref _generalInfoDbLeft, value, "GeneralInfoDbLeft");
        }

        public ObservableCollection<GeneralDbInfo> GeneralInfoDbRight
        {
            get => _generalInfoDbRight;
            set => SetProperty(ref _generalInfoDbRight, value, "GeneralInfoDbRight");
        }

        public ObservableCollection<DtoFullField> LeftDbFieldsInfo
        {
            get => _leftDbFieldsInfo;
            set => SetProperty(ref _leftDbFieldsInfo, value, "LeftDbFieldsInfo");
        }

        public ObservableCollection<DtoFullField> RightDbFieldsInfo
        {
            get => _rightDbFieldsInfo;
            set => SetProperty(ref _rightDbFieldsInfo, value, "RightDbFieldsInfo");
        }

        public string LeftDbProcedureSqript
        {
            get => _leftDbProcedureSqript;
            set => SetProperty(ref _leftDbProcedureSqript, value, "LeftDbProcedureSqript");
        }

        public string RightDbProcedureSqript
        {
            get => _rightDbProcedureSqript;
            set => SetProperty(ref _rightDbProcedureSqript, value, "RightDbProcedureSqript");
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

        public GeneralDbInfoViewModel(ICollectionEqualizer collectionEqualizer, IFieldsEqualizer fieldsEqualizer, IAutoComparator autoComparator)
        {
            _generalInfoDbLeft = new ObservableCollection<GeneralDbInfo>();
            _generalInfoDbRight = new ObservableCollection<GeneralDbInfo>();
            _collectionEqualizer = collectionEqualizer;
            _fieldsEqualizer = fieldsEqualizer;
            _autoComparator = autoComparator;
        }



        private RellayCommand _backCommand;

        private RellayCommand _autoCompareCommand;

        private RellayCommand _compareCommand;



        public RellayCommand BackCommand
        {
            get
            {
                return _backCommand ??
                    (_backCommand = new RellayCommand(obj =>
                    {
                        LeftDbFieldsInfo = null;
                        RightDbFieldsInfo = null;
                        LeftDbProcedureSqript = null;
                        RightDbProcedureSqript = null;

                    }));
            }
        }

        public RellayCommand AutoCompareCommand
        {
            get
            {
                return _autoCompareCommand ??
                    (_autoCompareCommand = new RellayCommand(obj =>
                    {
                        var result = _autoComparator.Compare(_primaryDbRepository, _secondaryDbRepository);
                        SendMessage(result);
                    }));
            }
        }

        public RellayCommand CompareCommand =>
            _compareCommand = new RellayCommand(CompareGeneralDbInfo);

        private void ItemSelection(Property property)
        {
            if (property != null)
            {
                switch (property.PropertyType)
                {
                    case "Table": { FetchTableFields(property.Name); break; }
                    case "Procedure": { FetchSqript(property.Name); break; }
                    case "Trigger": { FetchSqript(property.Name); break; }
                }
            }
        }

        public void ClearAll()
        {
            LeftDbFieldsInfo = null;
            RightDbFieldsInfo = null;
            LeftDbInfoReceiver = null;
            RightDbInfoReceiver = null;
            LeftDbProcedureSqript = null;
            RightDbProcedureSqript = null;
            GeneralInfoDbLeft.ClearIfNotEmpty();
            GeneralInfoDbRight.ClearIfNotEmpty();
        }

        private void CompareGeneralDbInfo(object obj)
        {
            var leftDbTables = _primaryDbRepository.GetTables().ToList();
            var rightDbTables = _secondaryDbRepository.GetTables().ToList();
            _collectionEqualizer.CollectionsEquation(leftDbTables, rightDbTables);

            var leftDbProcedures = _primaryDbRepository.GetProcedures().ToList();
            var rightDbProcedures = _secondaryDbRepository.GetProcedures().ToList();
            _collectionEqualizer.CollectionsEquation(leftDbProcedures, rightDbProcedures);

            var leftDbTriggers = _primaryDbRepository.GetTriggers().ToList();
            var rightDbTriggers = _secondaryDbRepository.GetTriggers().ToList();
            _collectionEqualizer.CollectionsEquation(leftDbTriggers, rightDbTriggers);

            FillColection(_generalInfoDbLeft, leftDbTables, leftDbProcedures, leftDbTriggers, _leftDbInfoReceiver);
            FillColection(_generalInfoDbRight, rightDbTables, rightDbProcedures, rightDbTriggers, _rightDbInfoReceiver);
        }

        private void FillColection(ObservableCollection<GeneralDbInfo> collection, List<string> tables, List<string> procedures, List<string> triggers, DbInfo db)
        {
            var result = DesignOfCollection(tables, procedures, triggers, db.DataBase.DbName);
            collection.ClearIfNotEmpty();
            collection.Add(result);
        }

        private void FetchTableFields(string name)
        {
            LeftDbFieldsInfo = GetFieldsInfo(_primaryDbRepository, name);
            RightDbFieldsInfo = GetFieldsInfo(_secondaryDbRepository, name);

            _fieldsEqualizer.CollectionsEquation(LeftDbFieldsInfo, RightDbFieldsInfo);
        }

        private ObservableCollection<DtoFullField> GetFieldsInfo(IRepository repository, string tableName) =>
            tableName != "null" ? repository.GetFieldsInfo(tableName).ToObservableCollection() : null;

        private void FetchSqript(string name)
        {
            LeftCompared = GetSqript(_primaryDbRepository, name);
            RightCompared = GetSqript(_secondaryDbRepository, name);

            LeftDbProcedureSqript = LeftCompared;
            RightDbProcedureSqript = RightCompared;
        }

        private string GetSqript(IRepository repository, string procedureName) =>
            procedureName != "null" ? repository.GetSqript(procedureName) : null;

        private void DataContextChanged(DbInfo db, ref IRepository repository)
        {
            repository = CreateRepository(db.DataBase.DbType);
            repository.CreateConnectionString(db.DataBase.DataSource, db.DataBase.ServerName,
                                              db.DataBase.DbName, db.DataBase.Login,
                                              db.DataBase.Password);
        }

        private IRepository CreateRepository(string SelectedDbType)
        {
            switch (SelectedDbType)
            {
                case "Microsoft Sql": { return new MicrosoftDb(); }
                case "SyBase": { return new SyBaseDb(); }
            }
            return null;
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
