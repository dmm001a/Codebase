Public Class cAdvantageMapping
	Private lsSRNumber As String
	Private lsAttribute As String
	Private lsTableName As String
	Private lsTableKey As String
	Private lsFieldToRetrieve As String



	Public Property SRNumber As String

        Get
            Return lsSRNumber
        End Get

        Set(sSRNumber As String)
            lsSRNumber = sSRNumber
        End Set

    End Property

    	Public Property Attribute As String

        Get
            Return lsAttribute
        End Get

        Set(sAttribute As String)
            lsAttribute = sAttribute
        End Set

    End Property

    	Public Property TableName As String

        Get
            Return lsTableName
        End Get

        Set(sTableName As String)
            lsTableName = sTableName
        End Set

    End Property

    	Public Property TableKey As String

        Get
            Return lsTableKey
        End Get

        Set(sTableKey As String)
            lsTableKey = sTableKey
        End Set

    End Property

    	Public Property FieldToRetrieve As String

        Get
            Return lsFieldToRetrieve
        End Get

        Set(sFieldToRetrieve As String)
            lsFieldToRetrieve = sFieldToRetrieve
        End Set

    End Property


End Class
