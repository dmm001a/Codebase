Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Button
Imports Microsoft.Data
Imports Microsoft.Data.SqlClient
Imports Org.BouncyCastle.Bcpg.Sig
Imports System.Windows.Forms
Public Class cTOMManagement

	Private sDNSString As String
	Private sDBName As String
	Private sDBUserName As String
	Private sDBPassword As String


	Public Sub New(sNewDNSString, sNewDBName, sNewDBUserName, sNewDBPassword)
		Try

			sDNSString = sNewDNSString
			sDBName = sNewDBName
			sDBUserName = sNewDBUserName
			sDBPassword = sNewDBPassword

		Catch oError As Exception
			Throw
		End Try
	End Sub

	Private Sub mClearControls(oForm As System.Windows.Forms.Form)
		Try

			For Each oControl As Control In oForm.Controls
				If (TypeOf oControl Is System.Windows.Forms.CheckBox) Or (TypeOf oControl Is System.Windows.Forms.Label) Then
					' your logic
				End If
			Next


		Catch oError As Exception
			Throw
		End Try
	End Sub
    
    Public Sub PopulateTOM(oPanel As System.Windows.Forms.Panel, Optional iOwnerID As Integer = -1)
        Try

            Dim sSQL As String
            Dim sOwner As String
            Dim iStartLeft As Integer
            Dim iLastLeft As Integer
            Dim iLastTop As Integer
            Dim iTopMargin As Integer
            Dim iLeftMargin as Integer
            Dim iStartTop As Integer
            Dim iRowHeight As Integer
            Dim iRowIndex As Integer
            Dim iRowWidth As Integer
            Dim iRowBottom As Integer
            Dim sJiraID As String
            Dim oControl As System.Windows.Forms.Control
            Dim oCheckBox As System.Windows.Forms.CheckBox
            Dim oLabel As Control
            Dim oUtilityClass As UtilityClass.cADONETDatabase
            Dim oSQLDataReader As SqlDataReader

            sSQL = "SELECT TParent.*, "
            sSQL = sSQL & " TParent.Task_ID * 100000 AS SortKey, "
            sSQL = sSQL & " O.Owner_Name "
            sSQL = sSQL & " FROM tb_TOM TParent "
            sSQL = sSQL & " INNER JOIN tb_TOM_Owner O ON TParent.Owner_ID = O.Owner_ID "
            sSQL = sSQL & " WHERE TParent.Parent_ID = 0 "
            sSQL = sSQL & " AND TParent.Task_Complete = 0 "
            If iOwnerID <> 11 Then
                sSQL = sSQL & " AND TParent.Owner_ID = " & iOwnerID
            End If

            sSQL = sSQL & " UNION ALL "

            sSQL = sSQL & " SELECT TChild.*, "
            sSQL = sSQL & " TChild.Parent_ID * 100000 + TChild.Task_ID AS SortKey, "
            sSQL = sSQL & " O.Owner_Name "
            sSQL = sSQL & " FROM tb_TOM TChild "
            sSQL = sSQL & " INNER JOIN tb_TOM_Owner O ON TChild.Owner_ID = O.Owner_ID "
            sSQL = sSQL & " WHERE TChild.Parent_ID IN (SELECT Task_ID FROM tb_TOM WHERE Parent_ID = 0) "
            sSQL = sSQL & " AND TChild.Task_Complete = 0 "
            If iOwnerID <> 11 Then
                sSQL = sSQL & " AND TChild.Owner_ID = " & iOwnerID
                sSQL = sSQL & " ORDER BY SortKey"
            Else
                sSQL = sSQL & " ORDER BY Owner_Name, SortKey"
            End If
            

            iStartLeft = 10
            iStartTop = 5
            iRowHeight = 25
            iRowWidth = 275
            iLeftMargin = 12
            iTopMargin = 5

            'Messagebox.Show(frmTom.pnlTom.Controls.Count)

            For Each ctrl As Control In oPanel.Controls
                ctrl.Dispose() 
            Next
            oPanel.Controls.Clear()

            oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)
            oUtilityClass.OpenConnection()

            oSQLDataReader = oUtilityClass.GetDataReader(sSQL)
            
            While oSQLDataReader IsNot Nothing AndAlso oSQLDataReader.Read()


                If iOwnerID = 11 Then
                    sOwner = oSQLDataReader("Owner_Name").ToString() & ":  "
                Else
                    sOwner = ""
                End If

                iLastLeft = iStartLeft
                iLastTop = iTopMargin

                'Add The Description (Jira Link or Text)
                sJiraID = oSQLDataReader("Jira_ID").ToString()

                If sJiraID.Length > 0 Then

                    oControl = New LinkLabel()
                    oControl.Text = sOwner & oSQLDataReader("Task_Description").ToString()
                    oControl.AutoSize = False
                    'oControl.Width = iRowWidth

                    Dim oLink As LinkLabel = DirectCast(oControl, LinkLabel)
                    oLink.LinkColor = Color.Blue
                    oLink.ActiveLinkColor = Color.DarkBlue
                    oLink.VisitedLinkColor = Color.Purple
                    oLink.Font = New Font(oLink.Font, FontStyle.Underline)
                    oLink.Tag = oSQLDataReader("Jira_ID").ToString()   ' store JiraID for click handler

                    oLink.Left = iLastLeft
                    oLink.Top = iLastTop + (iRowIndex * iRowHeight + iTopMargin)

                    AddHandler oLink.Click, AddressOf JiraLink_Click
                    oLabel = oLink
                    oLabel.Width = iRowWidth
                Else
                    oControl = New Label()
                    oControl.Text = sOwner & oSQLDataReader("Task_Description").ToString()
                    oControl.AutoSize = False
                    oControl.Width = iRowWidth
                    DirectCast(oControl, Label).TextAlign = ContentAlignment.MiddleLeft

                    If oSQLDataReader("Parent_ID") = 0 Then
                        oControl.Left = iLastLeft
                    Else
                        oControl.Left = iLastLeft + iLeftMargin
                    End If
                    oControl.Top = iLastTop + (iRowIndex * iRowHeight + iTopMargin)

                End If
                oPanel.Controls.Add(oControl)


                'Add the Checkbox
                oCheckBox = New System.Windows.Forms.CheckBox()
                oCheckBox.Name = "chk_" & oSQLDataReader("Task_ID").ToString()
                oCheckBox.Tag = oSQLDataReader("Task_ID").ToString()
                oCheckBox.Left = iRowWidth + 25

                oCheckBox.Top = iLastTop + (iRowIndex * iRowHeight)
                iLastLeft = oControl.Left
                iLastTop = oControl.Top
                iRowBottom = iStartTop + (iRowIndex * iRowHeight) + iRowHeight
                
                AddHandler oCheckBox.CheckedChanged, AddressOf TaskChecked
                oPanel.Controls.Add(oCheckBox)


                'Add The Progress Bar
                oControl = New ProgressBar()

                oControl.Left = iStartLeft
                oControl.Top = iRowBottom + 2   ' adjust spacing as needed
                oControl.Width = 175
                oControl.Height = 2                 ' thin line
                DirectCast(oControl, ProgressBar).Style = ProgressBarStyle.Continuous
                DirectCast(oControl, ProgressBar).Value = 100

                If oSQLDataReader("Parent_ID") = 0 Then
                    oPanel.Controls.Add(oControl)
                End If

                iRowIndex = iRowIndex + 1


            End While
           'Messagebox.Show(frmTom.pnlTom.Controls.Count)

            oSQLDataReader.Close()
            oUtilityClass.CloseConnection()

            oSQLDataReader = Nothing
            oUtilityClass = Nothing

        Catch oError As Exception
            Throw
        End Try
    End Sub

    Private Sub TaskChecked(sender As Object, e As EventArgs)
        Try

			Dim sSQL As String
            Dim iOwnerID As Integer
            Dim iTaskID As Integer
            Dim ofrmTOM As frmTom
            Dim oCheckBox As System.Windows.Forms.CheckBox 
			Dim oUtilityClass As UtilityClass.cADONETDatabase

            ofrmTOM = mGetOpenFormByName("frmTOM")
            oCheckBox = DirectCast(sender, System.Windows.Forms.CheckBox)

            iTaskID = CInt(oCheckBox.Tag)
            iOwnerID = CInt(ofrmTOM.cboTOMOwner.SelectedValue)

			oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)
			oUtilityClass.OpenConnection()

			    sSQL = "UPDATE tb_TOM SET Task_Complete = 1 WHERE Task_ID = " & iTaskID

			    oUtilityClass.ExecuteSQL(sSQL)


                PopulateTOM(ofrmTOM.pnlTom, iOwnerID)

			oUtilityClass.CloseConnection()
			oUtilityClass = Nothing

        Catch oError As Exception
            Throw
        End Try
    End Sub

    Private Sub JiraLink_Click(sender As Object, e As EventArgs)
		Try
			Dim oLink As LinkLabel = CType(sender, LinkLabel)
			Dim sJiraID As String = oLink.Tag.ToString()

			' Build your Jira URL
			Dim sURL As String = "https://wkenterprise.atlassian.net/browse/" & sJiraID

			Process.Start(New ProcessStartInfo With {
			.FileName = sURL,
			.UseShellExecute = True
		})
		Catch ex As Exception
			MessageBox.Show("Unable to open Jira link: " & ex.Message)
		End Try
	End Sub

	Public Sub UpdateTOM(oPanel As System.Windows.Forms.Panel)
		
	End Sub


	Public Sub AddTOM(iOwnerID As Integer, iParentID As Integer, sTaskDescription As String, sJiraID As String)
		Try

			Dim sSQL As String
			Dim oUtilityClass As UtilityClass.cADONETDatabase
			Dim oStringFunction As UtilityClass.cDatatypeHandler

			oStringFunction = New UtilityClass.cDatatypeHandler
			oUtilityClass = New UtilityClass.cADONETDatabase(sDNSString, sDBName, sDBUserName, sDBPassword)
			oUtilityClass.OpenConnection()

			sSQL = "INSERT INTO tb_Tom (Owner_ID, Parent_ID, Task_Description, Jira_ID) "
			sSQL = sSQL & "VALUES ("
			sSQL = sSQL & iOwnerID.ToString & ", "
			sSQL = sSQL & iParentID.ToString & ", "
			sSQL = sSQL & oStringFunction.SingleQuotes(sTaskDescription) & ", "
			sSQL = sSQL & oStringFunction.SingleQuotes(sJiraID) & ")"

			oUtilityClass.ExecuteSQL(sSQL)

			oUtilityClass.CloseConnection()

			oStringFunction = Nothing
			oUtilityClass = Nothing

		Catch oError As Exception
			Throw
		End Try
	End Sub
End Class
