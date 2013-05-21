Imports chanb.GlobalFunctions
Public Module JSONApi

    Public Function WpostsToJsonList(ByVal ids As Integer(), ByVal p As HTMLParameters) As String
        If ids.Length = 0 Then
            Return " "
        Else
            Dim sb As New StringBuilder
            For Each x In GetWpostList(ids)
                'sb.Append("""post"":""")
                sb.Append(GetSingleReplyHTML(x, p))
                ' sb.Append(""",")
            Next
            Dim st As String = sb.ToString
            ' st = st.Remove(st.Length - 1)
            ' st = st & "}"
            Return st
        End If
    End Function

    Private Function json(ByVal jsonname As String, ByVal data As String) As String
        Return "'" & jsonname & "':'" & data & "'"
    End Function

End Module
