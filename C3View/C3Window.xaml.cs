using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using C3;

namespace C3View
{
    // UI logic for the display window
    public partial class C3Window : Window
    {
        private CCRecordSet ccRecordSet;
        private string recordSetFileName;

        private DateTimeRange editDateTimeRange;
        private DateTimeRange summaryDateTimeRange;
        private string reportSelectedGroupBy;
        private string reportSelectedPeriod;
        private string reportSelectedAggregation;

        private string summaryAggregateColumn;
        private HashSet<string> requiredHeaderNames;
        private DataTable currentDataTable;
        private C3Configuration config;
        private Dictionary<string, string> columnFormats = new Dictionary<string, string>()
        {
            { Consts.TRANSACTIONTIME, Consts.DATETIMEDISPLAYFORMAT },
            { Consts.DESCRIPTION, string.Empty },
            { Consts.AMOUNT, Consts.AMOUNTDISPLAYFORMAT }
        };

        public C3Window()
        {
            InitializeComponent();
        }

        public C3Window(CCRecordSet ccRecordSet, string recordSetFileName, C3Configuration config) 
            : this()
        {
            this.ccRecordSet = ccRecordSet;
            this.recordSetFileName = recordSetFileName;
            this.config = config;
            this.editDateTimeRange = Selectors.timeFilters[Consts.TIMEEXPR_LASTMONTH];
            this.summaryDateTimeRange = Selectors.timeFilters[Consts.TIMEEXPR_LASTMONTH];

            this.requiredHeaderNames = new HashSet<string>(this.ccRecordSet.RequiredHeaderNames);
            currentDataTable = ccRecordSet.ToDataTable();

            UpdateEditDataGrid();

            foreach (var header in this.ccRecordSet.RequiredHeaderNames)
            {
                var col = new DataGridTextColumn
                {
                    Header = header,
                    Binding = new Binding(header)
                    {
                        StringFormat = columnFormats[header]
                    },
                    IsReadOnly = true
                };
                EditTabDatagrid.Columns.Add(col);
            }
            foreach (var header in config.columns)
            {
                var col = new DataGridComboBoxColumn
                {
                    Header = header.columnName,
                    ItemsSource = header.validValues,
                    SelectedValueBinding = new Binding(header.columnName)
                };
                EditTabDatagrid.Columns.Add(col);
            }

            EditTimeFilterComboBox.ItemsSource = Selectors.timeFilters.Keys;
            EditTimeFilterComboBox.SelectedItem = Selectors.timeFilters.Keys.FirstOrDefault();

            SummaryTimeFilterComboBox.ItemsSource = Selectors.timeFilters.Keys;
            SummaryTimeFilterComboBox.SelectedItem = Selectors.timeFilters.Keys.FirstOrDefault();
            SummaryAggregateComboBox.ItemsSource = this.ccRecordSet.PredictedHeaderNames;
            SummaryAggregateComboBox.SelectedItem = this.ccRecordSet.PredictedHeaderNames.FirstOrDefault();

            ReportGroupByComboBox.ItemsSource = this.ccRecordSet.PredictedHeaderNames;
            ReportGroupByComboBox.SelectedIndex = 0;
            ReportPeriodComboBox.ItemsSource = Selectors.periodSpecifiers.Keys;
            ReportPeriodComboBox.SelectedItem = Consts.PERIOD_SPECIFIER_MONTH;
            ReportAggregationComboBox.ItemsSource = Selectors.aggreations.Keys;
            ReportAggregationComboBox.SelectedItem = Consts.AGGREGATION_SUM;
        }

        private void UpdateEditDataGrid()
        {
            EditTabDatagrid.DataContext = Transforms.GetFilteredRecordSet(this.currentDataTable, this.editDateTimeRange)
                .AsDataView();
        }

        // Allows editing datagrid cells with two clicks rather than three.
        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                // Starts the Edit on the row;
                DataGrid grd = (DataGrid)sender;
                grd.BeginEdit(e);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CCRecordSet.FromDataTable(this.currentDataTable, config)
                    .SerializeToFile(this.recordSetFileName);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Couldn't save file: {ex.Message}");
            }
        }

        private void EditTimeFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ComboBox).SelectedItem.ToString();
            this.editDateTimeRange = Selectors.timeFilters[selectedItem];
            UpdateEditDataGrid();
        }

        // Summary tab methods ------------------------------------------------
        private void UpdateSummaryDataGrid()
        {
            if (this.summaryAggregateColumn != null)
            {
                var table = Transforms.GetSummaryTable(this.currentDataTable, this.summaryDateTimeRange, this.summaryAggregateColumn);
                SummaryTabDatagrid.ItemsSource = table;
            }
        }

        private void SummaryTimeFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ComboBox).SelectedItem.ToString();
            this.summaryDateTimeRange = Selectors.timeFilters[selectedItem];
            UpdateSummaryDataGrid();
        }

        private void SummaryAggregateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string columnName = (sender as ComboBox).SelectedItem.ToString();
            this.summaryAggregateColumn = columnName;
            UpdateSummaryDataGrid();
        }

        private void Datagrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.Decimal))
                (e.Column as DataGridTextColumn).Binding.StringFormat = Consts.AMOUNTDISPLAYFORMAT;
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = Consts.DATETIMEDISPLAYFORMAT;
        }

        // Report tab methods ------------------------------------------------
        private void UpdateReportDataGrid()
        {
            if (this.reportSelectedPeriod != null && this.reportSelectedGroupBy != null && this.reportSelectedAggregation != null)
            {
                C3PredictedColumn selectedColumn = this.config.columns
                    .Where(col => col.columnName.Equals(this.reportSelectedGroupBy, StringComparison.InvariantCultureIgnoreCase))
                    .FirstOrDefault();
                var periodGrouper = Selectors.periodSpecifiers[this.reportSelectedPeriod];
                var aggregation = Selectors.aggreations[this.reportSelectedAggregation];
                ReportTabDatagrid.ItemsSource = Transforms.GetPeriodSummary(this.currentDataTable, periodGrouper, selectedColumn, aggregation).AsDataView();
            }
        }

        private void ReportGroupByComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.reportSelectedGroupBy = (sender as ComboBox).SelectedItem.ToString();
            UpdateReportDataGrid();
        }

        private void ReportPeriodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.reportSelectedPeriod = (sender as ComboBox).SelectedItem.ToString();
            UpdateReportDataGrid();
        }

        private void ReportAggregationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.reportSelectedAggregation = (sender as ComboBox).SelectedItem.ToString();
            UpdateReportDataGrid();
        }

        // Global UI methods ------------------------------------------------
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                string tabName = ((e.Source as TabControl).SelectedItem as TabItem).Header.ToString();
                switch (tabName)
                {
                    case "Summary":
                        UpdateSummaryDataGrid();
                        break;
                    case "Report":
                        UpdateReportDataGrid();
                        break;
                }
            }
        }
    }
}
