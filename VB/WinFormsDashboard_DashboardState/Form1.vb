Imports DevExpress.DashboardCommon
Imports DevExpress.DashboardWin
Imports DevExpress.XtraEditors
Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports System.Xml.Linq

Namespace WinFormsDashboard_DashboardState
	Partial Public Class Form1
		Inherits XtraForm

		Private dState As New DashboardState()
		Private Const path As String = "Dashboards\SampleDashboardWithState.xml"
		Public Sub New()
			InitializeComponent()
			AddHandler dashboardViewer1.DashboardLoaded, AddressOf DashboardViewer1_DashboardLoaded
			AddHandler dashboardViewer1.SetInitialDashboardState, AddressOf DashboardViewer1_SetInitialDashboardState
			AddHandler dashboardViewer1.ConfigureDataConnection, AddressOf DashboardViewer1_ConfigureDataConnection
			AddHandler dashboardViewer1.CustomizeDashboardTitle, AddressOf DashboardViewer1_CustomizeDashboardTitle
			AddHandler Me.FormClosing, AddressOf Form1_FormClosing
			AddHandler Me.Load, AddressOf Form1_Load
		End Sub

		Private Sub DashboardViewer1_CustomizeDashboardTitle(ByVal sender As Object, ByVal e As CustomizeDashboardTitleEventArgs)
			Dim resetStateItem As DashboardToolbarItem = New DashboardToolbarItem("Reset State", New Action(Of DashboardToolbarItemClickEventArgs)(Sub(args)
				dashboardViewer1.SetDashboardState(CreateDashboardState())

			End Sub))
			resetStateItem.Caption = "Reset Dashboard State"
			e.Items.Add(resetStateItem)
		End Sub

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs)
			dashboardViewer1.LoadDashboard(path)
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
			dashboardViewer1.Dashboard.SaveToXml(path)
		End Sub

		Public Function CreateDashboardState() As DashboardState
			Dim state As New DashboardState()
			' Set a range for a Range Filter.
			state.Items.Add(New DashboardItemState("rangeFilterDashboardItem1") With {.RangeFilterState = New RangeFilterState(New RangeFilterSelection(New Date(2015, 1, 1), New Date(2017, 1, 1)))})
			' Specify master filter and drill-down values.
			state.Items.Add(New DashboardItemState("gridDashboardItem1") With {
				.MasterFilterValues = New List(Of Object())() From {
					New Object() { "Gravad lax" },
					New Object() { "Ikura" }
				},
				.DrillDownValues = New List(Of Object)() From {"Seafood"}
			})
			' Set a dashboard item layer.
			state.Items.Add(New DashboardItemState("treemapDashboardItem1") With {.SelectedLayerIndex = 1})
			' Specify a default tab page.
			state.Items.Add(New DashboardItemState("tabContainerDashboardItem1") With {.TabPageName = "dashboardTabPage2"})
			' Define a dashboard parameter value.
			state.Parameters.Add(New DashboardParameterState() With {.Name = "ParameterCountry", .Value = "UK", .Type = GetType(String)})
			Return state
		End Function
		Private Sub DashboardViewer1_ConfigureDataConnection(ByVal sender As Object, ByVal e As DashboardConfigureDataConnectionEventArgs)
			If e.DataSourceName = "SalesPerson" Then
				CType(e.ConnectionParameters, ExcelDataSourceConnectionParameters).FileName = "Data\SalesPerson.xlsx"
			End If
		End Sub
	End Class
End Namespace
