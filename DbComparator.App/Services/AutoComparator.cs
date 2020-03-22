using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Extensions;
using DbComparator.App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DbComparator.App.Services
{
    public class AutoComparator : IAutoComparator
    { 
        private ICollectionEqualizer _collectionEqualizer;

        private IFieldsEqualizer _fieldsEqualizer;

        private IRepository _primaryRepository;

        private IRepository _secondaryRepository;

        List<CompareResult> _results;


        public AutoComparator(ICollectionEqualizer collectionEqualizer, IFieldsEqualizer fieldsEqualizer)
        {
            _collectionEqualizer = collectionEqualizer;
            _fieldsEqualizer = fieldsEqualizer;
            _results = new List<CompareResult>();
        }

        public List<CompareResult> Compare(IRepository primaryRep, IRepository secondarRep)
        {
            _results.Clear();
            _primaryRepository = primaryRep;
            _secondaryRepository = secondarRep;

            TablesCompare();
            ProceduresCompare();
            TriggersCompare();
            return _results;
        }

        private void TablesCompare()
        {
            var prDbTables = GetTables(_primaryRepository);
            var seDbTables = GetTables(_secondaryRepository);
            _collectionEqualizer.CollectionsEquation(prDbTables, seDbTables);

            _results.Add(new CompareResult { Entity = "Tables", NotCoincide = DifferencesCounting(prDbTables, seDbTables) });
            _results.Add(new CompareResult { Entity = "Fields", NotCoincide = FieldsCompare(prDbTables, seDbTables) });
        }

        private List<string> GetTables(IRepository repository) =>
            repository.GetTables().ToList();

        private int FieldsCompare(List<string> primaryDbTables, List<string> secondaryDbTables)
        {
            int count = 0;

            for (int i = 0; i < primaryDbTables.Count; i++)
            {
                var firstTableFields = GetTableFields(_primaryRepository, primaryDbTables[i]);
                var secondTableFields = GetTableFields(_secondaryRepository, secondaryDbTables[i]);

                if (primaryDbTables[i] == "null")
                {
                    count += secondTableFields.Count;
                    continue;
                }
                else if (secondaryDbTables[i] == "null")
                {
                    count += firstTableFields.Count;
                    continue;
                }
                               
                _fieldsEqualizer.CollectionsEquation(firstTableFields, secondTableFields);

                

                for (int f = 0; f < firstTableFields.Count; f++)
                {
                    if (!FieldEquals(firstTableFields[f], secondTableFields[f]))
                    {
                        ++count;
                    }
                }
            }
            return count;
        }

        private ObservableCollection<DtoFullField> GetTableFields(IRepository repository, string tableName) =>
            repository.GetFieldsInfo(tableName).ToObservableCollection();

        private bool FieldEquals(DtoFullField left, DtoFullField right)
        {
            if (left.ConstraintKeys == right.ConstraintKeys &&
                left.ConstraintName == right.ConstraintName &&
                left.ConstraintType == right.ConstraintType &&
                left.FieldName      == right.FieldName      &&
                left.IsNullable     == right.IsNullable     &&
                left.OnDelete       == right.OnDelete       &&
                left.OnUpdate       == right.OnUpdate       &&
                left.References     == right.References     &&
                left.Size           == right.Size           &&
                left.TypeName       == right.TypeName)
            {
                return true;
            }
            return false;
        }

        private void ProceduresCompare()
        {
            var prProcedures = _primaryRepository.GetProcedures().ToList();
            var seProcedures = _secondaryRepository.GetProcedures().ToList();
            ProceduresSqriptCompare(prProcedures, seProcedures);
        }

        private void ProceduresSqriptCompare(List<string> prP, List<string> seP)
        {
            _collectionEqualizer.CollectionsEquation(prP, seP);
            _results.Add(new CompareResult { Entity = "Procedures", NotCoincide = DifferencesCounting(prP, seP) });
            _results.Add(new CompareResult { Entity = "Procedures sqripts", NotCoincide = ProcedureScriptCompare(prP, seP) });
        }

        private int ProcedureScriptCompare(List<string> prProcedures, List<string> seProcedures)
        {
            var leftDbScripts = GetScript(prProcedures, _primaryRepository.GetProcedureSqript);
            var rightDbScripts = GetScript(seProcedures, _secondaryRepository.GetProcedureSqript);
            return DifferencesCounting(leftDbScripts, rightDbScripts);
        }

        private void TriggersCompare()
        {
            var prTriggers = _primaryRepository.GetTriggers().ToList();
            var seTriggers = _secondaryRepository.GetTriggers().ToList();
            TriggersSqriptCompare(prTriggers, seTriggers);
        }

        private void TriggersSqriptCompare(List<string> prT, List<string> seT)
        {
            _collectionEqualizer.CollectionsEquation(prT, seT);
            _results.Add(new CompareResult { Entity = "Triggers", NotCoincide = DifferencesCounting(prT, seT) });
            _results.Add(new CompareResult { Entity = "Triggers sqripts", NotCoincide = TriggersScriptCompare(prT, seT) });
        }

        private int TriggersScriptCompare(List<string> prProcedures, List<string> seProcedures)
        {
            var leftDbScripts = GetScript(prProcedures, _primaryRepository.GetTriggerSqript);
            var rightDbScripts = GetScript(seProcedures, _secondaryRepository.GetTriggerSqript);
            return DifferencesCounting(leftDbScripts, rightDbScripts);
        }

        private List<string> GetScript(List<string> procedures, Func<string, string> func)
        {
            List<string> scripts = new List<string>();

            foreach (var procedure in procedures)
            {
                if (procedure == "null")
                {
                    scripts.Add("null");
                }
                else if (procedure != null)
                {
                    var sqript = func(procedure);

                    if (sqript != null)
                    {
                        scripts.Add(sqript);
                    }
                }
            }
            return scripts;
        }

        private int DifferencesCounting(List<string> primaryCol, List<string> secondaryCol)
        {
            int count = 0;
            foreach (var name in primaryCol)
            {
                if (name == "null" || !secondaryCol.Contains(name))
                {
                    ++count;
                }
            }
            return count;
        }
    }
}
