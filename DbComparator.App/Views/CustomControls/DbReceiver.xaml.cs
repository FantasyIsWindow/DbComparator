using DbComparator.App.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DbComparator.App.Views.CustomControls
{
    public partial class DbReceiver : UserControl
    {
        public DbReceiver()
        {
            InitializeComponent();
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            img.Source = (ImageSource)e.Data.GetData("ImageSource");
            var obj = (DbInfo)e.Data.GetData("Object");
            this.DataContext = obj;            
        }
    }
}
