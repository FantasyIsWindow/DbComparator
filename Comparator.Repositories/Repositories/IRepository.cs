﻿using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Comparator.Repositories.Repositories
{
    public interface IRepository
    {
        void CreateConnectionString(string source, string server, string dbName, string login, string password);

        IEnumerable<DtoFullField> GetFieldsInfo(string tableName);

        IEnumerable<string> GetProcedures();

        string GetSqript(string procedureName);

        IEnumerable<string> GetTables();

        IEnumerable<string> GetTriggers();

        Task<bool> IsConnectionAsync();
    }
}
 