<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNewCodeCategory
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
        cboCodeCategoryType = New ComboBox()
        lblCodeCategoryType = New Label()
        txtCodeCategoryName = New TextBox()
        lblCodeCategoryName = New Label()
        btnCodeCategoryAdd = New Button()
        cboCategory = New ComboBox()
        lblCategoryID = New Label()
        SuspendLayout()
        ' 
        ' cboCodeCategoryType
        ' 
        cboCodeCategoryType.DropDownStyle = ComboBoxStyle.DropDownList
        cboCodeCategoryType.FormattingEnabled = True
        cboCodeCategoryType.Items.AddRange(New Object() {"Category", "Sub-Category"})
        cboCodeCategoryType.Location = New Point(128, 19)
        cboCodeCategoryType.Name = "cboCodeCategoryType"
        cboCodeCategoryType.Size = New Size(195, 23)
        cboCodeCategoryType.TabIndex = 0
        ' 
        ' lblCodeCategoryType
        ' 
        lblCodeCategoryType.AutoSize = True
        lblCodeCategoryType.Location = New Point(12, 19)
        lblCodeCategoryType.Name = "lblCodeCategoryType"
        lblCodeCategoryType.Size = New Size(86, 15)
        lblCodeCategoryType.TabIndex = 1
        lblCodeCategoryType.Text = "Category Type:"
        ' 
        ' txtCodeCategoryName
        ' 
        txtCodeCategoryName.Location = New Point(128, 90)
        txtCodeCategoryName.Name = "txtCodeCategoryName"
        txtCodeCategoryName.Size = New Size(195, 23)
        txtCodeCategoryName.TabIndex = 2
        ' 
        ' lblCodeCategoryName
        ' 
        lblCodeCategoryName.AutoSize = True
        lblCodeCategoryName.Location = New Point(12, 98)
        lblCodeCategoryName.Name = "lblCodeCategoryName"
        lblCodeCategoryName.Size = New Size(93, 15)
        lblCodeCategoryName.TabIndex = 3
        lblCodeCategoryName.Text = "Category Name:"
        ' 
        ' btnCodeCategoryAdd
        ' 
        btnCodeCategoryAdd.Location = New Point(248, 134)
        btnCodeCategoryAdd.Name = "btnCodeCategoryAdd"
        btnCodeCategoryAdd.Size = New Size(75, 23)
        btnCodeCategoryAdd.TabIndex = 4
        btnCodeCategoryAdd.Text = "Add"
        btnCodeCategoryAdd.UseVisualStyleBackColor = True
        ' 
        ' cboCategory
        ' 
        cboCategory.DropDownStyle = ComboBoxStyle.DropDownList
        cboCategory.FormattingEnabled = True
        cboCategory.Location = New Point(129, 57)
        cboCategory.Name = "cboCategory"
        cboCategory.Size = New Size(194, 23)
        cboCategory.TabIndex = 5
        cboCategory.Visible = False
        ' 
        ' lblCategoryID
        ' 
        lblCategoryID.AutoSize = True
        lblCategoryID.Location = New Point(12, 60)
        lblCategoryID.Name = "lblCategoryID"
        lblCategoryID.Size = New Size(72, 15)
        lblCategoryID.TabIndex = 6
        lblCategoryID.Text = "Category ID:"
        lblCategoryID.Visible = False
        ' 
        ' frmNewCodeCategory
        ' 
        AcceptButton = btnCodeCategoryAdd
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Ivory
        ClientSize = New Size(335, 177)
        Controls.Add(lblCategoryID)
        Controls.Add(cboCategory)
        Controls.Add(btnCodeCategoryAdd)
        Controls.Add(lblCodeCategoryName)
        Controls.Add(txtCodeCategoryName)
        Controls.Add(lblCodeCategoryType)
        Controls.Add(cboCodeCategoryType)
        MaximizeBox = False
        Name = "frmNewCodeCategory"
        StartPosition = FormStartPosition.CenterScreen
        Text = "New Code Category"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents cboCodeCategoryType As ComboBox
    Friend WithEvents lblCodeCategoryType As Label
    Friend WithEvents txtCodeCategoryName As TextBox
    Friend WithEvents lblCodeCategoryName As Label
    Friend WithEvents btnCodeCategoryAdd As Button
    Friend WithEvents cboCategory As ComboBox
    Friend WithEvents lblCategoryID As Label
End Class
