using Comparator.Repositories.Models.DbModels;
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
    public class ShowDbInfoViewModel : ModelBase
    {
        public event EventHandler MessageHandler;

        private CollectionEqualizer _collectionEqualizer;

        private FieldsEqualizer _fieldsEqualizer;

        private AutoComparator _autoComparator;



        private IRepository _primaryDbRepository;

        private IRepository _secondaryDbRepository;

        private DbInfo _leftDbInfoReceiver;

        private DbInfo _rightDbInfoReceiver;

        private ObservableCollection<GeneralDbInfo> _generalInfoDbLeft;

        private ObservableCollection<GeneralDbInfo> _generalInfoDbRight;

        private ObservableCollection<FullField> _leftDbFieldsInfo;

        private ObservableCollection<FullField> _rightDbFieldsInfo;

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

        public ObservableCollection<FullField> LeftDbFieldsInfo
        {
            get => _leftDbFieldsInfo;
            set => SetProperty(ref _leftDbFieldsInfo, value, "LeftDbFieldsInfo");
        }

        public ObservableCollection<FullField> RightDbFieldsInfo
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

        public ShowDbInfoViewModel()
        {
            _generalInfoDbLeft = new ObservableCollection<GeneralDbInfo>();
            _generalInfoDbRight = new ObservableCollection<GeneralDbInfo>();
            _collectionEqualizer = new CollectionEqualizer();
            _fieldsEqualizer = new FieldsEqualizer();
            _autoComparator = new AutoComparator();
        }




        private RellayCommand _backCommand;

        private RellayCommand _itemSelectCommand;

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

        public RellayCommand ItemSelectCommand
        {
            get
            {
                return _itemSelectCommand ??
                    (_itemSelectCommand = new RellayCommand(obj =>
                    {
                        if (CheckingDbSelection()) // убрать когда разберусь с очисткой окна
                        {
                            if (obj is Property property)
                            {
                                if (property.PropertyType == "Table")
                                {
                                    FetchTableFields(property.Name);
                                }
                                else if (property.PropertyType == "Procedure")
                                {
                                    FetchProcedure(property.Name);
                                }
                            }
                        }
                    }));
            }
        }

        public RellayCommand AutoCompareCommand
        {
            get
            {
                return _autoCompareCommand ??
                    (_autoCompareCommand = new RellayCommand(async obj =>
                    {
                        if (CheckingDbSelection())
                        {
                            var result = await _autoComparator.CompareAsync(_primaryDbRepository, _secondaryDbRepository);
                            SendMessage(result);
                        }
                    }));
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

        public RellayCommand CompareCommand =>
            _compareCommand = new RellayCommand(CompareGeneralDbInfo);

        private void CompareGeneralDbInfo(object obj)
        {
            if (CheckingDbSelection())
            {
                var leftDbTables = _primaryDbRepository.GetTables().ToList();
                var rightDbTables = _secondaryDbRepository.GetTables().ToList();
                _collectionEqualizer.CollectionsEquation(leftDbTables, rightDbTables);

                var leftDbProcedures = _primaryDbRepository.GetProcedures().ToList();
                var rightDbProcedures = _secondaryDbRepository.GetProcedures().ToList();
                _collectionEqualizer.CollectionsEquation(leftDbProcedures, rightDbProcedures);

                FillColection(_generalInfoDbLeft, leftDbTables, leftDbProcedures, _leftDbInfoReceiver);
                FillColection(_generalInfoDbRight, rightDbTables, rightDbProcedures, _rightDbInfoReceiver);
            }
        }

        private void FillColection(ObservableCollection<GeneralDbInfo> collection, List<string> tables, List<string> procedures, DbInfo db)
        {
            var result = DesignOfCollection(tables, procedures, db.DataBase.DbName);
            collection.ClearIfNotEmpty();
            collection.Add(result);
        }

        private void FetchTableFields(string name)
        {
            LeftDbFieldsInfo = GetFieldsInfo(_primaryDbRepository, name);
            RightDbFieldsInfo = GetFieldsInfo(_secondaryDbRepository, name);

            _fieldsEqualizer.CollectionsEquation(LeftDbFieldsInfo, RightDbFieldsInfo);
        }

        private ObservableCollection<FullField> GetFieldsInfo(IRepository repository, string tableName) =>
            tableName != "null" ? repository.GetFieldsInfo(tableName).ToObservableCollection() : null;

        private void FetchProcedure(string name)
        {
            LeftCompared = GetProcedureSquript(_primaryDbRepository, name);
            RightCompared = GetProcedureSquript(_secondaryDbRepository, name);

            LeftDbProcedureSqript = LeftCompared;
            RightDbProcedureSqript = RightCompared;
        }

        private string GetProcedureSquript(IRepository repository, string procedureName) =>
            procedureName != "null" ? repository.GetProcedureSqript(procedureName) : null;

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

        public GeneralDbInfo DesignOfCollection(List<string> tables, List<string> procedures, string dbName)
        {
            var tempTables = (from t in tables
                              select new Property()
                              {
                                  Name = t,
                                  PropertyType = "Table"
                              }).ToList();

            var tempProcedures = (from p in procedures
                                  select new Property()
                                  {
                                      Name = p,
                                      PropertyType = "Procedure"
                                  }).ToList();

            GeneralDbInfo dataBase = new GeneralDbInfo()
            {
                Name = dbName,
                Entitys = new List<Entity>()
                {
                    new Entity() { Name = "Tables", Properties = tempTables },
                    new Entity() { Name = "Procedures", Properties = tempProcedures }
                }
            };

            return dataBase;
        }

        private bool CheckingDbSelection()
        {
            if (_leftDbInfoReceiver != null && _rightDbInfoReceiver != null)
            {
                if (_leftDbInfoReceiver.IsConnect && _rightDbInfoReceiver.IsConnect)
                {
                    return true;
                }
            }
            return false;
        }

        private void SendMessage(string message)
        {
            if (MessageHandler != null)
            {
                MessageEventArgs eventArgs = new MessageEventArgs();
                eventArgs.Message = message;
                MessageHandler(this, eventArgs);
            }
        }
    }
}
