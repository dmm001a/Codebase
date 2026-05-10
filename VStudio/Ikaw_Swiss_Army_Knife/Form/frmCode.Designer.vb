<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCode
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
        cboCodeCategory = New ComboBox()
        cboCodeSubCategory = New ComboBox()
        cboCodeTitle = New ComboBox()
        lblCodeCategory = New Label()
        lblCodeSubCategory = New Label()
        lblCodeTitle = New Label()
        rtCodeSnippet = New RichTextBox()
        btnSaveCode = New Button()
        btnAddCodeCategory = New Button()
        btnDeleteCode = New Button()
        SuspendLayout()
        ' 
        ' cboCodeCategory
        ' 
        cboCodeCategory.DropDownStyle = ComboBoxStyle.DropDownList
        cboCodeCategory.FormattingEnabled = True
        cboCodeCategory.Location = New Point(75, 10)
        cboCodeCategory.Name = "cboCodeCategory"
        cboCodeCategory.Size = New Size(103, 23)
        cboCodeCategory.TabIndex = 0
        ' 
        ' cboCodeSubCategory
        ' 
        cboCodeSubCategory.DropDownStyle = ComboBoxStyle.DropDownList
        cboCodeSubCategory.Enabled = False
        cboCodeSubCategory.FormattingEnabled = True
        cboCodeSubCategory.Location = New Point(295, 12)
        cboCodeSubCategory.Name = "cboCodeSubCategory"
        cboCodeSubCategory.Size = New Size(121, 23)
        cboCodeSubCategory.TabIndex = 1
        ' 
        ' cboCodeTitle
        ' 
        cboCodeTitle.Enabled = False
        cboCodeTitle.FormattingEnabled = True
        cboCodeTitle.Location = New Point(75, 45)
        cboCodeTitle.Name = "cboCodeTitle"
        cboCodeTitle.Size = New Size(341, 23)
        cboCodeTitle.TabIndex = 2
        ' 
        ' lblCodeCategory
        ' 
        lblCodeCategory.AutoSize = True
        lblCodeCategory.Location = New Point(12, 11)
        lblCodeCategory.Name = "lblCodeCategory"
        lblCodeCategory.Size = New Size(58, 15)
        lblCodeCategory.TabIndex = 3
        lblCodeCategory.Text = "Category:"
        ' 
        ' lblCodeSubCategory
        ' 
        lblCodeSubCategory.AutoSize = True
        lblCodeSubCategory.Location = New Point(206, 12)
        lblCodeSubCategory.Name = "lblCodeSubCategory"
        lblCodeSubCategory.Size = New Size(83, 15)
        lblCodeSubCategory.TabIndex = 4
        lblCodeSubCategory.Text = "Sub-Category:"
        ' 
        ' lblCodeTitle
        ' 
        lblCodeTitle.AutoSize = True
        lblCodeTitle.Location = New Point(12, 45)
        lblCodeTitle.Name = "lblCodeTitle"
        lblCodeTitle.Size = New Size(33, 15)
        lblCodeTitle.TabIndex = 5
        lblCodeTitle.Text = "Title:"
        ' 
        ' rtCodeSnippet
        ' 
        rtCodeSnippet.Location = New Point(12, 74)
        rtCodeSnippet.Name = "rtCodeSnippet"
        rtCodeSnippet.Size = New Size(404, 136)
        rtCodeSnippet.TabIndex = 6
        rtCodeSnippet.Text = ""
        ' 
        ' btnSaveCode
        ' 
        btnSaveCode.Enabled = False
        btnSaveCode.Location = New Point(341, 216)
        btnSaveCode.Name = "btnSaveCode"
        btnSaveCode.Size = New Size(75, 23)
        btnSaveCode.TabIndex = 7
        btnSaveCode.Text = "Save"
        btnSaveCode.UseVisualStyleBackColor = True
        ' 
        ' btnAddCodeCategory
        ' 
        btnAddCodeCategory.Location = New Point(180, 10)
        btnAddCodeCategory.Name = "btnAddCodeCategory"
        btnAddCodeCategory.Size = New Size(25, 23)
        btnAddCodeCategory.TabIndex = 8
        btnAddCodeCategory.Text = "+"
        btnAddCodeCategory.UseVisualStyleBackColor = True
        ' 
        ' btnDeleteCode
        ' 
        btnDeleteCode.Enabled = False
        btnDeleteCode.Location = New Point(12, 216)
        btnDeleteCode.Name = "btnDeleteCode"
        btnDeleteCode.Size = New Size(75, 23)
        btnDeleteCode.TabIndex = 9
        btnDeleteCode.Text = "Delete"
        btnDeleteCode.UseVisualStyleBackColor = True
        ' 
        ' frmCode
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Ivory
        ClientSize = New Size(424, 242)
        Controls.Add(btnDeleteCode)
        Controls.Add(btnAddCodeCategory)
        Controls.Add(btnSaveCode)
        Controls.Add(rtCodeSnippet)
        Controls.Add(lblCodeTitle)
        Controls.Add(lblCodeSubCategory)
        Controls.Add(lblCodeCategory)
        Controls.Add(cboCodeTitle)
        Controls.Add(cboCodeSubCategory)
        Controls.Add(cboCodeCategory)
        MaximizeBox = False
        Name = "frmCode"
        Text = "Code"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents cboCodeCategory As ComboBox
    Friend WithEvents cboCodeSubCategory As ComboBox
    Friend WithEvents cboCodeTitle As ComboBox
    Friend WithEvents lblCodeCategory As Label
    Friend WithEvents lblCodeSubCategory As Label
    Friend WithEvents lblCodeTitle As Label
    Friend WithEvents rtCodeSnippet As RichTextBox
    Friend WithEvents btnSaveCode As Button
    Friend WithEvents btnAddCodeCategory As Button
    Friend WithEvents btnDeleteCode As Button
End Class
