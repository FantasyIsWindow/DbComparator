using System;

namespace Comparator.Repositories.Repositories
{
    /// <summary>
    /// Sets options for supported database providers
    /// </summary>
    public enum Provider { MicrosoftSql, SyBase, MySql }

    public class RepositoryFactory
    {
        /// <summary>
        /// Returns the selected repository
        /// </summary>
        /// <param name="repository">Repository name</param>
        /// <returns>Repository</returns>
        public IRepository GetRepository(string repository) =>
            CreateRepository(StringToProvider(repository));

        /// <summary>
        /// Returns the repository
        /// </summary>
        /// <param name="repository">Repository name</param>
        /// <returns>Repository</returns>
        public IRepository GetRepository(Provider repository) =>
            CreateRepository(repository);

        /// <summary>
        /// Converts the repository name
        /// </summary>
        /// <param name="repository">Repository name</param>
        /// <returns>The converted repository name</returns>
        public Provider StringToProvider(string repository) => 
            (Provider)Enum.Parse(typeof(Provider), repository);

        /// <summary>
        /// Returns the repository
        /// </summary>
        /// <param name="repository">Repository name</param>
        /// <returns>Repository</returns>
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
