Public Class frmDBSync
    Private Sub btnCompare_Click(sender As Object, e As EventArgs) Handles btnCompare.Click
        Try

            Dim oDataMigration As cDataMigration

            oDataMigration = New cDataMigration

                Messagebox.Show(oDataMigration.mGetFilePath(cDataMigration.enuFileType.Schema))
                'Process.Start("devenv.exe", oDataMigration.mGetFilePath(oDataMigration.enuFileType.Schemam,

            oDataMigration = Nothing

        Catch oError As Exception
            MessageBox.Show(oError.Message)
        End Try
    End Sub
End Class