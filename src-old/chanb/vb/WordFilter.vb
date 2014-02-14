Public Class WordFilter

    Dim wordfilterDB As ValuesStore
    Dim smiliesDB As ValuesStore

    Sub New()
        Dim basepath As String = chanb.My.Request.PhysicalApplicationPath & "\bin\"
        wordfilterDB = New ValuesStore(basepath & "wordfilter")
        If EnableSmilies Then
            smiliesDB = New ValuesStore(basepath & "smilies")
        End If
    End Sub

    Public Function FilterText(ByVal text As String) As String
        For Each x In wordfilterDB.GetKeyList
            text = text.Replace(x, CStr(wordfilterDB.GetKey(x)))
        Next
        If EnableSmilies Then
            For Each x In smiliesDB.GetKeyList
                text = text.Replace(x, CStr(smiliesDB.GetKey(x)))
            Next
        End If
        Return text
    End Function

    Public Sub UpdateWF(ByVal name As String, ByVal newValue As String)
        wordfilterDB.AddKey(name, newValue)
        wordfilterDB.Save()
    End Sub

    Public Sub UpdateSmilies(ByVal name As String, ByVal newValue As String)
        smiliesDB.AddKey(name, newValue)
        smiliesDB.Save()
    End Sub


End Class
