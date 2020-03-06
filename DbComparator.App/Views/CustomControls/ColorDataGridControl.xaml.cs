using Comparator.Repositories.Models.DbModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DbComparator.App.Views.CustomControls
{
    public partial class ColorDataGridControl : UserControl
    {
        private ObservableCollection<FullField> _fields;

        private bool _isUpdate;

        public static DependencyProperty FieldsCollectionProperty = DependencyProperty.Register
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

        public static DependencyProperty FieldsToCompareCollectionProperty = DependencyProperty.Register
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

        private static void SetFieldsCollection(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorDataGridControl)d).SetItemSource();
        }

        private static void SetFieldsToCompareCollection(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorDataGridControl)d).SetFieldsToCompare();
        }

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
        }

        private void CellCompare()
        {
            if (FieldsCollection?.Count() != 0 && FieldsToCompareCollection?.Count() != 0)
            {
                var redColor = new SolidColorBrush(Colors.Red);
                var yellowColor = new SolidColorBrush(Colors.Yellow);
                               
                for (int i = 0; i < dataGrid.Items.Count; i++)
                {
                    for (int f = 0; f < 10; f++)
                    {
                        DataGridRow row = dataGrid.ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                        var fieldName = dataGrid.Columns[f].Header as string;
                        var textBlock = dataGrid.Columns[f].GetCellContent(row) as TextBlock;
                        string value = textBlock.Text;

                        if (!IsTrue(fieldName, value, i))
                        {
                            textBlock.Background = value == "" ? yellowColor : redColor;
                        }                
                    }
                }
            }

            _isUpdate = true;
        }

        private bool IsTrue(string fieldName, string name, int index)
        {
            switch (fieldName)
            {
                case "FieldName":       return _fields[index].FieldName      == name;
                case "TypeName":        return _fields[index].TypeName       == name;
                case "Size":            return _fields[index].Size           == name;
                case "IsNullable":      return _fields[index].IsNullable     == name;
                case "ConstraintType":  return _fields[index].ConstraintType == name;
                case "ConstraintName":  return _fields[index].ConstraintName == name;
                case "ConstraintKeys":  return _fields[index].ConstraintKeys == name;
                case "References":      return _fields[index].References     == name;
                case "OnUpdate":        return _fields[index].OnUpdate       == name;
                case "OnDelete":        return _fields[index].OnDelete       == name;
            }
            return false;
        }

        private void UserControl_LayoutUpdated(object sender, System.EventArgs e)
        {
            if(!_isUpdate)
            {
                CellCompare();
            }
        }
    }
}
