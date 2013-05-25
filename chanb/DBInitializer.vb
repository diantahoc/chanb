Public Class DBInitializer

    Dim chanbDLLPath As String = ""
    Dim cnxStr As String = ""

    Public ReadOnly Property ConnectionString() As String
        Get
            If cnxStr = "" Then
                If FileIO.FileSystem.FileExists(chanbDLLPath & "db.txt") = False Then
                    '  Throw New ArgumentException("Invalid or no database configuration file.")
                    cnxStr = ""
                Else
                    cnxStr = IO.File.ReadAllText(chanbDLLPath & "db.txt")
                End If
            End If
            Return cnxStr
        End Get
    End Property

    Sub New()
        chanbDLLPath = chanb.My.Request.PhysicalApplicationPath & "\bin\"
    End Sub

End Class
