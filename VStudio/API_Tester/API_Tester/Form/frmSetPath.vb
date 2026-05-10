Imports APITracker.MainMod

Public Class frmSetPath

    Friend Property eSavePathType As enuSavePathType


    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        Try

            Dim sValueToSave As String = Trim(txtPath.Text)

            Select Case eSavePathType
                'Case enuSavePathType.ExcelReport
                '    My.Settings.ExcelPath = sValueToSave

                Case enuSavePathType.LogAnalyzer
                    My.Settings.LogAnalyzerPath = sValueToSave

            ' Case enuSavePathType.DBPath
            '     My.Settings.DBPath = sValueToSave
            '     sConnectionString = Replace(sConnectionString, "{{DBPath}}", sValueToSave)
            '     frmMain.mLoadComboBox(frmMain.cboEnvironment)

                'Case enuSavePathType.DSNName
                '    My.Settings.DSNName = sValueToSave
                '    sConnectionString = sValueToSave
                '    mLoadComboBox(frmMain.cboEnvironment.SelectedIndex, frmMain.cboEnvironment)
            End Select

            My.Settings.Save()
            MsgBox("Setting Saved")
            Me.Close()

		Catch ex As Exception
			mHandleError(ex)
        End Try
    End Sub

    Private Sub frmSetPath_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try

            Dim sValueToGet As String = ""


            If eSavePathType = enuSavePathType.ExcelReport Then
                sValueToGet = My.Settings("ExcelPath").ToString()
            ElseIf eSavePathType = enuSavePathType.LogAnalyzer Then
                sValueToGet = My.Settings("LogAnalyzerPath").ToString()
            ElseIf eSavePathType = enuSavePathType.DSNName Then
                sValueToGet = My.Settings("DSNName").ToString()
            End If

            Me.txtPath.Text = sValueToGet

		Catch ex As Exception
			mHandleError(ex)
        End Try
    End Sub

    Private Sub txtPath_TextChanged(sender As Object, e As EventArgs) Handles txtPath.TextChanged

    End Sub
End Class