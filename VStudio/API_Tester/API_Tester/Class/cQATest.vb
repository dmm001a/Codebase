Public Class cQATest

	Private lSRNumber As String
	Private lAdvantageDBConnectionString As String
	Private lRequest As String
	Private lResponse As String
	Private lQAEndpoint As String
	Private lRequestType As String
	Private lAdvantageMapping As String
	Private lAdvantageResult As String
	Private lResponseStatusCode As String

	Public Property SRNumber As String

        Get
            Return lSRNumber
        End Get

        Set(sSRNumber As String)
            lSRNumber = sSRNumber
        End Set

    End Property

    	Public Property RequestType As String

        Get
            Return lRequestType
        End Get

        Set(sRequestType As String)

	       If (sRequestType.ToUpper = "POST" OR sRequestType.ToUpper = "GET") Then
			lRequestType = sRequestType
		  Else
			Throw New Exception("Request Type must be POST or GET.  Please check the spreadsheet Request Type column.")
		  End If
        End Set

    End Property

    Public Property QAEndpoint As String
        Get
            Return lQAEndpoint
        End Get

        Set(sQAEndpoint As String)
            lQAEndpoint = sQAEndpoint
        End Set
    End Property

	Public Property Request As String

        Get
            Return lRequest
        End Get

        Set(sRequest As String)
            lRequest = sRequest
        End Set

    End Property

    	Public Property Response As String

        Get
            Return lResponse
        End Get

        Set(sResponse As String)
            lResponse = sResponse
        End Set

    End Property


    Public Property AdvantageMapping As String

        Get
            Return lAdvantageMapping
        End Get

        Set(sAdvantageMapping As String)
            lAdvantageMapping = sAdvantageMapping
        End Set

    End Property

        Public Property AdvantageResult As String

        Get
            Return lAdvantageResult
        End Get

        Set(sAdvantageResult As String)
            lAdvantageResult = sAdvantageResult
        End Set

    End Property

    Public Property ResponseStatusCode As String

        Get
            Return lResponseStatusCode
        End Get

        Set(sResponseStatusCode As String)
            lResponseStatusCode = sResponseStatusCode 
        End Set

    End Property
    
End Class
