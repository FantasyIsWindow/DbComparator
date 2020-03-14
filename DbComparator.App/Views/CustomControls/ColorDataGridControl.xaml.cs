using Comparator.Repositories.Models.DbModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace DbComparator.App.Views.CustomControls
{
    public partial class ColorDataGridControl : UserControl
    {
        private ObservableCollection<FullField> _fields;

        private bool _isUpdate;

        private bool _isResize;

        public static readonly DependencyProperty FieldsCollectionProperty = DependencyProperty.Register
            (
                "FieldsCollection",
                typeof(ObservableCollection<FullField>),
                typeof(ColorDataGridControl),
                new FrameworkPropertyMetadata
                (
                    null,
                    new PropertyChangedCallback(SetFieldsCollection)
                )
            );

        public static readonly DependencyProperty FieldsToCompareCollectionProperty = DependencyProperty.Register
            (
                "FieldsToCompareCollection",
                typeof(ObservableCollection<FullField>),
                typeof(ColorDataGridControl),
                new FrameworkPropertyMetadata
                (
                    null,
                    new PropertyChangedCallback(SetFieldsToCompareCollection)
                )
            );

        public static readonly DependencyProperty HorizontalScrollOffsetProperty = DependencyProperty.Register
            (
                "HorizontalScrollOffset",
                typeof(double),
                typeof(ColorDataGridControl),
                new FrameworkPropertyMetadata
                (
                    0.0,
                    new PropertyChangedCallback(SetHorizontalScrollBarPosition)
                )
            );

        public static readonly DependencyProperty CurrentHorizontalScrollOffsetProperty = DependencyProperty.Register
            (
                "CurrentHorizontalScrollOffset",
                typeof(double),
                typeof(ColorDataGridControl)
            );

        public static readonly DependencyProperty SetCellsWidthProperty = DependencyProperty.Register
            (
                "SetCellsWidth",
                typeof(double[]),
                typeof(ColorDataGridControl)
            );

        public static readonly DependencyProperty GetCellsWidthProperty = DependencyProperty.Register
        (
            "GetCellsWidth",
            typeof(double[]),
            typeof(ColorDataGridControl)
        );

        public ObservableCollection<FullField> FieldsCollection
        {
            get => (ObservableCollection<FullField>)GetValue(FieldsCollectionProperty);
            set => SetValue(FieldsCollectionProperty, value);
        }

        public ObservableCollection<FullField> FieldsToCompareCollection
        {
            get => (ObservableCollection<FullField>)GetValue(FieldsToCompareCollectionProperty);
            set => SetValue(FieldsToCompareCollectionProperty, value);
        }

        public double HorizontalScrollOffset
        {
            get => (double)GetValue(HorizontalScrollOffsetProperty);
            set => SetValue(HorizontalScrollOffsetProperty, value);
        }     
        
        public double CurrentHorizontalScrollOffset
        {
            get => (double)GetValue(CurrentHorizontalScrollOffsetProperty);
            set => SetValue(CurrentHorizontalScrollOffsetProperty, value);
        }

        public double[] SetCellsWidth
        {
            get => (double[])GetValue(SetCellsWidthProperty);
            set => SetValue(SetCellsWidthProperty, value);
        }       
        
        public double[] GetCellsWidth
        {
            get => (double[])GetValue(GetCellsWidthProperty);
            set => SetValue(GetCellsWidthProperty, value);
        }

        private static void SetFieldsCollection(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((ColorDataGridControl)d).SetItemSource();
        

        private static void SetFieldsToCompareCollection(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((ColorDataGridControl)d).SetFieldsToCompare();
        

        private static void SetHorizontalScrollBarPosition(DependencyObject d, DependencyPropertyChangedEventArgs e) => 
            ((ColorDataGridControl)d).SetHorizontalOffsetValue(); 
        

        private void SetFieldsToCompare()
        {
            if (FieldsToCompareCollection != null)
            {
                _fields = FieldsToCompareCollection;
            }
        }

        public ColorDataGridControl()
        {
            InitializeComponent();          
        }

        public void SetItemSource()
        {
            dataGrid.ItemsSource = FieldsCollection;
            _isUpdate = false;
            _isResize = false;
        }

        private void UserControl_LayoutUpdated(object sender, System.EventArgs e)
        {
            if (!_isUpdate)
            {
                CellColorize();
                GetCurrentCellsWidth();
            }

            if(_isUpdate && !_isResize && CellWith())
            {                
                CellWidthAlignment();
            }
        }

        private bool CellWith() => 
            SetCellsWidth != null && SetCellsWidth.Length > 0 ? true : false;    
        

        private void CellColorize()
        {
            if (FieldsCollection?.Count() != 0 && FieldsToCompareCollection?.Count() != 0)
            {
                var redColor = new SolidColorBrush(Colors.Red);
                var yellowColor = new SolidColorBrush(Colors.Yellow);

                for (int i = 0; i < dataGrid.Items.Count; i++)
                {
                    for (int f = 0; f < dataGrid.Columns.Count; f++)
                    {
                        DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                        var cell = GetCells(row, f);
                        if (cell != null)
                        {
                            var fieldName = dataGrid.Columns[f].Header as string;
                            var textBlock = cell.Content as TextBlock;
                            string tbText = textBlock.Text;

                            if (!IsTrue(fieldName, tbText, i))
                            {
                                cell.Background = tbText == "" ? yellowColor : redColor;
                            }
                        }
                    }
                }
            }
            _isUpdate = true;
        }

        private DataGridCell GetCells(DataGridRow row, int index)
        {
            if (row != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(index);
                if (cell == null)
                {
                    dataGrid.ScrollIntoView(row, dataGrid.Columns[index]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(index);
                }
                return cell;
            }
            return null;
        }

        private T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private bool IsTrue(string fieldName, string name, int index)
        {
            switch (fieldName)
            {
                case "FieldName": return _fields[index].FieldName == name;
                case "TypeName": return _fields[index].TypeName == name;
                case "Size": return _fields[index].Size == name;
                case "IsNullable": return _fields[index].IsNullable == name;
                case "ConstraintType": return _fields[index].ConstraintType == name;
                case "ConstraintName": return _fields[index].ConstraintName == name;
                case "ConstraintKeys": return _fields[index].ConstraintKeys == name;
                case "References": return _fields[index].References == name;
                case "OnUpdate": return _fields[index].OnUpdate == name;
                case "OnDelete": return _fields[index].OnDelete == name;
            }
            return false;
        }

        private void dataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e) =>
            CurrentHorizontalScrollOffset = e.HorizontalOffset;
        

        private void SetHorizontalOffsetValue() =>
             SetHorizontalOffset(dataGrid, HorizontalScrollOffset);
        

        private bool SetHorizontalOffset(DependencyObject obj, double offset)
        {
            bool terminate = false;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if(child is ScrollViewer scrollV)
                {
                    scrollV.ScrollToHorizontalOffset(offset);
                    return true;
                }
            }

            if(!terminate)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    terminate = SetHorizontalOffset(VisualTreeHelper.GetChild(obj, i), offset);                  
                }
            }
            return false;
        }

        private void GetCurrentCellsWidth()
        {
            GetCellsWidth = new double[dataGrid.Columns.Count];

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                GetCellsWidth[i] = dataGrid.Columns[i].ActualWidth;
            }
        }

        private void CellWidthAlignment()
        {
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if(dataGrid.Columns[i].ActualWidth < SetCellsWidth[i])
                {
                    dataGrid.Columns[i].Width = SetCellsWidth[i];
                }
            }
            _isResize = true;
        }
    }
}
