Imports System.Data.SqlClient
Imports System.IO
Imports Microsoft.Data
Imports Microsoft.Data.SqlClient

Module ModDBBackup




    Public Function mBackupDatabase(
        ByVal sDatabaseName As String,
        ByVal sBackupFolder As String, 
        ByVal Optional sLabelID As String = "") As Boolean

        Try

            Dim sSQL As String
            Dim sLabelPart = "." & sLabelID & "."

            Dim sTimestamp As String = DateTime.Now.ToString("MMddyyyy_hhmmss_tt")
            Dim sBackupFile As String = Path.Combine(sBackupFolder, $"{sDatabaseName}{sLabelPart}{sTimestamp}.bak")
            Dim oUtilityClass As UtilityClass.cADONETDatabase

            ' Ensure folder exists
            If Not Directory.Exists(sBackupFolder) Then
                Directory.CreateDirectory(sBackupFolder)
            End If

            sSQL =
                $"BACKUP DATABASE [{sDatabaseName}] " &
                $"TO DISK = '{sBackupFile}' " &
                $"WITH INIT, FORMAT;"

                oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)


                    oUtilityClass.OpenConnection()
                        oUtilityClass.ExecuteSQL(sSQL)
                    oUtilityClass.CloseConnection()


        Catch ex As Exception
            MessageBox.Show("Backup failed: " & ex.Message)
            Return False
        End Try

    End Function



End Module
