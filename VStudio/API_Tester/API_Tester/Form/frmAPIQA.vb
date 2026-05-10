'Imports Microsoft.Office.Interop
Imports ADODB
Imports Microsoft.Data.SqlClient
Imports Microsoft.Office.Interop
Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Newtonsoft.Json.Linq



Public Class frmAPIQA

	Public sSpreadsheetOutputPath As String
	Private sExcelFilePath As String
	Private sDatabaseDNSString As String 
	Private sDBName As String
	Private sDatabaseUserName = "advviewer"
	Private sDatabasePassword = "Password1"

	Private Sub btnExecuteTest_Click(sender As Object, e As EventArgs) Handles btnExecuteTest.Click

		Dim lstQATest As List(Of cQATest)
		Dim lstAdvantageMapping As List(Of cAdvantageMapping)
		Dim oFileDialog As OpenFileDialog
		Dim oWaitingForm As New frmInProgress

		Try

				lstAdvantageMapping = New List(Of cAdvantageMapping)

				If Me.cboEnvironment.SelectedItem = "QA" Then
					sDBName = "AdvQAUpg2020"
				ElseIf Me.cboEnvironment.SelectedItem = "Stage" Then
					sDBName = "AdvStgUpg2020"
				Else
					Throw New Exception("Environment must be selected.")
				End If


			oFileDialog = New OpenFileDialog()

				oFileDialog.Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|All Files (*.*)|*.*"
				oFileDialog.InitialDirectory = Application.StartupPath

				If oFileDialog.ShowDialog() = DialogResult.OK Then

					sExcelFilePath = oFileDialog.FileName

					If mIsFileOpen(sExcelFilePath) = False Then

						oWaitingForm.Show()
						Application.DoEvents()

						lstQATest = mGetSpreadsheetValues(sExcelFilePath)

						mRunTest(lstQATest)
					
						If mCreateOutputSpreadsheet(sExcelFilePath) = False Then
							Throw New Exception("Output spreadsheet was not successfully created.")
						End If
						mUpdateSpreadsheetValues(lstQATest, Trim(Me.txtExcelOutputFilePath.Text))
						oWaitingForm.Close
					Else
						Messagebox.Show("This file is already open.  Please close the file and try again.  If you do not have the file open in Excel, please do a ctrl-alt delete and end the Excel process.  Then try again.")
					End If
				End If
			

			lstQATest = Nothing
			lstAdvantageMapping = Nothing
			oWaitingForm = Nothing

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub mRunTest(ByRef lstQATest As List(Of cQATest)) 

		
		Dim sEndpoint As String
		Dim sRequestType As String
		Dim sSRNumber As String
		Dim sRequestObject As String
		Dim sResponseObject As String
		Dim iRequestType As integer
		Dim iStatusResponse As Integer
		Dim oAdvantageMapping As cAdvantageMapping

		Try

			For Each oQATest As cQATest In lstQATest

				sRequestType = oQATest.RequestType
				If sRequestType.ToUpper = "POST" Then
					iRequestType = cstPOst
				ElseIf sRequestType.ToUpper = "GET" Then
					iRequestType = cstGet
				End If

				sEndpoint = oQATest.QAEndpoint
				If Me.cboEnvironment.SelectedItem = "Stage" Then
					sEndpoint = Replace(sEndpoint.tolower(), "qa-extnahlt.cdn.wkgbssvcs.com","stg-extnahlt.cdn.wkgbssvcs.com")
				End If
				
				sSRNumber = oQATest.SRNumber
				sRequestObject = oQATest.Request
				sRequestObject = mReplaceStandardAttribute(sRequestObject)

				iStatusResponse = frmMain.mCallEndpoint(frmMain.enuSystemSource.IIB, -1, iRequestType, sEndpoint, sRequestObject, sResponseObject)

				oQATest.Response = sResponseObject
				oQATest.ResponseStatusCode = Convert.ToString(iStatusResponse)

				'This will retrieve and populate all the mapped values into the oQATest class
				oAdvantageMapping = New cAdvantageMapping
				mGetAdvantageMappingValue(oQATest, oAdvantageMapping)
				oAdvantageMapping = Nothing

			Next

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub
	
Private Function mUpdateSpreadsheetValues(ByRef lstQATest As List(Of cQATest), ByVal sExcelFilePath As String) as Boolean

		Dim oConn As ADODB.Connection
		Dim sConnectionString As String
		Dim sSQL As String
		Dim sResponseObject As String
		Dim sStatusCode As String
		Dim sAdvantageResponse As String
		Dim bSuccessful As Boolean = False

		Msgbox(sExcelFilePath)
		sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & sExcelFilePath & ";Extended Properties='Excel 12.0 Xml;HDR=YES';"

		oConn = New ADODB.Connection
		oConn.Open(sConnectionString)
		
		Try
			For Each oQATest As cQATest In lstQATest

				sResponseObject = oQATest.Response
				sStatusCode = oQATest.ResponseStatusCode
				sAdvantageResponse = oQAtest.AdvantageResult

				sSQL = "UPDATE [Sheet1$] "
					sSQL = sSQL & " SET [Response] = '" & sResponseObject & "', "
					sSQL = sSQL & " [StatusCode] = " & mSingleQuotes(sStatusCode) & ", "
					sSQL = sSQL & " [Advantage Response] = " & mSingleQuotes(sAdvantageResponse)
				sSQL = sSQL & " WHERE [Sr No] = " & oQATest.SRNumber
				
				oConn.Execute(sSQL)

			Next
			
			Return bSuccessful

		Catch ex As Exception
			mHandleError(ex)
			bSuccessful = False

		Finally
			If oConn IsNot Nothing Then
				If oConn.State = ADODB.ObjectStateEnum.adStateOpen Then oConn.Close()
				oConn = Nothing
			End If
		End Try

		Return bSuccessful

	End Function

	Private Sub mGetAdvantageDBValue(ByRef oQATest As cQATest, ByRef oAdvantageMapping As cAdvantageMapping)
	    Dim oConn As SqlConnection
	    Dim oAdapter As SqlDataAdapter
	    Dim oDataSet As DataSet
	    Dim sConnString As String
	    Dim sSQL As String

	    Dim sSRNumber As String
	    Dim sRequest As String
	    Dim sTableName As String
	    Dim sFieldToRetrieve As String
	    Dim sKeyField As String
	    Dim sKeyValue As String
	    Dim sResult As String
	    Dim sDelimiter As String

	    Try
		   sSRNumber = oAdvantageMapping.SRNumber
		   sTableName = oAdvantageMapping.TableName
		   sFieldToRetrieve = oAdvantageMapping.FieldToRetrieve
		   sKeyField = oAdvantageMapping.TableKey
		   sRequest = oQATest.Request
		   sResult = oQATest.AdvantageResult
		   sKeyValue = mGetKeyValueFromRequestObject(oQATest, oAdvantageMapping.Attribute)

		   sConnString = "Data Source=" & sDatabaseDNSString & ";Initial Catalog=" & sDBName & ";User ID=" & sDatabaseUserName & ";Password=" & sDatabasePassword & ";TrustServerCertificate=True;"
        
		   ' Use parameter placeholder
		   sSQL = "SELECT " & sFieldToRetrieve & " FROM " & sTableName & " WHERE " & sKeyField & " = @KeyValue"

		   oConn = New SqlConnection(sConnString)
		   oDataSet = New DataSet()

		   ' Create adapter with parameterized command
		   oAdapter = New SqlDataAdapter(sSQL, oConn)
		   oAdapter.SelectCommand.Parameters.AddWithValue("@KeyValue", sKeyValue)

		   oAdapter.Fill(oDataSet, sTableName)

		   If oDataSet.Tables(sTableName).Rows.Count = 1 Then
			  If Len(Trim(sResult)) > 0 Then
				 sDelimiter = " - Next "
			  Else
				 sDelimiter = ""
			  End If
			  'sResult = sResult & sDelimiter & "Field " & sFieldToRetrieve & " in Table " & sTableName
			  'sResult = sResult & " = " & oDataSet.Tables(sTableName).Rows(0)(sFieldToRetrieve).ToString()
			  'sResult = sResult & " For " & sKeyField & " = " & sKeyValue
			  'sResult = sResult & sDelimiter & "Field Value for Field: " & sFieldToRetrieve & " is " & oDataSet.Tables(sTableName).Rows(0)(sFieldToRetrieve).ToString() 
			  'sResult = sResult & " in Table: " & sTableName
			  'sResult = sResult & " For KeyField: " & sKeyField & " = " & sKeyValue
			  sResult = sResult & sDelimiter & "Field Name: " & sFieldToRetrieve & " Field Value: " & oDataSet.Tables(sTableName).Rows(0)(sFieldToRetrieve).ToString()
			  sResult = sResult & " Table Name: " & sTableName
			  sResult = sResult & " Key Field Name: " & sKeyField & " Key Field Value: " & sKeyValue
			  oQATest.AdvantageResult = sResult

		   ElseIf oDataSet.Tables(sTableName).Rows.Count > 1 Then
			  oQATest.AdvantageResult = "Error during retrieval.  More than 1 row returned."
		   ElseIf oDataSet.Tables(sTableName).Rows.Count = 0 Then
			  oQATest.AdvantageResult = "No rows returned."
		   End If

	    Catch ex As Exception
		   mHandleError(ex)
	    End Try
	End Sub

	Function mIsFileOpen(sFilePath As String) As Boolean

	    Dim oFileStream As FileStream

	    Try
		   oFileStream = File.Open(sFilePath, FileMode.Open, FileAccess.Read, FileShare.None)
		   Return False ' success means not open elsewhere

	    Catch ex As IOException
		   Return True ' IOException means file is locked (likely open in Excel)
	    Finally
		   If oFileStream IsNot Nothing Then oFileStream.Close()
	    End Try

	End Function
	
	Private Function mGetKeyValueFromRequestObject(ByRef oQATest As cQATest, sAttributeName As String) As String
	    Try
		   Dim sRequestObject As String = oQATest.Request
		   Dim oJSON As JObject = JObject.Parse(sRequestObject)

		   ' Find the first property with the given name
		   Dim oProp As JProperty = oJSON.Descendants() _
			  .OfType(Of JProperty)() _
			  .FirstOrDefault(Function(p) p.Name.Equals(sAttributeName, StringComparison.OrdinalIgnoreCase))

		   If oProp IsNot Nothing Then
			  Return oProp.Value.ToString()
		   Else
			  Return ""
		   End If

	    Catch ex As Exception
		   mHandleError(ex)
		   Return ""
	    End Try
	End Function
	
	Private Function mGetAdvantageMappingValue(ByRef oQATest as cQATest, ByRef oAdvantageMapping As cAdvantageMapping)
		'Attribute:isbnNumber|TableName:CDSITM_M|TableKey:ISBN_EAN|FieldToRetrieve:ITS_CDE^Attribute:isbnNumber2|TableName:CDSITM_M2|TableKey:ISBN_EAN2|FieldToRetrieve:ITS_CDE2

			
			Dim sSRNumber As String
			Dim sAdvantageMapping As String
			Dim iCarrotCount As Integer
			Dim iIndexCount As integer

		Try

			sSRNumber = oQATest.SRNumber
			sAdvantageMapping = oQATest.AdvantageMapping

			If Len(Trim(sAdvantageMapping)) > 0 Then
				iCarrotCount = sAdvantageMapping.Split("^"c).Length

				For iIndexCount = 0 To (iCarrotCount - 1)
					mProcessAdvantageMappingIntoClass(iIndexCount, oAdvantageMapping, oQATest.SRNumber, sAdvantageMapping)
					mGetAdvantageDBValue(oQATest, oAdvantageMapping)
				Next
			End If


		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Function

	Private Sub mProcessAdvantageMappingIntoClass(ByVal iIndexCount As Integer, ByRef oAdvantageMapping As cAdvantageMapping, ByVal sSRNumber As String, ByVal sFullAdvantageMapping as String)

			'Attribute:isbnNumber|TableName:CDSITM_M|TableKey:ISBN_EAN|FieldToRetrieve1:ITS_CDE^Attribute:isbnNumber2|TableName:CDSITM_M2|TableKey:ISBN_EAN2|FieldToRetrieve1:ITS_CDE2
			'------------------------------------------------------
			Dim arrAdvantageMappingIndividualParts() As String
			Dim oGetPropertyNameFromClass As System.Reflection.PropertyInfo
			Dim sPropertyName As String
			Dim sPropertyValue As String
		Try

			oAdvantageMapping.SRNumber = sSRNumber

			Dim arrAdvantageFullMappingSection() As String = sFullAdvantageMapping.Split("^"c)

			For Each sEachAdvantageMappingElement As String In arrAdvantageFullMappingSection(iIndexCount).Split("|"c)

				arrAdvantageMappingIndividualParts = sEachAdvantageMappingElement.Split(":"c)

				If arrAdvantageMappingIndividualParts.Length = 2 Then

					sPropertyName = arrAdvantageMappingIndividualParts(0)   
					sPropertyValue = arrAdvantageMappingIndividualParts(1) 
					oGetPropertyNameFromClass = oAdvantageMapping.GetType().GetProperty(sPropertyName)

					If oGetPropertyNameFromClass IsNot Nothing AndAlso oGetPropertyNameFromClass.CanWrite Then
						oGetPropertyNameFromClass.SetValue(oAdvantageMapping, sPropertyValue, Nothing)
					End If

				Else
					Throw New Exception("mProcessAdvantageMappingIntoClass: arrAdvantageMappingIndividualParts length is not 2.  Split Value: " & sEachAdvantageMappingElement)
				End If

			Next
		

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Function mGetFileFriendlyTimeStamp() As String

		Dim dtNowTimestamp As String = DateTime.Now.ToString("MM-dd-yyyy HH:mm tt")
		Dim sFileFriendlyTimeStamp As String

		Try


			sFileFriendlyTimeStamp = dtNowTimestamp.Replace(" ", "_") _
									   .Replace("-", ".") _
									   .Replace(":", ".")

			mGetFileFriendlyTimeStamp = sFileFriendlyTimeStamp

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Function
	
	Private Function mCreateOutputSpreadsheet(sExcelFilePath As String) As Boolean

		Dim sExcelFolderPath As String
		Dim sExcelFileName As String
		Dim sExcelOutputCopyPath As String
		Dim bSuccess As Boolean = false
		Try

			sExcelFolderPath = Path.GetDirectoryName(sExcelFilePath)
			sExcelFileName = Path.GetFileNameWithoutExtension(sExcelFilePath)
			sExcelFileName = Replace(sExcelFileName, "_Template", "")
			sExcelOutputCopyPath = Path.Combine(sExcelFolderPath, sExcelFileName & "_TestResults." & mGetFileFriendlyTimeStamp() & ".xlsx")
			File.Copy(sExcelFilePath, sExcelOutputCopyPath, overwrite:=True)
			Me.txtExcelOutputFilePath.Text = sExcelOutputCopyPath
			bSuccess = True

			return(bSuccess)
		Catch ex As Exception
			bSuccess = False
			return(bSuccess)
			mHandleError(ex)
		End Try
	End Function
	
	Private Function mGetSpreadsheetValues(sExcelFilePath As String) as List(Of cQATest)

		Dim iRowCount As Integer = 0
		Dim iRequestColumnIndex As Integer = 0
		Dim iAdvantageMappingColumnIndex As Integer = 0
		Dim oQATest As cQATest
		Dim oConn As ADODB.Connection
		Dim oRST As ADODB.Recordset
		Dim lstQATest As List(Of cQATest)
		Dim sSQL As String
		Dim sConnectionString As String
		Dim iIndexCounter As Integer = 0
		Dim sSRNumber As String = ""
		Dim sQAEndpoint As String = ""
		Dim sRequestType As String = ""
		Dim sRequestValue As String = ""
		Dim sAdvantageMappingValue As String = ""

		sConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & sExcelFilePath & ";Extended Properties='Excel 12.0 Xml;HDR=YES';"
		oConn = New ADODB.Connection
		oRst = New ADODB.Recordset

		
		Try
			
			sSQL = "SELECT [Sr No], [Request], [QA Endpoint], [Request Type], [Advantage Mapping] FROM [Sheet1$]"
			lstQATest = New List(Of cQATest)
			
			oConn.Open(sConnectionString)

			oRst.Open(sSQL, oConn, CursorTypeEnum.adOpenForwardOnly, LockTypeEnum.adLockReadOnly)

			Do Until oRst.EOF
				sSRNumber = oRST("Sr No").value & ""
				sQAEndpoint = oRST("QA Endpoint").value & ""
				sRequestType = oRST("Request Type").value & ""
				sRequestValue = oRST("Request").value & ""
				sAdvantageMappingValue = oRST("Advantage Mapping").value & ""
				If sRequestValue.Trim() = "" OR sRequestType.Trim() = "" Then 
					Exit Do
				End If

				oQATest = New cQATest
				oQATest.SRNumber = sSRNumber
				oQATest.QAEndpoint = sQAEndpoint
				oQATest.RequestType = sRequestType
				oQATest.Request = sRequestValue
				oQATest.AdvantageMapping = sAdvantageMappingValue
				lstQATest.Add(oQATest)
				oQATest = Nothing

				oRST.MoveNext
			Loop

			oRst.Close
			oConn.Close

			oRst = Nothing
			oConn = Nothing

			mGetSpreadsheetValues = lstQATest



			
		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Function

	Private Sub frmAPIQA_Load(sender As Object, e As EventArgs) Handles Me.Load
		Try

			Me.cboEnvironment.SelectedItem = "QA"
			
			If Environment.MachineName = "SPYRO" Then
				sDatabaseDNSString = "advantage.db.com"
			Else
				sDatabaseDNSString = "stg-sqlstd2016-adverp.gsdwkglobal.com"
			End If

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub

	Private Sub txtExcelOutputFilePath_Enter(sender As Object, e As EventArgs) Handles txtExcelOutputFilePath.Enter
		Try

			Me.txtExcelOutputFilePath.SelectAll

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub


End Class