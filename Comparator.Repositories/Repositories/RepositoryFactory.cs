using System;

namespace Comparator.Repositories.Repositories
{
    public enum Provider { MicrosoftSql, SyBase, MySql }

    static public class RepositoryFactory
    {
        public static IRepository GetRepository(string repository)
        {
            var temp = Enum.Parse(typeof(Provider), repository);
            return CreateRepository((Provider)temp); ;
        }

        public static IRepository GetRepository(Provider repository) =>
            CreateRepository(repository);

        public static Provider StringToProvider(string repository) => 
            (Provider)Enum.Parse(typeof(Provider), repository);

        private static IRepository CreateRepository(Provider repository)
        {
            switch (repository)
            {
                case Provider.MicrosoftSql: return new MicrosoftDb();
                case Provider.MySql: return new MySqlDb();
                case Provider.SyBase: return new SyBaseDb();
            }
            return null;
        }        
    }
}
