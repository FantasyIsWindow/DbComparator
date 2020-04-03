using Comparator.Repositories.Models.DtoModels;
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
        private ObservableCollection<DtoFullField> _fields;

        private bool _isUpdate;

        private bool _isResize;

        public static readonly DependencyProperty FieldsCollectionProperty = DependencyProperty.Register
            (
                "FieldsCollection",
                typeof(ObservableCollection<DtoFullField>),
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
                typeof(ObservableCollection<DtoFullField>),
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

        public static readonly DependencyProperty IsAutoProperty = DependencyProperty.Register
            (
                "IsAuto",
                typeof(bool),
                typeof(ColorDataGridControl)
            );

        public ObservableCollection<DtoFullField> FieldsCollection
        {
            get => (ObservableCollection<DtoFullField>)GetValue(FieldsCollectionProperty);
            set => SetValue(FieldsCollectionProperty, value);
        }

        public ObservableCollection<DtoFullField> FieldsToCompareCollection
        {
            get => (ObservableCollection<DtoFullField>)GetValue(FieldsToCompareCollectionProperty);
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

        public bool IsAuto
        {
            get => (bool)GetValue(IsAutoProperty);
            set => SetValue(IsAutoProperty, value);
        }

        private static void SetFieldsCollection(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((ColorDataGridControl)d).SetItemSource();


        private static void SetFieldsToCompareCollection(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((ColorDataGridControl)d).SetFieldsToCompare();


        private static void SetHorizontalScrollBarPosition(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((ColorDataGridControl)d).SetHorizontalOffsetValue();
                     
        public ColorDataGridControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Setting field values for table comparison
        /// </summary>
        private void SetFieldsToCompare()
        {
            _fields = FieldsToCompareCollection;
        }

        /// <summary>
        /// Installing the data source data grid
        /// </summary>
        public void SetItemSource()
        {
            dataGrid.ItemsSource = FieldsCollection;
            _isResize = false;
            _isUpdate = false;
        }

        private void UserControl_LayoutUpdated(object sender, System.EventArgs e)
        {
            DataGridCellColorize();
            DataGridCellsResize();
        }

        /// <summary>
        /// Checking collections of tables and coloring cells depending on the comparison result
        /// </summary>
        private void DataGridCellColorize()
        {
            if (!_isUpdate && IsAuto)
            {
                if (IsEmpty(FieldsCollection) && IsEmpty(FieldsToCompareCollection))
                {
                    Colorize();
                }
                _isUpdate = true;
            }
        }

        /// <summary>
        /// Comparison and alignment of the width of the table cells
        /// </summary>
        private void DataGridCellsResize()
        {
            if (!_isResize)
            {
                GetCurrentCellsWidth();
                if (SetCellsWidth != null && SetCellsWidth.Length > 0)
                {
                    CellWidthAlignment();
                }
            }
        }

        /// <summary>
        /// Checking the collection for null or empty
        /// </summary>
        /// <param name="fields">Fields collection</param>
        /// <returns>Comparison result</returns>
        private bool IsEmpty(ObservableCollection<DtoFullField> fields)
        {
            if (fields == null)
            {
                return false;
            }

            foreach (var field in fields)
            {
                if (field.FieldName != "")
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Color cells based on the results of comparing the values of two tables
        /// </summary>
        private void Colorize()
        {
            if (FieldsCollection?.Count() != 0 && FieldsToCompareCollection?.Count() != 0)
            {
                var redColor = new SolidColorBrush(Colors.Red) { Opacity = 0.5 };
                var yellowColor = new SolidColorBrush(Colors.Yellow) { Opacity = 0.5 };

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
        }

        /// <summary>
        /// Finds a cell in the passed string
        /// </summary>
        /// <param name="row">Data grid row</param>
        /// <param name="index">Current index</param>
        /// <returns>Data grid cell</returns>
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

        /// <summary>
        /// Finds and returns the visual child of the required control
        /// </summary>
        /// <typeparam name="T">A generalized class</typeparam>
        /// <param name="parent">Primitives</param>
        /// <returns>Visual child</returns>
        private T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default;
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

        /// <summary>
        /// Compares the fields of a table
        /// </summary>
        /// <param name="fieldName">Reference value</param>
        /// <param name="name">Compare value</param>
        /// <param name="index">Current index</param>
        /// <returns>Comparison result</returns>
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
                case "References": return _fields[index].Referenced == name;
                case "OnUpdate": return _fields[index].OnUpdate == name;
                case "OnDelete": return _fields[index].OnDelete == name;
            }
            return false;
        }

        private void DataGrid_ScrollChanged(object sender, ScrollChangedEventArgs e) =>
            CurrentHorizontalScrollOffset = e.HorizontalOffset;

        /// <summary>
        /// Calling the method for setting the horizontal offset value
        /// </summary>
        private void SetHorizontalOffsetValue() =>
             SetHorizontalOffset(dataGrid, HorizontalScrollOffset);

        /// <summary>
        /// Setting the horizontal offset value
        /// </summary>
        /// <param name="obj">A control containing the Scroll Viewer</param>
        /// <param name="offset">Offset value</param>
        /// <returns>The result of the installation</returns>
        private bool SetHorizontalOffset(DependencyObject obj, double offset)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is ScrollViewer scrollV)
                {
                    scrollV.ScrollToHorizontalOffset(offset);
                    return true;
                }
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                SetHorizontalOffset(VisualTreeHelper.GetChild(obj, i), offset);
            }

            return false;
        }

        /// <summary>
        /// Get the width of cells in the current data table
        /// </summary>
        private void GetCurrentCellsWidth()
        {
            GetCellsWidth = new double[dataGrid.Columns.Count];

            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                GetCellsWidth[i] = dataGrid.Columns[i].ActualWidth;
            }
        }

        /// <summary>
        /// The alignment of the width of cells in data grid
        /// </summary>
        private void CellWidthAlignment()
        {
            for (int i = 0; i < dataGrid.Columns.Count; i++)
            {
                if (dataGrid.Columns[i].ActualWidth < SetCellsWidth[i])
                {
                    dataGrid.Columns[i].Width = SetCellsWidth[i];
                }
            }
            _isResize = true;
        }
    }
}
