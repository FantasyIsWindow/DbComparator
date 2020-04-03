using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DbComparator.App.Views.CustomControls
{
    public partial class DbInfoTransmitterControl : UserControl
    {
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register
            (
                "Icon", 
                typeof(string), 
                typeof(DbInfoTransmitterControl), 
                new FrameworkPropertyMetadata
                (
                    "", 
                    new PropertyChangedCallback(GetIcon)
                )
            );      

        public string Icon
        {
            get => (string)GetValue(IconProperty); 
            set => SetValue(IconProperty, value); 
        }

        public DbInfoTransmitterControl()
        {
            InitializeComponent();
        }                          

        private static void GetIcon(DependencyObject d, DependencyPropertyChangedEventArgs e) => 
            ((DbInfoTransmitterControl)d).InstallingIcon();

        /// <summary>
        /// Installing the icon
        /// </summary>
        private void InstallingIcon()
        {
            Uri imageUri = new Uri(Icon, UriKind.Relative);
            BitmapImage image = new BitmapImage(imageUri);
            img.Source = image;
        }

        /// <summary>
        /// Handling the mouse movement event
        /// </summary>
        /// <param name="e">Event</param>
        protected override void OnMouseMove(MouseEventArgs e) 
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DataObject data = new DataObject();
                data.SetData("ImageSource", this.img.Source);
                data.SetData("Object", this.DataContext);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        /// <summary>
        /// Getting feedback
        /// </summary>
        /// <param name="e">Event</param>
        protected override void OnGiveFeedback(GiveFeedbackEventArgs e) 
        {
            base.OnGiveFeedback(e);
            if (e.Effects.HasFlag(DragDropEffects.Copy))
            {
                Mouse.SetCursor(Cursors.Cross); 
            }
            else if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.Pen);
            }
            else
            {
                Mouse.SetCursor(Cursors.No);
            }
            e.Handled = true;
        }
    }
}
