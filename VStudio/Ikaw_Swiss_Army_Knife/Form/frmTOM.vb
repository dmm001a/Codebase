Imports System.Runtime.InteropServices.JavaScript.JSType
Imports Azure

Public Class frmTOM
    Private Sub frmTOM_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try


            Dim oPopulateTOM As cTOMManagement
            Dim x As Integer
            Dim y As Integer

            'x = Application.OpenForms(0).Right - Application.OpenForms(0).Width
            'y = Application.OpenForms(0).Top - Application.OpenForms(0).Height   'rise up from top-right

            Me.StartPosition = FormStartPosition.Manual

            x = Application.OpenForms(0).Right - Me.Width
            y = Application.OpenForms(0).Top - Me.Height   'rise up from top-right

            Me.Location = New Point(x, y)
            mFormButton(Me, False)

            oPopulateTOM = New cTOMManagement(sDNSString, sDBName, sDBUserName, sDBPassword)
            mPopulateTomOwnerComboBox(Me.cboTOMOwner, "Me")
            oPopulateTOM.PopulateTOM(Me.pnlTom, Me.cboTOMOwner.SelectedValue)

            oPopulateTOM = Nothing

            Me.AutoScroll = True
            Me.HorizontalScroll.Enabled = False
            Me.HorizontalScroll.Visible = False
            Me.VerticalScroll.Enabled = True
            Me.VerticalScroll.Visible = True

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub



    Private Sub btnNew_Click(sender As Object, e As EventArgs)
        Try

            Dim oFormNewTask As New frmNewTOM

            oFormNewTask.StartPosition = FormStartPosition.CenterScreen

            oFormNewTask.ShowDialog(Me)


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub btnNew_Click_1(sender As Object, e As EventArgs) Handles btnNew.Click
        Try

            frmNewTom.ShowDialog(Me)

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub


    Private Sub cboTOMOwner_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboTOMOwner.SelectedIndexChanged
        Try


            Dim oPopulateTOM As cTOMManagement

            If mIsInitialComboBoxLoad(Me.cboTOMOwner) = False Then  

                oPopulateTOM = New cTOMManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

                oPopulateTOM.PopulateTOM(Me.pnlTom, Me.cboTOMOwner.SelectedValue)

                oPopulateTOM = Nothing
            End If


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub
End Class