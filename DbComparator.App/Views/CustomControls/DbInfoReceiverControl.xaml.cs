using DbComparator.App.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DbComparator.App.Views.CustomControls
{
    public partial class DbInfoReceiverControl : UserControl
    {
        private readonly string _pathToDefaultIcon = @"/Resources/Icons/DefaultDbIcon.png";
        private readonly string _defaultMessage = "Drag here";

        public DbInfoReceiverControl()
        {
            InitializeComponent();
            SetDefaultValues();
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
                        
            img.Source = (ImageSource)e.Data.GetData("ImageSource");
            var obj = (DbInfo)e.Data.GetData("Object");
            label.Content = obj.DataBase.DbName;
            this.DataContext = obj;            
        }

        private void Receiver_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                SetDefaultValues();
            }
        }

        /// <summary>
        /// Setting the default value
        /// </summary>
        private void SetDefaultValues()
        {
            Uri imageUri = new Uri(_pathToDefaultIcon, UriKind.Relative);
            BitmapImage image = new BitmapImage(imageUri);
            label.Content = _defaultMessage;
            img.Source = image;
        }
    }
}
