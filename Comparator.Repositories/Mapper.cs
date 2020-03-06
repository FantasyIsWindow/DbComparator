using AutoMapper;
using Comparator.Repositories.Mappings;

namespace Comparator.Repositories
{
    public static class Mapper
    {
        private static IMapper _map;

        public static IMapper Map
        {
            get { return _map; }
            set { _map = value; }
        }


        static Mapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _map = config.CreateMapper();
        }
    }
}
