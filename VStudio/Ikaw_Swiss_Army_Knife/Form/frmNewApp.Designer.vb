<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNewApp
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
        txtApplicationName = New TextBox()
        btnAddApplication = New Button()
        lblApplicationName = New Label()
        SuspendLayout()
        ' 
        ' txtApplicationName
        ' 
        txtApplicationName.Location = New Point(126, 15)
        txtApplicationName.Name = "txtApplicationName"
        txtApplicationName.Size = New Size(217, 23)
        txtApplicationName.TabIndex = 0
        ' 
        ' btnAddApplication
        ' 
        btnAddApplication.Location = New Point(268, 62)
        btnAddApplication.Name = "btnAddApplication"
        btnAddApplication.Size = New Size(75, 23)
        btnAddApplication.TabIndex = 1
        btnAddApplication.Text = "Add"
        btnAddApplication.UseVisualStyleBackColor = True
        ' 
        ' lblApplicationName
        ' 
        lblApplicationName.AutoSize = True
        lblApplicationName.Location = New Point(12, 18)
        lblApplicationName.Name = "lblApplicationName"
        lblApplicationName.Size = New Size(106, 15)
        lblApplicationName.TabIndex = 2
        lblApplicationName.Text = "Application Name:"
        ' 
        ' frmNewApp
        ' 
        AcceptButton = btnAddApplication
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Ivory
        ClientSize = New Size(355, 95)
        Controls.Add(lblApplicationName)
        Controls.Add(btnAddApplication)
        Controls.Add(txtApplicationName)
        MaximizeBox = False
        Name = "frmNewApp"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Add New Application"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtApplicationName As TextBox
	Friend WithEvents btnAddApplication As Button
	Friend WithEvents lblApplicationName As Label
End Class
