Public Module Misc

    Public Function FormatHTMLMessage(ByVal title As String, ByVal msg As String, ByVal redirectpage As String, ByVal timeout As String, ByVal critical As Boolean) As String
        Dim messageTemplate As String = GenericMessageTemplate
        If Not title.Length < 1 Then messageTemplate = messageTemplate.Replace("%MSG TITLE%", title.ElementAt(0).ToString.ToUpper() & title.Remove(0, 1).ToLower()) Else messageTemplate = messageTemplate.Replace("%MSG TITLE%", "")
        messageTemplate = messageTemplate.Replace("%REDIRECT URL%", redirectpage)
        messageTemplate = messageTemplate.Replace("%REDIRECT DELAY%", timeout)
        messageTemplate = messageTemplate.Replace("%MESSAGE TEXT%", msg)
        If critical Then
            messageTemplate = messageTemplate.Replace("%C%", "DD0000")
        Else
            messageTemplate = messageTemplate.Replace("%C%", "66CCFF")
        End If
        Return messageTemplate
    End Function

End Module
