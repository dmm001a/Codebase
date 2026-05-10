Imports System.ComponentModel
Imports System.Data
Imports System.Data.Common
Imports System.Data.Entity.Core.Common.CommandTrees
Imports System.Data.Odbc
Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Microsoft.Office.Interop
Imports Windows.Win32.System
Public Class frmViewReport

	Private Enum enuViewType
		IIB = 1
		ESG = 2
		ADV = 3
	End Enum

	'Private goDictionary As Dictionary(Of Integer, String)
	Private iClickedRowIndex As Integer = -1
	Private iClickedColumnIndex As Integer = -1
	Private gOriginalDataTable As DataTable

	Private Sub frmViewReport_Shown(sender As Object, e As EventArgs) Handles Me.Shown
		Try

			mResizeDataGridViewFixedOffset(Me, Me.grdReport)
			mPopulateGridView()
			grdReport.ContextMenuStrip = ctxmnuReportRClick
			grdReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect
			mLoadComboBox(Me.cboTestDateTimeFilter)

			Me.txtFilter.PlaceholderText = "Search here"

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub mPopulateGridView( _
	    Optional bMaxOnly As Boolean = False)
		Try

			Dim sSQL As String = ""
			Dim oDatabase As cDatabase
			Dim oDatatable As DataTable
			Dim sUserName As String

			oDatabase = New cDatabase(gConnectionString)

			sUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name

			sSQL = "SELECT tr.ID, strftime('%m/%d/%Y', tr.Test_Date_Time) || ' ' || strftime('%I:%M:%S %p', tr.Test_Date_Time) AS Test_Date_Time, env.Environment_Desc, api.API_Desc, "
			sSQL = sSQL & "strftime('%s', IIB_End_Time) - strftime('%s', IIB_Start_Time) AS IIB_Response_Time, "
			sSQL = sSQL & "strftime('%s', ESG_End_Time) - strftime('%s', ESG_Start_Time) AS ESG_Response_Time, "
			sSQL = sSQL & "strftime('%s', ADV_End_Time) - strftime('%s', ADV_Start_Time) AS ADV_Response_Time, "
			sSQL = sSQL & "tr.IIB_Summary, tr.ESG_Summary, tr.Adv_Summary, "
			sSQL = sSQL & "strftime('%I:%M:%S %p', IIB_Start_Time) AS IIB_Test_Start_Time, strftime('%I:%M:%S %p', IIB_END_Time) AS IIB_Test_End_Time, "
			sSQL = sSQL & "strftime('%I:%M:%S %p', ESG_Start_Time) AS ESG_Test_Start_Time, strftime('%I:%M:%S %p', ESG_END_Time) AS ESG_Test_End_Time, "
			sSQL = sSQL & "strftime('%I:%M:%S %p', ADV_Start_Time) AS ADV_Test_Start_Time, strftime('%I:%M:%S %p', ADV_END_Time) AS ADV_Test_End_Time, "
			sSQL = sSQL & "tr.IIB_Request, tr.IIB_Response, "
			sSQL = sSQL & "tr.ESG_Request, tr.ESG_Response, tr.Adv_Request, tr.Adv_Response "
			sSQL = sSQL & "FROM tb_Environment env INNER JOIN tb_Test_Report tr ON env.Environment_ID = tr.Test_Environment_ID "
			sSQL = sSQL & "INNER JOIN tb_API api ON tr.Test_API_ID = api.API_ID "
			If bMaxOnly = True Then
				sSQL = sSQL & "WHERE tr.Test_Date_Time = (SELECT MAX(Test_Date_Time) FROM tb_Test_Report) AND tr.Created_By = "  & mSingleQuotes(sUserName)
			Else
					sSQL = sSQL & "WHERE tr.Created_By = " & mSingleQuotes(sUserName)
			End If
			sSQL = sSQL & "ORDER BY tr.Test_Date_Time DESC"

			oDatatable = oDatabase.mGetDataTable(sSQL)

			grdReport.DataSource = Nothing
			grdReport.Rows.Clear()

			grdReport.DataSource = oDataTable
			gOriginalDataTable = oDataTable

			mFormatGridView()

			oDatabase = nothing

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub mFormatGridView()


		Try

			grdReport.EnableHeadersVisualStyles = False
			grdReport.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(21, 96, 130) ' Soft blue
			grdReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White

			grdReport.RowsDefaultCellStyle.BackColor = Color.FromArgb(192, 230, 245)
			grdReport.AlternatingRowsDefaultCellStyle.BackColor = Color.White


			mSetupColumns()

			grdReport.ReadOnly = True

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub grdReport_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles grdReport.CellFormatting
		Try

			mGridCellFormatting(e)

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub


	Private Sub mGridCellFormatting(e As DataGridViewCellFormattingEventArgs)
		Dim iResponseTime As Integer
		Dim sResponse As String
		Try

			Dim colName As String = Ucase(grdReport.Columns(e.ColumnIndex).Name)


			Select Case colName
				Case "IIB_RESPONSE_TIME", "ESG_RESPONSE_TIME", "ADV_RESPONSE_TIME"
					If e.Value IsNot Nothing AndAlso Not IsDBNull(e.Value) Then
						If Integer.TryParse(e.Value.ToString(), iResponseTime) Then
							Select Case iResponseTime
								Case <= 5
									e.CellStyle.BackColor = Color.LightGreen
									e.CellStyle.ForeColor = Color.Black
								Case 6 To 10
									e.CellStyle.BackColor = Color.Yellow
									e.CellStyle.ForeColor = Color.Black
								Case > 10
									e.CellStyle.BackColor = Color.Red
									e.CellStyle.ForeColor = Color.White
							End Select
						End If
					End If

				Case "IIB_SUMMARY", "IIB_SUMMARY", "ADV_SUMMARY"
					If e.Value IsNot Nothing AndAlso Not IsDBNull(e.Value) Then
						sResponse = e.Value.ToString()

						If Len(Trim(sResponse)) > 0 Then
							If InStr(sResponse, "Request Successful") > 0 Then
								e.CellStyle.BackColor = Color.LightGreen
								e.CellStyle.ForeColor = Color.Black
							ElseIf InStr(sResponse, "Request Failed") > 0 Then
								e.CellStyle.BackColor = Color.Red
								e.CellStyle.ForeColor = Color.White
							Else
								e.CellStyle.BackColor = Color.Yellow
								e.CellStyle.ForeColor = Color.Black
							End If
						End If
					End If
			End Select

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub grdReport_MouseDown(sender As Object, e As MouseEventArgs) Handles grdReport.MouseDown
		Dim sCellValue As String = ""
		Dim iIIBSummaryColumnIndex As Integer = 7
		Dim iESGSummaryColumnIndex As Integer = 8
		Dim iADVSummaryColumnIndex As Integer = 9
		Try
			If e.Button = MouseButtons.Right Then
				Dim hit As DataGridView.HitTestInfo = grdReport.HitTest(e.X, e.Y)
				If hit.RowIndex >= 0 Then

					'-------------------------------------------------------------------------------------------------------------------------
					'Check if the menu's should be enabled based on the contents of the cells.
					'-------------------------------------------------------------------------------------------------------------------------
					'Test IIB Summary.  If it's blank then disable the right click option to view the content of IIB request and response
					sCellValue = grdReport.Rows(hit.RowIndex).Cells(iIIBSummaryColumnIndex).Value.ToString()
					If mLenTrim(sCellValue) = 0 Then
						ctxmnuReportRClick.Items("ViewIIBRequestResponseToolStripMenuItem").Enabled = False
					Else
						ctxmnuReportRClick.Items("ViewIIBRequestResponseToolStripMenuItem").Enabled = True
					End if


					'Test ESG Summary.  If it's blank then disable the right click option to view the content of ESG request and response
					sCellValue = grdReport.Rows(hit.RowIndex).Cells(iESGSummaryColumnIndex).Value.ToString()
					If mLenTrim(sCellValue) = 0 Then
						ctxmnuReportRClick.Items("ViewESGRequestResponseToolStripMenuItem").Enabled = False
					Else
						ctxmnuReportRClick.Items("ViewESGRequestResponseToolStripMenuItem").Enabled = True
					End if

					'Test ADV Summary.  If it's blank then disable the right click option to view the content of ADV request and response
					sCellValue = grdReport.Rows(hit.RowIndex).Cells(iADVSummaryColumnIndex).Value.ToString()
					If mLenTrim(sCellValue) = 0 Then
						ctxmnuReportRClick.Items("ViewADVRequestResponseToolStripMenuItem").Enabled = False
					Else
						ctxmnuReportRClick.Items("ViewADVRequestResponseToolStripMenuItem").Enabled = True
					End if
					'-------------------------------------------------------------------------------------------------------------------------
					'End Check
					'-------------------------------------------------------------------------------------------------------------------------
					grdReport.ClearSelection()
					grdReport.Rows(hit.RowIndex).Selected = True
					ctxmnuReportRClick.Show(grdReport, e.Location)
				End If
			End If
		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub ViewIIBRequestResponseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewIIBRequestResponseToolStripMenuItem.Click
		Try

			mOpenViewRequestResponseForm(enuViewType.IIB)

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub mOpenViewRequestResponseForm(eViewType As enuViewType)


		Dim sRequestColumnName As String = ""
		Dim sResponseColumnName As String = ""
		Dim sRequest As String = ""
		Dim sResponse As String = ""
		Dim sAPIName As String = ""
		Dim sAPIColumnName As String = ""
		Dim oSelectedRow As DataGridViewRow

		Try
			sAPIColumnName = "API_Desc"
			Select Case eViewType
				Case enuViewType.IIB
					sRequestColumnName = "IIB_Request"
					sResponseColumnName = "IIB_Response"
				Case enuViewType.ESG
					sRequestColumnName = "ESG_Request"
					sResponseColumnName = "ESG_Response"
				Case enuViewType.ADV
					sRequestColumnName = "Adv_Request"
					sResponseColumnName = "Adv_Response"
			End Select

			' Check that a row is selected
			If grdReport.SelectedRows.Count > 0 Then
				oSelectedRow = grdReport.SelectedRows(0)
				sAPIName = oSelectedRow.Cells(sAPIColumnName).Value.ToString()
				sRequest = oSelectedRow.Cells(sRequestColumnName).Value.ToString()
				sResponse = oSelectedRow.Cells(sResponseColumnName).Value.ToString()

				frmViewRequestResponse.prpAPIName = sAPIName
				frmViewRequestResponse.prpRequest = sRequest
				frmViewRequestResponse.prpResponse = sResponse

				frmViewRequestResponse.ShowDialog()

			End If


		Catch ex As Exception
			mHandleError(ex)
		End Try

		oSelectedRow = Nothing

	End Sub

	Private Sub ViewESGRequestResponseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewESGRequestResponseToolStripMenuItem.Click
		Try

			mOpenViewRequestResponseForm(enuViewType.ESG)

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub ViewADVRequestResponseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewADVRequestResponseToolStripMenuItem.Click
		Try

			mOpenViewRequestResponseForm(enuViewType.ADV)

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub grdReport_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles grdReport.CellContentClick

	End Sub

	Private Sub grdReport_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles grdReport.CellMouseDown
		Dim iRowIndex As Integer
		Dim iColumnIndex As Integer

		Try

			'This selects the row upon right click

			If e.Button = MouseButtons.Right Then
				iRowIndex = e.RowIndex
				iColumnIndex = e.ColumnIndex

				If iRowIndex >= 0 AndAlso iColumnIndex >= 0 Then
					iClickedRowIndex = iRowIndex
					iClickedColumnIndex = iColumnIndex

					grdReport.ClearSelection()
					grdReport.Rows(iRowIndex).Selected = True
				End If
			End If

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub



	Private Sub mExportGridViewToExcel()
		' Declare variables at top
		Dim oXLApp As Excel.Application = Nothing
		Dim oXLBooks As Excel.Workbooks = Nothing
		Dim oXLBook As Excel.Workbook = Nothing
		Dim oXLSheets As Excel.Sheets = Nothing
		Dim oXLSheet As Excel.Worksheet = Nothing
		Dim iRow As Integer
		Dim iCol As Integer
		Dim sFilePath As String = "C:\temp\grid_export.xlsx"
		Dim oWaitingForm As New frmInProgress

		Try
			' Delete existing file if present
			If System.IO.File.Exists(sFilePath) Then
				System.IO.File.Delete(sFilePath)
			End If

			Application.UseWaitCursor = True
			oWaitingForm.Show()
			Application.DoEvents()

			' Start Excel
			oXLApp = New Excel.Application()
			oXLBooks = oXLApp.Workbooks
			oXLBook = oXLBooks.Add()
			oXLSheets = oXLBook.Sheets
			oXLSheet = CType(oXLSheets(1), Excel.Worksheet)

			' (A) Toggle this to True if you want to watch Excel run
			oXLApp.Visible = False

			' (B) Turn off any prompts
			oXLApp.DisplayAlerts = False

			Application.UseWaitCursor = True
			oWaitingForm.Show()
			Application.DoEvents()

			' Write headers
			For iCol = 0 To grdReport.Columns.Count - 1
				oXLSheet.Cells(1, iCol + 1) = grdReport.Columns(iCol).HeaderText
			Next

			' Write data rows
			For iRow = 0 To grdReport.Rows.Count - 1
				For iCol = 0 To grdReport.Columns.Count - 1
					oXLSheet.Cells(iRow + 2, iCol + 1) = _
					    grdReport.Rows(iRow).Cells(iCol).Value
				Next
			Next

			' Save as .xlsx explicitly to avoid mismatches
			oXLBook.SaveAs( _
			    Filename:=sFilePath, _
			    FileFormat:=Excel.XlFileFormat.xlOpenXMLWorkbook _
			)

			Application.UseWaitCursor = False

			oXLApp.Visible = True
			oWaitingForm.Close()

		Catch ex As Exception
			mHandleError(ex)

		Finally
			' Close + quit only when running headless
			If oXLApp IsNot Nothing AndAlso Not oXLApp.Visible Then
				If oXLBook IsNot Nothing Then oXLBook.Close(False)
				oXLApp.Quit()
			End If

			' Release COM objects in reverse order
			If oXLSheet IsNot Nothing Then Marshal.ReleaseComObject(oXLSheet) : oXLSheet = Nothing
			If oXLSheets IsNot Nothing Then Marshal.ReleaseComObject(oXLSheets) : oXLSheets = Nothing
			If oXLBook IsNot Nothing Then Marshal.ReleaseComObject(oXLBook) : oXLBook = Nothing
			If oXLBooks IsNot Nothing Then Marshal.ReleaseComObject(oXLBooks) : oXLBooks = Nothing
			If oXLApp IsNot Nothing Then Marshal.ReleaseComObject(oXLApp) : oXLApp = Nothing

			' Force-finalize any lingering COM wrappers
			GC.Collect()
			GC.WaitForPendingFinalizers()
		End Try

		If oWaitingForm IsNot Nothing AndAlso Not oWaitingForm.IsDisposed Then
			oWaitingForm.Close()
		End If
		Application.UseWaitCursor = False
	End Sub

	Private Sub ExcelExportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExcelExportToolStripMenuItem.Click
		Try

			mExportGridViewToExcel

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub grdReport_BackgroundImageLayoutChanged(sender As Object, e As EventArgs) Handles grdReport.BackgroundImageLayoutChanged

	End Sub

	Private Sub grdReport_CellMouseDoubleClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles grdReport.CellMouseDoubleClick

		Dim sValue As String

		Try

			' Validate indices
			If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 0 Then
				sValue = grdReport.Rows(e.RowIndex).Cells(e.ColumnIndex).Value.ToString()
				MessageBox.Show(sValue, "View Cell Value", MessageBoxButtons.OK, MessageBoxIcon.Information)
			End If

			Exit Sub

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub


	Private Sub mSetupColumns()

		Dim columnsToCenter As String() = {"ID", "Test_Date_Time", "Environment_Desc", "IIB_Response_Time", "ESG_Response_Time", "ADV_Response_Time", "IIB_Start_Time", "IIB_End_Time", "ESG_Start_Time", "ESG_End_Time", "ADV_Start_Time", "ADV_End_Time"}
		Dim columnsToAutoSize As String() = {"ID", "Test_Date_Time", "Environment_Desc", "IIB_Response_Time", "ESG_Response_Time", "ADV_Response_Time", "IIB_Test_Start_Time", "IIB_Test_End_Time", "ESG_Test_Start_Time", "ESG_Test_End_Time", "ADV_Test_Start_Time", "ADV_Test_End_Time", "API_Desc"}


		Try

			grdReport.Columns("IIB_Summary").Width = 150
			grdReport.Columns("ESG_Summary").Width = 150
			grdReport.Columns("ADV_Summary").Width = 150

			For Each oColName In columnsToCenter
				If grdReport.Columns.Contains(oColName) Then
					grdReport.Columns(oColName).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
				End If
			Next


			For Each oCol As DataGridViewColumn In grdReport.Columns
				oCol.SortMode = DataGridViewColumnSortMode.Automatic
				oCol.HeaderText = oCol.HeaderText.Replace("_", " ")
			Next

			For Each oColName In columnsToAutoSize
				If grdReport.Columns.Contains(oColName) Then
					grdReport.Columns(oColName).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
				End If
			Next

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub



	Private Sub frmViewReport_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

		Try
			'If goDictionary IsNot Nothing Then
			'    goDictionary = Nothing
			'End If

		Catch ex As Exception
			mHandleError(ex)
		End Try

	End Sub

	Private Sub txtFilter_TextChanged(sender As Object, e As EventArgs) Handles txtFilter.TextChanged

		Dim sfilterText As String = txtFilter.Text.Trim().Replace("'", "''")
		Dim sfilterExpression As String = ""

		Try

			If grdReport.rows.Count > 0 Then
				If String.IsNullOrWhiteSpace(sfilterText) Then
					grdReport.DataSource = gOriginalDataTable
					Exit Sub
				End If

				For Each col As DataColumn In gOriginalDataTable.Columns
					Select Case col.DataType
						Case GetType(String), GetType(Integer), GetType(Long), GetType(DateTime), GetType(Boolean)
							If sfilterExpression <> "" Then sfilterExpression &= " OR "
							sfilterExpression &= $"CONVERT([{col.ColumnName}], 'System.String') LIKE '%{sfilterText}%'"
					End Select
				Next

				Me.cboTestDateTimeFilter.SelectedIndex = -1
				Dim oDataView As New DataView(gOriginalDataTable)
				oDataView.RowFilter = sfilterExpression
				grdReport.DataSource = oDataView
			End If

		Catch ex As Exception
			mHandleError(ex)
		End Try


	End Sub


	Private Sub cboTestDateTimeFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTestDateTimeFilter.SelectedIndexChanged
		Dim oComboBox As System.Windows.Forms.ComboBox
		Dim sFilterDate As String
		Dim dFilterDate As DateTime
		Dim sFilterExpression As String
		Dim oFilteredDataView As DataView

		Try

			oComboBox = CType(sender, System.Windows.Forms.ComboBox)
			sFilterDate = Trim(oComboBox.Text)

			If sFilterDate.Length > 0 Then

				If DateTime.TryParseExact(sFilterDate, "MM/dd/yyyy hh:mm:ss tt", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dFilterDate) Then
					' Format to match stored string format
					Dim sFormattedDate As String = dFilterDate.ToString("MM/dd/yyyy hh:mm:ss tt", Globalization.CultureInfo.InvariantCulture)

					' Build filter expression
					sFilterExpression = $"CONVERT(Test_Date_Time, 'System.String') = '{sFormattedDate}'"
					Me.txtFilter.Text = ""

					' Apply filter
					oFilteredDataView = New DataView(gOriginalDataTable)
					oFilteredDataView.RowFilter = sFilterExpression
					grdReport.DataSource = oFilteredDataView
				Else
					grdReport.DataSource = gOriginalDataTable
				End If
			Else
				grdReport.DataSource = gOriginalDataTable
			End If

		Catch ex As Exception
			grdReport.DataSource = gOriginalDataTable
			mHandleError(ex)
		Finally

		End Try
	End Sub

	Private Sub btnDeleteReportHistory_Click(sender As Object, e As EventArgs) Handles btnDeleteReportHistory.Click
	     
          Dim oDatabase As cDatabase
		Dim sSQL As String
		Dim iResponse As Integer

		Try

			If Me.grdReport.Rows.Count > 0 Then
				iResponse = MessageBox.Show("Are you sure you want to delete all records from Test History?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
			
				If iResponse = DialogResult.Yes Then
				    sSQL = "DELETE From tb_Test_Report"
				    oDatabase = New cDatabase(gConnectionString)

				    oDatabase.mExecuteSQL(sSQL)

				    oDatabase = Nothing
				    gOriginalDataTable.Clear
				    mPopulateGridView
				    
				    cboTestDateTimeFilter.DataSource = Nothing
				    cboTestDateTimeFilter.Items.Clear()
					cboTestDateTimeFilter.SelectedIndex = -1
					cboTestDateTimeFilter.Text = ""

				End If
			Else
				MessageBox.Show("There are no rows to delete.", "Can't Delete", MessageBoxButtons.OK, MessageBoxIcon.Information)
			End If

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	'    Private Sub cboTestDateTimeFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTestDateTimeFilter.SelectedIndexChanged

	'            Dim oComboBox As System.Windows.Forms.ComboBox
	'            Dim sSelectedText As String 
	'            Dim sfilterExpression As String
	'            Dim sDate As String
	'            Dim dDate As Date

	'        Try

	'            oComboBox = CType(sender, System.Windows.Forms.ComboBox)
	'            sDate = oComboBox.Text

	'            'If mLenTrim(sDate)> 0 Then
	'            '    If DateTime.TryParse(sDate, dDate) Then
	'            '        sfilterExpression &= $"CONVERT([{col.ColumnName}], 'System.String') LIKE '%{sfilterText}%'"
	'            '    Else
	'            '        grdReport.DataSource = dtOriginal
	'            '    End If
	'            'Else
	'            '    grdReport.DataSource = dtOriginal
	'            'End If


	'        Catch ex As Exception
	'            MessageBox.Show("cboTestDateTimeFilter_SelectedIndexChanged: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
	'        End Try
	'    End Sub
End Class