<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmViewReport
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		components = New ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewReport))
		grdReport = New DataGridView()
		ctxmnuReportRClick = New ContextMenuStrip(components)
		ViewIIBRequestResponseToolStripMenuItem = New ToolStripMenuItem()
		ViewESGRequestResponseToolStripMenuItem = New ToolStripMenuItem()
		ViewADVRequestResponseToolStripMenuItem = New ToolStripMenuItem()
		ExcelExportToolStripMenuItem = New ToolStripMenuItem()
		cboTestDateTimeFilter = New ComboBox()
		txtFilter = New TextBox()
		btnDeleteReportHistory = New Button()
		CType(grdReport, ComponentModel.ISupportInitialize).BeginInit()
		ctxmnuReportRClick.SuspendLayout()
		SuspendLayout()
		' 
		' grdReport
		' 
		grdReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
		grdReport.Location = New Point(13, 47)
		grdReport.Name = "grdReport"
		grdReport.Size = New Size(1242, 517)
		grdReport.TabIndex = 2
		' 
		' ctxmnuReportRClick
		' 
		ctxmnuReportRClick.Items.AddRange(New ToolStripItem() {ViewIIBRequestResponseToolStripMenuItem, ViewESGRequestResponseToolStripMenuItem, ViewADVRequestResponseToolStripMenuItem, ExcelExportToolStripMenuItem})
		ctxmnuReportRClick.Name = "ctxmnuReportRClick"
		ctxmnuReportRClick.Size = New Size(226, 92)
		' 
		' ViewIIBRequestResponseToolStripMenuItem
		' 
		ViewIIBRequestResponseToolStripMenuItem.Name = "ViewIIBRequestResponseToolStripMenuItem"
		ViewIIBRequestResponseToolStripMenuItem.Size = New Size(225, 22)
		ViewIIBRequestResponseToolStripMenuItem.Text = "View IIB Request-Response"
		' 
		' ViewESGRequestResponseToolStripMenuItem
		' 
		ViewESGRequestResponseToolStripMenuItem.Name = "ViewESGRequestResponseToolStripMenuItem"
		ViewESGRequestResponseToolStripMenuItem.Size = New Size(225, 22)
		ViewESGRequestResponseToolStripMenuItem.Text = "View ESG Request-Response"
		' 
		' ViewADVRequestResponseToolStripMenuItem
		' 
		ViewADVRequestResponseToolStripMenuItem.Name = "ViewADVRequestResponseToolStripMenuItem"
		ViewADVRequestResponseToolStripMenuItem.Size = New Size(225, 22)
		ViewADVRequestResponseToolStripMenuItem.Text = "View ADV Request-Response"
		' 
		' ExcelExportToolStripMenuItem
		' 
		ExcelExportToolStripMenuItem.Name = "ExcelExportToolStripMenuItem"
		ExcelExportToolStripMenuItem.Size = New Size(225, 22)
		ExcelExportToolStripMenuItem.Text = "Excel Export"
		' 
		' cboTestDateTimeFilter
		' 
		cboTestDateTimeFilter.DropDownStyle = ComboBoxStyle.DropDownList
		cboTestDateTimeFilter.FormattingEnabled = True
		cboTestDateTimeFilter.Location = New Point(13, 18)
		cboTestDateTimeFilter.Name = "cboTestDateTimeFilter"
		cboTestDateTimeFilter.Size = New Size(164, 23)
		cboTestDateTimeFilter.TabIndex = 0
		' 
		' txtFilter
		' 
		txtFilter.Location = New Point(183, 18)
		txtFilter.Name = "txtFilter"
		txtFilter.Size = New Size(955, 23)
		txtFilter.TabIndex = 1
		' 
		' btnDeleteReportHistory
		' 
		btnDeleteReportHistory.Location = New Point(1155, 18)
		btnDeleteReportHistory.Name = "btnDeleteReportHistory"
		btnDeleteReportHistory.Size = New Size(93, 23)
		btnDeleteReportHistory.TabIndex = 3
		btnDeleteReportHistory.Text = "Delete History"
		btnDeleteReportHistory.UseVisualStyleBackColor = True
		' 
		' frmViewReport
		' 
		AutoScaleDimensions = New SizeF(7F, 15F)
		AutoScaleMode = AutoScaleMode.Font
		ClientSize = New Size(1260, 570)
		Controls.Add(btnDeleteReportHistory)
		Controls.Add(txtFilter)
		Controls.Add(cboTestDateTimeFilter)
		Controls.Add(grdReport)
		Icon = CType(resources.GetObject("$this.Icon"), Icon)
		Name = "frmViewReport"
		StartPosition = FormStartPosition.CenterScreen
		Text = "View API Test Report"
		WindowState = FormWindowState.Maximized
		CType(grdReport, ComponentModel.ISupportInitialize).EndInit()
		ctxmnuReportRClick.ResumeLayout(False)
		ResumeLayout(False)
		PerformLayout()
	End Sub

	Friend WithEvents grdReport As DataGridView
    Friend WithEvents ctxmnuReportRClick As ContextMenuStrip
    Friend WithEvents ViewIIBRequestResponseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ViewESGRequestResponseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ViewADVRequestResponseToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExcelExportToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents cboTestDateTimeFilter As ComboBox
    Friend WithEvents txtFilter As TextBox
	Friend WithEvents btnDeleteReportHistory As Button
End Class
