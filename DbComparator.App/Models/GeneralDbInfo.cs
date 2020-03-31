using DbComparator.App.ViewModels;
using System.Collections.Generic;

namespace DbComparator.App.Models
{
    public class GeneralDbInfo : ModelBase
    {
        private string _name;

        private List<Entity> _entitys;

        public string Name
        {
            get => _name; 
            set => SetProperty(ref _name, value, "Name");
        }
               
        public List<Entity> Entitys
        {
            get => _entitys; 
            set => SetProperty(ref _entitys, value, "Entitys"); 
        }
    }
}
