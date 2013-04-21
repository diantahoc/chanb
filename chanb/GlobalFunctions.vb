Imports System.Data.OleDb

Public Module GlobalFunctions
    
    Dim ConnectionString As String = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=D:\d\tiny.accdb" ' Replace D:\d\tiny.accdb with your physical database path

    Function GetUserSelectedStyle(ByVal session As Web.SessionState.HttpSessionState) As String
        If session.Item("SS") = "" Then
            session.Item("SS") = "chan"
            Return "chan"
        Else
            Return session.Item("SS")
        End If
    End Function

    Function GetSessionPassword(ByVal seesion As Web.SessionState.HttpSessionState) As String
        If seesion("pass") = "" Then
            seesion("pass") = New String(seesion.SessionID, 0, 15)
            Return seesion("pass")
        Else
            Return seesion("pass")
        End If
    End Function

    Private Function ConvertNoNull(ByVal x As Object) As Object
        If TypeOf x Is DBNull Then
            Return Nothing
        Else
            Return x
        End If
    End Function

    Function FetchPostData(ByVal id As Long) As WPost
        Dim cnx As New OleDbConnection(ConnectionString)

        Dim queryString As String = "SELECT type, [time], comment, postername, email, [password], parentT, subject, imagename, IP FROM(board) WHERE(id = " & id & ")"
        Dim queryObject As New OleDbCommand(queryString, cnx)
        cnx.Open()
        Dim reader As OleDbDataReader = queryObject.ExecuteReader
        Dim po As New WPost(id)
        While reader.Read
            po.type = ConvertNoNull(reader(0))
            po.time = ConvertNoNull(reader(1))
            po.comment = ConvertNoNull(reader(2))
            po.name = ConvertNoNull(reader(3))
            po.email = ConvertNoNull(reader(4))
            po.password = ConvertNoNull(reader(5))
            po.parent = ConvertNoNull(reader(6))
            po.subject = ConvertNoNull(reader(7))
            po._imageP = ConvertNoNull(reader(8))
            po.ip = ConvertNoNull(reader(9))
        End While
        Return po
        reader.Close()
        cnx.Close()
    End Function

    Sub MakeThread(ByVal data As OPDATA)
        Dim cnx As New OleDbConnection(ConnectionString)
        Dim queryString As String = "INSERT INTO board(type, [time], comment, postername, email, [password], subject, imagename, IP, bumplevel) VALUES ('0', " & ConvertTimeToOLETIME(data.time) & ", '" & data.Comment & "', '" & data.name & "', '" & data.email & "', '" & data.password & "', '" & data.subject & "', '" & data.imageName & "','" & data.IP & "', " & CInt(GetThreadsCount() + 1) & ") "
        Dim queryObject As New OleDbCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Private Function GetThreadsCount() As Integer
        Dim cnx As New OleDbConnection(ConnectionString)
        Dim queryString As String = "SELECT ID FROM(board) WHERE(type = 0)"
        Dim queryObject As New OleDbCommand(queryString, cnx)
        cnx.Open()
        Dim i As Integer = 0
        Dim reader As OleDbDataReader = queryObject.ExecuteReader
        While reader.Read
            i += 1
        End While
        cnx.Close()
        Return i
    End Function

    Private Function ConvertTimeToOLETIME(ByVal d As Date) As String
        '#12/12/2012 8:51:00 AM#
        '#MM/DD/YYYY H:MM:SS PMAM#
        Dim s As String = "#" & d.Month & "/" & d.Day & "/" & d.Year & " "
        If d.Hour > 12 Then
            Dim t As String = d.Hour - 12 & ":" & d.Minute & ":" & d.Second & " PM"
            s = s & t & "#"
        Else
            Dim t As String = d.Hour & ":" & d.Minute & ":" & d.Second & " AM"
            s = s & t & "#"
        End If
        Return s
    End Function


    ''' <summary>
    ''' Append a post to thread
    ''' </summary>
    ''' <param name="id">id of the thread</param>
    ''' <param name="data"></param>
    ''' <remarks></remarks>
    Private Sub ReplyTo(ByVal id As Long, ByVal data As OPDATA)
        Dim cnx As New OleDbConnection(ConnectionString)
        Dim queryString As String = "INSERT INTO board (type, [time], comment, postername, email, [password], parentT, subject, imagename, IP) VALUES      ('1', " & ConvertTimeToOLETIME(data.time) & ", '" & data.Comment & "', '" & data.name & "', '" & data.email & "', '" & data.password & "', '" & id & "', '" & data.subject & "', '" & data.imageName & "' , '" & data.IP & "' )"
        Dim queryObject As New OleDbCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
        BumpThread(id, 1)
    End Sub

    Private Sub BumpThread(ByVal id As Integer, ByVal howmuch As Integer)
        Dim cnx As New OleDbConnection(ConnectionString)
        Dim queryString As String = "SELECT bumplevel FROM(board) WHERE(id = " & id & ")"
        Dim queryObject As New OleDbCommand(queryString, cnx)
        cnx.Open()
        Dim reader As OleDbDataReader = queryObject.ExecuteReader
        Dim bumplevel As Integer = 0
        While reader.Read
            bumplevel = reader(0)
        End While
        bumplevel += howmuch
        Dim updateQueryString As String = "UPDATE board SET bumplevel = " & bumplevel & " WHERE(board.ID = " & id & ")"
        Dim q As New OleDbCommand(updateQueryString, cnx)
        q.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Function GetThreadChildrenPosts(ByVal id As Long) As Integer()
        Dim ila As New List(Of Integer)
        Dim cnx As New OleDbConnection(ConnectionString)
        Dim queryString As String = "SELECT ID FROM(board) WHERE (parentT = '" & id & "') ORDER BY ID"
        Dim queryObject As New OleDbCommand(queryString, cnx)
        cnx.Open()
        Dim reader As OleDbDataReader = queryObject.ExecuteReader()
        While reader.Read
            ila.Add(reader.Item(0))
        End While
        reader.Close()
        cnx.Close()
        Return ila.ToArray
    End Function

    Function GetThreads(ByVal startIndex As Integer, ByVal count As Integer) As Integer()
        Dim ila As New List(Of Integer)
        Dim cnx As New OleDbConnection(ConnectionString)
        Dim queryString As String = "SELECT ID FROM(board) WHERE (type = 0) ORDER BY bumplevel DESC"
        Dim queryObject As New OleDbCommand(queryString, cnx)
        cnx.Open()
        Dim reader As OleDbDataReader = queryObject.ExecuteReader
        While reader.Read
            ila.Add(reader.Item(0))
        End While
        reader.Close()
        cnx.Close()
        'Ole does not seem to support the SQL Limit function
        Dim il As New List(Of Integer)
        For i As Integer = startIndex To count Step 1
            Try
                il.Add(ila.Item(i))
            Catch ex As Exception
            End Try
        Next
        ila.Clear()
        Return il.ToArray
        il.Clear()
    End Function

    ''' <summary>
    ''' Save single file
    ''' </summary>
    ''' <param name="f"></param>
    ''' <param name="checksize"></param>
    ''' <param name="reply"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function saveFile(ByVal f As HttpPostedFile, ByVal reply As Boolean) As String
        'Check if the file is a valid image
        Try
            Dim i As Drawing.Image = Drawing.Image.FromStream(f.InputStream)
            i.Dispose()
        Catch ex As Exception
            'No image is required when replying
            If f.ContentLength = 0 And reply = True Then
                Return ""
                Exit Function
            Else
                Throw New ArgumentException("Bad image data")
            End If
        End Try

        'Actuall saving code start here
        Dim dd As String = Date.UtcNow.ToFileTime
        Dim p As String = STORAGEFOLDER & "\" & dd & "." & f.FileName.Split(CChar(".")).ElementAt(f.FileName.Split(CChar(".")).Length - 1)
        Dim thumb As String = STORAGEFOLDER & "\th" & dd & ".png"
        Dim w As Drawing.Image = Drawing.Image.FromStream(f.InputStream)
        ResizeImage(w, 250).Save(thumb)
        f.SaveAs(p)
        'chanb name : size in bytes : dimensions : realname 
        Dim sp As String = dd & "." & f.FileName.Split(CChar(".")).ElementAt(f.FileName.Split(CChar(".")).Length - 1) & ":" & f.ContentLength & ":" & w.Size.ToString & ":" & f.FileName & ":" & MD5(f.InputStream)
        w.Dispose()
        Return sp
    End Function

    Private Function GetWPOSTIMAGE(ByVal sp As String) As WPostImage
        Dim wp As New WPostImage
        wp.chanbName = sp.Split(CChar(":")).ElementAt(0)
        wp.size = sp.Split(CChar(":")).ElementAt(1)
        wp.dimensions = sp.Split(CChar(":")).ElementAt(2)
        wp.realname = sp.Split(CChar(":")).ElementAt(3)
        wp.md5 = sp.Split(CChar(":")).ElementAt(4)
        Return wp
    End Function

    ''' <summary>
    ''' Save multiple files.
    ''' </summary>
    ''' <param name="li"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function saveMFile(ByVal li As HttpFileCollection) As String
        Dim s As New StringBuilder
        Dim list As New List(Of String)
        For Each a As String In li.Keys
            Dim f As HttpPostedFile = li.Item(a)
            'Check for valid image
            Dim er As Boolean = False
            Try
                Dim i As Drawing.Image = Drawing.Image.FromStream(f.InputStream)
                i.Dispose()
            Catch ex As Exception
                If f.ContentLength > 0 Then Throw New ArgumentException("Bad image data")
                er = True
            End Try

            Dim dd As String = Date.UtcNow.ToFileTime
            If Not er Then
                'Check file size
                If f.ContentLength > MaximumFileSize Then
                    Throw New ArgumentException(FileToBig)
                    Return ""
                    Exit Function
                Else
                    Dim p As String = STORAGEFOLDER & "\" & dd & "." & f.FileName.Split(CChar(".")).ElementAt(f.FileName.Split(CChar(".")).Length - 1)
                    Dim thumb As String = STORAGEFOLDER & "\th" & dd & ".png"
                    Dim i As Drawing.Image = Drawing.Image.FromStream(f.InputStream)
                    ResizeImage(i, 250).Save(thumb)
                    f.SaveAs(p)
                    'chanb name : size in bytes : dimensions : realname 
                    Dim sp As String = dd & "." & f.FileName.Split(CChar(".")).ElementAt(f.FileName.Split(CChar(".")).Length - 1) & ":" & f.ContentLength & ":" & i.Size.ToString & ":" & f.FileName & ":" & MD5(f.InputStream)
                    i.Dispose()
                    list.Add(sp)
                    list.Add(";")
                End If
            End If
        Next
        'Remove the last ';'
        list.RemoveAt(list.Count - 1)
        For Each x In list
            s.Append(x)
        Next
        list.Clear()
        Return s.ToString
    End Function

    Private Function DownSizeWithAspectRatio(ByVal targetMax As Integer, ByVal iSi As Drawing.Size) As Drawing.Size
        Dim ratioP As Integer = iSi.Width / targetMax
        Return New Drawing.Size(Fix(iSi.Width / ratioP), Fix(iSi.Height / ratioP))
    End Function

    Private Function ResizeImage(ByVal i As Drawing.Image, ByVal targetS As Integer) As Drawing.Image
        Return i.GetThumbnailImage(DownSizeWithAspectRatio(targetS, i.Size).Width, DownSizeWithAspectRatio(targetS, i.Size).Height, Nothing, System.IntPtr.Zero)
    End Function

    ''' <summary>
    ''' For full image
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetImageWEBPATH(ByVal name As String) As String
        Return StoragefolderWEB & name
    End Function

    ''' <summary>
    ''' For thumbnails
    ''' </summary>
    ''' <param name="name"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetImageWEBPATHRE(ByVal name As String) As String
        Return StoragefolderWEB & "th" & name.Split(".").ElementAt(0) & ".png"
    End Function

    'Sub GenerateThumbsForOldImages()
    '    For Each x As IO.FileInfo In FileIO.FileSystem.GetDirectoryInfo(STORAGEFOLDER).GetFiles
    '        If x.Name.StartsWith("th") Then
    '        Else
    '            Dim thumb As String = STORAGEFOLDER & "\th" & x.Name.Split(CChar(".")).ElementAt(0) & ".png"
    '            ResizeImage(Drawing.Image.FromFile(x.FullName), 250).Save(thumb)
    '        End If
    '    Next
    'End Sub

    Function ProcessPost(ByVal request As HttpRequest) As String
        Dim sb As New StringBuilder
        sb.Append("<html><head>")
        Select Case request.Item("mode")
            Case "thread"
                If request.Files.Count = 0 Then
                    sb.Append(ImageRequired)
                    sb.Append("<meta HTTP-EQUIV='REFRESH' content='2; url=default.aspx'>")
                Else
                    'Save file.
                    If request.Files("ufile").ContentLength = 0 Then
                        sb.Append("Bad or no image")
                    Else
                        'Check file size before saving.
                        If request.Files("ufile").ContentLength > MaximumFileSize Then
                            sb.Append(FileToBig)
                            sb.Append("<meta HTTP-EQUIV='REFRESH' content='10; url=default.aspx'>")
                            Exit Select
                        End If

                        Dim s As String = saveFile(request.Files("ufile"), False)

                        Dim er As New OPDATA

                        er.Comment = request.Item("comment")
                        er.email = request.Item("email")
                        If request.Item("postername") = "" Then er.name = AnonName Else er.name = request.Item("postername")
                        er.subject = request.Item("subject")

                        er.time = Date.UtcNow
                        er.imageName = s
                        er.password = request.Item("password")
                        er.IP = request.UserHostAddress
                        MakeThread(er)

                        sb.Append(SuccessfulPostString)
                        sb.Append("<meta HTTP-EQUIV='REFRESH' content='2; url=default.aspx'>")

                    End If
                End If
            Case "reply"
                Dim s As String = ""
                If request.Files.Count = 0 Then
                    s = ""
                Else
                    If request.Files.Count = 1 Then
                        s = saveFile(request.Files(0), True)
                    ElseIf request.Files.Count > 1 Then
                        s = saveMFile(request.Files)
                    End If
                End If
                Dim er As New OPDATA
                er.Comment = request.Item("comment")
                er.email = request.Item("email")
                If request.Item("postername") = "" Then er.name = AnonName Else er.name = request.Item("postername")
                er.subject = request.Item("subject")

                er.time = Date.UtcNow
                er.imageName = s
                er.password = request.Item("password")
                er.IP = request.UserHostAddress
                ReplyTo(request.Item("threadid"), er)

                sb.Append(SuccessfulPostString)
                sb.Append("<meta HTTP-EQUIV='REFRESH' content='2; url=dispatcher.aspx?id=" & request.Item("threadid") & "'>")
            Case "report"  
                For Each x As String In request.QueryString
                    If x.StartsWith("proc") Then
                        ReportPost(CInt(x.Replace("proc", "")), request.UserHostAddress, Date.UtcNow)
                    End If
                Next     
            Case "delete"
                Dim li As New List(Of String)
                Dim deletPass As String = request.Item("deletePass")
                For Each x As String In request.QueryString
                    If x.StartsWith("proc") Then
                        li.Add(x.Replace("proc", ""))
                    End If
                Next
                For Each x In li
                    Dim p As WPost = FetchPostData(x)
                    If p.password = deletPass Then
                        DeletePost(x)
                        sb.Append("Post number " & x & " has been deleted")
                    Else
                        sb.Append("Cannot delete post " & x & " , bad password")
                    End If
                    sb.Append("<br/>")
                Next
            Case Else
                sb.Append("Invalid Posting mode")
                sb.Append("<meta HTTP-EQUIV='REFRESH' content='2; url=default.aspx'>")
        End Select
        sb.Append("</head></html>")
        Return sb.ToString
    End Function

    Private Function MD5(ByVal s As IO.Stream) As String
        Return "MD5"
    End Function

    Private Function ProcessComment(ByVal comment As String) As String
        Return comment
    End Function

    Function GetRepliesHTML(ByVal threadID As Integer) As String
        Dim sa As New StringBuilder
        For Each x In GetThreadChildrenPosts(threadID)
            Dim po As WPost = (FetchPostData(x))
            Dim postHTML As String = postTemplate
            postHTML = postHTML.Replace("%ID%", po.PostID)
            postHTML = postHTML.Replace("%POST TEXT%", ProcessComment(po.comment))
            postHTML = postHTML.Replace("%DATE TEXT UTC%", po.time)
            postHTML = postHTML.Replace("%SUBJECT%", po.subject)
            postHTML = postHTML.Replace("%NAME%", po.name)
            postHTML = postHTML.Replace("%DATE UTC UNIX%", po.time.ToFileTime)
            postHTML = postHTML.Replace("%POST LINK%", "dispatcher.aspx?id=" & po.PostID)
            Dim imagesHTML As String = ""
            Dim sb As New StringBuilder
            If Not (po._imageP = "") Then
                For Each ima In po._imageP.Split(CChar(";"))

                    Dim r As String = imageTemplate
                    Dim wpi As WPostImage = GetWPOSTIMAGE(ima.Replace(";", ""))
                    r = r.Replace("%ID%", po.PostID)
                    r = r.Replace("%FILE NAME%", wpi.realname)
                    r = r.Replace("%IMAGE SRC%", GetImageWEBPATH(wpi.chanbName))
                    r = r.Replace("%FILE SIZE%", wpi.size)
                    r = r.Replace("%IMAGE SIZE%", wpi.dimensions)
                    r = r.Replace("%THUMB_LINK%", GetImageWEBPATHRE(wpi.chanbName))
                    r = r.Replace("%IMAGE MD5%", wpi.md5)
                    sb.Append(r)
                Next
                imagesHTML = sb.ToString
            End If
            postHTML = postHTML.Replace("%IMAGES%", imagesHTML)
            sa.Append(postHTML)
        Next
        Return sa.ToString
    End Function

    Private Sub DeleteAllPosts()
        Dim cnx As New OleDbConnection(ConnectionString)
        Dim queryString As String = "DELETE FROM board"
        Dim queryObject As New OleDbCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Private Sub ReportPost(ByVal id As Integer, ByVal reporterIP As String, ByVal time As Date)
        Dim cnx As New OleDbConnection(ConnectionString)
        Dim queryString As String = "INSERT INTO reports  postID, reporterIP, [time]) VALUES (" & id & ", '" & reporterIP & "', " & ConvertTimeToOLETIME(time) & ")"
        Dim queryObject As New OleDbCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Private Sub DeletePost(ByVal id As Integer)
        Dim cnx As New OleDbConnection(ConnectionString)
        Dim queryString As String = "DELETE FROM board WHERE(id = " & id & ")"
        Dim queryObject As New OleDbCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

End Module
