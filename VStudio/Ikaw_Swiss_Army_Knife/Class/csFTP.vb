Imports System.IO
Imports System.Net.WebRequestMethods
Imports System.Runtime.InteropServices.JavaScript.JSType
Imports Renci.SshNet
Imports Renci.SshNet.Sftp

Public Class csFTP

	Private prpHostName As String
	Private prpPort As Integer
	Private prpUserName As String
	Private prpPassword As String

	Public Sub New(sHostName As String, iPort As Integer, sUserName As String, sPassword As String)
		Try 

			prpHostName = sHostName
			prpPort = iPort
			prpUsername = sUserName
			prpPassword = sPassword

		Catch oError As Exception
			Throw
		End Try
	End Sub

	Public Function mDeployToInterServer( _
        sLocalRootPath As String, _
        sRemoteRootPath As String, _
        sApplicationDeploymentEnvironment As String, _
        Optional sAppFolderToInclude As String = Nothing) As Boolean

		Try
			' -------------------------------
			' Variable declarations (your style)
			' -------------------------------
			
			Dim arrFileExtensionInclude As String()
            Dim arrFileToExclude As String()
			Dim arrFolderToInclude As String()
            Dim bUploadResult As Boolean
			Dim bDeploymentComplete As Boolean

			bDeploymentComplete = False

			arrFolderToInclude = {"app", "shared_library"}
			arrFileExtensionInclude = {".php", ".js", ".css", ".txt", ".htm", ".json", ".html", ".png", ".log", ".pptx", ".xlsx"}
            arrFileToExclude = {"startup.js", "startup.php"}

	    	Using osFTP As New SftpClient(prpHostName, prpPort, prpUserName, prpPassword)

				osFTP.Connect()

				    If Not osFTP.IsConnected Then
					    Throw New Exception("Unable to connect to SFTP server.")
				    End If

                    bUploadResult = mRecursiveUpload(osFTP, sLocalRootPath, sRemoteRootPath, arrFolderToInclude, arrFileExtensionInclude, arrFileToExclude, sAppFolderToInclude)
                    If bUploadResult = False Then
                        Messagebox.Show("Vendor folder upload has returned a false result.  The process will not be stopped.")
                    End If

                    If Right(sLocalRootPath, 1) <> "\" Then
				        sLocalRootPath = sLocalRootPath & "\local\"
			        End If

                    bUploadResult = mRecursiveUpload(osFTP, sLocalRootPath, sRemoteRootPath, arrFolderToInclude, arrFileExtensionInclude, arrFileToExclude, sAppFolderToInclude)

                    If bUploadResult = False Then
                        Messagebox.Show("Application folder upload has returned a false result.  The process will not be stopped.")
                    End If
				osFTP.Disconnect()

			End Using

			bDeploymentComplete = True

			Return (bDeploymentComplete)

		Catch oError As Exception
			MessageBox.Show(oError.Message)
        End Try

	End Function

    Private Function mRecursiveUpload( _
        osFTP As SftpClient, _
        sLocalRootPath As String, _
        sRemoteRootPath As String, _
        Optional arrFolderToInclude As String() = Nothing, _
        Optional arrFileExtensionInclude As String() = Nothing, _
        Optional arrFileToExclude As String() = Nothing, _
        Optional sAppFolderToInclude As String = "") as Boolean

        Try

            Dim sRelativeTemp As String
            Dim sRelative As String
            Dim sRemoteDir As String
            Dim sRemoteFile As String
            Dim sFirstFolder As String
            Dim sFileName As String
            Dim bisFileInRootDirectory As Boolean
            Dim bRecursiveUploadResult As Boolean

            bRecursiveUploadResult = False

            For Each sLocalFile As String In Directory.EnumerateFiles(sLocalRootPath, "*.*", SearchOption.AllDirectories)

                    If InStr(sLocalFile, "\app\") > 0 AND Len(Trim(sAppFolderToInclude)) > 0 Then
                        If Instr(sLocalFile, sAppFolderToInclude) = 0 Then
                            Continue For
                        End If
                    End If

                    Dim sFolderName As String = New DirectoryInfo(Path.GetDirectoryName(sLocalFile)).Name.ToLower()

					If arrFileExtensionInclude.Contains(Path.GetExtension(sLocalFile).ToLower()) Then
						'Get Relative Path
						sRelativeTemp = sLocalFile.Substring(sLocalRootPath.Length).Trim({"\"c, "/"c})

						'Check If File is In The Root Directory
						bisFileInRootDirectory = Not sRelativeTemp.Contains("\") AndAlso Not sRelativeTemp.Contains("/")

						'If Not Root File Then Check if Directory of File is Valid
						If Not bisFileInRootDirectory Then
							sFirstFolder = sRelativeTemp.Split("\"c, "/"c)(0).ToLower()

							If Not arrFolderToInclude.Contains(sFirstFolder) Then
								Continue For
							End If
						End If

						'Get Root Paths
						sRelative = sRelativeTemp.Replace("\", "/")
						sRemoteFile = sRemoteRootPath & "/" & sRelative
						sRemoteDir = Path.GetDirectoryName(sRemoteFile).Replace("\", "/")

						'Create All The Directories For The Production Destination
						mCreateAllDirectory(osFTP, sRemoteDir)

						'Upload File
                        sFileName = Path.GetFileName(sLocalFile)

                        If arrFileToExclude.Contains(sFileName) = False Then
						    Using oFileStream As New FileStream(sLocalFile, FileMode.Open)
							    osFTP.UploadFile(oFileStream, sRemoteFile, True)
						    End Using
                        End If
					End If

				Next

                bRecursiveUploadResult = true

                return(bRecursiveUploadResult)

		Catch oError As Exception
			MessageBox.Show(oError.Message)
        End Try
    End Function


	
	Public Sub mArchiveProduction(osFTP As sftpclient, sRemoteRootPath As String, sRemoteArchiveRootPath As String)
		Try

		    For Each entry In osFTP.ListDirectory(sRemoteRootPath)
			   If entry.Name = "." OrElse entry.Name = ".." Then Continue For

			   Dim sourcePath = entry.FullName
			   Dim archivePath = sRemoteArchiveRootPath & "/" & entry.Name

			   If entry.IsDirectory Then
				  ' Recurse into subfolder
				  mArchiveProduction(osFTP, sourcePath, archivePath)
			   Else
				  ' Move file
				  osFTP.RenameFile(sourcePath, archivePath)
			   End If
		    Next
			
		Catch oError As Exception
			MessageBox.Show(oError.Message)
		End Try
	End Sub

	Private Sub mCreateAllDirectory(osFTP As SftpClient, sRemotePath As String)
		Try

		    Dim arrDirectories() as String
		    Dim sCurrentDirectory As String

		    arrDirectories = sRemotePath.Split(New Char() {"/"c}, StringSplitOptions.RemoveEmptyEntries)

		    For Each sDirectory In arrDirectories 
			   sCurrentDirectory &= "/" & sDirectory
			   If Not osFTP.Exists(sCurrentDirectory) Then
				  osFTP.CreateDirectory(sCurrentDirectory)
			   End If
		    Next

		Catch oError As Exception
		    MessageBox.Show(oError.Message)
		End Try
	End Sub
End Class
