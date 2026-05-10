<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSetPath
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSetPath))
		txtPath = New TextBox()
		cmdSave = New Button()
		SuspendLayout()
		' 
		' txtPath
		' 
		txtPath.Location = New Point(13, 12)
		txtPath.Name = "txtPath"
		txtPath.Size = New Size(400, 23)
		txtPath.TabIndex = 0
		' 
		' cmdSave
		' 
		cmdSave.Location = New Point(365, 41)
		cmdSave.Name = "cmdSave"
		cmdSave.Size = New Size(45, 20)
		cmdSave.TabIndex = 1
		cmdSave.Text = "Save"
		cmdSave.UseVisualStyleBackColor = True
		' 
		' frmSetPath
		' 
		AutoScaleDimensions = New SizeF(7F, 15F)
		AutoScaleMode = AutoScaleMode.Font
		ClientSize = New Size(422, 71)
		Controls.Add(cmdSave)
		Controls.Add(txtPath)
		Icon = CType(resources.GetObject("$this.Icon"), Icon)
		MaximizeBox = False
		MinimizeBox = False
		Name = "frmSetPath"
		StartPosition = FormStartPosition.CenterScreen
		Text = "frmSetPath"
		ResumeLayout(False)
		PerformLayout()
	End Sub

	Friend WithEvents txtPath As TextBox
    Friend WithEvents cmdSave As Button
End Class
