<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
	<System.Diagnostics.DebuggerStepThrough()>
	Private Sub InitializeComponent()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
		frmSettings = New GroupBox()
		chkALLAPI = New CheckBox()
		cmdExecute = New Button()
		frmTestEndpoints = New GroupBox()
		chkAdvEndpoint = New CheckBox()
		chkESGEndpoint = New CheckBox()
		chkIIBEndpoint = New CheckBox()
		cmdSave = New Button()
		lblValue = New Label()
		txtAttributeValue = New TextBox()
		lblAttribute = New Label()
		lblAPI = New Label()
		lblEnvironment = New Label()
		cboAttribute = New ComboBox()
		cboAPI = New ComboBox()
		cboEnvironment = New ComboBox()
		frmRequestObjects = New GroupBox()
		lblAdvantage = New Label()
		TxtAdvRequestObject = New TextBox()
		lblESG = New Label()
		txtESGRequestObject = New TextBox()
		lblIIB = New Label()
		txtIIBRequestObject = New TextBox()
		mnuMainMenu = New MenuStrip()
		OpenGridReportToolStripMenuItem = New ToolStripMenuItem()
		PollingToolStripMenuItem = New ToolStripMenuItem()
		OpenAPIQAToolStripMenuItem = New ToolStripMenuItem()
		mnuOpenLogAnalyzer = New ToolStripMenuItem()
		mnuOpenSOP = New ToolStripMenuItem()
		mnuOpenDBAdmin = New ToolStripMenuItem()
		mnuSetPath = New ToolStripMenuItem()
		mnuExcelReportPath = New ToolStripMenuItem()
		mnuLogAnalyzerPath = New ToolStripMenuItem()
		mnuDBPath = New ToolStripMenuItem()
		mnuDSNName = New ToolStripMenuItem()
		lblVersion = New ToolStripStatusLabel()
		SSStatus = New StatusStrip()
		ssVersion = New ToolStripStatusLabel()
		btnSaveRequestObject = New Button()
		frmSettings.SuspendLayout()
		frmTestEndpoints.SuspendLayout()
		frmRequestObjects.SuspendLayout()
		mnuMainMenu.SuspendLayout()
		SSStatus.SuspendLayout()
		SuspendLayout()
		' 
		' frmSettings
		' 
		frmSettings.Controls.Add(chkALLAPI)
		frmSettings.Controls.Add(cmdExecute)
		frmSettings.Controls.Add(frmTestEndpoints)
		frmSettings.Controls.Add(cmdSave)
		frmSettings.Controls.Add(lblValue)
		frmSettings.Controls.Add(txtAttributeValue)
		frmSettings.Controls.Add(lblAttribute)
		frmSettings.Controls.Add(lblAPI)
		frmSettings.Controls.Add(lblEnvironment)
		frmSettings.Controls.Add(cboAttribute)
		frmSettings.Controls.Add(cboAPI)
		frmSettings.Controls.Add(cboEnvironment)
		frmSettings.Location = New Point(14, 27)
		frmSettings.Name = "frmSettings"
		frmSettings.Size = New Size(1430, 176)
		frmSettings.TabIndex = 0
		frmSettings.TabStop = False
		frmSettings.Text = "Execution Settings"
		' 
		' chkALLAPI
		' 
		chkALLAPI.AutoSize = True
		chkALLAPI.Enabled = False
		chkALLAPI.Location = New Point(976, 56)
		chkALLAPI.Name = "chkALLAPI"
		chkALLAPI.Size = New Size(112, 19)
		chkALLAPI.TabIndex = 4
		chkALLAPI.Text = "Execute All API's"
		chkALLAPI.UseVisualStyleBackColor = True
		' 
		' cmdExecute
		' 
		cmdExecute.Enabled = False
		cmdExecute.Location = New Point(1323, 150)
		cmdExecute.Name = "cmdExecute"
		cmdExecute.Size = New Size(101, 20)
		cmdExecute.TabIndex = 5
		cmdExecute.Text = "Execute"
		cmdExecute.UseVisualStyleBackColor = True
		' 
		' frmTestEndpoints
		' 
		frmTestEndpoints.Controls.Add(chkAdvEndpoint)
		frmTestEndpoints.Controls.Add(chkESGEndpoint)
		frmTestEndpoints.Controls.Add(chkIIBEndpoint)
		frmTestEndpoints.Location = New Point(741, 37)
		frmTestEndpoints.Name = "frmTestEndpoints"
		frmTestEndpoints.Size = New Size(219, 47)
		frmTestEndpoints.TabIndex = 9
		frmTestEndpoints.TabStop = False
		frmTestEndpoints.Text = "Endpoints to Test:"
		' 
		' chkAdvEndpoint
		' 
		chkAdvEndpoint.AutoSize = True
		chkAdvEndpoint.Location = New Point(127, 20)
		chkAdvEndpoint.Name = "chkAdvEndpoint"
		chkAdvEndpoint.Size = New Size(83, 19)
		chkAdvEndpoint.TabIndex = 3
		chkAdvEndpoint.Text = "Advantage"
		chkAdvEndpoint.UseVisualStyleBackColor = True
		' 
		' chkESGEndpoint
		' 
		chkESGEndpoint.AutoSize = True
		chkESGEndpoint.Enabled = False
		chkESGEndpoint.Location = New Point(68, 21)
		chkESGEndpoint.Name = "chkESGEndpoint"
		chkESGEndpoint.Size = New Size(46, 19)
		chkESGEndpoint.TabIndex = 1
		chkESGEndpoint.Text = "ESG"
		chkESGEndpoint.UseMnemonic = False
		chkESGEndpoint.UseVisualStyleBackColor = True
		' 
		' chkIIBEndpoint
		' 
		chkIIBEndpoint.AutoSize = True
		chkIIBEndpoint.Checked = True
		chkIIBEndpoint.CheckState = CheckState.Checked
		chkIIBEndpoint.Location = New Point(16, 21)
		chkIIBEndpoint.Name = "chkIIBEndpoint"
		chkIIBEndpoint.Size = New Size(39, 19)
		chkIIBEndpoint.TabIndex = 2
		chkIIBEndpoint.Text = "IIB"
		chkIIBEndpoint.UseVisualStyleBackColor = True
		' 
		' cmdSave
		' 
		cmdSave.Enabled = False
		cmdSave.Location = New Point(611, 128)
		cmdSave.Name = "cmdSave"
		cmdSave.Size = New Size(71, 23)
		cmdSave.TabIndex = 8
		cmdSave.Text = "Save"
		cmdSave.UseVisualStyleBackColor = True
		' 
		' lblValue
		' 
		lblValue.AutoSize = True
		lblValue.Location = New Point(340, 133)
		lblValue.Name = "lblValue"
		lblValue.Size = New Size(38, 15)
		lblValue.TabIndex = 7
		lblValue.Text = "Value:"
		' 
		' txtAttributeValue
		' 
		txtAttributeValue.Enabled = False
		txtAttributeValue.Location = New Point(394, 127)
		txtAttributeValue.Name = "txtAttributeValue"
		txtAttributeValue.Size = New Size(178, 23)
		txtAttributeValue.TabIndex = 7
		' 
		' lblAttribute
		' 
		lblAttribute.AutoSize = True
		lblAttribute.Location = New Point(6, 128)
		lblAttribute.Name = "lblAttribute"
		lblAttribute.Size = New Size(57, 15)
		lblAttribute.TabIndex = 5
		lblAttribute.Text = "Attribute:"
		' 
		' lblAPI
		' 
		lblAPI.AutoSize = True
		lblAPI.Location = New Point(340, 51)
		lblAPI.Name = "lblAPI"
		lblAPI.Size = New Size(28, 15)
		lblAPI.TabIndex = 4
		lblAPI.Text = "API:"
		' 
		' lblEnvironment
		' 
		lblEnvironment.AutoSize = True
		lblEnvironment.Location = New Point(6, 55)
		lblEnvironment.Name = "lblEnvironment"
		lblEnvironment.Size = New Size(78, 15)
		lblEnvironment.TabIndex = 3
		lblEnvironment.Text = "Environment:"
		' 
		' cboAttribute
		' 
		cboAttribute.DropDownStyle = ComboBoxStyle.DropDownList
		cboAttribute.Enabled = False
		cboAttribute.FormattingEnabled = True
		cboAttribute.Location = New Point(126, 125)
		cboAttribute.Name = "cboAttribute"
		cboAttribute.Size = New Size(175, 23)
		cboAttribute.TabIndex = 6
		' 
		' cboAPI
		' 
		cboAPI.DropDownStyle = ComboBoxStyle.DropDownList
		cboAPI.Enabled = False
		cboAPI.FormattingEnabled = True
		cboAPI.Location = New Point(394, 50)
		cboAPI.Name = "cboAPI"
		cboAPI.Size = New Size(174, 23)
		cboAPI.TabIndex = 1
		' 
		' cboEnvironment
		' 
		cboEnvironment.DropDownStyle = ComboBoxStyle.DropDownList
		cboEnvironment.FormattingEnabled = True
		cboEnvironment.Location = New Point(126, 50)
		cboEnvironment.Name = "cboEnvironment"
		cboEnvironment.Size = New Size(175, 23)
		cboEnvironment.TabIndex = 0
		' 
		' frmRequestObjects
		' 
		frmRequestObjects.Controls.Add(lblAdvantage)
		frmRequestObjects.Controls.Add(TxtAdvRequestObject)
		frmRequestObjects.Controls.Add(lblESG)
		frmRequestObjects.Controls.Add(txtESGRequestObject)
		frmRequestObjects.Controls.Add(lblIIB)
		frmRequestObjects.Controls.Add(txtIIBRequestObject)
		frmRequestObjects.Location = New Point(11, 273)
		frmRequestObjects.Name = "frmRequestObjects"
		frmRequestObjects.Size = New Size(1432, 542)
		frmRequestObjects.TabIndex = 2
		frmRequestObjects.TabStop = False
		frmRequestObjects.Text = "Request Objects"
		' 
		' lblAdvantage
		' 
		lblAdvantage.AutoSize = True
		lblAdvantage.Font = New Font("Segoe UI", 14F, FontStyle.Bold)
		lblAdvantage.ForeColor = SystemColors.HotTrack
		lblAdvantage.Location = New Point(1128, 22)
		lblAdvantage.Name = "lblAdvantage"
		lblAdvantage.Size = New Size(108, 25)
		lblAdvantage.TabIndex = 5
		lblAdvantage.Text = "Advantage"
		' 
		' TxtAdvRequestObject
		' 
		TxtAdvRequestObject.Enabled = False
		TxtAdvRequestObject.Location = New Point(969, 48)
		TxtAdvRequestObject.Multiline = True
		TxtAdvRequestObject.Name = "TxtAdvRequestObject"
		TxtAdvRequestObject.ScrollBars = ScrollBars.Vertical
		TxtAdvRequestObject.Size = New Size(415, 484)
		TxtAdvRequestObject.TabIndex = 11
		' 
		' lblESG
		' 
		lblESG.AutoSize = True
		lblESG.Font = New Font("Segoe UI", 14F, FontStyle.Bold)
		lblESG.ForeColor = SystemColors.HotTrack
		lblESG.Location = New Point(700, 20)
		lblESG.Name = "lblESG"
		lblESG.Size = New Size(47, 25)
		lblESG.TabIndex = 3
		lblESG.Text = "ESG"
		' 
		' txtESGRequestObject
		' 
		txtESGRequestObject.Enabled = False
		txtESGRequestObject.Location = New Point(509, 48)
		txtESGRequestObject.Multiline = True
		txtESGRequestObject.Name = "txtESGRequestObject"
		txtESGRequestObject.ScrollBars = ScrollBars.Vertical
		txtESGRequestObject.Size = New Size(415, 484)
		txtESGRequestObject.TabIndex = 10
		' 
		' lblIIB
		' 
		lblIIB.AutoSize = True
		lblIIB.Font = New Font("Segoe UI", 14F, FontStyle.Bold)
		lblIIB.ForeColor = SystemColors.HotTrack
		lblIIB.Location = New Point(215, 22)
		lblIIB.Name = "lblIIB"
		lblIIB.Size = New Size(36, 25)
		lblIIB.TabIndex = 1
		lblIIB.Text = "IIB"
		' 
		' txtIIBRequestObject
		' 
		txtIIBRequestObject.Enabled = False
		txtIIBRequestObject.Location = New Point(24, 50)
		txtIIBRequestObject.Multiline = True
		txtIIBRequestObject.Name = "txtIIBRequestObject"
		txtIIBRequestObject.ScrollBars = ScrollBars.Vertical
		txtIIBRequestObject.Size = New Size(415, 484)
		txtIIBRequestObject.TabIndex = 9
		' 
		' mnuMainMenu
		' 
		mnuMainMenu.ImageScalingSize = New Size(20, 20)
		mnuMainMenu.Items.AddRange(New ToolStripItem() {OpenGridReportToolStripMenuItem, PollingToolStripMenuItem, OpenAPIQAToolStripMenuItem, mnuOpenLogAnalyzer, mnuOpenSOP, mnuOpenDBAdmin, mnuSetPath})
		mnuMainMenu.Location = New Point(0, 0)
		mnuMainMenu.Name = "mnuMainMenu"
		mnuMainMenu.Size = New Size(1455, 24)
		mnuMainMenu.TabIndex = 3
		' 
		' OpenGridReportToolStripMenuItem
		' 
		OpenGridReportToolStripMenuItem.Name = "OpenGridReportToolStripMenuItem"
		OpenGridReportToolStripMenuItem.Size = New Size(78, 20)
		OpenGridReportToolStripMenuItem.Text = "Test Report"
		' 
		' PollingToolStripMenuItem
		' 
		PollingToolStripMenuItem.Name = "PollingToolStripMenuItem"
		PollingToolStripMenuItem.Size = New Size(56, 20)
		PollingToolStripMenuItem.Text = "Polling"
		' 
		' OpenAPIQAToolStripMenuItem
		' 
		OpenAPIQAToolStripMenuItem.Name = "OpenAPIQAToolStripMenuItem"
		OpenAPIQAToolStripMenuItem.Size = New Size(36, 20)
		OpenAPIQAToolStripMenuItem.Text = "QA"
		' 
		' mnuOpenLogAnalyzer
		' 
		mnuOpenLogAnalyzer.Name = "mnuOpenLogAnalyzer"
		mnuOpenLogAnalyzer.Size = New Size(87, 20)
		mnuOpenLogAnalyzer.Text = "Log Analyzer"
		' 
		' mnuOpenSOP
		' 
		mnuOpenSOP.Name = "mnuOpenSOP"
		mnuOpenSOP.Size = New Size(80, 20)
		mnuOpenSOP.Text = "Documents"
		' 
		' mnuOpenDBAdmin
		' 
		mnuOpenDBAdmin.Name = "mnuOpenDBAdmin"
		mnuOpenDBAdmin.Size = New Size(73, 20)
		mnuOpenDBAdmin.Text = "DB Admin"
		' 
		' mnuSetPath
		' 
		mnuSetPath.DropDownItems.AddRange(New ToolStripItem() {mnuExcelReportPath, mnuLogAnalyzerPath, mnuDBPath, mnuDSNName})
		mnuSetPath.Name = "mnuSetPath"
		mnuSetPath.Size = New Size(35, 20)
		mnuSetPath.Text = "Set"
		' 
		' mnuExcelReportPath
		' 
		mnuExcelReportPath.Name = "mnuExcelReportPath"
		mnuExcelReportPath.Size = New Size(169, 22)
		mnuExcelReportPath.Text = "Excel Report Path"
		mnuExcelReportPath.Visible = False
		' 
		' mnuLogAnalyzerPath
		' 
		mnuLogAnalyzerPath.Name = "mnuLogAnalyzerPath"
		mnuLogAnalyzerPath.Size = New Size(169, 22)
		mnuLogAnalyzerPath.Text = "Log Analyzer Path"
		' 
		' mnuDBPath
		' 
		mnuDBPath.Name = "mnuDBPath"
		mnuDBPath.Size = New Size(169, 22)
		mnuDBPath.Text = "DB Path"
		mnuDBPath.Visible = False
		' 
		' mnuDSNName
		' 
		mnuDSNName.Name = "mnuDSNName"
		mnuDSNName.Size = New Size(169, 22)
		mnuDSNName.Text = "DSN Name"
		mnuDSNName.Visible = False
		' 
		' lblVersion
		' 
		lblVersion.Name = "lblVersion"
		lblVersion.Size = New Size(0, 17)
		lblVersion.TextAlign = ContentAlignment.MiddleRight
		' 
		' SSStatus
		' 
		SSStatus.ImageScalingSize = New Size(20, 20)
		SSStatus.Items.AddRange(New ToolStripItem() {lblVersion, ssVersion})
		SSStatus.Location = New Point(0, 819)
		SSStatus.Name = "SSStatus"
		SSStatus.Size = New Size(1455, 22)
		SSStatus.TabIndex = 1
		' 
		' ssVersion
		' 
		ssVersion.Name = "ssVersion"
		ssVersion.Size = New Size(0, 17)
		' 
		' btnSaveRequestObject
		' 
		btnSaveRequestObject.Enabled = False
		btnSaveRequestObject.Location = New Point(1298, 252)
		btnSaveRequestObject.Name = "btnSaveRequestObject"
		btnSaveRequestObject.Size = New Size(140, 25)
		btnSaveRequestObject.TabIndex = 4
		btnSaveRequestObject.Text = "Save Request Objects"
		btnSaveRequestObject.UseVisualStyleBackColor = True
		' 
		' frmMain
		' 
		AutoScaleDimensions = New SizeF(7F, 15F)
		AutoScaleMode = AutoScaleMode.Font
		ClientSize = New Size(1455, 841)
		Controls.Add(btnSaveRequestObject)
		Controls.Add(frmRequestObjects)
		Controls.Add(SSStatus)
		Controls.Add(mnuMainMenu)
		Controls.Add(frmSettings)
		Icon = CType(resources.GetObject("$this.Icon"), Icon)
		MainMenuStrip = mnuMainMenu
		MaximizeBox = False
		Name = "frmMain"
		StartPosition = FormStartPosition.CenterScreen
		Text = "API Tester - Beta"
		frmSettings.ResumeLayout(False)
		frmSettings.PerformLayout()
		frmTestEndpoints.ResumeLayout(False)
		frmTestEndpoints.PerformLayout()
		frmRequestObjects.ResumeLayout(False)
		frmRequestObjects.PerformLayout()
		mnuMainMenu.ResumeLayout(False)
		mnuMainMenu.PerformLayout()
		SSStatus.ResumeLayout(False)
		SSStatus.PerformLayout()
		ResumeLayout(False)
		PerformLayout()
	End Sub

	Friend WithEvents frmSettings As GroupBox
    Friend WithEvents lblAttribute As Label
    Friend WithEvents lblAPI As Label
    Friend WithEvents lblEnvironment As Label
    Friend WithEvents cboAttribute As ComboBox
    Friend WithEvents cboAPI As ComboBox
    Friend WithEvents cboEnvironment As ComboBox
    Friend WithEvents lblValue As Label
    Friend WithEvents txtAttributeValue As TextBox
    Friend WithEvents cmdSave As Button
    Friend WithEvents cmdExecute As Button
    Friend WithEvents frmTestEndpoints As GroupBox
    Friend WithEvents chkAdvEndpoint As CheckBox
    Friend WithEvents chkESGEndpoint As CheckBox
    Friend WithEvents chkIIBEndpoint As CheckBox
    Friend WithEvents frmRequestObjects As GroupBox
    Friend WithEvents txtIIBRequestObject As TextBox
    Friend WithEvents mnuMainMenu As MenuStrip
    Friend WithEvents lblIIB As Label
    Friend WithEvents lblESG As Label
    Friend WithEvents txtESGRequestObject As TextBox
    Friend WithEvents lblAdvantage As Label
    Friend WithEvents TxtAdvRequestObject As TextBox
    Friend WithEvents chkALLAPI As CheckBox
    Friend WithEvents mnuOpenLogAnalyzer As ToolStripMenuItem
    Friend WithEvents mnuOpenSOP As ToolStripMenuItem
    Friend WithEvents mnuSetPath As ToolStripMenuItem
    Friend WithEvents mnuExcelReportPath As ToolStripMenuItem
    Friend WithEvents mnuLogAnalyzerPath As ToolStripMenuItem
    Friend WithEvents mnuDBPath As ToolStripMenuItem
    Friend WithEvents mnuDSNName As ToolStripMenuItem
    Friend WithEvents lblVersion As ToolStripStatusLabel
    Friend WithEvents SSStatus As StatusStrip
    Friend WithEvents OpenGridReportToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ssVersion As ToolStripStatusLabel
    Friend WithEvents mnuOpenDBAdmin As ToolStripMenuItem
	Friend WithEvents btnSaveRequestObject As Button
	Friend WithEvents PollingToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents OpenAPIQAToolStripMenuItem As ToolStripMenuItem

End Class
