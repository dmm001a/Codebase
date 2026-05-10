Public Class cComboBoxBind

    Private lID As Integer
    Private lDescription As String

    Public Property pID() As Integer

        Get
            pID = lID
        End Get

        Set(ByVal iID As Integer)
            lID = iID
        End Set

    End Property

    Public Property pDescription() As String

        Get
            pDescription = lDescription
        End Get

        Set(ByVal sDescription As String)
            lDescription = sDescription
        End Set

    End Property

    Public Function mInitializecComboBoxBind(
            ByRef iID As Integer,
            ByRef sDescription As String) As Integer

        Try

            lID = iID
            lDescription = sDescription

        Catch ex As Exception
            Throw New Exception("Error: " & ex.Message)

        End Try

    End Function

End Class
