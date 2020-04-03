using DbComparator.App.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DbComparator.App.Views.CustomControls
{
    public partial class SinhronizedTreeViewControl : UserControl
    {
        public static readonly DependencyProperty TVItemsSourceProperty = DependencyProperty.Register
            (
                "TVItemsSource",
                typeof(ObservableCollection<GeneralDbInfo>),
                typeof(SinhronizedTreeViewControl),
                new FrameworkPropertyMetadata
                    (
                        null,
                        new PropertyChangedCallback(SetItemsSourceToTreeView)
                    )
            );

        public static readonly DependencyProperty VerticalScrollOffsetProperty = DependencyProperty.Register
            (
                "VerticalScrollOffset",
                typeof(double),
                typeof(SinhronizedTreeViewControl),
                new FrameworkPropertyMetadata
                    (
                        0.0,
                        new PropertyChangedCallback(SetVerticalScrollBarPosition)
                    )
            );

        public static readonly DependencyProperty CurrentVerticalScrollOffsetProperty = DependencyProperty.Register
            (
                "CurrentVerticalScrollOffset",
                typeof(double),
                typeof(SinhronizedTreeViewControl)
            );

        public static readonly DependencyProperty SetSelectedItemProperty = DependencyProperty.Register
            (
                "SetSelectedItem",
                typeof(Action<Property>),
                typeof(SinhronizedTreeViewControl),
                new PropertyMetadata(null)
            );

        public ObservableCollection<GeneralDbInfo> TVItemsSource
        {
            get => (ObservableCollection<GeneralDbInfo>)GetValue(TVItemsSourceProperty);
            set => SetValue(TVItemsSourceProperty, value);
        }

        public double VerticalScrollOffset
        {
            get => (double)GetValue(VerticalScrollOffsetProperty);
            set => SetValue(VerticalScrollOffsetProperty, value);
        }

        public double CurrentVerticalScrollOffset
        {
            get => (double)GetValue(CurrentVerticalScrollOffsetProperty);
            set => SetValue(CurrentVerticalScrollOffsetProperty, value);
        }

        public Action<Property> SetSelectedItem
        {
            get => (Action<Property>)GetValue(SetSelectedItemProperty);
            set => SetValue(SetSelectedItemProperty, value);
        }


        public SinhronizedTreeViewControl()
        {
            InitializeComponent();
        }

        private static void SetItemsSourceToTreeView(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((SinhronizedTreeViewControl)d).SetItemSource();

        private static void SetVerticalScrollBarPosition(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((SinhronizedTreeViewControl)d).SetVerticalOffsetValue();

        private void TreeView_ScrollChanged(object sender, ScrollChangedEventArgs e) =>
            CurrentVerticalScrollOffset = e.VerticalOffset;


        private void SetVerticalOffsetValue() =>
             SetVerticalOffset(treeView, VerticalScrollOffset);


        private void SetItemSource() =>
            treeView.ItemsSource = TVItemsSource;

        /// <summary>
        /// Setting the vertical offset value
        /// </summary>
        /// <param name="obj">A control containing the Scroll Viewer</param>
        /// <param name="offset">Offset value</param>
        /// <returns>The result of the installation</returns>
        private bool SetVerticalOffset(DependencyObject obj, double offset)
        {

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is ScrollViewer scrollV)
                {
                    scrollV.ScrollToVerticalOffset(offset);
                    return true;
                }
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                SetVerticalOffset(VisualTreeHelper.GetChild(obj, i), offset);
            }
            return false;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Property value)
            {
                SetSelectedItem?.Invoke(value);
            }
        }
    }
}
