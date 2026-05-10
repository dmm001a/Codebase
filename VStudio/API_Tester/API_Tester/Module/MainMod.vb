Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Diagnostics
Imports Microsoft.VisualBasic.Logging
Imports System.Net
Imports System.Windows.Forms
Imports System.Text.Json


Module MainMod
    'Private Declare Function CoCreateGuid Lib "ole32.dll" (ByRef guid As GUID) As Long

    <DllImport("ole32.dll")>
    Public Function CoCreateGuid(ByRef guid As GUID) As Integer
    End Function

    <DllImport("kernel32.dll")>
    Public Sub Sleep(ByVal dwMilliseconds As Integer)
    End Sub


#If VBA7 Then
Private Declare PtrSafe Function ShellExecute Lib "shell32.dll" _
    Alias "ShellExecuteA" (ByVal hwnd As LongPtr, ByVal lpOperation As String, _
    ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String, _
    ByVal nShowCmd As Long) As LongPtr
#Else
    Private Declare Function ShellExecute Lib "shell32.dll" _
    Alias "ShellExecuteA" (ByVal hwnd As Long, ByVal lpOperation As String,
    ByVal lpFile As String, ByVal lpParameters As String, ByVal lpDirectory As String,
    ByVal nShowCmd As Long) As Long
#End If


    <StructLayout(LayoutKind.Sequential)>
    Public Structure GUID
        Public Data1 As Integer
        Public Data2 As Short
        Public Data3 As Short
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)>
        Public Data4() As Byte
    End Structure

    Public Enum enuSavePathType
        ExcelReport = 1
        LogAnalyzer = 2
        DBPath = 3
        DSNName = 4
    End Enum



    Public Enum enuEnvironment
	   DEV = 1
        QA = 2
        Stage = 3
        Prod = 4
    End Enum

    Public Enum enuLogMessageType
	   Standard = 1
	   Exception = 2
	   Warning = 3
    End Enum

    Public gConnectionString As String
    Public arrAttributes(,) As Object

    Public Const cstGet As Integer = 1
    Public Const cstPost As Integer = 2
    Public sConnectionString As String
    Public Const adOpenForwardOnly As Integer = 0
    Public Const adOpenKeyset As Integer = 1
    Public Const adOpenDynamic As Integer = 2
    Public Const adOpenStatic As Integer = 3
    Public Const adLockReadOnly As Integer = 1
    Public Const adLockPessimistic As Integer = 2
    Public Const adLockOptimistic As Integer = 3
    Public Const adLockBatchOptimistic As Integer = 4


    Public Function GenerateUUID() As String
        Dim uuid As System.Guid = System.Guid.NewGuid()
        Return uuid.ToString()
    End Function


    Public Function mReplaceStandardAttribute(sRequestObject As String) As String

		Dim sNewUUID As String

		Try

			snewUUID = GenerateUUID()
			sRequestObject = Replace(sRequestObject, "{{randomUUID}}", snewUUID)
			sRequestObject = Replace(sRequestObject, "{{timestamp}}", snewUUID)
			sRequestObject = Replace(sRequestObject, "{{$timestamp}}", snewUUID)
			sRequestObject = Replace(sRequestObject, "{{$randomUUID}}", snewUUID)
			
			If InStr(sRequestObject, "publishPkgProductRequest") Then
				sRequestObject = Replace(sRequestObject, "{{RequestDateTime}}", Replace(Format(Now(), "yyyy-MM-dd HH:mm:ss"), " ", "T"))
			Else
				sRequestObject = Replace(sRequestObject, "{{RequestDateTime}}", Format(Now(), "yyyy-MM-dd HH:mm:ss"))
			End If

			return(sRequestObject)

		Catch ex As Exception
			mHandleError(ex)
		End Try
    End Function
Public Function mLoadComboBox(oComboBox As System.Windows.Forms.ComboBox, Optional iEnvironment As Integer = 0) As Integer

        ' Declare Variables
        Dim sSQL As String = ""
        Dim sDescriptionFieldName As String = ""
        Dim sIDFieldName As String = ""
        Dim oDatabase As cDatabase
        Dim oDatatable As DataTable
        Dim blankRow As DataRow
        Dim sComboDefaultText As String
        Dim iReturn As Integer = 0

        
    Try
        ' Build SQL Based on ComboBox
        If oComboBox.Name = "cboEnvironment" Then
            sSQL = "SELECT Environment_ID, Environment_Desc FROM tb_Environment ORDER BY Environment_ID"
            sIDFieldName = "Environment_ID"
            sDescriptionFieldName = "Environment_Desc"

        ElseIf oComboBox.Name = "cboAPI" Then
            sSQL = "SELECT API_ID, API_Desc FROM tb_API WHERE Active_API = 1"
            If iEnvironment = enuEnvironment.Prod Then
                sSQL &= " AND Production_Enabled = 1"
            End If
            sSQL &= " ORDER BY API_Desc"
            sIDFieldName = "API_ID"
            sDescriptionFieldName = "API_Desc"

        ElseIf oComboBox.Name = "cboAttribute" Then
            sSQL = "SELECT attrib.Attribute_ID, attrib.Attribute_Name " &
                   "FROM tb_Attribute attrib " &
                   "INNER JOIN tb_Attribute_Value attribvalue ON attrib.Attribute_ID = attribvalue.Attribute_ID " &
                   "WHERE attribvalue.Environment_ID = " & iEnvironment & " " &
                   "ORDER BY attrib.Attribute_Name"
            sIDFieldName = "Attribute_ID"
            sDescriptionFieldName = "Attribute_Name"
        ElseIf oComboBox.Name = "cboTestDateTimeFilter" Then
            sSQL = "SELECT ROW_NUMBER() OVER (ORDER BY Test_Date_Time DESC) AS Row_ID, "
                sSQL = sSQL & "strftime('%m/%d/%Y', Test_Date_Time) || ' ' || strftime('%I:%M:%S %p', Test_Date_Time) AS Test_Date_Time "
                sSQL = sSQL & "FROM (SELECT DISTINCT Test_Date_Time FROM tb_Test_Report ORDER BY Test_Date_Time DESC) AS DistinctDates"
            sIDFieldName = "Row_ID"
            sDescriptionFieldName = "Test_Date_Time"
        End If

        ' Instantiate and open connection
        oDatabase = New cDatabase(gConnectionString)
        oDatatable = New DataTable

        oDatatable = oDatabase.mGetDataTable(sSQL)

        ' Add blank row at the top

        If oComboBox.Name = "cboTestDateTimeFilter" Then
            sComboDefaultText =""
        Else
            sComboDefaultText = ""
        End If
        blankRow = oDatatable.NewRow()
        blankRow(sIDFieldName) = 0
        blankRow(sDescriptionFieldName) = sComboDefaultText
        oDatatable.Rows.InsertAt(blankRow, 0)

        ' Bind to ComboBox
        oComboBox.DisplayMember = sDescriptionFieldName
        oComboBox.ValueMember = sIDFieldName
        oComboBox.DataSource = oDatatable
        oComboBox.SelectedIndex = 0
        
        'If oComboBox.Name = "cboAPI" Then
        '    mPopulateAttributeArray(iEnvironment)
        'End If

        iReturn = 1
        oDatabase = Nothing
        blankRow = Nothing

    Catch ex As Exception
        iReturn = 0
        mHandleError(ex)
    End Try

    Return iReturn
    
End Function


    Public Sub mPopulateAttributeArray(iEnvironmentID As Integer)
        Try

        Dim oDatabase As cDatabase
        Dim oDatatable As DataTable
        Dim sSQL As String


        oDatabase = New cDatabase(gConnectionString)
        oDatatable = New DataTable

        sSQL = "SELECT attrib.Attribute_Name, attribval.Attribute_Value, attrib.Attribute_ID FROM tb_Attribute attrib INNER JOIN tb_Attribute_Value attribval "
            sSQL = sSQL & " ON attrib.Attribute_ID = attribval.Attribute_ID WHERE attribval.Environment_ID = " & iEnvironmentID


        oDatatable = oDatabase.mGetDataTable(sSQL)

            If oDatatable.Rows.Count > 0 Then
                Erase arrAttributes
                Dim rowCount As Integer = oDatatable.Rows.Count
                Dim colCount As Integer = oDatatable.Columns.Count
                ReDim arrAttributes(rowCount - 1, colCount - 1)

                For i As Integer = 0 To rowCount - 1
                    For j As Integer = 0 To colCount - 1
                        arrAttributes(i, j) = oDatatable.Rows(i)(j)
                    Next
                Next
            Else
                Erase arrAttributes
            End If



	    Catch ex As Exception
		   mHandleError(ex)
        End Try

    End Sub
   Public Function mGetSelectedComboValue(ByVal oComboBox As System.Windows.Forms.ComboBox) As Integer
        Try

        If (oComboBox.SelectedIndex <> -1 And oComboBox.SelectedIndex <> 0) Then
            mGetSelectedComboValue = oComboBox.SelectedValue
        Else
            mGetSelectedComboValue = 0
        End If

        Exit Function

	    Catch ex As Exception
		   mHandleError(ex)
        End Try
    End Function

    Public Function ByteArrayToHex(arr() As Byte, startIdx As Integer, endIdx As Integer) As String
		Dim hexStr As New StringBuilder()

		Try

		   For i As Integer = startIdx To endIdx
			  hexStr.Append(arr(i).ToString("X2"))
		   Next

		   Return hexStr.ToString()

		Catch ex As Exception
		   mHandleError(ex)
		End Try
    End Function

    

   Public Function mSingleQuotes(sString As String) As String

	Dim sCleanString As String
    Try

        If String.IsNullOrWhiteSpace(sString) OrElse sString = "12:00:00 AM" Then
            Return "NULL"
        Else
            ' Sanitize line breaks and escape single quotes
            sCleanString = sString.Replace(vbCrLf, " ").Replace(vbLf, " ").Replace(vbCr, " ")
            Return "'" & sCleanString.Replace("'", "''") & "'"
        End If

	Catch ex As Exception
		mHandleError(ex)
        Return "NULL"
    End Try
End Function


''' <summary>
''' Builds a Dictionary(Of Integer, String) using column captions from a DataTable.
''' Keys are sequential integers starting from 0.
''' </summary>
''' <param name="dtSource">The source DataTable whose column captions will be used.</param>
''' <returns>A Dictionary(Of Integer, String) where keys are column indexes and values are captions.</returns>
Public Function mBuildDictionaryFromColumnCaptions(ByVal oDatable As DataTable) As Dictionary(Of Integer, String)

    ' Declare variables
    Dim oDictionary As Dictionary(Of Integer, String)
    Dim iColIndex As Integer
    Dim iKey As Integer
    Dim sCaption As String
    Dim oColumn As DataColumn

    ' Instantiate dictionary
    oDictionary = New Dictionary(Of Integer, String)

    Try
        ' Validate input
        If oDatable Is Nothing Then
            Return oDictionary
        End If

        If oDatable.Columns.Count = 0 Then
            Return oDictionary
        End If

        ' Loop through columns
        For iColIndex = 0 To oDatable.Columns.Count - 1
            oColumn = oDatable.Columns(iColIndex)

            iKey = iColIndex
            sCaption = oColumn.Caption

            ' Fallback to column name if caption is empty
            If String.IsNullOrWhiteSpace(sCaption) Then
                sCaption = oColumn.ColumnName
            End If

            If Not oDictionary.ContainsKey(iKey) Then
                oDictionary.Add(iKey, sCaption)
            End If
        Next

	Catch ex As Exception
		mHandleError(ex)
    End Try

    Return oDictionary

End Function
    Public Function mLenTrim(sString As String) As Integer
        Try


	   	   mLenTrim = Len(Trim(sString))

		Catch ex As Exception
		   mHandleError(ex)
        End Try

    End Function

    Function mExtractSubstring(sSource As String, sTarget As String) As String
        Try

            Dim iPos As Integer

            iPos = InStr(1, sSource, sTarget)

            If iPos > 0 Then
                mExtractSubstring = Mid(sSource, iPos, Len(sTarget))
            Else
                mExtractSubstring = ""
            End If

		Catch ex As Exception
		   mHandleError(ex)
        End Try
    End Function

    

    Public Sub mOpenSharePointLink()
        Try
            Dim sURl As String
            sURl = "https://wolterskluwer.sharepoint.com/:w:/s/GBS-SystemsDelivery/SystemsDelivery/IQDq1E7Ww6nySJDn0sOnSI8PAdWmLWNoqxWdynYh8EieMNQ?e=qVLGAv"

            Try
                Dim psi As New ProcessStartInfo()
                psi.FileName = sURl
                psi.UseShellExecute = True

                Process.Start(psi)
            Catch ex As Exception
                MessageBox.Show("Could not open URL: " & ex.Message)
            End Try

            sURl = "https://wolterskluwer.sharepoint.com/:w:/s/GBS-SystemsDelivery/SystemsDelivery/IQCWJX2sGYAnRYC_a_ingPVxAatZvsGb1kCInCbkEoSpfdE?e=z8fvag"

            Try
                Dim psi As New ProcessStartInfo()
                psi.FileName = sURl
                psi.UseShellExecute = True

                Process.Start(psi)
            Catch ex As Exception
                MessageBox.Show("Could not open URL: " & ex.Message)
            End Try

		Catch ex As Exception
		   mHandleError(ex)
        End Try
    End Sub



Public Function mConvertUSDateTimeToISO(ByVal sUSDateTime As String) As String

    Dim dParsed As DateTime
    Dim sISODateTime As String

    Try

        If DateTime.TryParse(sUSDateTime, Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dParsed) Then
            sISODateTime = dParsed.ToString("yyyy-MM-dd HH:mm:ss")
            Return sISODateTime
        Else
            Return vbNullString
        End If
	   
	Catch ex As Exception
		mHandleError(ex)
    End Try

	Return vbNullString
End Function

Public Function mConvertISOToUS(ByVal sISODateTime As String) As String
    Dim dParsed As DateTime
    Dim sUSDateTime As String

    Try
        If DateTime.TryParseExact(sISODateTime, "yyyy-MM-dd HH:mm:ss", Globalization.CultureInfo.InvariantCulture, Globalization.DateTimeStyles.None, dParsed) Then
            sUSDateTime = dParsed.ToString("M/d/yyyy h:mm:ss tt")
            Return sUSDateTime
        Else
            Return vbNullString
        End If

	Catch ex As Exception
		mHandleError(ex)
    End Try

		Return vbNullString
End Function

Public Sub mLogMessage( _
	ByVal sMessage As String)

            
            Dim sSQL As String
            Dim iFileNum As Integer
            Dim sLogFilePath As String
            Dim sISODateTime As String
		  Dim sTimeStamp As String
		  Dim sOutputString As String
		  Dim oDatabase As cDatabase = Nothing
		  Dim oWriter As StreamWriter = Nothing

        Try
            oDatabase = New cDatabase(gConnectionString)


            ' Set the path for the log file
            sLogFilePath = My.Application.Info.DirectoryPath & "\app.log"
            sISODateTime = mConvertUSDateTimeToISO(Now.ToString("MM/dd/yyyy hh:mm:ss tt"))

            ' Get a free file number
            iFileNum = FreeFile()

		  If mLenTrim(sMessage) > 0 Then
			   sTimeStamp = "[" & sISODateTime & "] "
			   sOutputString = sTimeStamp & ": " & sMessage 
			   sMessage = sMessage.Replace("'", "''")
			  sSQL = "INSERT INTO tb_Message_Log (Message_Date_Time, Message_Text) VALUES ("
				 sSQL = sSQL & mSingleQuotes(sISODateTime) & ", " & mSingleQuotes(sOutputString) & ")"


			  oDatabase.mExecuteSQL(sSQL)
		   End If

        Catch ex As Exception
            MessageBox.Show("mLogMessage: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
	   Finally
		If Not IsNothing(oWriter) Then
			oWriter = Nothing
		End If
		If Not IsNothing(oDatabase) Then
			oDatabase = Nothing
		End If
        End Try
    End Sub

Public Sub mResizeDataGridViewFixedOffset( _
    oForm As System.Windows.Forms.Form, _
    oGridView As System.windows.Forms.DataGridView, _
    Optional ByVal iTopOffset As Integer = 50)

    Dim iFormWidth As Integer
    Dim iFormHeight As Integer

    Try
        iFormWidth = oForm.ClientSize.Width
        iFormHeight = oForm.ClientSize.Height

        With oGridView
            .Top = iTopOffset
            .Left = 0
            .Width = iFormWidth
            .Height = iFormHeight - iTopOffset
        End With

	Catch ex As Exception
		mHandleError(ex)
    End Try
End Sub


Public Function mHandleError(oError As Exception) As Integer

		
  		

    Try
    
		Dim iErrorNumber As Integer 
		Dim iReturnResult As Integer
		Dim iLineNumber As Integer
		Dim iMethodNameCounter As Integer
		Dim sCallerMethod As String
		Dim sErrorMessage As String
		Dim sFileName As String = ""
		Dim sOutputString As String
		Dim oStackTrace As System.Diagnostics.StackTrace
		Dim oMethod As Reflection.MethodBase

		iReturnResult = 0
		sCallerMethod = "Method Unknown"
		oStackTrace = New System.Diagnostics.StackTrace(oError, True)

		

		If oError.InnerException IsNot Nothing Then
			sCallerMethod = oError.InnerException.StackTrace
		Else
			sCallerMethod = oError.StackTrace
		End If


		If InStr(sCallerMethod, "(") > 0 Then
			sCallerMethod = sCallerMethod.Substring(6, Instr(sCallerMethod, "(") - 7)
		End if

		iErrorNumber = oError.HResult
		sErrorMessage = oError.Message
		
		If oError.HResult = &H80040E57 Then
			sErrorMessage = sErrorMessage & " This error most likely occurred because the First Cell of the Response Object needs a very long string in it.  "
			sErrorMessage = sErrorMessage & " The easiest way to do this is to copy the Request Object to the Response Object in the first row of your template spreadsheet."
		End If
		
		MessageBox.Show(sCallerMethod & " Error Number:" & iErrorNumber & "-" & sErrorMessage, "Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error)
		sCallerMethod  = sCallerMethod & iErrorNumber.ToString() & ":"

		If oError.InnerException IsNot Nothing Then
			sOutputString = sCallerMethod & oError.Message & Chr(13) & Chr(13) & oError.InnerException.StackTrace
		Else
			sOutputString  = sCallerMethod & oError.Message & Chr(13) & Chr(13) & oError.StackTrace
		End if
		mLogMessage(sOutputString)


		mSendTeamsMessage(sOutputString)
		iReturnResult = 1

		Return iReturnResult

    Catch ex As Exception

        MessageBox.Show("mHandleError: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

    Finally

    End Try

    
End Function



   Public Sub mOpenSQLiteAdmin(sDatabaseName As String)

    Dim sAppPath As String
    Dim sDBPath As String
    Dim oStartInfo As ProcessStartInfo

    Try
        sAppPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SQLiteDBAdmin.exe")
        sDBPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sDatabaseName)

        oStartInfo = New ProcessStartInfo(sAppPath)
        oStartInfo.Arguments = """" & sDBPath & """"

        Process.Start(oStartInfo)

	Catch ex As Exception
		mHandleError(ex)
    End Try

End Sub
    Public Function Base64Encode(text As String) As String
        Dim arrData As Byte() = Encoding.Default.GetBytes(text)
        Return Convert.ToBase64String(arrData)
    End Function


Public Function mBackupSQLiteDatabase(ByVal sBackupFolderName As String, ByVal sDatabaseName As String) As Boolean
    Dim sAppPath As String
    Dim sBackupPath As String
    Dim sSourcePath As String
    Dim sTimestamp As String
    Dim sBackupFileName As String
    Dim sBackupFullPath As String
    Dim arrFiles As String()
    Dim sFile As String
    Dim dFileDate As Date
    Dim iDaysOld As Integer

    Try

		Dim sUserName As String

		sUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name

		sUserName = Replace(sUserName, "\", "")

        ' Get application directory
        sAppPath = AppDomain.CurrentDomain.BaseDirectory
        

        ' Build full backup folder path
        sBackupPath = Path.Combine(sAppPath, sBackupFolderName)

        ' Ensure backup folder exists
        If Not Directory.Exists(sBackupPath) Then
            Directory.CreateDirectory(sBackupPath)
        End If

        ' Build source database path
        sSourcePath = Path.Combine(sAppPath, sDatabaseName)

        ' Create timestamp
        sTimestamp = Format(Now, "MM-dd-yyyy_hh.mm tt")

        ' Build backup file name
        sBackupFileName = "Bak_" & sUserName & "_" & sDatabaseName & "_" & sTimestamp & ".db"

        ' Build full backup file path
        sBackupFullPath = Path.Combine(sBackupPath, sBackupFileName)

        ' Copy database to backup location
        File.Copy(sSourcePath, sBackupFullPath, True)

        ' Delete old backups (> 3 days)
        arrFiles = Directory.GetFiles(sBackupPath, "Bak" & sDatabaseName & "_*.db")

        For Each sFile In arrFiles
            Try
                dFileDate = File.GetCreationTime(sFile)
                iDaysOld = DateDiff(DateInterval.Day, dFileDate, Now)

                If iDaysOld > 7 Then
                    File.Delete(sFile)
                End If
            Catch ex As Exception
                MessageBox.Show("mBackupSQLiteDatabase (File Loop): " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Next

        Return True

	Catch ex As Exception
		mHandleError(ex)
    End Try

            Return False
End Function






Public Function mSendTeamsMessage(ByVal sMessageText As String, Optional ByVal sWebhookUrl as String = "https://default8ac76c91e7f141ffa89c3553b2da2c.17.environment.api.powerplatform.com:443/powerautomate/automations/direct/workflows/c1e40bf2335842ec87bec79bca7596c8/triggers/manual/paths/invoke?api-version=1&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=f32DRMGcAGGPwFL1OdGiWE1ZrClpMbSm0fT99haDKXI") As Boolean

    ' Declare variables
	Dim oHttpRequest As Object
	Dim sJsonBody As String



	Try
	    sMessageText = System.Security.Principal.WindowsIdentity.GetCurrent().Name & Chr(13) & Chr(13) & sMessageText
	    'sMessageText = JsonSerializer.Serialize(sMessageText)
	    'sMessageText = "{""text"":""" & sMessageText & """}"
         sJsonBody = JsonSerializer.Serialize(New With {.text = sMessageText})

	    oHTTPRequest = CreateObject("WinHttp.WinHttpRequest.5.1")
	    oHttpRequest.Open("POST", sWebhookUrl, False)
	    oHttpRequest.SetRequestHeader("Content-Type", "application/json")
	    oHttpRequest.Send(sJsonBody)


''			There is no test for successful send.  If it fails, then it fails.
'	    If oHttpRequest.Status >= 200 And oHttpRequest.Status < 300 Then
'				MessageBox.Show(oHttpRequest.status)
'			Else
'				MessageBox.Show("Failed to send message. Status: " & oHttpRequest.Status)
'			End If


''			Check response
'	   If oHttpRequest.Status >= 200 And oHttpRequest.Status < 300 Then
'				Return True
'			Else
'				Throw New Exception("mSendTeamsMessage: HTTP Status " & oHttpRequest.Status & vbCrLf & oHttpRequest.ResponseText)
'				Return False
'			End If

	   Catch ex As Exception
		MessageBox.Show("mSendTeamsMessage: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Return False
    End Try

End Function

Public Function mIsValidJson(ByVal sJsonInput As String) As Boolean
    ' Declare variables
    Dim oJsonDocument As JsonDocument
    Dim iIsValid As Integer

    Try
        ' Attempt to parse JSON
        oJsonDocument = JsonDocument.Parse(sJsonInput)

        ' Dispose after validation
        oJsonDocument.Dispose()

        ' Set flag
        iIsValid = 1

    Catch ex As Exception
        ' Optional: comment out to suppress error reporting
        ' mHandleError(ex)

        ' Set flag
        iIsValid = 0
    End Try

    ' Return result
    If iIsValid = 1 Then
        Return True
    Else
        Return False
    End If
End Function


Public Function mFormatJson(ByVal sJsonInput As String) As String
    ' Declare variables
    Dim oJsonDocument As JsonDocument
    Dim sFormattedJson As String = ""
    Dim oWriterOptions As JsonWriterOptions
    Dim oStream As MemoryStream
    Dim oWriter As Utf8JsonWriter
    Dim arrBytes As Byte()

    Try

	   If mIsValidJson(sJsonInput) = True Then

		   ' Configure writer options
		   oWriterOptions = New JsonWriterOptions()
		   oWriterOptions.Indented = True

		   ' Parse input
		   oJsonDocument = JsonDocument.Parse(sJsonInput)

		   ' Create memory stream
		   oStream = New MemoryStream()

		   ' Create writer
		   oWriter = New Utf8JsonWriter(oStream, oWriterOptions)

		   ' Write formatted JSON
		   oJsonDocument.WriteTo(oWriter)

		   ' Flush writer
		   oWriter.Flush()

		   ' Convert to string
		   arrBytes = oStream.ToArray()
		   sFormattedJson = System.Text.Encoding.UTF8.GetString(arrBytes)

		   ' Dispose resources
		   oWriter.Dispose()
		   oStream.Dispose()
		   oJsonDocument.Dispose()

		Else
			sFormattedJson = sJsonInput
		End If

    Catch ex As Exception
        ' Defensive error handling
	   sFormattedJson = sJsonInput
        mHandleError(ex)
    End Try

    	' Return result
	Return sFormattedJson
End Function


	Public Sub CleanupEverything()
        Try

            '' Close any secondary forms
            'For Each f As Form In Application.OpenForms
            '    If Not f.IsDisposed Then f.Close()
            'Next

     Catch ex As Exception
        mHandleError(ex)
    End Try
    End Sub


End Module
