using Comparator.Repositories.Models.DbModels;
using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;

namespace Comparator.Repositories.Repositories
{
    public interface IRepository
    {
        void CreateConnectionString(string source, string server, string dbName, string login, string password);

        IEnumerable<FullField> GetFieldsInfo(string tableName);

        IEnumerable<string> GetProcedures();

        string GetProcedureSqript(string procedureName);

        IEnumerable<string> GetTables();
               
        bool IsConnection();
    }
}
 