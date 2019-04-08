using DevExpress.DashboardCommon;
using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WinFormsDashboard_DashboardState
{
    public partial class Form1 : XtraForm
    {
        DashboardState dState = new DashboardState();
        public Form1()
        {
            InitializeComponent();
            dashboardViewer1.DashboardLoaded += DashboardViewer1_DashboardLoaded;
            dashboardViewer1.SetInitialDashboardState += DashboardViewer1_SetInitialDashboardState;
            dashboardViewer1.ConfigureDataConnection += DashboardViewer1_ConfigureDataConnection;
            this.FormClosing += Form1_FormClosing;
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dashboardViewer1.LoadDashboard("Data\\SampleDashboardWithState.xml");
        }

        private void DashboardViewer1_DashboardLoaded(object sender, DevExpress.DashboardWin.DashboardLoadedEventArgs e)
        {
            XElement data = e.Dashboard.UserData;
            if (data != null)
            {
                if (data.Element("DashboardState") != null)
                {
                    XDocument dStateDocument = XDocument.Parse(data.Element("DashboardState").Value);
                    dState.LoadFromXml(XDocument.Parse(data.Element("DashboardState").Value));
                }
            }
        }

        private void DashboardViewer1_SetInitialDashboardState(object sender, SetInitialDashboardStateEventArgs e)
        {
            e.InitialState = dState;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            dState = dashboardViewer1.GetDashboardState();
            XElement userData = new XElement("Root", new XElement("DateModified", DateTime.Now), new XElement("DashboardState", dState.SaveToXml().ToString(SaveOptions.DisableFormatting)));
            dashboardViewer1.Dashboard.UserData = userData;
            dashboardViewer1.Dashboard.SaveToXml("Data\\SampleDashboardWithState.xml");
        }
        private void DashboardViewer1_ConfigureDataConnection(object sender, DashboardConfigureDataConnectionEventArgs e)
        {
            ExtractDataSourceConnectionParameters connParams = e.ConnectionParameters as ExtractDataSourceConnectionParameters;
            connParams.FileName = "Data\\SalesPerson.dat";
        }
    }
}
