#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataVirtualization;

#endregion

namespace DVFilterSort
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<CustomSortDescription> sortDescriptions;
        private CustomerProvider customerProvider;
        private int pageSize = 100;
        private int timePageInMemory = 5000;

        public MainWindow()
        {
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Off;

            InitializeComponent();

            string defaultSortColumnName = "CustomerSince";
            DataGridColumn defaultSortColumn = CustomersDataGrid.Columns.Single(dgc => GetColumnSortMemberPath(dgc) == defaultSortColumnName);
            sortDescriptions = new List<CustomSortDescription>
                {
                    new CustomSortDescription
                        {
                            PropertyName = defaultSortColumnName,
                            Direction = ListSortDirection.Descending,
                            Column = defaultSortColumn
                        }
                };
            RefreshData();
        }

        private void CustomerSinceDatePicker_DateChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void Customers_Sorting(object sender, DataGridSortingEventArgs e)
        {
            ApplySortColumn(e.Column);
            e.Handled = true;
        }

        private void RefreshData()
        {
            string sortString = GetCurrentSortString();
            customerProvider = new CustomerProvider(CustomerSinceDatePicker.DateFrom, CustomerSinceDatePicker.DateTo, sortString);
            var customerList = new AsyncVirtualizingCollection<Customer>(customerProvider, pageSize, timePageInMemory);
            DataContext = customerList;

            UpdateSortingVisualFeedback();

            CustomersDataGrid.SelectedIndex = 0;
        }

        private void ApplySortColumn(DataGridColumn column)
        {
            // If column was not sorted, we sort it ascending. If it was already sorted, we flip the sort direction.
            string sortColumn = GetColumnSortMemberPath(column);
            CustomSortDescription existingSortDescription = sortDescriptions.SingleOrDefault(sd => sd.PropertyName == sortColumn);
            if (existingSortDescription == null)
            {
                existingSortDescription = new CustomSortDescription
                    {
                        PropertyName = sortColumn,
                        Direction = ListSortDirection.Ascending,
                        Column = column
                    };
                sortDescriptions.Add(existingSortDescription);
            }
            else
            {
                existingSortDescription.Direction = (existingSortDescription.Direction == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            // If user is not pressing Shift, we remove all SortDescriptions except the current one.
            bool isShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) != 0;
            if (!isShiftPressed)
            {
                for (int i = sortDescriptions.Count - 1; i >= 0; i--)
                {
                    CustomSortDescription csd = sortDescriptions[i];
                    if (csd.PropertyName != sortColumn)
                    {
                        sortDescriptions.RemoveAt(i);
                    }
                }
            }

            RefreshData();
        }

        private string GetColumnSortMemberPath(DataGridColumn column)
        {
            string prefixToRemove = "Data.";
            string fullSortColumn = DataGridHelper.GetSortMemberPath(column);
            string sortColumn = fullSortColumn.Substring(prefixToRemove.Length);
            return sortColumn;
        }

        private string GetCurrentSortString()
        {
            // The result string is created, taking into account all sorted columns in the order they were sorted.
            var result = new StringBuilder();
            string separator = String.Empty;
            foreach (CustomSortDescription sd in sortDescriptions)
            {
                result.Append(separator);
                result.Append(sd.PropertyName);
                if (sd.Direction == ListSortDirection.Descending)
                {
                    result = result.Append(" DESC");
                }
                separator = ", ";
            }

            return result.ToString();
        }

        private void UpdateSortingVisualFeedback()
        {
            foreach (CustomSortDescription csd in sortDescriptions)
            {
                csd.Column.SortDirection = csd.Direction;
            }
        }
    }

    public class CustomSortDescription
    {
        public string PropertyName { get; set; }
        public ListSortDirection Direction { get; set; }
        public DataGridColumn Column { get; set; }
    }

    public enum USRegion
    {
        Northeast,
        Southeast,
        Midwest,
        Southwest,
        West
    }
}