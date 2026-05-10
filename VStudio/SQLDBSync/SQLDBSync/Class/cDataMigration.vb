Imports System.Xml.Linq

Public Class cDataMigration

    Private prpWorkingDirectory As String = AppDomain.CurrentDomain.BaseDirectory
    Private prpSchemaCompareFileName As String = "Generic_Schema_Compare.scmp"
    Private prpDataCompareFileName As String = "Generic_Data_Compare.dcmp"
    

    Public Enum enuDeploymentType
        Smoke
        QA
        Prod
    End Enum

    Public Enum enuFileType
        Schema
        Data
    End Enum

    Public Enum enuComparisonDatabase
        DR_Tracker
        Ikawsoft_Central
        Roadmap
    End Enum

    Public Enum enuRetrievalType 
        FileName
        ModelName
    End Enum

    Private Function GetCompareValue(iFileType As enuFileType, iRetrievalType As enuRetrievalType) As String
        Try

            If iRetrievalType = enuRetrievalType.FileName Then
                Select Case iFileType
                    Case enuFileType.Schema : Return prpSchemaCompareFileName
                    Case enuFileType.Data : Return prpDataCompareFileName
                End Select
            Elseif iRetrievalType = enuRetrievalType.ModelName Then
                Select Case iFileType
                    Case enuFileType.Schema : Return "SourceModelProvider"
                    Case enuFileType.Data : Return "TargetModelProvider"
                End Select

            End If

        Catch oError As Exception
            Throw oError
        End Try
    End Function

    Private Sub mUpdateConnectionString(iDeploymentType as enuDeploymentType, iFileType As enuFileType, iDatabase As enuComparisonDatabase)

            Try

                Dim sFilePath As String
                Dim sModelProviderName As String
                Dim sConnectionString As String
                Dim sDatabaseName As String
                Dim sDeploymentType As String
                Dim sReplacementString As String
                Dim oComparisonFile As XDocument
                Dim oConnNode As XElement

                sFilePath = prpWorkingDirectory & GetCompareValue(iFileType, enuRetrievalType.FileName)
                sModelProviderName = GetCompareValue(iFileType, enuRetrievalType.ModelName)

                oComparisonFile = XDocument.Load(sFilePath)

                oConnNode = oComparisonFile.Descendants(sModelProviderName).Descendants("ConnectionString").FirstOrDefault()

                ' Update the value
                If oConnNode IsNot Nothing Then
                    sConnectionString = oConnNode.value
                    sDatabaseName = iDatabase.ToString()
                    sDeploymentType = iDeploymentType.ToString()
                    sReplacementString = ";Initial Catalog=" & sDeploymentType & "_" & sDatabaseName

                    sConnectionString = Replace(sConnectionString, ";Initial Catalog=local_DR_Tracker", sReplacementString)

                    oConnNode.Value = sConnectionString
                End If

                ' Save the updated XML document
                oComparisonFile.Save(sFilePath)

            Catch oError As Exception
                Throw
            End Try
    End Sub

    Public Function mSetTargetValues(iDeploymentType As enuDeploymentType, iFileType As enuFileType, iDatabase As enuComparisonDatabase) As String
        Try

            mUpdateConnectionString(iFileType, iDeploymentType, iDatabase)

        Catch oError As Exception
            Throw oError
        End Try
    End Function

    Public Function mGetFilePath(iFileType As enuFileType) As String
        Try

            Dim sFilePath As String

            sFilePath = prpWorkingDirectory & "\" & GetCompareValue(iFileType, enuRetrievalType.FileName)
            
            return(sFilePath)

        Catch oError As Exception
            Throw oError
        End Try
    End Function

End Class
