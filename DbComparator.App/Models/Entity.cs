using DbComparator.App.ViewModels;
using System.Collections.Generic;

namespace DbComparator.App.Models
{
    public class Entity : ModelBase
    {
        private string _name;

        private List<Property> _properties;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, "Name");
        }

        public List<Property> Properties
        {
            get => _properties;
            set => SetProperty(ref _properties, value, "Properties");
        }
    }
}
