Public Class frmPolling

	Private WithEvents tmPolling As New Timer()

	Private Sub btnActivate_Click(sender As Object, e As EventArgs) Handles btnActivate.Click

		Dim iIntervalValue As Integer
		Dim iIntervalValueConversion As Integer
		Dim oCheckAllAPI As System.Windows.Forms.CheckBox

		oCheckAllAPI = frmMain.chkALLAPI

	   	Try


			If Me.btnActivate.Text = "Activate" Then

				If Me.txtIntervalValue.Text = "" Or Not IsNumeric(Trim(Me.txtIntervalValue.Text)) Then
					Throw New Exception("Interval Value is required and must be numeric.")
				Elseif Me.cboInterval.SelectedItem = "" Then
					Throw New Exception("Interval Type is required")
				Elseif Me.cboInterval.SelectedItem = "Seconds" And Cint(Me.txtIntervalValue.Text) < 5 Then
					Throw New Exception("Minimum of 5 second interval is required.")
				Elseif oCheckAllAPI.Checked = True And Me.cboInterval.SelectedItem = "Seconds" Then
					Throw New Exception("A minimum interval of 1 minute is required to put an All API Test on the scheduler.")
				End If

				
				iIntervalValue = CInt(Trim(Me.txtIntervalValue.Text))

				If Me.cboInterval.SelectedItem = "Seconds" Then
					iIntervalValueConversion = iIntervalValue * 1000
				ElseIf Me.cboInterval.SelectedItem = "Minutes" Then
					iIntervalValueConversion = iIntervalValue * 60000
				End If

				frmMain.mRunTest(False)
				tmPolling.Interval = iIntervalValueConversion ' 1000 ms = 1 second
				tmPolling.Start() 

				Me.btnActivate.Text = "Deactivate" 
			Elseif Me.btnActivate.Text = "Deactivate" Then
				tmPolling.Stop()
				Me.btnActivate.Text = "Activate"
			End If
			
			oCheckAllAPI = Nothing

		Catch ex As Exception
			mHandleError(ex)
		End Try
	End Sub


	Private Sub tmPolling_Tick(sender As Object, e As EventArgs) Handles tmPolling.Tick
		Try

			frmMain.mRunTest(False)

		Catch ex As Exception
			mHandleError(ex)
		End Try		
	End Sub

	Private Sub frmPolling_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		Try
			tmPolling.Stop()
		Catch ex As Exception
			mHandleError(ex)
		End Try	
	End Sub
End Class