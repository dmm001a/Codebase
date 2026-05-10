Imports System.Data.SQLite
Imports System.Security.Cryptography
Public Class cDatabase
    


    Private lConnectionString As String

    Public Property pConnectionString() As String

        Get
            return lConnectionString
        End Get

        Set(ByVal sConnectionString As String)
            lConnectionString = sConnectionString
        End Set

    End Property

   Public Sub New(ByVal sConnectionString As String)

            Me.pConnectionString = sConnectionString

    End Sub

    

    Private Function mOpenConnection() As SQLiteConnection
        Try

            Dim oConn As SQLiteConnection

            oConn = New SQLiteConnection(Me.pConnectionString)

            mOpenConnection = oConn

        Catch ex As Exception
            Throw
        End Try
    End Function


    Public Function mGetDataTable(ByVal sSQL As String) As DataTable
        Try
            
             Dim oConn As SQLiteConnection
             Dim oCmd As SQLiteCommand
             Dim oAdapter As SQLiteDataAdapter
             Dim oDatatable As DataTable


            oConn = Me.mOpenConnection()
            oConn.Open

            oCmd = New SQLiteCommand(sSQL, oConn)
            oAdapter = New SQLiteDataAdapter(oCmd)
            oDatatable = New DataTable()

            oAdapter.Fill(oDatatable)

            oConn.Close()
            oConn.Dispose()
            oCmd.Dispose()
            oAdapter.Dispose()

            oConn = Nothing
            oCmd = Nothing
            oAdapter = Nothing

            mGetDataTable = oDatatable

        Catch ex As Exception
            Throw
        End Try
    End Function


Public Function mExecuteSQL(ByVal sSQL As String) As Integer
        Dim oConn As SQLiteConnection = Nothing
        Dim oCmd As SQLiteCommand = Nothing

    Try
        oConn = Me.mOpenConnection()
        oCmd = New SQLiteCommand(sSQL, oConn)
        oConn.Open

        Dim rowsAffected As Integer = oCmd.ExecuteNonQuery()

        oConn.Close()
        oCmd.Dispose()
        oConn.Dispose()

        Return rowsAffected

    Catch ex As Exception
        Throw
    Finally
        oCmd = Nothing
        oConn = Nothing
    End Try
End Function

End Class
