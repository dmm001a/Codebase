<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPolling
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmPolling))
		cboInterval = New ComboBox()
		txtIntervalValue = New TextBox()
		btnActivate = New Button()
		SuspendLayout()
		' 
		' cboInterval
		' 
		cboInterval.DropDownStyle = ComboBoxStyle.DropDownList
		cboInterval.FormattingEnabled = True
		cboInterval.Items.AddRange(New Object() {"Seconds", "Minutes"})
		cboInterval.Location = New Point(96, 12)
		cboInterval.Name = "cboInterval"
		cboInterval.Size = New Size(121, 23)
		cboInterval.TabIndex = 0
		' 
		' txtIntervalValue
		' 
		txtIntervalValue.Location = New Point(14, 12)
		txtIntervalValue.Name = "txtIntervalValue"
		txtIntervalValue.Size = New Size(64, 23)
		txtIntervalValue.TabIndex = 1
		' 
		' btnActivate
		' 
		btnActivate.Location = New Point(115, 51)
		btnActivate.Name = "btnActivate"
		btnActivate.Size = New Size(100, 26)
		btnActivate.TabIndex = 2
		btnActivate.Text = "Activate"
		btnActivate.UseVisualStyleBackColor = True
		' 
		' frmPolling
		' 
		AutoScaleDimensions = New SizeF(7F, 15F)
		AutoScaleMode = AutoScaleMode.Font
		ClientSize = New Size(223, 89)
		Controls.Add(btnActivate)
		Controls.Add(txtIntervalValue)
		Controls.Add(cboInterval)
		Icon = CType(resources.GetObject("$this.Icon"), Icon)
		MaximizeBox = False
		MinimizeBox = False
		Name = "frmPolling"
		StartPosition = FormStartPosition.CenterScreen
		Text = "Polling Setup"
		ResumeLayout(False)
		PerformLayout()
	End Sub
	Friend WithEvents cboInterval As ComboBox
	Friend WithEvents txtIntervalValue As TextBox
	Friend WithEvents btnActivate As Button
End Class
