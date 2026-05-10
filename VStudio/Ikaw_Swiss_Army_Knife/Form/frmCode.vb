Imports System.IO
Imports System.Runtime.InteropServices.JavaScript.JSType
Imports Microsoft.Data.SqlClient

Public Class frmCode


    Private Sub frmCode_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            Dim iLeft As Integer
            Dim iTop As Integer

            Me.StartPosition = FormStartPosition.Manual

            iLeft = Application.OpenForms(0).Right - Me.Width
            iTop = Application.OpenForms(0).Top - Me.Height   'rise up from top-right

            Me.Location = New Point(iLeft, iTop)

            mPopulateCodeCategoryComboBox(Me.cboCodeCategory, enuCategoryType.MainCategory, "JS")

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub


    Public Sub mPopulateCodeTitleComboBox(
        oCodeTitleComboBox As System.Windows.Forms.ComboBox,
        Optional sDefaultValueText As String = "")

        Try

            Dim sSQL As String
            Dim sIDFieldName As String
            Dim sDescriptionFieldName As String
            Dim iCodeCategoryID As Integer
            Dim iCodeSubCategoryID As Integer
            Dim oLabelManagement As cLabelManagement

            iCodeCategoryID = Me.cboCodeCategory.SelectedValue
            iCodeSubCategoryID = Me.cboCodeSubCategory.SelectedValue

            sSQL = "SELECT Code_ID, Code_Title "
            sSQL = sSQL & "FROM tb_Code WHERE Category_ID = " & iCodeCategoryID
            sSQL = sSQL & " AND Sub_Category_ID = " & iCodeSubCategoryID
            sSQL = sSQL & " AND Active = 1 "
            sSQL = sSQL & " ORDER BY Code_Title"

            sIDFieldName = "Code_ID"
            sDescriptionFieldName = "Code_Title"

            oLabelManagement = New cLabelManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

            oLabelManagement.mPopulateComboBox(oCodeTitleComboBox, sIDFieldName, sDescriptionFieldName, sSQL, True, sDefaultValueText)

            oLabelManagement = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboCodeCategory_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboCodeCategory.SelectedIndexChanged
        Try

            Dim iCategoryID As Integer

            mResetFormControls(1)


            If mIsInitialComboBoxLoad(Me.cboCodeCategory) = False Then
                iCategoryID = Me.cboCodeCategory.SelectedValue
                Me.cboCodeSubCategory.Enabled = True
                mPopulateCodeCategoryComboBox(Me.cboCodeSubCategory, enuCategoryType.SubCategory, "", iCategoryID)
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Public Function mResetFormControls(iCaller As Integer)
        Try

            If iCaller = 1 Then
                Me.cboCodeSubCategory.Tag = ""
            End If
            Me.cboCodeTitle.Tag = ""
            Me.rtCodeSnippet.Text = ""

            If iCaller = 1 Then
                Me.cboCodeSubCategory.SelectedIndex = -1
            End If

            Me.cboCodeTitle.SelectedIndex = -1

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Function

    Private Sub cboCodeSubCategory_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboCodeSubCategory.SelectedIndexChanged

        Try

            mResetFormControls(2)

            If mIsInitialComboBoxLoad(Me.cboCodeSubCategory) = False Then
                Me.cboCodeTitle.Enabled = True
                mPopulateCodeTitleComboBox(Me.cboCodeTitle, "")
                Me.btnSaveCode.Enabled = True
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboCodeTitle_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboCodeTitle.SelectedIndexChanged
        Try

            Dim sSQL As String
            Dim sCodeText As String
            Dim iCodeID As Integer
            Dim oUtilityClass As UtilityClass.cADONETDatabase
            Dim oSQLDataReader As SqlDataReader

            If mIsInitialComboBoxLoad(Me.cboCodeTitle) = False Then

                iCodeID = Me.cboCodeTitle.SelectedValue

                sSQL = "SELECT * FROM tb_Code WHERE Code_ID = " & iCodeID

                oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)
                oUtilityClass.OpenConnection()

                oSQLDataReader = oUtilityClass.GetDataReader(sSQL)

                oSQLDataReader.Read()
                sCodeText = oSQLDataReader("Code_Text").ToString()
                Me.rtCodeSnippet.Text = sCodeText

                oSQLDataReader.Close()
                oUtilityClass.CloseConnection()

                oSQLDataReader = Nothing
                oUtilityClass = Nothing

                Me.btnDeleteCode.Enabled = True
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub btnSaveCode_Click(sender As Object, e As EventArgs) Handles btnSaveCode.Click
        Try

            Dim sCodeTitle As String
            Dim sCodeText As String
            Dim sSQL As String
            Dim iCategoryID As Integer
            Dim iSubCategoryID As Integer
            Dim iCodeID As Integer
            Dim oUtilityClass As UtilityClass.cADONETDatabase

            iCategoryID = Me.cboCodeCategory.SelectedValue
            iSubCategoryID = Me.cboCodeSubCategory.SelectedValue
            iCodeID = Me.cboCodeTitle.SelectedValue
            sCodeText = "'" & Replace(Trim(Me.rtCodeSnippet.Text), "'", "''") & "'"


            If Me.cboCodeTitle.SelectedIndex > -1 Then
                sSQL = "UPDATE tb_Code SET Code_Text = " & sCodeText & " WHERE Code_ID = " & iCodeID


            Elseif Me.cboCodeTitle.SelectedIndex = -1 Then

                sCodeTitle = "'" & Replace(Trim(me.cboCodeTitle.Text), "'", "''") & "'"

                sSQL = "INSERT INTO tb_Code "
                sSQL = sSQL & " (Category_ID ,Sub_Category_ID, Code_Title, Code_Text)"
                sSQL = sSQL & " VALUES ("
                sSQL = sSQL & iCategoryID & ", "
                sSQL = sSQL & iSubCategoryID & ", "
                sSQL = sSQL & sCodeTitle & ", "
                sSQL = sSQL & sCodeText
                sSQL = sSQL & ")"

            End If

            oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)

            oUtilityClass.OpenConnection()

            oUtilityClass.ExecuteSQL(sSQL)

            oUtilityClass.CloseConnection()

            Me.Text = "Snippet Saved."

            oUtilityClass = Nothing
        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub btnAddCodeCategory_Click(sender As Object, e As EventArgs) Handles btnAddCodeCategory.Click
        Try

            frmNewCodeCategory.ShowDialog()

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub btnDeleteCode_Click(sender As Object, e As EventArgs) Handles btnDeleteCode.Click

        Try

            Dim sCodeText As String
            Dim sSQL As String
            Dim iCodeID As Integer
            Dim dlgConfirmDelete As DialogResult
            Dim oDatabase As UtilityClass.cADONETDatabase

            iCodeID = Me.cboCodeTitle.SelectedValue
            sCodeText = Me.rtCodeSnippet.Text

            If iCodeID > 0 And Len(Trim(sCodeText)) > 0 Then
                dlgConfirmDelete = MessageBox.Show("Delete this record?", 
                                           "Confirm Deletion", 
                                           MessageBoxButtons.YesNo, 
                                           MessageBoxIcon.Warning, 
                                           MessageBoxDefaultButton.Button2)

                If dlgConfirmDelete = DialogResult.Yes Then
                    sSQL = "DELETE FROM tb_Code WHERE Code_ID = " & iCodeID

                    oDatabase = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)
                
                    oDatabase.OpenConnection
                        oDatabase.ExecuteSQL(sSQL)
                    oDatabase.CloseConnection

                    Me.rtCodeSnippet.Text = ""
                    Me.cboCodeTitle.Tag = ""
                    mPopulateCodeTitleComboBox(Me.cboCodeTitle, "")
                    Me.cboCodeTitle.Tag = "1"
                End If

            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try

    End Sub
End Class