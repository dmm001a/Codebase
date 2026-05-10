Public Class frmViewRequestResponse
    Public prpRequest As String
    Public prpResponse As String
    Public prpAPIName As String

    Private Sub frmViewRequestResponse_Load(sender As Object, e As EventArgs) Handles Me.Load
        Try
            lblAPIName.Width = Me.Width - 5
            lblAPIName.TextAlign = ContentAlignment.MiddleCenter

            lblAPIName.Text = prpAPIName
            Me.TxtRequest.Text= mFormatJson(prpRequest)
            Me.txtResponse.Text = mFormatJson(prpResponse)

	Catch ex As Exception
		mHandleError(ex)
        End Try
    End Sub
End Class