using System.Collections.Generic;

namespace DbComparator.App.Models
{
    public class Entity
    {
        public string Name { get; set; }

        public List<Property> Properties { get; set; }
    }
}
