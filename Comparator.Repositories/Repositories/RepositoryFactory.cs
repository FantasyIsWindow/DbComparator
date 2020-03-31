using System;

namespace Comparator.Repositories.Repositories
{
    public enum Provider { MicrosoftSql, SyBase, MySql }

    public class RepositoryFactory
    {
        public IRepository GetRepository(string repository) =>
            CreateRepository(StringToProvider(repository));         

        public IRepository GetRepository(Provider repository) =>
            CreateRepository(repository);

        public Provider StringToProvider(string repository) => 
            (Provider)Enum.Parse(typeof(Provider), repository);

        private IRepository CreateRepository(Provider repository)
        {
            switch (repository)
            {
                case Provider.MicrosoftSql: return new MicrosoftDb();
                case Provider.MySql:        return new MySqlDb();
                case Provider.SyBase:       return new SyBaseDb();
            }
            return null;
        }        
    }
}
