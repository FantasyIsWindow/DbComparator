using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Repositories;
using DbComparator.App.Infrastructure.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbComparator.App.Services
{
    public class AutoComparator
    {
        private CollectionEqualizer _collectionEqualizer;
        private FieldsEqualizer _fieldsEqualizer;
        private IRepository _primaryRepository;
        private IRepository _secondaryRepository;

        public AutoComparator()
        {
            _collectionEqualizer = new CollectionEqualizer();
            _fieldsEqualizer = new FieldsEqualizer();
        }

        public async Task<string> CompareAsync(IRepository primaryRep, IRepository secondarRep)
        {
            _primaryRepository = primaryRep;
            _secondaryRepository = secondarRep;

            var tablesMismatches = TablesCompare();
            var proceduresMismatches = ProceduresCompare();
            return await Task.Run(() => CreateResultString(tablesMismatches, proceduresMismatches));
        }

        private Dictionary<string, int> TablesCompare()
        {
            Dictionary<string, int> comparesResults = new Dictionary<string, int>();

            var primaryDbTables = GetTables(_primaryRepository);
            var secondaryDbTables = GetTables(_secondaryRepository);
            _collectionEqualizer.CollectionsEquation(primaryDbTables, secondaryDbTables);


            comparesResults.Add("Tables", DifferencesCounting(primaryDbTables, secondaryDbTables));
            comparesResults.Add("Fields", FieldsCompare(primaryDbTables, secondaryDbTables));

            return comparesResults;
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

        private ObservableCollection<FullField> GetTableFields(IRepository repository, string tableName) =>
            repository.GetFieldsInfo(tableName).ToObservableCollection();

        private bool FieldEquals(FullField left, FullField right)
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

        private Dictionary<string, int> ProceduresCompare()
        {
            Dictionary<string, int> comparesResults = new Dictionary<string, int>();

            var prProcedures = GetProcedures(_primaryRepository);
            var seProcedures = GetProcedures(_secondaryRepository);
            _collectionEqualizer.CollectionsEquation(prProcedures, seProcedures);

            comparesResults.Add("Procedures", DifferencesCounting(prProcedures, seProcedures));
            comparesResults.Add("Sqripts", ScriptsCompare(prProcedures, seProcedures));
            return comparesResults;
        }

        private List<string> GetProcedures(IRepository repository) =>
            repository.GetProcedures().ToList();

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
                    var sqript = repository.GetProcedureSqript(procedure);

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

        private string CreateResultString(Dictionary<string, int> tablesMismatches, Dictionary<string, int> proceduresMismatches)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"Tables:\r\n\tnot correspond - {tablesMismatches["Tables"]}.");
            builder.Append($"\r\nTables fields:\r\n\tnot correspond - {tablesMismatches["Fields"]} rows.");
            builder.Append($"\r\nProcedures:\r\n\tnot correspond - {proceduresMismatches["Procedures"]}.");
            builder.Append($"\r\nProcedures sqripts:\r\n\tnot correspond - {proceduresMismatches["Sqripts"]}");

            return builder.ToString();
        }
    }
}
