Imports System.Runtime.InteropServices.JavaScript.JSType

Public Class frmNewApp
	Private Sub btnSaveApplication_Click(sender As Object, e As EventArgs) Handles btnAddApplication.Click
		Try

			Dim sApplicationName As String
			Dim sApplicationRootFolder As String
			Dim sSQL As String
			Dim oDatabase As UtilityClass.cADONETDatabase
			Dim oDatatypeHandler As UtilityClass.cDatatypeHandler


		
			sApplicationName = Me.txtApplicationName.Text

			If sApplicationName.trim.Length = 0 Then
				Throw New Exception("Application name must be greater than zero characters.") 
			End If

			sApplicationRootFolder = mDisplayFolderDialogBox("Select the applicaton root folder.", Application.StartupPath)
			If sApplicationRootFolder.trim.Length = 0 Then
				Throw New Exception("Application root folder must be greater than zero characters.") 
			End If

			oDatatypeHandler = New UtilityClass.cDatatypeHandler
			oDatabase = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)

				sApplicationName = oDatatypeHandler.SingleQuotes(sApplicationName)
				sApplicationRootFolder = oDatatypeHandler.SingleQuotes(sApplicationRootFolder)

			oDatabase.OpenConnection
					sSQL = "INSERT INTO tb_Source_App (App_Name, App_Root_Path) VALUES ("
						sSQL = sSQL & sApplicationName & ","
						sSQL = sSQL & sApplicationRootFolder & ")"

					oDatabase.ExecuteSQL(sSQL)
				oDatabase.CloseConnection


				 mPopulateAppComboBox(frmSwissArmyKnife.cboApp)
				 Me.txtApplicationName.Text = ""
				 Me.Close()

			oDatabase = Nothing
			oDatatypeHandler = Nothing

		Catch oError As Exception
			MessageBox.Show(oError.Message)
		End Try
	End Sub

    Private Sub frmNewApp_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try

            Me.Activate()

		Catch oError As Exception
			MessageBox.Show(oError.Message)
		End Try
    End Sub
End Class