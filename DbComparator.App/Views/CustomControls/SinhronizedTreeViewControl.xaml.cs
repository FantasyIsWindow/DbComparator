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
               
        private void treeView_ScrollChanged(object sender, ScrollChangedEventArgs e) =>
            CurrentVerticalScrollOffset = e.VerticalOffset;


        private void SetVerticalOffsetValue() =>
             SetVerticalOffset(treeView, VerticalScrollOffset);


        private void SetItemSource() =>
            treeView.ItemsSource = TVItemsSource;
        

        private bool SetVerticalOffset(DependencyObject obj, double offset)
        {
            bool terminate = false;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is ScrollViewer scrollV)
                {
                    scrollV.ScrollToVerticalOffset(offset);
                    return true;
                }
            }

            if (!terminate)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    terminate = SetVerticalOffset(VisualTreeHelper.GetChild(obj, i), offset);
                }
            }
            return false;
        }

        private void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetSelectedItem?.Invoke((Property)e.NewValue);
        }
    }
}
