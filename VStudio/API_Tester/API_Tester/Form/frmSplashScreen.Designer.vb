<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmSplashScreen
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSplashScreen))
		picSplashScreen = New PictureBox()
		CType(picSplashScreen, ComponentModel.ISupportInitialize).BeginInit()
		SuspendLayout()
		' 
		' picSplashScreen
		' 
		picSplashScreen.Image = CType(resources.GetObject("picSplashScreen.Image"), Image)
		picSplashScreen.InitialImage = My.Resources.Resources.smaller_splash_Copilot_20250726_181924
		picSplashScreen.Location = New Point(-1, 0)
		picSplashScreen.Name = "picSplashScreen"
		picSplashScreen.Size = New Size(511, 338)
		picSplashScreen.TabIndex = 0
		picSplashScreen.TabStop = False
		' 
		' frmSplashScreen
		' 
		AutoScaleDimensions = New SizeF(7F, 15F)
		AutoScaleMode = AutoScaleMode.Font
		ClientSize = New Size(508, 339)
		Controls.Add(picSplashScreen)
		Icon = CType(resources.GetObject("$this.Icon"), Icon)
		MaximizeBox = False
		MinimizeBox = False
		Name = "frmSplashScreen"
		RightToLeftLayout = True
		StartPosition = FormStartPosition.CenterScreen
		Text = "API Tester"
		CType(picSplashScreen, ComponentModel.ISupportInitialize).EndInit()
		ResumeLayout(False)
	End Sub

	Friend WithEvents picSplashScreen As PictureBox
End Class
