using DbComparator.App.Models;
using System.Windows;
using System.Windows.Controls;

namespace DbComparator.App.Resources.Selectors
{
    public class DbListBoxSelector : DataTemplateSelector
    {
        public DataTemplate Connected { get; set; }

        public DataTemplate Disconected { get; set; }

        public DataTemplate AddNew { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DbInfo info)
            {
                if (info.DataBase == null)
                {
                    return AddNew;
                }
                else if (info.DataBase.Reference == "No")
                {
                    return info.IsConnect  ? Connected   :
                           !info.IsConnect ? Disconected : 
                           null;
                }
                else if (info.DataBase.Reference == "Yes")
                {
                    return info.IsConnect ? Connected :
                           !info.IsConnect ? Disconected :
                           null;
                }
            }

            return null;
        }
    }
}
