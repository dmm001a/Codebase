Imports System.IO
Imports System.Runtime.InteropServices.JavaScript.JSType
Imports System.Security.Cryptography.X509Certificates
Imports Azure
Imports Microsoft.Data
Imports Microsoft.Data.SqlClient
Imports Microsoft.Web.WebView2.Core
Imports Newtonsoft.Json

Public Class frmNote

    Private Async Sub frmNote_Load(sender As Object, e As EventArgs) Handles Me.Load

        Try
            AddHandler wvNotes.WebMessageReceived, AddressOf EditorMessageReceived

            Dim xLeft As Integer
            Dim yTop As Integer

            'Form Positioning
            Me.StartPosition = FormStartPosition.Manual

            xLeft = Screen.PrimaryScreen.WorkingArea.Right - Me.Width
            yTop = Screen.PrimaryScreen.WorkingArea.Top + (Screen.PrimaryScreen.WorkingArea.Height - Me.Height) \ 2

            Me.Location = New Point(xLeft, yTop)
            '---------------------

            'Control Population
            mPopulateTopicComboBox(Me.cboNoteTopic, "None")


            Dim env = Await CoreWebView2Environment.CreateAsync(
                Nothing,
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                             "IkawSwissArmyKnife", "WebView2Data")
            )

            Await Me.wvNotes.EnsureCoreWebView2Async(env)
            Me.wvNotes.Source = New Uri(sCKEditorHTMLPath)




        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub


    Public Function mResetNote() As Integer

        Try


            'Dim html As String = File.ReadAllText(sCKEditorHTMLPath)
            Dim nowStamp As String = DateTime.Now.ToString("MM-dd-yyyy hh:mm tt")
            Dim html As String = "Meeting Notes - " & nowStamp

            Dim js = $"setEditorData({JsonConvert.SerializeObject(html)})"
            wvNotes.ExecuteScriptAsync(js)

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Function

    Private Async Sub EditorMessageReceived(
        sender As Object,
        e As Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs)

        Try

            Dim msg As String = e.WebMessageAsJson

            If msg IsNot Nothing AndAlso msg.Contains("""type"":""ready""") Then
                Dim nowStamp As String = DateTime.Now.ToString("MM-dd-yyyy hh:mm tt")

                Dim safe = nowStamp _
                    .Replace("\", "\\") _
                    .Replace("""", "\""") _
                    .Replace(vbCrLf, "\n")

                Dim script = $"window.setEditorData(""Meeting Notes - {safe}"");"
                Await wvNotes.ExecuteScriptAsync(script)
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try

    End Sub

    Private async Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try

            Dim sNoteName As String
            Dim sNoteText As String
            Dim sSQL As String
            Dim iNoteID As Integer
            Dim iRecordsAffected As Integer

            Dim oUtilityClass As UtilityClass.cADONETDatabase

            If Me.cboNoteName.SelectedIndex > 0 Then

                iNoteID = Me.cboNoteName.SelectedValue
                sNoteName = Me.cboNoteName.Text

                sNoteText = Await wvNotes.ExecuteScriptAsync("window.getEditorData();")
                sNoteText = System.Text.Json.JsonSerializer.Deserialize(Of String)(sNoteText)
                'sNoteText = Await Me.wvNotes.ExecuteScriptAsync("quill.root.innerHTML;")
                'sNoteText = System.Text.Json.JsonSerializer.Deserialize(Of String)(sNoteText)            
                sNoteText = "'" & Replace(sNoteText, "'", "''") & "'"

                oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)

                sSQL = "UPDATE tb_Note SET Note_Text = " & sNoteText
                sSQL = sSQL & " WHERE Note_ID = " & iNoteID

                oUtilityClass.OpenConnection()

                iRecordsAffected = oUtilityClass.ExecuteSQL(sSQL)

                Me.Text = sNoteName & " Record Saved."

                oUtilityClass.CloseConnection()

                oUtilityClass = Nothing

            Else If Me.cboNoteName.SelectedIndex <= 0 Then
                frmNewNote.Show
            End If


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub


    Public async Sub mPopulateNote(ByVal bStitch As Boolean, ByVal iTopicID As Integer, ByVal iNoteID As Integer)

        Try


            Dim sSQL As String
            Dim sOutputString As String
            Dim sSafeHTML As String
            Dim sJSOutput As String
            Dim oUtilityClass As UtilityClass.cADONETDatabase
            Dim oSQLDataReader As SqlDataReader

            If bStitch = True AND iTopicID > 0 Then
                sSQL = "SELECT * FROM tb_Note WHERE Topic_ID = " & iTopicID
                sSQL = sSQL & " ORDER BY Note_ID"
            ElseIf bStitch = False AND iNoteID > -1
                sSQL = "SELECT * FROM tb_Note WHERE Note_ID = " & iNoteID
            End If

            If sSQL IsNot Nothing AndAlso sSQL.Length > 0 Then
                oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)
                oUtilityClass.OpenConnection()

                oSQLDataReader = oUtilityClass.GetDataReader(sSQL)

                While oSQLDataReader IsNot Nothing AndAlso oSQLDataReader.Read()
                    sOutputString = sOutputString & oSQLDataReader("Note_Text").ToString() & "<br><br>"
                End While

                If sOutputString IsNot Nothing Then
                    sSafeHTML = sOutputString _
                        .Replace("\", "\\") _
                        .Replace("""", "\""") _
                        .Replace(vbCrLf, "\n")

                    sSafeHTML = $"window.setEditorData(""{sSafeHTML}"");"
                    Await Me.wvNotes.ExecuteScriptAsync(sSafeHTML)

                    'sSafeHTML = sOutputString.Replace("'", "\'").Replace(vbCrLf, "")
                    'sJSOutput = $"quill.root.innerHTML = '{sSafeHTML}';"
                    'Await Me.wvNotes.ExecuteScriptAsync(sJSOutput)
                End If

                oSQLDataReader.Close()
                oUtilityClass.CloseConnection()
            End If

            oSQLDataReader = Nothing
            oUtilityClass = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub



    Public Sub mPopulateNoteNameComboBox(oNoteNameComboBox As System.Windows.Forms.ComboBox, iTopicID As Integer,
        Optional sDefaultValueText As String = "")
        Try

            Dim sSQL As String
            Dim sIDFieldName As String
            Dim sDescriptionFieldName As String
            Dim oLabelManagement As cLabelManagement

            sSQL = "SELECT Note_ID, "
            sSQL = sSQL & "Note_Name + ' ' + FORMAT(Entered_Date, 'MM/dd/yyyy hh:mm tt') AS Note_Name "
            sSQL = sSQL & "FROM tb_Note WHERE Active = 1 "
            sSQL = sSQL & "AND Topic_ID = " & iTopicID
            sSQL = sSQL & "ORDER BY Note_Name"

            sIDFieldName = "Note_ID"
            sDescriptionFieldName = "Note_Name"

            oLabelManagement = New cLabelManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

            oLabelManagement.mPopulateComboBox(oNoteNameComboBox, sIDFieldName, sDescriptionFieldName, sSQL, True, sDefaultValueText)

            oLabelManagement = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private async Sub cboTopic_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboNoteTopic.SelectedIndexChanged
        Try

            If mIsInitialComboBoxLoad(Me.cboNoteTopic) = False Then
                mPopulateNoteNameComboBox(Me.cboNoteName, Me.cboNoteTopic.SelectedValue)
                mResetNote()
            End If

            If Me.cboNoteTopic.SelectedIndex = 0 Then
                mResetNote()
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboNoteName_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboNoteName.SelectedIndexChanged
        Try

            If mIsInitialComboBoxLoad(Me.cboNoteName) = False Then
                mPopulateNote(Me.chkStitch.Checked, Me.cboNoteTopic.SelectedValue, Me.cboNoteName.SelectedValue)

            End If


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub btnAddNoteTopic_Click(sender As Object, e As EventArgs) Handles btnAddNoteTopic.Click
        Try

            frmNewNoteTopic.ShowDialog()

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub



    Private Sub btnDeleteNote_Click(sender As Object, e As EventArgs) Handles btnDeleteNote.Click
        Try

            Dim sSQL As String
            Dim iNoteID As Integer
            Dim dlgConfirmDelete As DialogResult
            Dim oDatabase As UtilityClass.cADONETDatabase

            iNoteID = Me.cboNoteName.SelectedValue

            If iNoteID Then
                dlgConfirmDelete = MessageBox.Show("Delete this record?",
                                           "Confirm Deletion",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Warning,
                                           MessageBoxDefaultButton.Button2)

                If dlgConfirmDelete = DialogResult.Yes Then
                    sSQL = "DELETE FROM tb_Note WHERE Note_ID = " & iNoteID

                    oDatabase = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)

                    oDatabase.OpenConnection
                    oDatabase.ExecuteSQL(sSQL)
                    oDatabase.CloseConnection

                    Dim htmlPath As String = "C:\ProgramData\IkawSoft\ckeditor\ckeditor.html"
                    Me.wvNotes.Source = New Uri(htmlPath)

                    Me.cboNoteName.Tag = ""
                    mPopulateNoteNameComboBox(Me.cboNoteName, Me.cboNoteTopic.SelectedValue, "")
                    Me.cboNoteName.Tag = "1"
                End If

            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub chkStitch_CheckedChanged(sender As Object, e As EventArgs) Handles chkStitch.CheckedChanged
        Try
        
        If Me.chkstitch.Checked = True Then
            mPopulateNote(Me.chkStitch.Checked, Me.cboNoteTopic.SelectedValue, Me.cboNoteName.SelectedValue)
        End If


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub
End Class