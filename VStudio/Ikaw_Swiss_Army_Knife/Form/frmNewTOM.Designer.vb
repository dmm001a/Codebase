<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNewTOM
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
        cboParent = New ComboBox()
        txtTaskDescription = New TextBox()
        lblParent = New Label()
        lblTaskDescription = New Label()
        lblJiraID = New Label()
        txtJiraID = New TextBox()
        btnAddTask = New Button()
        cboTOMOwner = New ComboBox()
        lblTomOwner = New Label()
        SuspendLayout()
        ' 
        ' cboParent
        ' 
        cboParent.DropDownStyle = ComboBoxStyle.DropDownList
        cboParent.FormattingEnabled = True
        cboParent.Location = New Point(107, 44)
        cboParent.Name = "cboParent"
        cboParent.Size = New Size(186, 23)
        cboParent.TabIndex = 0
        ' 
        ' txtTaskDescription
        ' 
        txtTaskDescription.Location = New Point(107, 73)
        txtTaskDescription.MaxLength = 50
        txtTaskDescription.Name = "txtTaskDescription"
        txtTaskDescription.Size = New Size(186, 23)
        txtTaskDescription.TabIndex = 1
        ' 
        ' lblParent
        ' 
        lblParent.AutoSize = True
        lblParent.Location = New Point(9, 41)
        lblParent.Name = "lblParent"
        lblParent.Size = New Size(44, 15)
        lblParent.TabIndex = 2
        lblParent.Text = "Parent:"
        ' 
        ' lblTaskDescription
        ' 
        lblTaskDescription.AutoSize = True
        lblTaskDescription.Location = New Point(9, 76)
        lblTaskDescription.Name = "lblTaskDescription"
        lblTaskDescription.Size = New Size(70, 15)
        lblTaskDescription.TabIndex = 3
        lblTaskDescription.Text = "Description:"
        ' 
        ' lblJiraID
        ' 
        lblJiraID.AutoSize = True
        lblJiraID.Location = New Point(12, 113)
        lblJiraID.Name = "lblJiraID"
        lblJiraID.Size = New Size(41, 15)
        lblJiraID.TabIndex = 4
        lblJiraID.Text = "Jira ID:"
        ' 
        ' txtJiraID
        ' 
        txtJiraID.Location = New Point(107, 110)
        txtJiraID.MaxLength = 50
        txtJiraID.Name = "txtJiraID"
        txtJiraID.Size = New Size(186, 23)
        txtJiraID.TabIndex = 5
        ' 
        ' btnAddTask
        ' 
        btnAddTask.Location = New Point(232, 152)
        btnAddTask.Name = "btnAddTask"
        btnAddTask.Size = New Size(61, 20)
        btnAddTask.TabIndex = 6
        btnAddTask.Text = "Add"
        btnAddTask.UseVisualStyleBackColor = True
        ' 
        ' cboTOMOwner
        ' 
        cboTOMOwner.DropDownStyle = ComboBoxStyle.DropDownList
        cboTOMOwner.FormattingEnabled = True
        cboTOMOwner.Location = New Point(107, 12)
        cboTOMOwner.Name = "cboTOMOwner"
        cboTOMOwner.Size = New Size(186, 23)
        cboTOMOwner.TabIndex = 7
        ' 
        ' lblTomOwner
        ' 
        lblTomOwner.AutoSize = True
        lblTomOwner.Location = New Point(12, 12)
        lblTomOwner.Name = "lblTomOwner"
        lblTomOwner.Size = New Size(45, 15)
        lblTomOwner.TabIndex = 8
        lblTomOwner.Text = "Owner:"
        ' 
        ' frmNewTOM
        ' 
        AcceptButton = btnAddTask
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Ivory
        ClientSize = New Size(305, 184)
        Controls.Add(lblTomOwner)
        Controls.Add(cboTOMOwner)
        Controls.Add(btnAddTask)
        Controls.Add(txtJiraID)
        Controls.Add(lblJiraID)
        Controls.Add(lblTaskDescription)
        Controls.Add(lblParent)
        Controls.Add(txtTaskDescription)
        Controls.Add(cboParent)
        MaximizeBox = False
        MinimizeBox = False
        Name = "frmNewTOM"
        StartPosition = FormStartPosition.CenterScreen
        Text = "New Tom"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents cboParent As ComboBox
    Friend WithEvents txtTaskDescription As TextBox
    Friend WithEvents lblParent As Label
    Friend WithEvents lblTaskDescription As Label
    Friend WithEvents lblJiraID As Label
    Friend WithEvents txtJiraID As TextBox
    Friend WithEvents btnAddTask As Button
    Friend WithEvents cboTOMOwner As ComboBox
    Friend WithEvents lblTomOwner As Label
End Class
