<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmViewRequestResponse
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmViewRequestResponse))
		TxtRequest = New TextBox()
		txtResponse = New TextBox()
		lblRequest = New Label()
		lblAPIName = New Label()
		SuspendLayout()
		' 
		' TxtRequest
		' 
		TxtRequest.BackColor = Color.White
		TxtRequest.Location = New Point(15, 71)
		TxtRequest.Multiline = True
		TxtRequest.Name = "TxtRequest"
		TxtRequest.ReadOnly = True
		TxtRequest.ScrollBars = ScrollBars.Both
		TxtRequest.Size = New Size(384, 511)
		TxtRequest.TabIndex = 0
		' 
		' txtResponse
		' 
		txtResponse.BackColor = Color.White
		txtResponse.Location = New Point(453, 71)
		txtResponse.Multiline = True
		txtResponse.Name = "txtResponse"
		txtResponse.ReadOnly = True
		txtResponse.ScrollBars = ScrollBars.Both
		txtResponse.Size = New Size(384, 511)
		txtResponse.TabIndex = 1
		' 
		' lblRequest
		' 
		lblRequest.AutoSize = True
		lblRequest.Font = New Font("Segoe UI", 14F, FontStyle.Bold)
		lblRequest.ForeColor = SystemColors.HotTrack
		lblRequest.Location = New Point(169, 42)
		lblRequest.Name = "lblRequest"
		lblRequest.Size = New Size(83, 25)
		lblRequest.TabIndex = 6
		lblRequest.Text = "Request"
		' 
		' lblAPIName
		' 
		lblAPIName.AutoSize = True
		lblAPIName.Font = New Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
		lblAPIName.ForeColor = SystemColors.Highlight
		lblAPIName.Location = New Point(5, 4)
		lblAPIName.Name = "lblAPIName"
		lblAPIName.Size = New Size(0, 45)
		lblAPIName.TabIndex = 8
		' 
		' frmViewRequestResponse
		' 
		AutoScaleDimensions = New SizeF(7F, 15F)
		AutoScaleMode = AutoScaleMode.Font
		ClientSize = New Size(868, 594)
		Controls.Add(lblAPIName)
		Controls.Add(lblRequest)
		Controls.Add(txtResponse)
		Controls.Add(TxtRequest)
		Icon = CType(resources.GetObject("$this.Icon"), Icon)
		Name = "frmViewRequestResponse"
		StartPosition = FormStartPosition.CenterScreen
		Text = "View Request Response"
		ResumeLayout(False)
		PerformLayout()
	End Sub

	Friend WithEvents TxtRequest As TextBox
    Friend WithEvents txtResponse As TextBox
    Friend WithEvents lblRequest As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents lblAPIName As Label
End Class
