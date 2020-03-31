using DbComparator.App.ViewModels;

namespace DbComparator.App.Models
{
    public class Property : ModelBase
    {
        private string _propertyType;

        private string _name;

        private string _color;

        public string PropertyType
        {
            get => _propertyType; 
            set => SetProperty(ref _propertyType, value, "PropertyType"); 
        }

        public string Name
        {
            get => _name; 
            set => SetProperty(ref _name, value, "Name"); 
        }
                                    
        public string Color
        {
            get => _color; 
            set => SetProperty(ref _color, value, "Color"); 
        }
    }
}
