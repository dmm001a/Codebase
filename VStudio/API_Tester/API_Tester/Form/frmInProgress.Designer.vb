<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmInProgress
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmInProgress))
		lblPleaseWait = New Label()
		SuspendLayout()
		' 
		' lblPleaseWait
		' 
		lblPleaseWait.AutoSize = True
		lblPleaseWait.Font = New Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
		lblPleaseWait.Location = New Point(6, 5)
		lblPleaseWait.Name = "lblPleaseWait"
		lblPleaseWait.Size = New Size(410, 25)
		lblPleaseWait.TabIndex = 0
		lblPleaseWait.Text = "This process will take some time.  Please wait."
		' 
		' frmInProgress
		' 
		AutoScaleDimensions = New SizeF(7F, 15F)
		AutoScaleMode = AutoScaleMode.Font
		ClientSize = New Size(422, 40)
		Controls.Add(lblPleaseWait)
		ForeColor = SystemColors.HotTrack
		Icon = CType(resources.GetObject("$this.Icon"), Icon)
		MaximizeBox = False
		MinimizeBox = False
		Name = "frmInProgress"
		StartPosition = FormStartPosition.CenterScreen
		Text = "In Progress"
		ResumeLayout(False)
		PerformLayout()
	End Sub

	Friend WithEvents lblPleaseWait As Label
End Class
