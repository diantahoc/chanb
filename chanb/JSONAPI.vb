Imports chanb.GlobalFunctions
Public Module JSONApi

    Public Function WpostsToJsonList(ByVal ids As Integer(), ByVal p As HTMLParameters) As String
        If ids.Length = 0 Then
            Return "<span class='hide'></span>"
        Else
            Dim sb As New StringBuilder
            ' sb.Append("{")
            For Each x In GetWpostList(ids)
                ' sb.Append("""post" & CStr(x.PostID) & """: """)
                sb.Append(GetPostHTML(x, p))
                ' sb.Append(""",")
            Next
            'Dim st As String = sb.ToString
            'st = st.Remove(st.Length - 1)
            'st = st & "}"
            Return sb.ToString
        End If
    End Function

    Public Function GetFileList(ByVal tid As Integer, ByVal filetypes As String(), ByVal includeArch As Boolean) As String
        Dim sb As New StringBuilder
        Dim data As WPost() = GetThreadData(tid, includeArch)
        For Each x As WPost In data
            If Not x._imageP = "" Then
                For Each sp In x._imageP.Split(CChar(";"))
                    Dim image As WPostImage = GetWPostImage(sp)
                    For Each ext In filetypes
                        If image.Extension = ext.ToUpper Then
                            sb.Append(GetImageWEBPATH(image.ChanbName))
                            sb.Append(vbNewLine)
                        End If
                    Next
                Next
            End If
        Next
        Return sb.ToString
    End Function

    Public Function GetThreadPageNumber(ByVal tid As Integer) As Integer

        Dim currentThread As Integer() = GetThreads(0, (MaximumPages * ThreadPerPage) - 1, True, False)

        Dim pageNt As Integer = -1

        For Page As Integer = 1 To MaximumPages Step 1

            For threadN As Integer = 1 To ThreadPerPage Step 1

                If currentThread(threadN) = tid Then pageNt = Page + 1


            Next

            Page = pageNt
        Next

        Return pageNt
    End Function
End Module
