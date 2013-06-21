Imports System.Data.Common

' Enable PDF Thumbnail generation
' Require TallComponent PDFRasterizer
#Const EnablePDF = True

Public Module GlobalFunctions

    Private dbi As New DBInitializer

#Region "Board Functions"

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

    Private Sub CheckForPrunedThreads()
        Dim currentThread As Integer() = GetThreads(0, (MaximumPages * ThreadPerPage) - 1, True, False) ' list of thread that haven't reached the last page. 
        Dim allThread As Integer() = GetThreads(0, MaximumPages * ThreadPerPage, True, False) ' list of all current threads.
        Dim l As New List(Of Integer) ' list of thread that should be pruned
        For Each x As Integer In allThread
            If Array.IndexOf(currentThread, x) = -1 Then
                'This thread should be pruned.
                l.Add(x)
            End If
        Next
        For Each x As Integer In l
            PrunePost(x, AutoDeleteFiles)
        Next
    End Sub

    ''' <summary>
    ''' Save single file.
    ''' </summary>
    ''' <param name="f"></param>
    ''' <param name="reply"></param>
    ''' <returns>File Name.</returns>
    ''' <remarks></remarks>
    Private Function saveFile(ByVal f As HttpPostedFile, ByVal reply As Boolean) As String
        ' If f.ContentLength > MaximumFileSize Then Throw New Exception(FileToBig)
        If FileIsImage(f) Then

            Dim w As Drawing.Image

            Try
                'Check if the file is a valid image
                w = Drawing.Image.FromStream(f.InputStream)
                'If RemoveEXIFData Then
                '    w = RemoveEXIF(w)
                'End If
            Catch ex As Exception
                'No image is required when replying
                If f.ContentLength = 0 And reply = True Then
                    Return ""
                    Exit Function
                Else
                    Throw New ArgumentException(BadOrNoImage)
                End If
            End Try


            Dim fileextension As String = f.FileName.Split(CChar(".")).ElementAt(f.FileName.Split(CChar(".")).Length - 1).ToLower

            Dim dd As String = CStr(Date.UtcNow.ToFileTime)
            'Full image path
            Dim p As String = StorageFolder & "\" & dd & "." & fileextension
            'Thumb path
            Dim thumb As String
            If fileextension = "png" Then
                thumb = StorageFolderThumbs & "\th" & dd & ".png"
            Else
                thumb = StorageFolderThumbs & "\th" & dd & ".jpg"
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

            'Save the image.
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
                    sp = wpi.ChanbName & ":" & CStr(wpi.Size) & ":" & wpi.Dimensions & ":" & wpi.RealName & ":" & wpi.MD5
                End If
            Else
                'chanb name : size in bytes : dimensions : realname : md5
                sp = dd & "." & fileextension & ":" & f.ContentLength & ":" & w.Size.Width & "x" & w.Size.Height & ":" & RemoveSpecialChars(f.FileName) & ":" & md5string
                w.Dispose()
            End If
            Return sp

        Else 'Maybe a PDF or SVG file or no file
            Dim fileextension As String = f.FileName.Split(CChar(".")).ElementAt(f.FileName.Split(CChar(".")).Length - 1)

            Select Case fileextension.ToUpper()
                Case "SVG"
                    Dim dd As String = CStr(Date.UtcNow.ToFileTime)
                    Dim p As String = StorageFolder & "\" & dd & "." & fileextension
                    'Thumb path
                    Dim thumb As String = StorageFolderThumbs & "\th" & dd & ".png"
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

                        Dim svgBi As Drawing.Bitmap

                        Try
                            Dim svgDoc As Svg.SvgDocument = Svg.SvgDocument.Open(p)
                            svgBi = svgDoc.Draw()
                        Catch ex As Exception
                            svgBi = New Drawing.Bitmap(150, 30)
                            Dim g As Drawing.Graphics = Drawing.Graphics.FromImage(svgBi)
                            g.Clear(Drawing.Color.White)
                            g.DrawString("SVG File", New Drawing.Font(Drawing.FontFamily.GenericMonospace, 20, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Pixel), Drawing.Brushes.Black, 0, 0)
                            g.Dispose()
                        End Try


                        'If IO.File.ReadAllText(p).Contains("<script>") Then

                        'End If

                        If (svgBi.Width * svgBi.Height) < 62500 Then
                            svgBi.Save(thumb)
                        Else
                            ResizeImage(svgBi, 250).Save(thumb)
                        End If
                        sp = dd & "." & fileextension & ":" & f.ContentLength & ":" & svgBi.Size.Width & "x" & svgBi.Size.Height & ":" & RemoveSpecialChars(f.FileName) & ":" & md5string
                        svgBi.Dispose()
                    End If

                    Return sp
                Case "PDF"
#If EnablePDF Then
                    Dim dd As String = CStr(Date.UtcNow.ToFileTime)
                    Dim p As String = StorageFolder & "\" & dd & "." & fileextension
                    'Thumb path
                    Dim thumb As String = StorageFolderThumbs & "\th" & dd & ".jpg"
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
                        sp = dd & "." & fileextension & ":" & f.ContentLength & ":" & pdfBi.Size.Width & "x" & pdfBi.Size.Height & ":" & RemoveSpecialChars(f.FileName) & ":" & md5string
                        pdfBi.Dispose()

                    End If
                    Return sp
#Else
                    Dim dd As String = CStr(Date.UtcNow.ToFileTime)
                    Dim p As String = StorageFolder & "\" & dd & "." & fileextension
                    'Thumb path
                    Dim thumb As String = StorageFolderThumbs & "\th" & dd & ".jpg"
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

                        graph.Clear(Drawing.Color.White)
                        graph.DrawString("PDF File", New Drawing.Font(Drawing.FontFamily.GenericMonospace, 20, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Pixel), Drawing.Brushes.Black, 0, 0)

                        graph.Dispose()


                        If (pdfBi.Width * pdfBi.Height) < 62500 Then
                            pdfBi.Save(thumb, Drawing.Imaging.ImageFormat.Jpeg)
                        Else
                            ResizeImage(pdfBi, 250).Save(thumb, Drawing.Imaging.ImageFormat.Jpeg)
                        End If
                        sp = dd & "." & fileextension & ":" & f.ContentLength & ":" & pdfBi.Size.Width & "x" & pdfBi.Size.Height & ":" & RemoveSpecialChars(f.FileName) & ":" & md5string
                        pdfBi.Dispose()

                    End If
                    Return sp
#End If

                Case "WEBM"

                    Dim dd As String = CStr(Date.UtcNow.ToFileTime)
                    Dim p As String = StorageFolder & "\" & dd & "." & fileextension
                    'Thumb path
                    Dim thumb As String = StorageFolderThumbs & "\th" & dd & ".jpg"
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


                        sp = dd & "." & fileextension & ":" & f.ContentLength & ":" & "video" & ":" & RemoveSpecialChars(f.FileName) & ":" & md5string


                    End If
                    Return sp

                Case "" ' A case of "" may occure when no file is uploaded. Simply return nothing.
                    Return ""
                Case Else
                    Throw New ArgumentException("Unsupported file type")
            End Select
        End If
    End Function

    Private Function RemoveEXIF(ByVal i As Drawing.Image) As Drawing.Image
        Dim mem As New IO.MemoryStream
        i.Save(mem, Drawing.Imaging.ImageFormat.Bmp)
        Dim bi As Drawing.Image = Drawing.Image.FromStream(mem)
        bi.Save(mem, i.RawFormat)
        Return Drawing.Image.FromStream(mem)
    End Function

    Public Function GetWPostImage(ByVal sp As String) As WPostImage
        Dim wp As New WPostImage
        wp.ChanbName = sp.Split(CChar(":")).ElementAt(0)
        wp.Size = CLng(sp.Split(CChar(":")).ElementAt(1))
        wp.Dimensions = sp.Split(CChar(":")).ElementAt(2)
        wp.RealName = sp.Split(CChar(":")).ElementAt(3)
        wp.MD5 = sp.Split(CChar(":")).ElementAt(4)
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

    Private Function FileIsImage(ByVal f As HttpPostedFile) As Boolean
        Dim extension As String = f.FileName.Split(CChar(".")).ElementAt(f.FileName.Split(CChar(".")).Length - 1).ToLower ' ToLower because string comparaison is case sensitive.
        Dim supportedImages As String() = {"jpg", "jpeg", "png", "bmp", "gif", "apng"}
        Dim bo As Boolean = False
        For Each x In supportedImages
            If extension = x Then
                bo = True
            End If
        Next
        Return bo
    End Function

    Private Function FileIsImage(ByVal fileExtension As String) As Boolean
        Dim supportedImages As String() = {"jpg", "jpeg", "png", "bmp", "gif", "apng", "svg", "pdf"}
        Dim bo As Boolean = False
        For Each x In supportedImages
            If fileExtension.ToLower = x Then
                bo = True
            End If
        Next
        Return bo
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
        Dim jpgThumb As String = StorageFolderThumbs & "\th" & name.Split(CChar(".")).ElementAt(0) & ".jpg"
        If FileIO.FileSystem.FileExists(jpgThumb) Then
            Return StoragefolderWEB & "thumbs/th" & name.Split(CChar(".")).ElementAt(0) & ".jpg"
        Else
            'Must be a png thumbnail
            Return StoragefolderWEB & "thumbs/th" & name.Split(CChar(".")).ElementAt(0) & ".png"
        End If
    End Function

    Private Function MakeBannedMessage(ByVal IP As String) As String
        Return BannedMessage
    End Function

    Friend Function GetPostHTML(ByVal po As WPost, ByVal pa As HTMLParameters) As String
        Dim postHTML As String

        Dim pageHandlerName As String = "default"
        If Not pa.isCurrentThread Then pageHandlerName = "archive"

        'Load the appropriate post template, and process specific templates variables.
        Select Case po.type
            Case WPost.PostType.Thread
                postHTML = OPPostTemplate

                If pa.replyButton Then
                    postHTML = postHTML.Replace("%REPLY BUTTON%", replyButtonHTML)
                Else
                    postHTML = postHTML.Replace("%REPLY BUTTON%", "")
                End If

                If po.isSticky Then
                    postHTML = postHTML.Replace("%STICKY%", "<img alt='" & stickyStr & "' title='" & stickyStr & "' src='" & WebRoot & "res/sticky.png' />")
                Else
                    postHTML = postHTML.Replace("%STICKY%", "")
                End If
                If po.locked Then
                    postHTML = postHTML.Replace("%LOCKED%", "<img alt='" & lockedStr & "' title='" & lockedStr & "' src='" & WebRoot & "res/locked.png' />")
                Else
                    postHTML = postHTML.Replace("%LOCKED%", "")
                End If

                postHTML = postHTML.Replace("%REPLY COUNT%", CStr(GetRepliesCount(CInt(po.PostID), Not pa.isCurrentThread).TotalReplies))

            Case WPost.PostType.Reply
                postHTML = ReplyPostTemplate
            Case Else
                Return ""
        End Select

        'Process generic post variables

        If EnableUserID Then
            Dim idHt As String = UserIDHtmlSPAN
            idHt = idHt.Replace("%UID%", po.posterID)
            postHTML = postHTML.Replace("%UID%", idHt)
            postHTML = postHTML.Replace("%UIDS%", po.posterID)
        Else
            postHTML = postHTML.Replace("%UID%", "")
            postHTML = postHTML.Replace("%UIDS%", "")
        End If
        If po.email = "" Then
            postHTML = postHTML.Replace("%NAMESPAN%", "<span class='name'>%NAME%</span>")
        Else
            postHTML = postHTML.Replace("%NAMESPAN%", "<a href='mailto:%EMAIL%' class='useremail'><span class='name'>%NAME%</span></a>")
            postHTML = postHTML.Replace("%EMAIL%", po.email)
        End If

        postHTML = postHTML.Replace("%ID%", CStr(po.PostID))

        postHTML = postHTML.Replace("%SUBJECT%", po.subject)
        postHTML = postHTML.Replace("%NAME%", po.name)
        postHTML = postHTML.Replace("%DATE UTC UNIX%", CStr(po.time.ToFileTime))
        postHTML = postHTML.Replace("%DATE UTC TEXT%", GetTimeString(po.time))


        If StaticHTML Then
            postHTML = postHTML.Replace("%POST LINK%", ThreadHTMLWebPath & po.PostID & ".html#p" & po.PostID)
        Else
            If po.archived Then
                If ConvertArchivedThreadToHTML Then
                    postHTML = postHTML.Replace("%POST LINK%", ArchivedTHTMLWebPath & po.PostID & ".html#p" & po.PostID)
                Else
                    postHTML = postHTML.Replace("%POST LINK%", pageHandlerName & ".aspx?id=" & po.PostID & "#p" & po.PostID)
                End If
            Else
                postHTML = postHTML.Replace("%POST LINK%", pageHandlerName & ".aspx?id=" & po.PostID & "#p" & po.PostID)
            End If
        End If

        postHTML = postHTML.Replace("%IMAGES%", GetFilesHTML(po))

        If pa.IsModerator Then postHTML = postHTML.Replace("%MODPANEL%", pa.modMenu.Replace("%ID%", CStr(po.PostID))) Else postHTML = postHTML.Replace("%MODPANEL%", "")
        ''Post text  
        Dim cm As String = ProcessComment(po, pageHandlerName, pa)
        If pa.isTrailPost Then
            If cm.Length > 1500 Then
                cm = cm.Remove(1500)
                If StaticHTML Then
                    cm = cm & commentToolong.Replace("%POSTLINK%", ThreadHTMLWebPath & po.PostID & ".html#p" & po.PostID)
                Else
                    cm = cm & commentToolong.Replace("%POSTLINK%", pageHandlerName & ".aspx?id=" & po.PostID & "#p" & po.PostID)
                End If
                postHTML = postHTML.Replace("%POST TEXT%", cm)
            Else
                postHTML = postHTML.Replace("%POST TEXT%", cm)
            End If ''
        Else
            postHTML = postHTML.Replace("%POST TEXT%", cm)
        End If

        Return postHTML
    End Function

    Public Function ProcessPost(ByVal request As HttpRequest, ByVal Session As Web.SessionState.HttpSessionState) As String
        ' Dim sb As New StringBuilder
        Dim message As String = ""
        Dim mode As String = request.Item("mode")
        Dim cont As Boolean = True
        If Session.Item("lastpost") Is "" Or Session.Item("lastpost") Is Nothing Then
            Session.Item("lastpost") = Now.ToString
        Else
            Dim i As Date = Date.Parse(CStr(Session.Item("lastpost")))
            If CInt((Now - i).TotalSeconds) < TimeBetweenRequestes Then
                message = FormatHTMLMessage("Error", FloodDetected.Replace("%", CStr(TimeBetweenRequestes)), "", "8888", True)
                cont = False
            Else
                Session.Item("lastpost") = Now.ToString
            End If
        End If
        If EnableCaptcha And (mode = "thread" Or mode = "reply") Then

            If Not Session("captcha") Is Nothing Then
                If Not Session("captcha").ToString = request.Item("usercaptcha") Then
                    message = FormatHTMLMessage("Error", wrongCaptcha, "", "8888", True)
                    cont = False
                End If
            Else
                message = FormatHTMLMessage("Error", wrongCaptcha, "", "8888", True)
                cont = False
            End If

        End If

        For Each fileKey As String In request.Files.AllKeys
            Dim f As HttpPostedFile = request.Files(fileKey)
            If f.ContentLength > MaximumFileSize Then
                message = FormatHTMLMessage(errorStr, FileToBig.Replace("%NAME%", f.FileName).Replace("%L%", CStr(FormatSizeString(MaximumFileSize))), "default.aspx", "8888", True)
                cont = False
            End If
        Next

        ''Post processing begin here 
        If cont Then
            If IsIPBanned(request.UserHostAddress) Then
                message = FormatHTMLMessage(BannedMessage, MakeBannedMessage(request.UserHostAddress), "default.aspx", "60", True)
            Else

                Select Case mode
                    Case "thread"
                        If request.Files.Count = 0 Then
                            message = FormatHTMLMessage("error", ImageRequired, "default.aspx", "60", True)
                        Else
                            'Save file.
                            If request.Files("ufile").ContentLength = 0 Then
                                message = FormatHTMLMessage("error", ImageRequired, "default.aspx", "60", True)
                            Else
                                'Check file size before saving.
                                If request.Files("ufile").ContentLength > MaximumFileSize Then
                                    message = FormatHTMLMessage("error", FileToBig, "default.aspx", "10", True)
                                    Exit Select
                                End If
                                Dim s As String = saveFile(request.Files("ufile"), False)
                                Dim er As New OPData
                                er.Comment = ProcessInputs(request.Item("comment"))
                                er.email = ProcessInputs(request.Item("email"))
                                If request.Item("postername") = "" Then er.name = AnonNameStr Else er.name = ProcessInputs(request.Item("postername"))
                                er.subject = ProcessInputs(request.Item("subject"))
                                er.time = Date.UtcNow
                                er.imageName = s
                                er.password = ProcessInputs(request.Item("password"))
                                er.IP = request.UserHostAddress
                                er.UserAgent = request.UserAgent.Replace("<", "").Replace(">", "") ' I replace < and > to prevent spoffing a user agent that contain <script> tags.
                                If request.Cookies("pass") IsNot Nothing Then request.Cookies("pass").Value = request.Item("password") Else request.Cookies.Add(New HttpCookie("pass", request.Item("password")))
                                Dim tid As Integer = MakeThread(er)

                                If Not er.name = "" Then Session("posterName") = er.name
                                If Not er.email = "" Then Session("posterEmail") = er.email

                                message = FormatHTMLMessage(SuccessfulPostString, SuccessfulPostString, "default.aspx?id=" & tid, "1", False)
                            End If
                        End If

                    Case "reply"

                        Dim threadid As Integer = CInt(request.Item("threadid"))

                        If IsLocked(threadid) Then
                            message = FormatHTMLMessage("error", lockedMessage, "default.aspx?id=" & threadid, "5", True)
                            Exit Select
                        End If

                        If IsArchived(threadid) Then
                            message = FormatHTMLMessage("error", arhivedMessage, "archive.aspx?id=" & threadid, "5", True)
                            Exit Select
                        End If

                        If EnableImpresonationProtection Then
                            If IsPosterNameAlreadyTaken(request.UserHostAddress, request.Item("postername"), threadid) Then
                                message = FormatHTMLMessage(errorStr, nameAlreadyUsed.Replace("%", request.Item("postername")), "default.aspx?id=" & threadid, "5", True)
                                Exit Select
                            End If
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
                                    If countFiles Then er.Comment = er.Comment & CStr(vbNewLine & pos & "&#47;" & totalFiles)
                                    advanced = True
                                Else
                                    If countFiles Then er.Comment = pos & "&#47;" & totalFiles Else er.Comment = String.Empty
                                End If

                                er.email = ProcessInputs(request.Item("email"))
                                If request.Item("postername") = "" Then er.name = AnonNameStr Else er.name = ProcessInputs(request.Item("postername"))
                                er.subject = ProcessInputs(request.Item("subject"))
                                er.time = Date.UtcNow
                                er.imageName = singleImage
                                er.password = ProcessInputs(request.Item("password"))
                                er.IP = request.UserHostAddress
                                er.UserAgent = request.UserAgent.Replace("<", "").Replace(">", "")
                                ReplyTo(threadid, er)
                                pos += 1
                            Next
                            message = FormatHTMLMessage(SuccessfulPostString, SuccessfulPostString, "default.aspx?id=" & request.Item("threadid"), "1", False)
                        Else
                            'Single file, or multiple files post.
                            Dim er As New OPData
                            If (request.Item("comment").Length = 0 Or request.Item("comment").Trim.Length = 0) And s = "" Then
                                'no image and no text
                                'blank post
                                message = FormatHTMLMessage("Error", noBlankpost, "", "7777", True)
                            Else
                                er.Comment = ProcessInputs(request.Item("comment"))
                                er.email = ProcessInputs(request.Item("email"))
                                If request.Item("postername") = "" Then er.name = AnonNameStr Else er.name = ProcessInputs(request.Item("postername"))
                                er.subject = ProcessInputs(request.Item("subject"))
                                er.time = Date.UtcNow
                                er.imageName = s
                                er.password = ProcessInputs(request.Item("password"))
                                er.IP = request.UserHostAddress
                                er.UserAgent = request.UserAgent.Replace("<", "").Replace(">", "") ' I replace < and > to prevent spoffing a user agent that contain <script> tags.
                                If request.Cookies("pass") IsNot Nothing Then request.Cookies("pass").Value = request.Item("password") Else request.Cookies.Add(New HttpCookie("pass", request.Item("password")))
                                ReplyTo(threadid, er)

                                If Not er.name = "" Then Session("posterName") = er.name
                                If Not er.email = "" Then Session("posterEmail") = er.email

                                message = FormatHTMLMessage(SuccessfulPostString, SuccessfulPostString, "default.aspx?id=" & request.Item("threadid"), "1", False)
                            End If
                        End If
                        'Check if to bump thread or not
                        If Not ProcessInputs(request.Item("email")) = "sage" And (GetRepliesCount(threadid, True).TotalReplies <= BumpLimit) Then BumpThread(threadid)

                        If StaticHTML Then UpdateThreadHtml(threadid)

                    Case reportStr

                        Dim il As New List(Of Integer)
                        For Each x As String In request.QueryString
                            If x.StartsWith("proc") Then
                                il.Add(CInt(x.Replace("proc", "")))
                            End If
                        Next
                        If il.Count = 0 Then
                            message = FormatHTMLMessage("error", NoPostWasSelected, "default.aspx", "60", True)
                        Else
                            Dim t As New StringBuilder
                            For Each x In il
                                ReportPost(x, request.UserHostAddress, Date.UtcNow)
                                t.Append(ReportedSucess.Replace("%", CStr(x)))
                                t.Append("<br>")
                            Next
                            message = FormatHTMLMessage("Ok", t.ToString, "", "7777", False)

                        End If

                    Case deleteStr
                        Dim li As New List(Of Integer)
                        Dim deletPass As String = request.Item("deletePass")
                        For Each x As String In request.QueryString
                            If x.StartsWith("proc") Then
                                li.Add(CInt(x.Replace("proc", "")))
                            End If
                        Next
                        If li.Count = 0 Then
                            message = FormatHTMLMessage("Error", NoPostWasSelected, "", "8888", True)
                        Else
                            Dim t As New StringBuilder
                            For Each p As WPost In GetWpostList(li.ToArray)
                                If p.password = deletPass Then
                                    PrunePost(CInt(p.PostID), AutoDeleteFiles)
                                    t.Append(PostDeletedSuccess.Replace("%", CStr(p.PostID)))
                                Else
                                    t.Append(CannotDeletePostBadPassword.Replace("%", CStr(p.PostID)))
                                End If
                                t.Append("<br/>")
                            Next
                            message = FormatHTMLMessage("Ok", t.ToString, "", "7777", False)
                        End If
                    Case Else
                        message = FormatHTMLMessage("Error", invalidPostmodestr, "default.aspx", "2", True)
                End Select
            End If
        End If 'End of post processing.
        Return message
    End Function

    Private Function ProcessComment(ByVal po As WPost, ByVal pageHandlerName As String, ByVal para As HTMLParameters) As String
        Dim sb As New StringBuilder
        Dim wf As New WordFilter
        Dim comment As String = po.comment

        comment = wf.FilterText(comment)
        For Each x In comment.Split(CChar(vbNewLine))
            If Not (x = "") Then
                'Check if greentext
                If x.StartsWith("&gt;") And Not x.StartsWith("&gt;&gt;") Then
                    sb.Append("<span class='quote'>" & x & "</span>")
                    'Some times, X start with a line terminator that is not vbnewline, so i remove it

                ElseIf (x.Remove(0, 1).StartsWith("&gt;") And Not x.Remove(0, 1).StartsWith("&gt;&gt;")) Then
                    sb.Append("<span class='quote'>" & x.Remove(0, 1) & "</span>")

                ElseIf IsXvalidQuote(x) Then
                    If po.archived Then
                        If ConvertArchivedThreadToHTML Then
                            sb.Append("<a href='" & ArchivedTHTMLWebPath & CStr(po.parent) & ".html#p" & x.Replace("&gt;&gt;", "") & "'>" & x & "</a>")
                        Else
                            sb.Append("<a href='" & pageHandlerName & ".aspx?id=" & CStr(po.parent) & "#p" & x.Replace("&gt;&gt;", "") & "'>" & x & "</a>")
                        End If
                    Else
                        If StaticHTML Then
                            sb.Append("<a href='" & ThreadHTMLWebPath & CStr(po.parent) & ".html#p" & x.Replace("&gt;&gt;", "") & "'>" & x & "</a>")
                        Else
                            sb.Append("<a href='" & pageHandlerName & ".aspx?id=" & CStr(po.parent) & "#p" & x.Replace("&gt;&gt;", "") & "'>" & x & "</a>")
                        End If
                    End If
                    'Some times, X start with a line terminator that is not vbnewline, so i remove it
                ElseIf IsXvalidQuote(x.Remove(0, 1)) Then
                    If po.archived Then
                        If ConvertArchivedThreadToHTML Then
                            sb.Append("<a href='" & ArchivedTHTMLWebPath & CStr(po.parent) & ".html#p" & x.Remove(0, 1).Replace("&gt;&gt;", "") & "'>" & x.Remove(0, 1) & "</a>")
                        Else
                            sb.Append("<a href='" & pageHandlerName & ".aspx?id=" & CStr(po.parent) & "#p" & x.Remove(0, 1).Replace("&gt;&gt;", "") & "'>" & x.Remove(0, 1) & "</a>")
                        End If
                    Else
                        If StaticHTML Then
                            sb.Append("<a href='" & ThreadHTMLWebPath & CStr(po.parent) & ".html#p" & x.Remove(0, 1).Replace("&gt;&gt;", "") & "'>" & x.Remove(0, 1) & "</a>")
                        Else
                            sb.Append("<a href='" & pageHandlerName & ".aspx?id=" & CStr(po.parent) & "#p" & x.Remove(0, 1).Replace("&gt;&gt;", "") & "'>" & x.Remove(0, 1) & "</a>")
                        End If
                    End If
                Else
                    sb.Append(x)
                End If
                sb.Append("<br>")
            End If
        Next

        Dim finalcomment As New StringBuilder

        Dim sr As String = sb.ToString
        sr = MatchAndProcessBBCodes("spoiler", sr)
        sr = MatchAndProcessBBCodes("code", sr)
        sr = MatchAndProcessBBCodes("md", sr)



        'If sr.Length > 1500 Then
        '    finalcomment.Append(sr.Remove(1500))
        'Else
        '    finalcomment.Append(sr)
        'End If



        'If sr.Contains("<pre>") Then finalcomment.Append("</pre>")
        'If sr.Contains("<s>") Then finalcomment.Append("</s>")

        'If sr.Contains("<div>") Then finalcomment.Append("</pre>")

        Return sr
    End Function

    ''' <summary>
    ''' Return a string for thread displayed in the main page.
    ''' </summary>
    ''' <param name="threadID"></param>
    ''' <param name="para"></param>
    ''' <param name="trailposts"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetStreamThreadHTML(ByVal threadID As Integer, ByVal para As HTMLParameters, ByVal trailposts As Integer) As String
        Dim postHtml As String = ThreadTemplate
        postHtml = postHtml.Replace("%ID%", CStr(threadID))

        Dim OPandChildrenIDS As New List(Of Integer)

        OPandChildrenIDS.Add(CInt(threadID))
        OPandChildrenIDS.AddRange(GetLastXPosts(threadID, trailposts, Not para.isCurrentThread))

        Dim wpolist As WPost() = GetWpostList(OPandChildrenIDS.ToArray)
        OPandChildrenIDS.Clear()

        postHtml = postHtml.Replace("%POST HTML%", GetPostHTML(wpolist(0), para))

        Dim trailswithimages As Integer = 0
        If trailposts > 0 And (wpolist.Length - 1 > 0) Then
            Dim sb As New StringBuilder
            For i As Integer = 1 To wpolist.Length - 1 Step 1
                sb.Append(GetPostHTML(wpolist(i), para))
                If Not wpolist(i)._imageP = "" Then
                    trailswithimages += 1
                End If
            Next
            postHtml = postHtml.Replace("%TRAILS%", sb.ToString)
        Else
            postHtml = postHtml.Replace("%TRAILS%", "")
        End If

        Dim repC As ThreadReplies = GetRepliesCount(threadID, Not para.isCurrentThread)
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
        If para.isCurrentThread Then
            If StaticHTML Then postHtml = postHtml.Replace("%POSTLINK%", ThreadHTMLWebPath & threadID & ".html") Else postHtml = postHtml.Replace("%POSTLINK%", "default.aspx?id=" & threadID)
        Else
            If StaticHTML Then postHtml = postHtml.Replace("%POSTLINK%", ArchivedTHTMLWebPath & threadID & ".html") Else postHtml = postHtml.Replace("%POSTLINK%", "default.aspx?id=" & threadID)
        End If
        Return postHtml
    End Function

    ''' <summary>
    ''' Get the complete thread html, over a single SQL query.
    ''' </summary>
    ''' <param name="threadID"></param>
    ''' <param name="p"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetThreadHTML(ByVal threadID As Integer, ByVal p As HTMLParameters) As String
        Dim sb As New StringBuilder
        Dim data As WPost() = GetThreadData(threadID, Not p.isCurrentThread)
        sb.Append(GetPostHTML(data(0), p))

        For i = 1 To data.Length - 1 Step 1
            sb.Append(GetPostHTML(data(i), p))
        Next
        Return sb.ToString
    End Function

    Function GetModeratorHTMLMenu(ByVal id As String, ByVal powers As String) As String
        Dim power As String() = powers.Split(CChar("-"))
        Dim sb As New StringBuilder
        Dim noscriptitem As New StringBuilder
        sb.Append("<script>document.write('")
        sb.Append("<div id=\'moda%ID%\'><select id=\'selc%ID%\' onchange=""updatemodlink(\'%ID%\')"">".Replace("%ID%", CStr(id)))
        For i As Integer = 0 To power.Length - 1 Step 1
            If power(i) = "1" Then
                sb.Append(modMenu(i).Replace("'", "\'"))
                noscriptitem.Append(modMenuNoscript(i))
            End If
        Next
        sb.Append("</select><a href=\'#\' id=\'modhref%ID%\' class=\'form-button\' target=\'_blank\' >OK</a></div>');</script><noscript>%NOSCRIPTMENU%</noscript>".Replace("%ID%", CStr(id)).Replace("%NOSCRIPTMENU%", noscriptitem.ToString))
        Return sb.ToString
    End Function

    Sub BanPosterByPost(ByVal postID As Integer)
        Dim po As WPost = FetchPostData(postID)
        Dim newText As String = po.comment & bannedMessageHTML
        If IsIPBanned(po.ip) = False Then
            BanPoster(po.ip, postID)
            UpdatePostText(postID, newText, True)
        End If
    End Sub

    Function GetRepliesHTML(ByVal threadID As Integer, ByVal para As HTMLParameters) As String
        Dim sa As New StringBuilder
        For Each po As WPost In GetWpostList(GetThreadChildrenPostsIDs(threadID, Not para.isCurrentThread))
            sa.Append(GetPostHTML(po, para))
        Next
        sa.Append("<hr />")
        Return sa.ToString
    End Function

    Private Function GetFilesHTML(ByVal po As WPost) As String
        Dim sb As New StringBuilder

        If Not (po._imageP = "") Then
            'At least one image is found. Check for more than 1 file
            Dim filesList As String() = po._imageP.Split(CChar(";"))

            If filesList.Count > 1 Then
                'We need to make HTML file rotator.
                Dim items As New StringBuilder
                Dim noscriptItems As New StringBuilder
                Dim advanced As Boolean = False ' The first one is marked as active, the rest as notactive
                Dim rotatorHTML As String = FilesRotatorTemplate

                For Each file As String In filesList

                    Dim wpi As WPostImage = GetWPostImage(file)

                    If FileIsImage(wpi.Extension) Then
                        Dim scriptItem As String = GetImageHTML(wpi, po.PostID)

                        If Not advanced Then scriptItem = scriptItem.Replace("%AN%", "active") Else scriptItem = scriptItem.Replace("%AN%", "notactive")
                        scriptItem = scriptItem.Replace("%filec%", "")

                        items.Append(scriptItem)
                        noscriptItems.Append(GetImageHTMLNoScript(wpi))

                    Else ' It's a video or an audio file.

                        Select Case wpi.Extension
                            Case "WEBM"
                                Dim scriptItem As String = GetVideoFileHTML(wpi, po.PostID, "webm")

                                If Not advanced Then scriptItem = scriptItem.Replace("%AN%", "active") Else scriptItem = scriptItem.Replace("%AN%", "notactive")

                                scriptItem = scriptItem.Replace("%filec%", "")

                                items.Append(scriptItem)
                                noscriptItems.Append(GetVideoFileHTMLNoScript(wpi))

                        End Select

                    End If

                    advanced = True
                Next

                rotatorHTML = rotatorHTML.Replace("%ID%", CStr(po.PostID))
                rotatorHTML = rotatorHTML.Replace("%IMAGECOUNT%", CStr(filesList.Count))
                rotatorHTML = rotatorHTML.Replace("%ITEMS%", items.ToString)
                rotatorHTML = rotatorHTML.Replace("%NOS%", noscriptItems.ToString)
                sb.Append(rotatorHTML)

            Else
                'Single image
                Dim wpi As WPostImage = GetWPostImage(po._imageP)
                If FileIsImage(wpi.Extension) Then
                    Dim item As String = GetImageHTML(wpi, po.PostID)
                    item = item.Replace("%filec%", "file") ' We need the 'file' html class in single image mode.
                    item = item.Replace("%AN%", "") ' No need for active/notactive html class since there is no rotator.
                    sb.Append(item)
                Else

                    Select Case wpi.Extension
                        Case "WEBM"
                            Dim item As String = GetVideoFileHTML(wpi, po.PostID, "webm")

                            item = item.Replace("%filec%", "file")
                            item = item.Replace("%AN%", "")

                            sb.Append(item)
                    End Select

                End If ' File extension check block
            End If ' Multiple file check block
        End If  'If there is no files, no action is needed.
        Return sb.ToString
    End Function

    Private Function GetImageHTML(ByVal wpi As WPostImage, ByVal postId As Integer) As String
        Dim r As String = ImageTemplate
        r = r.Replace("%ID%", CStr(postId))
        r = r.Replace("%FILE NAME%", wpi.RealName)
        If wpi.RealName.Length > 18 Then
            r = r.Replace("%FILE SNAME%", New String(CType(wpi.RealName, Char()), 0, 15) & "...")
        Else
            r = r.Replace("%FILE SNAME%", wpi.RealName)
        End If
        r = r.Replace("%IMAGE TEXT DL%", WebRoot & "img.aspx?cn=" & wpi.ChanbName & "&rn=" & wpi.RealName)
        r = r.Replace("%IMAGE DL%", GetImageWEBPATH(wpi.ChanbName))
        r = r.Replace("%IMAGE SRC%", GetImageWEBPATH(wpi.ChanbName))
        r = r.Replace("%FILE SIZE%", FormatSizeString(wpi.Size))
        r = r.Replace("%IMAGE SIZE%", wpi.Dimensions)
        r = r.Replace("%THUMB_LINK%", GetImageWEBPATHRE(wpi.ChanbName))
        r = r.Replace("%IMAGE MD5%", wpi.MD5)
        r = r.Replace("%IMAGE EXT%", wpi.Extension)
        r = r.Replace("%Search Engine Links%", GetSearchEngineLinks(GetImageWEBPATHRE(wpi.ChanbName)))
        Return r
    End Function

    Private Function GetImageHTMLNoScript(ByVal wpi As WPostImage) As String
        Dim nr As String = noscriptItemHTML
        nr = nr.Replace("%IMAGE SRC%", GetImageWEBPATH(wpi.ChanbName))
        nr = nr.Replace("%IMAGE DL%", GetImageWEBPATH(wpi.ChanbName))
        nr = nr.Replace("%FILE NAME%", wpi.RealName)
        nr = nr.Replace("%THUMB_LINK%", GetImageWEBPATHRE(wpi.ChanbName))
        Return nr
    End Function

    Private Function GetVideoFileHTML(ByVal wpi As WPostImage, ByVal postId As Integer, ByVal ext As String) As String
        Dim r As String = VideoItemTemplate
        r = r.Replace("%ID%", CStr(postId))
        r = r.Replace("%FILE NAME%", wpi.RealName)
        If wpi.RealName.Length > 18 Then
            r = r.Replace("%FILE SNAME%", New String(CType(wpi.RealName, Char()), 0, 15) & "...")
        Else
            r = r.Replace("%FILE SNAME%", wpi.RealName)
        End If
        r = r.Replace("%FILE SIZE%", FormatSizeString(wpi.Size))
        r = r.Replace("%IMAGE TEXT DL%", WebRoot & "img.aspx?cn=" & wpi.ChanbName & "&rn=" & wpi.RealName)
        r = r.Replace("%VIDEO LINK%", GetImageWEBPATH(wpi.ChanbName))
        r = r.Replace("%IMAGE MD5%", wpi.MD5)
        r = r.Replace("%IMAGE EXT%", wpi.Extension)
        r = r.Replace("%NO VIDEO SUPPORT%", noVideoSupportStr)
        r = r.Replace("%EXT%", ext)
        Return r
    End Function

    Private Function GetVideoFileHTMLNoScript(ByVal wpi As WPostImage) As String
        Dim nr As String = noscriptVideoHTML
        nr = nr.Replace("%VIDEO LINK%", GetImageWEBPATH(wpi.ChanbName))
        nr = nr.Replace("%FILE NAME%", wpi.RealName)
        Return nr
    End Function

    Private Function GetSearchEngineLinks(ByVal thumbnailURL As String) As String
        Dim g As New StringBuilder
        For Each searchEngine As String In searchEngineLinkList
            g.Append(searchEngine.Replace("%THUMB_LINK%", thumbnailURL))
            g.Append("&nbsp;")
        Next
        Return g.ToString
    End Function

    Public Sub PermaDeleteAllPosts(ByVal deletefiles As Boolean)
        DatabaseEngine.ExecuteNonQuery("TRUNCATE TABLE board")
        If deletefiles Then
            For Each x As IO.FileInfo In FileIO.FileSystem.GetDirectoryInfo(StorageFolder).GetFiles()
                x.Delete()
            Next
        End If
    End Sub

    Public Sub PrunePost(ByVal id As Integer, ByVal dF As Boolean)
        If EnableArchive Then
            Archive(id)
        Else
            Dim w As WPost = FetchPostData(id)
            If w.type = WPost.PostType.Thread Then ' post is a thread, delete replies first.
                If dF Then
                    For Each x As WPost In GetThreadData(id, True)
                        DeletePostFiles(x)
                    Next
                End If
                DeleteThread(id)
            Else
                If dF Then DeletePostFiles(w)
                DeletePost(id)
            End If
        End If
    End Sub

    Public Sub Archive(ByVal id As Integer)
        Dim w As WPost = FetchPostData(id)
        If w.type = WPost.PostType.Thread Then
            ArchiveThread(id)
            'Static thread in the archive.
            If ConvertArchivedThreadToHTML Then
                UpdateThreadHtmlArchive(id)
            End If
        Else
            ArchivePost(id)
        End If
    End Sub

    Private Sub ArchiveThread(ByVal threadid As Integer)
        DatabaseEngine.ExecuteNonQuery("UPDATE board SET mta = 1 WHERE (parentT = " & threadid & ") OR (ID = " & threadid & ")")
    End Sub

    Private Sub ArchivePost(ByVal id As Integer)
        DatabaseEngine.ExecuteNonQuery("UPDATE board SET mta = 1 WHERE(id = " & id & ")")
    End Sub

    Private Sub DeleteThread(ByVal threadid As Integer)
        DatabaseEngine.ExecuteNonQuery("DELETE FROM board WHERE (parentT = " & threadid & ") OR (ID = " & threadid & ")")
    End Sub

    Private Sub DeletePost(ByVal id As Integer)
        DatabaseEngine.ExecuteNonQuery("DELETE FROM board WHERE (id = " & id & ")")
    End Sub

    Private Sub DeletePostFiles(ByVal po As WPost)
        If po._imageP = "" Then
            Exit Sub
        Else
            For Each x In po._imageP.Split(CChar(";"))
                Dim ima As WPostImage = GetWPostImage(po._imageP)
                If ImageExist(ima.MD5, CInt(po.PostID)) = False Then
                    Dim realPath As String = StorageFolder & "\" & ima.ChanbName
                    Dim thumbPath As String = StorageFolderThumbs & "\th" & ima.ChanbName
                    IO.File.Delete(realPath)
                    'Don't delete thumbs when archive is enabled.
                    If Not EnableArchive Then IO.File.Delete(thumbPath)
                End If
            Next
        End If
    End Sub

    Public Function GeneratePageHTML(ByVal isArchive As Boolean, ByVal session As HttpSessionState, ByVal Request As HttpRequest, ByVal Response As HttpResponse) As String
        Dim DisplayingThread As Boolean = Not (Request.Item("id") = "")

        Dim pageHTML As String = GenerateGenericHTML()

        Dim pageHandlerLink As String = "default"
        If isArchive Then pageHandlerLink = "archive"

        If isArchive Then
            pageHTML = pageHTML.Replace("%POSTING MODE%", archiveNotice)
        Else
            If DisplayingThread Then
                pageHTML = pageHTML.Replace("%POSTING MODE%", postingModeReplyHtml)
            Else
                pageHTML = pageHTML.Replace("%POSTING MODE%", "")
            End If
        End If

        If isArchive Then
            pageHTML = pageHTML.Replace("%POSTDIVCLASS%", "hide")
        Else
            pageHTML = pageHTML.Replace("%POSTDIVCLASS%", "")
        End If

        If DisplayingThread Then
            pageHTML = pageHTML.Replace("%POST FORM MODE%", "reply")
        Else
            pageHTML = pageHTML.Replace("%POST FORM MODE%", "thread")
        End If

        pageHTML = pageHTML.Replace("%POST FORM TID%", Request.Item("id"))

        If DisplayingThread Then
            pageHTML = pageHTML.Replace("%POST FORM BUTTON%", replyStr)
        Else
            pageHTML = pageHTML.Replace("%POST FORM BUTTON%", newThreadStr)
        End If

        If EnableCaptcha And Not isArchive Then
            pageHTML = pageHTML.Replace("%CAPTCHA PHOLDER%", captchaTableEntryHtml)
        Else
            pageHTML = pageHTML.Replace("%CAPTCHA PHOLDER%", "")
        End If

        If DisplayingThread Then
            pageHTML = pageHTML.Replace("%ADD NEW FILES PHOLDER%", addNewFileButtonHTML)
        Else
            pageHTML = pageHTML.Replace("%ADD NEW FILES PHOLDER%", "")
        End If

        pageHTML = pageHTML.Replace("%MAXIMUM FILE SIZE%", FormatSizeString(MaximumFileSize))
        pageHTML = pageHTML.Replace("%SESSION PASSWORD%", GetSessionPassword(Request.Cookies, session))

        If DisplayingThread Then
            pageHTML = pageHTML.Replace("%POSTING RULES%", postingRulesHTML)
            pageHTML = pageHTML.Replace("%THREAD COUNT%", "")
        Else
            pageHTML = pageHTML.Replace("%POSTING RULES%", "")
            pageHTML = pageHTML.Replace("%THREAD COUNT%", threadCountHTMLli.Replace("%", CStr(GetThreadsCount(isArchive))))
        End If

        If DisplayingThread Then
            pageHTML = pageHTML.Replace("%RETURN BUTTON DESKTOP%", DesktopReturnButtonHTML.Replace("%P%", pageHandlerLink & ".aspx"))
            pageHTML = pageHTML.Replace("%RETURN BUTTON MOBILE%", MobileReturnButtonHTML.Replace("%P%", pageHandlerLink & ".aspx"))
        Else
            pageHTML = pageHTML.Replace("%RETURN BUTTON DESKTOP%", "")
            pageHTML = pageHTML.Replace("%RETURN BUTTON MOBILE%", "")
        End If

        If Not (session("posterName") Is Nothing Or session("posterEmail") Is Nothing) Then
            pageHTML = pageHTML.Replace("%COOKIE EMAIL%", session("posterEmail").ToString)
            pageHTML = pageHTML.Replace("%COOKIE NAME%", session("posterName").ToString)
        Else
            pageHTML = pageHTML.Replace("%COOKIE EMAIL%", "")
            pageHTML = pageHTML.Replace("%COOKIE NAME%", "")
        End If

        pageHTML = pageHTML.Replace("%META NO CACHE%", "")

        '####################################### BODY PROCESSING CODE #######################################
        Dim body As New StringBuilder
        Dim para As New HTMLParameters()
        para.IsModerator = CBool(session("mod"))
        para.ModeratorPowers = CStr(session("modpowers"))
        para.modMenu = CStr(session("modmenu"))
        para.isCurrentThread = Not isArchive

        Dim validID As Boolean = False
        Try
            Dim i = CInt(Request.Item("id"))
            validID = True
        Catch ex As Exception
            validID = False
        End Try


        If DisplayingThread And validID Then

            body.Append("<script type='text/javascript'> timer();</script>")

            'Display a thread and children posts 
            Dim opID As Integer = CInt(Request.Item("id"))
            opID = Math.Abs(opID)
            Dim po As WPost = FetchPostData(opID)

            If StaticHTML Then
                If FileIO.FileSystem.FileExists(ThreadStorageFolder & "\" & opID & ".html") = False Then UpdateThreadHtml(opID)
                Response.Redirect(ThreadHTMLWebPath & opID & ".html")
            End If

            If po.type = WPost.PostType.Unknown Then
                Response.Redirect(pageHandlerLink & ".aspx")
            End If

            If po.archived And Not isArchive Then
                Response.Redirect("archive.aspx?id=" & po.PostID)
            ElseIf po.archived = False And isArchive Then
                Response.Redirect("default.aspx?id=" & po.PostID)
            End If

            ' Check if it is a reply or a thread , 0 = thread, 1 = reply
            ' If it is a reply, redirect to parent thread.
            If CInt(po.type) = 1 Then Response.Redirect(pageHandlerLink & ".aspx?id=" & po.parent & "#p" & po.PostID)


            para.replyButton = False
            para.isTrailPost = False

            body.Append("<div class='thread' id='t" & opID & "'>")
            body.Append(GetThreadHTML(opID, para))
            body.Append("</div><hr ></hr>")

        Else

            'Display a list of current threads
            Dim startIndex As Integer = 0
            para.replyButton = True
            para.isTrailPost = True
            If Not (Request.Item("startindex") = "") Then startIndex = CInt(Request.Item("startindex")) * (ThreadPerPage)
            For Each x In GetThreads(startIndex, ThreadPerPage - 1 + startIndex, False, isArchive)
                body.Append(GetStreamThreadHTML(x, para, TrailPosts))
            Next

        End If
        pageHTML = pageHTML.Replace("%BODY%", body.ToString)
        '####################################### END OF BODY PROCESSING CODE #######################################

        '####################################### BEGIN OF PAGE LIST PROCESSING CODE ################################
        If Not DisplayingThread Then ' Show pages numbers list

            Dim sb As New StringBuilder
            sb.Append("<div align='center' class='pagelist desktop'>")

            Dim threadCount As Integer = GetThreadsCount(isArchive)
            Dim pagesCount As Double = threadCount / ThreadPerPage
            If pagesCount > (Fix(pagesCount)) Then
                pagesCount = Fix(pagesCount) + 1
            End If
            Dim startIndexA As Integer
            Try
                startIndexA = CInt(Request.Item("startindex"))
            Catch ex As Exception
                startIndexA = 0
            End Try
            If startIndexA = 0 Then
                sb.Append("<div class='prev'><a class='form-button-disabled'>" & prevStr & "</a></div>")
            Else
                sb.Append("<div><a class='form-button' href='" & pageHandlerLink & ".aspx?startindex=" & CStr(startIndexA - 1) & "'>" & prevStr & "</a></div>")
            End If
            sb.Append("<div class='pages'>")
            For i As Integer = 0 To CInt((pagesCount - 1)) Step 1
                If i = startIndexA Then
                    sb.Append("[<strong><a href='" & pageHandlerLink & ".aspx?startindex=" & i & "'>" & i + 1 & "</a></strong>]")
                Else
                    sb.Append("[<a href='" & pageHandlerLink & ".aspx?startindex=" & i & "'>" & i + 1 & "</a>]")
                End If
            Next
            sb.Append("</div>")
            If startIndexA = pagesCount - 1 Or threadCount = 0 Then ' last page
                sb.Append("<div class='next'><a class='form-button-disabled'>" & nextStr & "</a></div></div>")
            Else
                sb.Append("<div><a class='form-button' href='" & pageHandlerLink & ".aspx?startindex=" & CStr(startIndexA + 1) & "'>" & nextStr & "</a></div></div>")
            End If

            pageHTML = pageHTML.Replace("%PAGES LIST%", sb.ToString)
        Else
            pageHTML = pageHTML.Replace("%PAGES LIST%", "")
        End If
        '####################################### END OF PAGE LIST PROCESSING CODE ################################

        Return pageHTML
    End Function

    Public Function GenerateCatalogPage(ByVal Request As HttpRequest, ByVal session As HttpSessionState) As String
        Dim pageHTML As String = GenerateGenericHTML()


        pageHTML = pageHTML.Replace("%POSTING MODE%", "")
        pageHTML = pageHTML.Replace("%POSTDIVCLASS%", "")
        pageHTML = pageHTML.Replace("%POST FORM MODE%", "thread")
        pageHTML = pageHTML.Replace("%POST FORM TID%", "")
        pageHTML = pageHTML.Replace("%POST FORM BUTTON%", newThreadStr)


        If EnableCaptcha Then
            pageHTML = pageHTML.Replace("%CAPTCHA PHOLDER%", captchaTableEntryHtml)
        Else
            pageHTML = pageHTML.Replace("%CAPTCHA PHOLDER%", "")
        End If

        pageHTML = pageHTML.Replace("%ADD NEW FILES PHOLDER%", "")

        pageHTML = pageHTML.Replace("%MAXIMUM FILE SIZE%", FormatSizeString(MaximumFileSize))
        pageHTML = pageHTML.Replace("%SESSION PASSWORD%", GetSessionPassword(Request.Cookies, session))

        pageHTML = pageHTML.Replace("%POSTING RULES%", "")
        pageHTML = pageHTML.Replace("%THREAD COUNT%", "")

        If Not (session("posterName") Is Nothing Or session("posterEmail") Is Nothing) Then
            pageHTML = pageHTML.Replace("%COOKIE EMAIL%", session("posterEmail").ToString)
            pageHTML = pageHTML.Replace("%COOKIE NAME%", session("posterName").ToString)
        Else
            pageHTML = pageHTML.Replace("%COOKIE EMAIL%", "")
            pageHTML = pageHTML.Replace("%COOKIE NAME%", "")
        End If

        pageHTML = pageHTML.Replace("%META NO CACHE%", "")
        pageHTML = pageHTML.Replace("%RETURN BUTTON DESKTOP%", DesktopReturnButtonHTML.Replace("%P%", "default.aspx"))
        pageHTML = pageHTML.Replace("%RETURN BUTTON MOBILE%", MobileReturnButtonHTML.Replace("%P%", "default.aspx"))


        '####################################### BODY PROCESSING CODE #######################################
        Dim body As New StringBuilder

        body.Append(GenerateCatalogItems(GetThreads(0, GetThreadsCount(False), False, False)))

        pageHTML = pageHTML.Replace("%BODY%", body.ToString)

        pageHTML = pageHTML.Replace("%PAGES LIST%", "")


        Return pageHTML
    End Function

    Private Function GenerateCatalogItems(ByVal ids As Integer()) As String
        Dim sb As New StringBuilder
        Dim data As WPost() = GetWpostList(ids)
        sb.Append("<div align='center' id='threads'>")
        For Each x In data
            Dim t As String = CatalogItemTemplate
            Dim i As WPostImage = GetWPostImage(x._imageP)
            Dim ci As ThreadReplies = GetRepliesCount(CInt(x.PostID), False)
            t = t.Replace("%ID%", CStr(x.PostID))
            t = t.Replace("%POST LINK%", "default.aspx?id=" & CStr(x.PostID))
            t = t.Replace("%THUMB SRC%", GetImageWEBPATHRE(i.ChanbName))
            t = t.Replace("%IMAGE MD5%", i.MD5)
            t = t.Replace("%TC%", CStr(ci.TextReplies))
            t = t.Replace("%IC%", CStr(ci.ImageReplies))
            t = t.Replace("%POST TEXT%", x.comment)
            sb.Append(t)
        Next
        sb.Append("</div>")
        Return sb.ToString
    End Function

    Private Function GenerateGenericHTML() As String
        Dim pageHTML As String = FullPageTemplate
        pageHTML = pageHTML.Replace("%BTITLE%", BoardTitle)
        pageHTML = pageHTML.Replace("%BDESC%", BoardDesc)
        pageHTML = pageHTML.Replace("%ROOT%", WebRoot)
        pageHTML = pageHTML.Replace("%LANG nameString%", nameStr)
        pageHTML = pageHTML.Replace("%LANG emailString%", emailStr)
        pageHTML = pageHTML.Replace("%LANG subjectString%", subjectStr)
        pageHTML = pageHTML.Replace("%LANG commentString%", commentStr)
        pageHTML = pageHTML.Replace("%LANG fileStr%", filesStr)
        pageHTML = pageHTML.Replace("%LANG passwordStr%", passwordStr)
        pageHTML = pageHTML.Replace("%LANG FOR PD%", forPostDelStr)
        pageHTML = pageHTML.Replace("%LANG catalogStr%", CatalogStr)
        pageHTML = pageHTML.Replace("%LANG bottomStr%", BottomStr)
        pageHTML = pageHTML.Replace("%LANG refreshStr%", RefreshStr)
        pageHTML = pageHTML.Replace("%LANG topStr%", TopStr)
        pageHTML = pageHTML.Replace("%LANG badCaptha%", wrongCaptcha)
        pageHTML = pageHTML.Replace("%LANG reportStr%", reportStr)
        pageHTML = pageHTML.Replace("%LANG deleteStr%", deleteStr)
        pageHTML = pageHTML.Replace("%LANG newthreadStr%", newThreadStr)
        pageHTML = pageHTML.Replace("%LANG archiveStr%", ArchiveStr)
        pageHTML = pageHTML.Replace("%FOOTER TEXT%", footerText)
        Return pageHTML
    End Function

#End Region

#Region "Database Functions"


    Function FetchPostData(ByVal id As Integer) As WPost
        Dim queryString As String = "SELECT type, time, comment, postername, email, password, parentT, subject, imagename, IP, ua, posterID, sticky, locked, mta FROM  board  WHERE (id = " & id & ")"
        Dim queryObject As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim reader As IDataReader = queryObject.Reader
        Dim po As New WPost(id)
        While reader.Read
            po.type = CType(reader.GetInt32(0), WPost.PostType)
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
        queryObject.Connection.Close()
        Return po
    End Function

    Function MakeThread(ByVal data As OPData) As Integer
        Dim queryStr As String = ""
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand()

        Select Case dbType
            Case "mssql"
                queryStr = "INSERT INTO board (type, time, comment, postername, email, password, subject, imagename, IP, bumplevel, ua, sticky, mta) OUTPUT INSERTED.ID " & _
                                          " VALUES (@type, @time, @comment, @postername, @email, @password, @subject, @imagename, @IP, @bumplevel, @ua, @sticky, @mta)"
            Case "mysql"
                queryStr = "INSERT INTO board (type, time, comment, postername, email, password, subject, imagename, IP, bumplevel, ua, sticky, mta) " & _
                "  VALUES (@type, @time, @comment, @postername, @email, @password, @subject, @imagename, @IP, @bumplevel, @ua, @sticky, @mta)  ; SELECT last_insert_id()"
            Case Else
                If isInstalled Then
                    Throw New Exception(dbTypeInvalid)
                End If
        End Select

        command.CommandText = queryStr

        command.Parameters.Add(DatabaseEngine.MakeParameter("@type", 0, System.Data.DbType.Int32)) ' Set post type to thread
        command.Parameters.Add(DatabaseEngine.MakeParameter("@time", data.time, System.Data.DbType.DateTime))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@comment", data.Comment, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@postername", data.name, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@email", data.email, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@password", data.password, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@subject", data.subject, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@imagename", data.imageName, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@IP", data.IP, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@bumplevel", Now, System.Data.DbType.DateTime))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@ua", data.UserAgent, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@sticky", 0, System.Data.DbType.Int32))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@mta", 0, System.Data.DbType.Int32))

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)

        Dim postID As Integer
        While query.Reader.Read
            postID = CInt(query.Reader(0))
        End While

        query.Reader.Close()
        command.Dispose()

        'We need to update op poster ID
        If EnableUserID Then
            command = DatabaseEngine.GenerateDbCommand

            command.CommandText = "UPDATE board SET posterID = @posterID WHERE (ID = @tid)"

            command.Parameters.Add(DatabaseEngine.MakeParameter("@posterID", GenerateUID(postID, data.IP), System.Data.DbType.String))
            command.Parameters.Add(DatabaseEngine.MakeParameter("@tid", postID, System.Data.DbType.Int32))

            DatabaseEngine.ExecuteNonQuery(command, query.Connection)

        End If

        query.Connection.Close()

        If StaticHTML Then UpdateThreadHtml(postID)
        CheckForPrunedThreads() ' Check for thread that reach beyond the last page, and prune them when necessary.
        Return postID
    End Function

    ''' <summary>
    ''' Append a post to thread
    ''' </summary>
    ''' <param name="id">id of the thread</param>
    ''' <param name="data">Poster data</param>
    ''' <remarks></remarks>
    Private Sub ReplyTo(ByVal id As Integer, ByVal data As OPData)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand

        command.CommandText = "INSERT INTO board (type, time, comment, postername, email, password, parentT, subject, imagename, IP, ua, posterID, mta) VALUES" & _
                                "(@type, @time, @comment, @postername, @email, @password, @parentT, @subject, @imagename, @IP, @ua, @posterId, @mta)"


        command.Parameters.Add(DatabaseEngine.MakeParameter("@type", 1, System.Data.DbType.Int32)) ' Mark the post as a reply

        command.Parameters.Add(DatabaseEngine.MakeParameter("@parentT", CInt(id), System.Data.DbType.Int32)) ' Set the post owner thread

        'Insert Post data

        command.Parameters.Add(DatabaseEngine.MakeParameter("@time", data.time, System.Data.DbType.DateTime))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@comment", CStr(data.Comment), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@postername", CStr(data.name), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@email", CStr(data.email), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@password", CStr(data.password), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@subject", CStr(data.subject), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@imagename", CStr(data.imageName), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@IP", CStr(data.IP), System.Data.DbType.String)) '

        command.Parameters.Add(DatabaseEngine.MakeParameter("@ua", CStr(data.UserAgent), System.Data.DbType.String)) '

        command.Parameters.Add(DatabaseEngine.MakeParameter("@posterId", GenerateUID(id, data.IP), System.Data.DbType.String)) '

        command.Parameters.Add(DatabaseEngine.MakeParameter("@mta", 0, System.Data.DbType.Int32)) ' Mark the post as not archived

        DatabaseEngine.ExecuteNonQuery(command)
    End Sub

    Public Function GetThreadsCount(ByVal archive As Boolean) As Integer
        Dim queryString As String = ""
        If archive Then queryString = "Select Count(ID) as T FROM board where (type=0) AND (mta=1) " Else queryString = "Select Count(ID) as T FROM board where (type=0) AND (mta=0)"
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim i As Integer = 0
        While query.Reader.Read
            i = CInt(query.Reader(0))
        End While
        query.Connection.Close()
        Return i
    End Function

    Private Function IsPosterNameAlreadyTaken(ByVal IP As String, ByVal name As String, ByVal tid As Integer) As Boolean
        If name = "" Then name = AnonNameStr
        If name = AnonNameStr Then
            Return False
        Else
            Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
            Dim queryStr As String = ""
            Select Case dbType
                Case "mssql"
                    queryStr = "SELECT TOP 1 IP FROM board WHERE (postername = @name) AND (parentT = @tid OR ID = @tid) ORDER BY ID"
                Case "mysql"
                    queryStr = "SELECT IP FROM board WHERE (postername = @name) AND (parentT = @tid OR ID = @tid) ORDER BY ID LIMIT 0,1"
                Case Else
                    If isInstalled Then
                        Throw New Exception(dbTypeInvalid)
                    End If
            End Select

            command.CommandText = queryStr

            command.Parameters.Add(DatabaseEngine.MakeParameter("@name", name, Data.DbType.String))
            command.Parameters.Add(DatabaseEngine.MakeParameter("@tid", tid, Data.DbType.Int32))


            Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
            Dim takerIP As String = ""
            While query.Reader.Read
                takerIP = query.Reader.GetString(0)
            End While
            query.Connection.Close()
            If takerIP = "" Then
                'New name
                Return False
            Else
                'Already used name
                Return Not (takerIP = IP)
            End If
            ' If the takerIP ( the first one who posted with that name ) Have the same IP as the IP variable, then it is the same person, otherwise It is not the same person and we should note that the name was taken.
        End If
    End Function

    Private Sub BumpThread(ByVal id As Integer)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "UPDATE board SET bumplevel = @bump WHERE (ID = @id)"
        command.Parameters.Add(DatabaseEngine.MakeParameter("@bump", Now, Data.DbType.DateTime))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@id", id, Data.DbType.Int32))
        DatabaseEngine.ExecuteNonQuery(command)
    End Sub


    Public Function GetThreadChildrenPostsIDs(ByVal id As Long, ByVal includearchived As Boolean) As Integer()
        Dim ila As New List(Of Integer)
        Dim queryString As String = ""
        If includearchived Then
            queryString = "SELECT ID FROM board  WHERE (parentT = " & id & ") ORDER BY ID"
        Else
            queryString = "SELECT ID FROM board  WHERE (parentT = " & id & ") AND (mta = 0) ORDER BY ID"
        End If
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        While query.Reader.Read
            ila.Add(CInt(query.Reader(0)))
        End While
        query.Connection.Close()
        Return ila.ToArray
    End Function

    ''' <summary>
    ''' Retrive all ids over a single sql connection.
    ''' </summary>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetWpostList(ByVal id As Integer()) As WPost()
        Dim il As New List(Of WPost)
        If id.Length = 0 Then
            Return il.ToArray
        Else
            'Prepare sql connection string.
            'Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
            'command.CommandText = "SELECT ID, type, time, comment, postername, email, password, parentT, subject, imagename, IP, ua, posterID, sticky, locked, mta FROM  board  WHERE (ID = @id) ORDER BY ID ASC"

            'For Each x As Int32 In id
            '    command.Parameters.Add(DatabaseEngine.MakeParameter("@id", x, Data.DbType.Int32))
            'Next

            Dim sb As New StringBuilder
            sb.Append("WHERE (ID = " & CStr(id(0)) & ")")

            For i As Integer = 1 To id.Length - 1 Step 1
                sb.Append(" OR (ID = " & id(i) & ") ")
            Next

            Dim queryString As String = "SELECT ID, type, time, comment, postername, email, password, parentT, subject, imagename, IP, ua, posterID, sticky, locked, mta FROM  board  " & sb.ToString & " ORDER BY ID ASC"

            Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
            While query.Reader.Read
                Dim po As New WPost(query.Reader.GetInt32(0))
                po.type = CType(query.Reader.GetInt32(1), WPost.PostType)
                po.time = CDate(ConvertNoNull(query.Reader(2)))
                po.comment = CStr(ConvertNoNull(query.Reader(3)))
                po.name = CStr(ConvertNoNull(query.Reader(4)))
                po.email = CStr(ConvertNoNull(query.Reader(5)))
                po.password = CStr(ConvertNoNull(query.Reader(6)))
                po.parent = CInt(ConvertNoNull(query.Reader(7)))
                po.subject = CStr(ConvertNoNull(query.Reader(8)))
                po._imageP = CStr(ConvertNoNull(query.Reader(9)))
                po.ip = CStr(ConvertNoNull(query.Reader(10)))
                po.ua = CStr(ConvertNoNull(query.Reader(11)))
                po.posterID = CStr(ConvertNoNull(query.Reader(12)))
                If CInt(ConvertNoNull(query.Reader(13))) = 1 Then po.isSticky = True Else po.isSticky = False
                If CInt(ConvertNoNull(query.Reader(14))) = 1 Then po.locked = True Else po.locked = False
                If CInt(ConvertNoNull(query.Reader(15))) = 1 Then po.archived = True Else po.archived = False
                il.Add(po)
            End While
            query.Connection.Close()
            Return il.ToArray
        End If
    End Function

    Function GetThreadData(ByVal threadID As Integer, ByVal includeArchivedPosts As Boolean) As WPost()
        Dim il As New List(Of WPost)
        Dim queryString As String = ""
        If includeArchivedPosts Then
            queryString = "SELECT ID, type, time, comment, postername, email, password, parentT, subject, imagename, IP, ua, posterID, sticky, locked, mta FROM  board  WHERE (ID = " & threadID & ") OR (parentT = " & threadID & ") ORDER BY ID"
        Else
            queryString = "SELECT ID, type, time, comment, postername, email, password, parentT, subject, imagename, IP, ua, posterID, sticky, locked, mta FROM  board  WHERE (ID = " & threadID & ") OR (parentT = " & threadID & ") AND (mta = 0) ORDER BY ID"
        End If

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)

        While query.Reader.Read
            Dim po As New WPost(query.Reader.GetInt32(0))
            po.type = CType(query.Reader.GetInt32(1), WPost.PostType)
            po.time = CDate(ConvertNoNull(query.Reader(2)))
            po.comment = CStr(ConvertNoNull(query.Reader(3)))
            po.name = CStr(ConvertNoNull(query.Reader(4)))
            po.email = CStr(ConvertNoNull(query.Reader(5)))
            po.password = CStr(ConvertNoNull(query.Reader(6)))
            po.parent = CInt(ConvertNoNull(query.Reader(7)))
            po.subject = CStr(ConvertNoNull(query.Reader(8)))
            po._imageP = CStr(ConvertNoNull(query.Reader(9)))
            po.ip = CStr(ConvertNoNull(query.Reader(10)))
            po.ua = CStr(ConvertNoNull(query.Reader(11)))
            po.posterID = CStr(ConvertNoNull(query.Reader(12)))
            If CInt(ConvertNoNull(query.Reader(13))) = 1 Then po.isSticky = True Else po.isSticky = False
            If CInt(ConvertNoNull(query.Reader(14))) = 1 Then po.locked = True Else po.locked = False
            If CInt(ConvertNoNull(query.Reader(15))) = 1 Then po.archived = True Else po.archived = False
            il.Add(po)
        End While

        query.Connection.Close()
        Return il.ToArray
    End Function

    Function GetThreads(ByVal startIndex As Integer, ByVal count As Integer, ByVal ignoreStickies As Boolean, ByVal arhive As Boolean) As Integer()
        If Not arhive Then
            Dim ila As New List(Of Integer)

            Dim query As ChanbQuery

            If Not ignoreStickies Then
                Dim stickiesQueryStr As String = "SELECT ID FROM board  WHERE (type = 0) AND (sticky = 1) AND (mta = 0) ORDER BY ID"
                query = DatabaseEngine.ExecuteQueryReader(stickiesQueryStr)

                While query.Reader.Read
                    Try
                        ila.Add(query.Reader.GetInt32(0))
                    Catch ex As Exception
                    End Try
                End While
                query.Reader.Close()
            End If

            Dim queryString As String = "SELECT ID FROM board  WHERE (type = 0) AND (sticky = 0) AND (mta = 0) ORDER BY bumplevel DESC"

            If query Is Nothing Then query = DatabaseEngine.ExecuteQueryReader(queryString) Else query = DatabaseEngine.ExecuteQueryReader(queryString, query.Connection)

            While query.Reader.Read
                ila.Add(CInt(query.Reader.Item(0)))
            End While
            query.Reader.Close()
            query.Connection.Close()

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

            Dim queryString As String = "SELECT ID FROM board  WHERE (type = 0) AND (mta = 1) ORDER BY bumplevel DESC"

            Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)

            While query.Reader.Read
                ila.Add(CInt(query.Reader.Item(0)))
            End While
            query.Reader.Close()
            query.Connection.Close()

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
        Dim queryString As String = "SELECT password FROM mods WHERE (username LIKE '" & name & "')"
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim sqlPassMd5 As String = ""
        While query.Reader.Read
            sqlPassMd5 = query.Reader.GetString(0)
        End While
        query.Connection.Close()
        Return (MD5(password) = sqlPassMd5)
    End Function

    Function GetImageDataByMD5(ByVal md5 As String) As WPostImage
        Dim queryString As String = ""
        Select Case dbType
            Case "mssql"
                queryString = "SELECT TOP 1 imagename FROM board WHERE (imagename LIKE '%" & md5 & "%')"
            Case "mysql"
                queryString = "SELECT imagename FROM board WHERE (imagename LIKE '%" & md5 & "%') LIMIT 0,1"
            Case Else
                If isInstalled Then
                    Throw New Exception(dbTypeInvalid)
                End If
        End Select
        Dim wpi As New WPostImage
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim imageNameStr As String = ""
        While query.Reader.Read
            imageNameStr = query.Reader.GetString(0)
        End While
        If imageNameStr = "" Then
            Throw New ArgumentException("No image exist with the specified MD5.")
        Else
            Dim array As String() = imageNameStr.Split(CChar(";"))
            Dim selectedX As String = ""
            For Each x In array
                Dim p As WPostImage = GetWPostImage(x)
                If p.MD5 = md5 Then
                    wpi = p
                End If
            Next
        End If
        query.Connection.Close()
        Return wpi
    End Function

    Public Function ImageExist(ByVal md5 As String, Optional ByVal excludedPost As Integer = -1) As Boolean
        Dim queryString As String = ""
        Select Case dbType
            Case "mssql"
                If excludedPost = -1 Then
                    queryString = "SELECT TOP 1 ID FROM board WHERE (imagename LIKE '%" & md5 & "%')"
                Else
                    queryString = "SELECT TOP 1 ID FROM board WHERE (ID <> " & excludedPost & ") AND (imagename LIKE '%" & md5 & "%')"
                End If
            Case "mysql"
                If excludedPost = -1 Then
                    queryString = "SELECT ID FROM board WHERE (imagename LIKE '%" & md5 & "%') LIMIT 0,1"
                Else
                    queryString = "SELECT ID FROM board WHERE (ID <> " & excludedPost & ") AND (imagename LIKE '%" & md5 & "%') LIMIT 0,1"
                End If
            Case Else
                If isInstalled Then
                    Throw New Exception(dbTypeInvalid)
                End If
        End Select

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)

        Dim exist As Boolean = False
        While query.Reader.Read
            exist = Not (TypeOf query.Reader(0) Is DBNull)
            'If query.Reader.GetInt32(0) = 0 Then
            '    b = False
            'Else
            '    b = True
            'End If
        End While
        query.Connection.Close()
        Return exist
    End Function

    Public Sub NewMod(ByVal name As String, ByVal pas As String, Optional ByVal powers As String = "")
        If powers = "" Then
            powers = DefaultModPowers
        End If
        DatabaseEngine.ExecuteNonQuery("INSERT INTO mods (username, password, power) VALUES ('" & name & "', '" & MD5(pas) & "', '" & powers & "')")
    End Sub

    Public Function IsIPBanned(ByVal IP As String) As Boolean
        Dim queryString As String = "SELECT ID FROM bans WHERE (IP LIKE '" & IP & "')"
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim banned As Boolean = False
        While query.Reader.Read
            banned = Not (TypeOf query.Reader(0) Is DBNull)
        End While
        query.Connection.Close()
        Return banned
    End Function

    Private Function GetBanData(ByVal IP As String) As BanData
        Dim queryString As String = "SELECT ID, perm, expiry, comment, post FROM bans WHERE (IP LIKE '" & IP & "')"
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim data As New BanData
        While query.Reader.Read
            data.ID = query.Reader.GetInt32(0)
            data.PERM = CBool(query.Reader.GetInt32(1))
            data.EXPIRY = query.Reader.GetDateTime(2)
            data.COMMENT = query.Reader.GetString(3)
            data.POSTNO = query.Reader.GetInt32(4)
        End While
        data.IP = IP
        query.Reader.Close()
        query.Connection.Close()
        Return data
    End Function

    Function GetModPowers(ByVal modname As String) As String
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand

        command.CommandText = "SELECT power FROM mods WHERE (username LIKE @modname)"

        command.Parameters.Add(DatabaseEngine.MakeParameter("@modname", modname, Data.DbType.String))

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
        Dim powstr As String = ""
        While query.Reader.Read
            powstr = query.Reader.GetString(0)
        End While
        query.Connection.Close()
        Return powstr
    End Function

    Private Sub UpdatePostText(ByVal postID As Integer, ByVal newText As String, ByVal allowHTML As Boolean)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "UPDATE board SET comment = @newtext WHERE (ID = @id)"
        command.Parameters.Add(DatabaseEngine.MakeParameter("@id", postID, Data.DbType.Int32))
        If Not allowHTML Then newText = ProcessInputs(newText)
        command.Parameters.Add(DatabaseEngine.MakeParameter("@newtext", newText, Data.DbType.String))
        DatabaseEngine.ExecuteNonQuery(command)
    End Sub

    Private Sub BanPoster(ByVal IP As String, ByVal postID As Integer)
        DatabaseEngine.ExecuteNonQuery("INSERT INTO bans (perm, post, IP) VALUES (0, " & postID & ", '" & IP & "')")
    End Sub

    Private Function GetLastXPosts(ByVal threadID As Integer, ByVal x As Integer, ByVal includearhived As Boolean) As Integer()
        Dim queryString As String = ""
        Dim i As Integer = 0
        If includearhived Then i = 1
        Select Case dbType
            Case "mssql"
                queryString = "SELECT TOP " & x & " ID FROM board WHERE (parentT = " & threadID & ") AND (mta = " & i & ") ORDER BY ID DESC"
            Case "mysql"
                queryString = "SELECT ID FROM board WHERE (parentT = " & threadID & ") AND (mta = " & i & ") ORDER BY ID DESC LIMIT 0, " & CStr(x)
            Case Else
                If isInstalled Then
                    Throw New Exception(dbTypeInvalid)
                End If
        End Select
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim il As New List(Of Integer)
        While query.Reader.Read
            il.Add(CInt(query.Reader(0)))
        End While
        query.Reader.Close()
        query.Connection.Close()
        Return il.ToArray
    End Function

    Private Function GetRepliesCount(ByVal threadID As Integer, ByVal countArchived As Boolean) As ThreadReplies
        Dim s As String = ""
        If countArchived Then s = " AND ( mta = 1 )" Else s = " AND ( mta = 0 )"
        Dim textRepliesCount As String = "Select Count(ID) as T from board where (parentT=" & threadID & ") AND (imagename LIKE '')" & s
        Dim imageRepliesCount As String = "Select Count(ID) as T from board where (parentT=" & threadID & ")  AND (imagename LIKE '%.%')" & s

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(textRepliesCount)

        Dim textRC As Integer = 0
        Dim iRC As Integer = 0

        While query.Reader.Read
            textRC = CInt(query.Reader(0))
        End While

        query.Reader.Close()

        query = DatabaseEngine.ExecuteQueryReader(imageRepliesCount, query.Connection)
        While query.Reader.Read
            iRC = CInt(query.Reader(0))
        End While
        query.Reader.Close()
        query.Connection.Close()

        Dim t As New ThreadReplies
        t.TextReplies = textRC
        t.ImageReplies = iRC
        Return t
    End Function

    Public Sub ToggleSticky(ByVal threadID As Integer)
        Dim i As Integer = 0 ' 0 unsticky the thread.
        If Not IsSticky(threadID) Then i = 1 'Need to sticky it.  
        DatabaseEngine.ExecuteNonQuery("UPDATE board SET sticky = " & i & " WHERE (ID = " & threadID & " )")
    End Sub

    Private Function IsSticky(ByVal id As Integer) As Boolean
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader("SELECT sticky FROM board  WHERE (ID = " & id & " )")
        Dim p As Boolean = False
        While query.Reader.Read
            If TypeOf query.Reader(0) Is DBNull Or CInt(ConvertNoNull(query.Reader(0))) <> 1 Then
                p = False
            Else
                p = True
            End If
        End While
        query.Connection.Close()
        Return p
    End Function

    Public Sub ToggleLock(ByVal threadID As Integer)
        Dim i As Integer = 0
        If Not IsLocked(threadID) Then i = 1
        DatabaseEngine.ExecuteNonQuery("UPDATE board SET locked = " & i & " WHERE (ID = " & threadID & " )")
    End Sub

    Private Function IsLocked(ByVal id As Integer) As Boolean
        Dim queryString As String = "SELECT locked FROM board WHERE (ID = " & id & " )"
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim p As Boolean = False
        While query.Reader.Read
            If TypeOf query.Reader(0) Is DBNull Or CInt(ConvertNoNull(query.Reader(0))) <> 1 Then
                p = False
            Else
                p = True
            End If
        End While
        query.Connection.Close()
        Return p
    End Function

    Private Function IsArchived(ByVal id As Integer) As Boolean
        Dim queryString As String = "SELECT mta FROM board WHERE (ID = " & id & " )"
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim p As Boolean = False
        While query.Reader.Read
            If CInt(ConvertNoNull(query.Reader(0))) = 1 Then
                p = True
            Else
                p = False
            End If
        End While
        query.Connection.Close()
        Return p
    End Function

    Public Function GetThreadRepliesAfter(ByVal threadID As Integer, ByVal afterPost As Integer, ByVal includeArchived As Boolean) As Integer()
        Dim queryString As String
        If includeArchived Then
            queryString = "SELECT ID FROM board WHERE (parentT = " & threadID & ") AND ( ID > " & afterPost & " ) ORDER BY ID ASC"
        Else
            queryString = "SELECT ID FROM board WHERE (parentT = " & threadID & ") AND ( ID > " & afterPost & " ) AND (mta = 0) ORDER BY ID ASC"
        End If
        Dim queryObject As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim il As New List(Of Integer)
        While queryObject.Reader.Read
            il.Add(CInt(ConvertNoNull(queryObject.Reader(0))))
        End While
        queryObject.Connection.Close()
        Return il.ToArray
    End Function

    Private Sub ReportPost(ByVal id As Integer, ByVal reporterIP As String, ByVal time As Date)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "INSERT INTO reports (postID, reporterIP, time) VALUES (@id, @reporterIP, @time)"
        command.Parameters.Add(DatabaseEngine.MakeParameter("@id", id, Data.DbType.Int32))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@reporterIP", reporterIP, Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@time", time, Data.DbType.DateTime))
        DatabaseEngine.ExecuteNonQuery(command)
    End Sub

#End Region

#Region "Misc Functions"

    Private Function ConvertNoNull(ByVal x As Object) As Object
        If TypeOf x Is DBNull Then Return Nothing Else Return x
    End Function

    'Private Function EscapeChar(ByVal x As String) As String
    '    'HTML ISO 8879 Numerical Character References 
    '    'http://sunsite.berkeley.edu/amher/iso_8879.html
    '    Select Case x
    '        Case ";"
    '            Return "&#59;"
    '        Case "#"
    '            Return "&#35;"
    '        Case "&"
    '            Return "&amp;"
    '        Case "<"
    '            Return "&lt;"
    '        Case ">"
    '            Return "&gt;"
    '        Case "%"
    '            Return "&#37;"
    '        Case "$"
    '            Return "&#36;"
    '        Case "'"
    '            Return "&#39;"
    '        Case "("
    '            Return "&#40;"
    '        Case ")"
    '            Return "&#41;"
    '        Case "*"
    '            Return "&#42;"
    '        Case "+"
    '            Return "&#43;"
    '        Case "/"
    '            Return "&#47;"
    '        Case ":"
    '            Return "&#58;"
    '        Case "="
    '            Return "&#61;"
    '        Case "@"
    '            Return "&#64;"
    '        Case "["
    '            Return "&#91;"
    '        Case "]"
    '            Return "&#93;"
    '        Case "\"
    '            Return "&#92;"
    '        Case "^"
    '            Return "&#94;"
    '        Case "{"
    '            Return "&#123;"
    '        Case "}"
    '            Return "&#125;"
    '        Case "|"
    '            Return "&#124;"
    '        Case "~"
    '            Return "&#126;"
    '        Case """" ' means "
    '            Return "&quot;"
    '        Case Else
    '            Return x
    '    End Select
    'End Function

    Private Function ProcessInputs(ByVal str As String) As String
        'Dim sb As New StringBuilder
        'For Each c As Char In CType(str, Char())
        '    sb.Append(EscapeChar(c))
        'Next
        'Return sb.ToString
        Return HttpUtility.HtmlEncode(str)
    End Function

    Private Function RemoveSpecialChars(ByVal t As String) As String
        t = t.Replace(":", "")
        t = t.Replace(";", "")
        Return t
    End Function

    Private Function MD5(ByVal s As IO.Stream) As String
        Dim md5s As New System.Security.Cryptography.MD5CryptoServiceProvider
        Return ByteArrayToString(md5s.ComputeHash(s))
    End Function

    Function MD5(ByVal s As String) As String
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

    Private Function IsXvalidQuote(ByVal x As String) As Boolean
        Dim b As Boolean = False
        ' A valid quote should be in >>Integer format.
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

    Public Function FormatSizeString(ByVal size As Long) As String
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

    Private Function GenerateUID(ByVal parentPost As Integer, ByVal IP As String) As String
        Return New String(CType(MD5(CStr(parentPost) & IP), Char()), 0, 8)
    End Function

    Private Function GetTimeString(ByVal d As Date) As String
        'ISO 8601 date time format
        Return d.Year & "-" & d.Month & "-" & d.Day & " " & d.Hour & ":" & d.Minute & ":" & d.Second
    End Function

#End Region

#Region "Image processing"

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

#End Region

#Region "BB codes processing"

    Private Function MatchAndProcessBBCodes(ByVal codename As String, ByVal data As String) As String
        Select Case codename
            Case "spoiler"
                If data.Contains("[spoiler]") And data.Contains("[/spoiler]") Then
                    Dim matches As String() = MatchBBCode(data, "spoiler")
                    For Each x In MatchBBCode(data, "spoiler")
                        data = data.Replace("[spoiler]" & x & "[/spoiler]", "<s>" & x & "</s>")
                    Next
                    Return data
                Else
                    Return data
                End If
            Case "code"
                If data.Contains("[code]") And data.Contains("[/code]") Then
                    Try
                        Dim colorizer As New ColorCode.CodeColorizer()
                        For Each x In MatchBBCode(data, "code")
                            Dim codeStr As String = HttpUtility.HtmlDecode(x)
                            codeStr = codeStr.Replace("<br>", String.Empty)

                            Dim codeLang As String = GetCodeLang(codeStr)
                            If Not (codeLang = "") Then
                                codeStr = codeStr.Replace("[lang]" & codeLang & "[/lang]", String.Empty)
                            End If
                            data = data.Replace("[code]" & x & "[/code]", colorizer.Colorize(codeStr, GetCCLI(codeLang)))
                        Next
                        Return data
                    Catch ex As Exception
                        Return data
                    End Try
                Else
                    Return data
                End If
            Case "md"
                If data.Contains("[md]") And data.Contains("[/md]") Then
                    Dim md As New MarkdownSharp.Markdown
                    For Each x In MatchBBCode(data, "md")
                        Dim mdt As String = x
                        mdt = mdt.Replace("<br>", String.Empty)
                        data = data.Replace("[md]" & x & "[/md]", md.Transform(mdt))
                    Next
                    Return data
                Else
                    Return data
                End If
                'Case "latex"
                '    For Each x In MatchBBCode(data, "latex")
                '        Dim tempfile As String = STORAGEFOLDER & "\temp" & Now.ToFileTime & ".tex"
                '        Dim tempfileOUT As String = tempfile & "OUT"
                '        Dim mdt As String = x
                '        mdt = RemoveHTMLEscapes(mdt)
                '        mdt = mdt.Replace("<br>", "")
                '        IO.File.WriteAllText(tempfile, x)
                '        TexDotNet.TexUtilities.CreateExpressionTree(x)
                '        Dim rend As New TexDotNet.
                '        data = data.Replace(x, "")
                '    Next
                '    data = data.Replace("[latex]", String.Empty)
                '    data = data.Replace("[/latex]", String.Empty)
                '    Return data
            Case Else
                Return data
        End Select
    End Function

    ''' <summary>
    ''' Get a list of all matches for a given bb code.
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="bbcode"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function MatchBBCode(ByVal text As String, ByVal bbcode As String) As String()
        Dim il As New List(Of String)
        Dim regSTR As String = "\[/?" & bbcode & "\]"
        Dim st As String() = Regex.Split(text, regSTR)
        Dim isAmatch As Boolean = False
        For i As Integer = 0 To st.Length - 1 Step 1
            If Not isAmatch Then
                'Not a match 
            Else
                il.Add(st.ElementAt(i))
            End If
            isAmatch = Not isAmatch
        Next
        Return il.ToArray
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

#Region "Static Mode HTML files updaters."

    Sub UpdateThreadHtml(ByVal tid As Integer)
        If IsTIDLOCKED(tid) Then
            QueueReupdate(tid)
        Else
            Dim indexHTMLpath As String = ThreadStorageFolder & "\" & tid & ".html"

            ClearSignaledUpdates(tid)
            LockTID(tid)

            IO.File.WriteAllText(indexHTMLpath, GeneratePageHTMLStatic(tid))

            If CheckForSingaledUpdates(tid) = True Then
                ClearSignaledUpdates(tid)
                UpdateThreadHtml(tid)
            End If

            UnlockTID(tid)
        End If
    End Sub

    Sub UpdateThreadHtmlArchive(ByVal tid As Integer)
        Dim indexHTMLpath As String = ArchivedTStorageFolder & "\" & tid & ".html"
        IO.File.WriteAllText(indexHTMLpath, GeneratePageHTMLStatic(tid))
    End Sub

    Private Sub LockTID(ByVal tid As Integer)
        DatabaseEngine.ExecuteNonQuery("INSERT INTO lockedT (locked) VALUES (" & tid & ")")
    End Sub

    Private Sub UnlockTID(ByVal tid As Integer)
        DatabaseEngine.ExecuteNonQuery("DELETE FROM lockedT WHERE (locked = " & tid & ")")
    End Sub

    Private Sub QueueReupdate(ByVal tid As Integer)
        DatabaseEngine.ExecuteNonQuery("INSERT INTO ioqueue (tid) VALUES (" & tid & ")")
    End Sub

    Private Sub ClearSignaledUpdates(ByVal tid As Integer)
        DatabaseEngine.ExecuteNonQuery("DELETE FROM ioqueue WHERE (tid = " & tid & ")")
    End Sub

    Private Function CheckForSingaledUpdates(ByVal tid As Integer) As Boolean
        Dim queryStr As String = ""
        Select Case dbType
            Case "mssql"
                queryStr = "SELECT TOP 1 * FROM ioqueue WHERE (tid = " & tid & ")"
            Case "mysql"
                queryStr = "SELECT * FROM ioqueue WHERE (tid = " & tid & ") LIMIT 0,1"
            Case Else
                If isInstalled Then
                    Throw New Exception(dbTypeInvalid)
                Else
                    Return Nothing
                End If
        End Select
        Dim queryOjb As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryStr)
        Dim b As Boolean = False
        While queryOjb.Reader.Read
            If TypeOf queryOjb.Reader(0) Is DBNull Then b = False Else b = True
        End While
        queryOjb.Connection.Close()
        Return b
    End Function

    Private Function IsTIDLOCKED(ByVal tid As Integer) As Boolean
        Dim queryStr As String = ""
        Select Case dbType
            Case "mssql"
                queryStr = "SELECT TOP 1 * FROM lockedT WHERE (locked = " & tid & ")"
            Case "mysql"
                queryStr = "SELECT * FROM lockedT WHERE (locked = " & tid & ") LIMIT 0,1"
            Case Else
                If isInstalled Then
                    Throw New Exception(dbTypeInvalid)
                Else
                    Return Nothing
                End If
        End Select
        Dim queryOjb As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryStr)
        Dim b As Boolean = False
        While queryOjb.Reader.Read
            If TypeOf queryOjb.Reader(0) Is DBNull Then b = False Else b = True
        End While
        queryOjb.Connection.Close()
        Return b
    End Function

    Public Function GeneratePageHTMLStatic(ByVal tid As Integer) As String
        Dim pageHTML As String = GenerateGenericHTML()

        tid = Math.Abs(tid)
        Dim po As WPost = FetchPostData(tid)

        Dim pageHandlerLink As String = "default"
        If po.archived Then pageHandlerLink = "archive"

        If po.archived Then
            pageHTML = pageHTML.Replace("%POSTING MODE%", archiveNotice)
        Else
            pageHTML = pageHTML.Replace("%POSTING MODE%", postingModeReplyHtml)
        End If


        If po.archived Then
            pageHTML = pageHTML.Replace("%POSTDIVCLASS%", "hide")
        Else
            pageHTML = pageHTML.Replace("%POSTDIVCLASS%", "")
        End If

        If po.archived = False Then
            pageHTML = pageHTML.Replace("%POST FORM MODE%", "reply")
        Else
            pageHTML = pageHTML.Replace("%POST FORM MODE%", "thread")
        End If

        pageHTML = pageHTML.Replace("%POST FORM TID%", CStr(tid))

        pageHTML = pageHTML.Replace("%POST FORM BUTTON%", replyStr)

        If EnableCaptcha And Not po.archived Then
            pageHTML = pageHTML.Replace("%CAPTCHA PHOLDER%", captchaTableEntryHtml)
        Else
            pageHTML = pageHTML.Replace("%CAPTCHA PHOLDER%", "")
        End If

        pageHTML = pageHTML.Replace("%ADD NEW FILES PHOLDER%", addNewFileButtonHTML)

        pageHTML = pageHTML.Replace("%MAXIMUM FILE SIZE%", FormatSizeString(MaximumFileSize))
        pageHTML = pageHTML.Replace("%SESSION PASSWORD%", "")

        If po.archived Then
            pageHTML = pageHTML.Replace("%POSTING RULES%", "")
        Else
            pageHTML = pageHTML.Replace("%POSTING RULES%", postingRulesHTML)
        End If

        pageHTML = pageHTML.Replace("%THREAD COUNT%", "")

        pageHTML = pageHTML.Replace("%META NO CACHE%", "<META HTTP-EQUIV='pragma' CONTENT='no-cache'>")

        pageHTML = pageHTML.Replace("%RETURN BUTTON DESKTOP%", DesktopReturnButtonHTML.Replace("%P%", WebRoot & pageHandlerLink & ".aspx"))
        pageHTML = pageHTML.Replace("%RETURN BUTTON MOBILE%", MobileReturnButtonHTML.Replace("%P%", WebRoot & pageHandlerLink & ".aspx"))

        '####################################### BODY PROCESSING CODE #######################################
        Dim body As New StringBuilder
        Dim para As New HTMLParameters()

        para.isCurrentThread = Not po.archived

        body.Append("<script type='text/javascript'> timer();</script>")

        para.replyButton = False
        para.isTrailPost = False

        body.Append("<div class='thread' id='t" & tid & "'>")
        body.Append(GetThreadHTML(tid, para))
        body.Append("</div><hr ></hr>")

        pageHTML = pageHTML.Replace("%BODY%", body.ToString)

        pageHTML = pageHTML.Replace("%PAGES LIST%", "")

        Return pageHTML
    End Function

#End Region

End Module
