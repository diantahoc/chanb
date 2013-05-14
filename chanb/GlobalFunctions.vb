Imports System.Data.SqlClient

#Const EnablePDF = True

Public Module GlobalFunctions

    Dim g As New DBInitializer
    Dim SQLConnectionString As String = g.ConnectionString

    Function GetUserSelectedStyle(ByVal session As Web.SessionState.HttpSessionState) As String
        If CStr(session.Item("SS")) = "" Then
            session.Item("SS") = "chan"
            Return "chan"
        Else
            Return CStr(session.Item("SS"))
        End If
    End Function

    Function GetSessionPassword(ByVal cookies As Web.HttpCookieCollection, ByVal session As Web.SessionState.HttpSessionState) As String
        If cookies("pass") Is Nothing Then
            Dim c As New HttpCookie("pass", New String(CType(session.SessionID, Char()), 0, 15))
            c.Expires = (Now + New TimeSpan(0, 60, 0))
            cookies.Add(c)
            Return cookies("pass").Value
        Else
            Return cookies("pass").Value
        End If
    End Function

    Private Function ConvertNoNull(ByVal x As Object) As Object
        If TypeOf x Is DBNull Then Return Nothing Else Return x
    End Function

    Private Function ProcessInputs(ByVal x As String) As String
        Dim lowcaseX As String = x
        'HTML ISO 8879 Numerical Character References 
        'http://sunsite.berkeley.edu/amher/iso_8879.html
        lowcaseX = lowcaseX.Replace("&", "&amp;")
        lowcaseX = lowcaseX.Replace("<", "&lt;")
        lowcaseX = lowcaseX.Replace(">", "&gt;")
        lowcaseX = lowcaseX.Replace("%", "&#37;")
        lowcaseX = lowcaseX.Replace("$", "&#36;")
        lowcaseX = lowcaseX.Replace("'", "&#39;")
        lowcaseX = lowcaseX.Replace("(", "&#40;")
        lowcaseX = lowcaseX.Replace(")", "&#41;")
        lowcaseX = lowcaseX.Replace("*", "&#42;")
        lowcaseX = lowcaseX.Replace("+", "&#43;")
        lowcaseX = lowcaseX.Replace("/", "&#47;")
        lowcaseX = lowcaseX.Replace(":", "&#58;")
        lowcaseX = lowcaseX.Replace("=", "&#61;")
        lowcaseX = lowcaseX.Replace("@", "&#64;")
        lowcaseX = lowcaseX.Replace("[", "&#91;")
        lowcaseX = lowcaseX.Replace("]", "&#93;")
        lowcaseX = lowcaseX.Replace("\", "&#92;")
        lowcaseX = lowcaseX.Replace("^", "&#94;")
        lowcaseX = lowcaseX.Replace("{", "&#123;")
        lowcaseX = lowcaseX.Replace("|", "&#124;")
        lowcaseX = lowcaseX.Replace("}", "&#125;")
        lowcaseX = lowcaseX.Replace("~", "&#126;")
        lowcaseX = lowcaseX.Replace(My.Resources.doublequotes, "&quot;")
        Return lowcaseX
    End Function

    Function FetchPostData(ByVal id As Long) As WPost
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT type, time, comment, postername, email, password, parentT, subject, imagename, IP, ua, posterID, sticky, locked, mta FROM  board  WHERE (id = " & id & ")"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        Dim po As New WPost(id)
        While reader.Read
            po.type = CStr(ConvertNoNull(reader(0)))
            po.time = CDate(ConvertNoNull(reader(1)))
            po.comment = CStr(ConvertNoNull(reader(2)))
            po.name = CStr(ConvertNoNull(reader(3)))
            po.email = CStr(ConvertNoNull(reader(4)))
            po.password = CStr(ConvertNoNull(reader(5)))
            po.parent = CInt(ConvertNoNull(reader(6)))
            po.subject = CStr(ConvertNoNull(reader(7)))
            po._imageP = CStr(ConvertNoNull(reader(8)))
            po.ip = CStr(ConvertNoNull(reader(9)))
            po.ua = CStr(ConvertNoNull(reader(10)))
            po.posterID = CStr(ConvertNoNull(reader(11)))
            If CInt(ConvertNoNull(reader(12))) = 1 Then po.isSticky = True Else po.isSticky = False
            If CInt(ConvertNoNull(reader(13))) = 1 Then po.locked = True Else po.locked = False
            If CInt(ConvertNoNull(reader(14))) = 1 Then po.archived = True Else po.archived = False
        End While
        Return po
        reader.Close()
        cnx.Close()
    End Function

    Private Sub MakeThread(ByVal data As OPData)
        Dim cnx As New SqlConnection(SQLConnectionString)
        'Insert thread data
        Dim queryString As String = "INSERT INTO board (type, time, comment, postername, email, password, subject, imagename, IP, bumplevel, ua, sticky, mta) VALUES ('0', " & ConvertTimeToSQLTIME(data.time) & ", N'" & data.Comment & "', N'" & data.name & "', N'" & data.email & "', N'" & data.password & "', N'" & data.subject & "', N'" & data.imageName & "','" & data.IP & "', " & ConvertTimeToSQLTIME(data.time) & ", '" & data.UserAgent & "', 0, 0 ) "
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        'Retrive thread ID
        queryString = "SELECT ID FROM board WHERE (IP LIKE '" & data.IP & "') AND (ua LIKE '" & data.UserAgent & "') AND (password = N'" & data.password & "') AND (time =  CONVERT(DATETIME, " & ConvertTimeToSQLTIME(data.time) & ", 102))"
        queryObject = New SqlCommand(queryString, cnx)
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        Dim postID As Integer
        While reader.Read
            postID = CInt(reader(0))
        End While
        reader.Close()
        'Update thread data with OP posterID
        queryObject = New SqlCommand("UPDATE board SET posterID = '" & GenerateUID(postID, data.IP) & "' WHERE (IP LIKE '" & data.IP & "') AND (ua LIKE '" & data.UserAgent & "') AND (password = N'" & data.password & "') AND (time =  CONVERT(DATETIME, " & ConvertTimeToSQLTIME(data.time) & ", 102))", cnx)
        queryObject.ExecuteNonQuery()
        cnx.Close()
        CheckForPrunedThreads()
    End Sub

    Private Sub CheckForPrunedThreads()
        Dim currentThread As Integer() = GetThreads(0, (MaximumPages * ThreadPerPage) - 1, True, False) ' list of thread that haven't reached the last page. 
        Dim allThread As Integer() = GetThreads(0, MaximumPages * ThreadPerPage, True, False) ' list of all current threads.
        Dim l As New List(Of Integer) ' list of thread that should be pruned
        For Each x As Integer In allThread
            If Array.IndexOf(currentThread, x) = -1 Then
                'This thread should be deleted.
                l.Add(x)
            End If
        Next
        For Each x As Integer In l
            DeletePost(x, DeleteFiles)
        Next
    End Sub

    Function GetThreadsCount(ByVal archive As Boolean) As Integer
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = ""
        If archive Then
            queryString = "Select Count(ID) as [Total Records] from board where (type=0) AND (mta=1)"
        Else
            queryString = "Select Count(ID) as [Total Records] from board where (type=0) AND (mta=0)"
        End If
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim i As Integer = 0
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        While reader.Read
            i = CInt(reader(0))
        End While
        cnx.Close()
        Return i
    End Function

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
    ''' <param name="data">Poster data</param>
    ''' <remarks></remarks>
    Private Sub ReplyTo(ByVal id As Integer, ByVal data As OPData)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "INSERT INTO board (type, [time], comment, postername, email, [password], parentT, subject, imagename, IP, ua, posterID, mta) VALUES      ('1', " & ConvertTimeToSQLTIME(data.time) & ", N'" & data.Comment & "', N'" & data.name & "', N'" & data.email & "', N'" & data.password & "', '" & id & "', N'" & data.subject & "', N'" & data.imageName & "' , '" & data.IP & "' , '" & data.UserAgent & "','" & GenerateUID(id, data.IP) & "', 0 )"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Private Sub BumpThread(ByVal id As Integer)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim updateQueryString As String = "UPDATE board SET bumplevel = " & ConvertTimeToSQLTIME(Date.UtcNow) & " WHERE(board.ID = " & id & ")"
        Dim q As New SqlCommand(updateQueryString, cnx)
        cnx.Open()
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
            ila.Add(CInt(reader(0)))
        End While
        reader.Close()
        cnx.Close()
        Return ila.ToArray
    End Function

    Function GetThreads(ByVal startIndex As Integer, ByVal count As Integer, ByVal ignoreStickies As Boolean, ByVal arhive As Boolean) As Integer()
        If Not arhive Then
            Dim ila As New List(Of Integer)
            Dim cnx As New SqlConnection(SQLConnectionString)
            cnx.Open()
            If Not ignoreStickies Then
                Dim queryString1 As String = "SELECT ID FROM board  WHERE (type = 0) AND (sticky = 1) AND (mta = 0) ORDER BY ID"
                Dim queryObject1 As New SqlCommand(queryString1, cnx)
                Dim reader1 As SqlDataReader = queryObject1.ExecuteReader
                While reader1.Read
                    Try
                        ila.Add(CInt(ConvertNoNull(reader1(0))))
                    Catch ex As Exception
                    End Try
                End While
                reader1.Close()
            End If
            Dim queryString As String = "SELECT ID FROM board  WHERE (type = 0) AND (sticky = 0) AND (mta = 0) ORDER BY bumplevel DESC"
            Dim queryObject As New SqlCommand(queryString, cnx)
            Dim reader As SqlDataReader = queryObject.ExecuteReader
            While reader.Read
                ila.Add(CInt(reader.Item(0)))
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
        Else
            'Get archived thread. Stickies are ignored in the archive
            Dim ila As New List(Of Integer)
            Dim cnx As New SqlConnection(SQLConnectionString)
            cnx.Open()
            Dim queryString As String = "SELECT ID FROM board  WHERE (type = 0) AND (mta = 1) ORDER BY bumplevel DESC"
            Dim queryObject As New SqlCommand(queryString, cnx)
            Dim reader As SqlDataReader = queryObject.ExecuteReader
            While reader.Read
                ila.Add(CInt(reader.Item(0)))
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
        End If
    End Function

    Function IsModLoginValid(ByVal name As String, ByVal password As String) As Boolean
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT password FROM mods WHERE (username LIKE '" & name & "')"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim sqlPassMd5 As String = ""
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        While reader.Read
            sqlPassMd5 = CStr(ConvertNoNull(reader(0)))
        End While
        reader.Close()
        cnx.Close()
        Return (MD5(password) = sqlPassMd5)
    End Function

    ''' <summary>
    ''' Save single file
    ''' </summary>
    ''' <param name="f"></param>
    ''' <param name="reply"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function saveFile(ByVal f As HttpPostedFile, ByVal reply As Boolean) As String
        Dim isImage As Boolean = FileIsImage(f)

        If isImage Then

            Try
                'Check if the file is a valid image
                Dim i As Drawing.Image = Drawing.Image.FromStream(f.InputStream)
                i.Dispose()

            Catch ex As Exception
                'No image is required when replying
                If f.ContentLength = 0 And reply = True Then
                    Return ""
                    Exit Function
                Else
                    Throw New ArgumentException(BadOrNoImage)
                End If
            End Try

            Dim w As Drawing.Image = Drawing.Image.FromStream(f.InputStream)
            Dim fileextension As String = f.FileName.Split(CChar(".")).ElementAt(f.FileName.Split(CChar(".")).Length - 1)

            Dim dd As String = CStr(Date.UtcNow.ToFileTime)
            'Full image path
            Dim p As String = STORAGEFOLDER & "\" & dd & "." & fileextension
            'Thumb path
            Dim thumb As String = ""
            If fileextension = "png" Then
                thumb = STORAGEFOLDER & "\th" & dd & ".png"
            Else
                thumb = STORAGEFOLDER & "\th" & dd & ".jpg"
            End If

            'Check if resize is needed.
            If (w.Width * w.Height) < 62500 Then
                If fileextension = "png" Then
                    w.Save(thumb, Drawing.Imaging.ImageFormat.Png)
                Else
                    w.Save(thumb, Drawing.Imaging.ImageFormat.Jpeg)
                End If
            Else
                If fileextension = "png" Then
                    ResizeImage(w, 250).Save(thumb, Drawing.Imaging.ImageFormat.Png)
                Else
                    ResizeImage(w, 250).Save(thumb, Drawing.Imaging.ImageFormat.Jpeg)
                End If
            End If
            'Save the file.
            f.SaveAs(p)

            'Calculate the file hash. I save the file then calulate the hash because f.InputStream always return a null stream. 
            Dim fs As New IO.FileStream(p, IO.FileMode.Open)
            Dim md5string As String = MD5(fs)
            fs.Close()

            Dim sp As String = ""

            If (Not AllowDuplicatesFiles) And ImageExist(md5string) Then
                'If image already exist, we fetch the matching image data from the database, and delete the saved files.
                If SmartLinkDuplicateImages = False Then
                    FileIO.FileSystem.DeleteFile(p)
                    FileIO.FileSystem.DeleteFile(thumb)
                    Throw New ArgumentException(duplicateFile)
                Else
                    Dim wpi As WPostImage = GetImageDataByMD5(md5string)
                    'Delete previously saved files.
                    FileIO.FileSystem.DeleteFile(p)
                    FileIO.FileSystem.DeleteFile(thumb)
                    sp = wpi.chanbName & ":" & CStr(wpi.size) & ":" & wpi.dimensions & ":" & wpi.realname & ":" & wpi.md5
                End If
            Else
                'chanb name : size in bytes : dimensions : realname : md5
                sp = dd & "." & fileextension & ":" & f.ContentLength & ":" & w.Size.Width & "x" & w.Size.Height & ":" & f.FileName & ":" & md5string
                w.Dispose()
            End If
            Return sp

        Else 'Maybe a PDF or SVG file or no file
            Dim fileextension As String = f.FileName.Split(CChar(".")).ElementAt(f.FileName.Split(CChar(".")).Length - 1)

            Select Case fileextension.ToUpper()
                Case "SVG"
                    Dim dd As String = CStr(Date.UtcNow.ToFileTime)
                    Dim p As String = STORAGEFOLDER & "\" & dd & "." & fileextension
                    'Thumb path
                    Dim thumb As String = STORAGEFOLDER & "\th" & dd & ".png"
                    f.SaveAs(p)

                    Dim fs As New IO.FileStream(p, IO.FileMode.Open)
                    Dim md5string As String = MD5(fs)
                    fs.Close()

                    Dim sp As String = ""

                    If (Not AllowDuplicatesFiles) And ImageExist(md5string) Then

                        If SmartLinkDuplicateImages = False Then
                            FileIO.FileSystem.DeleteFile(p)
                            FileIO.FileSystem.DeleteFile(thumb)
                            Throw New ArgumentException(duplicateFile)
                        Else
                            Dim wpi As WPostImage = GetImageDataByMD5(md5string)
                            FileIO.FileSystem.DeleteFile(p)
                            sp = wpi.chanbName & ":" & CStr(wpi.size) & ":" & wpi.dimensions & ":" & wpi.realname & ":" & wpi.md5
                        End If
                    Else

                        Dim svgDoc As Svg.SvgDocument = Svg.SvgDocument.Open(p)
                        Dim svgBi As Drawing.Bitmap = svgDoc.Draw()

                        If (svgBi.Width * svgBi.Height) < 62500 Then
                            svgBi.Save(thumb)
                        Else
                            ResizeImage(svgBi, 250).Save(thumb)
                        End If
                        sp = dd & "." & fileextension & ":" & f.ContentLength & ":" & svgBi.Size.Width & "x" & svgBi.Size.Height & ":" & f.FileName & ":" & md5string
                        svgBi.Dispose()
                    End If

                    Return sp
                Case "PDF"
#If EnablePDF Then
 Dim dd As String = CStr(Date.UtcNow.ToFileTime)
                    Dim p As String = STORAGEFOLDER & "\" & dd & "." & fileextension
                    'Thumb path
                    Dim thumb As String = STORAGEFOLDER & "\th" & dd & ".jpg"
                    f.SaveAs(p)

                    Dim fs As New IO.FileStream(p, IO.FileMode.Open)
                    Dim md5string As String = MD5(fs)
                    fs.Close()

                    Dim sp As String = ""

                    If (Not AllowDuplicatesFiles) And ImageExist(md5string) Then
                        If SmartLinkDuplicateImages = False Then
                            FileIO.FileSystem.DeleteFile(p)
                            FileIO.FileSystem.DeleteFile(thumb)
                            Throw New ArgumentException(duplicateFile)
                        Else
                            Dim wpi As WPostImage = GetImageDataByMD5(md5string)
                            FileIO.FileSystem.DeleteFile(p)
                            sp = wpi.chanbName & ":" & CStr(wpi.size) & ":" & wpi.dimensions & ":" & wpi.realname & ":" & wpi.md5
                        End If
                    Else

                        Dim fileS As New IO.FileStream(p, IO.FileMode.Open)

                        Dim pd As New TallComponents.PDF.Rasterizer.Document(fileS)
                        Dim page As TallComponents.PDF.Rasterizer.Page = pd.Pages(0)

                        Dim scale As Double = 150 / 72

                        Dim pdfBi As Drawing.Bitmap = New Drawing.Bitmap(CInt(scale * page.Width), CInt(scale * page.Height))

                        Dim graph As Drawing.Graphics = Drawing.Graphics.FromImage(pdfBi)
                        graph.SmoothingMode = Drawing.Drawing2D.SmoothingMode.AntiAlias
                        graph.ScaleTransform(CSng(scale), CSng(scale))
                        graph.Clear(Drawing.Color.White)
                        page.Draw(graph)

                        graph.Dispose()
                        fileS.Close()

                        If (pdfBi.Width * pdfBi.Height) < 62500 Then
                            pdfBi.Save(thumb, Drawing.Imaging.ImageFormat.Jpeg)
                        Else
                            ResizeImage(pdfBi, 250).Save(thumb, Drawing.Imaging.ImageFormat.Jpeg)
                        End If
                        sp = dd & "." & fileextension & ":" & f.ContentLength & ":" & pdfBi.Size.Width & "x" & pdfBi.Size.Height & ":" & f.FileName & ":" & md5string
                        pdfBi.Dispose()

                    End If
                    Return sp
#Else
                    Dim dd As String = CStr(Date.UtcNow.ToFileTime)
                    Dim p As String = STORAGEFOLDER & "\" & dd & "." & fileextension
                    'Thumb path
                    Dim thumb As String = STORAGEFOLDER & "\th" & dd & ".jpg"
                    f.SaveAs(p)

                    Dim fs As New IO.FileStream(p, IO.FileMode.Open)
                    Dim md5string As String = MD5(fs)
                    fs.Close()

                    Dim sp As String = ""

                    If (Not AllowDuplicatesFiles) And ImageExist(md5string) Then
                        If SmartLinkDuplicateImages = False Then
                            FileIO.FileSystem.DeleteFile(p)
                            FileIO.FileSystem.DeleteFile(thumb)
                            Throw New ArgumentException(duplicateFile)
                        Else
                            Dim wpi As WPostImage = GetImageDataByMD5(md5string)
                            FileIO.FileSystem.DeleteFile(p)
                            sp = wpi.ChanbName & ":" & CStr(wpi.Size) & ":" & wpi.Dimensions & ":" & wpi.RealName & ":" & wpi.MD5
                        End If
                    Else

                        Dim pdfBi As Drawing.Bitmap = New Drawing.Bitmap(150, 30)

                        Dim graph As Drawing.Graphics = Drawing.Graphics.FromImage(pdfBi)
                        graph.FillRegion(Drawing.Brushes.White, New Drawing.Region(New Drawing.RectangleF(0, 0, 150, 30)))
                        graph.DrawString("PDF File", New Drawing.Font(Drawing.FontFamily.GenericSansSerif, 20, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Pixel), Drawing.Brushes.Red, 0, 0)

                        graph.Dispose()


                        If (pdfBi.Width * pdfBi.Height) < 62500 Then
                            pdfBi.Save(thumb, Drawing.Imaging.ImageFormat.Jpeg)
                        Else
                            ResizeImage(pdfBi, 250).Save(thumb, Drawing.Imaging.ImageFormat.Jpeg)
                        End If
                        sp = dd & "." & fileextension & ":" & f.ContentLength & ":" & pdfBi.Size.Width & "x" & pdfBi.Size.Height & ":" & f.FileName & ":" & md5string
                        pdfBi.Dispose()

                    End If
                    Return sp
#End If
                Case ""
                    Return ""
                Case Else
                    Throw New ArgumentException("Unsupported file type")
            End Select
        End If
    End Function

    Private Function FileIsImage(ByVal f As HttpPostedFile) As Boolean
        Dim extension As String = f.FileName.Split(CChar(".")).ElementAt(f.FileName.Split(CChar(".")).Length - 1)
        Dim supportedImages As String() = {"jpg", "jpeg", "png", "bmp", "gif"}
        Dim bo As Boolean = False
        For Each x In supportedImages
            If extension = x Then
                bo = True
            End If
        Next
        Return bo
    End Function

    Private Function GetImageDataByMD5(ByVal md5 As String) As WPostImage
        Dim wpi As New WPostImage
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT TOP 1 imagename FROM board WHERE (imagename LIKE '%" & md5 & "%')"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim imageNameStr As String = ""
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        While reader.Read
            imageNameStr = CStr(ConvertNoNull(reader(0)))
        End While
        If imageNameStr = "" Then
            Throw New ArgumentException("No image exist with the specified MD5.")
        Else
            Dim array As String() = imageNameStr.Split(CChar(";"))
            Dim selectedX As String = ""
            For Each x In array
                Dim p As WPostImage = GetWPOSTIMAGE(x)
                If p.md5 = md5 Then
                    wpi = p
                End If
            Next
        End If
        Return wpi
    End Function

    Private Function ImageExist(ByVal md5 As String, Optional ByVal excludedPost As Integer = -1) As Boolean
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = ""
        If excludedPost = -1 Then
            queryString = "SELECT TOP 1 ID FROM board WHERE (imagename LIKE '%" & md5 & "%')"
        Else
            queryString = "SELECT TOP 1 ID FROM board WHERE (ID <> " & excludedPost & ") AND (imagename LIKE '%" & md5 & "%')"
        End If
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        Dim b As Boolean = False
        While reader.Read
            If TypeOf reader(0) Is DBNull Then
                b = False
            Else
                b = True
            End If
        End While
        cnx.Close()
        Return b
    End Function

    Private Function GetWPostImage(ByVal sp As String) As WPostImage
        Dim wp As New WPostImage
        wp.chanbName = sp.Split(CChar(":")).ElementAt(0)
        wp.size = CLng(sp.Split(CChar(":")).ElementAt(1))
        wp.dimensions = sp.Split(CChar(":")).ElementAt(2)
        wp.realname = sp.Split(CChar(":")).ElementAt(3)
        wp.md5 = sp.Split(CChar(":")).ElementAt(4)
        wp.Extension = wp.RealName.Split(CChar(".")).ElementAt(wp.RealName.Split(CChar(".")).Length - 1).ToUpper
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

            If (f.ContentLength > 0) Then
                list.Add(saveFile(f, True))
                list.Add(";")

            Else
                'Maybe an empty file field.
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
        Dim ratioP As Double = isi.Width / targetMax
        Return New Drawing.Size(CInt(Fix(isi.Width / ratioP)), CInt(Fix(isi.Height / ratioP)))
    End Function

    Private Function ResizeImage(ByVal i As Drawing.Image, ByVal targetS As Integer) As Drawing.Image
        Select Case ResizeMethode
            Case 0 'naive quality
                Return i.GetThumbnailImage(DownSizeWithAspectRatio(targetS, i.Size).Width, DownSizeWithAspectRatio(targetS, i.Size).Height, Nothing, System.IntPtr.Zero)
            Case 1 ' Medium Quality 
                Dim sizef As Drawing.Size = DownSizeWithAspectRatio(targetS, i.Size)
                Dim bi As New Drawing.Bitmap(sizef.Width, sizef.Height)
                Dim g As Drawing.Graphics = Drawing.Graphics.FromImage(bi)
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.Bicubic
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.PixelOffsetMode = Drawing.Drawing2D.PixelOffsetMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality

                Dim imageRectangle As Drawing.Rectangle = New Drawing.Rectangle(0, 0, sizef.Width, sizef.Height)
                g.DrawImage(i, imageRectangle)
                Return bi
                bi.Dispose()
                g.Dispose()

            Case 2 'Fastest Methode
                Dim sizef As Drawing.Size = DownSizeWithAspectRatio(targetS, i.Size)
                Dim bi As New Drawing.Bitmap(sizef.Width, sizef.Height)
                Dim g As Drawing.Graphics = Drawing.Graphics.FromImage(bi)
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.Default
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighSpeed
                g.PixelOffsetMode = Drawing.Drawing2D.PixelOffsetMode.HighSpeed
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighSpeed
                Dim imageRectangle As Drawing.Rectangle = New Drawing.Rectangle(0, 0, sizef.Width, sizef.Height)
                g.DrawImage(i, imageRectangle)
                Return bi
                bi.Dispose()
                g.Dispose()
            Case Else
                Return i.GetThumbnailImage(DownSizeWithAspectRatio(targetS, i.Size).Width, DownSizeWithAspectRatio(targetS, i.Size).Height, Nothing, System.IntPtr.Zero)
        End Select
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
        Dim jpgThumb As String = STORAGEFOLDER & "\th" & name.Split(CChar(".")).ElementAt(0) & ".jpg"
        If FileIO.FileSystem.FileExists(jpgThumb) Then
            Return StoragefolderWEB & "th" & name.Split(CChar(".")).ElementAt(0) & ".jpg"
        Else
            'Must be a png thumbnail
            Return StoragefolderWEB & "th" & name.Split(CChar(".")).ElementAt(0) & ".png"
        End If
    End Function

    Sub NewMod(ByVal name As String, ByVal pas As String, Optional ByVal powers As String = "")
        If powers = "" Then
            powers = DefaultModPowers
        End If
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "INSERT INTO mods (username, password, powers) VALUES ('" & name & "', '" & MD5(pas) & "', '" & powers & "')"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Function IsIPBanned(ByVal IP As String) As Boolean
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT ID FROM bans WHERE (IP LIKE '" & IP & "')"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        Dim banned As Boolean = False
        While reader.Read
            banned = Not (TypeOf reader(0) Is DBNull)
            'If TypeOf reader(0) Is DBNull Then
            '    banned = False
            '    'User is not banned since there is no ID entry for the specified IP address
            'Else
            '    banned = True
            'End If
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
            data.ID = CInt(ConvertNoNull(reader(0)))
            data.PERM = CBool(ConvertNoNull(reader(1)))
            data.EXPIRY = CDate(ConvertNoNull(reader(2)))
            data.COMMENT = CStr(ConvertNoNull(reader(3)))
            data.POSTNO = CInt(ConvertNoNull(reader(4)))
        End While
        data.IP = IP
        reader.Close()
        cnx.Close()
        Return data
    End Function

    Private Function MakeBannedMessage(ByVal IP As String) As String
        Return BannedMessage
    End Function

    Function ProcessPost(ByVal request As HttpRequest, ByVal Session As Web.SessionState.HttpSessionState) As String
        Dim sb As New StringBuilder
        sb.Append("<html><head>")
        Dim cont As Boolean = True
        If Session.Item("lastpost") Is "" Or Session.Item("lastpost") Is Nothing Then
            Session.Item("lastpost") = Now.ToString
        Else
            Dim i As Date = Date.Parse(CStr(Session.Item("lastpost")))
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
                        Else
                            'Save file.
                            If request.Files("ufile").ContentLength = 0 Then
                                sb.Append(BadOrNoImage)
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
                                'If er.Comment.Length > 4000 Then
                                '    er.Comment = er.Comment.Remove(3999)
                                'End If
                                er.email = ProcessInputs(request.Item("email"))
                                If request.Item("postername") = "" Then er.name = AnonName Else er.name = ProcessInputs(request.Item("postername"))
                                er.subject = ProcessInputs(request.Item("subject"))
                                er.time = Date.UtcNow
                                er.imageName = s
                                er.password = ProcessInputs(request.Item("password"))
                                er.IP = request.UserHostAddress
                                er.UserAgent = request.UserAgent.Replace("<", "").Replace(">", "") ' I replace < and > to prevent spoffing a user agent that contain <script> tags.
                                If request.Cookies("pass") IsNot Nothing Then request.Cookies("pass").Value = request.Item("password") Else request.Cookies.Add(New HttpCookie("pass", request.Item("password")))
                                MakeThread(er)
                                sb.Append(SuccessfulPostString)
                                sb.Append("<meta HTTP-EQUIV='REFRESH' content='2; url=default.aspx'>")
                            End If
                        End If

                    Case "reply"

                        If IsLocked(CInt(request.Item("threadid"))) Then
                            sb.Append(lockedMessage)
                            sb.Append("<meta HTTP-EQUIV='REFRESH' content='5; url=default.aspx?id=" & request.Item("threadid") & "'>")
                            Exit Select
                        End If

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

                        Dim totalFiles As Integer = s.Split(CChar(";")).Count

                        If request.Item("finp") = "yes" And totalFiles > 1 Then ' Add each file to a seperate post, and dump the files.
                            Dim pos As Integer = 1
                            If request.Cookies("pass") IsNot Nothing Then request.Cookies("pass").Value = request.Item("password") Else request.Cookies.Add(New HttpCookie("pass", request.Item("password")))
                            Dim countFiles As Boolean = (request.Item("countf") = "yes")
                            Dim advanced As Boolean = False
                            For Each singleImage As String In s.Split(CChar(";"))
                                Dim er As New OPData
                                If Not advanced Then
                                    er.Comment = ProcessInputs(request.Item("comment"))
                                    'If er.Comment.Length > 4000 Then
                                    '    If countFiles Then er.Comment.Remove(3999 - CStr(vbNewLine & pos & "&#47;" & totalFiles).Length) Else er.Comment.Remove(3999)
                                    'End If
                                    If countFiles Then er.Comment = er.Comment & CStr(vbNewLine & pos & "&#47;" & totalFiles)
                                    advanced = True
                                Else
                                    If countFiles Then er.Comment = pos & "&#47;" & totalFiles
                                End If
                                er.email = ProcessInputs(request.Item("email"))
                                If request.Item("postername") = "" Then er.name = AnonName Else er.name = ProcessInputs(request.Item("postername"))
                                er.subject = ProcessInputs(request.Item("subject"))
                                er.time = Date.UtcNow
                                er.imageName = singleImage
                                er.password = ProcessInputs(request.Item("password"))
                                er.IP = request.UserHostAddress
                                er.UserAgent = request.UserAgent.Replace("<", "").Replace(">", "")
                                ReplyTo(CInt(request.Item("threadid")), er)
                                pos += 1
                            Next
                            sb.Append(SuccessfulPostString)
                        Else
                            'Single file, or multiple files post.
                            Dim er As New OPData
                            If ProcessInputs(request.Item("comment")) = "" And s = "" Then
                                'no image and no text
                                'blank post
                                sb.Append(noBlankpost)
                            Else
                                er.Comment = ProcessInputs(request.Item("comment"))
                                'If er.Comment.Length > 4000 Then
                                '    er.Comment = er.Comment.Remove(3999)
                                'End If
                                er.email = ProcessInputs(request.Item("email"))
                                If request.Item("postername") = "" Then er.name = AnonName Else er.name = ProcessInputs(request.Item("postername"))
                                er.subject = ProcessInputs(request.Item("subject"))
                                er.time = Date.UtcNow
                                er.imageName = s
                                er.password = ProcessInputs(request.Item("password"))
                                er.IP = request.UserHostAddress
                                er.UserAgent = request.UserAgent.Replace("<", "").Replace(">", "") ' I replace < and > to prevent spoffing a user agent that contain <script> tags.
                                If request.Cookies("pass") IsNot Nothing Then request.Cookies("pass").Value = request.Item("password") Else request.Cookies.Add(New HttpCookie("pass", request.Item("password")))
                                ReplyTo(CInt(request.Item("threadid")), er)
                                sb.Append(SuccessfulPostString)
                            End If
                        End If

                        If Not ProcessInputs(request.Item("email")) = "sage" And (GetRepliesCount(CInt(request.Item("threadid"))).TotalReplies <= BumpLimit) Then BumpThread(CInt(request.Item("threadid")))

                        sb.Append("<meta HTTP-EQUIV='REFRESH' content='1; url=default.aspx?id=" & request.Item("threadid") & "'>")

                    Case reportStr
                        Dim il As New List(Of Integer)
                        For Each x As String In request.QueryString
                            If x.StartsWith("proc") Then
                                il.Add(CInt(x.Replace("proc", "")))
                            End If
                        Next
                        If il.Count = 0 Then
                            sb.Append(NoPostWasSelected)
                        Else
                            For Each x In il
                                ReportPost(x, request.UserHostAddress, Date.UtcNow)
                                sb.Append(ReportedSucess.Replace("%", CStr(x)))
                            Next
                        End If

                    Case deleteStr
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
                                Dim p As WPost = FetchPostData(CLng(x))
                                If p.password = deletPass Then
                                    DeletePost(CInt(x), DeleteFiles)
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
        Dim md5s As New System.Security.Cryptography.MD5CryptoServiceProvider
        Return ByteArrayToString(md5s.ComputeHash(s))
    End Function

    Private Function MD5(ByVal s As String) As String
        Dim md5s As New System.Security.Cryptography.MD5CryptoServiceProvider
        Dim bytes() As Byte = System.Text.Encoding.ASCII.GetBytes(s)
        Return ByteArrayToString(md5s.ComputeHash(bytes))
    End Function

    Private Function ByteArrayToString(ByVal arrInput() As Byte) As String
        Dim sb As New StringBuilder
        For Each x As Byte In arrInput
            sb.Append(x.ToString("X"))
        Next
        Return sb.ToString().ToLower
    End Function

    Private Function ProcessComment(ByVal comment As String, ByVal parentPost As Integer) As String
        Dim sb As New StringBuilder
        Dim li As String() = comment.Split(CChar(vbNewLine))
        For Each x In li
            If Not (x = "") Then
                'Check if greentext
                If x.StartsWith("&gt;") And Not x.StartsWith("&gt;&gt;") Then
                    sb.Append("<span class='quote'>" & x & "</span>")
                    'Some times, X start with a line terminator that is not vbnewline, so i remove it

                ElseIf (x.Remove(0, 1).StartsWith("&gt;") And Not x.Remove(0, 1).StartsWith("&gt;&gt;")) Then
                    sb.Append("<span class='quote'>" & x.Remove(0, 1) & "</span>")

                ElseIf IsXvalidQuote(x) Then
                    sb.Append("<a href='default.aspx?id=" & parentPost & "#p" & x.Replace("&gt;&gt;", "") & "'>" & x & "</a>")

                    'Some times, X start with a line terminator that is not vbnewline, so i remove it
                ElseIf IsXvalidQuote(x.Remove(0, 1)) Then
                    sb.Append("<a href='default.aspx?id=" & parentPost & "#p" & x.Remove(0, 1).Replace("&gt;&gt;", "") & "'>" & x.Remove(0, 1) & "</a>")

                Else
                    sb.Append(x)
                End If
                sb.Append("<br>")
            End If
        Next

        Dim sr As String = sb.ToString
        sr = MatchBBCode("spoiler", sr)
        sr = MatchBBCode("code", sr)
        sr = MatchBBCode("md", sr)
        Return sr
    End Function

#Region "BB codes processing"

    ''' <summary>
    ''' Return the bbcode and the data inside.
    ''' </summary>
    ''' <param name="codename"></param>
    ''' <param name="data"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function MatchBBCode(ByVal codename As String, ByVal data As String) As String
        Select Case codename
            Case "spoiler"
                Dim regSTR As String = "\[/?spoiler\]"
                Dim mdata As String = data.Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#47;", "/")
                Dim st As String() = Regex.Split(mdata, regSTR)
                Dim ismath As Boolean = False
                For i As Integer = 0 To st.Length - 1 Step 1
                    If Not ismath Then
                        'Not a match 
                    Else
                        mdata = mdata.Replace(st.ElementAt(i), "<s>" & st.ElementAt(i) & "</s>")
                    End If
                    ismath = Not ismath
                Next
                mdata = mdata.Replace("[spoiler]", "")
                mdata = mdata.Replace("[/spoiler]", "")
                Return mdata
            Case "code"
                Try
                    Dim regSTR As String = "\[/?code\]"
                    Dim mdata As String = data.Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#47;", "/") ' replace [ ] / html equivalent with the real character to make sure the regex will catch them
                    Dim st As String() = Regex.Split(mdata, regSTR)
                    Dim ismath As Boolean = False
                    For i As Integer = 0 To st.Length - 1 Step 1
                        If Not ismath Then
                            'Not a match 
                        Else
                            Dim codeStr As String = st.ElementAt(i)
                            codeStr = codeStr.Replace("<br>", vbNewLine)

                            Dim codeLang As String = GetCodeLang(codeStr)
                            codeStr = codeStr.Replace("[lang]", "")
                            codeStr = codeStr.Replace("[/lang]", "")
                            If Not (codeLang = "") Then codeStr = codeStr.Replace(codeLang, "")
                            codeStr = RemoveHTMLEscapes(codeStr)

                            Dim colorizer As New ColorCode.CodeColorizer()

                            mdata = mdata.Replace(st.ElementAt(i), colorizer.Colorize(codeStr, GetCCLI(codeLang)))
                        End If
                        ismath = Not ismath
                    Next
                    mdata = mdata.Replace("[code]", "")
                    mdata = mdata.Replace("[/code]", "")
                    Return mdata
                Catch ex As Exception
                    Return data
                End Try
            Case "md"
                Dim regSTR As String = "\[/?md\]"
                Dim mdata As String = data.Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#47;", "/")
                Dim st As String() = Regex.Split(mdata, regSTR)
                Dim ismath As Boolean = False
                For i As Integer = 0 To st.Length - 1 Step 1
                    If Not ismath Then
                        'Not a match 
                    Else
                        Dim md As New MarkdownSharp.Markdown
                        Dim mdtext As String = st.ElementAt(i)
                        mdtext = RemoveHTMLEscapes(mdtext)
                        mdtext = mdtext.Replace("<br>", "")
                        mdata = mdata.Replace(st.ElementAt(i), md.Transform(mdtext))
                    End If
                    ismath = Not ismath
                Next
                mdata = mdata.Replace("[md]", "")
                mdata = mdata.Replace("[/md]", "")
                Return mdata
            Case Else
                Return data
        End Select
    End Function

    Private Function RemoveHTMLEscapes(ByVal c As String) As String
        Dim lowcaseX As String = c
        lowcaseX = lowcaseX.Replace("&amp;", "&")
        lowcaseX = lowcaseX.Replace("&lt;", "<")
        lowcaseX = lowcaseX.Replace("&gt;", ">")
        lowcaseX = lowcaseX.Replace("&ndash;", "-")
        lowcaseX = lowcaseX.Replace("&mdash;", "—")
        lowcaseX = lowcaseX.Replace("&#37;", "%")
        lowcaseX = lowcaseX.Replace("&#36;", "$")
        lowcaseX = lowcaseX.Replace("&#39;", "'")
        lowcaseX = lowcaseX.Replace("&#40;", "(")
        lowcaseX = lowcaseX.Replace("&#41;", ")")
        lowcaseX = lowcaseX.Replace("&#42;", "*")
        lowcaseX = lowcaseX.Replace("&#43;", "+")
        lowcaseX = lowcaseX.Replace("&#44;", ",")
        lowcaseX = lowcaseX.Replace("&#47;", "/")
        lowcaseX = lowcaseX.Replace("&#58;", ":")
        lowcaseX = lowcaseX.Replace("&#61;", "=")
        lowcaseX = lowcaseX.Replace("&#64;", "@")
        lowcaseX = lowcaseX.Replace("&#91;", "[")
        lowcaseX = lowcaseX.Replace("&#93;", "]")
        lowcaseX = lowcaseX.Replace("&#92;", "\")
        lowcaseX = lowcaseX.Replace("&#94;", "^")
        lowcaseX = lowcaseX.Replace("&#96;", "`")
        lowcaseX = lowcaseX.Replace("&#95;", "_")
        lowcaseX = lowcaseX.Replace("&#123;", "{")
        lowcaseX = lowcaseX.Replace("&#124;", "|")
        lowcaseX = lowcaseX.Replace("&#125;", "}")
        lowcaseX = lowcaseX.Replace("&#126;", "~")
        lowcaseX = lowcaseX.Replace("&quot;", My.Resources.doublequotes)
        Return lowcaseX
    End Function

    Private Function GetCodeLang(ByVal x As String) As String
        Dim st As String() = Regex.Split(x, "\[/?lang\]")
        Dim ismath As Boolean = False
        Dim codel As String = ""
        For i As Integer = 0 To st.Length - 1 Step 1
            If Not ismath Then
                'Not a match 
            Else
                codel = st.ElementAt(i)
            End If
            ismath = Not ismath
        Next
        Return codel
    End Function

    Private Function GetCCLI(ByVal lang As String) As ColorCode.ILanguage
        Select Case lang
            Case "asax"
                Return ColorCode.Languages.Asax
            Case "ashx"
                Return ColorCode.Languages.Ashx
            Case "aspx"
                Return ColorCode.Languages.Aspx
            Case "aspxcs"
                Return ColorCode.Languages.AspxCs
            Case "aspxvb"
                Return ColorCode.Languages.AspxVb
            Case "cpp"
                Return ColorCode.Languages.Cpp
            Case "csharp"
                Return ColorCode.Languages.CSharp
            Case "css"
                Return ColorCode.Languages.Css
            Case "fsharp"
                Return ColorCode.Languages.FSharp
            Case "html"
                Return ColorCode.Languages.Html
            Case "java"
                Return ColorCode.Languages.Java
            Case "javascript"
                Return ColorCode.Languages.JavaScript
            Case "js"
                Return ColorCode.Languages.JavaScript
            Case "php"
                Return ColorCode.Languages.Php
            Case "ps"
                Return ColorCode.Languages.PowerShell
            Case "sql"
                Return ColorCode.Languages.Sql
            Case "typescript"
                Return ColorCode.Languages.Typescript
            Case "vbdotnet"
                Return ColorCode.Languages.VbDotNet
            Case "vb"
                Return ColorCode.Languages.VbDotNet
            Case "xml"
                Return ColorCode.Languages.Xml
            Case "c"
                Return ColorCode.Languages.Cpp
            Case Else
                Return ColorCode.Languages.Cpp
        End Select
    End Function

#End Region

    Private Function IsXvalidQuote(ByVal x As String) As Boolean
        Dim b As Boolean = False
        ' A valid quote should be in >>Int32 format.
        Try
            If x.StartsWith("&gt;&gt;") Then
                Dim i = CInt(x.Replace("&gt;&gt;", ""))
                b = True
            Else
                b = False
            End If
        Catch ex As Exception
            b = False
        End Try
        Return b
    End Function

    Function GetOPPostHTML(ByVal id As Integer, ByVal parameters As HTMLParameters) As String
        Dim po As WPost = FetchPostData(id)
        Dim imageData As WPostImage = GetWPostImage(po._imageP)
        Dim postHTML As String = opPostTemplate
        If parameters.replyButton Then
            postHTML = postHTML.Replace("%REPLY BUTTON%", replyButtonHTML)
        Else
            postHTML = postHTML.Replace("%REPLY BUTTON%", "")
        End If
        If EnableUserID Then
            Dim idHt As String = idHtml
            idHt = idHt.Replace("%UID%", po.posterID)
            postHTML = postHTML.Replace("%UID%", idHt)
        Else
            postHTML = postHTML.Replace("%UID%", "")
        End If
        If po.email = "" Then
            postHTML = postHTML.Replace("%NAMESPAN%", "<span class='name'>%NAME%</span>")
        Else
            postHTML = postHTML.Replace("%NAMESPAN%", "<a href='mailto:%EMAIL%' class='useremail'><span class='name'>%NAME%</span></a>")
        End If
        If po.isSticky Then
            postHTML = postHTML.Replace("%STICKY%", "<img alt='" & stickyStr & "' title='" & stickyStr & "' src='res/sticky.png' />")
        Else
            postHTML = postHTML.Replace("%STICKY%", "")
        End If
        If po.locked Then
            postHTML = postHTML.Replace("%LOCKED%", "<img alt='" & lockedStr & "' title='" & lockedStr & "' src='res/locked.png' />")
        Else
            postHTML = postHTML.Replace("%LOCKED%", "")
        End If
        postHTML = postHTML.Replace("%ID%", CStr(po.PostID))
        postHTML = postHTML.Replace("%IMAGE LINK%", GetImageWEBPATH(imageData.ChanbName))
        postHTML = postHTML.Replace("%CHANB FILE NAME%", imageData.ChanbName)
        postHTML = postHTML.Replace("%FILE NAME%", imageData.RealName)
        postHTML = postHTML.Replace("%FILE SIZE%", FormatSizeString(imageData.Size))
        postHTML = postHTML.Replace("%IMAGE DIMENSIONS%", imageData.Dimensions)
        postHTML = postHTML.Replace("%THUMB LINK%", GetImageWEBPATHRE(imageData.ChanbName))
        postHTML = postHTML.Replace("%MD5%", imageData.MD5)
        postHTML = postHTML.Replace("%EMAIL%", po.email)
        postHTML = postHTML.Replace("%SUBJECT%", po.subject)
        postHTML = postHTML.Replace("%NAME%", po.name)
        postHTML = postHTML.Replace("%DATE UTC UNIX%", CStr(po.time.ToFileTime))
        postHTML = postHTML.Replace("%DATE UTC TEXT%", GetTimeString(po.time))
        postHTML = postHTML.Replace("%POST LINK%", "default.aspx?id=" & po.PostID & "#p" & po.PostID)
        ''Post text
        If parameters.isTrailPost Then
            Dim cm As String = ProcessComment(po.comment, CInt(po.PostID))
            If cm.Length > 1500 Then
                cm = cm.Remove(1500)
                cm = cm & commentToolong.Replace("%POSTLINK%", "default.aspx?id=" & CStr(po.PostID))
                postHTML = postHTML.Replace("%POST TEXT%", cm)
            Else
                postHTML = postHTML.Replace("%POST TEXT%", cm)
            End If ''
        Else
            postHTML = postHTML.Replace("%POST TEXT%", ProcessComment(po.comment, CInt(po.PostID)))
        End If
        postHTML = postHTML.Replace("%REPLY COUNT%", CStr(GetRepliesCount(id).TotalReplies))
        postHTML = postHTML.Replace("%IMAGE EXT%", imageData.Extension)
        If parameters.IsModerator Then postHTML = postHTML.Replace("%MODPANEL%", parameters.modMenu.Replace("%ID%", CStr(po.PostID))) Else postHTML = postHTML.Replace("%MODPANEL%", "")
        Return postHTML
    End Function

    'Private Function GenerateUID(ByVal po As WPost) As String
    '    If po.type = "0" Then
    '        Dim idstr As String = po.PostID & po.ip
    '        Return New String(CType(MD5(idstr), Char()), 0, 8)
    '    Else
    '        Dim idstr As String = po.parent & po.ip
    '        Return New String(CType(MD5(idstr), Char()), 0, 8)
    '    End If
    'End Function

    Function GetModPowers(ByVal modname As String) As String
        Dim cn As New SqlConnection(SQLConnectionString)
        cn.Open()
        Dim q As New SqlCommand("SELECT power FROM mods WHERE (username LIKE '" & modname & "')", cn)
        Dim powstr As String = ""
        Dim reader As SqlDataReader = q.ExecuteReader
        While reader.Read
            powstr = CStr(ConvertNoNull(reader(0)))
        End While
        reader.Close()
        cn.Close()
        Return powstr.Replace("'", "")
    End Function

    Function GetModeratorHTMLMenu(ByVal id As String, ByVal powers As String) As String
        Dim power As String() = powers.Split(CChar("-"))
        Dim sb As New StringBuilder
        For i As Integer = 0 To power.Length - 1 Step 1
            If power(i) = "1" Then
                sb.Append(modMenu(i).Replace("%ID%", CStr(id)))
                sb.Append("&nbsp;")
            End If
        Next
        Return sb.ToString
    End Function

    Private Function GenerateUID(ByVal parentPost As Integer, ByVal IP As String) As String
        Return New String(CType(MD5(CStr(parentPost) & IP), Char()), 0, 8)
    End Function

    Private Function GetLastXPosts(ByVal threadID As Integer, ByVal x As Integer) As Integer()
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim query As New SqlCommand("SELECT TOP " & x & " ID FROM board WHERE(parentT = " & threadID & ") ORDER BY ID DESC", cnx)
        cnx.Open()
        Dim il As New List(Of Integer)
        Dim reader As SqlDataReader = query.ExecuteReader
        While reader.Read
            il.Add(CInt(reader(0)))
        End While
        reader.Close()
        cnx.Close()
        Return il.ToArray
    End Function

    Sub BanPosterByPost(ByVal postID As Integer)
        Dim po As WPost = FetchPostData(postID)
        If IsIPBanned(po.ip) = False Then
            Dim newText As String = po.comment & bannedMessageHTML
            BanPoster(po.ip, postID)
            UpdatePostText(postID, newText, True)
        End If
    End Sub

    Private Sub UpdatePostText(ByVal postID As Integer, ByVal newText As String, ByVal allowHTML As Boolean)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = ""
        If newText.Length > 4000 Then
            newText = newText.Remove(3999)
        End If
        If allowHTML Then
            queryString = "UPDATE board SET comment = N'" & newText & "' WHERE(ID = " & postID & ")"
        Else
            queryString = "UPDATE board SET comment = N'" & ProcessInputs(newText) & "' WHERE(ID = " & postID & ")"
        End If
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

    Function GetThreadHTML(ByVal threadID As Integer, ByVal para As HTMLParameters, ByVal trailposts As Integer) As String
        Dim postHtml As String = threadTemplate
        postHtml = postHtml.Replace("%ID%", CStr(threadID))
        Dim trailswithimages As Integer = 0
        If trailposts > 0 Then
            Dim sb As New StringBuilder
            For Each x In GetLastXPosts(threadID, trailposts).Reverse
                Dim t As String = GetSingleReplyHTML(x, para)
                sb.Append(t)
                If t.Contains("data-md5") Then
                    trailswithimages += 1 ' TODO : Implement a different method for this.
                End If
            Next
            postHtml = postHtml.Replace("%TRAILS%", sb.ToString)
        Else
            postHtml = postHtml.Replace("%TRAILS%", "")
        End If
        postHtml = postHtml.Replace("%POST HTML%", GetOPPostHTML(threadID, para))

        Dim repC As ThreadReplies = GetRepliesCount(threadID)
        If repC.TotalReplies - trailposts <= 0 Then
            postHtml = postHtml.Replace("%AN%", "hide")
        Else
            postHtml = postHtml.Replace("%AN%", "")

            Dim tOm As Integer = repC.TextReplies - (trailposts - trailswithimages)
            Dim tIm As Integer = repC.ImageReplies - trailswithimages

            If tOm <= 0 Then
                postHtml = postHtml.Replace("%TCOUNT%", "")
            Else
                postHtml = postHtml.Replace("%TCOUNT%", p1str.Replace("%", CStr(tOm)))
            End If

            If tIm > 0 And tOm > 0 Then
                postHtml = postHtml.Replace("%AND%", andStr)
            Else
                postHtml = postHtml.Replace("%AND%", "")
            End If

            If tIm <= 0 Then
                postHtml = postHtml.Replace("%ICOUNT%", "")
            Else
                postHtml = postHtml.Replace("%ICOUNT%", p2str.Replace("%", CStr(tIm)))
            End If
            postHtml = postHtml.Replace("%omitStr%", omittedStr)
        End If
        postHtml = postHtml.Replace("%POSTLINK%", "default.aspx?id=" & threadID)
        Return postHtml
    End Function

    Private Function GetTimeString(ByVal d As Date) As String
        Return d.ToString
    End Function

    Private Function GetRepliesCount(ByVal threadID As Integer) As ThreadReplies
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim textRepliesCount As String = "Select Count(ID) as [Total Records] from board where (parentT=" & threadID & ") AND (imagename LIKE '')"
        Dim imageRepliesCount As String = "Select Count(ID) as [Total Records] from board where (parentT=" & threadID & ")  AND (imagename LIKE '%.%')"
        Dim queryObject As New SqlCommand(textRepliesCount, cnx)
        cnx.Open()
        Dim textRC As Integer = 0
        Dim iRC As Integer = 0
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        While reader.Read
            textRC = CInt(reader(0))
        End While
        reader.Close()
        queryObject.Dispose()
        queryObject = New SqlCommand(imageRepliesCount, cnx)
        reader = queryObject.ExecuteReader
        While reader.Read
            iRC = CInt(reader(0))
        End While
        reader.Close()
        cnx.Close()
        queryObject.Dispose()
        Dim t As New ThreadReplies
        t.TextReplies = textRC
        t.ImageReplies = iRC
        Return t
    End Function

    Function GetRepliesHTML(ByVal threadID As Integer, ByVal para As HTMLParameters) As String
        Dim sa As New StringBuilder
        For Each x In GetThreadChildrenPosts(threadID)
            sa.Append(GetSingleReplyHTML(x, para))
        Next
        sa.Append("<hr></hr>")
        Return sa.ToString
    End Function

    Private Function GetSingleReplyHTML(ByVal postid As Integer, ByVal para As HTMLParameters) As String
        Dim po As WPost = (FetchPostData(postid))
        Dim postHTML As String = postTemplate
        If po.email = "" Then
            postHTML = postHTML.Replace("%NAMESPAN%", "<span class='name'>%NAME%</span>")
        Else
            postHTML = postHTML.Replace("%NAMESPAN%", "<a href='mailto:%EMAIL%' class='useremail'><span class='name'>%NAME%</span></a>")
        End If
        If EnableUserID Then
            Dim idHt As String = idHtml
            idHt = idHt.Replace("%UID%", po.posterID)
            postHTML = postHTML.Replace("%UID%", idHt)
        Else
            postHTML = postHTML.Replace("%UID%", "")
        End If
        postHTML = postHTML.Replace("%EMAIL%", po.email)
        postHTML = postHTML.Replace("%ID%", CStr(po.PostID))
        If para.isTrailPost Then
            Dim cm As String = ProcessComment(po.comment, po.parent)
            If cm.Length > 1500 Then
                cm = cm.Remove(1500)
                cm = cm & commentToolong.Replace("%POSTLINK%", "default.aspx?id=" & po.parent & "#p" & po.PostID)
                postHTML = postHTML.Replace("%POST TEXT%", cm)
                cm = ""
            Else
                postHTML = postHTML.Replace("%POST TEXT%", cm)
            End If
        Else
            postHTML = postHTML.Replace("%POST TEXT%", ProcessComment(po.comment, po.parent))
        End If
        postHTML = postHTML.Replace("%DATE TEXT UTC%", GetTimeString(po.time))
        postHTML = postHTML.Replace("%SUBJECT%", po.subject)
        postHTML = postHTML.Replace("%NAME%", po.name)
        postHTML = postHTML.Replace("%DATE UTC UNIX%", CStr(po.time.ToFileTime))
        postHTML = postHTML.Replace("%POST LINK%", "default.aspx?id=" & po.parent & "#p" & po.PostID)
        'If para.IsModerator Then postHTML = postHTML.Replace("%MODPANEL%", GetModeratorHTMLMenu(CStr(po.PostID), para.ModeratorPowers)) Else postHTML = postHTML.Replace("%MODPANEL%", "")
        If para.IsModerator Then postHTML = postHTML.Replace("%MODPANEL%", para.modMenu.Replace("%ID%", CStr(po.PostID))) Else postHTML = postHTML.Replace("%MODPANEL%", "")
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
                Dim noscriptItems As New StringBuilder
                Dim count As Integer = po._imageP.Split(CChar(";")).Count
                Dim advanced As Boolean = False ' The first one is marked as active, the rest as notactive
                Dim rotatorTemplat As String = rotatorTemplate
                For Each ima In po._imageP.Split(CChar(";"))
                    Dim r As String = imageTemplate
                    Dim wpi As WPostImage = GetWPOSTIMAGE(ima.Replace(";", ""))
                    r = r.Replace("%ID%", CStr(po.PostID))
                    If Not advanced Then r = r.Replace("%AN%", "active") Else r = r.Replace("%AN%", "notactive")
                    r = r.Replace("%filec%", "")
                    r = r.Replace("%FILE NAME%", wpi.realname)
                    r = r.Replace("%IMAGE SRC%", GetImageWEBPATH(wpi.chanbName))
                    r = r.Replace("%FILE SIZE%", FormatSizeString(wpi.size))
                    r = r.Replace("%IMAGE SIZE%", wpi.dimensions)
                    r = r.Replace("%THUMB_LINK%", GetImageWEBPATHRE(wpi.chanbName))
                    r = r.Replace("%IMAGE MD5%", wpi.MD5)
                    r = r.Replace("%IMAGE EXT%", wpi.Extension)
                    Dim nr As String = noscriptItemHTML
                    nr = nr.Replace("%IMAGE SRC%", GetImageWEBPATH(wpi.chanbName))
                    nr = nr.Replace("%FILE NAME%", wpi.realname)
                    nr = nr.Replace("%THUMB_LINK%", GetImageWEBPATHRE(wpi.chanbName))
                    noscriptItems.Append(nr)
                    items.Append(r)
                    advanced = True
                Next
                rotatorTemplat = rotatorTemplat.Replace("%ID%", CStr(po.PostID))
                rotatorTemplat = rotatorTemplat.Replace("%IMAGECOUNT%", CStr(count))
                rotatorTemplat = rotatorTemplat.Replace("%ITEMS%", items.ToString.Replace(vbNewLine, "").Replace("'", "\'"))
                rotatorTemplat = rotatorTemplat.Replace("%NOS%", noscriptItems.ToString)
                sb.Append(rotatorTemplat)
            Else
                'Single image
                Dim r As String = imageTemplate
                Dim wpi As WPostImage = GetWPOSTIMAGE(po._imageP.Replace(";", ""))
                r = r.Replace("%ID%", CStr(po.PostID))
                r = r.Replace("%filec%", "file")
                r = r.Replace("%AN%", "") ' No need for active/notactive class since there is no rotator.
                r = r.Replace("%FILE NAME%", wpi.realname)
                r = r.Replace("%IMAGE SRC%", GetImageWEBPATH(wpi.chanbName))
                r = r.Replace("%FILE SIZE%", FormatSizeString(wpi.size))
                r = r.Replace("%IMAGE SIZE%", wpi.dimensions)
                r = r.Replace("%THUMB_LINK%", GetImageWEBPATHRE(wpi.chanbName))
                r = r.Replace("%IMAGE MD5%", wpi.md5)
                r = r.Replace("%IMAGE EXT%", wpi.Extension)
                sb.Append(r)
            End If
        Else
            'No image
        End If
        Return sb.ToString
    End Function

    Sub PermaDeleteAllPosts(ByVal deletefiles As Boolean)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "TRUNCATE TABLE board"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
        If deletefiles Then
            For Each x As IO.FileInfo In FileIO.FileSystem.GetDirectoryInfo(STORAGEFOLDER).GetFiles()
                x.Delete()
            Next
        End If
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
        If EnableArchive Then
            ArchivePost(id)
        Else
            Dim w As WPost = FetchPostData(id)
            If w.type = "0" Then ' post is a thread, delete replies first.
                For Each x In GetThreadChildrenPosts(id)
                    DeleteP(x, dF)
                Next
                DeleteP(id, dF)
            Else
                DeleteP(id, dF)
            End If
        End If
    End Sub

    Sub ArchivePost(ByVal id As Integer)
        Dim w As WPost = FetchPostData(id)
        If w.type = "0" Then
            For Each x In GetThreadChildrenPosts(id)
                ArchiveP(x)
            Next
            ArchiveP(id)
        Else
            ArchiveP(id)
        End If
    End Sub

    Private Sub ArchiveP(ByVal id As Integer)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryObject As New SqlCommand("UPDATE board SET mta = 1 WHERE(id = " & id & ")", cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        SqlConnection.ClearPool(cnx)
        cnx.Close()
    End Sub

    Private Sub DeleteP(ByVal id As Integer, ByVal dF As Boolean)
        If DeleteFiles Then DeletePostFiles(id)
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
                If ImageExist(ima.md5, postID) = False Then
                    Dim realPath As String = STORAGEFOLDER & "\" & ima.chanbName
                    Dim thumbPath As String = STORAGEFOLDER & "\th" & ima.chanbName
                    IO.File.Delete(realPath)
                    IO.File.Delete(thumbPath)
                End If
            Next
        End If
    End Sub

    Private Function FormatSizeString(ByVal size As Long) As String
        Dim B As Long = 1024
        Dim K As Long = 1024 * B
        Dim M As Long = 1024 * K
        Dim G As Long = 1024 * G
        Dim KB As Long = CLng(Fix(size / B))
        Dim MB As Long = CLng(Fix(size / K))
        Dim GB As Long = CLng(Fix(size / M))
        If KB = 0 Then
            Return size & " B"
        ElseIf KB > 0 And MB = 0 Then
            Return KB & " KB"
        ElseIf MB > 0 And GB = 0 Then
            Return MB & " MB"
        ElseIf GB > 0 Then
            Return GB & " GB"
        Else
            Return CStr(size)
        End If
    End Function

    Sub ToggleSticky(ByVal threadID As Integer)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim i As Integer = 0 ' 0 unsticky the thread.
        If Not IsSticky(threadID) Then
            'Need to sticky it.
            i = 1
        End If
        Dim queryString As String = "UPDATE board SET sticky = " & i & " WHERE (ID = " & threadID & " )"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Private Function IsSticky(ByVal id As Integer) As Boolean
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT sticky FROM board  WHERE (ID = " & id & " )"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        Dim p As Boolean = False
        While reader.Read
            If TypeOf reader(0) Is DBNull Or CInt(ConvertNoNull(reader(0))) <> 1 Then
                p = False
            Else
                p = True
            End If
        End While
        cnx.Close()
        Return p
    End Function

    Sub ToggleLock(ByVal threadID As Integer)
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim i As Integer = 0
        If Not IsLocked(threadID) Then
            i = 1
        End If
        Dim queryString As String = "UPDATE board SET locked = " & i & " WHERE (ID = " & threadID & " )"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        queryObject.ExecuteNonQuery()
        cnx.Close()
    End Sub

    Private Function IsLocked(ByVal id As Integer) As Boolean
        Dim cnx As New SqlConnection(SQLConnectionString)
        Dim queryString As String = "SELECT locked FROM board  WHERE (ID = " & id & " )"
        Dim queryObject As New SqlCommand(queryString, cnx)
        cnx.Open()
        Dim reader As SqlDataReader = queryObject.ExecuteReader
        Dim p As Boolean = False
        While reader.Read
            If TypeOf reader(0) Is DBNull Or CInt(ConvertNoNull(reader(0))) <> 1 Then
                p = False
            Else
                p = True
            End If
        End While
        cnx.Close()
        Return p
    End Function

    'Function LoginMod(ByVal name As String, ByVal pass As String) As HttpCookie
    '    If IsModLoginValid(name, pass) = True Then
    '        Dim md5pass As String = MD5(pass)
    '        Dim auth As String = GenerateModAuth(name, md5pass)
    '        Dim modcr As New HttpCookie("modcred")
    '        modcr.Values.Add("modname", name)
    '        modcr.Values.Add("modpass", md5pass)
    '        modcr.Values.Add("modauth", auth)
    '        Return modcr
    '    Else
    '        Return New HttpCookie("modcred")
    '    End If
    'End Function

    'Function VerifyModCookie(ByRef c As HttpCookieCollection) As Boolean
    '    If (c("modcred") IsNot Nothing) Then
    '        Dim name As String = c("modcred").Values("modname")
    '        Dim passScrambled As String = c("modcred").Values("modpass")
    '        Dim auth As String = c("modcred").Values("modauth")
    '        If GenerateModAuth(name, passScrambled) = auth Then
    '            Return True
    '        Else
    '            Return False
    '        End If
    '    Else
    '        Return False
    '    End If
    'End Function

    'Private Function GenerateModAuth(ByVal name As String, ByVal passmd5 As String) As String
    '    Dim privateSalt As String = "eigjwergiewrhgiworhgisrh4g86df4h86d4h86dghag" ' Change this to a random value
    '    Dim sha1 As New System.Security.Cryptography.SHA1CryptoServiceProvider
    '    Dim bytes() As Byte = System.Text.Encoding.ASCII.GetBytes(name & privateSalt & passmd5)
    '    Return ByteArrayToString(sha1.ComputeHash(bytes))
    'End Function

End Module
