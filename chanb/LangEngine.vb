Public Class LangEngine

    Dim dictObj As ValuesStore

    Sub New(ByVal lang As String)
        Dim basepath As String = chanb.My.Request.PhysicalApplicationPath & "\langs\"
        If FileIO.FileSystem.FileExists(basepath & lang & ".as") = False Then
            dictObj = New ValuesStore(basepath & "en")
            'Throw New ArgumentException("Specified language file does not exist")
        Else
            dictObj = New ValuesStore(basepath & lang)
        End If
    End Sub

    Public Function Retrive(ByVal name As String) As String
        If dictObj.KeyExist(name) = False Then
            Return "Invalid key"
        Else
            Return (CStr(dictObj.GetKey(name)))
        End If
    End Function

End Class
