<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDBSync
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
        Me.cboApplication = New System.Windows.Forms.ComboBox()
        Me.cboEnvironment = New System.Windows.Forms.ComboBox()
        Me.lblSystem = New System.Windows.Forms.Label()
        Me.lblEnvironment = New System.Windows.Forms.Label()
        Me.btnCompare = New System.Windows.Forms.Button()
        Me.SuspendLayout
        '
        'cboApplication
        '
        Me.cboApplication.FormattingEnabled = true
        Me.cboApplication.Items.AddRange(New Object() {"DR", "Roadmap", "Ikawsoft Central"})
        Me.cboApplication.Location = New System.Drawing.Point(117, 40)
        Me.cboApplication.Name = "cboApplication"
        Me.cboApplication.Size = New System.Drawing.Size(164, 21)
        Me.cboApplication.TabIndex = 0
        '
        'cboEnvironment
        '
        Me.cboEnvironment.FormattingEnabled = true
        Me.cboEnvironment.Items.AddRange(New Object() {"Smoke", "QA", "Prod"})
        Me.cboEnvironment.Location = New System.Drawing.Point(118, 80)
        Me.cboEnvironment.Name = "cboEnvironment"
        Me.cboEnvironment.Size = New System.Drawing.Size(162, 21)
        Me.cboEnvironment.TabIndex = 1
        '
        'lblSystem
        '
        Me.lblSystem.AutoSize = true
        Me.lblSystem.Location = New System.Drawing.Point(17, 41)
        Me.lblSystem.Name = "lblSystem"
        Me.lblSystem.Size = New System.Drawing.Size(44, 13)
        Me.lblSystem.TabIndex = 2
        Me.lblSystem.Text = "System:"
        '
        'lblEnvironment
        '
        Me.lblEnvironment.AutoSize = true
        Me.lblEnvironment.Location = New System.Drawing.Point(16, 83)
        Me.lblEnvironment.Name = "lblEnvironment"
        Me.lblEnvironment.Size = New System.Drawing.Size(69, 13)
        Me.lblEnvironment.TabIndex = 3
        Me.lblEnvironment.Text = "Environment:"
        '
        'btnCompare
        '
        Me.btnCompare.Location = New System.Drawing.Point(209, 134)
        Me.btnCompare.Name = "btnCompare"
        Me.btnCompare.Size = New System.Drawing.Size(71, 24)
        Me.btnCompare.TabIndex = 4
        Me.btnCompare.Text = "Compare"
        Me.btnCompare.UseVisualStyleBackColor = true
        '
        'frmDBSync
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(366, 220)
        Me.Controls.Add(Me.btnCompare)
        Me.Controls.Add(Me.lblEnvironment)
        Me.Controls.Add(Me.lblSystem)
        Me.Controls.Add(Me.cboEnvironment)
        Me.Controls.Add(Me.cboApplication)
        Me.Name = "frmDBSync"
        Me.Text = "frmDBSync"
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

    Friend WithEvents cboApplication As ComboBox
    Friend WithEvents cboEnvironment As ComboBox
    Friend WithEvents lblSystem As Label
    Friend WithEvents lblEnvironment As Label
    Friend WithEvents btnCompare As Button
End Class
