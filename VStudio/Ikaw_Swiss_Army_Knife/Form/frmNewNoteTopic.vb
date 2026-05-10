Public Class frmNewNoteTopic
    Private Sub btnAddTopic_Click(sender As Object, e As EventArgs) Handles btnAddTopic.Click
        Try

            Dim sTopicName As String
            Dim sSQL As String
            Dim ofrmNote As frmNote
            
            Dim oUtilityClass As UtilityClass.cADONETDatabase


            sTopicName  = Trim(Me.txtTopicName.Text)

            If sTopicName  = "" Then
                Throw New Exception("Topic Name is required.")
            End If

            sTopicName = "'" & sTopicName & "'"

            sSQL = "INSERT INTO tb_Note_Topic"
                sSQL = sSQL & " (Topic_Name)"
            sSQL = sSQL & " VALUES ("
                sSQL = sSQL & sTopicName
            sSQL = sSQL & ")"


            oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)

            oUtilityClass.OpenConnection()

                    oUtilityClass.ExecuteSQL(sSQL)
            
            oUtilityClass.CloseConnection()

            
            Messagebox.Show("Category Saved.")

            ofrmNote = mGetOpenFormByName("frmNote")
            mPopulateTopicComboBox(ofrmNote.cboNoteTopic, sTopicName)          

            Me.Close
            Me.Dispose

            oUtilityClass  = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub
End Class