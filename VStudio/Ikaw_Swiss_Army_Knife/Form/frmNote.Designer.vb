<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNote
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
        wvNotes = New Microsoft.Web.WebView2.WinForms.WebView2()
        btnSave = New Button()
        cboNoteTopic = New ComboBox()
        lblTopic = New Label()
        cboNoteName = New ComboBox()
        lblRetrieveNote = New Label()
        chkStitch = New CheckBox()
        btnAddNoteTopic = New Button()
        btnDeleteNote = New Button()
        CType(wvNotes, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' wvNotes
        ' 
        wvNotes.AllowExternalDrop = True
        wvNotes.CreationProperties = Nothing
        wvNotes.DefaultBackgroundColor = Color.White
        wvNotes.Location = New Point(3, 34)
        wvNotes.Name = "wvNotes"
        wvNotes.Size = New Size(601, 692)
        wvNotes.TabIndex = 0
        wvNotes.ZoomFactor = 1R
        ' 
        ' btnSave
        ' 
        btnSave.Location = New Point(521, 734)
        btnSave.Name = "btnSave"
        btnSave.Size = New Size(82, 28)
        btnSave.TabIndex = 1
        btnSave.Text = "Save"
        btnSave.UseVisualStyleBackColor = True
        ' 
        ' cboNoteTopic
        ' 
        cboNoteTopic.DropDownStyle = ComboBoxStyle.DropDownList
        cboNoteTopic.FormattingEnabled = True
        cboNoteTopic.Location = New Point(71, 6)
        cboNoteTopic.Name = "cboNoteTopic"
        cboNoteTopic.Size = New Size(133, 23)
        cboNoteTopic.TabIndex = 2
        ' 
        ' lblTopic
        ' 
        lblTopic.AutoSize = True
        lblTopic.Location = New Point(28, 9)
        lblTopic.Name = "lblTopic"
        lblTopic.Size = New Size(39, 15)
        lblTopic.TabIndex = 3
        lblTopic.Text = "Topic:"
        ' 
        ' cboNoteName
        ' 
        cboNoteName.DropDownStyle = ComboBoxStyle.DropDownList
        cboNoteName.FormattingEnabled = True
        cboNoteName.Location = New Point(305, 8)
        cboNoteName.Name = "cboNoteName"
        cboNoteName.Size = New Size(299, 23)
        cboNoteName.TabIndex = 4
        ' 
        ' lblRetrieveNote
        ' 
        lblRetrieveNote.AutoSize = True
        lblRetrieveNote.Location = New Point(266, 11)
        lblRetrieveNote.Name = "lblRetrieveNote"
        lblRetrieveNote.Size = New Size(36, 15)
        lblRetrieveNote.TabIndex = 5
        lblRetrieveNote.Text = "Note:"
        ' 
        ' chkStitch
        ' 
        chkStitch.AutoSize = True
        chkStitch.Location = New Point(210, 8)
        chkStitch.Name = "chkStitch"
        chkStitch.Size = New Size(56, 19)
        chkStitch.TabIndex = 6
        chkStitch.Text = "Stitch"
        chkStitch.UseVisualStyleBackColor = True
        ' 
        ' btnAddNoteTopic
        ' 
        btnAddNoteTopic.Location = New Point(3, 6)
        btnAddNoteTopic.Name = "btnAddNoteTopic"
        btnAddNoteTopic.Size = New Size(25, 23)
        btnAddNoteTopic.TabIndex = 7
        btnAddNoteTopic.Text = "+"
        btnAddNoteTopic.UseVisualStyleBackColor = True
        ' 
        ' btnDeleteNote
        ' 
        btnDeleteNote.Location = New Point(6, 732)
        btnDeleteNote.Name = "btnDeleteNote"
        btnDeleteNote.Size = New Size(82, 28)
        btnDeleteNote.TabIndex = 8
        btnDeleteNote.Text = "Delete"
        btnDeleteNote.UseVisualStyleBackColor = True
        ' 
        ' frmNote
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = Color.Ivory
        ClientSize = New Size(609, 769)
        Controls.Add(btnDeleteNote)
        Controls.Add(btnAddNoteTopic)
        Controls.Add(chkStitch)
        Controls.Add(lblRetrieveNote)
        Controls.Add(cboNoteName)
        Controls.Add(lblTopic)
        Controls.Add(cboNoteTopic)
        Controls.Add(btnSave)
        Controls.Add(wvNotes)
        Name = "frmNote"
        StartPosition = FormStartPosition.Manual
        Text = "Notes"
        CType(wvNotes, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents wvNotes As Microsoft.Web.WebView2.WinForms.WebView2
    Friend WithEvents btnSave As Button
    Friend WithEvents cboNoteTopic As ComboBox
    Friend WithEvents lblTopic As Label
    Friend WithEvents cboNoteName As ComboBox
    Friend WithEvents lblRetrieveNote As Label
    Friend WithEvents chkStitch As CheckBox
    Friend WithEvents btnAddNoteTopic As Button
    Friend WithEvents btnDeleteNote As Button
End Class
