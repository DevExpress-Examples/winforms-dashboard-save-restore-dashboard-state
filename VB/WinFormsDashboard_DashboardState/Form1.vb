Imports DevExpress.DashboardCommon
Imports DevExpress.DashboardWin
Imports DevExpress.XtraEditors

Namespace WinFormsDashboard_DashboardState
    Partial Public Class Form1
        Inherits XtraForm

        Private dState As New DashboardState()
        Public Sub New()
            InitializeComponent()
            AddHandler dashboardViewer1.DashboardLoaded, AddressOf DashboardViewer1_DashboardLoaded
            AddHandler dashboardViewer1.SetInitialDashboardState, AddressOf DashboardViewer1_SetInitialDashboardState
            AddHandler dashboardViewer1.ConfigureDataConnection, AddressOf DashboardViewer1_ConfigureDataConnection
            AddHandler Me.FormClosing, AddressOf Form1_FormClosing
            AddHandler Me.Load, AddressOf Form1_Load
        End Sub

        Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs)
            dashboardViewer1.LoadDashboard("Data\SampleDashboardWithState.xml")
        End Sub

        Private Sub DashboardViewer1_DashboardLoaded(ByVal sender As Object, ByVal e As DevExpress.DashboardWin.DashboardLoadedEventArgs)
            Dim data As XElement = e.Dashboard.UserData
            If data IsNot Nothing Then
                If data.Element("DashboardState") IsNot Nothing Then
                    Dim dStateDocument As XDocument = XDocument.Parse(data.Element("DashboardState").Value)
                    dState.LoadFromXml(XDocument.Parse(data.Element("DashboardState").Value))
                End If
            End If
        End Sub

        Private Sub DashboardViewer1_SetInitialDashboardState(ByVal sender As Object, ByVal e As SetInitialDashboardStateEventArgs)
            e.InitialState = dState
        End Sub
        Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
            dState = dashboardViewer1.GetDashboardState()
            Dim userData As New XElement("Root", New XElement("DateModified", Date.Now), New XElement("DashboardState", dState.SaveToXml().ToString(SaveOptions.DisableFormatting)))
            dashboardViewer1.Dashboard.UserData = userData
            dashboardViewer1.Dashboard.SaveToXml("Data\SampleDashboardWithState.xml")
        End Sub
        Private Sub DashboardViewer1_ConfigureDataConnection(ByVal sender As Object, ByVal e As DashboardConfigureDataConnectionEventArgs)
            Dim connParams As ExtractDataSourceConnectionParameters = TryCast(e.ConnectionParameters, ExtractDataSourceConnectionParameters)
            connParams.FileName = "Data\SalesPerson.dat"
        End Sub
    End Class
End Namespace
