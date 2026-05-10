Imports System.ComponentModel
Imports System.ComponentModel.Design
Imports System.IO
Imports System.Reflection
Imports System.Resources
Imports System.Runtime.InteropServices
Imports System.Runtime.InteropServices.JavaScript.JSType
Imports System.Runtime.Intrinsics
Imports System.Security.Policy
Imports System.Text
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Button
Imports Azure.Core.Pipeline
Imports Microsoft.Data
Imports Microsoft.Identity.Client.AuthScheme
Imports Microsoft.VisualBasic
Imports Org.BouncyCastle.Asn1
Imports Org.BouncyCastle.X509
Imports Windows.Win32.UI



Public Class frmSwissArmyKnife


    ' Import the SetWindowPos function from user32.dll


    Public Enum enuFormAction
        Toggle = 1
        Open = 2
        Close = 3
    End Enum




    Private Sub btnLaunch_Click(sender As Object, e As EventArgs) Handles btnLaunch.Click
        Try

            Dim sApplicationSuiteName As String

            Cursor.Current = Cursors.WaitCursor
            Application.DoEvents()

            If Me.cboApplicationSuite.SelectedIndex = -1 Then
                Throw New Exception("Application suite must be selected.")
            Else
                sApplicationSuiteName = cboApplicationSuite.SelectedItem
            End If

            mProcessApplicationSelection(sApplicationSuiteName)

            Cursor.Current = Cursors.Default
            Application.DoEvents()

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub mProcessApplicationSelection(sApplicationSuiteName As String)
        Try

            Dim sProductionDeploymentSecurityCode As String
            Dim iDeploymentType As Integer
            Dim arrApplications() As Object

            'Open any selected application
            arrApplications = mGetApplicationArray(sApplicationSuiteName)

            For Each sApplication As String In arrApplications

                If sApplication = "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe" Then
                    mOpenApplication(sApplication, "--profile -directory=""Profile 2"" http://localhost/local/app/DR/main.php", False)

                    '-----------------------------------

                ElseIf sApplicationSuiteName = "Deploy Web Application" Then

                    If (Me.cboDeploymentLocation.SelectedItem = "Prod") Then
                        iDeploymentType = enuDeploymentType.Prod

                        sProductionDeploymentSecurityCode = InputBox("Enter the Production Deployment Code")

                        If Trim(sProductionDeploymentSecurityCode) <> "9361" Then
                            Throw New Exception("Production Deployment denied because of incorrect deployment code.")
                        End If

                    ElseIf (Me.cboDeploymentLocation.SelectedItem = "Smoke") Then
                        iDeploymentType = enuDeploymentType.Smoke
                    ElseIf (Me.cboDeploymentLocation.SelectedItem = "QA") Then
                        iDeploymentType = enuDeploymentType.QA
                    ElseIf (Me.cboDeploymentLocation.SelectedItem = "Local") Then
                        iDeploymentType = enuDeploymentType.Local
                        If mLocalDeploy() Then
                            Me.lblMessage.Text = DateTime.Now & " Local Deployed"
                        End If
                    Else
                        Throw New Exception("Deployment requires an Environment to be selected.")
                    End If

                    If (iDeploymentType <> enuDeploymentType.None And iDeploymentType <> enuDeploymentType.Local) Then
                        mFTPCode(iDeploymentType)
                    End If


                Else
                    mOpenApplication(sApplication, "", True)
                End If

            Next



        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Function mLocalDeploy() As Boolean

        Dim bSuccess As Boolean

        Try

            bSuccess = False

            My.Computer.FileSystem.CopyDirectory(sRootSourceApplicationPath, sRootLocalhostDeploymentPath, overwrite:=True)

            bSuccess = True

        Catch oError As Exception
            bSuccess = False
            MessageBox.Show(oError.Message)
        End Try

        Return (bSuccess)

    End Function


    Private Sub mFTPCode(iDeploymentType As Integer)

        Try

            Dim sArchiveDirectory As String
            Dim sNewVersionNumber As String
            Dim sDeployedVersionNumber As String
            Dim sApplicationDeploymentEnvironment As String
            Dim sDeploymentHoldingBinPath As String
            Dim sDeploymentCompleteMessage As String
            Dim bDeploymentComplete As Boolean
            Dim osFTP As csFTP

            If iDeploymentType = enuDeploymentType.QA Or iDeploymentType = enuDeploymentType.Prod Then
                frmDeploymentVersion.ShowDialog(Me)
            End If

            osFTP = New csFTP("ikawsoft.com", 22, "devonmanelski83", "Tup3l0H0n3y#")

            If iDeploymentType >= 2 Then
                sApplicationDeploymentEnvironment = [Enum].GetName(GetType(enuDeploymentType), iDeploymentType)
            End If

            If iDeploymentType = enuDeploymentType.Smoke Then

                'Get New Version #
                sNewVersionNumber = mIncrementVersionNumberFile(sRootSourceApplicationPath & "\local\app\DR\version_number.txt")

                'Set Archive Pathing
                sArchiveDirectory = "archive." & sNewVersionNumber

                'Run Archive
                My.Computer.FileSystem.CopyDirectory(sRootSourceApplicationPath, sRootDeploymentHoldingBinPath & "\" & sArchiveDirectory, overwrite:=True)
                mDeleteOldFolders(sRootDeploymentHoldingBinPath)
            End If


            If (iDeploymentType = enuDeploymentType.QA OR iDeploymentType = enuDeploymentType.Prod)Then
                mOfuscateAllJsFiles(sRootDeploymentHoldingBinPath & "\" & prpSelectedDeploymentFolder)
            End If

            If prpSelectedDeploymentFolder IsNot Nothing Then
                sDeploymentHoldingBinPath = sRootDeploymentHoldingBinPath & "\" & prpSelectedDeploymentFolder
            Else
                sDeploymentHoldingBinPath = sRootDeploymentHoldingBinPath & "\" & sArchiveDirectory
            End If


            bDeploymentComplete = mTransportFiles(osFTP, sDeploymentHoldingBinPath, "/var/www/html/", sApplicationDeploymentEnvironment.ToLower)

            If iDeploymentType = enuDeploymentType.Smoke Then
                sDeployedVersionNumber = sNewVersionNumber
                sDefaultFolder  = sRootDeploymentHoldingBinPath & "\" & "archive." & sNewVersionNumber
            Else
                sDeployedVersionNumber = Replace(prpSelectedDeploymentFolder, "archive.", "")
                'C:\Users\devon\Deployment_Holding_Bin\IkawsoftWebApps\archive.1.9.6
                sDefaultFolder  = sRootDeploymentHoldingBinPath & "\" & prpSelectedDeploymentFolder
            End If

            mCheckInCode(sApplicationDeploymentEnvironment & " Deployed: " & sDeployedVersionNumber, 1)

            If bDeploymentComplete = True Then
                sDeploymentCompleteMessage = DateTime.Now & " " & sDeployedVersionNumber & " " & sApplicationDeploymentEnvironment & " Deployed"
            End If

            If bDeploymentComplete = True Then
                Me.lblMessage.Text = sDeploymentCompleteMessage
                If iDeploymentType >= 2 Then
                    'Messagebox.Show("startup js and php must be deployed manually if updates were made.")
                End If
            End If

            osFTP = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Function mTransportFiles(osFTP as csFTP, sOriginationPath As String, sDestinationPath As String, sApplicationDeploymentEnvironment As String) As Boolean
        Try

            Dim sAppToDeploy As String

            sAppToDeploy = Trim(Me.cboDeployApp.SelectedItem)

            mTransportFiles = osFTP.mDeployToInterServer(sOriginationPath, sDestinationPath & sApplicationDeploymentEnvironment.ToLower, sApplicationDeploymentEnvironment.ToLower, sAppToDeploy)

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Function

    Private Sub mDeleteOldFolders(sRootPath As String)
        Try

            ' Get all archive.* folders and sort by creation date (newest → oldest)
            Dim dirs = IO.Directory.GetDirectories(sRootPath, "archive.*") _
                .OrderByDescending(Function(d) IO.Directory.GetCreationTime(d)) _
                .ToList()

            ' Skip the newest 5, delete the rest
            For Each oldDir In dirs.Skip(10)
                IO.Directory.Delete(oldDir, recursive:=True)
            Next


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub
    Public Sub mOfuscateAllJsFiles(sRootPath As String)
        Try

            ' Validate root folder
            If Not Directory.Exists(sRootPath) Then
                Throw New DirectoryNotFoundException("Folder not found: " & sRootPath)
            End If

            ' Recursively enumerate all .js files
            Dim oJSFiles = Directory.GetFiles(sRootPath, "*.js", SearchOption.AllDirectories)

            For Each oJSFile As String In oJSFiles
                Try

                    Dim sFileName As String = Path.GetFileName(oJSFile)

                    ' Skip files that contain "extensibility"
                    If sFileName.IndexOf("extensibility", StringComparison.OrdinalIgnoreCase) < 0 Then
                        mObfuscateFile(oJSFile)
                    End If

                Catch ex As Exception
                    ' Optional: log or display errors
                    Console.WriteLine("Error processing " & oJSFile & ": " & ex.Message)
                End Try
            Next

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Public Function mObfuscateFile(sInputPath As String) As String
        Try

            Dim sObfuscatedCode As String
            Dim sTempOutputFile As String = Path.Combine("C:\Users\devon\OneDrive\Programming_Projects\Deployment_Holding_Bin", "obf_" & Guid.NewGuid().ToString() & ".js")
            Dim oObfuscatorProcess As New Process()

            oObfuscatorProcess.StartInfo.FileName = "C:\Users\devon\AppData\Roaming\npm\javascript-obfuscator.cmd"
            oObfuscatorProcess.StartInfo.Arguments = $"""{sInputPath}"" --output ""{sTempOutputFile}"""
            oObfuscatorProcess.StartInfo.UseShellExecute = False
            oObfuscatorProcess.StartInfo.RedirectStandardOutput = True
            oObfuscatorProcess.StartInfo.RedirectStandardError = True
            oObfuscatorProcess.StartInfo.CreateNoWindow = True
            oObfuscatorProcess.Start()

            Dim sTempOutput As String = oObfuscatorProcess.StandardOutput.ReadToEnd()
            Dim sErrorMessage As String = oObfuscatorProcess.StandardError.ReadToEnd()
            oObfuscatorProcess.WaitForExit()

            If Not String.IsNullOrEmpty(sErrorMessage) Then
                Throw New Exception("Obfuscation error: " & sErrorMessage)
            End If

            sObfuscatedCode = File.ReadAllText(sTempOutputFile)
            File.Delete(sTempOutputFile)

            File.WriteAllText(sInputPath, sObfuscatedCode, New UTF8Encoding(False))

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Function


    Public Function mIncrementVersionNumberFile(sVersionFilePath As String) As String
        Try

            Dim sOldVersionNumber As String = File.ReadAllText(sVersionFilePath).Trim()
            Dim arrParts() As String = sOldVersionNumber.Split("."c)

            Dim iMajor As Integer = Integer.Parse(arrParts(0))
            Dim iMinor As Integer = Integer.Parse(arrParts(1))
            Dim iThird As Integer = Integer.Parse(arrParts(2))

            ' Increment patch first
            iThird += 1

            ' Handle rollover: 0–9 only
            If iThird > 9 Then
                iThird = 0
                iMinor += 1
            End If

            If iMinor > 9 Then
                iMinor = 0
                iMajor += 1
            End If


            Dim sNewVersionNumber As String = $"{iMajor}.{iMinor}.{iThird}"
            File.WriteAllText(sVersionFilePath, sNewVersionNumber)

            Return (sNewVersionNumber)

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Function
    Public Function mGetApplicationArray(sApplicationSuiteName As String) As Object
        Try

            Dim arrApplications() As Object

            If sApplicationSuiteName = "Web Development Suite" Then
                arrApplications = {
                    "C:\Users\devon\AppData\Local\Programs\Microsoft VS Code\Code.exe",
                   "ms-todo:",
                    "C:\Program Files\Microsoft Office\root\Office16\OUTLOOK.EXE",
                    "C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
                    "shell:appsFolder\SSMS.2aa993ca",
                    "shell:appsFolder\Microsoft.Copilot_8wekyb3d8bbwe!App"
                }


            ElseIf sApplicationSuiteName = "Deploy Web Application" Then
                arrApplications = {
                    ""
                }
            ElseIf sApplicationSuiteName = "To Do" Then
                arrApplications = {
                    "ms-todo:"
                }
            ElseIf sApplicationSuiteName = "SQL Server" Then
                arrApplications = {
                    "shell:appsFolder\SSMS.2aa993ca"
                }
            ElseIf sApplicationSuiteName = "Visual Studio" Then
                arrApplications = {
                    "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\devenv.exe"
                }
            ElseIf sApplicationSuiteName = "Notepad++" Then
                arrApplications = {
                    "C:\Program Files\Notepad++\notepad++.exe"
                }

            ElseIf sApplicationSuiteName = "Screenshot" Then
                arrApplications = {
                    "snippingtool.exe"
                }
            ElseIf sApplicationSuiteName = "Filezilla" Then
                arrApplications = {
                    "C:\Program Files\FileZilla FTP Client\filezilla.exe"
                }
            ElseIf sApplicationSuiteName = "Putty" Then
                arrApplications = {
                    "C:\Program Files\PuTTY\putty.exe"
                }
            ElseIf sApplicationSuiteName = "Smartsheet" Then
                arrApplications = {
                    "C:\Users\devon\AppData\Local\Programs\Smartsheet\DesktopApp\Smartsheet.exe"
                }
            ElseIf sApplicationSuiteName = "VS Code" Then
                arrApplications = {
                    "C:\Users\devon\AppData\Local\Programs\Microsoft VS Code\Code.exe"
                }
            ElseIf sApplicationSuiteName = "API Tester" Then
                arrApplications = {
                    "C:\Users\devon\OneDrive - Wolters Kluwer\SystemsDelivery - Internal_Applications\API_Tester\API_Tester.exe"
                }
            ElseIf sApplicationSuiteName = "AWS Workspace" Then
                arrApplications = {
                    "C:\Program Files\Amazon Web Services, Inc\Amazon WorkSpaces\workspaces.exe"
                }
            ElseIf sApplicationSuiteName = "Facebook" Then
                arrApplications = {
                    "C:\ProgramData\IkawSoft\SwissArmyKnife\Facebook.lnk"
                }
            ElseIf sApplicationSuiteName = "LinkedIn" Then
                arrApplications = {
                    "C:\ProgramData\IkawSoft\SwissArmyKnife\LinkedIn.lnk"
                }
            ElseIf sApplicationSuiteName = "Copilot" Then
                arrApplications = {
                    "shell:appsFolder\Microsoft.Copilot_8wekyb3d8bbwe!App"
                }
            End If

            Return (arrApplications)
        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Function

    Private Sub frmLauncher_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try

            AddHandler AppDomain.CurrentDomain.AssemblyResolve,
                Function(oDLLSender, oArguments)
                    Dim sDLLName As String
                    Dim sDLLFullPath As String

                    sDLLName = New AssemblyName(oArguments.Name).Name & ".dll"

                    sDLLFullPath = "C:\ProgramData\IkawSharedDLL\" & sDLLName

                    If IO.File.Exists(sDLLFullPath) Then
                        Return Assembly.LoadFrom(sDLLFullPath)
                    End If

                    Return Nothing
                End Function

            Dim ixPos As Integer
            Dim iyPos As Integer
            Dim oWorkArea As Rectangle


            oWorkArea = Screen.PrimaryScreen.WorkingArea
            ixPos = oWorkArea.Right - Me.Width
            iyPos = oWorkArea.Bottom - Me.Height

            Me.StartPosition = FormStartPosition.Manual
            Me.Location = New Point(ixPos, iyPos)

            'mPopulateAppComboBox(Me.cboDeployApp)
            mPopulateAppComboBox(Me.cboApp)

            Me.cboDeployApp.SelectedItem = "DR"
            Me.cboApp.SelectedValue = My.Settings.LastAppID
            Me.cboLabel.Enabled = True
            'Me.cboApplicationSuite.SelectedIndex = 0
            Me.cboDeploymentLocation.SelectedIndex = 0

            Me.KeyPreview = True
           
            dtUSTPicker.Format = DateTimePickerFormat.Custom
            dtUSTPicker.CustomFormat = "hh:mm tt"
            dtUSTPicker.ShowUpDown = True
            Me.cboOriginTimeZone.SelectedIndex = 0
            Me.cboDestinationTimeZone.SelectedIndex = 2

            Me.ctlMain.BackColor = Color.LightSteelBlue

            mFormButton(Me)
            
            'Me.TopMost = true


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub mOpenApplication(sApplicationPath As String, sArguments As String, bAsAdmin As Boolean)
        Try


            Dim sExeName As String
            Dim bOpenApplication As Boolean
            Dim oProcessStart As New ProcessStartInfo()
            Dim oRunning() As Process

            bOpenApplication = False
            sExeName = ""

            sExeName = IO.Path.GetFileNameWithoutExtension(sApplicationPath)

            If sApplicationPath.EndsWith(".msc", StringComparison.OrdinalIgnoreCase) = True Then
                bOpenApplication = True
            ElseIf sApplicationPath = "explorer.exe" Then
                bOpenApplication = True
                bAsAdmin = false
            ElseIf (sApplicationPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) = True and sExeName.Length > 0) Then

                oRunning = Process.GetProcessesByName(sExeName)

                If oRunning.Length > 0 Then
                    bOpenApplication = False
                Else
                    bOpenApplication = True
                End If

            Else
                bOpenApplication = True
            End If


            If (bOpenApplication = True) Then 'Application is not running or is msc file type

                If sApplicationPath.StartsWith("shell:", StringComparison.OrdinalIgnoreCase) Then
                    oProcessStart.FileName = "explorer.exe"
                    oProcessStart.Arguments = sApplicationPath
                Else
                    oProcessStart.FileName = sApplicationPath
                    oProcessStart.Verb = "open"
                End If
                oProcessStart.UseShellExecute = True
                


                If Not String.IsNullOrWhiteSpace(sArguments) Then
                    oProcessStart.Arguments = sArguments.Trim()
                End If

                If bAsAdmin = True AND sApplicationPath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) = True Then
                    oProcessStart.Verb = "runas"   ' Requests elevation
                End If


                Process.Start(oProcessStart)
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboApplicationSuite_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboApplicationSuite.SelectedIndexChanged
        Try

            Dim iArrayIndex As Integer
            Dim sApplicationName As String
            Dim arrValidMatch(20) As String

            sApplicationName = Me.cboApplicationSuite.SelectedItem
            iArrayIndex = 0

            arrValidMatch(0) = "AWS Workspace"
            arrValidMatch(1) = "Cheat Sheet"
            arrValidMatch(2) = "Filezilla"
            arrValidMatch(3) = "IIS"
            arrValidMatch(4) = "Notepad++"
            arrValidMatch(5) = "Putty"
            arrValidMatch(6) = "Snippet Store"
            arrValidMatch(7) = "Screenshot"
            arrValidMatch(8) = "SQL Server"
            arrValidMatch(9) = "To Do"
            arrValidMatch(10) = "Visual Studio"
            arrValidMatch(11) = "Windows Services"
            arrValidMatch(12) = "Heidi"
            arrValidMatch(13) = "Smartsheet"
            arrValidMatch(14) = "Web Development Suite"
            arrValidMatch(15) = "VS Code"
            arrValidMatch(16) = "API Tester"
            arrValidMatch(17) = "Phone Link"
            arrValidMatch(18) = "Facebook"
            arrValidMatch(19) = "LinkedIn"
            arrValidMatch(20) = "Copilot"




            For iArrayIndex = LBound(arrValidMatch) To UBound(arrValidMatch)
                If sApplicationName = arrValidMatch(iArrayIndex) Then
                    mProcessApplicationSelection(sApplicationName)
                    Exit Sub
                End If
            Next

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboApplicationSuite_KeyDown(sender As Object, e As KeyEventArgs) Handles cboApplicationSuite.KeyDown
        Try

            If e.KeyCode = Keys.Enter Then
                Me.btnLaunch.PerformClick()
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub frmLauncher_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Try

            Dim ofrmTOM As frmTOM

            If Me.WindowState = FormWindowState.Minimized Then

                ofrmTOM = mGetOpenFormByName("frmTom")

                If ofrmTOM IsNot Nothing Then
                    ofrmTOM.Dispose
                    ofrmTOM.Close
                End If
            Elseif Me.WindowState = FormWindowState.Normal Then

                 'Me.tmrActivate.Enabled = true

            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub



    Private Sub btnAddApp_Click(sender As Object, e As EventArgs) Handles btnAddApp.Click
        Try

            frmNewApp.ShowDialog()

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub btnGetLabel_Click(sender As Object, e As EventArgs) Handles btnGetLabel.Click
        Try

            Dim sRetrieveFolder As String
            Dim iAppID As Integer
            Dim iLabelNumber As Integer
            Dim oGetLabel As cLabelManagement

            If Me.cboApp.SelectedIndex = 0 Then
                Throw New Exception("A valid Application must be chosen.")
            Else
                iAppID = Me.cboApp.SelectedValue
            End If

            If Me.cboLabel.SelectedIndex = 0 Then
                Throw New Exception("A valid label must be chosen.")
            Else
                iLabelNumber = Me.cboLabel.SelectedValue
            End If

            sRetrieveFolder = mDisplayFolderDialogBox("Select a folder", Application.StartupPath)

            oGetLabel = New cLabelManagement(sDNSString, sDBName, sDBUserName, sDBPassword)


            oGetLabel.GetLabel(sRetrieveFolder, iAppID, iLabelNumber)

            MessageBox.Show("Get Label is complete.")
            oGetLabel = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboApp_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboApp.SelectedIndexChanged
        Try

            Dim iSelectedAppID As Integer

            iSelectedAppID = cboApp.SelectedValue

            If mIsInitialComboBoxLoad(cboApp) = False Then
                mSetAppRootPath(iSelectedAppID)
                mPopulateLabelComboBox(cboLabel, iSelectedAppID)
                cboLabel.Enabled = True
                If iSelectedAppID > 0 Then
                    My.Settings.LastAppID = iSelectedAppID
                    My.Settings.Save
                End If
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub btnCheckInLabel_Click(sender As Object, e As EventArgs) Handles btnCheckInLabel.Click
        Try


            Dim sNewLabelText As String
            Dim iSelectedAppID As Integer

            If (Me.cboApp.SelectedIndex = 0) Then
                Throw New Exception("An application must be selected.")
            End If

            sNewLabelText = InputBox("Enter the new label description:", "New Label")
            iSelectedAppID = Me.cboApp.SelectedValue


            mCheckInCode(sNewLabelText, iSelectedAppID)

            Me.lblMessage.Text = DateTime.Now & " " & sNewLabelText & " Check In Complete"

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub



    Public Function mCheckInCode(ByVal sNewLabelText As String, ByVal iSelectedAppID As Integer) As String

        Try

            Dim oCheckInLabel As cLabelManagement

            oCheckInLabel = New cLabelManagement(sDNSString, sDBName, sDBUserName, sDBPassword)

            If sNewLabelText.Length > 0 Then

                If sNewLabelText.Length > 100 Then
                    Throw New Exception("Label is too long.  It cannot be more than 100 characters.")
                End If

                oCheckInLabel.SetNextLabelNumber(iSelectedAppID)
                oCheckInLabel.CheckInLabel(iSelectedAppID, sDefaultFolder, sDefaultFolder, sNewLabelText)
                
                If iSelectedAppID = 1 Then 'DR Tracker
                    mBackupDatabase("local_DR_Tracker","C:\ProgramData\IkawSoft\Source_Control_DB_Backup", oCheckInLabel.iNextLabelNumber.ToString())
                    mBackupDatabase("local_Ikawsoft_Central","C:\ProgramData\IkawSoft\Source_Control_DB_Backup", oCheckInLabel.iNextLabelNumber.ToString())
                Else If iSelectedAppID = 7 Then 'Swiss Army Knife
                    mBackupDatabase("Ikaw_Swiss_Army_Knife","C:\ProgramData\IkawSoft\Source_Control_DB_Backup", oCheckInLabel.iNextLabelNumber.ToString())
                End If

                mPopulateLabelComboBox(Me.cboLabel, iSelectedAppID)

            Else
                Throw New Exception("A Label is required.")
            End If

            oCheckInLabel = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Function

    Private Sub frmLauncherSourceControl_KeyPress(sender As Object, e As KeyPressEventArgs) Handles Me.KeyPress
        Try

            If Not cboApplicationSuite.Focused Then
                cboApplicationSuite.Focus()
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub



    Public Sub mFormAction(sFormName As String, Optional eFormAction as enuFormAction = enuFormAction.Toggle)

        Try

            Dim oForm As Form
            Dim tFormType As Type

            oForm = Nothing
            tFormType = Nothing

            mGetFormReference(Me, sFormName, oForm, tFormType)

            
            If tFormType IsNot Nothing Then
                
                    If oForm Is Nothing Then
                        If eFormAction = enuFormAction.Toggle Then
                            oForm = DirectCast(Activator.CreateInstance(tFormType), Form)

                            oForm.Show(Me)
                        Else If eFormAction = enuFormAction.Open Then
                            oForm.Show(Me)
                        End If
                    Else If oForm IsNot Nothing Then
                        If eFormAction = enuFormAction.Close Then
                            oForm.Close()
                        Else
                            oForm.Close()

                    End If
                End If

            End If


            oForm = nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub


    Private Sub frmLauncherSourceControl_DoubleClick(sender As Object, e As EventArgs) Handles Me.DoubleClick
        'Try

        '    If ofrmTom IsNot Nothing AndAlso Not ofrmTom.IsDisposed Then
        '        ofrmTom.Close()
        '    End If

        '    Me.WindowState = FormWindowState.Minimized

        'Catch oError As Exception
        '    MessageBox.Show(oError.Message)
        'End Try
    End Sub



    Private Sub frmLauncherSourceControl_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Try


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub


    Private Sub ctlMain_Click(sender As Object, e As EventArgs) Handles ctlMain.Click
        Try

            Dim sTabText As String

            sTabText = ctlMain.SelectedTab.Text
            mHandleTabClick(sTabText)

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub


    Private Sub mHandleTabClick(sTabText As String)
        Try

            Dim ofrmTom As frmTOM

            ofrmTom = mGetOpenFormByName("frmTOM")

            If sTabText = "TOM" Then
                mFormAction("frmTOM", enuFormAction.Toggle)
                Me.ctlMain.SelectedIndex = 0
            Else If sTabText = "Notes" Then
                mFormAction("frmNote", enuFormAction.Toggle)
                Me.ctlMain.SelectedIndex = 0
            Else If sTabText = "Code" Then
                mFormAction("frmCode", enuFormAction.Toggle)
            Else If sTabText = "Min" Then
                Me.ctlMain.SelectedIndex = 0
                Me.WindowState = FormWindowState.Minimized
            End If

            If sTabText <> "TOM" And ofrmTom IsNot Nothing Then
                ofrmTom.Close
            End if

            If sTabText = "Close" Then
                Me.ctlMain.SelectedIndex = 0
                Me.Close
            End If


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub frmLauncherSourceControl_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        Try

            'mFormAction("frmTOM", enuFormAction.Close)

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub btnConvertTime_Click(sender As Object, e As EventArgs) Handles btnConvertTime.Click
        Try

            
            Dim eOriginTimeZone As enuTimeZone
            Dim eDestinationTimeZone As enuTimeZone


            If Me.cboOriginTimeZone.SelectedIndex = -1 OR Me.cboDestinationTimeZone.SelectedIndex = -1 Then
                Throw New Exception("Time Zone must selected.")
            Elseif Me.cboOriginTimeZone.SelectedItem = Me.cboDestinationTimeZone.SelectedItem Then
                Throw New Exception("Origin and Destination Time Zone cannot be the same.")
            End If

            If Me.cboOriginTimeZone.SelectedItem = "CT" Then
                eOriginTimeZone = enuTimeZone.CT
            Else if Me.cboOriginTimeZone.SelectedItem = "ET" Then
                eOriginTimeZone = enuTimeZone.ET
            Else if Me.cboOriginTimeZone.SelectedItem = "IST" Then
                eOriginTimeZone = enuTimeZone.IST
            Else if Me.cboOriginTimeZone.SelectedItem = "UTC" Then
                eOriginTimeZone = enuTimeZone.UTC
            End If

            If Me.cboDestinationTimeZone.SelectedItem = "CT" Then
                eDestinationTimeZone = enuTimeZone.CT
            Else if Me.cboDestinationTimeZone.SelectedItem = "ET" Then
                eDestinationTimeZone = enuTimeZone.ET
            Else if Me.cboDestinationTimeZone.SelectedItem = "IST" Then
                eDestinationTimeZone = enuTimeZone.IST
            Else if Me.cboDestinationTimeZone.SelectedItem = "UTC" Then
                eDestinationTimeZone = enuTimeZone.UTC
            End If

            Me.txtConvertedTime.Text = mConvertTimeZone(Me.dtUSTPicker.Value, eOriginTimeZone, eDestinationTimeZone)

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub frmSwissArmyKnife_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Try

            Dim ofrmTOM As frmTOM

            ofrmTOM = mGetOpenFormByName("frmTom")

            If ofrmTOM IsNot Nothing Then
                ofrmTOM.Dispose
                ofrmTOM.Close
                ofrmTOM = Nothing
            End If

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboOriginTimeZone_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboOriginTimeZone.SelectedIndexChanged
        Try

            Dim sSelectedText As String
            Dim sFormat As String

            sSelectedText = Me.cboOriginTimeZone.Text

            Me.dtUSTPicker.Format = DateTimePickerFormat.Custom
            If sSelectedText = "UTC" Then
                sFormat = "HH:mm"
            Else
                sFormat =  "hh:mm tt"
            End if

            dtUSTPicker.CustomFormat = sFormat
            Me.txtConvertedTime.Text = ""


        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub tmrActivate_Tick(sender As Object, e As EventArgs)
        Try



        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub

    Private Sub cboApp_Click(sender As Object, e As EventArgs) Handles cboApp.Click

    End Sub
End Class
