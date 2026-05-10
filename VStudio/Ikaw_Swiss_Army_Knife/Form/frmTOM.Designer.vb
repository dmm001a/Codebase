<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTOM
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
        pnlTom = New Panel()
        btnNew = New Button()
        cboTOMOwner = New ComboBox()
        SuspendLayout()
        ' 
        ' pnlTom
        ' 
        pnlTom.AutoScroll = True
        pnlTom.Location = New Point(25, 49)
        pnlTom.Name = "pnlTom"
        pnlTom.Size = New Size(423, 342)
        pnlTom.TabIndex = 1
        ' 
        ' btnNew
        ' 
        btnNew.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnNew.Location = New Point(413, 397)
        btnNew.Name = "btnNew"
        btnNew.Size = New Size(35, 31)
        btnNew.TabIndex = 3
        btnNew.Text = "+"
        btnNew.UseVisualStyleBackColor = True
        ' 
        ' cboTOMOwner
        ' 
        cboTOMOwner.DropDownStyle = ComboBoxStyle.DropDownList
        cboTOMOwner.FormattingEnabled = True
        cboTOMOwner.Location = New Point(25, 20)
        cboTOMOwner.Name = "cboTOMOwner"
        cboTOMOwner.Size = New Size(126, 23)
        cboTOMOwner.TabIndex = 4
        ' 
        ' frmTOM
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Ivory
        ClientSize = New Size(460, 440)
        ControlBox = False
        Controls.Add(cboTOMOwner)
        Controls.Add(btnNew)
        Controls.Add(pnlTom)
        FormBorderStyle = FormBorderStyle.None
        Name = "frmTOM"
        ShowIcon = False
        ShowInTaskbar = False
        StartPosition = FormStartPosition.Manual
        Text = "Top Of Mind"
        ResumeLayout(False)
    End Sub
    Friend WithEvents pnlTom As Panel
    Friend WithEvents btnNew As Button
    Friend WithEvents cboTOMOwner As ComboBox
End Class
