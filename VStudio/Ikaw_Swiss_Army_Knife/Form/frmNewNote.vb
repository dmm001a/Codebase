Imports System.Runtime.InteropServices.JavaScript.JSType

Public Class frmNewNote
    Private Sub cboTopic_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTopic.SelectedIndexChanged

    End Sub

    Private Sub frmNewNote_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try

            Dim sCurrentTopic As String
            Dim ofrmNote As frmNote


            ofrmNote = mGetOpenFormByName("frmNote")

            sCurrentTopic = ofrmNote.cboNoteTopic.Text

            mPopulateTopicComboBox(Me.cboTopic, sCurrentTopic)

            Me.Activate()

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private async Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Try

            
            Dim sNoteName As String
            Dim sNoteText As String
            Dim sSQL As String
            Dim iTopicID As Integer
            Dim iRecordsAffected As Integer
            Dim ofrmNote As frmNote
            Dim oUtilityClass As UtilityClass.cADONETDatabase

            ofrmNote = mGetOpenFormByName("frmNote")
            oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)


            iTopicID = Me.cboTopic.SelectedValue
            sNoteName = Trim(Me.txtNoteName.text)

            sNoteText = Await ofrmNote.wvNotes.ExecuteScriptAsync("window.getEditorData();")
            sNoteText = System.Text.Json.JsonSerializer.Deserialize(Of String)(sNoteText)


            If sNoteName IsNot Nothing AndAlso sNoteName.Length > 0 Then
                sSQL = "INSERT INTO tb_Note (Topic_ID, Note_Name, Note_Text) VALUES ("
                    sSQL = sSQL & iTopicID & ", "
                    sSQL = sSQL & "'" & Replace(sNoteName, "'", "''") & "', "
                    sSQL = sSQL & "'" & Replace(sNoteText, "'", "''") & "')"

                    oUtilityClass.OpenConnection()
                        iRecordsAffected = oUtilityClass.ExecuteSQL(sSQL)
                        Me.Text = sNoteName & " Record saved."
                        ofrmNote.mPopulateNoteNameComboBox(ofrmNote.cboNoteName, iTopicID, sNoteName)
                        Me.Close
                    oUtilityClass.CloseConnection()

                    Me.txtNoteName.Text = ""
            Else
                Throw New Exception("Note Name is required to save a note.")
            End If

            oUtilityClass = Nothing
            Me.Close()
            Me.Dispose()

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub
End Class