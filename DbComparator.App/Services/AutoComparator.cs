using Comparator.Repositories.Models.DtoModels;
using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Extensions;
using DbComparator.App.Models;
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
            var prProcedures = GetProcedures(_primaryRepository);
            var seProcedures = GetProcedures(_secondaryRepository);
            SqriptsCompare(prProcedures, seProcedures, "Procedures", "Procedures sqripts");
        }

        private void TriggersCompare()
        {
            var prTriggers = GetTriggers(_primaryRepository);
            var seTriggers = GetTriggers(_secondaryRepository);
            SqriptsCompare(prTriggers, seTriggers, "Triggers", "Triggers sqripts");
        }

        private List<string> GetProcedures(IRepository repository) =>
            repository.GetProcedures().ToList();

        private List<string> GetTriggers(IRepository repository) => 
            repository.GetTriggers().ToList();

        private void SqriptsCompare(List<string> prP, List<string> seP, string firstName, string secondName)
        {
            _collectionEqualizer.CollectionsEquation(prP, seP);
            _results.Add(new CompareResult { Entity = firstName, NotCoincide = DifferencesCounting(prP, seP) });
            _results.Add(new CompareResult { Entity = secondName, NotCoincide = ScriptsCompare(prP, seP) });
        }

        private int ScriptsCompare(List<string> prProcedures, List<string> seProcedures)
        {
            var leftDbScripts = GetProceduresScript(_primaryRepository, prProcedures);
            var rightDbScripts = GetProceduresScript(_secondaryRepository, seProcedures);
            return DifferencesCounting(leftDbScripts, rightDbScripts);
        }

        private List<string> GetProceduresScript(IRepository repository, List<string> procedures)
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
                    var sqript = repository.GetSqript(procedure);

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
