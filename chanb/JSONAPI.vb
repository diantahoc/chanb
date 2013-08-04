Imports chanb.GlobalFunctions
Imports Newtonsoft.Json

Public Module JSONApi

    Public Function WpostsToJsonList(ByVal ids As Integer(), ByVal p As HTMLParameters) As String
        Dim il As New List(Of PublicWpost)
        For Each x As WPost In GetWpostList(ids)
            Dim i As New PublicWpost
            i.Comment = ProcessComment(x, "default", Nothing)
            i.Email = x.email
            i.Time = x.time
            i.Type = x.type
            i.Files = x.files
            i.ID = x.PostID
            i.posterId = x.posterID
            i.HasFiles = x.HasFile
            i.Name = x.name
            i.Subject = x.subject
            i.ParentThread = x.parent
            il.Add(i)
        Next
        Return JsonConvert.SerializeObject(il, Formatting.None)
    End Function

    Public Function GetFileList(ByVal tid As Integer, ByVal filetypes As String(), ByVal includeArch As Boolean) As String
        Dim il As New List(Of String)
        Dim data As WPost() = GetThreadData(tid, includeArch)
        For Each x As WPost In data
            For Each file As WPostImage In x.files
                For Each ext In filetypes
                    If file.Extension = ext.ToUpper Then il.Add(GetImageWEBPATH(file.ChanbName))
                Next
            Next
        Next
        Return JsonConvert.SerializeObject(il, Formatting.None)
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

    Private Class PublicWpost

        Public ID As Integer
        Public HasFiles As Boolean
        Public Files As WPostImage() = {}
        Public Name As String
        Public Email As String
        Public Subject As String
        Public Comment As String
        Public posterId As String
        Public Type As WPost.PostType
        Public ParentThread As Integer
        Public Time As DateTime

    End Class

    Public Function GetSELinks() As String
        Return JsonConvert.SerializeObject(searchEngineLinkList, Formatting.None)
    End Function
End Module
