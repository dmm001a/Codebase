Imports System.IO
Imports System.Runtime.InteropServices.JavaScript.JSType
Imports System.Text
Imports Microsoft.Data.SqlClient


Public Class cLabelManagement

	Public iNextLabelNumber As Integer
	Private sDNSString as String
	Private sDBName As String
	Private sDBUserName As String
	Private sDBPassword As String
    

    Public Sub New(sNewDNSString, sNewDBName, sNewDBUserName, sNewDBPassword)
		Try
			iNextLabelNumber = -1
			sDNSString = sNewDNSString
			sDBName = sNewDBName
			sDBUserName = sNewDBUserName
			sDBPassword = sNewDBPassword

		Catch oError As Exception
			Throw
		End Try
    End Sub

    Public Sub CheckInLabel(iAppID As Integer, sRootPath As String, sRootDirectoryPath As String, sLabelDescription As String)
        Try

		  Dim sFolderName As String
		  Dim sFileName As String
		  Dim sFileContents As String

		  If iNextLabelNumber = -1 Then
			Throw New Exception("Please call SetNextLabelNumber before calling CheckInLabel.")
		  End If

            ' Enumerate all files in the current directory
            For Each sFilePath In Directory.EnumerateFiles(sRootDirectoryPath)
			 sFileName = Path.GetFileName(sFilePath)
			 If mIsValidFileToRead(sFileName) Then
				sFileContents = File.ReadAllText(sFilePath, Encoding.UTF8)
				InsertFileContents(iAppID, iNextLabelNumber, sLabelDescription, sRootPath, sFilePath, sFileName, sFileContents)
			 End If
            Next

            ' Recurse into subdirectories
            For Each sDirectoryPath In Directory.EnumerateDirectories(sRootDirectoryPath)
			 sFolderName = Path.GetFileName(sDirectoryPath)
			 If Not sFolderName.StartsWith(".") Then
	                CheckInLabel(iAppID, sRootPath, sDirectoryPath, sLabelDescription)
			 End If
            Next

        Catch oError As Exception
            Throw
        End Try
    End Sub
    
    	Public Sub GetLabel(sCreateRootPath As String, iAppID As Integer, iLabelNumber As Integer)
	    Try

		   Dim sSQL As String
		   Dim sRootPath As String
		   Dim sFilePath As String
		   Dim sRelativePath As String
		   Dim sFileName As String
		   Dim sDestinationFullFilePath As String
		   Dim sFileContents As String
		   Dim sCheckDirectory As String

		   Dim oUtilityClass As UtilityClass.cADONETDatabase
		   Dim oSQLDataReader As SqlDataReader
		   

		   sSQL = "SELECT * FROM tb_Source_Repository WHERE App_ID = " & iAppID & " AND Label_Number = " & iLabelNumber

		   oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)
		   oUtilityClass.OpenConnection()

		   oSQLDataReader = oUtilityClass.GetDataReader(sSQL)

		   While oSQLDataReader IsNot Nothing AndAlso oSQLDataReader.Read()
				sRootPath = oSQLDataReader("Root_Path").ToString()
				sFilePath = oSQLDataReader("File_Path").ToString()
				sRelativePath = Replace(sFilePath, sRootPath, "")
				sFileName = oSQLDataReader("File_Name").ToString()
				sFileContents = oSQLDataReader("File_Contents").ToString()
				sDestinationFullFilePath = sCreateRootPath + sRelativePath

				sCheckDirectory = Path.GetDirectoryName(sDestinationFullFilePath)

				If Not Directory.Exists(sCheckDirectory) Then
				    Directory.CreateDirectory(sCheckDirectory)
				End If


				File.WriteAllText(sDestinationFullFilePath, sFileContents, New System.Text.UTF8Encoding(False))

		   End While
		   

		   oSQLDataReader.Close()
		   oUtilityClass.CloseConnection()

		   oSQLDataReader= Nothing
		   oUtilityClass = Nothing

	    Catch oError As Exception
		   Throw
	    End Try
	End Sub

   Public Sub mPopulateComboBox( _
        oComboBox As System.Windows.Forms.ComboBox, _
        sIDFieldName As String, _
        sDescriptionFieldName As String, _
        sSQL As String, _
        Optional bBlankRow As Boolean = True, _
        Optional sDefaultValueText As String = "")

        Try  

            Dim oDatabase As UtilityClass.cADONETDatabase
            Dim oDatatable As DataTable
            Dim oBlankComboBoxRow As DataRow

            oDatabase = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)
            oDatatable = New DataTable

            ' Clear existing binding
            oComboBox.DataSource = Nothing
            oComboBox.Items.Clear()   ' safe now because DataSource is gone
            oComboBox.SelectedIndex = -1

            oDatabase.OpenConnection()
            oDatatable = oDatabase.GetDatatTable(sSQL)

            If bBlankRow Then
                oBlankComboBoxRow = oDatatable.NewRow()
                oBlankComboBoxRow(sIDFieldName) = -1
                oBlankComboBoxRow(sDescriptionFieldName) = ""
                oDatatable.Rows.InsertAt(oBlankComboBoxRow, 0)
            End If

            ' Bind to ComboBox
            oComboBox.DisplayMember = sDescriptionFieldName
            oComboBox.ValueMember = sIDFieldName
            oComboBox.DataSource = oDatatable
            oComboBox.Tag = "1"

            ' Apply default value if provided
            If sDefaultValueText <> "" Then
                oComboBox.SelectedIndex = oComboBox.FindString(sDefaultValueText)
            Else
                oComboBox.SelectedIndex = 0
            End If

        Catch oError As Exception
            Throw
        End Try

    End Sub


    Private Function mIsValidFileToRead(sFileName As String) as Boolean
	   Dim bIsValidFileReturn As Boolean

	   Try

		    Dim sFileExtension As String 
		    
		    Dim arrAllowedExtensions As String() = {
			    ".txt", ".log", ".csv", ".tsv",
			    ".json", ".xml", ".yaml", ".yml",
			    ".ini", ".cfg", ".conf", ".config",
			    ".md", ".rst",
			    ".vb", ".cs", ".js", ".ts", ".html", ".htm",
			    ".css", ".scss", ".sass",
			    ".php", ".py", ".rb", ".java", ".sql",
			    ".sh", ".ps1", ".bat", ".cmd",
			    ".env", ".toml", ".properties",
			    ".svg", ".xaml", ".csproj", ".vbproj", 
			    ".sln", ".js", ".php", ".css"			    
		    }


		    bIsValidFileReturn = False

		    sFileExtension = Path.GetExtension(sFileName).ToLower()

		    If arrAllowedExtensions.Contains(sFileExtension) Then
			   bIsValidFileReturn = True
		    End If

        Catch oError As Exception
            Throw
        End Try

	   Return (bIsValidFileReturn)
	End Function

	Public Sub SetNextLabelNumber(iAppID As Integer)
	    Try
		   Dim sSQL As String
		   Dim oUtilityClass As UtilityClass.cADONETDatabase
		   Dim oSQLDataReader As SqlDataReader
		   

		   sSQL = "SELECT ISNULL(MAX(Label_Number), 0) + 1 AS Next_Label_Number FROM tb_Source_Repository WHERE App_ID = " & iAppID

		   oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)
		   oUtilityClass.OpenConnection()

		   oSQLDataReader = oUtilityClass.GetDataReader(sSQL)

		   If oSQLDataReader IsNot Nothing AndAlso oSQLDataReader.Read() Then
			  iNextLabelNumber = CInt(oSQLDataReader("Next_Label_Number"))
		   End If

		   oSQLDataReader.Close()
		   oUtilityClass.CloseConnection()

		   oSQLDataReader= Nothing
		   oUtilityClass = Nothing

	    Catch oError As Exception
		   Throw
	    End Try
	End Sub

    Private Function InsertFileContents(iAppID As Integer, iNextLabelNumber As integer, sLabelDescription As String, sRoothPath as String, sFilePath As String, sFileName As String, sFileContents As String) As Integer
	   Dim iReturnInteger As Integer

        Try

		Dim sSQL as String
		
		Dim oDatabase As UtilityClass.cADONETDatabase
		Dim oDatatypeHandler As UtilityClass.cDatatypeHandler


		oDatatypeHandler = New UtilityClass.cDatatypeHandler
		sLabelDescription = oDatatypeHandler.SingleQuotes(sLabelDescription)
		sRoothPath = oDatatypeHandler.SingleQuotes(sRoothPath)
		sFilePath = oDatatypeHandler.SingleQuotes(sFilePath)
		sFileName = oDatatypeHandler.SingleQuotes(sFileName)
		sFileContents = "N" & oDatatypeHandler.SingleQuotes(sFileContents)


		sSQL = "INSERT INTO tb_Source_Repository (App_ID, Label_Number, Label_Description, Root_Path, File_Path, File_Name, File_Contents) "
		sSQL = sSQL & " VALUES ("
			sSQL = sSQL & iAppID & ", "
			sSQL = sSQL & iNextLabelNumber & ", "
			sSQL = sSQL & sLabelDescription & ", "
			sSQL = sSQL & sRoothPath & ", "
			sSQL = sSQL & sFilePath & ", "
			sSQL = sSQL & sFileName & ", "
			sSQL = sSQL & sFileContents & ")"


		oDatabase = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)
		
			oDatabase.OpenConnection
				
				iReturnInteger = oDatabase.ExecuteSQL(sSQL)

			oDatabase.CloseConnection
			

		oDatabase = Nothing
		oDatatypeHandler = Nothing
		  

        Catch oError As Exception
            Throw
        End Try

	   return(iReturnInteger)

    End Function
End Class


    'Public Sub ReadFiles(sRootPath As String, processFile As Action(Of String, String))
    '    Try

		  'Dim sFileContents As String

    '        ' Enumerate all files in the current directory
    '        For Each sFilePath In Directory.EnumerateFiles(sRootPath)
    '            sFileContents = File.ReadAllText(sFilePath, Encoding.UTF8)
    '            processFile(sFilePath, sFileContents)
    '        Next

    '        ' Recurse into subdirectories
    '        For Each sDirectoryPath In Directory.EnumerateDirectories(sRootPath)
    '            ReadFiles(sDirectoryPath, processFile)
    '        Next

    '    Catch oError As Exception
    '        MessageBox.Show(oError.Message)
    '    End Try
    'End Sub