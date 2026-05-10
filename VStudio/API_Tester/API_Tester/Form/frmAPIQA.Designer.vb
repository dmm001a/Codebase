<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmAPIQA
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmAPIQA))
		btnExecuteTest = New Button()
		txtExcelOutputFilePath = New TextBox()
		lblExcelOutputFilePath = New Label()
		lblEnvironment = New Label()
		cboEnvironment = New ComboBox()
		SuspendLayout()
		' 
		' btnExecuteTest
		' 
		btnExecuteTest.Location = New Point(711, 144)
		btnExecuteTest.Name = "btnExecuteTest"
		btnExecuteTest.Size = New Size(142, 27)
		btnExecuteTest.TabIndex = 0
		btnExecuteTest.Text = "Execute Test"
		btnExecuteTest.UseVisualStyleBackColor = True
		' 
		' txtExcelOutputFilePath
		' 
		txtExcelOutputFilePath.Location = New Point(125, 105)
		txtExcelOutputFilePath.Name = "txtExcelOutputFilePath"
		txtExcelOutputFilePath.ReadOnly = True
		txtExcelOutputFilePath.Size = New Size(728, 23)
		txtExcelOutputFilePath.TabIndex = 11
		' 
		' lblExcelOutputFilePath
		' 
		lblExcelOutputFilePath.AutoSize = True
		lblExcelOutputFilePath.Location = New Point(44, 108)
		lblExcelOutputFilePath.Name = "lblExcelOutputFilePath"
		lblExcelOutputFilePath.Size = New Size(75, 15)
		lblExcelOutputFilePath.TabIndex = 12
		lblExcelOutputFilePath.Text = "Output Path:"
		' 
		' lblEnvironment
		' 
		lblEnvironment.AutoSize = True
		lblEnvironment.Location = New Point(41, 24)
		lblEnvironment.Name = "lblEnvironment"
		lblEnvironment.Size = New Size(78, 15)
		lblEnvironment.TabIndex = 13
		lblEnvironment.Text = "Environment:"
		' 
		' cboEnvironment
		' 
		cboEnvironment.DropDownStyle = ComboBoxStyle.DropDownList
		cboEnvironment.FormattingEnabled = True
		cboEnvironment.Items.AddRange(New Object() {"QA", "Stage"})
		cboEnvironment.Location = New Point(125, 21)
		cboEnvironment.Name = "cboEnvironment"
		cboEnvironment.Size = New Size(145, 23)
		cboEnvironment.TabIndex = 14
		' 
		' frmAPIQA
		' 
		AutoScaleDimensions = New SizeF(7F, 15F)
		AutoScaleMode = AutoScaleMode.Font
		ClientSize = New Size(881, 183)
		Controls.Add(cboEnvironment)
		Controls.Add(lblEnvironment)
		Controls.Add(lblExcelOutputFilePath)
		Controls.Add(txtExcelOutputFilePath)
		Controls.Add(btnExecuteTest)
		Icon = CType(resources.GetObject("$this.Icon"), Icon)
		MaximizeBox = False
		Name = "frmAPIQA"
		StartPosition = FormStartPosition.CenterScreen
		Text = "API Quality Assurance"
		ResumeLayout(False)
		PerformLayout()
	End Sub

	Friend WithEvents btnExecuteTest As Button
	Friend WithEvents txtExcelOutputFilePath As TextBox
	Friend WithEvents lblExcelOutputFilePath As Label
	Friend WithEvents lblEnvironment As Label
	Friend WithEvents cboEnvironment As ComboBox
End Class
