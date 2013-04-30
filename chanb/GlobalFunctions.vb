Imports System.Data.SqlClient

Public Module GlobalFunctions

    Dim SQLConnectionString As String = "" ' Replace with your connection string

    Dim htmlElements As String() = {"<!--", "<!DOCTYPE>", "<a>", "<abbr>", "<acronym>", "<address>", "<applet>", "<area>", "<article>", "<aside>", "<audio>", "<b>", "<base>", "<basefont>", "<bdi>", "<bdo>", "<big>", "<blockquote>", "<body>", "<br>", "<canvas>", "<caption>", "<center>", "<cite>", "<code>", "<col>", "<colgroup>", "<command>", "<datalist>", "<dd>", "<del>", "<details>", "<dfn>", "<dialog>", "<dir>", "<div>", "<dl>", "<dt>", "<em>", "<embed>", "<fieldset>", "<figcaption>", "<figure", "<font>", "<footer>", "<form>", "<frame>", "<frameset>", "<h1>", "<h2>", "<h3>", "<h4>", "<h5>", "<h6>", "<head>", "<header>", "<hgroup>", "<hr>", "<html>", "<i>", "<iframe>", "<img>", "<input>", "<ins>", "<kbd>", "<keygen>", "<label>", "<legend>", "<li>", "<link>", "<map>", "<mark>", "<menu>", "<meta>", "<meter>", "<nav>", "<noframes>", "<noscript>", "<object>", "<ol>", "<optgroup>", "<option>", "<output>", "<p>", "<param>", "<pre>", "<progress>", "<q>", "<rp>", "<rt>", "<ruby>", "<s>", "<samp>", "<script>", "<section>", "<select>", "<small>", "<source>", "<span>", "<strike>", "<strong>", "<style>", "<sub>", "<summary>", "<sup>", "<table>", "<tbody>", "<td>", "<textarea>", "<tfoot>", "<th>", "<thead>", "<time>", "<title>", "<tr>", "<track>", "<tt>", "<u>", "<ul>", "<var>", "<video>", "<wbr>", "</a>", "</abbr>", "</acronym>", "</address>", "</applet>", "</area>", "</article>", "</aside>", "</audio>", "</b>", "</base>", "</basefont>", "</bdi>", "</bdo>", "</big>", "</blockquote>", "</body>", "</br>", "</canvas>", "</caption>", "</center>", "</cite>", "</code>", "</col>", "</colgroup>", "</command>", "</datalist>", "</dd>", "</del>", "</details>", "</dfn>", "</dialog>", "</dir>", "</div>", "</dl>", "</dt>", "</em>", "</embed>", "</fieldset>", "</figcaption>", "</figure", "</font>", "</footer>", "</form>", "</frame>", "</frameset>", "</h1>", "</h2>", "</h3>", "</h4>", "</h5>", "</h6>", "</head>", "</header>", "</hgroup>", "</hr>", "</html>", "</i>", "</iframe>", "</img>", "</input>", "</ins>", "</kbd>", "</keygen>", "</label>", "</legend>", "</li>", "</link>", "</map>", "</mark>", "</menu>", "</meta>", "</meter>", "</nav>", "</noframes>", "</noscript>", "</object>", "</ol>", "</optgroup>", "</option>", "</output>", "</p>", "</param>", "</pre>", "</progress>", "</q>", "</rp>", "</rt>", "</ruby>", "</s>", "</samp>", "</script>", "</section>", "</select>", "</small>", "</source>", "</span>", "</strike>", "</strong>", "</style>", "</sub>", "</summary>", "</sup>", "</table>", "</tbody>", "</td>", "</textarea>", "</tfoot>", "</th>", "</thead>", "</time>", "</title>", "</tr>", "</track>", "</tt>", "</u>", "</ul>", "</var>", "</video>", "</wbr>"}

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

    Private Function ProcessInputs(ByVal x As String) As String
        Dim sqlElements As String() = {"'+", "+'", "'<", "<'", "'>", ">'", "'+'", "'<'", "'>'", "'", "/'", "'/'", "'/", "<script>", "</script>"}
        Dim lowcaseX As String = x
        For Each a In htmlElements
            lowcaseX = lowcaseX.Replace(a.ToLower, "")
            lowcaseX = lowcaseX.Replace(a, "")
            lowcaseX = lowcaseX.Replace(a.ToUpper, "")
        Next
        For Each a In sqlElements
            lowcaseX = lowcaseX.Replace(a.ToLower, "")
            lowcaseX = lowcaseX.Replace(a.ToUpper, "")
        Next
        lowcaseX = lowcaseX.Replace(My.Resources.doublequotes, "")
        Return lowcaseX
    End Function

    Function FetchPostData(ByVal id As Long) As WPost
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT type, time, comment, postername, email, password, parentT, subject, imagename, IP FROM  board  WHERE (id = " & id & ")"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader
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
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "INSERT INTO board (type, time, comment, postername, email, password, subject, imagename, IP, bumplevel) VALUES ('0', " & ConvertTimeToSQLTIME(data.time) & ", '" & data.Comment & "', '" & data.name & "', '" & data.email & "', '" & data.password & "', '" & data.subject & "', '" & data.imageName & "','" & data.IP & "', " & CInt(GetThreadsCount() + 1) & ") "
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Function GetThreadsCount() As Integer
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT ID FROM board  WHERE(type = 0)"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim i As Integer = 0
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        While reader.Read
            i += 1
        End While
        cnx.Close()
        Return i
    End Function

    'Private Function ConvertTimeToOLETIME(ByVal d As Date) As String
    '    '#12/12/2012 8:51:00 AM#
    '    '#MM/DD/YYYY H:MM:SS PMAM#
    '    Dim s As String = "#" & d.Month & "/" & d.Day & "/" & d.Year & " "
    '    If d.Hour > 12 Then
    '        Dim t As String = d.Hour - 12 & ":" & d.Minute & ":" & d.Second & " PM"
    '        s = s & t & "#"
    '    Else
    '        Dim t As String = d.Hour & ":" & d.Minute & ":" & d.Second & " AM"
    '        s = s & t & "#"
    '    End If
    '    Return s
    'End Function

    Private Function ConvertTimeToSQLTIME(ByVal d As Date) As String
        ''4/23/2013 5:45:45 PM'
        ' 'MM/DD/YYY H:MM:SS AMPM'
        Dim s As String = "'" & d.Month & "/" & d.Day & "/" & d.Year & " " '& d.Hour & ":" & d.Minute & ":" & d.Second & "'"
        If d.Hour > 12 Then
            Dim t As String = d.Hour - 12 & ":" & d.Minute & ":" & d.Second & " PM"
            s = s & t & "'"
        Else
            Dim t As String = d.Hour & ":" & d.Minute & ":" & d.Second & " AM"
            s = s & t & "'"
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
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "INSERT INTO board (type, [time], comment, postername, email, [password], parentT, subject, imagename, IP) VALUES      ('1', " & ConvertTimeToSQLTIME(data.time) & ", '" & data.Comment & "', '" & data.name & "', '" & data.email & "', '" & data.password & "', '" & id & "', '" & data.subject & "', '" & data.imageName & "' , '" & data.IP & "' )"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
        If Not data.email = "sage" Then BumpThread(id, 1)
    End Sub

    Private Sub BumpThread(ByVal id As Integer, ByVal howmuch As Integer)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT bumplevel FROM board  WHERE(id = " & id & ")"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        Dim bumplevel As Integer = 0
        While reader.Read
            bumplevel = reader(0)
        End While
        bumplevel += howmuch
        reader.Close()
        Dim updateQueryString As String = "UPDATE board SET bumplevel = " & bumplevel & " WHERE(board.ID = " & id & ")"
        Dim q As New SqlCommand(updateQueryString, cnx)
        q.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Function GetThreadChildrenPosts(ByVal id As Long) As Integer()
        Dim ila As New List(Of Integer)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT ID FROM board  WHERE (parentT = " & id & ") ORDER BY ID"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader()
        While reader.Read
            ila.Add(reader.Item(0))
        End While
        reader.Close()
        cnx.Close()
        Return ila.ToArray
    End Function

    Function GetThreads(ByVal startIndex As Integer, ByVal count As Integer) As Integer()
        Dim ila As New List(Of Integer)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT ID FROM board  WHERE (type = 0) ORDER BY bumplevel DESC"
        Dim queryObject As New sqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        While reader.Read
            ila.Add(reader.Item(0))
        End While
        reader.Close()
        cnx.Close()
        'MS SQL does not seem to support the MySQL Limit startIndex, count function
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

    Function IsModLoginValid(ByVal name As String, ByVal password As String)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT password FROM mods  WHERE (username LIKE '" & name & "')"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim sqlPassMd5 As String = ""
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        While reader.Read
            sqlPassMd5 = ConvertNoNull(reader(0))
        End While
        reader.Close()
        cnx.Close()
        Return (MD5(password) = sqlPassMd5)
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
        If w.Width < 250 Then
            w.Save(thumb)
        Else
            ResizeImage(w, 250).Save(thumb)
        End If
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
                    If i.Width < 250 Then
                        i.Save(thumb)
                    Else
                        ResizeImage(i, 250).Save(thumb)
                    End If
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
        If list.Count > 1 Then list.RemoveAt(list.Count - 1)
        For Each x In list
            s.Append(x)
        Next
        list.Clear()
        Return s.ToString
    End Function

    Private Function DownSizeWithAspectRatio(ByVal targetMax As Integer, ByVal isi As Drawing.Size) As Drawing.Size
        Dim ratioP As Integer = isi.Width / targetMax
        Return New Drawing.Size(Fix(isi.Width / ratioP), Fix(isi.Height / ratioP))
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

    Sub NewMod(ByVal name As String, ByVal pas As String)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "INSERT INTO mods (username, password) VALUES ('" & name & "', '" & MD5(pas) & "')"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    'Sub GenerateThumbsForOldImages()
    '    For Each x As IO.FileInfo In FileIO.FileSystem.GetDirectoryInfo(STORAGEFOLDER).GetFiles
    '        If x.Name.StartsWith("th") Then
    '        Else
    '            Dim thumb As String = STORAGEFOLDER & "\th" & x.Name.Split(CChar(".")).ElementAt(0) & ".png"
    '            ResizeImage(Drawing.Image.FromFile(x.FullName), 250).Save(thumb)
    '        End If
    '    Next
    'End Sub

    Function IsIPBanned(ByVal IP As String) As Boolean
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT ID FROM bans WHERE (IP LIKE '" & IP & "')"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        Dim banned As Boolean = False
        While reader.Read
            If TypeOf reader(0) Is DBNull Then
                banned = False
                'User is not banned since there is no ID entry for the specified IP address
            Else
                banned = True
            End If
        End While
        reader.Close()
        cnx.Close()
        Return banned
    End Function

    Private Function GetBanData(ByVal IP As String) As BanData
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT ID, perm, expiry, comment, post FROM bans WHERE (IP LIKE '" & IP & "')"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        Dim data As New BanData
        While reader.Read
            data.ID = ConvertNoNull(reader(0))
            data.PERM = ConvertNoNull(reader(1))
            data.EXPIRY = ConvertNoNull(reader(2))
            data.COMMENT = ConvertNoNull(reader(3))
            data.POSTNO = ConvertNoNull(reader(4))
        End While
        data.IP = IP
        reader.Close()
        cnx.Close()
        Return data
    End Function

    Private Function MakeBannedMessage(ByVal IP As String) As String
        Return "You are banned!"
    End Function

    Function ProcessPost(ByVal request As HttpRequest, ByVal Session As Web.SessionState.HttpSessionState) As String
        Dim sb As New StringBuilder
        sb.Append("<html><head>")
        Dim cont As Boolean = True
        If Session.Item("lastpost") Is "" Or Session.Item("lastpost") Is Nothing Then
            Session.Item("lastpost") = Now.ToString
        Else
            Dim i As Date = Date.Parse(Session.Item("lastpost"))
            If (Now - i).TotalSeconds < TimeBetweenRequestes Then
                sb.Append(FloodDetected)
                cont = False
            Else
                Session.Item("lastpost") = Now.ToString
            End If
        End If
        ''Post processing begin here 
        If cont Then
            If IsIPBanned(request.UserHostAddress) Then
                sb.Append(MakeBannedMessage(request.UserHostAddress))
            Else
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

                                Dim er As New OPData

                                er.Comment = ProcessInputs(request.Item("comment"))
                                er.email = ProcessInputs(request.Item("email"))
                                If request.Item("postername") = "" Then er.name = AnonName Else er.name = ProcessInputs(request.Item("postername"))
                                er.subject = ProcessInputs(request.Item("subject"))

                                er.time = Date.UtcNow
                                er.imageName = s
                                er.password = ProcessInputs(request.Item("password"))
                                er.IP = request.UserHostAddress

                                Session.Item("pass") = request.Item("password")

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
                        Dim er As New OPData
                        If ProcessInputs(request.Item("comment")) = "" And s = "" Then
                            'no image and no text
                            'blank post
                            sb.Append("Blank post are not allowed")
                        Else
                            er.Comment = ProcessInputs(request.Item("comment"))
                            er.email = ProcessInputs(request.Item("email"))
                            If request.Item("postername") = "" Then er.name = AnonName Else er.name = ProcessInputs(request.Item("postername"))
                            er.subject = ProcessInputs(request.Item("subject"))
                            er.time = Date.UtcNow
                            er.imageName = s
                            er.password = ProcessInputs(request.Item("password"))
                            er.IP = request.UserHostAddress
                            Session.Item("pass") = ProcessInputs(request.Item("password"))
                            ReplyTo(CInt(request.Item("threadid")), er)
                            sb.Append(SuccessfulPostString)
                        End If
                        sb.Append("<meta HTTP-EQUIV='REFRESH' content='2; url=default.aspx?id=" & request.Item("threadid") & "'>")
                    Case "report"
                        For Each x As String In request.QueryString
                            If x.StartsWith("proc") Then
                                ReportPost(CInt(x.Replace("proc", "")), request.UserHostAddress, Date.UtcNow)
                                sb.Append(ReportedSucess.Replace("%", x))
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
                        If li.Count = 0 Then
                            sb.Append(NoPostWasSelected)
                        Else
                            For Each x In li
                                Dim p As WPost = FetchPostData(x)
                                If p.password = deletPass Then
                                    DeletePost(x, DeleteFiles)
                                    sb.Append(PostDeletedSuccess.Replace("%", x))
                                Else
                                    sb.Append(CannotDeletePostBadPassword.Replace("%", x))
                                End If
                                sb.Append("<br/>")
                            Next
                        End If
                    Case Else
                        sb.Append("Invalid Posting mode")
                        sb.Append("<meta HTTP-EQUIV='REFRESH' content='2; url=default.aspx'>")
                End Select
            End If
        End If
        sb.Append("</head></html>")
        Return sb.ToString
    End Function

    Private Function MD5(ByVal s As IO.Stream) As String
        Dim md5s As New System.Security.Cryptography.MD5CryptoServiceProvider()
        Dim hash() As Byte = md5s.ComputeHash(s)
        Return ByteArrayToString(hash)
    End Function

    Private Function MD5(ByVal s As String) As String
        Dim md5s As New System.Security.Cryptography.MD5CryptoServiceProvider()
        Dim hash() As Byte = System.Text.Encoding.ASCII.GetBytes(s)
        Return ByteArrayToString(hash)
    End Function

    Private Function ByteArrayToString(ByVal arrInput() As Byte) As String
        Dim sb As New System.Text.StringBuilder(arrInput.Length * 2)
        For i As Integer = 0 To arrInput.Length - 1
            sb.Append(arrInput(i).ToString("X2"))
        Next
        Return sb.ToString().ToLower
    End Function

    Private Function ProcessComment(ByVal comment As String) As String
        'Dim sb As New StringBuilder
        'Dim li As String() = comment.Split(vbNewLine)
        'For Each x In li
        '    'Check if greentext
        '    If x.StartsWith(">") And Not x.StartsWith(">>") Then
        '        sb.Append("<span class='quote'>&gt;" & x.Replace(">", "") & "</span></br>")
        '    End If
        '    'Check if quote
        '    If x.StartsWith(">>") Then
        '        sb.Append("<a href='#'>" & x & "</a><br/>")
        '    End If
        '    'Check if normal
        '    If Not x.StartsWith(">") Then
        '        sb.Append(x)
        '    End If
        '    sb.Append("</br>")
        '    sb.Append(vbNewLine)
        'Next

        Dim v As String = comment
        For Each x In htmlElements
            v = v.Replace(x, "")
            v = v.Replace(x.ToUpper, "")
        Next
        Return v
    End Function

    Function GetOPPostHTML(ByVal id As Integer, ByVal replyButton As Boolean, ByVal isMod As Boolean) As String
        Dim po As WPost = FetchPostData(id)
        Dim imageData As WPostImage = GetWPOSTIMAGE(po._imageP)
        Dim postHTML As String = opPostTemplate
        If replyButton Then
            postHTML = postHTML.Replace("%REPLY BUTTON%", replyButtonHTML)
        Else
            postHTML = postHTML.Replace("%REPLY BUTTON%", "")
        End If
        postHTML = postHTML.Replace("%ID%", po.PostID)
        postHTML = postHTML.Replace("%IMAGE LINK%", GetImageWEBPATH(imageData.chanbName))
        postHTML = postHTML.Replace("%CHANB FILE NAME%", imageData.chanbName)
        postHTML = postHTML.Replace("%FILE NAME%", imageData.realname)
        postHTML = postHTML.Replace("%FILE SIZE%", imageData.size)
        postHTML = postHTML.Replace("%IMAGE DIMENSIONS%", imageData.dimensions)
        postHTML = postHTML.Replace("%THUMB LINK%", GetImageWEBPATHRE(imageData.chanbName))
        postHTML = postHTML.Replace("%MD5%", imageData.md5)
        postHTML = postHTML.Replace("%SUBJECT%", po.subject)
        postHTML = postHTML.Replace("%NAME%", po.name)
        postHTML = postHTML.Replace("%DATE UTC UNIX%", po.time.ToFileTime)
        postHTML = postHTML.Replace("%DATE UTC TEXT%", GetTimeString(po.time))
        postHTML = postHTML.Replace("%POST LINK%", "default.aspx?id=" & po.PostID & "#p" & po.PostID)
        postHTML = postHTML.Replace("%POST TEXT%", ProcessComment(po.comment))
        postHTML = postHTML.Replace("%REPLY COUNT%", GetRepliesCount(id))
        If isMod Then postHTML = postHTML.Replace("%MODPANEL%", "<a href='modaction.aspx?action=banpost&postid=" & po.PostID & "'>Ban</a><a href='modaction.aspx?action=delpost&id=" & po.PostID & "'>Delete</a>") Else postHTML = postHTML.Replace("%MODPANEL%", "")
        Return postHTML
    End Function

    Private Function GetLastXPosts(ByVal threadID As Integer, ByVal x As Integer) As Integer()
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim query As New SqlCommand("SELECT TOP " & x & " ID FROM board WHERE(parentT = " & threadID & ") ORDER BY ID DESC", cnx)
        cnx.Open()
        Dim il As New List(Of Integer)
        Dim reader As SqlDataReader = query.ExecuteReader
        While reader.Read
            il.Add(reader(0))
        End While
        reader.Close()
        cnx.Close()
        Return il.ToArray
    End Function

    Sub BanPosterByPost(ByVal postID As Integer)
        Dim po As WPost = FetchPostData(postID)
        If IsIPBanned(po.ip) = False Then
            Dim newText As String = po.comment & vbNewLine & "<br/><strong style='color: red;'>USER WAS BANNED FOR THIS POST</strong>"
            BanPoster(po.ip, postID)
            UpdatePostText(postID, newText)
        End If
    End Sub

    Private Sub UpdatePostText(ByVal postID As Integer, ByVal newText As String)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "UPDATE board SET comment = '\" & newText.Replace("'", "") & "' WHERE(ID = " & postID & ")"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Private Sub BanPoster(ByVal IP As String, ByVal postID As Integer)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "INSERT INTO bans (perm, post, IP) VALUES (0, " & postID & ", '" & IP & "')"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Function GetThreadHTML(ByVal threadID As Integer, ByVal isMod As Boolean, ByVal trailposts As Integer) As String
        Dim postHtml As String = threadTemplate
        postHtml = postHtml.Replace("%ID%", threadID)
        If trailposts > 0 Then
            Dim sb As New StringBuilder
            For Each x In GetLastXPosts(threadID, trailposts).Reverse
                sb.Append(GetSingleReplyHTML(x, isMod))
            Next
            postHtml = postHtml.Replace("%TRAILS%", sb.ToString)
        Else
            postHtml = postHtml.Replace("%TRAILS%", "")
        End If
        postHtml = postHtml.Replace("%POST HTML%", GetOPPostHTML(threadID, True, isMod))
        Dim repC As Integer = GetRepliesCount(threadID)
        If repC - trailposts <= 0 Then
            postHtml = postHtml.Replace("%COUNT%", 0)
            postHtml = postHtml.Replace("%AN%", "hide")
        Else
            postHtml = postHtml.Replace("%COUNT%", repC - trailposts)
            postHtml = postHtml.Replace("%AN%", "")
        End If
        postHtml = postHtml.Replace("%POSTLINK%", "default.aspx?id=" & threadID)
        Return postHtml
    End Function

    Private Function GetTimeString(ByVal d As Date) As String
        Return d.ToString
    End Function

    Private Function GetRepliesCount(ByVal threadID As Integer) As Integer
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT   type   FROM board WHERE (parentT=" & threadID & ")"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim i As Integer = 0
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        While reader.Read
            i += 1
        End While
        cnx.Close()
        Return i
    End Function

    Function GetRepliesHTML(ByVal threadID As Integer, ByVal isMod As Boolean) As String
        Dim sa As New StringBuilder
        Dim il As Integer() = GetThreadChildrenPosts(threadID)
        For Each x In GetThreadChildrenPosts(threadID)
            sa.Append(GetSingleReplyHTML(x, isMod))
        Next
        sa.Append("<hr></hr")
        Return sa.ToString
    End Function

    Private Function GetSingleReplyHTML(ByVal postid As Integer, ByVal isMod As Boolean) As String
        Dim po As WPost = (FetchPostData(postid))
        Dim postHTML As String = postTemplate
        If po.email = "" Then
            postHTML = postHTML.Replace("%NAMESPAN%", "<span class='name'>%NAME%</span>")
        Else
            postHTML = postHTML.Replace("%NAMESPAN%", "<a href='mailto:%EMAIL%' class='useremail'><span class='name'>%NAME%</span></a>")
        End If
        postHTML = postHTML.Replace("%EMAIL%", po.email)
        postHTML = postHTML.Replace("%ID%", po.PostID)
        postHTML = postHTML.Replace("%POST TEXT%", ProcessComment(po.comment))
        postHTML = postHTML.Replace("%DATE TEXT UTC%", GetTimeString(po.time))
        postHTML = postHTML.Replace("%SUBJECT%", po.subject)
        postHTML = postHTML.Replace("%NAME%", po.name)
        postHTML = postHTML.Replace("%DATE UTC UNIX%", po.time.ToFileTime)
        postHTML = postHTML.Replace("%POST LINK%", "default.aspx?id=" & po.parent & "#p" & po.PostID)
        If isMod Then postHTML = postHTML.Replace("%MODPANEL%", "<a href='modaction.aspx?action=banpost&postid=" & po.PostID & "'>Ban</a><a href='modaction.aspx?action=delpost&id=" & po.PostID & "'>Delete</a>") Else postHTML = postHTML.Replace("%MODPANEL%", "")
        postHTML = postHTML.Replace("%IMAGES%", GetImagesHTML(po))
        Return postHTML
    End Function

    Private Function GetImagesHTML(ByVal po As WPost) As String
        Dim sb As New StringBuilder
        If Not (po._imageP = "") Then
            'At least one image is found. Check for more than 2 images
            If po._imageP.Split(CChar(";")).Count > 1 Then
                'Add rotator script
                Dim items As New StringBuilder
                Dim count As Integer = po._imageP.Split(CChar(";")).Count
                Dim advanced As Boolean = False ' The first one is marked as active, the rest as notactive
                Dim rotatorTemplat As String = rotatorTemplate
                For Each ima In po._imageP.Split(CChar(";"))
                    Dim r As String = imageTemplate
                    Dim wpi As WPostImage = GetWPOSTIMAGE(ima.Replace(";", ""))
                    r = r.Replace("%ID%", po.PostID)
                    If Not advanced Then r = r.Replace("%AN%", "active") Else r = r.Replace("%AN%", "notactive")
                    r = r.Replace("%filec%", "")
                    r = r.Replace("%FILE NAME%", wpi.realname)
                    r = r.Replace("%IMAGE SRC%", GetImageWEBPATH(wpi.chanbName))
                    r = r.Replace("%FILE SIZE%", wpi.size)
                    r = r.Replace("%IMAGE SIZE%", wpi.dimensions)
                    r = r.Replace("%THUMB_LINK%", GetImageWEBPATHRE(wpi.chanbName))
                    r = r.Replace("%IMAGE MD5%", wpi.md5)
                    items.Append(r)
                    advanced = True
                Next
                rotatorTemplat = rotatorTemplat.Replace("%ID%", po.PostID)
                rotatorTemplat = rotatorTemplat.Replace("%IMAGECOUNT%", count)
                rotatorTemplat = rotatorTemplat.Replace("%ITEMS%", items.ToString)
                sb.Append(rotatorTemplat)
            Else
                'Single image
                Dim r As String = imageTemplate
                Dim wpi As WPostImage = GetWPOSTIMAGE(po._imageP.Replace(";", ""))
                r = r.Replace("%ID%", po.PostID)
                r = r.Replace("%filec%", "file")
                r = r.Replace("%AN%", "") ' No need for active/notactive class since there is no rotator.
                r = r.Replace("%FILE NAME%", wpi.realname)
                r = r.Replace("%IMAGE SRC%", GetImageWEBPATH(wpi.chanbName))
                r = r.Replace("%FILE SIZE%", wpi.size)
                r = r.Replace("%IMAGE SIZE%", wpi.dimensions)
                r = r.Replace("%THUMB_LINK%", GetImageWEBPATHRE(wpi.chanbName))
                r = r.Replace("%IMAGE MD5%", wpi.md5)
                sb.Append(r)
            End If
        Else
            'No image
        End If
        Return sb.ToString
    End Function

    Private Sub DeleteAllPosts()
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "TRUNCATE TABLE board"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Private Sub ReportPost(ByVal id As Integer, ByVal reporterIP As String, ByVal time As Date)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "INSERT INTO reports  (postID, reporterIP, time) VALUES (" & id & ", '" & reporterIP & "', " & ConvertTimeToSQLTIME(time) & ")"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Sub DeletePost(ByVal id As Integer, ByVal dF As Boolean)
        Dim w As WPost = FetchPostData(id)
        If w.type = 0 Then
            For Each x In GetThreadChildrenPosts(id)
                DeleteP(x, dF)
            Next
            DeleteP(id, dF)
        Else
            DeleteP(id, dF)
        End If
    End Sub

    Private Sub DeleteP(ByVal id As Integer, ByVal dF As Boolean)
        If deleteFiles Then DeletePostFiles(id)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "DELETE FROM board WHERE(id = " & id & ")"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Private Sub DeletePostFiles(ByVal postID As Integer)
        Dim w As WPost = FetchPostData(postID)
        If w._imageP = "" Then
            Exit Sub
        Else
            For Each x In w._imageP.Split(CChar(";"))
                Dim ima As WPostImage = GetWPOSTIMAGE(w._imageP)
                Dim realPath As String = STORAGEFOLDER & "\" & ima.chanbName
                Dim thumbPath As String = STORAGEFOLDER & "\th" & ima.chanbName
                IO.File.Delete(realPath)
                IO.File.Delete(thumbPath)
            Next
        End If
    End Sub

End Module
