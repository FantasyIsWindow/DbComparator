using System.Collections.Generic;

namespace DbComparator.App.Models
{
    public class GeneralDbInfo
    {
        public string Name { get; set; }

        public List<Entity> Entitys { get; set; }
    }
}
