Imports System.DirectoryServices.ActiveDirectory
Imports System.IO
Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Ikaw_Launcher_Source_Control.frmCode
Imports Ikaw_Swiss_Army_Knife.frmCode
Imports Microsoft.Data
Imports Microsoft.Data.SqlClient
Imports Renci.SshNet

Module modMain

	Public sDNSString As String = "advantage.db.com"
	Public sDBName As String = "Ikaw_Swiss_Army_Knife"
	Public sDBUserName As String = "Ikaw_Source_Control_User"
	Public sDBPassword As String = "Tup3l0H0n3y#"
	Public sDefaultFolder As String

    Public sRootSourceApplicationPath as String = "C:\Users\devon\OneDrive\Programming_Projects\VSCode\IkawsoftWebApps"
    Public sRootDeploymentHoldingBinPath as String = "C:\Users\devon\Deployment_Holding_Bin\IkawsoftWebApps"
    Public sRootLocalhostDeploymentPath As String = "C:\inetpub\wwwroot"
    Public sCKEditorHTMLPath As String = "C:\ProgramData\IkawSoft\ckeditor\ckeditor.html"

    Public Enum enuDeploymentType
        None = 0
        Local = 1
        Smoke = 2
        QA = 3
        Prod = 4
    End Enum

    Public Enum enuTimeZone
        None = 0
        ET = 1
        CT = 2
        IST = 3
        UTC = 4
    End Enum

    Enum enuCategoryType
        MainCategory = 1
        SubCategory = 2
    End Enum

    Public prpSelectedDeploymentFolder As String

    Public Sub mPopulateCodeCategoryComboBox(
        oCodeCategoryComboBox As System.Windows.Forms.ComboBox,
        iCategoryTypeID As Integer,
        Optional sDefaultValueText As String = "", 
        Optional iCategoryID As Integer = -1)

        Try

            Dim sSQL As String
            Dim sIDFieldName As String
            Dim sDescriptionFieldName As String
            Dim oLabelManagement As cLabelManagement

            If iCategoryTypeID = enuCategoryType.MainCategory Then

                sSQL = "SELECT "
                sSQL = sSQL & "tb_Code_Category.Category_ID, "
                sSQL = sSQL & "tb_Code_Category.Category_Name "
                sSQL = sSQL & "FROM "
                    sSQL = sSQL & "tb_Code_Category "
                sSQL = sSQL & "WHERE Category_Type = " & iCategoryTypeID
                sSQL = sSQL & " AND Active = 1 "
                sSQL = sSQL & " ORDER BY "
                sSQL = sSQL & "Category_Name"

                sIDFieldName = "Category_ID"
                sDescriptionFieldName = "Category_Name"

            Elseif iCategoryTypeID = enuCategoryType.SubCategory Then

                If iCategoryID < 0 Then
                    Throw New Exception("Invalid Category ID")
                End If

                sSQL = "SELECT tccm.Sub_Category_ID, tcc.Category_Name "
                sSQL = sSQL & "FROM tb_Code_Category_Mapping AS tccm "
                sSQL = sSQL & "INNER JOIN tb_Code_Category AS tcc "
		            sSQL = sSQL & "ON tccm.Sub_Category_ID = tcc.Category_ID "
                sSQL = sSQL & " WHERE tccm.Category_ID = " & iCategoryID
                sSQL = sSQL & " AND tcc.Active = 1 "
                sSQL = sSQL & " ORDER BY "
                sSQL = sSQL & "tcc.Category_Name"

                sIDFieldName = "Sub_Category_ID"
                sDescriptionFieldName = "Category_Name"
            End If



            oLabelManagement = New cLabelManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

            oLabelManagement.mPopulateComboBox(oCodeCategoryComboBox, sIDFieldName, sDescriptionFieldName, sSQL, True, sDefaultValueText)

            oLabelManagement = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

        Public Sub mFormButton(oForm As Form, Optional bShowMinButton As Boolean = True)
            Try

                ' --- BASE BUTTON SIZE (auto-adjust for small forms) ---
                Dim btnSize As Integer = Math.Max(20, Math.Min(28, oForm.ClientSize.Height - 2))
                Dim oSwissArmyForm As frmSwissArmyKnife

                ' ============================================================
                ' CLOSE BUTTON
                ' ============================================================
                Dim lblClose As New Label()
                lblClose.Text = "✕"
                lblClose.Font = New Font("Segoe UI", 10, FontStyle.Bold)
                lblClose.ForeColor = Color.Black
                lblClose.BackColor = Color.Transparent
                lblClose.TextAlign = ContentAlignment.MiddleCenter
                lblClose.Size = New Size(btnSize, btnSize)
                lblClose.Anchor = AnchorStyles.Top Or AnchorStyles.Right
                lblClose.Location = New Point(oForm.ClientSize.Width - btnSize - 2, 2)

                AddHandler lblClose.Click, Sub() oForm.Close()
                oForm.Controls.Add(lblClose)

                ' ============================================================
                ' MINIMIZE BUTTON (optional)
                ' ============================================================
                If bShowMinButton Then
                    Dim lblMin As New Label()
                    lblMin.Text = "–"   ' en dash looks cleaner than hyphen
                    lblMin.Font = New Font("Segoe UI", 12, FontStyle.Bold)
                    lblMin.ForeColor = Color.Black
                    lblMin.BackColor = Color.Transparent
                    lblMin.TextAlign = ContentAlignment.MiddleCenter
                    lblMin.Size = New Size(btnSize, btnSize)
                    lblMin.Anchor = AnchorStyles.Top Or AnchorStyles.Right

                    ' Position: immediately left of Close
                    lblMin.Location = New Point(lblClose.Left - btnSize - 2, 2)

                    AddHandler lblMin.Click,
                        Sub()
                            oSwissArmyForm = mGetOpenFormByName("frmSwissArmyKnife")
                               
                            If oSwissArmyForm IsNot Nothing Then
                                'oSwissArmyForm.tmrActivate.Enabled = false
                            End If
            
                            oForm.WindowState = FormWindowState.Minimized
                        End Sub

                    oForm.Controls.Add(lblMin)
                    lblMin.BringToFront()
                End If

                ' Bring close button to front last
                lblClose.BringToFront()

            Catch oError As Exception
                MessageBox.Show(oError.Message)
            End Try
        End Sub

    Public Sub mPopulateTopicComboBox(oTopicComboBox As System.Windows.Forms.ComboBox, Optional sDefaultValueText As String = "")
        Try

            Dim sSQL As String
            Dim sIDFieldName As String
            Dim sDescriptionFieldName As String

            sSQL = "SELECT Topic_ID, Topic_Name "
            sSQL = sSQL & "FROM tb_Note_Topic WHERE Active = 1 ORDER BY Topic_Name"

            sIDFieldName = "Topic_ID"
            sDescriptionFieldName = "Topic_Name"
            Dim oLabelManagement As cLabelManagement

            oLabelManagement = New cLabelManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

                oLabelManagement.mPopulateComboBox(oTopicComboBox, sIDFieldName, sDescriptionFieldName, sSQL, True, sDefaultValueText)

            oLabelManagement = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Public Function mIsInitialComboBoxLoad(cboComboBox As System.Windows.Forms.ComboBox) As Boolean
        Try

            Dim sLoadedOnceTag As String
            Dim iTag As Integer
            Dim bIsInitialComboBoxLoad As Boolean


            bIsInitialComboBoxLoad = True

            sLoadedOnceTag = Convert.ToString(cboComboBox.Tag)
            If Integer.TryParse(sLoadedOnceTag, iTag) AndAlso iTag = 1 Then
                bIsInitialComboBoxLoad = False
            End If

            return(bIsInitialComboBoxLoad)

        Catch oError As Exception
            MessageBox.Show(oError.Message)
            return(True)
        End Try
    End Function





    Private Sub DeleteRemoteDirectoryContents(osFTP As SftpClient, sRemotePath As String)
        Try 
            ' Normalize path
            If sRemotePath.EndsWith("/") = False Then
                sRemotePath &= "/"
            End If

            Dim items = osFTP.ListDirectory(sRemotePath)

            For Each item In items

                ' Skip the special entries
                If item.Name = "." OrElse item.Name = ".." Then
                    Continue For
                End If

                If item.IsDirectory Then
                    ' Recursively delete subfolder contents
                    DeleteRemoteDirectoryContents(osFTP, item.FullName)

                    ' Delete the now-empty folder
                    osFTP.DeleteDirectory(item.FullName)

                ElseIf item.IsRegularFile Then
                    ' Delete file
                    osFTP.DeleteFile(item.FullName)
                End If

            Next
        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Public Function mConvertTimeZone(tInputTime As DateTime, eOriginTimeZone As enuTimeZone, eDestinationTimeZone As enuTimeZone) As String
        Try 

            Dim tzSourceTimeZone  As TimeZoneInfo
            Dim tzDestinationTimeZone  As TimeZoneInfo
            Dim dtSourceTimeTime As DateTime
            Dim dtUTCTime As DateTime
            Dim dtFinalTime As DateTime

            ' handles EST/EDT automatically using Eastern Time Zone
            If eOriginTimeZone = enuTimeZone.ET Then
                tzSourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")
            ElseIf eOriginTimeZone = enuTimeZone.CT Then
                tzSourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")
            ElseIf eOriginTimeZone = enuTimeZone.IST Then
                tzSourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
            ElseIf eOriginTimeZone = enuTimeZone.UTC Then
                tzSourceTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC")
            End If

            If eDestinationTimeZone = enuTimeZone.ET Then
                tzDestinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")
            ElseIf eDestinationTimeZone = enuTimeZone.CT Then
                tzDestinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time")
            ElseIf eDestinationTimeZone = enuTimeZone.IST Then
                tzDestinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")
            ElseIf eDestinationTimeZone = enuTimeZone.UTC Then
                tzDestinationTimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC")
            End If

            ' Treat input as a wall-clock time with no timezone metadata
            dtSourceTimeTime = DateTime.SpecifyKind(tInputTime, DateTimeKind.Unspecified)

            ' Convert source → UTC
            dtUTCTime = TimeZoneInfo.ConvertTimeToUtc(dtSourceTimeTime, tzSourceTimeZone)

            ' Convert UTC → destination
            dtFinalTime = TimeZoneInfo.ConvertTimeFromUtc(dtUTCTime, tzDestinationTimeZone)

            Return dtFinalTime.ToString("hh:mm tt")

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Function


    Public Function mDisplayFolderDialogBox(sMessageToDisplay As String, sDefaultPath As String) As String
		Dim sResponseToDialog As String
		sResponseToDialog = ""

		Try

			Using oFolderDialog As New FolderBrowserDialog()
				oFolderDialog.Description = sMessageToDisplay
				oFolderDialog.SelectedPath = sDefaultPath

				If oFolderDialog.ShowDialog() = DialogResult.OK Then
					sResponseToDialog = oFolderDialog.SelectedPath
				End If
			End Using

		Catch oError As Exception
			MessageBox.Show(oError.Message)
		End Try

		Return (sResponseToDialog)

	End Function


    Public Function mGetOpenFormByName(ByVal sFormName As String) As System.Windows.Forms.Form
        Try

            Dim oReturnForm As System.Windows.Forms.Form

            oReturnForm = Application.OpenForms().Cast(Of Form)().FirstOrDefault(Function(f) f.Name = sFormName)

            Return(oReturnForm)

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try        
    End Function

    Public Sub mGetFormReference(ByRef oMainForm As Form, ByVal sFormName As String, ByRef oForm As Form, ByRef tFormType As Type)
        Try 
            
            Dim tTempFormType As Type

            oForm = Nothing

            tTempFormType  = Type.GetType(oMainForm.GetType().Namespace & "." & sFormName)

            If tTempFormType IsNot Nothing Then
                oForm = Application.OpenForms.Cast(Of Form)().FirstOrDefault(Function(f) f.GetType() Is tTempFormType)
                tFormType = tTempFormType
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub


	Public Sub mPopulateAppComboBox(oAppComboBox As System.Windows.Forms.ComboBox)
		Try

			Dim sSQL As String
			Dim sIDFieldName As String
			Dim sDescriptionFieldName As String
			Dim oLabelManagement As cLabelManagement

			sSQL = "SELECT App_ID, App_Name "
			sSQL = sSQL & "FROM tb_Source_App ORDER BY App_Name"

			sIDFieldName = "App_ID"
			sDescriptionFieldName = "App_Name"

			oLabelManagement = New cLabelManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

			oLabelManagement.mPopulateComboBox(oAppComboBox, sIDFieldName, sDescriptionFieldName, sSQL)

			oLabelManagement = Nothing

		Catch oError As Exception
			MessageBox.Show(oError.Message)
		End Try
	End Sub


    Public Sub mPopulateTomOwnerComboBox(oAppComboBox As System.Windows.Forms.ComboBox, 
        Optional sDefaultValue As String = "")
        Try

            Dim sSQL As String
            Dim sIDFieldName As String
            Dim sDescriptionFieldName As String
            Dim oLabelManagement As cLabelManagement

            sSQL = "SELECT Owner_ID, Owner_Name FROM tb_TOM_Owner ORDER BY Owner_Name"

            sIDFieldName = "Owner_ID"
            sDescriptionFieldName = "Owner_Name"

            oLabelManagement = New cLabelManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

            oLabelManagement.mPopulateComboBox(oAppComboBox, sIDFieldName, sDescriptionFieldName, sSQL, false, sDefaultValue)

            oLabelManagement = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub
	Public Sub mSetAppRootPath(iAppID As Integer)
		Try

			Dim sSQL As String
			Dim oDatabase As UtilityClass.cADONETDatabase
			Dim oDataReader As SqlDataReader

			sSQL = "SELECT App_Root_Path FROM tb_Source_App WHERE App_ID = " & iAppID
			oDatabase = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)

			oDatabase.OpenConnection()
			oDataReader = oDatabase.GetDataReader(sSQL)

			While oDataReader IsNot Nothing AndAlso oDataReader.Read()
				sDefaultFolder = oDataReader("App_Root_Path").ToString()
			End While

			oDataReader.Close()
			oDatabase.CloseConnection()

			oDatabase = Nothing

		Catch oError As Exception
			MessageBox.Show(oError.Message)
		End Try
	End Sub

	Public Sub mPopulateLabelComboBox(oLabelComboBox As System.Windows.Forms.ComboBox, iAppID As Integer)
		Try

			Dim sSQL As String
			Dim sIDFieldName As String
			Dim sDescriptionFieldName As String
			Dim oLabelManagement As cLabelManagement

			sSQL = "SELECT DISTINCT Top (10) Label_Number, Label_Description, FORMAT(Entered_Date, 'MM/dd/yyyy hh:mm tt') AS Created_Date, "
			sSQL = sSQL & "CAST(Label_Number AS nvarchar(20)) + ' - ' + CAST(Label_Description AS nvarchar(200)) + ' - ' + FORMAT(Entered_Date, 'MM/dd/yyyy hh:mm tt') AS DisplayText "
			sSQL = sSQL & "FROM tb_Source_Repository WHERE App_ID = " & iAppID & " ORDER BY Label_Number DESC"

			sIDFieldName = "Label_Number"
			sDescriptionFieldName = "DisplayText"

			oLabelManagement = New cLabelManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

			oLabelManagement.mPopulateComboBox(oLabelComboBox, sIDFieldName, sDescriptionFieldName, sSQL)

			oLabelManagement = Nothing

		Catch oError As Exception
			MessageBox.Show(oError.Message)
		End Try
	End Sub
End Module
