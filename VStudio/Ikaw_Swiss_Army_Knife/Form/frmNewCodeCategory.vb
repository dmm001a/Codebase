Imports System.Runtime.InteropServices.JavaScript.JSType
Imports Azure.Identity

Public Class frmNewCodeCategory
    Private Sub btnCodeCategoryAdd_Click(sender As Object, e As EventArgs) Handles btnCodeCategoryAdd.Click
        Try

            Dim sCategoryName As String
            Dim sSQL As String
            Dim iCategoryID As Integer
            Dim iCategoryTypeID As Integer
            Dim iCodeID As Integer
            Dim iNewID As Integer
            Dim oUtilityClass As UtilityClass.cADONETDatabase

            If Trim(Me.cboCodeCategoryType.SelectedItem) = "Category" Then
                iCategoryTypeID = enuCategoryType.MainCategory
            Else If Trim(Me.cboCodeCategoryType.SelectedItem) = "Sub-Category" Then
                iCategoryTypeID = enuCategoryType.SubCategory
                iCategoryID = Me.cboCategory.SelectedValue
            Else
                Throw New Exception("A Category Type must be selected.")
            End If


            sCategoryName = Trim(Me.txtCodeCategoryName.Text)

            If sCategoryName = "" Then
                Throw New Exception("Category Name is required.")
            End If

            sCategoryName = "'" & Replace(sCategoryName, "'", "''") & "'"

            oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)

            oUtilityClass.OpenConnection()

                sSQL = "INSERT INTO tb_Code_Category "
                    sSQL = sSQL & " (Category_Type, Category_Name) "
                    sSQL = sSQL & "OUTPUT INSERTED.Category_ID "
                sSQL = sSQL & "VALUES ("
                    sSQL = sSQL & iCategoryTypeID & ", "
                    sSQL = sSQL & sCategoryName
                sSQL = sSQL & ")"

                iNewID  = oUtilityClass.ExecuteSQL(sSQL)

                If iCategoryTypeID = enuCategoryType.SubCategory Then
                    sSQL = "INSERT INTO tb_Code_Category_Mapping "
                        sSQL = sSQL & " (Category_ID, Sub_Category_ID) "
                    sSQL = sSQL & "VALUES ("
                        sSQL = sSQL & iCategoryID & ", "
                        sSQL = sSQL & iNewID
                    sSQL = sSQL & ")"

                    oUtilityClass.ExecuteSQL(sSQL)
                End If

            oUtilityClass.CloseConnection()

            Messagebox.Show("Category Saved.")

            mRefreshCodeForm(iCategoryTypeID, sCategoryName)

            Me.Close
            Me.Dispose

            oUtilityClass = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub


    Private Sub mRefreshCodeForm(ByVal iCategoryTypeID As Integer, ByVal sCategoryName As String)
        Try

            Dim iCategoryID As Integer
            Dim oCodeForm As frmCode
            Dim oCategoryCodeComboBox As System.Windows.Forms.ComboBox

            oCodeForm = mGetOpenFormByName("frmCode")

            If iCategoryTypeID = enuCategoryType.MainCategory Then
                iCategoryID = -1
                oCategoryCodeComboBox = oCodeForm.cboCodeCategory
            Elseif iCategoryTypeID = enuCategoryType.SubCategory Then
                iCategoryID = Me.cboCategory.SelectedValue
                oCategoryCodeComboBox = oCodeForm.cboCodeSubCategory
            End If

            oCategoryCodeComboBox.Tag = ""
            mPopulateCodeCategoryComboBox(oCategoryCodeComboBox, iCategoryTypeID, sCategoryName, iCategoryID)
            oCategoryCodeComboBox.Tag = 1


            oCodeForm = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboCodeCategoryType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboCodeCategoryType.SelectedIndexChanged
        Try

            Dim iCategoryTypeID As Integer

            If Trim(Me.cboCodeCategoryType.SelectedItem) = "Category" Then
                iCategoryTypeID = 1
            Else If Trim(Me.cboCodeCategoryType.SelectedItem) = "Sub-Category" Then
                iCategoryTypeID = 2
            End If

            If iCategoryTypeID = 2 Then
                Me.lblCategoryID.Visible = True
                Me.cboCategory.Visible = True
                mPopulateCodeCategoryComboBox(Me.cboCategory, 1, "", -1)
            Else
                Me.lblCategoryID.Visible = False
                Me.cboCategory.Visible = False
            End If


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboCategory_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboCategory.SelectedIndexChanged

    End Sub

    Private Sub frmNewCodeCategory_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Try
            Me.Dispose
        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub
End Class