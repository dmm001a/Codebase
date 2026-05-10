<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmDeploymentVersion
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
        cboAppVersion = New ComboBox()
        btnDeploy = New Button()
        SuspendLayout()
        ' 
        ' cboAppVersion
        ' 
        cboAppVersion.DropDownStyle = ComboBoxStyle.DropDownList
        cboAppVersion.FormattingEnabled = True
        cboAppVersion.Location = New Point(12, 12)
        cboAppVersion.Name = "cboAppVersion"
        cboAppVersion.Size = New Size(296, 23)
        cboAppVersion.TabIndex = 0
        ' 
        ' btnDeploy
        ' 
        btnDeploy.Location = New Point(200, 50)
        btnDeploy.Name = "btnDeploy"
        btnDeploy.Size = New Size(107, 22)
        btnDeploy.TabIndex = 1
        btnDeploy.Text = "Deploy"
        btnDeploy.UseVisualStyleBackColor = True
        ' 
        ' frmDeploymentVersion
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(320, 82)
        Controls.Add(btnDeploy)
        Controls.Add(cboAppVersion)
        MaximizeBox = False
        Name = "frmDeploymentVersion"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Deployment Version"
        ResumeLayout(False)
    End Sub

    Friend WithEvents cboAppVersion As ComboBox
    Friend WithEvents btnDeploy As Button
End Class
