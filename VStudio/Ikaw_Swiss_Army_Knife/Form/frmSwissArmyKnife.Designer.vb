<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSwissArmyKnife
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
        ctlMain = New TabControl()
        tbLauncher = New TabPage()
        cboDeployApp = New ComboBox()
        cboDeploymentLocation = New ComboBox()
        btnLaunch = New Button()
        cboApplicationSuite = New ComboBox()
        tbTOM = New TabPage()
        tbNotes = New TabPage()
        tbSourceControl = New TabPage()
        cboApp = New ComboBox()
        btnGetLabel = New Button()
        cboLabel = New ComboBox()
        btnAddApp = New Button()
        btnCheckInLabel = New Button()
        tbCode = New TabPage()
        tbTime = New TabPage()
        cboDestinationTimeZone = New ComboBox()
        txtConvertedTime = New TextBox()
        btnConvertTime = New Button()
        cboOriginTimeZone = New ComboBox()
        dtUSTPicker = New DateTimePicker()
        lblMessage = New Label()
        ctlMain.SuspendLayout()
        tbLauncher.SuspendLayout()
        tbSourceControl.SuspendLayout()
        tbTime.SuspendLayout()
        SuspendLayout()
        ' 
        ' ctlMain
        ' 
        ctlMain.Controls.Add(tbLauncher)
        ctlMain.Controls.Add(tbTOM)
        ctlMain.Controls.Add(tbNotes)
        ctlMain.Controls.Add(tbSourceControl)
        ctlMain.Controls.Add(tbCode)
        ctlMain.Controls.Add(tbTime)
        ctlMain.Font = New Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        ctlMain.Location = New Point(1, 9)
        ctlMain.Name = "ctlMain"
        ctlMain.RightToLeft = RightToLeft.No
        ctlMain.SelectedIndex = 0
        ctlMain.Size = New Size(520, 61)
        ctlMain.TabIndex = 2
        ' 
        ' tbLauncher
        ' 
        tbLauncher.Controls.Add(cboDeployApp)
        tbLauncher.Controls.Add(cboDeploymentLocation)
        tbLauncher.Controls.Add(btnLaunch)
        tbLauncher.Controls.Add(cboApplicationSuite)
        tbLauncher.Location = New Point(4, 22)
        tbLauncher.Name = "tbLauncher"
        tbLauncher.Padding = New Padding(3)
        tbLauncher.Size = New Size(512, 35)
        tbLauncher.TabIndex = 1
        tbLauncher.Text = "Launch"
        tbLauncher.UseVisualStyleBackColor = True
        ' 
        ' cboDeployApp
        ' 
        cboDeployApp.DropDownStyle = ComboBoxStyle.DropDownList
        cboDeployApp.FormattingEnabled = True
        cboDeployApp.Items.AddRange(New Object() {"DR", "Roadmap", "JiraForm", "Retirement"})
        cboDeployApp.Location = New Point(264, 6)
        cboDeployApp.Name = "cboDeployApp"
        cboDeployApp.Size = New Size(87, 21)
        cboDeployApp.TabIndex = 10
        ' 
        ' cboDeploymentLocation
        ' 
        cboDeploymentLocation.DropDownStyle = ComboBoxStyle.DropDownList
        cboDeploymentLocation.FormattingEnabled = True
        cboDeploymentLocation.Items.AddRange(New Object() {"Local", "Smoke", "QA", "Prod"})
        cboDeploymentLocation.Location = New Point(362, 6)
        cboDeploymentLocation.Name = "cboDeploymentLocation"
        cboDeploymentLocation.Size = New Size(69, 21)
        cboDeploymentLocation.TabIndex = 4
        ' 
        ' btnLaunch
        ' 
        btnLaunch.Location = New Point(434, 6)
        btnLaunch.Name = "btnLaunch"
        btnLaunch.Size = New Size(75, 23)
        btnLaunch.TabIndex = 3
        btnLaunch.Text = "Launch"
        btnLaunch.UseVisualStyleBackColor = True
        ' 
        ' cboApplicationSuite
        ' 
        cboApplicationSuite.DropDownStyle = ComboBoxStyle.DropDownList
        cboApplicationSuite.FormattingEnabled = True
        cboApplicationSuite.Items.AddRange(New Object() {"Web Development Suite", "Deploy Web Application", "API Tester", "AWS Workspace", "Copilot", "Facebook", "LinkedIn", "Notepad++", "Screenshot", "Smartsheet", "SQL Server", "To Do", "Visual Studio", "VS Code"})
        cboApplicationSuite.Location = New Point(6, 6)
        cboApplicationSuite.Name = "cboApplicationSuite"
        cboApplicationSuite.Size = New Size(253, 21)
        cboApplicationSuite.TabIndex = 2
        ' 
        ' tbTOM
        ' 
        tbTOM.Location = New Point(4, 22)
        tbTOM.Name = "tbTOM"
        tbTOM.Padding = New Padding(3)
        tbTOM.Size = New Size(512, 35)
        tbTOM.TabIndex = 3
        tbTOM.Text = "TOM"
        tbTOM.UseVisualStyleBackColor = True
        ' 
        ' tbNotes
        ' 
        tbNotes.Location = New Point(4, 22)
        tbNotes.Name = "tbNotes"
        tbNotes.Size = New Size(512, 35)
        tbNotes.TabIndex = 4
        tbNotes.Text = "Notes"
        tbNotes.UseVisualStyleBackColor = True
        ' 
        ' tbSourceControl
        ' 
        tbSourceControl.Controls.Add(cboApp)
        tbSourceControl.Controls.Add(btnGetLabel)
        tbSourceControl.Controls.Add(cboLabel)
        tbSourceControl.Controls.Add(btnAddApp)
        tbSourceControl.Controls.Add(btnCheckInLabel)
        tbSourceControl.Location = New Point(4, 22)
        tbSourceControl.Name = "tbSourceControl"
        tbSourceControl.Padding = New Padding(3)
        tbSourceControl.Size = New Size(512, 35)
        tbSourceControl.TabIndex = 2
        tbSourceControl.Text = "Source"
        tbSourceControl.UseVisualStyleBackColor = True
        ' 
        ' cboApp
        ' 
        cboApp.DropDownStyle = ComboBoxStyle.DropDownList
        cboApp.FormattingEnabled = True
        cboApp.Location = New Point(116, 5)
        cboApp.Name = "cboApp"
        cboApp.Size = New Size(90, 21)
        cboApp.TabIndex = 9
        ' 
        ' btnGetLabel
        ' 
        btnGetLabel.Location = New Point(66, 5)
        btnGetLabel.Name = "btnGetLabel"
        btnGetLabel.Size = New Size(44, 23)
        btnGetLabel.TabIndex = 8
        btnGetLabel.Text = "Get"
        btnGetLabel.UseVisualStyleBackColor = True
        ' 
        ' cboLabel
        ' 
        cboLabel.DropDownStyle = ComboBoxStyle.DropDownList
        cboLabel.FormattingEnabled = True
        cboLabel.Location = New Point(212, 5)
        cboLabel.Name = "cboLabel"
        cboLabel.Size = New Size(233, 21)
        cboLabel.TabIndex = 7
        ' 
        ' btnAddApp
        ' 
        btnAddApp.Location = New Point(0, 5)
        btnAddApp.Name = "btnAddApp"
        btnAddApp.Size = New Size(63, 23)
        btnAddApp.TabIndex = 6
        btnAddApp.Text = "Add App"
        btnAddApp.UseVisualStyleBackColor = True
        ' 
        ' btnCheckInLabel
        ' 
        btnCheckInLabel.Location = New Point(451, 5)
        btnCheckInLabel.Name = "btnCheckInLabel"
        btnCheckInLabel.Size = New Size(61, 23)
        btnCheckInLabel.TabIndex = 5
        btnCheckInLabel.Text = "Check In"
        btnCheckInLabel.UseVisualStyleBackColor = True
        ' 
        ' tbCode
        ' 
        tbCode.Location = New Point(4, 22)
        tbCode.Name = "tbCode"
        tbCode.Size = New Size(512, 35)
        tbCode.TabIndex = 8
        tbCode.Text = "Code"
        tbCode.UseVisualStyleBackColor = True
        ' 
        ' tbTime
        ' 
        tbTime.Controls.Add(cboDestinationTimeZone)
        tbTime.Controls.Add(txtConvertedTime)
        tbTime.Controls.Add(btnConvertTime)
        tbTime.Controls.Add(cboOriginTimeZone)
        tbTime.Controls.Add(dtUSTPicker)
        tbTime.Location = New Point(4, 22)
        tbTime.Name = "tbTime"
        tbTime.Size = New Size(512, 35)
        tbTime.TabIndex = 7
        tbTime.Text = "Time"
        tbTime.UseVisualStyleBackColor = True
        ' 
        ' cboDestinationTimeZone
        ' 
        cboDestinationTimeZone.DropDownStyle = ComboBoxStyle.DropDownList
        cboDestinationTimeZone.FormattingEnabled = True
        cboDestinationTimeZone.Items.AddRange(New Object() {"ET", "CT", "IST", "UTC"})
        cboDestinationTimeZone.Location = New Point(59, 7)
        cboDestinationTimeZone.Name = "cboDestinationTimeZone"
        cboDestinationTimeZone.Size = New Size(44, 21)
        cboDestinationTimeZone.TabIndex = 4
        ' 
        ' txtConvertedTime
        ' 
        txtConvertedTime.Location = New Point(301, 7)
        txtConvertedTime.Name = "txtConvertedTime"
        txtConvertedTime.Size = New Size(131, 22)
        txtConvertedTime.TabIndex = 3
        ' 
        ' btnConvertTime
        ' 
        btnConvertTime.Location = New Point(239, 8)
        btnConvertTime.Name = "btnConvertTime"
        btnConvertTime.Size = New Size(56, 20)
        btnConvertTime.TabIndex = 2
        btnConvertTime.Text = "Convert"
        btnConvertTime.UseVisualStyleBackColor = True
        ' 
        ' cboOriginTimeZone
        ' 
        cboOriginTimeZone.DropDownStyle = ComboBoxStyle.DropDownList
        cboOriginTimeZone.FormattingEnabled = True
        cboOriginTimeZone.Items.AddRange(New Object() {"ET", "CT", "IST", "UTC"})
        cboOriginTimeZone.Location = New Point(7, 7)
        cboOriginTimeZone.Name = "cboOriginTimeZone"
        cboOriginTimeZone.Size = New Size(44, 21)
        cboOriginTimeZone.TabIndex = 1
        ' 
        ' dtUSTPicker
        ' 
        dtUSTPicker.Format = DateTimePickerFormat.Time
        dtUSTPicker.Location = New Point(110, 6)
        dtUSTPicker.Name = "dtUSTPicker"
        dtUSTPicker.Size = New Size(123, 22)
        dtUSTPicker.TabIndex = 0
        ' 
        ' lblMessage
        ' 
        lblMessage.AutoSize = True
        lblMessage.Location = New Point(300, 9)
        lblMessage.Name = "lblMessage"
        lblMessage.Size = New Size(0, 15)
        lblMessage.TabIndex = 3
        ' 
        ' frmSwissArmyKnife
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.LightSteelBlue
        ClientSize = New Size(522, 70)
        Controls.Add(lblMessage)
        Controls.Add(ctlMain)
        FormBorderStyle = FormBorderStyle.None
        MaximizeBox = False
        Name = "frmSwissArmyKnife"
        StartPosition = FormStartPosition.Manual
        Text = "App Launcher"
        TopMost = True
        ctlMain.ResumeLayout(False)
        tbLauncher.ResumeLayout(False)
        tbSourceControl.ResumeLayout(False)
        tbTime.ResumeLayout(False)
        tbTime.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents ctlMain As TabControl
	Friend WithEvents tbLauncher As TabPage
	Friend WithEvents btnLaunch As Button
	Friend WithEvents cboApplicationSuite As ComboBox
	Friend WithEvents tbSourceControl As TabPage
	Friend WithEvents cboApp As ComboBox
	Friend WithEvents btnGetLabel As Button
	Friend WithEvents cboLabel As ComboBox
	Friend WithEvents btnAddApp As Button
	Friend WithEvents btnCheckInLabel As Button
	Friend WithEvents lblMessage As Label
    Friend WithEvents cboDeploymentLocation As ComboBox
    Friend WithEvents cboNavigation As ComboBox
    Friend WithEvents tbTOM As TabPage
    Friend WithEvents tbNotes As TabPage
    Friend WithEvents tbTime As TabPage
    Friend WithEvents dtUSTPicker As DateTimePicker
    Friend WithEvents btnConvertTime As Button
    Friend WithEvents cboOriginTimeZone As ComboBox
    Friend WithEvents txtConvertedTime As TextBox
    Friend WithEvents tbCode As TabPage
    Friend WithEvents cboDestinationTimeZone As ComboBox
    Friend WithEvents cboDeployApp As ComboBox

End Class
