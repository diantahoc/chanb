Public Class LangEngine

    Dim dictObj As ValuesStore
    Dim languageDirectory As String = chanb.My.Request.PhysicalApplicationPath & "\bin\langs\"

    Sub New(ByVal lang As String)
        If FileIO.FileSystem.FileExists(languageDirectory & lang & ".dic") Then
            dictObj = New ValuesStore(languageDirectory & lang)
        Else
            Throw New ArgumentException("This language does not exist")
        End If
    End Sub

    Public Function Retrive(ByVal name As String) As String
        Return dictObj.GetKey(name)
    End Function

End Class
