using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WinFormsDashboard_DashboardState
{
    public partial class Form1 : XtraForm
    {
        public static readonly string PropertyName = "DashboardState";
        const string path = @"Dashboards\SampleDashboardWithState.xml";
        public Form1()
        {
            InitializeComponent();
            dashboardViewer1.SetInitialDashboardState += DashboardViewer1_SetInitialDashboardState;
            dashboardViewer1.ConfigureDataConnection += DashboardViewer1_ConfigureDataConnection;
            dashboardViewer1.CustomizeDashboardTitle += DashboardViewer1_CustomizeDashboardTitle;
            this.FormClosing += Form1_FormClosing;
            this.Load += Form1_Load;
        }

        private void DashboardViewer1_CustomizeDashboardTitle(object sender, CustomizeDashboardTitleEventArgs e)
        {
            DashboardToolbarItem resetStateItem = new DashboardToolbarItem("Reset State",
                new Action<DashboardToolbarItemClickEventArgs>((args) =>
                {
                    dashboardViewer1.SetDashboardState(CreateDashboardState()); ;
                }));
            resetStateItem.Caption = "Reset Dashboard State";
            e.Items.Add(resetStateItem);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dashboardViewer1.LoadDashboard(path);
        }
        DashboardState GetStateFromCustomProperty() {
            var customPropertyValue = dashboardViewer1.Dashboard.CustomProperties.GetValue(PropertyName);
            DashboardState state = new DashboardState();
            if(!string.IsNullOrEmpty(customPropertyValue)) {
                var xmlStateEl = XDocument.Parse(customPropertyValue);
                state.LoadFromXml(xmlStateEl);
            }
            return state;
        }
        private void DashboardViewer1_SetInitialDashboardState(object sender, SetInitialDashboardStateEventArgs e)
        {   
            var state = GetStateFromCustomProperty();
            e.InitialState = state;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var state = dashboardViewer1.GetDashboardState();
            var stateValue = state.SaveToXml().ToString(SaveOptions.DisableFormatting);
            dashboardViewer1.Dashboard.CustomProperties.SetValue("DashboardState", stateValue);
            dashboardViewer1.Dashboard.SaveToXml(path);
        }

        public DashboardState CreateDashboardState()
        {
            DashboardState state = new DashboardState();
            // Set a range for a Range Filter.
            state.Items.Add(new DashboardItemState("rangeFilterDashboardItem1")
            {
                RangeFilterState = new RangeFilterState(new RangeFilterSelection(new DateTime(2015, 1, 1), new DateTime(2017, 1, 1)))
            });
            // Specify master filter and drill-down values.
            state.Items.Add(new DashboardItemState("gridDashboardItem1")
            {
                MasterFilterValues = new List<object[]>() { new object[] { "Gravad lax" }, new object[] { "Ikura" } },
                DrillDownValues = new List<object>() { "Seafood" }
            });
            // Set a dashboard item layer.
            state.Items.Add(new DashboardItemState("treemapDashboardItem1")
            {
                SelectedLayerIndex = 1
            });
            // Specify a default tab page.
            state.Items.Add(new DashboardItemState("tabContainerDashboardItem1")
            {
                TabPageName = "dashboardTabPage2"
            });
            // Define a dashboard parameter value.
            state.Parameters.Add(new DashboardParameterState()
            {
                Name = "ParameterCountry",
                Value = "UK",
                Type = typeof(string)
            });
            return state;
        }
        private void DashboardViewer1_ConfigureDataConnection(object sender, DashboardConfigureDataConnectionEventArgs e)
        {
            if (e.DataSourceName == "SalesPerson")
                ((ExcelDataSourceConnectionParameters)e.ConnectionParameters).FileName = @"Data\SalesPerson.xlsx";
        }
    }
}
