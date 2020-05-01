using Comparator.Repositories.Models.DtoModels;
using System.Collections.Generic;
using System.Text;

namespace Comparator.Repositories.Parsers
{
    internal interface ITableCreator
    {
        string GetTableScript(List<DtoFullField> info, string tableName, StringBuilder foreignKeys);
    }
}
