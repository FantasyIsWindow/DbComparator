using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Extensions;
using DbComparator.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DbComparator.App.Services
{
    public class GeneralComparator
    {
        private CollectionEqualizer _collectionEqualizer;

        private IRepository _lsRepository;

        private IRepository _rsRepository;

        private const string MAIN_COLOR = "Black";

        private const string IS_NULL_COLOR = "LightGray";

        private const string COLOR_OF_EQUALITY = "DarkGreen";

        private const string COLOR_OF_INEQUALITY = "Red";

        private List<CompareResult> _compareResults;


        public GeneralComparator()
        {
            _collectionEqualizer = new CollectionEqualizer();
        }

        /// <summary>
        /// Comparison of collections of main information by changing color values
        /// </summary>
        /// <param name="lsRepository">Left side current repositury</param>
        /// <param name="rsRepository">Right side current repositury</param>
        /// <param name="lsGeneralInfo">Left side collecton</param>
        /// <param name="rsGeneralInfo">Right side collecton</param>
        public void CompareGeneralInfo(IRepository lsRepository, 
                                       IRepository rsRepository, 
                                       List<GeneralDbInfo> lsGeneralInfo, 
                                       List<GeneralDbInfo> rsGeneralInfo)
        {
            _compareResults = new List<CompareResult>();
            _lsRepository = lsRepository;
            _rsRepository = rsRepository;
            Compare(lsGeneralInfo, rsGeneralInfo);
        }

        /// <summary>
        /// Collection compare
        /// </summary>
        /// <param name="lsGenInfo">Left side collecton</param>
        /// <param name="rsGenInfo">Right side collecton</param>
        private void Compare(List<GeneralDbInfo> lsGenInfo, List<GeneralDbInfo> rsGenInfo)
        {
            var lsTables = _lsRepository.GetTables().ToList();
            var rsTables = _rsRepository.GetTables().ToList();
            _collectionEqualizer.GeneralEqualizer(lsTables, rsTables);

            var lsProced = _lsRepository.GetProcedures().ToList();
            var rsProced = _rsRepository.GetProcedures().ToList();
            _collectionEqualizer.GeneralEqualizer(lsProced, rsProced);

            var lsTrig = _lsRepository.GetTriggers().ToList();
            var rsTrig = _rsRepository.GetTriggers().ToList();
            _collectionEqualizer.GeneralEqualizer(lsTrig, rsTrig);

            var left = DesignOfCollection(lsTables, lsProced, lsTrig, _lsRepository.DbName);
            var right = DesignOfCollection(rsTables, rsProced, rsTrig, _rsRepository.DbName);

            lsGenInfo.Add(left);
            rsGenInfo.Add(right);
        }

        /// <summary>
        /// Building a collection
        /// </summary>
        /// <param name="tables">Tables collection</param>
        /// <param name="procedures">Procedures collection</param>
        /// <param name="triggers">Triggers collection</param>
        /// <param name="dbName">Data base name</param>
        /// <returns>Redesigned collection</returns>
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
                    new Entity() { Name = "Tables", Properties = tempTables},
                    new Entity() { Name = "Procedures", Properties = tempProcedures },
                    new Entity() { Name = "Triggers", Properties = tempTriggers }
                }
            };

            return dataBase;
        }

        /// <summary>
        /// Returns a new rebuilt collection
        /// </summary>
        /// <param name="collection">Iterate through the collection</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>A new rebuilt collection</returns>
        private List<Property> GetProperties(List<string> collection, string propertyName) =>
            (from name in collection
             select new Property
             {
                 Name = name,
                 PropertyType = propertyName,
                 Color = name != "null" ? MAIN_COLOR : IS_NULL_COLOR
             }).ToList();

        /// <summary>
        /// A comparison of the collections and change the color value fields
        /// </summary>
        /// <param name="lsGenInfo">Left side collecton</param>
        /// <param name="rsGenInfo">Right side collecton</param>
        /// <returns>Collection with changed field color</returns>
        public List<CompareResult> Colorize(GeneralDbInfo lsGenInfo, GeneralDbInfo rsGenInfo)
        {
            for (int i = 0; i < lsGenInfo.Entitys.Count; i++)
            {
                if (lsGenInfo.Entitys[i].Name == "Tables")
                {
                    FieldsCompare("Tables",
                                  "Fields",
                                  lsGenInfo.Entitys[i].Properties,
                                  rsGenInfo.Entitys[i].Properties);
                }
                else if (lsGenInfo.Entitys[i].Name == "Procedures")
                {
                    ScriptCompare("Procedures",
                                  "Procedures sqripts",
                                  lsGenInfo.Entitys[i].Properties,
                                  rsGenInfo.Entitys[i].Properties,
                                  _lsRepository.GetProcedureSqript,
                                  _rsRepository.GetProcedureSqript);
                }
                else if (lsGenInfo.Entitys[i].Name == "Triggers")
                {
                    ScriptCompare("Triggers",
                                  "Triggers sqripts",
                                  lsGenInfo.Entitys[i].Properties,
                                  rsGenInfo.Entitys[i].Properties,
                                  _lsRepository.GetTriggerSqript,
                                  _rsRepository.GetTriggerSqript);
                }
            }

            return _compareResults;
        }

        /// <summary>
        /// Comparison of fields
        /// </summary>
        /// <param name="name">Name of the first entity</param>
        /// <param name="scrName">Name of the second entity</param>
        /// <param name="lsProprties">Left properties collection</param>
        /// <param name="rsProprties">Right properties collection</param>
        private void FieldsCompare(string name, string scrName, List<Property> lsProprties, List<Property> rsProprties)
        {
            int entityCount = 0;
            int fieldsCount = 0;
            bool flag;
            for (int f = 0; f < lsProprties.Count; f++)
            {
                flag = true;
                if (lsProprties[f].Name == "null" || rsProprties[f].Name == "null")
                {
                    ++entityCount;
                    continue;
                }

                var lsScr = _lsRepository.GetFieldsInfo(lsProprties[f].Name).ToObservableCollection();
                var rsScr = _rsRepository.GetFieldsInfo(rsProprties[f].Name).ToObservableCollection();
                _collectionEqualizer.FieldsAlignment(lsScr, rsScr);

                for (int i = 0; i < lsScr.Count; i++)
                {
                    if (!FieldsEquals(lsScr[i], rsScr[i]))
                    {
                        ++fieldsCount;
                        flag = false;
                    }
                }
                ColorationPairedEntity(lsProprties, rsProprties, flag, f);
            }

            _compareResults.Add(new CompareResult { Entity = name, NotCoincide = entityCount });
            _compareResults.Add(new CompareResult { Entity = scrName, NotCoincide = fieldsCount });
        }

        /// <summary>
        /// Comparison of script
        /// </summary>
        /// <param name="name">Name of the first entity</param>
        /// <param name="scrName">Name of the second entity</param>
        /// <param name="lsScripts">Left side script</param>
        /// <param name="rsScripts">Right side script</param>
        /// <param name="lsFunc">Left-hand script selection function</param>
        /// <param name="rsFunc">Right-hand script selection function</param>
        private void ScriptCompare(string name, string scrName, List<Property> lsScripts, List<Property> rsScripts, Func<string, string> lsFunc, Func<string, string> rsFunc)
        {
            int entityCount = 0;
            int scriptCount = 0;

            for (int f = 0; f < lsScripts.Count; f++)
            {
                if (lsScripts[f].Name == "null" || rsScripts[f].Name == "null")
                {
                    ++entityCount;
                    continue;
                }

                string lsScr = lsFunc(lsScripts[f].Name);
                string rsScr = rsFunc(rsScripts[f].Name);

                bool flag = lsScr == rsScr;
                if (!flag)
                {
                    ++scriptCount;
                }
                ColorationPairedEntity(lsScripts, rsScripts, flag, f);
            }

            _compareResults.Add(new CompareResult { Entity = name, NotCoincide = entityCount });
            _compareResults.Add(new CompareResult { Entity = scrName, NotCoincide = scriptCount });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lsEntity">Left side collection</param>
        /// <param name="rsEntity">Right side collection</param>
        /// <param name="condition">Condition</param>
        /// <param name="index">Index</param>
        private void ColorationPairedEntity(List<Property> lsEntity, List<Property> rsEntity, bool condition, int index)
        {
            if (condition)
            {
                lsEntity[index].Color = COLOR_OF_EQUALITY;
                rsEntity[index].Color = COLOR_OF_EQUALITY;
            }
            else
            {
                lsEntity[index].Color = COLOR_OF_INEQUALITY;
                rsEntity[index].Color = COLOR_OF_INEQUALITY;
            }
        }

        /// <summary>
        /// Comparing fields with the return of the comparison result
        /// </summary>
        /// <param name="lsField">Left side field</param>
        /// <param name="rsField">Right side field</param>
        /// <returns>Comparison result</returns>
        private bool FieldsEquals(DtoFullField lsField, DtoFullField rsField)
        {
            return lsField.FieldName      != rsField.FieldName      ? false :
                   lsField.TypeName       != rsField.TypeName       ? false :
                   lsField.Size           != rsField.Size           ? false :
                   lsField.IsNullable     != rsField.IsNullable     ? false :
                   lsField.ConstraintType != rsField.ConstraintType ? false :
                   lsField.ConstraintName != rsField.ConstraintName ? false :
                   lsField.ConstraintKeys != rsField.ConstraintKeys ? false :
                   lsField.Referenced     != rsField.Referenced     ? false :
                   lsField.OnUpdate       != rsField.OnUpdate       ? false :
                   lsField.OnDelete       != rsField.OnDelete       ? false : true;
        }
    }
}
