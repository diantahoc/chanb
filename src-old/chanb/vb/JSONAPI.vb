﻿Imports chanb.GlobalFunctions
Imports Newtonsoft.Json

Friend Module JSONApi

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
        Dim currentThread As Integer() = GetThreads(0, (MaximumPages * ThreadPerPage) - 1, False, False)
        Dim p As Integer = -1
        If Array.IndexOf(currentThread, tid) = -1 Then
            Return -1
        Else
            Dim pagei As Integer = Array.IndexOf(currentThread, tid) + 1
            For i As Integer = 1 To MaximumPages Step 1
                If ((1 * i) <= pagei) And (pagei <= i * ThreadPerPage) Then
                    p = i
                    Exit For
                End If
            Next
        End If
        Return p
    End Function

    Public Function GetThreadPosts(ByVal id As Integer) As String
        Dim command As Common.DbCommand = DatabaseEngine.GenerateDbCommand("SELECT ID FROM board WHERE (parentT = @id) AND (mta = @mta)")
        command.Parameters.Add(MakeParameter("@id", id, Data.DbType.Int32))
        command.Parameters.Add(MakeParameter("@mta", False, Data.DbType.Boolean))
        Dim q As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
        Dim il As New List(Of Integer)
        While q.Reader.Read
            il.Add(q.Reader.GetInt32(0))
        End While
        q.Reader.Close()
        q.Connection.Close()
        Return JsonConvert.SerializeObject(il, Formatting.None)
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

    Private Class PublicCatalogItem
        Public ID As Integer
        Public HasFiles As Boolean
        Public Files As WPostImage() = {}
        Public Name As String
        Public Email As String
        Public Subject As String
        Public Comment As String
        Public posterId As String
        Public Time As DateTime
    End Class

    Public Function GetSELinks() As String
        Return JsonConvert.SerializeObject(searchEngineLinkList, Formatting.None)
    End Function

    Public Function JSON_GetCatalog() As String
        ''Todo: Add page number.
        Dim il As New List(Of PublicCatalogItem)
        Dim threads As Integer() = GetThreads(0, (MaximumPages * ThreadPerPage), False, False)
        Dim wpostlist As WPost() = GetWpostList(threads)


        'For o As Integer = 1 To MaximumPages Step 1
        '    Dim ila As New List(Of PublicCatalogItem)

        '    For u As Integer = o To ThreadPerPage - 1

        '        Dim i As New PublicCatalogItem
        '        Dim x As WPost = wpostlist(u)
        '        i.Comment = ProcessComment(x, "default", Nothing)
        '        i.Email = x.email
        '        i.Time = x.time
        '        i.Files = x.files
        '        i.ID = x.PostID
        '        i.posterId = x.posterID
        '        i.HasFiles = x.HasFile
        '        i.Name = x.name
        '        i.Subject = x.subject
        '        ila.Add(i)
        '    Next

        '    il.Add(ila.ToArray())
        'Next

        For Each x As WPost In wpostlist
            Dim i As New PublicCatalogItem
            i.Comment = ProcessComment(x, "default", Nothing)
            i.Email = x.email
            i.Time = x.time
            i.Files = x.files
            i.ID = x.PostID
            i.posterId = x.posterID
            i.HasFiles = x.HasFile
            i.Name = x.name
            i.Subject = x.subject
            il.Add(i)
        Next
        Return JsonConvert.SerializeObject(il, Formatting.None)
    End Function

    Public Function JSON_GetCatalog_ID_Only() As String
        Return JsonConvert.SerializeObject(GetThreads(0, (MaximumPages * ThreadPerPage), False, False), Formatting.None)
    End Function

    Public Function JSON_GetPageThreads_ID_Only(ByVal pageIndex As Integer) As String
        pageIndex = pageIndex * ThreadPerPage
        Dim threads As Integer() = GetThreads(pageIndex, ThreadPerPage - 1 + pageIndex, False, False)
        Return JsonConvert.SerializeObject(threads, Formatting.None)
    End Function

    Public Function JSON_GetPageThreads(ByVal pageIndex As Integer) As String
        pageIndex = pageIndex * ThreadPerPage
        Dim threads As Integer() = GetThreads(pageIndex, ThreadPerPage - 1 + pageIndex, False, False)
        Dim il As New List(Of PublicCatalogItem)
        For Each x As WPost In GetWpostList(threads)
            Dim i As New PublicCatalogItem
            i.Comment = ProcessComment(x, "default", Nothing)
            i.Email = x.email
            i.Time = x.time
            i.Files = x.files
            i.ID = x.PostID
            i.posterId = x.posterID
            i.HasFiles = x.HasFile
            i.Name = x.name
            i.Subject = x.subject
            il.Add(i)
        Next
        Return JsonConvert.SerializeObject(il, Formatting.None)
    End Function

End Module
