<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNewNote
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
        cboTopic = New ComboBox()
        txtNoteName = New TextBox()
        btnAdd = New Button()
        lblTopic = New Label()
        lblNoteName = New Label()
        SuspendLayout()
        ' 
        ' cboTopic
        ' 
        cboTopic.DropDownStyle = ComboBoxStyle.DropDownList
        cboTopic.FormattingEnabled = True
        cboTopic.Location = New Point(96, 31)
        cboTopic.Name = "cboTopic"
        cboTopic.Size = New Size(195, 23)
        cboTopic.TabIndex = 0
        ' 
        ' txtNoteName
        ' 
        txtNoteName.Location = New Point(96, 69)
        txtNoteName.Name = "txtNoteName"
        txtNoteName.Size = New Size(195, 23)
        txtNoteName.TabIndex = 1
        ' 
        ' btnAdd
        ' 
        btnAdd.Location = New Point(216, 110)
        btnAdd.Name = "btnAdd"
        btnAdd.Size = New Size(75, 23)
        btnAdd.TabIndex = 2
        btnAdd.Text = "Add"
        btnAdd.UseVisualStyleBackColor = True
        ' 
        ' lblTopic
        ' 
        lblTopic.AutoSize = True
        lblTopic.Location = New Point(12, 31)
        lblTopic.Name = "lblTopic"
        lblTopic.Size = New Size(39, 15)
        lblTopic.TabIndex = 3
        lblTopic.Text = "Topic:"
        ' 
        ' lblNoteName
        ' 
        lblNoteName.AutoSize = True
        lblNoteName.Location = New Point(12, 72)
        lblNoteName.Name = "lblNoteName"
        lblNoteName.Size = New Size(71, 15)
        lblNoteName.TabIndex = 4
        lblNoteName.Text = "Note Name:"
        ' 
        ' frmNewNote
        ' 
        AcceptButton = btnAdd
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Ivory
        ClientSize = New Size(301, 145)
        Controls.Add(lblNoteName)
        Controls.Add(lblTopic)
        Controls.Add(btnAdd)
        Controls.Add(txtNoteName)
        Controls.Add(cboTopic)
        MaximizeBox = False
        Name = "frmNewNote"
        StartPosition = FormStartPosition.CenterScreen
        Text = "Save Note"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents cboTopic As ComboBox
    Friend WithEvents txtNoteName As TextBox
    Friend WithEvents btnAdd As Button
    Friend WithEvents lblTopic As Label
    Friend WithEvents lblNoteName As Label
End Class
