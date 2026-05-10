Imports ADODB
Imports System.ComponentModel
Imports System.Data.SQLite
Imports System.IO
Imports System.Runtime.Intrinsics
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock
Imports Windows.Win32.System





Public Class frmMain

	Public const cstDatabaseName As String = "API_TESTER.DB"
	Private oViewReportForm As frmViewReport
	Public Enum enuSystemSource
        IIB = 1
        ESG = 2
        Adv = 3
    End Enum




	Private Sub mValidatePreExecute()
		Try

			'Validation
			If (cboEnvironment.SelectedIndex = -1 Or cboEnvironment.SelectedIndex = 0) Then
				Throw New Exception("mValidatePreExecute - Please select an environment")
			End If

			If ((cboAPI.SelectedIndex = -1 Or cboAPI.SelectedIndex = 0) And chkALLAPI.Checked = False) Then
				Throw New Exception("mValidatePreExecute - Please indicate that you want to execute All API's or choose a specific API to execute.")
			End If

			If (chkIIBEndpoint.Checked = False And chkESGEndpoint.Checked = False And chkAdvEndpoint.Checked = False) Then
				Throw New Exception("mValidatePreExecute - Please select at least one endpoint to execute an API test on.")
			End If

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub mInitializeExecuteVariables(
		   ByRef iEnvironmentID As Integer,
		   ByRef sEnvironment As String,
		   ByRef iAPIID As Integer,
		   ByRef bAllAPI As Boolean,
		   ByRef bTestIIBEndpoint As Boolean,
		   ByRef bTestESGEndpoint As Boolean,
		   ByRef bTestADVEndpoint As Boolean)

		Try

			'Capture the Variable Configuration
			iEnvironmentID = mGetSelectedComboValue(cboEnvironment)
			sEnvironment = cboEnvironment.Text
			iAPIID = mGetSelectedComboValue(cboAPI)

			If chkALLAPI.Checked = True Then
				bAllAPI = True
			ElseIf chkALLAPI.Checked = False Then
				iAPIID = mGetSelectedComboValue(cboAPI)
			End If

			If chkIIBEndpoint.Checked = True Then
				bTestIIBEndpoint = True
			End If

			If chkESGEndpoint.Checked = True Then
				bTestESGEndpoint = True
			End If

			If chkAdvEndpoint.Checked = True Then
				bTestADVEndpoint = True
			End If

			' If chkUseIPAddress = True Then
			'     bUseIPAddress = True
			' End If

			' If chkUseIPAddress = True Then
			'     bUseIPAddress = True
			' End If

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub mGetRootDNS(
		   ByVal iEnvironmentID As Integer,
		   ByRef sIIBEndpoint As String,
		   ByRef sESGEndpoint As String,
		   ByRef sADVEndpoint As String)


		Dim oDatabase As cDatabase
		Dim oDataTable As DataTable
		Dim sSQL As String

		Try
			sSQL = "SELECT * FROM tb_Environment WHERE Environment_ID = " & iEnvironmentID

			oDatabase = New cDatabase(gConnectionString)
			oDataTable = oDatabase.mGetDataTable(sSQL)

			sIIBEndpoint = "https://" & oDataTable.Rows(0)("IIB_Root_DNS")
			sESGEndpoint = "https://" & oDataTable.Rows(0)("ESG_Root_DNS")
			sADVEndpoint = "https://" & oDataTable.Rows(0)("ADV_Root_DNS")

			oDatabase = Nothing

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Function mProcessEndpointCall(
	    ByVal enuSystem As enuSystemSource,
	    ByVal iLocalRequestType As Integer,
	    ByVal iAPIID As Integer,
	    ByVal sFullURL As String,
	    ByRef sRequestObject As String,
	    ByRef sResponseObject As String,
	    ByRef dStartTime As Date,
	    ByRef dEndTime As Date,
	    ByRef iTimeDifference As Integer) As String

		Dim iResponseCode As Integer

		Try

			'sIIBRequestObject = oRst("IIB_Request_Object")

			If mLenTrim(sRequestObject) > 0 Then

				sRequestObject = mReplaceAttribute(sRequestObject)

				dStartTime = DateTime.Now.ToString("hh:mm:ss tt")
				'mLogMessage("EndpointInCommandExecute " & iAPIID & " " & sIIBEndpoint)
				iResponseCode = mCallEndpoint(enuSystem, iAPIID, iLocalRequestType, sFullURL, sRequestObject, sResponseObject)
				dEndTime = DateTime.Now.ToString("hh:mm:ss tt")


				iTimeDifference = DateDiff("s", dStartTime, dEndTime)

				mProcessEndpointCall = mProcessResponse(enuSystem, sResponseObject, iResponseCode)


			End If

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Function

	Private Function mProcessResponse(ByVal enuSystem As enuSystemSource, ByRef sResponseObject As String, ByVal iResponseCode As Integer) As String

		Dim sSystemDescription As String = ""
		Dim iStringStart As Integer = 0

		Try

			If enuSystem = enuSystemSource.IIB Then
				sSystemDescription = "IIB"
			ElseIf enuSystem = enuSystemSource.ESG Then
				sSystemDescription = "ESG"
			ElseIf enuSystem = enuSystemSource.Adv Then
				sSystemDescription = "ADV"
			End If

			If iResponseCode = 200 Then
				mProcessResponse = sSystemDescription & " Request Successful Response Code: " & iResponseCode.ToString
			ElseIf InStr(sResponseObject, """status"":""OK""") Then
				mProcessResponse = sSystemDescription & " Request Successful"
			ElseIf (enuSystem = enuSystemSource.Adv AndAlso InStr(sResponseObject, "{""Results"":")) Then
				mProcessResponse = sSystemDescription & " Request Successful"
			ElseIf InStr(sResponseObject, """status"":""FAILED""") Then
				mProcessResponse = sSystemDescription & " Request Failed"
			ElseIf InStr(sResponseObject, """errorMessage""") Then
				iStringStart = InStr(sResponseObject, """errorMessage""")
				mProcessResponse = sSystemDescription & " Request Failed"
			ElseIf InStr(sResponseObject, "Error") Then
				mProcessResponse = sSystemDescription & " Request Failed"
			Else
				mProcessResponse = "Unknown Response"
			End If


		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Function



	Private Sub mBuildReportSQL(
		   ByVal eSystemType As enuSystemSource,
		   ByRef sSQL As String,
		   ByVal dTestDateTime As Date,
		   ByVal iEnvironmentID As Integer,
		   ByVal iAPIID As Integer,
		   Optional ByVal sRequestObject As String = "",
		   Optional ByVal sResponseObject As String = "",
		   Optional ByVal dStartTime As String = "",
		   Optional ByVal dEndTime As String = "",
		   Optional ByVal sResponseReport As String = "")


		Dim sUserName As String

		sUserName = System.Security.Principal.WindowsIdentity.GetCurrent().Name
		Try

			If mLenTrim(sSQL) = 0 Then


				sSQL = "INSERT INTO tb_Test_Report "
				sSQL = sSQL & "("
				sSQL = sSQL & "Test_Date_Time, "
				sSQL = sSQL & "Test_Environment_ID, "
				sSQL = sSQL & "Test_API_ID, "
				sSQL = sSQL & "IIB_Request, "
				sSQL = sSQL & "IIB_Response, "
				sSQL = sSQL & "IIB_Start_Time, "
				sSQL = sSQL & "IIB_End_Time, "
				sSQL = sSQL & "IIB_Summary, "
				sSQL = sSQL & "ESG_Request, "
				sSQL = sSQL & "ESG_Response, "
				sSQL = sSQL & "ESG_Start_Time, "
				sSQL = sSQL & "ESG_End_Time, "
				sSQL = sSQL & "ESG_Summary, "
				sSQL = sSQL & "Adv_Request, "
				sSQL = sSQL & "Adv_Response, "
				sSQL = sSQL & "Adv_Start_Time, "
				sSQL = sSQL & "Adv_End_Time, "
				sSQL = sSQL & "Adv_Summary, "
				sSQL = sSQL & "Created_By)"
				sSQL = sSQL & " VALUES ("
				sSQL = sSQL & mSingleQuotes(CStr(mConvertUSDateTimeToISO(dTestDateTime))) & ", "
				sSQL = sSQL & iEnvironmentID & ", "
				sSQL = sSQL & iAPIID & ", "

			Else
				If eSystemType = enuSystemSource.Adv Then
					sSQL = sSQL & mSingleQuotes(sRequestObject) & ", "
					sSQL = sSQL & mSingleQuotes(sResponseObject) & ", "
					If mLenTrim(sRequestObject) > 0 Then
						sSQL = sSQL & mSingleQuotes(CStr(mConvertUSDateTimeToISO(dStartTime))) & ", "
						sSQL = sSQL & mSingleQuotes(CStr(mConvertUSDateTimeToISO(dEndTime))) & ", "
					Else
						sSQL = sSQL & "NULL , "
						sSQL = sSQL & "NULL , "
					End If
					sSQL = sSQL & mSingleQuotes(sResponseReport) & ", "
					sSQL = sSQL & mSingleQuotes(sUserName)
					sSQL = sSQL & ")"
				Else
					sSQL = sSQL & mSingleQuotes(sRequestObject) & ", "
					sSQL = sSQL & mSingleQuotes(sResponseObject) & ", "
					If mLenTrim(sRequestObject) > 0 Then
						sSQL = sSQL & mSingleQuotes(CStr(mConvertUSDateTimeToISO(dStartTime))) & ", "
						sSQL = sSQL & mSingleQuotes(CStr(mConvertUSDateTimeToISO(dEndTime))) & ", "
					Else
						sSQL = sSQL & "NULL , "
						sSQL = sSQL & "NULL , "
					End If
					sSQL = sSQL & mSingleQuotes(sResponseReport)
					sSQL = sSQL & ", "
				End If
				'mLogMessage(sSQL)
			End If

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub


	Public Function mCallEndpoint( _
		 ByVal enuSystem As enuSystemSource, _
		 ByVal iAPIID As Integer, _
		 ByRef iRequestType As Integer, _
		 ByRef sEndpoint As String, _
		 ByRef sRequestObject As String, _
		 ByRef sStatusResponse As String) As Integer

		Dim oRestAPI As WinHttp.WinHttpRequest
		Dim sRequestType As String

		Dim sErrorString As String = ""
		Dim sLogString As String = ""
		Dim sAuthHeader As String

		Try
			oRestAPI = New WinHttp.WinHttpRequest
			oRestAPI.SetTimeouts(90000, 90000, 90000, 90000)

			'sLogString = " System:" & enuSystem & " Request Type:" & eRequestType & " Endpoint:" & sEndpoint & " Request Object: " & sRequestObject
			'mLogMessage(sLogString)

			sAuthHeader = "Basic " & Base64Encode("WWREST:Advantage2019!")

			If iRequestType = cstPost Then
				sRequestType = "POST"
			ElseIf iRequestType = cstGet Then
				sRequestType = "GET"
			Else
				Throw New Exception("Get Or Post is required.")
			End If

			'sLogString = " System:" & enuSystem & " Request Type:" & eRequestType & " Endpoint:" & sEndpoint & " Request Object: " & sRequestObject
			'mLogMessage(sLogString)
			sRequestObject = mFormatJson(sRequestObject)

			If mIsValidJson(sRequestObject) = False Then
				mSendTeamsMessage(iAPIID & " Request Object is not a valid json.")

				Exit Function
			End If
			If enuSystem = enuSystemSource.IIB Or enuSystem = enuSystemSource.ESG Then

				If iRequestType = cstPost Then
					oRestAPI.Open(sRequestType, sEndpoint, False)
					oRestAPI.Send(sRequestObject)
				ElseIf iRequestType = cstGet Then
					oRestAPI.Open(sRequestType, sEndpoint & sRequestObject, False)
					oRestAPI.Send()
				End If

				sStatusResponse = "Status:  " & oRestAPI.Status & vbCrLf & oRestAPI.StatusText & " " & oRestAPI.ResponseText
				mCallEndpoint = oRestAPI.Status
			ElseIf enuSystem = enuSystemSource.Adv Then

				If iRequestType = cstGet Then
					'mLogMessage(sEndpoint & sRequestObject)
					oRestAPI.Open(sRequestType, sEndpoint & sRequestObject, False) 'For Advantage the request object is a querstring in the url
					oRestAPI.SetRequestHeader("Authorization", sAuthHeader)
					oRestAPI.Send()
				ElseIf iRequestType = cstPost Then
					oRestAPI.Open(sRequestType, sEndpoint, False) 'For Advantage the request object is a querstring in the url
					oRestAPI.SetRequestHeader("Authorization", sAuthHeader)
					oRestAPI.Send(sRequestObject)
				End If
				sStatusResponse = "Status:  " & oRestAPI.Status & " " & oRestAPI.StatusText & " " & oRestAPI.ResponseText
				mCallEndpoint = oRestAPI.Status


				'             Dim http As Object
				'   '         Dim sURL As String
				'   '         Set http = CreateObject("WinHttp.WinHttprequest.5.1")
				'   '         sURL = "https://stage2020-advantage.gsdwkglobal.com/advantage-api-WKH-ST2/health"
				'   '         http.Open "Get", sURL, False
				'   '         http.send
				'   '         MsgBox http.responseText
			Else
				Throw New Exception("mCallEndpoint: enuSystem argument is not valid")
			End If

		Catch ex As Exception
			Dim sEnhancedMessage As String = "mCallEndpoint - API ID: " & iAPIID & " — " & ex.Message
			mHandleError(New Exception(sEnhancedMessage, ex))
		End Try
	End Function



	Public Function mReplaceAttribute(sRequestObject As String) As String
		Try

			Dim sReplacementWildcard As String
			Dim sReplacementValue As String
			Dim iRandomNumber As Integer
			Dim iInt As Integer
			Dim snewUUID As String

			Randomize() ' Initializes the random number generator
			iRandomNumber = Int((9000 * Rnd()) + 1000)

			sRequestObject = mReplaceStandardAttribute(sRequestObject)


			'-------------------------------------------------------------------------------------------
			'Go Through the attributes table values replace the request object call wildcardparamters with the values in the table
			'Fields in the Array are Attribute Name and Attribute Value
			' arrAttributes(fieldIndex, recordIndex)
			' So vData(0, 0) Is the first field of the first record
			' UBound(arrAttributes, 1) gives you the number of fields minus one
			' UBound(arrAttributes, 2) gives you the number of records minus one
			'-------------------------------------------------------------------------------------------
			For iInt = 0 To UBound(arrAttributes, 1)
				sReplacementWildcard = "{{" & arrAttributes(iInt, 0) & "}}"
				sReplacementValue = arrAttributes(iInt, 1)
				sRequestObject = sRequestObject.Replace(sReplacementWildcard, sReplacementValue)
			Next

			sRequestObject = sRequestObject.Replace("[RndNbr]", CStr(iRandomNumber))



		Catch ex As Exception
			mHandleError(ex)
		End Try

		mReplaceAttribute = sRequestObject

	End Function


	Private Sub chkESGEndpoint_Click()
		Try

			If chkESGEndpoint.Checked = True Then
				chkAdvEndpoint.Checked = True
			End If

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub


	Private Sub mnuExcelReportPath_Click() Handles mnuExcelReportPath.Click
		Try

			Dim oSetPathForm As New frmSetPath

			oSetPathForm.eSavePathType = enuSavePathType.ExcelReport
			oSetPathForm.ShowDialog()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub


	Private Sub cmdTestAdvantage_Click()
		Try

			Dim http As Object
			Dim sURL As String
			Dim sAuthHeader As String

			http = CreateObject("WinHttp.WinHttprequest.5.1")


			'sURL = "https://stage2020-advantage.gsdwkglobal.com/advantage-api-WKH-ST2/customers/000000018769?includeAllAddresses=true&includeCreditSummary=false"
			sURL = InputBox("Put in URL")
			http.Open("Get", sURL, False)
			sAuthHeader = "Basic " & Base64Encode("WWREST:Advantage2019!")
			http.SetRequestHeader("Authorization", sAuthHeader)
			http.send
			MsgBox("Fourth Attempt (Credentials with Base64 Call ):  " & http.responseText)

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub




	'    Private Sub mBackupDatabase()
	'        Try

	'        Dim sdbPath As String
	'        Dim sbackupPath As String
	'        Dim slastBackupDateStr As String
	'        Dim dlastBackupDate As Date
	'        Dim dcurrentDate As Date
	'        Dim snewBackupFileName As String

	'        sdbPath = "D:\APITracker\API.accdb"
	'        sbackupPath = "D:\APITracker\Backup\"
	'        dcurrentDate = Date

	'        ' ✅ Create backup folder if it doesn't exist
	'        If Dir(sbackupPath, vbDirectory) = "" Then
	'            MkDir(sbackupPath)
	'        End If

	'        ' Retrieve last backup date from registry
	'        slastBackupDateStr = GetSetting("APITracker", "Backup", "LastBackupDate", "")

	'        If slastBackupDateStr <> "" Then
	'            dlastBackupDate = CDate(slastBackupDateStr)
	'        Else
	'            dlastBackupDate = #01-Jan-1900# ' Default to long ago if not found
	'            End If

	'        ' Check if 7 days have passed
	'        If DateDiff("d", dlastBackupDate, dcurrentDate) >= 7 Then
	'            ' Format new filename: API.mm.dd.yyyy.accdb
	'            snewBackupFileName = "API." & Format(dcurrentDate, "mm.dd.yyyy") & ".accdb"

	'            ' Copy the database
	'            FileCopy sdbPath, sbackupPath & snewBackupFileName

	'                ' Update registry with new backup date
	'            SaveSetting "APITracker", "Backup", "LastBackupDate", Format(dcurrentDate, "mm/dd/yyyy")
	'            End If

	'        Exit Sub

	'ErrorHandler:
	'        MsgBox(Err.Number & " - mBackupDatabase: " & Err.Description)
	'    End Sub



	'Private Sub Command2_Click()
	' Dim httpRequest As Object
	' Set httpRequest = CreateObject("MSXML2.XMLHTTP")

	' ' Open the request
	' httpRequest.Open "POST", "https://example.com/api", False

	' ' Set headers
	' httpRequest.setRequestHeader "Content-Type", "application/json"
	' httpRequest.setRequestHeader "Authorization", "Bearer YOUR_ACCESS_TOKEN"

	' ' Prepare the request body
	' Dim requestBody As String
	' requestBody = "{""key1"":""value1"",""key2"":""value2""}"

	' ' Send the request
	' httpRequest.Send requestBody

	' ' Handle the response
	' If httpRequest.Status = 200 Then
	'     MsgBox "Request successful: " & httpRequest.responseText
	' Else
	'     MsgBox "Request failed with status: " & httpRequest.Status
	' End If

	' ' Clean up
	' Set httpRequest = Nothing




	' Private Sub FormatRequestObject()
	'     Dim requestObject As String

	'     ' Constructing a sample JSON-like request object
	'     requestObject = "{ " & vbCrLf & _
	'                     "  ""method"": ""POST""," & vbCrLf & _
	'                     "  ""url"": ""https://api.example.com/resource""," & vbCrLf & _
	'                     "  ""headers"": {" & vbCrLf & _
	'                     "    ""Content-Type"": ""application/json""," & vbCrLf & _
	'                     "    ""Authorization"": ""Bearer YOUR_TOKEN""" & vbCrLf & _
	'                     "  }," & vbCrLf & _
	'                     "  ""body"": {" & vbCrLf & _
	'                     "    ""key1"": ""value1""," & vbCrLf & _
	'                     "    ""key2"": ""value2""" & vbCrLf & _
	'                     "  }" & vbCrLf & _
	'                     "}"

	'     ' Displaying the formatted request object in a textbox
	'     Text1.Text = requestObject
	' End Sub


	'End Sub








	Private Sub mnuLogAnalyzerPath_Click() Handles mnuLogAnalyzerPath.Click
		Try

			Dim oSetPathForm As New frmSetPath

			oSetPathForm.eSavePathType = enuSavePathType.LogAnalyzer
			oSetPathForm.ShowDialog()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub


	Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
		Try

			Dim iEnvironmentID As Integer
			Dim iAttributeID As Integer
			Dim iAPIID As Integer
			Dim sAttributeValue As String

			Dim oDatabase As cDatabase
			Dim sSQL As String

			oDatabase = New cDatabase(gConnectionString)
			iEnvironmentID = mGetSelectedComboValue(cboEnvironment)
			iAttributeID = mGetSelectedComboValue(cboAttribute)
			iAPIID = mGetSelectedComboValue(cboAPI)
			sAttributeValue = Trim(Me.txtAttributeValue.Text)

			sSQL = "UPDATE tb_Attribute_Value SET Attribute_Value = " & mSingleQuotes(sAttributeValue)
			sSQL = sSQL & " WHERE Environment_ID = " & iEnvironmentID
			sSQL = sSQL & " AND Attribute_ID = " & iAttributeID

			If oDatabase.mExecuteSQL(sSQL) > 0 Then
				MessageBox.Show("Succesfully saved request objects for " & cboAPI.Text)
			Else
				MessageBox.Show("No record updated.")
			End If
			'mPopulateAttributeArray(iEnvironmentID)            

			oDatabase = Nothing

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Public Sub cmdExecute_Click(sender As Object, e As EventArgs) Handles cmdExecute.Click

		Try

			mRunTest()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub


	Public Sub mRunTest(Optional bShowPopup As Boolean = True)

		'Declare Variables
		Dim iEnvironmentID As Integer = 0
		Dim sEnvironment As String = ""
		Dim iAPIID As Integer = 0
		Dim dTestDateTime As Date
		Dim bAllAPI As Boolean = False
		Dim bTestIIBEndpoint As Boolean = False
		Dim bTestESGEndpoint As Boolean = False
		Dim bTestAdvEndpoint As Boolean = False
		' Dim bUseIPAddress As Boolean = False
		Dim sIIBEndpoint As String = ""
		Dim sESGEndpoint As String = ""
		Dim sADVEndpoint As String = ""
		Dim sIIBFullURL As String = ""
		Dim sESGFullURL As String = ""
		Dim sADVFullURL As String = ""
		Dim iIIBRequestType As Integer = 0
		Dim iESGRequestType As Integer = 0
		Dim iADVRequestType As Integer = 0
		Dim sIIBRequestObject As String = ""
		Dim sIIBResponseObject As String = ""
		Dim sIIBResponseReport As String = ""
		Dim sESGRequestObject As String = ""
		Dim sESGResponseObject As String = ""
		Dim sESGResponseReport As String = ""
		Dim sAdvRequestObject As String = ""
		Dim sAdvResponseObject As String = ""
		Dim sAdvResponseReport As String = ""
		Dim dIIBStartTime As Date = Date.MinValue
		Dim dESGStartTime As Date = Date.MinValue
		Dim dADVStartTime As Date = Date.MinValue
		Dim dIIBEndTime As Date = Date.MinValue
		Dim dESGEndTime As Date = Date.MinValue
		Dim dADVEndTime As Date = Date.MinValue
		Dim iIIBTimeDifference As Integer = 0
		Dim iESGTimeDifference As Integer = 0
		Dim iADVTimeDifference As Integer = 0
		Dim iStringStart As Integer = 0
		Dim result As DialogResult = DialogResult.None

		'Database Variables
		Dim oDatabase As cDatabase = Nothing
		Dim oDatatable As DataTable = Nothing
		Dim sSQL As String = ""
		Dim sReportInsertSQL As String = ""
		Dim oWaitingForm As New frmInProgress


		Try
			oDatabase = New cDatabase(gConnectionString)

			If oViewReportForm IsNot Nothing Then
				If Not oViewReportForm.IsDisposed AndAlso oViewReportForm.Visible Then
					oViewReportForm.Close()
				End If
				oViewReportForm = Nothing
			End If

			oWaitingForm.Show()
			Application.DoEvents()


			mValidatePreExecute()
			mInitializeExecuteVariables(iEnvironmentID, sEnvironment, iAPIID, bAllAPI, bTestIIBEndpoint, bTestESGEndpoint, bTestAdvEndpoint)
			mGetRootDNS(iEnvironmentID, sIIBEndpoint, sESGEndpoint, sADVEndpoint)
			iAPIID = mGetSelectedComboValue(cboAPI)


			'Complete the endpoint by adding the extension to the root domain/dns retrieved in the above code

			If bAllAPI = False Then
				sSQL = "SELECT * FROM tb_API WHERE API_ID = " & iAPIID
			ElseIf bAllAPI = True Then
				sSQL = "SELECT * FROM tb_API WHERE Include_In_All = 1 AND Active_API = 1"
				If iEnvironmentID = enuEnvironment.Prod Then
					sSQL = sSQL & " AND Production_Enabled = True"
				End If
			End If

			oDatatable = oDatabase.mGetDataTable(sSQL)

			dTestDateTime = Now()
			mPopulateAttributeArray(iEnvironmentID)


			For Each Row As DataRow In oDatatable.Rows


				sIIBFullURL = sIIBEndpoint & Row("IIB_Endpoint_Ext")
				sESGFullURL = sESGEndpoint & Row("ESG_Endpoint_Ext")
				sADVFullURL = sADVEndpoint 'The Adv call is the server root + a url with query string variables.  The extension is added in the mcallendpoint routine
				sAdvRequestObject = Row("ADV_Request_Object")
				sESGRequestObject = Row("ESG_Request_Object")
				sIIBRequestObject = Row("IIB_Request_Object")


				iAPIID = Row("API_ID")

				If (chkIIBEndpoint.Checked = True Or chkESGEndpoint.Checked = True Or chkAdvEndpoint.Checked = True Or bAllAPI = True) Then
					mBuildReportSQL(enuSystemSource.IIB, sReportInsertSQL, dTestDateTime, iEnvironmentID, iAPIID)
				End If

				'Advantage
				If (chkAdvEndpoint.Checked = True And InStr(Ucase(sAdvRequestObject), "UNAVAILABLE") = 0) Then
					iADVRequestType = Row("ADV_Request_Type")
					sAdvResponseReport = mProcessEndpointCall(enuSystemSource.Adv, iADVRequestType, iAPIID, sADVFullURL, sAdvRequestObject, sAdvResponseObject, dADVStartTime, dADVEndTime, iADVTimeDifference)
				ElseIf (chkAdvEndpoint.Checked = True And InStr(Ucase(sAdvRequestObject), "UNAVAILABLE") > 0) Then
					sAdvResponseReport = iAPIID & " is marked as unavailable in the tb_API table."
					sADVFullURL = ""
					sAdvRequestObject = ""
				Else
					sADVFullURL = ""
					sAdvRequestObject = ""
				End If

				'ESG
				If (chkESGEndpoint.Checked = True And InStr(sESGRequestObject, "Unavailable") = 0) Then
					iESGRequestType = Row("ESG_Request_Type")
					sESGResponseReport = mProcessEndpointCall(enuSystemSource.ESG, iESGRequestType, iAPIID, sESGFullURL, sESGRequestObject, sESGResponseObject, dESGStartTime, dESGEndTime, iESGTimeDifference)
				ElseIf (chkESGEndpoint.Checked = True And InStr(sESGRequestObject, "Unavailable") > 0) Then
					sESGResponseReport = iAPIID & " is marked as unavailable in the tb_API table."
					sESGFullURL = ""
					sESGRequestObject = ""
				Else
					sESGFullURL = ""
					sESGRequestObject = ""
				End If

				'IIB
				If (chkIIBEndpoint.Checked = True And InStr(sIIBRequestObject, "Unavailable") = 0) Then
					iIIBRequestType = Row("IIB_Request_Type")
					sIIBResponseReport = mProcessEndpointCall(enuSystemSource.IIB, iIIBRequestType, iAPIID, sIIBFullURL, sIIBRequestObject, sIIBResponseObject, dIIBStartTime, dIIBEndTime, iIIBTimeDifference)
				ElseIf (chkIIBEndpoint.Checked = True And InStr(sIIBRequestObject, "Unavailable") > 0) Then
					sIIBResponseReport = iAPIID & " is marked as unavailable in the tb_API table."
					sIIBFullURL = ""
					sIIBRequestObject = ""
				Else
					sIIBFullURL = ""
					sIIBRequestObject = ""
				End If


				mBuildReportSQL(enuSystemSource.IIB, sReportInsertSQL, dTestDateTime, iEnvironmentID, iAPIID, sIIBRequestObject, sIIBResponseObject, dIIBStartTime, dIIBEndTime, sIIBResponseReport)
				mBuildReportSQL(enuSystemSource.ESG, sReportInsertSQL, dTestDateTime, iEnvironmentID, iAPIID, sESGRequestObject, sESGResponseObject, dESGStartTime, dESGEndTime, sESGResponseReport)
				mBuildReportSQL(enuSystemSource.Adv, sReportInsertSQL, dTestDateTime, iEnvironmentID, iAPIID, sAdvRequestObject, sAdvResponseObject, dADVStartTime, dADVEndTime, sAdvResponseReport)

				oDatabase.mExecuteSQL(sReportInsertSQL)
				sReportInsertSQL = ""

				'Reset Advantage Variables
				sAdvRequestObject = ""
				sAdvResponseObject = ""
				dADVStartTime = DateTime.MinValue
				dADVEndTime = DateTime.MinValue
				sAdvResponseReport = ""

				'Reset ESG Variables
				sESGRequestObject = ""
				sESGResponseObject = ""
				dESGStartTime = DateTime.MinValue
				dESGEndTime = DateTime.MinValue
				sESGResponseReport = ""

				'Reset IIB Variables
				sIIBRequestObject = ""
				sIIBResponseObject = ""
				dIIBStartTime = DateTime.MinValue
				dIIBEndTime = DateTime.MinValue
				sIIBResponseReport = ""


			Next

			Cursor.Current = Cursors.Default

			oWaitingForm.Close()

			If bShowPopup = True Then
				result = MessageBox.Show("Test Complete.  Do you want to view the results", "View Test Results?", MessageBoxButtons.YesNo)
				If result = DialogResult.Yes Then
					frmViewReport.Show()
				End If
			End IF

		Catch ex As Exception
			mHandleError(ex)
		Finally
			If Not IsNothing(oDatabase) Then
				oDatabase = Nothing
			End If
		End Try

		If oWaitingForm IsNot Nothing AndAlso Not oWaitingForm.IsDisposed Then
			oWaitingForm.Close()
		End If
	End Sub


	Private Sub mnuOpenSOP_Click(sender As Object, e As EventArgs) Handles mnuOpenSOP.Click
		Try

			mOpenSharePointLink()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub



	Private Sub mnuOpenLogAnalyzer_Click(sender As Object, e As EventArgs) Handles mnuOpenLogAnalyzer.Click
		Try

			Dim sPath As String

			sPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Adv-log-analyzer.ps1")

			If File.Exists(sPath) = False Then

				sPath = My.Settings.LogAnalyzerPath
				If mLenTrim(sPath) = 0 Then
					Throw New Exception("Log Analyzer path is not set. Please configure using the Set menu in this application.")
				End If
			End If

			Dim psi As New ProcessStartInfo()
			psi.FileName = "powershell.exe"
			psi.Arguments = "-NoExit -ExecutionPolicy Bypass -File """ & sPath & """"
			psi.UseShellExecute = True
			psi.WindowStyle = ProcessWindowStyle.Normal

			Process.Start(psi)

		Catch ex As Exception
			mHandleError(ex)
		End Try

	End Sub

	Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Try
			'Look at the shown event for the actual execution code
			'Me.Visible = False
			Me.SuspendLayout()

			Me.Activate()
			Me.BringToFront()
			Me.Focus()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub




	Private Sub cboEnvironment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboEnvironment.SelectedIndexChanged
		Try
			Dim sSecurityCode As String
			Dim iEnvironmentID As Integer
			'Dim combo = DirectCast(sender, ComboBox)

			If (cboEnvironment.SelectedIndex = -1 Or cboEnvironment.SelectedItem Is Nothing Or cboEnvironment.SelectedValue Is Nothing Or cboEnvironment.SelectedIndex = 0) Then

				Me.cboAPI.SelectedIndex = -1
				Me.cboAPI.Enabled = False

				Me.TxtAdvRequestObject.Text = ""
				Me.TxtESGRequestObject.Text = ""
				Me.TxtIIBRequestObject.Text = ""
				Me.TxtAdvRequestObject.Enabled = False
				Me.TxtESGRequestObject.Enabled = False
				Me.TxtIIBRequestObject.Enabled = False

				Me.btnSaveRequestObject.Enabled = False
				Me.cboAttribute.SelectedIndex = -1
				Me.cboAttribute.Enabled = False
				Me.txtAttributeValue.Text = ""
				Me.txtAttributeValue.Enabled = False
				Me.chkALLAPI.enabled = False
				Me.cmdSave.Enabled = False
				Me.cmdExecute.Enabled = false
			End If
			If CInt(cboEnvironment.SelectedValue) = enuEnvironment.Prod Then
				sSecurityCode = InputBox("Please enter security code to access prod credentials", "Production Access Requested")

				If sSecurityCode <> "9361" Then
					cboAPI.Items.Clear()
					cboEnvironment.SelectedIndex = -1
					Throw New Exception("Access Denied")
				End If
			End If


			If (cboEnvironment.SelectedIndex <> -1 And cboEnvironment.SelectedIndex <> 0) Then
				Me.cboAPI.Enabled = True
				iEnvironmentID = mGetSelectedComboValue(cboEnvironment)
				mLoadComboBox(cboAPI, iEnvironmentID)
				'mPopulateAttributeArray(iEnvironmentID)
				Me.chkALLAPI.enabled = True
			End If


		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub cboAttribute_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboAttribute.SelectedIndexChanged
		Dim iEnvironmentID As Integer
		Dim iAttributeID As Integer
		Dim sAttributeValue As String

		Dim oDatabase As cDatabase
		Dim oDatatable As DataTable
		Dim sSQL As String
		Try

			oDatabase = New cDatabase(gConnectionString)
			iEnvironmentID = mGetSelectedComboValue(cboEnvironment)
			iAttributeID = mGetSelectedComboValue(cboAttribute)

			sSQL = "SELECT Attribute_Value FROM tb_Attribute_Value"
			sSQL = sSQL & " WHERE Environment_ID = " & iEnvironmentID
			sSQL = sSQL & " AND Attribute_ID = " & iAttributeID

			oDatatable = oDatabase.mGetDataTable(sSQL)

			If oDatatable.Rows.Count = 1 Then
				sAttributeValue = oDatatable.Rows(0)("Attribute_Value")
			Else
				sAttributeValue = ""
			End If

			Me.txtAttributeValue.Text = sAttributeValue
			cmdSave.Enabled = True

			oDatabase = Nothing

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub cboAPI_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboAPI.SelectedIndexChanged
		Try

			Dim oDatabase As cDatabase
			Dim oDataTable As DataTable
			Dim sSQL As String
			Dim sIIBRequestObject As String
			Dim sESGRequestObject As String
			Dim sADVRequestObject As String



			If (cboAPI.SelectedIndex <> -1 And cboAPI.SelectedIndex <> 0) Then

				'Force An Error
				'Dim arr() As Integer = {1, 2, 3}
				'Dim val As Integer = arr(5)  ' Runtime error: Index out of bounds

				If (cboEnvironment.SelectedIndex = -1 Or cboEnvironment.SelectedIndex = 0) Then
					Me.cboAttribute.Enabled = False
					Me.txtAttributeValue.Enabled = False
					Me.btnSaveRequestObject.Enabled = False
					Throw New Exception("Please select an environment before selecting an API.")
				Else
					Me.cboAttribute.Enabled = True
					Me.btnSaveRequestObject.Enabled = True
					Me.TxtAdvRequestObject.Enabled = true
					Me.TxtESGRequestObject.Enabled = true
					Me.TxtIIBRequestObject.Enabled = true
					Me.txtAttributeValue.Enabled = True
					Me.cmdExecute.enabled = True
				End If


				If chkALLAPI.Checked = True Then
					chkALLAPI.Checked = False
				End If

				sSQL = "SELECT IIB_Request_Object, ESG_Request_Object, Adv_Request_Object FROM tb_API WHERE API_ID = " & mGetSelectedComboValue(cboAPI)

				oDatabase = New cDatabase(gConnectionString)
				oDataTable = oDatabase.mGetDataTable(sSQL)


				'Populate variables to hold Request Objects
				sIIBRequestObject = oDatatable.Rows(0)("IIB_Request_Object")
				sESGRequestObject = oDatatable.Rows(0)("ESG_Request_Object")
				sADVRequestObject = oDatatable.Rows(0)("ADV_Request_Object")

				'Populate the textboxes with the request objects
				txtIIBRequestObject.Text = mFormatJson(sIIBRequestObject)
				txtESGRequestObject.Text = mFormatJson(sESGRequestObject)
				TxtAdvRequestObject.Text = mFormatJson(sADVRequestObject)


				'---------------------------------------------------
				'Load the attribute combo box
				'---------------------------------------------------
				If mLoadComboBox(cboAttribute, cboEnvironment.SelectedIndex) = 1 Then
					Me.TxtAdvRequestObject.Enabled = true
					Me.TxtESGRequestObject.Enabled = true
					Me.TxtIIBRequestObject.Enabled = true
				End If


				cmdExecute.Enabled = True
				oDatabase = Nothing
			Else
				Me.TxtAdvRequestObject.Text = ""
				Me.TxtESGRequestObject.Text = ""
				Me.TxtIIBRequestObject.Text = ""
				Me.TxtAdvRequestObject.Enabled = False
				Me.TxtESGRequestObject.Enabled = False
				Me.TxtIIBRequestObject.Enabled = False
				Me.btnSaveRequestObject.Enabled = False
				Me.cboAttribute.SelectedIndex = -1
				Me.cboAttribute.Enabled = False
				Me.txtAttributeValue.Text = ""
				Me.txtAttributeValue.Enabled = False

				If me.chkallapi.Checked = False Then
					Me.cmdExecute.Enabled = False
				End if
			End If

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

	End Sub

	Private Sub frmMain_CausesValidationChanged(sender As Object, e As EventArgs) Handles Me.CausesValidationChanged

	End Sub

	Private Sub frmMain_Closed(sender As Object, e As EventArgs) Handles Me.Closed


		Try
			System.Windows.Forms.Application.Exit()
		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub mnuDSNName_Click(sender As Object, e As EventArgs) Handles mnuDSNName.Click
		Try

			Dim oSetPathForm As New frmSetPath

			oSetPathForm.eSavePathType = enuSavePathType.DSNName
			oSetPathForm.ShowDialog()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub OpenGridReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenGridReportToolStripMenuItem.Click
		Try

			oViewReportForm = New frmViewReport()

			oViewReportForm.Show()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub frmMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
		Try

			Dim dAppDateTimestamp As DateTime

			Me.Visible = False
			Me.SuspendLayout()

			dAppDateTimestamp = System.IO.File.GetLastWriteTime(Application.ExecutablePath)

			gConnectionString = "Data Source=" & Application.StartupPath & cstDatabaseName & ";Version=3;"
			Me.SSVersion.Text = "Version Date and Time: " & dAppDateTimestamp.ToString
			mBackupSQLiteDatabase("APITesterBackup", cstDatabaseName)

			mLoadComboBox(Me.cboEnvironment)

			Me.Visible = True
			Me.Activate()
			Me.BringToFront()
			Me.Focus()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub cmdExecute_DragDrop(sender As Object, e As DragEventArgs) Handles cmdExecute.DragDrop

	End Sub

	Private Sub frmMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

		Try


			CleanupEverything()
			Application.Exit()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub OpenDBAdminToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles mnuOpenDBAdmin.Click
		Try

			mOpenSQLiteAdmin(cstDatabaseName)

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub chkALLAPI_CheckedChanged(sender As Object, e As EventArgs) Handles chkALLAPI.CheckedChanged
		Try

			Dim oCheckBox As CheckBox

			oCheckbox = CType(sender, CheckBox)

			If oCheckBox.Checked = True Then
				If cboEnvironment.SelectedIndex > 0 Then
					cboAPI.SelectedIndex = 0
					cboAPI.Enabled = False
					cmdExecute.Enabled = True
				End If
			Elseif oCheckBox.Checked = False Then
				If cboAPI.SelectedIndex <= 0 Or cboEnvironment.SelectedIndex <= 0 Then
					cmdExecute.Enabled = False
					If cboEnvironment.SelectedIndex <= 0 Then
						cboAPI.Enabled = False
					Else
						cboAPI.Enabled = True
					End If
				End If
			End If

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub btnSaveRequestObject_Click(sender As Object, e As EventArgs) Handles btnSaveRequestObject.Click
		Try

			Dim iAPIID As Integer

			Dim oDatabase As cDatabase
			Dim sSQL As String
			Dim sIIBRequestObject As String
			Dim sESGRequestObject As String
			Dim sADVRequestObject As String

			oDatabase = New cDatabase(gConnectionString)

			If cboAPI.SelectedIndex <> -1 And cboAPI.SelectedIndex <> 0 Then
				iAPIID = mGetSelectedComboValue(cboAPI)
				sIIBRequestObject = Trim(txtIIBRequestObject.Text)
				sESGRequestObject = Trim(txtESGRequestObject.Text)
				sADVRequestObject = Trim(TxtAdvRequestObject.Text)
			Else
				Throw New Exception("An API must be selected in order or to update request objects.")
			End If

			sSQL = "UPDATE tb_API SET IIB_Request_Object = " & mSingleQuotes(sIIBRequestObject) & ", "
			sSQL = sSQL & "ESG_Request_Object = " & mSingleQuotes(sESGRequestObject) & ", "
			sSQL = sSQL & "ADV_Request_Object = " & mSingleQuotes(sADVRequestObject)
			sSQL = sSQL & " WHERE API_ID = " & iAPIID


			If oDatabase.mExecuteSQL(sSQL) > 0 Then
				MessageBox.Show("Succesfully saved request objects for " & cboAPI.Text)
			Else
				MessageBox.Show("No record updated.")
			End If

			oDatabase = Nothing

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub PollingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PollingToolStripMenuItem.Click
		Try


			If (Me.cboEnvironment.SelectedIndex = -1 OR Me.cboEnvironment.SelectedIndex = 0) Then
				Throw New Exception("No Environment selected.  Please setup the main form with the test that you want to run in this scheduler.")
			Elseif ((Me.cboAPI.SelectedIndex = -1 OR Me.cboAPI.SelectedIndex = 0) AND Me.chkALLAPI.Checked = False) Then
				Throw New Exception("No API Selected either individual or all API's.  Please setup the main form with the test that you want to run in this scheduler.")
			End If

			frmPolling.ShowDialog()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs)
		mSendTeamsMessage("My Message")
	End Sub

	Private Sub OpenAPIQAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenAPIQAToolStripMenuItem.Click
		Try

			Dim oViewAPIQAForm = frmAPIQA

			oViewAPIQAForm = New frmAPIQA()

			oViewAPIQAForm.Show()

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub
End Class
