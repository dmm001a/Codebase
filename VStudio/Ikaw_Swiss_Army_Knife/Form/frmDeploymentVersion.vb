Imports System.Runtime.InteropServices.JavaScript.JSType
Imports System.IO

Public Class frmDeploymentVersion
    Private Sub btnDeploy_Click(sender As Object, e As EventArgs) Handles btnDeploy.Click
        Try

            prpSelectedDeploymentFolder = Me.cboAppVersion.Text
            Me.Close
            Me.Dispose

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub frmDeploymentVersion_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try

            
            Dim sFolderName As String
            Dim arrFolders() As String

            
            If Directory.Exists(sRootDeploymentHoldingBinPath) = False Then
                MessageBox.Show("Folder not found: " & sRootDeploymentHoldingBinPath)
                Exit Sub
            End If

            cboAppVersion.Items.Clear()

            arrFolders = Directory.GetDirectories(sRootDeploymentHoldingBinPath)

            For Each sFolderPath As String In arrFolders
                sFolderName = Path.GetFileName(sFolderPath)
                cboAppVersion.Items.Add(sFolderName)
            Next

            If cboAppVersion.Items.Count > 0 Then
                cboAppVersion.SelectedIndex = cboAppVersion.Items.Count - 1
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub


End Class