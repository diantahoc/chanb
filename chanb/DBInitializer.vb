Public Class DBInitializer

    Dim chanbDLLPath As String = ""

    Public ReadOnly Property ConnectionString() As String
        Get
            If FileIO.FileSystem.FileExists(chanbDLLPath & "db.txt") = False Then
                Throw New ArgumentException("Invalid or no database congfiguration file")
                Return ""
            Else
                Return IO.File.ReadAllText(chanbDLLPath & "db.txt")
            End If
        End Get
    End Property

    Sub New()
        chanbDLLPath = chanb.My.Request.PhysicalApplicationPath & "\bin\"
    End Sub

End Class
