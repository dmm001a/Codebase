<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNewNoteTopic
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
        txtTopicName = New TextBox()
        lblTopicName = New Label()
        btnAddTopic = New Button()
        SuspendLayout()
        ' 
        ' txtTopicName
        ' 
        txtTopicName.Location = New Point(57, 28)
        txtTopicName.Name = "txtTopicName"
        txtTopicName.Size = New Size(228, 23)
        txtTopicName.TabIndex = 0
        ' 
        ' lblTopicName
        ' 
        lblTopicName.AutoSize = True
        lblTopicName.Location = New Point(7, 31)
        lblTopicName.Name = "lblTopicName"
        lblTopicName.Size = New Size(39, 15)
        lblTopicName.TabIndex = 1
        lblTopicName.Text = "Topic:"
        ' 
        ' btnAddTopic
        ' 
        btnAddTopic.Location = New Point(210, 69)
        btnAddTopic.Name = "btnAddTopic"
        btnAddTopic.Size = New Size(75, 23)
        btnAddTopic.TabIndex = 2
        btnAddTopic.Text = "Add"
        btnAddTopic.UseVisualStyleBackColor = True
        ' 
        ' frmNewNoteTopic
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Ivory
        ClientSize = New Size(297, 110)
        Controls.Add(btnAddTopic)
        Controls.Add(lblTopicName)
        Controls.Add(txtTopicName)
        MaximizeBox = False
        Name = "frmNewNoteTopic"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Add Note Topic"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents txtTopicName As TextBox
    Friend WithEvents lblTopicName As Label
    Friend WithEvents btnAddTopic As Button
End Class
