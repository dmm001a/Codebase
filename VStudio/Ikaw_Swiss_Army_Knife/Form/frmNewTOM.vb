Imports System.ComponentModel
Imports System.Runtime.InteropServices.JavaScript.JSType
Imports System.Security.Cryptography.X509Certificates
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class frmNewTOM
    Private Sub btnAddTask_Click(sender As Object, e As EventArgs) Handles btnAddTask.Click
        Try

            Dim iOwnerID As Integer
            Dim iParentID As Integer
            Dim sTaskDescription As String
            Dim sJiraID As String

            Dim oPopulateTOM As cTOMManagement

            iOwnerID = CInt(Me.cboTOMOwner.SelectedValue)
            iParentID = CInt(Me.cboParent.SelectedValue)
            sTaskDescription = Trim(Me.txtTaskDescription.Text)
            sJiraID = Trim(Me.txtJiraID.Text)

            oPopulateTOM = New cTOMManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

            oPopulateTOM.AddTOM(iOwnerID, iParentID, sTaskDescription, sJiraID)
            oPopulateTOM.PopulateTOM(Application.OpenForms().OfType(Of frmTOM)().FirstOrDefault().pnlTom, iOwnerID)

            oPopulateTOM = Nothing

            Me.Close()
            Me.Dispose()

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub frmNewTask_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try

            Dim sDefaultValue As String
            Dim sSQL As String
            Dim iOwnerID As Integer
            Dim oForm As Form
            Dim ofrmTom As frmTom
            Dim tFormType As Type
            Dim oComboBoxLoad As cLabelManagement

            

            mGetFormReference(Application.OpenForms(0), "frmTOM", oForm, tFormType)

            ofrmTom = TryCast(oForm, frmTom)

            If ofrmTom IsNot Nothing Then
                sDefaultValue = ofrmTom.cboTOMOwner.Text
                iOwnerID = CInt(ofrmTom.cboTOMOwner.SelectedValue)
            End If

            mPopulateTomOwnerComboBox(Me.cboTOMOwner, sDefaultValue)

            oComboBoxLoad = New cLabelManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

            sSQL = "SELECT 0 as Task_ID, 'None' as Task_Description FROM tb_Tom "
            sSQL = sSQL & " UNION SELECT Task_ID, Task_Description FROM tb_TOM "
            sSQL = sSQL & " WHERE "
            sSQL = sSQL & " Parent_ID = 0 "
            sSQL = sSQL & " AND Task_Complete = 0 "
            sSQL = sSQL & " AND Owner_ID = " & iOwnerID
            sSQL = sSQL & " ORDER BY Task_Description"

            oComboBoxLoad.mPopulateComboBox(Me.cboParent, "Task_ID", "Task_Description", sSQL, False, sDefaultValue)


            Me.Activate()

            oComboBoxLoad = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboParent_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboParent.SelectedIndexChanged

    End Sub

    Private Sub frmNewTOM_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Try

            Me.Dispose

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub
End Class