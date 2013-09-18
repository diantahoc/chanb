Imports System.Data.Common

' Enable PDF Thumbnail generation
' Require TallComponent PDFRasterizer
#Const EnablePDF = True

Friend Module GlobalFunctions

    Friend dbi As New DBInitializer()
    Public CFH_Plugins As CustomFileHandler() = GetCFHPlugins()

#Region "Board Functions"

    Function GetSessionPassword(ByVal cookies As Web.HttpCookieCollection, ByVal session As Web.SessionState.HttpSessionState) As String
        If cookies("posterpass") Is Nothing Then
            Dim c As New HttpCookie("posterpass", New String(CType(session.SessionID, Char()), 0, 15))
            c.Expires = (Now + New TimeSpan(0, 60, 0))
            cookies.Add(c)
            Return cookies("posterpass").Value
        Else
            Return cookies("posterpass").Value
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

    Private Sub SaveAllFilesToSinglePost(ByVal li As List(Of HttpPostedFile), ByVal postId As Integer)
        Dim connection As DbConnection = DatabaseEngine.GetDBConnection
        connection.Open()
        For Each file As HttpPostedFile In li
            SavePostFile(file, True, postId, connection)
        Next
        connection.Close()
    End Sub

    Private Sub SavePostFile(ByVal f As HttpPostedFile, ByVal isReply As Boolean, ByVal postId As Integer, Optional ByVal Connection As DbConnection = Nothing)
        'TODO: Fix this possible bug :Images are replaced if "it has to do with the way moot names images with the UNIX timestamp and only happens if someone uploads an image at the same time as you down to the exact second".
        'Possible bug fix 

        'If FileIO.FileSystem.FileExists(p) Then
        '    Do Until (Not FileIO.FileSystem.FileExists(p))
        '        dd = CStr(Date.UtcNow.ToFileTime())
        '        p = StorageFolder & "\" & dd & "." & fileextension
        '    Loop
        '    f.SaveAs(p)
        'Else
        '    f.SaveAs(p)
        'End If
        ' Or use a random number in addition to the unix time stamp.
        ' This increase the number of collosion posibilities but does not get rid of the problem.



        Dim rand As New Random
        
        Dim fileextension As String = f.FileName.Split(CChar("."))(f.FileName.Split(CChar(".")).Length - 1).ToLower()

        Select Case fileextension.ToUpper()
            Case "JPG", "JPEG", "PNG", "BMP", "GIF", "APNG"

                Dim w As Drawing.Image

                Try
                    'Check if the file is a valid image
                    w = Drawing.Image.FromStream(f.InputStream)
                    'If RemoveEXIFData Then
                    '    w = RemoveEXIF(w)
                    'End If
                Catch ex As Exception
                    'No image is required when replying
                    If f.ContentLength = 0 And isReply Then
                        Exit Sub
                    Else
                        Throw New ArgumentException(BadOrNoImage)
                    End If
                End Try

                Dim ChanbName As String = CStr(Date.UtcNow.ToFileTime) & "r" & CStr(rand.Next(0, 1024))

                'Full image path
                Dim FullImagePath As String = StorageFolder & "\" & ChanbName & "." & fileextension

                'Save the image.
                f.SaveAs(FullImagePath)

                ' f.InputStream.Seek(0, IO.SeekOrigin.Begin)
                Dim fs As New IO.FileStream(FullImagePath, IO.FileMode.Open)
                Dim md5string As String = MD5(fs)
                fs.Close()

                Dim thumb As String

                Dim isTransparant As Boolean = ImageHaveTransparentBits(CType(w, Drawing.Bitmap))
                Dim format As System.Drawing.Imaging.ImageFormat

                If isTransparant Then
                    format = System.Drawing.Imaging.ImageFormat.Png
                    thumb = StorageFolderThumbs & "\th" & ChanbName & ".png"
                Else
                    format = System.Drawing.Imaging.ImageFormat.Jpeg
                    thumb = StorageFolderThumbs & "\th" & ChanbName & ".jpg"
                End If


                'Check if resize is needed.
                If (w.Width * w.Height) < 62500 Then
                    w.Save(thumb, format)
                Else
                    ResizeImage(w, 250).Save(thumb, format)
                End If


                If (Not AllowDuplicatesFiles) And FileExistInDB(md5string, Connection) Then
                    'If image already exist, we fetch the matching image data from the database, and delete the saved files.
                    If SmartLinkDuplicateImages = False Then
                        Delete_File_Fromdisk(FullImagePath)
                        Delete_File_Fromdisk(thumb)
                        Throw New ArgumentException(duplicateFile)
                    Else
                        Dim wpi As WPostImage = GetFileDataByMD5(md5string, Connection)
                        'Delete previously saved files.
                        Delete_File_Fromdisk(FullImagePath)
                        Delete_File_Fromdisk(thumb)
                        'Change the necessary variables
                        wpi.PostID = postId
                        wpi.RealName = f.FileName

                        AddFileToDatabase(wpi, postId, Connection)
                    End If
                Else


                    Dim wpi As New WPostImage
                    wpi.ChanbName = ChanbName & "." & fileextension
                    wpi.Size = f.ContentLength
                    wpi.Dimensions = w.Size.Width & "x" & w.Size.Height
                    wpi.Extension = fileextension.ToUpper
                    wpi.RealName = f.FileName
                    wpi.MD5 = md5string
                    wpi.PostID = postId
                    wpi.MimeType = GetMimeType(fileextension)
                    AddFileToDatabase(wpi, postId, Connection)

                    w.Dispose()
                End If

            Case "SVG"
                Dim dd As String = CStr(Date.UtcNow.ToFileTime) & "r" & CStr(rand.Next(0, 1024))
                Dim p As String = StorageFolder & "\" & dd & "." & fileextension

                f.SaveAs(p)

                Dim fs As New IO.FileStream(p, IO.FileMode.Open)
                Dim md5string As String = MD5(fs)
                fs.Close()



                If (Not AllowDuplicatesFiles) And FileExistInDB(md5string, Connection) Then

                    If SmartLinkDuplicateImages = False Then
                        Delete_File_Fromdisk(p)
                        Throw New ArgumentException(duplicateFile)
                    Else
                        Dim wpi As WPostImage = GetFileDataByMD5(md5string, Connection)
                        Delete_File_Fromdisk(p)
                        wpi.RealName = f.FileName
                        wpi.PostID = postId
                        AddFileToDatabase(wpi, postId, Connection)

                    End If

                Else

                    Dim svgBi As Drawing.Bitmap

                    'Thumb path
                    Dim thumb As String = StorageFolderThumbs & "\th" & dd & ".png"

                    Try
                        Dim svgDoc As Svg.SvgDocument = Svg.SvgDocument.Open(p)
                        svgBi = svgDoc.Draw()
                    Catch ex As Exception
                        svgBi = New Drawing.Bitmap(150, 30)
                        Dim g As Drawing.Graphics = Drawing.Graphics.FromImage(svgBi)
                        g.Clear(Drawing.Color.White)
                        g.DrawString("SVG", New Drawing.Font(Drawing.FontFamily.GenericMonospace, 20, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Pixel), Drawing.Brushes.Black, 0, 0)
                        g.Dispose()
                    End Try

                    Dim isTransparant As Boolean = ImageHaveTransparentBits(svgBi)
                    Dim format As System.Drawing.Imaging.ImageFormat

                    If isTransparant Then
                        format = System.Drawing.Imaging.ImageFormat.Png
                        thumb = StorageFolderThumbs & "\th" & dd & ".png"
                    Else
                        format = System.Drawing.Imaging.ImageFormat.Jpeg
                        thumb = StorageFolderThumbs & "\th" & dd & ".jpg"
                    End If

                    If (svgBi.Width * svgBi.Height) < 62500 Then
                        svgBi.Save(thumb, format)
                    Else
                        ResizeImage(svgBi, 250).Save(thumb, format)
                    End If

                    Dim wpi As New WPostImage
                    wpi.ChanbName = dd & "." & fileextension
                    wpi.Size = f.ContentLength
                    wpi.Dimensions = svgBi.Size.Width & "x" & svgBi.Size.Height
                    wpi.Extension = fileextension.ToUpper
                    wpi.RealName = f.FileName
                    wpi.MD5 = md5string
                    wpi.PostID = postId
                    wpi.MimeType = GetMimeType(fileextension)
                    svgBi.Dispose()

                    AddFileToDatabase(wpi, postId, Connection)

                End If


            Case "PDF"

#If EnablePDF Then
                Dim dd As String = CStr(Date.UtcNow.ToFileTime) & "r" & CStr(rand.Next(0, 1024))
                Dim p As String = StorageFolder & "\" & dd & "." & fileextension
                'Thumb path
                Dim thumb As String = StorageFolderThumbs & "\th" & dd & ".jpg"
                f.SaveAs(p)

                Dim fs As New IO.FileStream(p, IO.FileMode.Open)
                Dim md5string As String = MD5(fs)
                fs.Close()



                If (Not AllowDuplicatesFiles) And FileExistInDB(md5string, Connection) Then
                    If SmartLinkDuplicateImages = False Then
                        Delete_File_Fromdisk(p)
                        Delete_File_Fromdisk(thumb)
                        Throw New ArgumentException(duplicateFile)
                    Else
                        Dim wpi As WPostImage = GetFileDataByMD5(md5string, Connection)
                        wpi.RealName = f.FileName
                        wpi.PostID = postId
                        AddFileToDatabase(wpi, postId, Connection)
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

                    Dim wpi As New WPostImage
                    wpi.ChanbName = dd & "." & fileextension
                    wpi.Size = f.ContentLength
                    wpi.Dimensions = pdfBi.Size.Width & "x" & pdfBi.Size.Height
                    wpi.Extension = fileextension.ToUpper
                    wpi.RealName = f.FileName
                    wpi.MD5 = md5string
                    wpi.PostID = postId
                    wpi.MimeType = GetMimeType(fileextension)
                    pdfBi.Dispose()

                    AddFileToDatabase(wpi, postId, Connection)

                End If

#Else
                    Dim dd As String = CStr(Date.UtcNow.ToFileTime) & "r" & CStr(rand.Next(0, 1024))
                    Dim p As String = StorageFolder & "\" & dd & "." & fileextension
                    'Thumb path
                    Dim thumb As String = StorageFolderThumbs & "\th" & dd & ".jpg"
                    f.SaveAs(p)

                    Dim fs As New IO.FileStream(p, IO.FileMode.Open)
                    Dim md5string As String = MD5(fs)
                    fs.Close()


                    If (Not AllowDuplicatesFiles) And FileExistInDB(md5string, Connection)  Then
                        If SmartLinkDuplicateImages = False Then
                           Delete_File_Fromdisk(p)
                           Delete_File_Fromdisk(thumb)
                            Throw New ArgumentException(duplicateFile)
                        Else
                            Dim wpi As WPostImage = GetFileDataByMD5(md5string, Connection)
                            FileIO.FileSystem.DeleteFile(p)

                            wpi.RealName = f.FileName
                            wpi.PostID = postId
                            AddFileToDatabase(wpi, postId, Connection)
                        End If
                    Else

                        Dim pdfBi As Drawing.Bitmap = New Drawing.Bitmap(150, 30)


                        Dim graph As Drawing.Graphics = Drawing.Graphics.FromImage(pdfBi)

                        graph.Clear(Drawing.Color.White)
                        graph.DrawString("PDF", New Drawing.Font(Drawing.FontFamily.GenericMonospace, 20, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Pixel), Drawing.Brushes.Black, 0, 0)

                        graph.Dispose()


                        If (pdfBi.Width * pdfBi.Height) < 62500 Then
                            pdfBi.Save(thumb, Drawing.Imaging.ImageFormat.Jpeg)
                        Else
                            ResizeImage(pdfBi, 250).Save(thumb, Drawing.Imaging.ImageFormat.Jpeg)
                        End If

                        Dim wpi As New WPostImage
                        wpi.ChanbName = dd & "." & fileextension
                        wpi.Size = f.ContentLength
                        wpi.Dimensions = pdfBi.Size.Width & "x" & pdfBi.Size.Height
                        wpi.Extension = fileextension.ToUpper
                        wpi.RealName = f.FileName
                        wpi.MD5 = md5string
                        wpi.PostID = postId
                        wpi.MimeType = GetMimeType(fileextension)
                        pdfBi.Dispose()

                        AddFileToDatabase(wpi, postId, Connection)

                    End If

#End If

            Case "WEBM"

                If Not Validators.ValidateWEBM(f.InputStream) Then
                    Throw New Exception("Invalid WEBM file")
                End If

                Dim dd As String = CStr(Date.UtcNow.ToFileTime) & "r" & CStr(rand.Next(0, 1024))
                Dim p As String = StorageFolder & "\" & dd & "." & fileextension
                f.SaveAs(p)

                Dim fs As New IO.FileStream(p, IO.FileMode.Open)
                Dim md5string As String = MD5(fs)


                fs.Close()

                If (Not AllowDuplicatesFiles) And FileExistInDB(md5string, Connection) Then
                    If SmartLinkDuplicateImages = False Then
                        Delete_File_Fromdisk(p)
                        Throw New ArgumentException(duplicateFile)
                    Else
                        Dim wpi As WPostImage = GetFileDataByMD5(md5string, Connection)
                        Delete_File_Fromdisk(p)
                        wpi.RealName = f.FileName
                        wpi.PostID = postId
                        AddFileToDatabase(wpi, postId, Connection)
                    End If
                Else

                    Dim wpi As New WPostImage
                    wpi.ChanbName = dd & "." & fileextension
                    wpi.Size = f.ContentLength
                    wpi.Dimensions = "video"
                    wpi.Extension = fileextension.ToUpper
                    wpi.RealName = f.FileName
                    wpi.MD5 = md5string
                    wpi.PostID = postId
                    wpi.MimeType = GetMimeType(fileextension)
                    AddFileToDatabase(wpi, postId, Connection)

                End If

            Case "MP3", "OGG"

                Select Case fileextension
                    Case "MP3"
                        If Not Validators.ValidateMP3(f.InputStream) Then
                            Throw New Exception("Invalid MP3 file")
                        End If
                    Case "OGG"
                        If Not Validators.ValidateOGG(f.InputStream) Then
                            Throw New Exception("Invalid OGG file")
                        End If
                End Select

                Dim dd As String = CStr(Date.UtcNow.ToFileTime) & "r" & CStr(rand.Next(0, 1024))
                Dim p As String = StorageFolder & "\" & dd & "." & fileextension
                f.SaveAs(p)
                Dim fs As New IO.FileStream(p, IO.FileMode.Open)
                Dim md5string As String = MD5(fs)
                fs.Close()

                If (Not AllowDuplicatesFiles) And FileExistInDB(md5string, Connection) Then
                    If SmartLinkDuplicateImages = False Then
                        Delete_File_Fromdisk(p)
                        Throw New ArgumentException(duplicateFile)
                    Else
                        Dim wpi As WPostImage = GetFileDataByMD5(md5string, Connection)
                        Delete_File_Fromdisk(p)
                        wpi.RealName = f.FileName
                        wpi.PostID = postId
                        AddFileToDatabase(wpi, postId, Connection)
                    End If
                Else

                    Dim wpi As New WPostImage
                    wpi.ChanbName = dd & "." & fileextension
                    wpi.Size = f.ContentLength
                    wpi.Dimensions = "audio"
                    wpi.Extension = fileextension.ToUpper
                    wpi.RealName = f.FileName
                    wpi.MD5 = md5string
                    wpi.PostID = postId
                    wpi.MimeType = GetMimeType(fileextension)
                    AddFileToDatabase(wpi, postId, Connection)

                End If
            Case "" ' A case of "" may occure when no file is uploaded. Simply return nothing.
                Return
            Case Else
                Dim fileHandled As Boolean = False
                For i As Integer = 0 To CFH_Plugins.Count - 1 Step 1
                    Dim cfh As CustomFileHandler = CFH_Plugins(i)
                    If cfh.Is_FileSupported(fileextension) Then
                        fileHandled = True
                        Dim dd As String = CStr(Date.UtcNow.ToFileTime) & "r" & CStr(rand.Next(0, 1024))
                        Dim p As String = StorageFolder & "\" & dd & "." & fileextension
                        f.SaveAs(p)


                        'Let's check the thumbnail

                        Dim thumb As String = ""

                        Dim thumbData As CFHThumbData = cfh.GetHTTPFileThumb(f.InputStream)

                        If thumbData.thumbRequired Then

                            Dim isTransparant As Boolean = ImageHaveTransparentBits(CType(thumbData.thumbData, Drawing.Bitmap))

                            Dim format As Drawing.Imaging.ImageFormat

                            If isTransparant Then
                                thumb = StorageFolderThumbs & "\th" & dd & ".png"
                                format = Drawing.Imaging.ImageFormat.Png
                            Else
                                thumb = StorageFolderThumbs & "\th" & dd & ".jpg"
                                format = Drawing.Imaging.ImageFormat.Jpeg
                            End If

                            If (thumbData.thumbData.Width * thumbData.thumbData.Height) < 62500 Then
                                thumbData.thumbData.Save(thumb, format)
                            Else
                                ResizeImage(thumbData.thumbData, 250).Save(thumb, format)
                            End If
                        End If

                        Dim fs As New IO.FileStream(p, IO.FileMode.Open)
                        Dim md5string As String = MD5(fs)
                        fs.Close()

                        If (Not AllowDuplicatesFiles) And FileExistInDB(md5string, Connection) Then
                            If SmartLinkDuplicateImages = False Then

                                Delete_File_Fromdisk(p)
                                If thumbData.thumbRequired Then Delete_File_Fromdisk(thumb)

                                Throw New ArgumentException(duplicateFile)
                            Else
                                Dim wpi As WPostImage = GetFileDataByMD5(md5string, Connection)
                                Delete_File_Fromdisk(p)
                                If thumbData.thumbRequired Then Delete_File_Fromdisk(thumb)
                                wpi.RealName = f.FileName
                                wpi.PostID = postId
                                AddFileToDatabase(wpi, postId, Connection)
                            End If
                        Else

                            Dim wpi As New WPostImage
                            wpi.ChanbName = dd & "." & fileextension
                            wpi.Size = f.ContentLength
                            wpi.Dimensions = thumbData.thumbDimensions
                            wpi.Extension = fileextension.ToUpper
                            wpi.RealName = f.FileName
                            wpi.MD5 = md5string
                            wpi.PostID = postId
                            wpi.MimeType = thumbData.mimeType
                            AddFileToDatabase(wpi, postId, Connection)

                        End If

                        Exit For
                    End If
                Next
                If Not fileHandled Then Throw New Exception("Unsupported file type")
        End Select
    End Sub

    ''' <summary>
    ''' Check if the image have transparent pixels.
    ''' </summary>
    ''' <param name="image"></param>
    ''' <returns></returns>
    Private Function ImageHaveTransparentBits(ByVal image As Drawing.Bitmap) As Boolean
        Return ChanbCore.ImageManipulation.ImageHaveTransparentBits(image)
    End Function

    Private Function GetCFHPlugins() As CustomFileHandler()
        Dim ila As New List(Of CustomFileHandler)
        For Each x As IO.FileInfo In FileIO.FileSystem.GetDirectoryInfo(chanb.My.Request.PhysicalApplicationPath & "\bin").GetFiles("*.dll", IO.SearchOption.TopDirectoryOnly)
            If Not (x.Name = "chanb.dll") Then
                Try
                    Dim assembly As Reflection.Assembly = Reflection.Assembly.LoadFrom(x.FullName)

                    Dim type As Type
                    Dim found As Boolean = False

                    For Each type In assembly.GetTypes()
                        If GetType(CustomFileHandler).IsAssignableFrom(type) Then
                            found = True
                            Exit For
                        End If
                    Next

                    If found Then
                        Dim instance As CustomFileHandler
                        instance = CType(Activator.CreateInstance(type), CustomFileHandler)
                        ila.Add(instance)
                    End If
                Catch ex As Exception
                End Try
            End If
        Next
        Return ila.ToArray
    End Function

    ' This function save progressive thumbnails.
    'Private Sub SaveThumbnail(ByVal chanbName As String, ByVal i As Drawing.Image, ByVal fileextension As String)

    '    If fileextension = "png" Then
    '        'Save thumnail as png
    '        Dim thumb As String = StorageFolderThumbs & "\th" & chanbName & ".png"

    '        Dim codec As Drawing.Imaging.ImageCodecInfo = Drawing.Imaging.ImageCodecInfo.GetImageEncoders().First(Function(c) c.MimeType = "image/png")

    '        Dim parameters As New Drawing.Imaging.EncoderParameters(3)
    '        parameters.Param(0) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L)
    '        parameters.Param(1) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.ScanMethod, CInt(Fix(Drawing.Imaging.EncoderValue.ScanMethodInterlaced)))
    '        parameters.Param(2) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.RenderMethod, CInt(Fix(Drawing.Imaging.EncoderValue.RenderProgressive)))

    '        Dim bi As Drawing.Image = ResizeImage(i, 250)
    '        bi.Save(thumb, codec, parameters)
    '        bi.Dispose()
    '        parameters.Dispose()
    '    Else
    '        Dim thumb As String = StorageFolderThumbs & "\th" & chanbName & ".jpg"
    '        Dim codec As Drawing.Imaging.ImageCodecInfo = Drawing.Imaging.ImageCodecInfo.GetImageEncoders().First(Function(c) c.MimeType = "image/jpeg")
    '        Dim parameters As New Drawing.Imaging.EncoderParameters(3)
    '        parameters.Param(0) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L)
    '        parameters.Param(1) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.ScanMethod, CInt(Fix(Drawing.Imaging.EncoderValue.ScanMethodInterlaced)))
    '        parameters.Param(2) = New Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.RenderMethod, CInt(Fix(Drawing.Imaging.EncoderValue.RenderProgressive)))

    '        Dim bi As Drawing.Image = ResizeImage(i, 250)
    '        bi.Save(thumb, codec, parameters)
    '        bi.Dispose()
    '        parameters.Dispose()
    '    End If

    'End Sub

    'Private Function RemoveEXIF(ByVal i As Drawing.Image) As Drawing.Image
    '    Dim mem As New IO.MemoryStream
    '    i.Save(mem, Drawing.Imaging.ImageFormat.Bmp)
    '    Dim bi As Drawing.Image = Drawing.Image.FromStream(mem)
    '    bi.Save(mem, i.RawFormat)
    '    Return Drawing.Image.FromStream(mem)
    'End Function

    Private Function FileHasThumb(ByVal fileExtension As String) As Boolean
        Dim supportedImages As String() = {"jpg", "jpeg", "png", "bmp", "gif", "apng", "svg", "pdf"}
        Dim bo As Boolean = False
        For Each x In supportedImages
            If fileExtension.ToLower() = x Then
                bo = True
            End If
        Next

        If Not bo Then
            'everything has failed. Let's check the custom files 
            For i As Integer = 0 To CFH_Plugins.Length - 1 Step 1
                If CFH_Plugins(i).FileHasThumb(fileExtension.ToLower()) Then bo = True
            Next
        End If

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

                If ShowThreadRepliesCount Then
                    Dim replyC As ThreadReplies = GetRepliesCount(po.PostID, Not pa.isCurrentThread)
                    If replyC.TotalReplies > 0 Then postHTML = postHTML.Replace("%REPLY COUNT SPAN%", replyCountSpan.Replace("%REPLY COUNT%", CStr(replyC.TotalReplies))) Else postHTML = postHTML.Replace("%REPLY COUNT SPAN%", String.Empty)
                Else
                    postHTML = postHTML.Replace("%REPLY COUNT SPAN%", String.Empty)
                End If

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

        postHTML = postHTML.Replace("%LANG reportStr%", reportStr)
        postHTML = postHTML.Replace("%LANG deleteStr%", deleteStr)
        postHTML = postHTML.Replace("%ROOT%", WebRoot)

        Dim parentPoID As Integer = po.PostID
        If po.type = WPost.PostType.Reply Then parentPoID = po.parent

        If StaticHTML Then
            postHTML = postHTML.Replace("%POST LINK%", ThreadHTMLWebPath & parentPoID & ".html#p" & po.PostID)
        Else
            If po.archived Then
                If ConvertArchivedThreadToHTML Then
                    postHTML = postHTML.Replace("%POST LINK%", ArchivedTHTMLWebPath & parentPoID & ".html#p" & po.PostID)
                Else
                    postHTML = postHTML.Replace("%POST LINK%", pageHandlerName & ".aspx?id=" & parentPoID & "#p" & po.PostID)
                End If
            Else
                postHTML = postHTML.Replace("%POST LINK%", pageHandlerName & ".aspx?id=" & parentPoID & "#p" & po.PostID)
            End If
        End If

        postHTML = postHTML.Replace("%IMAGES%", GetFilesHTML(po))

        If pa.isModerator Or pa.isAdmin Then postHTML = postHTML.Replace("%MODPANEL%", GetModeratorHTMLMenu(pa.CredPowers).Replace("%ID%", CStr(po.PostID))) Else postHTML = postHTML.Replace("%MODPANEL%", "")

        ''Post text  
        Dim commentShortened As Boolean = False
        If pa.isTrailPost And po.comment.Length > 1500 Then
            po.comment = po.comment.Remove(1500)
            commentShortened = True
        End If

        Dim cm As String = ProcessComment(po, pageHandlerName, pa)
        If pa.isTrailPost Then
            If commentShortened Then
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

    Public Function ProcessPost(ByVal Context As HttpContext) As String
        Dim message As String = ""

        Dim Session As HttpSessionState = Context.Session
        Dim Request As HttpRequest = Context.Request

        Dim mode As String = Request.Item("mode")

        Dim isAdmin As Boolean = False
        If Not GetCookie(Context, "admin") = "" Then isAdmin = CBool(GetCookie(Context, "admin"))

        Dim isMod As Boolean = False
        If Not GetCookie(Context, "mod") = "" Then isMod = CBool(GetCookie(Context, "mod"))

        Dim cont As Boolean = True
        'Flood detection check
        If session.Item("lastpost") Is "" Or session.Item("lastpost") Is Nothing Then
            session.Item("lastpost") = Now.ToString
        Else
            Dim i As Date = Date.Parse(CStr(session.Item("lastpost")))
            If CInt((Now - i).TotalSeconds) <= TimeBetweenRequestes And (mode = "thread" Or mode = "reply") And Not (isAdmin) Then
                message = FormatHTMLMessage("Error", FloodDetected.Replace("%", CStr(TimeBetweenRequestes)), "", "8888", True)
                cont = False
            Else
                session.Item("lastpost") = Now.ToString
            End If
        End If

        Dim sessionCaptcha As String = GetCookie(Context, "captcha")

        'Captcha check. Administrator does not need to enter captcha
        If EnableCaptcha And (mode = "thread" Or mode = "reply") And (Not isAdmin) Then
            If Not sessionCaptcha = "" Then
                If Not sessionCaptcha = Request.Item("usercaptcha") Then
                    message = FormatHTMLMessage(errorStr, wrongCaptcha, "default.aspx", "77", True)
                    cont = False
                Else
                    'Captcha is correct
                    'we clear the cookie to prevent using the same captcha.
                    'TODO : Fix the bug where if a bot kept using the same cookie with the same correct captcha resolution to spam.
                    RemoveCookie(Context, "captcha")
                End If
            Else
                message = FormatHTMLMessage(errorStr, wrongCaptcha, "default.aspx", "77", True)
                cont = False
            End If

        End If

       
        'Check for files bigger than the allowed limits.
        For Each fileKey As String In Request.Files.AllKeys
            Dim f As HttpPostedFile = Request.Files(fileKey)
            If f.ContentLength > MaximumFileSize Then
                message = FormatHTMLMessage(errorStr, FileToBig.Replace("%NAME%", f.FileName).Replace("%L%", CStr(FormatSizeString(MaximumFileSize))), "default.aspx", "8888", True)
                cont = False
            End If
        Next

        ''Post processing begin here 
        If cont Then
            If IsIPBanned(Request.UserHostAddress) Then
                message = FormatHTMLMessage("", "", WebRoot & "banned.aspx", "0", False)
            Else

                Select Case mode
                    Case "thread"
                        If Request.Files.Count = 0 Or Request.Files("ufile").ContentLength = 0 Then
                            message = FormatHTMLMessage("error", ImageRequired, "default.aspx", "60", True)
                        Else

                            Dim er As New OPData
                            er.Comment = ProcessInputs(Request.Item("comment"))
                            er.email = ProcessInputs(Request.Item("email")).Trim

                            If Request.Item("postername").Trim() = "" Then er.name = AnonNameStr Else er.name = ProcessInputs(Request.Item("postername"))

                            If isAdmin Then er.name = adminPostName
                            If isMod Then er.name = modPostName

                            er.subject = ProcessInputs(Request.Item("subject")).Trim
                            er.time = Date.UtcNow
                            er.password = ProcessInputs(Request.Item("password"))
                            er.IP = Request.UserHostAddress
                            er.HasFile = True
                            er.UserAgent = Request.UserAgent.Replace("<", String.Empty).Replace(">", String.Empty)

                            If Request.Cookies("pass") IsNot Nothing Then Request.Cookies("pass").Value = Request.Item("password") Else Request.Cookies.Add(New HttpCookie("pass", Request.Item("password")))

                            Dim tid As Integer = MakeThread(er)

                            Try
                                SavePostFile(Request.Files("ufile"), False, tid)
                            Catch ex As Exception
                                DeletePost(tid)
                                message = FormatHTMLMessage(errorStr, ex.Message, "default.aspx", "10", True)
                                Exit Select
                            End Try

                            message = FormatHTMLMessage(SuccessfulPostString, SuccessfulPostString, "default.aspx?id=" & tid, "1", False)

                        End If 'File count check

                    Case "reply"

                        Dim threadid As Integer = CInt(Request.Item("threadid"))

                        Dim brd As beforeReplyData = _get_beforeReplyData(threadid)

                        If brd.isGone Then
                            message = FormatHTMLMessage("error", thread404Str, "default.aspx", "5", True)
                            Exit Select
                        End If

                        If brd.isLocked Then
                            message = FormatHTMLMessage("error", lockedMessage, "default.aspx?id=" & threadid, "5", True)
                            Exit Select
                        End If

                        If brd.isArchived Then
                            message = FormatHTMLMessage("error", arhivedMessage, "archive.aspx?id=" & threadid, "5", True)
                            Exit Select
                        End If


                        If EnableImpresonationProtection Then
                            If IsPosterNameAlreadyTaken(Request.UserHostAddress, Request.Item("postername"), threadid) Then
                                message = FormatHTMLMessage(errorStr, nameAlreadyUsed.Replace("%", Request.Item("postername")), "default.aspx?id=" & threadid, "5", True)
                                Exit Select
                            End If
                        End If

                        If Request.Cookies("pass") IsNot Nothing Then Request.Cookies("pass").Value = Request.Item("password") Else Request.Cookies.Add(New HttpCookie("pass", Request.Item("password")))

                        Dim properFiles As New List(Of HttpPostedFile)

                        For Each key As String In Request.Files.AllKeys

                            Dim f As HttpPostedFile = Request.Files.Item(key)
                            If (f.ContentLength > 0) Then
                                properFiles.Add(f)
                            Else
                                'Maybe an empty file field.
                            End If
                        Next



                        'postId is partially global here since
                        'when a user post multiple files to each post, I want to redirect him the last reply id he made (typically default.aspx?id=threadid#pPostID).
                        Dim postId As Integer
                        Dim sage As Boolean = False

                        If Request.Item("finp") = "yes" And properFiles.Count > 1 Then ' Add each file to a seperate post, and dump the files.

                            Dim pos As Integer = 1
                            Dim countFiles As Boolean = (Request.Item("countf") = "yes")
                            Dim advanced As Boolean = False
                            For Each file As HttpPostedFile In properFiles
                                Dim er As New OPData

                                If Not advanced Then
                                    er.Comment = ProcessInputs(Request.Item("comment"))
                                    If countFiles Then er.Comment = er.Comment & CStr(vbNewLine & pos & "/" & properFiles.Count)
                                    advanced = True
                                Else
                                    If countFiles Then er.Comment = pos & "/" & properFiles.Count Else er.Comment = String.Empty
                                End If

                                If ProcessInputs(Request.Item("email")).Trim() = "sage" Then
                                    er.email = ""
                                    sage = True
                                Else
                                    er.email = ProcessInputs(Request.Item("email"))
                                End If

                                er.HasFile = True
                                If Request.Item("postername").Trim() = "" Then er.name = AnonNameStr Else er.name = ProcessInputs(Request.Item("postername"))

                                If isAdmin Then er.name = adminPostName
                                If isMod Then er.name = modPostName

                                er.subject = ProcessInputs(Request.Item("subject"))
                                er.time = Date.UtcNow
                                er.password = ProcessInputs(Request.Item("password"))
                                er.IP = Request.UserHostAddress
                                er.UserAgent = Request.UserAgent.Replace("<", "").Replace(">", "")
                                postId = ReplyTo(threadid, er)
                                SavePostFile(file, True, postId)

                                pos += 1
                            Next

                            message = FormatHTMLMessage(SuccessfulPostString, SuccessfulPostString, "default.aspx?id=" & threadid & "#p" & postId, "1", False)

                        Else
                            'Single file, or multiple files post.
                            Dim er As New OPData

                            If (Request.Item("comment").Length = 0 Or Request.Item("comment").Trim.Length = 0) And properFiles.Count = 0 Then
                                'no file and no text == blank post
                                message = FormatHTMLMessage("Error", noBlankpost, "", "7777", True)
                                Exit Select
                            Else

                                er.Comment = ProcessInputs(Request.Item("comment"))

                                If ProcessInputs(Request.Item("email")).Trim() = "sage" Then
                                    er.email = ""
                                    sage = True
                                Else
                                    er.email = ProcessInputs(Request.Item("email"))
                                End If

                                If Request.Item("postername").Trim() = "" Then er.name = AnonNameStr Else er.name = ProcessInputs(Request.Item("postername"))

                                If isAdmin Then er.name = adminPostName
                                If isMod Then er.name = modPostName

                                er.subject = ProcessInputs(Request.Item("subject"))
                                er.time = Date.UtcNow

                                er.password = ProcessInputs(Request.Item("password"))
                                er.IP = Request.UserHostAddress
                                er.UserAgent = Request.UserAgent.Replace("<", "").Replace(">", "")
                                er.HasFile = Not (properFiles.Count = 0)
                                postId = ReplyTo(threadid, er)

                                If er.HasFile Then
                                    Try
                                        SaveAllFilesToSinglePost(properFiles, postId)
                                    Catch ex As Exception
                                        DeletePost(postId) ' To prevent posts marked as having files but no files was saved.
                                        message = FormatHTMLMessage(errorStr, ex.Message, "default.aspx", "10", True)
                                        Exit Select
                                    End Try
                                End If

                                message = FormatHTMLMessage(SuccessfulPostString, SuccessfulPostString, "default.aspx?id=" & threadid & "#p" & postId, "1", False)
                            End If
                        End If
                        properFiles.Clear()
                        'Check if to bump thread or not

                        If Not (sage Or GetRepliesCount(threadid, True).TotalReplies >= BumpLimit) Then BumpThread(threadid)

                        If StaticHTML Then UpdateThreadHtml(threadid)

                    Case "report"

                        Dim id As Integer
                        Try
                            id = CInt(Request.Item("id"))
                        Catch ex As Exception
                            message = FormatHTMLMessage("error", NoPostWasSelected, "default.aspx", "60", True)
                            Exit Select
                        End Try


                        If id = 0 Then
                            message = FormatHTMLMessage("error", NoPostWasSelected, "default.aspx", "60", True)
                            Exit Select
                        End If

                        If Request("reportreason") = "" Then
                            message = FormatHTMLMessage("Ok", "", WebRoot & "report.aspx?id=" & CStr(id) & "&badcap=no", "0", False)
                        Else
                            If EnableCaptcha Then
                                'Check captcha
                                If Request("usercaptcha") = sessionCaptcha Then
                                    ' OK PROCEED TO REPORT POST
                                    ReportPost(id, Request.UserHostAddress, Date.UtcNow, Request.Item("reportreason"))
                                    message = FormatHTMLMessage("Ok", ReportedSucess.Replace("%", CStr(id)), "", "7777", False)
                                Else
                                    'REDIRECT TO REPORT PAGE WITH ERROR CAPTCHA NOTICE
                                    message = FormatHTMLMessage("Ok", "", WebRoot & "report.aspx?id=" & CStr(id) & "&badcap=yes", "0", False)
                                End If

                            Else
                                'REPORT POST
                                ReportPost(id, Request.UserHostAddress, Date.UtcNow, Request.Item("reportreason"))
                                message = FormatHTMLMessage("Ok", ReportedSucess.Replace("%", CStr(id)), "", "7777", False)
                            End If
                        End If

                    Case "delete"

                        'GenerateDeletePostPage

                        Dim id As Integer
                        Try
                            id = CInt(Request.Item("id"))
                        Catch ex As Exception
                            message = FormatHTMLMessage("error", NoPostWasSelected, "default.aspx", "60", True)
                            Exit Select
                        End Try

                        If id = 0 Then
                            message = FormatHTMLMessage("error", NoPostWasSelected, "default.aspx", "60", True)
                            Exit Select
                        End If

                        If Request("deletepass") = "" Then
                            message = FormatHTMLMessage("Ok", "", WebRoot & "deletepost.aspx?id=" & CStr(id), "0", False)
                        Else
                            Dim po As WPost = FetchPostData(id)
                            If EnableCaptcha Then
                                'Check captcha
                                If Request("usercaptcha") = sessionCaptcha Then
                                    ' OK. CHECK PASSWORD
                                    If po.password = CStr(Request.Item("deletepass")) Then
                                        'PRUNE IT
                                        PrunePost(id, AutoDeleteFiles)
                                        message = FormatHTMLMessage("Ok", PostDeletedSuccess.Replace("%", CStr(id)), "", "7777", False)
                                    Else
                                        'TELL THE USR THE PASSWORD IS WRONG
                                        message = FormatHTMLMessage("Ok", "", WebRoot & "deletepost.aspx?id=" & CStr(id) & "&badpas=yes", "0", False)
                                    End If
                                Else
                                    'REDIRECT TO DELETE POST PAGE WITH ERROR CAPTCHA NOTICE
                                    message = FormatHTMLMessage("Ok", "", WebRoot & "deletepost.aspx?id=" & CStr(id) & "&badcap=yes", "0", False)
                                End If

                            Else
                                'CHECK PASSWORD
                                If po.password = Request.Item("deletepass") Then
                                    'PRUNE IT
                                    PrunePost(id, AutoDeleteFiles)
                                    message = FormatHTMLMessage("Ok", PostDeletedSuccess.Replace("%", CStr(id)), "", "7777", False)
                                Else
                                    'TELL THE USER THE PASSWORD IS WRONG
                                    message = FormatHTMLMessage("Ok", "", WebRoot & "deletepost.aspx?id=" & CStr(id) & "&badpas=yes", "0", False)
                                End If
                            End If
                        End If
                    Case Else
                        message = FormatHTMLMessage("Error", invalidPostmodestr, "default.aspx", "2", True)
                End Select
            End If
        End If 'End of post processing.
        Return message
    End Function

    Friend Function ProcessPostAPI(ByVal context As HttpContext) As ApiResponse
        Dim resp As New ApiResponse

        Dim request As HttpRequest = context.Request
        Dim Session As HttpSessionState = context.Session

        Dim mode As String = request.Item("postmode")

        Dim isAdmin As Boolean = False
        If Not GetCookie(context, "admin") = "" Then isAdmin = CBool(GetCookie(context, "admin"))

        Dim isMod As Boolean = False
        If Not GetCookie(context, "mod") = "" Then isMod = CBool(GetCookie(context, "mod"))

        Dim ContinueProcessing As Boolean = True

        'Flood detection check
        If String.IsNullOrEmpty(CStr(Session.Item("lastpost"))) Then
            Session.Item("lastpost") = Now.ToString
        Else
            Dim i As Date = Date.Parse(CStr(Session.Item("lastpost")))
            If CInt((Now - i).TotalSeconds) <= TimeBetweenRequestes And (mode = "thread" Or mode = "reply") And Not (isAdmin) Then
                resp.ErrorType = ApiResponse.ErrType.Spam
                resp.ResponseType = ApiResponse.ResType.Err
                ContinueProcessing = False
            Else
                Session.Item("lastpost") = Now.ToString
            End If
        End If

        'Captcha check. Administrator does not need to enter captcha
        If EnableCaptcha And (mode = "thread" Or mode = "reply") And (Not isAdmin) Then

            Dim captchaStr As String = GetCookie(context, "captcha")
            If Not captchaStr = "" Then
                If Not captchaStr = request.Item("usercaptcha") Then
                    resp.ResponseType = ApiResponse.ResType.Err
                    resp.ErrorType = ApiResponse.ErrType.Captcha
                    ContinueProcessing = False
                Else
                    RemoveCookie(context, "captcha")
                End If
            Else
                resp.ResponseType = ApiResponse.ResType.Err
                resp.ErrorType = ApiResponse.ErrType.Captcha
                ContinueProcessing = False
            End If

        End If


        'Check for files bigger than the allowed limits.

        For Each fileKey As String In request.Files.AllKeys
            Dim f As HttpPostedFile = request.Files(fileKey)
            If f.ContentLength > MaximumFileSize Then
                resp.ErrorType = ApiResponse.ErrType.FileSize
                resp.ResponseType = ApiResponse.ResType.Err
                resp.ErrorMessage = f.FileName
                ContinueProcessing = False
            End If
        Next


        ''Post processing begin here 
        If ContinueProcessing Then
            If IsIPBanned(request.UserHostAddress) Then
                resp.ResponseType = ApiResponse.ResType.Err
                resp.ErrorType = ApiResponse.ErrType.Banned
            Else

                Select Case mode

                    Case "thread"
                        If request.Files.Count = 0 Or request.Files("ufile").ContentLength = 0 Then
                            resp.ErrorType = ApiResponse.ErrType.FileRequired
                            resp.ResponseType = ApiResponse.ResType.Err
                        Else

                            Dim er As New OPData
                            er.Comment = ProcessInputs(request.Item("comment"))
                            er.email = ProcessInputs(request.Item("email")).Trim

                            If request.Item("postername").Trim() = "" Then er.name = AnonNameStr Else er.name = ProcessInputs(request.Item("postername"))

                            If isAdmin Then er.name = adminPostName
                            If isMod Then er.name = modPostName

                            er.subject = ProcessInputs(request.Item("subject")).Trim
                            er.time = Date.UtcNow
                            er.password = ProcessInputs(request.Item("password"))
                            er.IP = request.UserHostAddress
                            er.HasFile = True
                            er.UserAgent = request.UserAgent.Replace("<", String.Empty).Replace(">", String.Empty)

                            If request.Cookies("pass") IsNot Nothing Then request.Cookies("pass").Value = request.Item("password") Else request.Cookies.Add(New HttpCookie("pass", request.Item("password")))

                            Dim tid As Integer = MakeThread(er)

                            Try
                                SavePostFile(request.Files("ufile"), False, tid)
                            Catch ex As Exception
                                DeletePost(tid)
                                resp.ErrorType = ApiResponse.ErrType.ServerError
                                resp.ResponseType = ApiResponse.ResType.Err
                                resp.ErrorMessage = ex.Message
                                Exit Select
                            End Try

                            resp.ResponseType = ApiResponse.ResType.NewThread
                            resp.PostID = tid
                            resp.ThreadID = tid

                        End If 'File count check

                    Case "reply"

                        Dim threadid As Integer = CInt(request.Item("threadid"))

                        Dim brd As beforeReplyData = _get_beforeReplyData(threadid)

                        If brd.isGone Then


                            resp.ResponseType = ApiResponse.ResType.Info
                            resp.ErrorMessage = thread404Str

                            Exit Select
                        End If

                        If brd.isLocked Then
                            resp.ResponseType = ApiResponse.ResType.Info
                            resp.ErrorMessage = lockedMessage
                            Exit Select
                        End If

                        If brd.isArchived Then
                            resp.ResponseType = ApiResponse.ResType.Info
                            resp.ErrorMessage = arhivedMessage
                            Exit Select
                        End If


                        If EnableImpresonationProtection Then
                            If IsPosterNameAlreadyTaken(request.UserHostAddress, request.Item("postername"), threadid) Then
                                resp.ResponseType = ApiResponse.ResType.Err
                                resp.ErrorType = ApiResponse.ErrType.ImpersonationProtection
                                resp.ErrorMessage = request.Item("postername")
                                Exit Select
                            End If
                        End If


                        If request.Cookies("pass") IsNot Nothing Then request.Cookies("pass").Value = request.Item("password") Else request.Cookies.Add(New HttpCookie("pass", request.Item("password")))

                        Dim properFiles As New List(Of HttpPostedFile)

                        For Each key As String In request.Files.AllKeys

                            Dim f As HttpPostedFile = request.Files.Item(key)
                            If (f.ContentLength > 0) Then
                                properFiles.Add(f)
                            Else
                                'Maybe an empty file field.
                            End If
                        Next



                        'postId is partially global here since
                        'when a user post multiple files to each post, I want to redirect him the last reply id he made (typically default.aspx?id=threadid#pPostID).
                        Dim postId As Integer
                        Dim sage As Boolean = False

                        If request.Item("finp") = "yes" And properFiles.Count > 1 Then ' Add each file to a seperate post, and dump the files.

                            Dim pos As Integer = 1
                            Dim countFiles As Boolean = (request.Item("countf") = "yes")
                            Dim advanced As Boolean = False

                            For Each file As HttpPostedFile In properFiles
                                Dim er As New OPData

                                If Not advanced Then
                                    er.Comment = ProcessInputs(request.Item("comment"))
                                    If countFiles Then er.Comment = er.Comment & CStr(vbNewLine & pos & "/" & properFiles.Count)
                                    advanced = True
                                Else
                                    If countFiles Then er.Comment = pos & "/" & properFiles.Count Else er.Comment = String.Empty
                                End If

                                If ProcessInputs(request.Item("email")).Trim() = "sage" Then
                                    er.email = ""
                                    sage = True
                                Else
                                    er.email = ProcessInputs(request.Item("email"))
                                End If

                                er.HasFile = True
                                If request.Item("postername").Trim() = "" Then er.name = AnonNameStr Else er.name = ProcessInputs(request.Item("postername"))

                                If isAdmin Then er.name = adminPostName
                                If isMod Then er.name = modPostName

                                er.subject = ProcessInputs(request.Item("subject"))
                                er.time = Date.UtcNow
                                er.password = ProcessInputs(request.Item("password"))
                                er.IP = request.UserHostAddress
                                er.UserAgent = request.UserAgent.Replace("<", "").Replace(">", "")
                                postId = ReplyTo(threadid, er)
                                SavePostFile(file, True, postId)

                                pos += 1
                            Next

                            resp.ResponseType = ApiResponse.ResType.Reply
                            resp.ThreadID = threadid
                            resp.PostID = postId

                        Else
                            'Single file, or multiple files post.
                            Dim er As New OPData
                            Dim comment As String = request.Item("comment").Trim()

                            If comment.Length = 0 And properFiles.Count = 0 Then
                                'no file and no text == blank post
                                resp.ResponseType = ApiResponse.ResType.Err
                                resp.ErrorType = ApiResponse.ErrType.BlankPost
                                Exit Select
                            Else

                                er.Comment = ProcessInputs(comment)

                                If ProcessInputs(request.Item("email")).Trim() = "sage" Then
                                    er.email = ""
                                    sage = True
                                Else
                                    er.email = ProcessInputs(request.Item("email"))
                                End If

                                If request.Item("postername").Trim() = "" Then er.name = AnonNameStr Else er.name = ProcessInputs(request.Item("postername"))

                                If isAdmin Then er.name = adminPostName
                                If isMod Then er.name = modPostName

                                er.subject = ProcessInputs(request.Item("subject"))
                                er.time = Date.UtcNow

                                er.password = ProcessInputs(request.Item("password"))
                                er.IP = request.UserHostAddress
                                er.UserAgent = request.UserAgent.Replace("<", "").Replace(">", "")
                                er.HasFile = Not (properFiles.Count = 0)
                                postId = ReplyTo(threadid, er)

                                If er.HasFile Then
                                    Try
                                        SaveAllFilesToSinglePost(properFiles, postId)
                                    Catch ex As Exception
                                        DeletePost(postId) ' To prevent posts marked as having files but no files was saved.
                                        resp.ResponseType = ApiResponse.ResType.Err
                                        resp.ErrorType = ApiResponse.ErrType.ServerError
                                        resp.ErrorMessage = ex.Message
                                        Exit Select
                                    End Try
                                End If

                                resp.ResponseType = ApiResponse.ResType.Reply
                                resp.ThreadID = threadid
                                resp.PostID = postId

                            End If
                        End If
                        properFiles.Clear()
                        'Check if to bump thread or not

                        If Not (sage Or GetRepliesCount(threadid, True).TotalReplies >= BumpLimit) Then BumpThread(threadid)

                        If StaticHTML Then UpdateThreadHtml(threadid)


                    Case Else
                        resp.ResponseType = ApiResponse.ResType.Err
                        resp.ErrorType = ApiResponse.ErrType.InvalidRequest
                End Select
            End If
        End If 'End of post processing.
        Return resp
    End Function

    Public Class ApiResponse
        Public ResponseType As ResType = ResType.Undefined
        Public ErrorMessage As String
        Public ErrorType As ErrType = ErrType.Undefined

        Public ThreadID As Integer = -1
        Public PostID As Integer = -1

        Public Enum ResType As Integer
            Undefined = -1
            Info = 0
            Err = 1
            NewThread = 2
            Reply = 3
        End Enum

        Public Enum ErrType As Integer
            Undefined = -1
            Captcha = 0
            FileSize = 1
            BlankPost = 2
            Spam = 3
            ImpersonationProtection = 4
            FileRequired = 5
            Banned = 6
            ServerError = 7
            InvalidRequest = 8
        End Enum


    End Class

    Private Function _get_beforeReplyData(ByVal threadid As Integer) As beforeReplyData
        Dim command As ChanbQuery = DatabaseEngine.ExecuteQueryReader("SELECT ID, locked, mta FROM board WHERE ID = " & CStr(threadid))
        Dim a As New beforeReplyData
        While command.Reader.Read
            If IsDBNull(command.Reader(0)) Then
                a.isGone = True
            Else
                a.isGone = False
                a.isLocked = command.Reader.GetBoolean(1)
                a.isArchived = command.Reader.GetBoolean(2)
            End If
        End While
        command.Reader.Close()
        command.Connection.Close()
        Return a
    End Function

    Private Class beforeReplyData
        Public isArchived As Boolean
        Public isLocked As Boolean
        Public isGone As Boolean
    End Class

    Dim wf As New WordFilter
    Friend Function ProcessComment(ByVal po As WPost, ByVal pageHandlerName As String, ByVal para As HTMLParameters) As String
        If po.comment = String.Empty Then
            Return String.Empty
        Else
            Dim sb As New StringBuilder
            po.comment = wf.FilterText(po.comment)

            For Each line In po.comment.Split(CChar(vbNewLine))
                Dim x As String = line.Replace(vbNewLine, String.Empty)
                If Not (x = "") Then

                    'Check if greentext
                    If x.StartsWith("&gt;") And Not x.StartsWith("&gt;&gt;") Then
                        sb.Append("<span class=""quote"">" & x & "</span")

                        ' Some times, X start with a line terminator that is not vbnewline, so i remove it
                    ElseIf (x.Remove(0, 1).StartsWith("&gt;") And Not x.Remove(0, 1).StartsWith("&gt;&gt;")) Then
                        sb.Append("<span class=""quote"">" & x.Remove(0, 1) & "</span>")

                    ElseIf IsXvalidQuote(x) Or IsXvalidQuote(x.Remove(0, 1)) Then
                        Dim quotet As String = x
                        If IsXvalidQuote(x.Remove(0, 1)) Then quotet = x.Remove(0, 1)

                        If po.archived Then
                            If ConvertArchivedThreadToHTML Then
                                sb.Append(formatBacklink(ArchivedTHTMLWebPath & CStr(po.parent) & ".html#p" & quotet.Replace("&gt;&gt;", String.Empty), x))
                            Else
                                sb.Append(formatBacklink(pageHandlerName & ".aspx?id=" & CStr(po.parent) & "#p" & quotet.Replace("&gt;&gt;", String.Empty), x))
                            End If
                        Else
                            If StaticHTML Then
                                sb.Append(formatBacklink(ThreadHTMLWebPath & CStr(po.parent) & ".html#p" & quotet.Replace("&gt;&gt;", String.Empty), x))
                            Else
                                sb.Append(formatBacklink(pageHandlerName & ".aspx?id=" & CStr(po.parent) & "#p" & quotet.Replace("&gt;&gt;", String.Empty), x))
                            End If
                        End If

                    Else
                        sb.Append(x)
                    End If
                    sb.Append("<br/>")
                End If
            Next

            Dim finalcomment As New StringBuilder

            Dim sr As String = sb.ToString
            sr = MatchAndProcessBBCodes("spoiler", sr)
            sr = MatchAndProcessBBCodes("code", sr)
            sr = MatchAndProcessBBCodes("md", sr)
            sr = MatchAndProcessBBCodes("q", sr)

            Return sr
        End If
    End Function

    Private Function formatBacklink(ByVal link As String, ByVal text As String) As String
        Return "<a class=""backlink"" href=""" & link & """> " & text & " </a>"
    End Function

    ''' <summary>
    ''' Return a string for threads displayed in the main page.
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
        If trailposts > 0 And (wpolist.Length > 1) Then
            Dim sb As New StringBuilder
            For i As Integer = 1 To wpolist.Length - 1 Step 1
                sb.Append(GetPostHTML(wpolist(i), para))
                If Not wpolist(i).FileCount = 0 Then
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

            If tOm >= 1 And tIm >= 1 Then
                Dim summary As String = summaryPandIStr.Replace("%P%", CStr(tOm)).Replace("%I%", CStr(tIm))
                postHtml = postHtml.Replace("%SUMMARY%", summary & summaryClickToViewStr)
                postHtml = postHtml.Replace("%MOBILESUMMARY%", summary)
            End If

            If tOm >= 1 And tIm <= 0 Then
                Dim summary As String = summaryPonlyStr.Replace("%P%", CStr(tOm))
                postHtml = postHtml.Replace("%SUMMARY%", summary & summaryClickToViewStr)
                postHtml = postHtml.Replace("%MOBILESUMMARY%", summary)
            End If

            If tOm <= 0 And tIm >= 1 Then
                Dim summary As String = summaryIonlyStr.Replace("%I%", CStr(tIm))
                postHtml = postHtml.Replace("%SUMMARY%", summary & summaryClickToViewStr)
                postHtml = postHtml.Replace("%MOBILESUMMARY%", summary)
            End If

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
    Function GetThreadHTML(ByVal threadID As Integer, ByVal p As HTMLParameters) As String
        Dim sb As New StringBuilder
        Dim data As WPost() = GetThreadData(threadID, Not p.isCurrentThread)
        For i = 0 To data.Length - 1 Step 1
            sb.Append(GetPostHTML(data(i), p))
        Next
        Return sb.ToString
    End Function

    Function GetModeratorHTMLMenu(ByVal powers As String) As String
        If powers = "admin" Then
            'adminMenuItems
            Dim sb As New StringBuilder
            For Each x In adminMenuItems
                sb.Append(x)
            Next
            sb.Append("<li<a>-----</a>li>")
            Return sb.ToString
        Else
            If IsModPowersValid(powers) Then
                Dim power As String() = powers.Split(CChar("-"))
                Dim sb As New StringBuilder
                For i As Integer = 0 To power.Length - 1 Step 1
                    If power(i) = "1" Then sb.Append(modMenuItems(i))
                Next
                sb.Append("<li><a>-----</a></li>")
                Return sb.ToString
            Else
                Return "<!-- invalid mod powers -->"
            End If
        End If
    End Function

    Sub BanPosterByPost(ByVal postID As Integer, ByVal silentBan As Boolean, ByVal modname As String, ByVal reason As String)
        Dim po As WPost = FetchPostData(postID)
        If IsIPBanned(po.ip) = False Then
            'internalname$autoperm$length$canview$Localised ban reason.
            '0             1         2      3          4
            Dim reasonData As String() = {}

            For Each x In modBanReasons
                Dim sp As String() = x.Split(CChar("$"))
                If sp.ElementAt(0) = reason Then
                    reasonData = sp
                End If
            Next

            If reasonData Is Nothing Then
                Throw New ArgumentNullException("Invalid reason")
                Exit Sub
            End If

            Dim permBan As Boolean = (reasonData.ElementAt(1) = "yes")
            Dim length As Date = Now + New TimeSpan(CInt(reasonData.ElementAt(2)), 0, 0, 0)

            Dim canbrowse As Boolean = (reasonData.ElementAt(3) = "yes")

            BanPoster(po.ip, postID, reason, modname, length, permBan, canbrowse)

            If Not silentBan Then
                Dim newText As String = po.comment & bannedMessageHTML
                UpdatePostText(postID, newText, True)
            End If
        End If
    End Sub

    Sub BanPosterByPost_CR(ByVal postID As Integer, ByVal silentBan As Boolean, ByVal cutomReason As String, ByVal modname As String, ByVal perm As Boolean, ByVal exptime As Date, ByVal canbrowse As Boolean)
        Dim po As WPost = FetchPostData(postID)
        If Not IsIPBanned(po.ip) Then
            BanPoster(po.ip, postID, cutomReason, modname, exptime, perm, canbrowse)
            If Not silentBan Then
                Dim newText As String = po.comment & bannedMessageHTML
                UpdatePostText(postID, newText, True)
            End If
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

        If Not po.FileCount = 0 Then

            'Check for multiple files.
            If po.FileCount > 1 Then
                'We need to add HTML file rotator.
                Dim items As New StringBuilder
                Dim noscriptItems As New StringBuilder
                Dim isNext As Boolean = False ' The first file is marked as 'active', the rest as 'notactive'
                Dim rotatorHTML As String = FilesRotatorTemplate

                For Each wpi As WPostImage In po.files

                    If FileHasThumb(wpi.Extension) Then
                        Dim scriptItem As String = GetImageHTML(wpi)

                        If Not isNext Then scriptItem = scriptItem.Replace("%AN%", "active") Else scriptItem = scriptItem.Replace("%AN%", "notactive")
                        scriptItem = scriptItem.Replace("%filec%", "")

                        items.Append(scriptItem)
                        noscriptItems.Append(GetImageHTMLNoScript(wpi))

                    Else ' It's a video or an audio file.

                        Select Case wpi.Extension
                            Case "WEBM"
                                Dim scriptItem As String = GetVideoFileHTML(wpi, po.PostID, "webm")
                                If Not isNext Then scriptItem = scriptItem.Replace("%AN%", "active") Else scriptItem = scriptItem.Replace("%AN%", "notactive")
                                scriptItem = scriptItem.Replace("%filec%", "")
                                items.Append(scriptItem)
                                noscriptItems.Append(GetVideoFileHTMLNoScript(wpi))
                            Case "MP3"
                                Dim scriptItem As String = GetAudioFileHTML(wpi, po.PostID, "mpeg")
                                If Not isNext Then scriptItem = scriptItem.Replace("%AN%", "active") Else scriptItem = scriptItem.Replace("%AN%", "notactive")
                                scriptItem = scriptItem.Replace("%filec%", "")
                                items.Append(scriptItem)
                                noscriptItems.Append(GetAudioFileHTMLNoScript(wpi))
                            Case "OGG"
                                Dim scriptItem As String = GetAudioFileHTML(wpi, po.PostID, "ogg")
                                If Not isNext Then scriptItem = scriptItem.Replace("%AN%", "active") Else scriptItem = scriptItem.Replace("%AN%", "notactive")
                                scriptItem = scriptItem.Replace("%filec%", "")
                                items.Append(scriptItem)
                                noscriptItems.Append(GetAudioFileHTMLNoScript(wpi))
                            Case Else
                                Dim fileHandled As Boolean = False
                                Dim scriptItem As String = ""
                                Dim noscriptItem As String = ""

                                For i As Int32 = 0 To CFH_Plugins.Length - 1 Step 1
                                    Dim cfh As CustomFileHandler = CFH_Plugins(i)
                                    If cfh.Is_FileSupported(wpi.Extension) Then
                                        fileHandled = True
                                        scriptItem = cfh.GetFileHTML(wpi) _
                                        .Replace("%IMAGE TEXT DL%", WebRoot & "img.aspx?cn=" & wpi.ChanbName & "&rn=" & wpi.RealName) _
                                        .Replace("%FILE SIZE%", chanb.FormatSizeString(wpi.Size)) _
                                        .Replace("%Search Engine Links%", GetSearchEngineLinks(GetImageWEBPATHRE(wpi.ChanbName))) _
                                        .Replace("%filec%", "")
                                        If Not isNext Then scriptItem = scriptItem.Replace("%AN%", "active") Else scriptItem = scriptItem.Replace("%AN%", "notactive")

                                        noscriptItem = cfh.GetFileHTML_NoScript(wpi)

                                        Exit For
                                    End If

                                Next

                                If fileHandled Then
                                    items.Append(scriptItem)
                                    noscriptItems.Append(noscriptItem)
                                End If

                        End Select

                    End If

                    isNext = True
                Next

                rotatorHTML = rotatorHTML.Replace("%ID%", CStr(po.PostID))
                rotatorHTML = rotatorHTML.Replace("%IMAGECOUNT%", CStr(po.FileCount))
                rotatorHTML = rotatorHTML.Replace("%ITEMS%", items.ToString)
                rotatorHTML = rotatorHTML.Replace("%NOS%", noscriptItems.ToString)
                sb.Append(rotatorHTML)

            Else
                'Single file
                Dim wpi As WPostImage = po.files(0)
                If FileHasThumb(wpi.Extension) Then
                    Dim item As String = GetImageHTML(wpi)
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
                        Case "MP3"
                            Dim item As String = GetAudioFileHTML(wpi, po.PostID, "mpeg")
                            item = item.Replace("%filec%", "file")
                            item = item.Replace("%AN%", "")
                            sb.Append(item)
                        Case "OGG"
                            Dim item As String = GetAudioFileHTML(wpi, po.PostID, "ogg")
                            item = item.Replace("%filec%", "file")
                            item = item.Replace("%AN%", "")
                            sb.Append(item)
                        Case Else

                            Dim fileHandled As Boolean = False
                            Dim result As String = ""

                            For i As Int32 = 0 To CFH_Plugins.Length - 1 Step 1
                                Dim cfh As CustomFileHandler = CFH_Plugins(i)
                                If cfh.Is_FileSupported(wpi.Extension) Then
                                    fileHandled = True
                                    result = cfh.GetFileHTML(wpi) _
                                    .Replace("%IMAGE TEXT DL%", WebRoot & "img.aspx?cn=" & wpi.ChanbName & "&rn=" & wpi.RealName) _
                                    .Replace("%FILE SIZE%", chanb.FormatSizeString(wpi.Size)) _
                                    .Replace("%Search Engine Links%", GetSearchEngineLinks(GetImageWEBPATHRE(wpi.ChanbName))) _
                                    .Replace("%AN%", "").Replace("%filec%", "file")
                                    Exit For
                                End If

                            Next

                            If fileHandled Then
                                sb.Append(result)
                            End If
                    End Select

                End If ' File extension check block
            End If ' Multiple file check block
        End If  'If there is no files, no action is needed.
        Return sb.ToString
    End Function

    Private Function GetImageHTML(ByVal wpi As WPostImage) As String
        Dim r As String = ImageTemplate
        r = r.Replace("%ID%", CStr(wpi.PostID))
        r = r.Replace("%FILE NAME%", wpi.RealName)
        r = r.Replace("%IMAGE TEXT DL%", WebRoot & "img.aspx?cn=" & wpi.ChanbName & "&rn=" & wpi.RealName)
        r = r.Replace("%IMAGE DL%", wpi.ImageWebPath)
        r = r.Replace("%IMAGE SRC%", wpi.ImageWebPath)
        r = r.Replace("%FILE SIZE%", FormatSizeString(wpi.Size))
        r = r.Replace("%IMAGE SIZE%", wpi.Dimensions)
        r = r.Replace("%THUMB_LINK%", wpi.ImageThumbailWebPath)
        r = r.Replace("%IMAGE MD5%", wpi.MD5)
        r = r.Replace("%IMAGE EXT%", wpi.Extension)
        r = r.Replace("%Search Engine Links%", GetSearchEngineLinks(GetImageWEBPATHRE(wpi.ChanbName)))
        Return r
    End Function

    Private Function GetImageHTMLNoScript(ByVal wpi As WPostImage) As String
        Dim nr As String = noscriptItemHTML
        nr = nr.Replace("%IMAGE SRC%", wpi.ImageWebPath)
        nr = nr.Replace("%IMAGE DL%", wpi.ImageWebPath)
        nr = nr.Replace("%FILE NAME%", wpi.RealName)
        nr = nr.Replace("%THUMB_LINK%", wpi.ImageThumbailWebPath)
        Return nr
    End Function

    Private Function GetVideoFileHTML(ByVal wpi As WPostImage, ByVal postId As Integer, ByVal ext As String) As String
        Dim r As String = VideoItemTemplate
        r = r.Replace("%ID%", CStr(postId))
        r = r.Replace("%FILE NAME%", wpi.RealName)
        r = r.Replace("%FILE SIZE%", FormatSizeString(wpi.Size))
        r = r.Replace("%IMAGE TEXT DL%", WebRoot & "img.aspx?cn=" & wpi.ChanbName & "&rn=" & wpi.RealName)
        r = r.Replace("%VIDEO LINK%", wpi.ImageWebPath)
        r = r.Replace("%IMAGE MD5%", wpi.MD5)
        r = r.Replace("%IMAGE EXT%", wpi.Extension)
        r = r.Replace("%NO VIDEO SUPPORT%", noVideoSupportStr)
        r = r.Replace("%EXT%", ext)
        Return r
    End Function

    Private Function GetVideoFileHTMLNoScript(ByVal wpi As WPostImage) As String
        Dim nr As String = noscriptVideoHTML
        nr = nr.Replace("%VIDEO LINK%", wpi.ImageWebPath)
        nr = nr.Replace("%FILE NAME%", wpi.RealName)
        Return nr
    End Function

    Private Function GetAudioFileHTML(ByVal wpi As WPostImage, ByVal postId As Integer, ByVal ext As String) As String
        Dim r As String = AudioItemTemplate
        r = r.Replace("%ID%", CStr(postId))
        r = r.Replace("%FILE NAME%", wpi.RealName)
        r = r.Replace("%FILE SIZE%", FormatSizeString(wpi.Size))
        r = r.Replace("%IMAGE TEXT DL%", WebRoot & "img.aspx?cn=" & wpi.ChanbName & "&rn=" & wpi.RealName)
        r = r.Replace("%LINK%", wpi.ImageWebPath)
        r = r.Replace("%IMAGE MD5%", wpi.MD5)
        r = r.Replace("%IMAGE EXT%", wpi.Extension)
        r = r.Replace("%NO AUDIO SUPPORT%", noVideoSupportStr)
        r = r.Replace("%EXT%", ext)
        Return r
    End Function

    Private Function GetAudioFileHTMLNoScript(ByVal wpi As WPostImage) As String
        Dim nr As String = noscriptAudioHTML
        nr = nr.Replace("%LINK%", wpi.ImageWebPath)
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
            Dim files As IO.FileInfo() = FileIO.FileSystem.GetDirectoryInfo(StorageFolder).GetFiles()
            For i As Integer = 0 To files.Length - 1 Step 1
                files(i).Delete()
            Next
            files = Nothing
            DatabaseEngine.ExecuteNonQuery("TRUNCATE TABLE files")
        End If
    End Sub

    ''' <summary>
    ''' Prune a post. It either delete a post or archive it, based on specific conditions. 
    ''' </summary>
    ''' <param name="id">Post id. Can be a thread or a reply.</param>
    ''' <param name="dF">Delete files</param>
    ''' <remarks></remarks>
    Public Sub PrunePost(ByVal id As Integer, ByVal dF As Boolean, Optional ByVal forceDelete As Boolean = False)
        If EnableArchive And Not forceDelete Then
            Archive(id)
        Else
            Dim w As WPost = FetchPostData(id)
            If w.type = WPost.PostType.Thread Then ' post is a thread, delete replies first.
                If dF Then ' dF is a shortcut for delete files.
                    For Each x As WPost In GetThreadData(id, True)
                        DeletePostFiles(x)
                    Next
                End If
                DeleteThread(id)
            ElseIf w.type = WPost.PostType.Reply Then
                If dF Then DeletePostFiles(w)
                DeletePost(id)
                If StaticHTML Then
                    UpdateThreadHtml(w.parent)
                End If
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
        ElseIf w.type = WPost.PostType.Reply Then
            ArchivePost(id)
            'Update parent thread.
            If StaticHTML Then
                UpdateThreadHtml(w.parent)
            End If
        Else
            Return
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
        If po.FileCount = 0 Then
            Return
        Else
            For Each ima As WPostImage In po.files
                If FileExistInDB(ima.MD5, po.PostID) = False Then ' I exclude this post from file checking so we don't delete another post file that is using the same file.
                    Dim realPath As String = StorageFolder & "\" & ima.ChanbName
                    Dim thumbPath As String = StorageFolderThumbs & "\th" & ima.ChanbName
                    Delete_File_Fromdisk(realPath)
                    'Don't delete thumbs when archive is enabled.
                    If Not EnableArchive Then Delete_File_Fromdisk(thumbPath)
                End If
            Next
        End If
    End Sub



    Public Function GeneratePageHTML(ByVal isArchive As Boolean, ByVal context As HttpContext) As String
        ''Todo : fix this

        Dim request As HttpRequest = context.Request
        Dim response As HttpResponse = context.Response
        Dim session As HttpSessionState = context.Session


        If Not CanIPBrowse(Request.UserHostAddress) Then
            Response.Redirect(WebRoot & "banned.aspx")
        End If

        Dim DisplayingThread As Boolean = Not (Request.Item("id") = "")

        Dim pageHTML As String = GenerateGenericHTML()

        Dim pageHandlerLink As String = "default"
        If isArchive Then pageHandlerLink = "archive"

        If isArchive Then
            pageHTML = pageHTML.Replace("%POSTING MODE%", archiveNotice)
            pageHTML = pageHTML.Replace("%POSTDIVCLASS%", "hide")
        Else
            pageHTML = pageHTML.Replace("%POSTDIVCLASS%", "")
            If DisplayingThread Then
                pageHTML = pageHTML.Replace("%POSTING MODE%", postingModeReplyHtml)
            Else
                pageHTML = pageHTML.Replace("%POSTING MODE%", "")
            End If
        End If

        pageHTML = pageHTML.Replace("%POST FORM TID%", Request.Item("id"))


        If EnableCaptcha And Not isArchive Then
            pageHTML = pageHTML.Replace("%CAPTCHA PHOLDER%", captchaTableEntryHtml)
        Else
            pageHTML = pageHTML.Replace("%CAPTCHA PHOLDER%", "")
        End If


        If DisplayingThread Then
            'Posting rules
            pageHTML = pageHTML.Replace("%POSTING RULES%", postingRulesHTML)
            pageHTML = pageHTML.Replace("%THREAD COUNT%", "")

            'Return buttons
            pageHTML = pageHTML.Replace("%RETURN BUTTON DESKTOP%", DesktopReturnButtonHTML.Replace("%P%", pageHandlerLink & ".aspx"))
            pageHTML = pageHTML.Replace("%RETURN BUTTON MOBILE%", MobileReturnButtonHTML.Replace("%P%", pageHandlerLink & ".aspx"))

            'Add new file button
            pageHTML = pageHTML.Replace("%ADD NEW FILES PHOLDER%", addNewFileButtonHTML)

            'Reply/New thread button
            pageHTML = pageHTML.Replace("%POST FORM MODE%", "reply")
            pageHTML = pageHTML.Replace("%POST FORM BUTTON%", replyStr)
        Else
            'Posting rules
            pageHTML = pageHTML.Replace("%POSTING RULES%", "")
            pageHTML = pageHTML.Replace("%THREAD COUNT%", threadCountHTMLli.Replace("%", CStr(GetThreadsCount(isArchive))))

            'Return buttons
            pageHTML = pageHTML.Replace("%RETURN BUTTON DESKTOP%", "")
            pageHTML = pageHTML.Replace("%RETURN BUTTON MOBILE%", "")

            'Add new file button
            pageHTML = pageHTML.Replace("%ADD NEW FILES PHOLDER%", "")

            'Reply/New thread button
            pageHTML = pageHTML.Replace("%POST FORM MODE%", "thread")
            pageHTML = pageHTML.Replace("%POST FORM BUTTON%", newThreadStr)
        End If

        pageHTML = pageHTML.Replace("%EXTRA META%", "")
        pageHTML = pageHTML.Replace("%MAXIMUM FILE SIZE%", FormatSizeString(MaximumFileSize))
        pageHTML = pageHTML.Replace("%SESSION PASSWORD%", GetSessionPassword(Request.Cookies, session))


        '####################################### BODY PROCESSING CODE #######################################
        Dim body As New StringBuilder
        Dim para As New HTMLParameters()
        If Not GetCookie(context, "mod") = "" Then para.isModerator = CBool(GetCookie(context, "mod"))
        If Not GetCookie(context, "admin") = "" Then para.isAdmin = CBool(GetCookie(context, "admin"))
        If Not GetCookie(context, "credpower") = "" Then para.CredPowers = CStr(GetCookie(context, "credpower"))

        para.isCurrentThread = Not isArchive

        Dim validID As Boolean = False
        Try
            Dim i = CInt(request.Item("id"))
            validID = True
        Catch ex As Exception
            validID = False
        End Try


        If DisplayingThread And validID Then

            'Display a thread and children posts 
            Dim opID As Integer = CInt(request.Item("id"))
            opID = Math.Abs(opID)
            Dim po As WPost = FetchPostData(opID)

            If StaticHTML Then
                If FileIO.FileSystem.FileExists(ThreadStorageFolder & "\" & opID & ".html") = False Then UpdateThreadHtml(opID)
                response.Redirect(ThreadHTMLWebPath & opID & ".html")
            End If

            If po.type = WPost.PostType.Unknown Then
                response.Redirect(pageHandlerLink & ".aspx")
            End If

            If po.archived And Not isArchive Then
                response.Redirect("archive.aspx?id=" & po.PostID)
            ElseIf po.archived = False And isArchive Then
                response.Redirect("default.aspx?id=" & po.PostID)
            End If

            ' Check if it is a reply or a thread , 0 = thread, 1 = reply
            ' If it is a reply, redirect to parent thread.
            If CInt(po.type) = 1 Then response.Redirect(pageHandlerLink & ".aspx?id=" & po.parent & "#p" & po.PostID)


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
            If Not (request.Item("startindex") = "") Then startIndex = CInt(request.Item("startindex")) * (ThreadPerPage)
            For Each x In GetThreads(startIndex, ThreadPerPage - 1 + startIndex, False, isArchive)
                body.Append(GetStreamThreadHTML(x, para, TrailPosts))
            Next

        End If
        pageHTML = pageHTML.Replace("%BODY%", body.ToString)
        '####################################### END OF BODY PROCESSING CODE #######################################

        '####################################### BEGIN OF PAGE LIST PROCESSING CODE ################################
        If Not DisplayingThread Then ' Show pages numbers list

            Dim sb As New StringBuilder
            sb.Append("<div align=""center"" class=""pagelist desktop"">")

            Dim threadCount As Integer = GetThreadsCount(isArchive)
            Dim pagesCount As Double = threadCount / ThreadPerPage
            If pagesCount > (Fix(pagesCount)) Then
                pagesCount = Fix(pagesCount) + 1
            End If

            Dim startIndexA As Integer

            Try
                startIndexA = CInt(request.Item("startindex"))
            Catch ex As Exception
                startIndexA = 0
            End Try

            If startIndexA > 0 Then
                sb.Append("<div><a class='buttonBlue' href='" & pageHandlerLink & ".aspx?startindex=" & CStr(startIndexA - 1) & "'>" & prevStr & "</a></div>")
            End If

            sb.Append("<div class='pages'>")

            For i As Integer = 0 To CInt((pagesCount - 1)) Step 1
                If i = startIndexA Then
                    sb.Append("<strong><a class='buttonBlue' href='" & pageHandlerLink & ".aspx?startindex=" & i & "'>" & i + 1 & "</a></strong>")
                Else
                    sb.Append("<a class='buttonBlue' href='" & pageHandlerLink & ".aspx?startindex=" & i & "'>" & i + 1 & "</a>")
                End If
            Next

            sb.Append("</div>")

            If Not (startIndexA = pagesCount - 1 Or threadCount = 0) Then ' last page
                sb.Append("<div><a class='buttonBlue' href='" & pageHandlerLink & ".aspx?startindex=" & CStr(startIndexA + 1) & "'>" & nextStr & "</a></div>")
            End If

            sb.Append("</div>")

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

        pageHTML = pageHTML.Replace("%EXTRA META%", "<link rel=""stylesheet"" href=""" & WebRoot & "catalog.css"">")
        pageHTML = pageHTML.Replace("%RETURN BUTTON DESKTOP%", DesktopReturnButtonHTML.Replace("%P%", "default.aspx"))
        pageHTML = pageHTML.Replace("%RETURN BUTTON MOBILE%", MobileReturnButtonHTML.Replace("%P%", "default.aspx"))


        '####################################### BODY PROCESSING CODE #######################################
        'GenerateCatalogItems(GetThreads(0, GetThreadsCount(False), False, False))
        pageHTML = pageHTML.Replace("%BODY%", GenerateCatalogItems(GetThreads(0, (ThreadPerPage * MaximumPages) - 1, False, False)) & "<hr/>")

        pageHTML = pageHTML.Replace("%PAGES LIST%", "")

        Return pageHTML
    End Function

    Function GenerateReportPage(ByVal context As HttpContext) As String
        Dim pageHTML As String = ReportPageTemplate

        pageHTML = pageHTML.Replace("%BTITLE%", BoardTitle).Replace("%BDESC%", BoardDesc).Replace("%ROOT%", WebRoot).Replace("%FOOTER TEXT%", footerText)

        Dim id As Integer = -1
        Try
            id = Math.Abs(CInt(context.Request.Item("id")))
        Catch ex As Exception
            Return FormatHTMLMessage("error", NoPostWasSelected, "", "8888", True)
            Exit Function
        End Try

        If id = 0 Then
            Return FormatHTMLMessage("error", NoPostWasSelected, "", "8888", True)
            Exit Function
        End If

        pageHTML = pageHTML.Replace("%ID%", CStr(id))

        If EnableCaptcha Then
            pageHTML = pageHTML.Replace("%CP%", "")
            pageHTML = pageHTML.Replace("%LANG verificationStr%", verificationStr)
        Else
            pageHTML = pageHTML.Replace("%CP%", "hide")
        End If

        If context.Request("badcap") = "yes" Then
            pageHTML = pageHTML.Replace("%badc%", "")
        Else
            pageHTML = pageHTML.Replace("%badc%", "hide")
        End If


        pageHTML = pageHTML.Replace("%LANG reportStr%", reportStr)
        pageHTML = pageHTML.Replace("%LANG reportReasonStr%", reportReasonStr)
        pageHTML = pageHTML.Replace("%LANG wrongCaptchaStr%", wrongCaptcha)

        Dim reasons As New StringBuilder

        For Each x In reportReasons
            Dim reason As String() = x.Split(CChar("$"))
            reasons.Append("<option name=""" & reason(0) & """>" & reason(1) & "</option>")
        Next


        pageHTML = pageHTML.Replace("%REASONS%", reasons.ToString)

        Return pageHTML
    End Function

    Function GenerateDeletePostPage(ByVal context As HttpContext) As String
        Dim pageHTML As String = DeletePostPageTemplate

        pageHTML = pageHTML.Replace("%BTITLE%", BoardTitle).Replace("%BDESC%", BoardDesc).Replace("%ROOT%", WebRoot).Replace("%FOOTER TEXT%", footerText)

        Dim id As Integer = -1
        Try
            id = Math.Abs(CInt(context.Request.Item("id")))
        Catch ex As Exception
            Return FormatHTMLMessage("error", NoPostWasSelected, "", "8888", True)
            Exit Function
        End Try

        If id = 0 Then
            Return FormatHTMLMessage("error", NoPostWasSelected, "", "8888", True)
            Exit Function
        End If

        pageHTML = pageHTML.Replace("%ID%", CStr(id))

        If EnableCaptcha Then
            pageHTML = pageHTML.Replace("%CP%", "")
            pageHTML = pageHTML.Replace("%LANG verificationStr%", verificationStr)
        Else
            pageHTML = pageHTML.Replace("%CP%", "hide")
        End If

        If context.Request("badpas") = "yes" Then
            pageHTML = pageHTML.Replace("%badp%", "")
        Else
            pageHTML = pageHTML.Replace("%badp%", "hide")
        End If

        If context.Request("badcap") = "yes" Then
            pageHTML = pageHTML.Replace("%badc%", "")
        Else
            pageHTML = pageHTML.Replace("%badc%", "hide")
        End If

        pageHTML = pageHTML.Replace("%LANG deleteStr%", deleteStr)
        pageHTML = pageHTML.Replace("%LANG enterPassword%", passwordStr)

        pageHTML = pageHTML.Replace("%LANG wrongCaptchaStr%", wrongCaptcha)

        pageHTML = pageHTML.Replace("%LANG wrongPasswordStr%", CannotDeletePostBadPassword.Replace("%", CStr(id)))

        Return pageHTML
    End Function

    Function GenerateModSBR(ByVal context As HttpContext) As String
        Dim pageHTML As String = modSBRPageTemplate

        pageHTML = pageHTML.Replace("%BTITLE%", BoardTitle).Replace("%BDESC%", BoardDesc).Replace("%ROOT%", WebRoot).Replace("%FOOTER TEXT%", footerText)

        Dim id As Integer = -1
        Try
            id = Math.Abs(CInt(context.Request.Item("id")))
        Catch ex As Exception
            Return FormatHTMLMessage("error", NoPostWasSelected, "", "8888", True)
            Exit Function
        End Try

        If id = 0 Then
            Return FormatHTMLMessage("error", NoPostWasSelected, "", "8888", True)
            Exit Function
        End If

        Dim isAdmin As Boolean = False
        If Not GetCookie(context, "admin") = "" Then isAdmin = CBool(GetCookie(context, "admin"))

        If isAdmin Then
            pageHTML = pageHTML.Replace("%formname%", "adminaction")
        Else
            pageHTML = pageHTML.Replace("%formname%", "modaction")
        End If

        pageHTML = pageHTML.Replace("%ID%", CStr(id))
        pageHTML = pageHTML.Replace("%SIB%", context.Request.Item("sib"))

        pageHTML = pageHTML.Replace("%LANG modSelectBanReason%", Language.modSelectBanReason)
        pageHTML = pageHTML.Replace("%LANG banUserStr%", banuserStr)

        Dim sb As New StringBuilder
        For Each x In modBanReasons
            'internalname$autoperm$length$canview$Localised ban reason.
            Dim reasonData As String() = x.Split(CChar("$"))
            Dim opt As String = "<option value=""" & reasonData(0) & """>" & reasonData(4) & "</option>"
            sb.Append(opt)
        Next
        pageHTML = pageHTML.Replace("%REASONS%", sb.ToString)
        Return pageHTML
    End Function

    Function GenerateBanPage(ByVal context As HttpContext) As String
        Dim bd As BanData = GetBanData(context.Request.UserHostAddress)
        If bd.BanEffective Then
            If Now > bd.ExpirationDate Then
                DropBan(context.Request.UserHostAddress)
                context.Response.Redirect(WebRoot & "default.aspx")
                Return ""
            Else
                Dim pageHTML As String = BanPageTemplate
                pageHTML = pageHTML.Replace("%BTITLE%", BoardTitle).Replace("%BDESC%", BoardDesc).Replace("%ROOT%", WebRoot).Replace("%FOOTER TEXT%", footerText)
                pageHTML = pageHTML.Replace("%REASON%", _GetLocalisedBanReason(bd.Comment))
                pageHTML = pageHTML.Replace("%BOD%", GetTimeString(bd.BannedOn))
                pageHTML = pageHTML.Replace("%IP%", bd.IP)
                pageHTML = pageHTML.Replace("%POSTNO%", CStr(bd.PostNumber))
                If bd.Permanant Then
                    pageHTML = pageHTML.Replace("%EXP%", "Never")
                Else
                    pageHTML = pageHTML.Replace("%EXP%", GetTimeString(bd.ExpirationDate))
                End If
                pageHTML = pageHTML.Replace("%LANG bannedMessageStr%", BannedMessage)
                pageHTML = pageHTML.Replace("%LANG banReasonStr%", banReasonStr)
                pageHTML = pageHTML.Replace("%LANG bannedOnStr%", bannedOnStr)
                pageHTML = pageHTML.Replace("%LANG banIpStr%", banIpStr)
                pageHTML = pageHTML.Replace("%LANG banPostNoStr%", banPostNoStr)
                pageHTML = pageHTML.Replace("%LANG banExpiryStr%", banExpiryStr)
                Return pageHTML
            End If
        Else
            DropBan(context.Request.UserHostAddress)
            context.Response.Redirect("default.aspx")
            Return ""
        End If
    End Function

    Sub GenerateModEditPostPage(ByVal context As HttpContext)
        Dim postId As Integer
        Try
            postId = CInt(context.Request.Item("id"))
        Catch ex As Exception
            context.Response.Write(FormatHTMLMessage(errorStr, invalidIdStr, "", "888888", True))
            context.Response.End()
        End Try

        Dim isAdmin As Boolean = False
        If Not GetCookie(context, "admin") = "" Then isAdmin = CBool(GetCookie(context, "admin"))

        If postId > 0 Then
            Dim postData As WPost = FetchPostData(postId)
            Dim pageHTML As String = editPostPageTemplate
            pageHTML = pageHTML.Replace("%BTITLE%", BoardTitle) _
            .Replace("%BDESC%", BoardDesc).Replace("%ROOT%", WebRoot) _
            .Replace("%FOOTER TEXT%", footerText) _
            .Replace("%ID%", CStr(postId)) _
            .Replace("%POST TEXT%", HttpUtility.HtmlDecode(postData.comment).Replace("</textarea>", "")) 'If the post text contain </textarea>, I remove it.
            If CStr(context.Request("shw")) = "yes" Then
                pageHTML = pageHTML.Replace("%SHOWHTMLWARNING%", "<span style='color:red'>HTML is Allowed</span><br/>")
            Else
                pageHTML = pageHTML.Replace("%SHOWHTMLWARNING%", "")
            End If
            If isAdmin Then
                pageHTML = pageHTML.Replace("%formname%", "adminaction")
            Else
                pageHTML = pageHTML.Replace("%formname%", "modaction")
            End If
            context.Response.Write(pageHTML)
        Else
            context.Response.Write(FormatHTMLMessage(errorStr, invalidIdStr, "", "888888", True))
            context.Response.End()
        End If
    End Sub

    Private Function _GetLocalisedBanReason(ByVal e As String) As String
        Dim a As String = ""
        For Each x In modBanReasons
            Dim b As String() = x.Split(CChar("$"))
            If b(0) = e Then a = b(4)
        Next
        Return a
    End Function

    'Private Function GenerateCatalogItems(ByVal ids As Integer()) As String
    '    Dim sb As New StringBuilder
    '    Dim data As WPost() = GetWpostList(ids)
    '    sb.Append("<div align=""center"" id=""threads"">")
    '    For Each thread As WPost In data
    '        Dim t As String = CatalogItemTemplate
    '        Dim i As WPostImage = thread.files(0)
    '        Dim ci As ThreadReplies = GetRepliesCount(CInt(thread.PostID), False)
    '        t = t.Replace("%ID%", CStr(thread.PostID))
    '        t = t.Replace("%POST LINK%", "default.aspx?id=" & CStr(thread.PostID))
    '        t = t.Replace("%THUMB SRC%", GetImageWEBPATHRE(i.ChanbName))
    '        t = t.Replace("%IMAGE MD5%", i.MD5)
    '        t = t.Replace("%TC%", CStr(ci.TextReplies))
    '        t = t.Replace("%IC%", CStr(ci.ImageReplies))
    '        t = t.Replace("%POST TEXT%", ProcessInputs(thread.comment))
    '        sb.Append(t)
    '    Next
    '    sb.Append("</div>")
    '    Return sb.ToString
    'End Function

    Private Function GenerateCatalogItems(ByVal ids As Integer()) As String
        Dim sb As New StringBuilder
        Dim data As WPost() = GetWpostList(ids)


        '<li>
        '				<a href="%POST LINK%" title="%SHORT TITLE%">
        '					<div class="icon"><img class="t" src="%THUMB SRC%"/></div>
        '					<h2>R: <strong>%TC%</strong> / I: <strong>%IC%</strong></h2>
        '					<p class="info">%POST TEXT%</p>
        '                	<p class="website">View thread</p></a>


        '                	</li
        sb.Append("<ul id=""browserlist"" class='wrap'>")

        For Each thread As WPost In data
            sb.Append("<li>")

            Dim i As WPostImage = Nothing
            If thread.FileCount > 0 Then
                i = thread.files(0)
            End If
            
            Dim ci As ThreadReplies = GetRepliesCount(CInt(thread.PostID), False)

            Dim postTitle As String = ""
            If thread.subject = "" Then
                If thread.comment = "" Then

                Else
                    If thread.comment.Length > 50 Then
                        postTitle = New String(CType(ProcessInputs(thread.comment), Char()), 0, 50) & "..."
                    Else
                        postTitle = thread.comment
                    End If
                End If
            Else
                postTitle = thread.subject
            End If

            sb.Append("<a href=""%POST LINK%"">".Replace("%POST LINK%", WebRoot & "default.aspx?id=" & CStr(thread.PostID)))


            If i IsNot Nothing Then
                If FileHasThumb(i.Extension) Then
                    sb.Append("<div class=""icon""><img class=""t"" src=""$""/></div>".Replace("$", i.ImageThumbailWebPath))
                Else
                    sb.Append("<div class=""icon""><h2>No image</h2></div>")
                End If
            End If
           
            If ci.TotalReplies > 0 Then

                sb.Append("<h2>")

                If ci.TextReplies > 0 Then
                    sb.Append("R: <strong>" & CStr(ci.TextReplies) & "</strong>")
                End If

                If ci.TextReplies > 0 And ci.ImageReplies > 0 Then
                    sb.Append("/")
                End If


                If ci.ImageReplies > 0 Then
                    sb.Append("I: <strong>" & CStr(ci.ImageReplies) & "</strong>")
                End If

                sb.Append("</h2>")
            Else

                sb.Append("<h2>No replies</h2>")
            End If

          
            If postTitle.Length > 0 Then
                sb.Append("<p class=""info"">$</p>".Replace("$", postTitle))

            End If
         

            sb.Append("<p class=""website"">View thread</p></a></li>")

        Next
        sb.Append("</ul>")
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
        Dim queryString As String = "SELECT type, time, comment, postername, email, password, parentT, subject, IP, ua, posterID, sticky, locked, mta, hasFile FROM  board  WHERE (id = " & id & ")"
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
            po.ip = CStr(ConvertNoNull(reader(8)))
            po.ua = CStr(ConvertNoNull(reader(9)))
            po.posterID = CStr(ConvertNoNull(reader(10)))
            po.isSticky = reader.GetBoolean(11)
            po.locked = reader.GetBoolean(12)
            po.archived = reader.GetBoolean(13)
            po.HasFile = reader.GetBoolean(14)
        End While

        queryObject.Reader.Close()
        If po.HasFile Then
            po.files = GetPostFiles(id, queryObject.Connection)
        End If
        queryObject.Connection.Close()
        Return po
    End Function

    Function MakeThread(ByVal data As OPData) As Integer
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand()

        command.CommandText = "INSERT INTO board (type, time, comment, postername, email, password, subject, IP, bumplevel, ua, sticky, mta, locked, hasFile) " & _
        "  VALUES (@type, @time, @comment, @postername, @email, @password, @subject, @IP, @bumplevel, @ua, @sticky, @mta, @locked, @hasFile) ; SELECT ID FROM board WHERE (time = @time) AND (IP = @IP)"

        command.Parameters.Add(DatabaseEngine.MakeParameter("@type", 0, System.Data.DbType.Int32)) ' Set post type to thread
        command.Parameters.Add(DatabaseEngine.MakeParameter("@time", data.time, System.Data.DbType.DateTime))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@comment", data.Comment, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@postername", data.name, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@email", data.email, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@password", data.password, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@subject", data.subject, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@IP", data.IP, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@bumplevel", Now, System.Data.DbType.DateTime))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@ua", data.UserAgent, System.Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@sticky", False, System.Data.DbType.Boolean))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@mta", False, System.Data.DbType.Boolean))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@locked", False, System.Data.DbType.Boolean))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@hasFile", data.HasFile, System.Data.DbType.Boolean))

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
    Private Function ReplyTo(ByVal id As Integer, ByVal data As OPData) As Integer
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand

        command.CommandText = "INSERT INTO board (type, time, comment, postername, email, password, parentT, subject, IP, ua, posterID, mta, locked, sticky, hasFile) VALUES" & _
                                "(@type, @time, @comment, @postername, @email, @password, @parentT, @subject, @IP, @ua, @posterId, @mta, @locked, @sticky, @hasFile) ; SELECT ID FROM board WHERE (time = @time) AND (IP = @IP)"


        command.Parameters.Add(DatabaseEngine.MakeParameter("@type", 1, System.Data.DbType.Int32)) ' Mark the post as a reply

        command.Parameters.Add(DatabaseEngine.MakeParameter("@parentT", CInt(id), System.Data.DbType.Int32)) ' Set the post owner thread

        'Insert Post data

        command.Parameters.Add(DatabaseEngine.MakeParameter("@time", data.time, System.Data.DbType.DateTime))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@comment", CStr(data.Comment), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@postername", CStr(data.name), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@email", CStr(data.email), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@password", CStr(data.password), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@subject", CStr(data.subject), System.Data.DbType.String))

        command.Parameters.Add(DatabaseEngine.MakeParameter("@IP", CStr(data.IP), System.Data.DbType.String)) '

        command.Parameters.Add(DatabaseEngine.MakeParameter("@ua", CStr(data.UserAgent), System.Data.DbType.String)) '

        command.Parameters.Add(DatabaseEngine.MakeParameter("@posterId", GenerateUID(id, data.IP), System.Data.DbType.String)) '

        command.Parameters.Add(DatabaseEngine.MakeParameter("@mta", False, System.Data.DbType.Boolean)) ' Mark the post as not archived
        command.Parameters.Add(DatabaseEngine.MakeParameter("@locked", False, System.Data.DbType.Boolean))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@sticky", False, System.Data.DbType.Boolean))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@hasFile", data.HasFile, System.Data.DbType.Boolean))

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)

        Dim postID As Integer
        While query.Reader.Read
            postID = CInt(query.Reader(0))
        End While

        query.Reader.Close()
        query.Connection.Close()
        command.Dispose()
        Return postID
    End Function

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
            Dim sb As New StringBuilder
            sb.Append("WHERE (ID = " & CStr(id(0)) & ")")

            For i As Integer = 1 To id.Length - 1 Step 1
                sb.Append(" OR (ID = " & id(i) & ") ")
            Next

            Dim queryString As String = "SELECT ID, type, time, comment, postername, email, password, parentT, subject, IP, ua, posterID, sticky, locked, mta, hasFile FROM  board  " & sb.ToString & " ORDER BY ID ASC"

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
                po.ip = CStr(ConvertNoNull(query.Reader(9)))
                po.ua = CStr(ConvertNoNull(query.Reader(10)))
                po.posterID = CStr(ConvertNoNull(query.Reader(11)))
                po.isSticky = query.Reader.GetBoolean(12)
                po.locked = query.Reader.GetBoolean(13)
                po.archived = query.Reader.GetBoolean(14)
                po.HasFile = query.Reader.GetBoolean(15)
                il.Add(po)
            End While
            query.Reader.Close()

            For Each po As WPost In il
                If po.HasFile Then
                    po.files = GetPostFiles(po.PostID, query.Connection)
                End If
            Next

            query.Connection.Close()
            Return il.ToArray
        End If
    End Function

    Function GetThreadData(ByVal threadID As Integer, ByVal includeArchivedPosts As Boolean) As WPost()
        Dim il As New List(Of WPost)
        Dim queryString As String = ""
        If includeArchivedPosts Then
            queryString = "SELECT ID, type, time, comment, postername, email, password, parentT, subject, IP, ua, posterID, sticky, locked, mta, hasFile FROM  board  WHERE (ID = " & threadID & ") OR (parentT = " & threadID & ") ORDER BY ID"
        Else
            queryString = "SELECT ID, type, time, comment, postername, email, password, parentT, subject, IP, ua, posterID, sticky, locked, mta, hasFile FROM  board  WHERE (ID = " & threadID & ") OR (parentT = " & threadID & ") AND (mta = 0) ORDER BY ID"
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
            po.ip = CStr(ConvertNoNull(query.Reader(9)))
            po.ua = CStr(ConvertNoNull(query.Reader(10)))
            po.posterID = CStr(ConvertNoNull(query.Reader(11)))
            po.isSticky = query.Reader.GetBoolean(12)
            po.locked = query.Reader.GetBoolean(13)
            po.archived = query.Reader.GetBoolean(14)
            po.HasFile = query.Reader.GetBoolean(15)
            il.Add(po)
        End While

        query.Reader.Close()

        'OP post is at index 0.

        '  Dim a As New ThreadReplies

        For i As Integer = 0 To il.Count - 1 Step 1
            If il(i).HasFile Then
                il(i).files = GetPostFiles(il(i).PostID, query.Connection)
                '      a.ImageReplies += 1
            Else
                '     a.TextReplies += 1
            End If
        Next
        ' il(0).replyCount = a
        query.Connection.Close()
        Return il.ToArray
    End Function

    Function GetThreads(ByVal startIndex As Integer, ByVal count As Integer, ByVal ignoreStickies As Boolean, ByVal arhive As Boolean) As Integer()
        If Not arhive Then
            Dim ila As New List(Of Integer)

            Dim query As New ChanbQuery

            If Not ignoreStickies Then
                Dim stickiesQueryStr As String = "SELECT ID FROM board  WHERE (type = 0) AND (sticky = 1) AND (mta = 0) ORDER BY ID DESC"
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

            If query.Connection Is Nothing Then query = DatabaseEngine.ExecuteQueryReader(queryString) Else query = DatabaseEngine.ExecuteQueryReader(queryString, query.Connection)

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

    Public Function GetLoginInfo(ByVal name As String, ByVal password As String) As LoginInfo
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand("SELECT password, power FROM mods WHERE (username LIKE @username)")
        command.Parameters.Add(MakeParameter("@username", name, Data.DbType.String))
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
        Dim sqlPassMd5 As String = ""
        Dim power As String = ""
        While query.Reader.Read
            sqlPassMd5 = CStr(ConvertNoNull(query.Reader(0)))
            power = CStr(ConvertNoNull(query.Reader(1)))
        End While
        query.Connection.Close()
        Dim lgi As New LoginInfo
        lgi.LogInValid = (MD5(password) = sqlPassMd5)
        If power = "admin" Then
            lgi.AccountType = LoginInfo.ChanbAccountType.Administrator
        Else
            lgi.AccountType = LoginInfo.ChanbAccountType.Moderator
        End If
        lgi.Powers = power
        Return lgi
    End Function

    Public Sub NewMod(ByVal name As String, ByVal pas As String, Optional ByVal powers As String = "")
        If powers = "" Then powers = DefaultModPowers
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "INSERT INTO mods (username, password, power) VALUES (@username, @pass, @powers)"
        command.Parameters.Add(DatabaseEngine.MakeParameter("@username", name, Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@pass", MD5(pas), Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@powers", powers, Data.DbType.String))
        DatabaseEngine.ExecuteNonQuery(command)
    End Sub

    Public Sub DeleteMod(ByVal name As String)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "DELETE FROM mods WHERE (username LIKE @username)"
        command.Parameters.Add(DatabaseEngine.MakeParameter("@username", name, Data.DbType.String))
        DatabaseEngine.ExecuteNonQuery(command)
    End Sub

    Public Sub UpdateMod(ByVal name As String, ByVal updatetype As Integer, ByVal newValue As String)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand()
        Select Case updatetype
            Case 1
                command.CommandText = "UPDATE mods SET password = @password WHERE (username = @username)"
                command.Parameters.Add(DatabaseEngine.MakeParameter("@username", name, Data.DbType.String))
                command.Parameters.Add(DatabaseEngine.MakeParameter("@password", MD5(newValue), Data.DbType.String))
            Case 2
                If IsModPowersValid(newValue) Then
                    command.CommandText = "UPDATE mods SET powers = @powers WHERE (username = @username)"
                    command.Parameters.Add(DatabaseEngine.MakeParameter("@username", name, Data.DbType.String))
                    command.Parameters.Add(DatabaseEngine.MakeParameter("@powers", (newValue), Data.DbType.String))
                Else
                    command.Dispose()
                    Throw New ArgumentException("Cannot update mod powers, bad powers")
                End If
            Case Else
                command.Dispose()
                Return
        End Select
        DatabaseEngine.ExecuteNonQuery(command)
    End Sub

    Private Function IsModPowersValid(ByVal d As String) As Boolean
        If d.Split(CChar("-")).Length = DefaultModPowers.Split(CChar("-")).Length Then
            For Each x In d.Split(CChar("-"))
                If Not (x = "0" Or x = "1") Then
                    Return False
                End If
            Next
            Return True
        Else
            Return False
        End If
    End Function

    Public Function IsIPBanned(ByVal IP As String) As Boolean
        Dim dic As New ValuesStore(banFile)
        If dic.KeyExist(IP) Then
            Dim data As String() = dic.GetKey(IP).Split(CChar(":"))
            Dim effectiveInKey As Boolean = CBool(data(0))
            If effectiveInKey Then
                'Check if the ban has expired.
                Dim b As BanData = GetBanData(IP)
                If b.BanEffective Then

                    If b.Permanant Then
                        Return True
                    Else
                        If Now >= b.ExpirationDate Then
                            DropBan(IP)
                            Return False
                        Else
                            Return True
                        End If
                    End If

                Else
                    DropBan(IP) 'fix a bug where a ban is not dropped from the ban file if the database record was deleted manually.
                    Return False
                End If
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Private Function CanIPBrowse(ByVal IP As String) As Boolean
        'Dim b As BanData = GetBanData(IP)
        'If b.BanEffective Then
        '    Return b.CanBrowse
        'Else
        '    Return True
        'End If
        Dim dic As New ValuesStore(banFile)
        If dic.KeyExist(IP) Then
            Dim data As String() = dic.GetKey(IP).Split(CChar(":"))
            Dim banned As Boolean = CBool(data(0))
            Dim canBr As Boolean = CBool(data(1))
            If banned Then
                Return canBr
            Else
                Return True
            End If
        Else
            Return True
        End If
    End Function

    Private Sub DropBan(ByVal IP As String)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand("UPDATE bans SET effective = @p WHERE (IP LIKE @ip)")
        command.Parameters.Add(MakeParameter("@p", False, Data.DbType.Boolean))
        command.Parameters.Add(MakeParameter("@ip", IP, Data.DbType.String))
        DatabaseEngine.ExecuteNonQuery(command)
        RemoveIP_FromBanFile(IP)
    End Sub

    Private Function GetBanData(ByVal IP As String) As BanData
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "SELECT ID, perm, expiry, comment, post, canview, modname, bannedon, effective FROM bans WHERE (IP LIKE @ip)"
        command.Parameters.Add(MakeParameter("@ip", IP, System.Data.DbType.String))
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
        Dim data As New BanData
        data.IP = IP

        While query.Reader.Read
            If TypeOf query.Reader(0) Is DBNull Then
                data.BanEffective = False
                data.BanID = -1
                data.CanBrowse = True
                data.Comment = ""
                data.ExpirationDate = Now - New TimeSpan(1, 0, 0, 0)
                data.BannedOn = Now - New TimeSpan(2, 0, 0, 0)
                data.PostNumber = -1
            Else
                data.BanID = CInt(ConvertNoNull(query.Reader(0)))
                data.Permanant = query.Reader.GetBoolean(1)
                data.ExpirationDate = CDate(ConvertNoNull(query.Reader(2)))
                data.Comment = CStr(ConvertNoNull(query.Reader(3)))
                data.PostNumber = CInt(ConvertNoNull(query.Reader(4)))
                data.CanBrowse = query.Reader.GetBoolean(5)
                data.ModeratorName = CStr(ConvertNoNull(query.Reader(6)))
                data.BannedOn = CDate(ConvertNoNull(query.Reader(7)))
                data.BanEffective = query.Reader.GetBoolean(8)
            End If
        End While

        query.Reader.Close()
        query.Connection.Close()
        Return data
    End Function

    Friend Sub UpdatePostText(ByVal postID As Integer, ByVal newText As String, ByVal allowHTML As Boolean)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "UPDATE board SET comment = @newtext WHERE (ID = @id)"
        command.Parameters.Add(DatabaseEngine.MakeParameter("@id", postID, Data.DbType.Int32))
        If Not allowHTML Then newText = ProcessInputs(newText)
        command.Parameters.Add(DatabaseEngine.MakeParameter("@newtext", newText, Data.DbType.String))
        DatabaseEngine.ExecuteNonQuery(command)
    End Sub

    Private Sub BanPoster(ByVal IP As String, ByVal postID As Integer, ByVal comment As String, ByVal modname As String, ByVal expirationdate As Date, ByVal permanant As Boolean, Optional ByVal CanBrowse As Boolean = True)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "INSERT INTO bans (perm, expiry, comment, post, IP, canview, modname, bannedon) VALUES " & _
                                " (@perm, @expiry, @comment, @post, @IP, @canview, @modname, @bannedon)"
        command.Parameters.Add(MakeParameter("@perm", permanant, Data.DbType.Boolean))
        command.Parameters.Add(MakeParameter("@expiry", expirationdate, Data.DbType.DateTime))
        command.Parameters.Add(MakeParameter("@comment", comment, Data.DbType.String))
        command.Parameters.Add(MakeParameter("@post", postID, Data.DbType.Int32))
        command.Parameters.Add(MakeParameter("@IP", IP, Data.DbType.String))
        command.Parameters.Add(MakeParameter("@canview", CanBrowse, Data.DbType.Boolean))
        command.Parameters.Add(MakeParameter("@modname", modname, Data.DbType.String))
        command.Parameters.Add(MakeParameter("@bannedon", Now, Data.DbType.DateTime))
        DatabaseEngine.ExecuteNonQuery(command)
        AddIP_ToBanfile(IP, CanBrowse)
    End Sub

    Private Sub AddIP_ToBanfile(ByVal ip As String, ByVal canbrowse As Boolean)
        Dim dic As New ValuesStore(banFile)
        dic.AddKey(ip, "True:" & CStr(canbrowse))
        dic.Save()
    End Sub

    Private Sub RemoveIP_FromBanFile(ByVal IP As String)
        Dim dic As New ValuesStore(banFile)
        If dic.KeyExist(IP) Then
            dic.AddKey(IP, "False:True")
            dic.Save()
        End If
    End Sub

    Public Sub UpdateBanFile()
        Dim dic As New ValuesStore(banFile)
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader("SELECT IP, effective, canview FROM bans")
        While query.Reader.Read
            dic.AddKey(query.Reader.GetString(0), CStr(query.Reader.GetBoolean(1)) & ":" & CStr(query.Reader.GetBoolean(2)))
        End While
        dic.Save()
        query.Reader.Close()
        query.Connection.Close()
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
        Dim textRepliesCount As DbCommand = DatabaseEngine.GenerateDbCommand("SELECT Count(ID) As T FROM board WHERE (parentT = @id)  AND (hasFile = @f) AND (mta = @mta)")
        Dim imageRepliesCount As DbCommand = DatabaseEngine.GenerateDbCommand("SELECT Count(ID) As T FROM board WHERE (parentT = @id) AND (hasFile = @f) AND (mta = @mta)")

        textRepliesCount.Parameters.Add(MakeParameter("@mta", countArchived, Data.DbType.Boolean))
        textRepliesCount.Parameters.Add(MakeParameter("@id", threadID, Data.DbType.Int32))
        textRepliesCount.Parameters.Add(MakeParameter("@f", False, Data.DbType.Boolean))

        imageRepliesCount.Parameters.Add(MakeParameter("@mta", countArchived, Data.DbType.Boolean))
        imageRepliesCount.Parameters.Add(MakeParameter("@f", True, Data.DbType.Boolean))
        imageRepliesCount.Parameters.Add(MakeParameter("@id", threadID, Data.DbType.Int32))

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
        Dim stk As Boolean = Not IsSticky(threadID)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "UPDATE board SET sticky = @stk WHERE (ID = @id )"
        command.Parameters.Add(MakeParameter("@stk", stk, Data.DbType.Boolean))
        command.Parameters.Add(MakeParameter("@id", threadID, Data.DbType.Int32))
        DatabaseEngine.ExecuteNonQuery(command)
    End Sub

    Private Function IsSticky(ByVal id As Integer) As Boolean
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader("SELECT sticky FROM board  WHERE (ID = " & id & " )")
        Dim p As Boolean
        While query.Reader.Read
            p = query.Reader.GetBoolean(0)
        End While
        query.Connection.Close()
        Return p
    End Function

    Public Sub ToggleLock(ByVal threadID As Integer)
        Dim lck As Boolean = Not IsLocked(threadID)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "UPDATE board SET locked = @lck WHERE (ID = @id )"
        command.Parameters.Add(MakeParameter("@lck", lck, Data.DbType.Boolean))
        command.Parameters.Add(MakeParameter("@id", threadID, Data.DbType.Int32))
        DatabaseEngine.ExecuteNonQuery(command)
    End Sub

    Private Function IsLocked(ByVal id As Integer) As Boolean
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader("SELECT locked FROM board WHERE (ID = " & id & ")")
        Dim p As Boolean
        While query.Reader.Read
            p = query.Reader.GetBoolean(0)
        End While
        query.Connection.Close()
        Return p
    End Function

    Private Function IsArchived(ByVal id As Integer) As Boolean
        Dim queryString As String = "SELECT mta FROM board WHERE (ID = " & id & " )"
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(queryString)
        Dim p As Boolean
        While query.Reader.Read
            p = query.Reader.GetBoolean(0)
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

    Private Sub ReportPost(ByVal id As Integer, ByVal reporterIP As String, ByVal time As Date, ByVal reason As String)
        Dim command As DbCommand = DatabaseEngine.GenerateDbCommand
        command.CommandText = "INSERT INTO reports (postID, reporterIP, time, comment) VALUES (@id, @reporterIP, @time, @comment)"
        command.Parameters.Add(DatabaseEngine.MakeParameter("@id", id, Data.DbType.Int32))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@reporterIP", reporterIP, Data.DbType.String))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@time", time, Data.DbType.DateTime))
        command.Parameters.Add(DatabaseEngine.MakeParameter("@comment", reason, Data.DbType.String))
        DatabaseEngine.ExecuteNonQuery(command)
    End Sub

    Friend Function GetReports(ByVal start As Integer, ByVal endo As Integer) As Report()
        Dim il As New List(Of Report)

        '    Dim command As ChanbQuery = DatabaseEngine.ExecuteQueryReader("SELECT reporterIP, ID, postID, time, comment FROM reports")

        Dim commandText As String = ""

        Select Case dbType
            Case "mssql"
                commandText = "SELECT * FROM ( SELECT TOP " & CStr(endo) & " reporterIP, ID, postID, time, comment FROM ( SELECT TOP " & CStr(start + endo) & " reporterIP, ID, postID, time, comment FROM reports ORDER BY ID Asc) as newtbl order by ID desc) as newtbl2 order by ID asc"
            Case "mysql"
                commandText = "SELECT reporterIP, ID, postID, time, comment FROM reports LIMIT " & CStr(start) & "," & CStr(endo)
        End Select
        Dim command As ChanbQuery = DatabaseEngine.ExecuteQueryReader(commandText)



        While command.Reader.Read
            Dim a As New Report(command.Reader.GetString(0), command.Reader.GetInt32(1))
            a.PostID = command.Reader.GetInt32(2)
            a.Time = command.Reader.GetDateTime(3)
            a.ReportComment = command.Reader.GetString(4)
            il.Add(a)
        End While

        command.Reader.Close()
        command.Connection.Close()
        Return il.ToArray

    End Function

#End Region

#Region "Misc Functions"

    Public Function GetMimeType(ByVal fileExtension As String) As String
        Select Case fileExtension.ToUpper
            Case "JPG"
                Return "image/jpeg"
            Case "JPEG"
                Return "image/jpeg"
            Case "BMP"
                Return "image/bmp"
            Case "GIF"
                Return "image/gif"
            Case "PNG"
                Return "image/png"
            Case "SVG"
                Return "image/svg+xml"
            Case "PDF"
                Return "application/acrobat"
            Case "WEBM"
                Return "video/webm"
            Case "OGG"
                Return "audio/ogg"
            Case "MP3"
                Return "audio/mpeg"
            Case Else
                Return "application/octet-stream"
        End Select
    End Function

    Private Sub Delete_File_Fromdisk(ByVal path As String)
        If IO.File.Exists(path) Then IO.File.Delete(path)
    End Sub

    Private Function ConvertNoNull(ByVal x As Object) As Object
        If TypeOf x Is DBNull Then Return Nothing Else Return x
    End Function

    Private Function ProcessInputs(ByVal str As String) As String
        Dim sb As New StringBuilder
        For i As Integer = 0 To str.Length - 1 Step 1
            sb.Append(ToLatin(str(i)))
        Next
        Return HttpUtility.HtmlEncode(sb.ToString)
    End Function

    Private Function ToLatin(ByVal c As String) As String
        Select Case c
            Case "О" 'cyrillic 
                Return "O"
            Case "о" 'cyrillic
                Return "o"
            Case "ס" 'hebrew, sometimes this looks like latin o
                Return "o"
            Case "А" 'cyr
                Return "A"
            Case "а" 'cyr
                Return "a"
            Case "В" 'cyr
                Return "B"
            Case "Β" 'greek
                Return "B"
            Case "Е" 'cyr
                Return "E"
            Case "е" 'cyr
                Return "e"
            Case "Ε" 'greek
                Return "E"
            Case "Ѕ" 'cyr
                Return "S"
            Case "ѕ" 'cyr
                Return "s"
            Case "І" 'cyr
                Return "I"
            Case "Ӏ" 'cyr palochka
                Return "I"
            Case "і" 'cyr
                Return "i"
            Case "Ј" 'cyr
                Return "J"
            Case "ј" 'cyr
                Return "j"
            Case "Ο" ' greek
                Return "O"
            Case "ο" 'greek
                Return "o"
            Case "Κ" 'greek
                Return "K"
            Case "M" 'cyr
                Return "M"
            Case "H" 'cyr
                Return "H"
            Case "Р" 'cyr
                Return "P"
            Case "р" 'cyr
                Return "p"
            Case "С"
                Return "C"
            Case "с"
                Return "c"
            Case "Т" 'cyr
                Return "T"
            Case "Τ" 'greek
                Return "T"
            Case "у" 'cyr
                Return "y"
            Case "Υ" ' greek
                Return "Y"
            Case "Ү" 'cyr
                Return "Y"
            Case "Ь" 'cyr
                Return "b"
            Case "Х" 'cyr
                Return "X"
            Case "х" 'cyr
                Return "x"
            Case "ҫ" 'cyr
                Return "ç"
            Case "Ҫ" 'cyr
                Return "Ç"
            Case "Ԛ" 'cyr
                Return "Q"
                ''Not sure about these
            Case "һ" 'cyr
                Return "h"
            Case "ԛ" 'cyr
                Return "q"
            Case Else
                Return c
        End Select
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
        Dim KB As Long = CLng(Fix(size / 1024))
        Dim MB As Long = CLng(Fix(size / 1048576))
        Dim GB As Long = CLng(Fix(size / 1073741824))
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

    Friend Function GetTimeString(ByVal d As Date) As String
        'ISO 8601 date time format
        Return d.Year & "-" & d.Month & "-" & d.Day & " " & d.Hour & ":" & d.Minute & ":" & d.Second
    End Function

#End Region

#Region "Image processing"

    Private Function ResizeImage(ByVal i As Drawing.Image, ByVal targetMax As Integer) As Drawing.Image
        Dim thumbSize As Drawing.Size = New Drawing.Size(targetMax, CInt(Fix(i.Height / (i.Width / targetMax))))
        Dim thumbImage As Drawing.Image

        Select Case ResizeMethode
            Case 0 'naive quality
                thumbImage = i.GetThumbnailImage(thumbSize.Width, thumbSize.Height, Nothing, System.IntPtr.Zero)
            Case 1 ' Medium Quality 
                Dim bi As New Drawing.Bitmap(thumbSize.Width, thumbSize.Height)
                Dim g As Drawing.Graphics = Drawing.Graphics.FromImage(bi)
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.Bicubic
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighQuality
                g.PixelOffsetMode = Drawing.Drawing2D.PixelOffsetMode.HighQuality
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighQuality

                Dim imageRectangle As Drawing.Rectangle = New Drawing.Rectangle(0, 0, thumbSize.Width, thumbSize.Height)
                g.DrawImage(i, imageRectangle)
                g.Dispose()
                thumbImage = bi
            Case 2 'Fastest Methode
                Dim bi As New Drawing.Bitmap(thumbSize.Width, thumbSize.Height)
                Dim g As Drawing.Graphics = Drawing.Graphics.FromImage(bi)
                g.InterpolationMode = Drawing.Drawing2D.InterpolationMode.Default
                g.SmoothingMode = Drawing.Drawing2D.SmoothingMode.HighSpeed
                g.PixelOffsetMode = Drawing.Drawing2D.PixelOffsetMode.HighSpeed
                g.CompositingQuality = Drawing.Drawing2D.CompositingQuality.HighSpeed
                Dim imageRectangle As Drawing.Rectangle = New Drawing.Rectangle(0, 0, thumbSize.Width, thumbSize.Height)
                g.DrawImage(i, imageRectangle)
                g.Dispose()
                thumbImage = bi
            Case Else
                thumbImage = i.GetThumbnailImage(thumbSize.Width, thumbSize.Height, Nothing, System.IntPtr.Zero)
        End Select

        If thumbImage Is Nothing Then thumbImage = i.GetThumbnailImage(thumbSize.Width, thumbSize.Height, Nothing, System.IntPtr.Zero)

        Try
            If CheckEXIFOrientation Then
                Dim fs As New IO.MemoryStream
                i.Save(fs, i.RawFormat)

                Dim orientation As Integer = GetImageOrientation(fs)
                fs.Dispose()

                Select Case orientation
                    Case 1
                        Exit Select
                    Case 2
                        thumbImage.RotateFlip(Drawing.RotateFlipType.RotateNoneFlipX)
                    Case 3
                        thumbImage.RotateFlip(Drawing.RotateFlipType.Rotate180FlipNone)
                    Case 4
                        thumbImage.RotateFlip(Drawing.RotateFlipType.RotateNoneFlipY)
                    Case 5
                        thumbImage.RotateFlip(Drawing.RotateFlipType.Rotate90FlipY)
                    Case 6
                        thumbImage.RotateFlip(Drawing.RotateFlipType.Rotate90FlipNone)
                    Case 7
                        thumbImage.RotateFlip(Drawing.RotateFlipType.Rotate90FlipX)
                    Case 8
                        thumbImage.RotateFlip(Drawing.RotateFlipType.Rotate270FlipNone)
                    Case Else
                        Exit Select
                End Select
            End If
        Catch ex As Exception
        End Try
        Return thumbImage
    End Function

    Private Function GetImageOrientation(ByVal fs As IO.Stream) As Integer
        fs.Seek(0, IO.SeekOrigin.Begin)
        Dim orient As New Object
        Try
            Dim exR As New ExifLib.ExifReader(fs)
            exR.GetTagValue(ExifLib.ExifTags.Orientation, orient)
            exR.Dispose()
        Catch ex As Exception
        End Try
        If orient Is Nothing Then Return 1 Else Return CInt(orient)
    End Function

#End Region

#Region "BB codes processing"

    Private Function MatchAndProcessBBCodes(ByVal codename As String, ByVal data As String) As String
        Select Case codename
            Case "spoiler"
                If data.Contains("[spoiler]") And data.Contains("[/spoiler]") Then
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
                        For Each x In MatchBBCode(data, "code")
                            Dim codeStr As String = x
                            Dim codeLang As String = GetCodeLang(codeStr)
                            Dim prC As String = get_pr_lang(codeLang)
                            If Not (codeLang = "") Then codeStr = codeStr.Replace("[lang]" & codeLang & "[/lang]", String.Empty).Trim()
                            If (prC = "") Then
                                data = data.Replace("[code]" & x & "[/code]", "<pre class='prettyprint linenums'>" & codeStr & "</pre>")
                            Else
                                data = data.Replace("[code]" & x & "[/code]", "<script src='" & WebRoot & "js/pr/lang-" & prC & ".js'></script><pre class='prettyprint linenums lang-" & prC & "'>" & codeStr & "</pre>")
                            End If
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
                        mdt = mdt.Replace("<br/>", String.Empty)
                        data = data.Replace("[md]" & x & "[/md]", md.Transform(mdt))
                    Next
                    Return data
                Else
                    Return data
                End If
            Case "q"
                If data.Contains("[q]") And data.Contains("[/q]") Then
                    For Each x In MatchBBCode(data, "q")
                        data = data.Replace("[q]" & x & "[/q]", "<tt class=""tt"">" & x & "</tt>")
                    Next
                    Return data
                Else
                    Return data
                End If
                'Case "backlink"
                '    Dim re As New Regex("/&gt;&gt;([0-9]+)/")
                '    For Each x In MatchBBCode(data, re)
                '        data = data.Replace(x, formatBacklink("#" & x.Replace("&gt;&gt;", ""), x))
                '    Next
                '    Return data
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

    Private Function get_pr_lang(ByVal l As String) As String
        Select Case l.ToLower.Trim
            Case "scheme", "scm"
                Return "scm"
            Case "lsp", "lisp"
                Return "lisp"
            Case "apollo", "agc", "aea"
                Return "apollo"
            Case "basic", "cbm"
                Return "basic"
            Case "scala"
                Return "scala"
            Case "pascal"
                Return "pascal"
            Case "vb", "vbs", "vb.net"
                Return "vb"
            Case "go"
                Return "go"
            Case "haskel", "hs"
                Return "hs"
            Case "erlang", "erl"
                Return "erlang"
            Case "clojure", "clj"
                Return "clj"
            Case "css"
                Return "css"
            Case "lua"
                Return "lua"
            Case "c", "cpp", "cplusplus"
                Return ""
            Case "ocaml", "SML", "F#"
                Return "ml"
            Case "sql"
                Return "sql"
            Case Else
                Return ""
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


    'Private Function MatchBBCode(ByVal text As String, ByVal Regexp As Regex) As String()
    '    Dim il As New List(Of String)
    '    Dim st As String() = Regexp.Split(text)
    '    Dim isAmatch As Boolean = False
    '    For i As Integer = 0 To st.Length - 1 Step 1
    '        If Not isAmatch Then
    '            'Not a match 
    '        Else
    '            il.Add(st.ElementAt(i))
    '        End If
    '        isAmatch = Not isAmatch
    '    Next
    '    Return il.ToArray
    'End Function

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

    'Private Function GetCCLI(ByVal lang As String) As ColorCode.ILanguage
    '    Select Case lang
    '        Case "asax"
    '            Return ColorCode.Languages.Asax
    '        Case "ashx"
    '            Return ColorCode.Languages.Ashx
    '        Case "aspx"
    '            Return ColorCode.Languages.Aspx
    '        Case "aspxcs"
    '            Return ColorCode.Languages.AspxCs
    '        Case "aspxvb"
    '            Return ColorCode.Languages.AspxVb
    '        Case "cpp"
    '            Return ColorCode.Languages.Cpp
    '        Case "csharp"
    '            Return ColorCode.Languages.CSharp
    '        Case "css"
    '            Return ColorCode.Languages.Css
    '        Case "fsharp"
    '            Return ColorCode.Languages.FSharp
    '        Case "html"
    '            Return ColorCode.Languages.Html
    '        Case "java"
    '            Return ColorCode.Languages.Java
    '        Case "javascript"
    '            Return ColorCode.Languages.JavaScript
    '        Case "js"
    '            Return ColorCode.Languages.JavaScript
    '        Case "php"
    '            Return ColorCode.Languages.Php
    '        Case "ps"
    '            Return ColorCode.Languages.PowerShell
    '        Case "sql"
    '            Return ColorCode.Languages.Sql
    '        Case "typescript"
    '            Return ColorCode.Languages.Typescript
    '        Case "vbdotnet"
    '            Return ColorCode.Languages.VbDotNet
    '        Case "vb"
    '            Return ColorCode.Languages.VbDotNet
    '        Case "xml"
    '            Return ColorCode.Languages.Xml
    '        Case "c"
    '            Return ColorCode.Languages.Cpp
    '        Case Else
    '            Return ColorCode.Languages.Cpp
    '    End Select
    'End Function

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
            If IsDBNull(queryOjb.Reader(0)) Then b = False Else b = True
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
            pageHTML = pageHTML.Replace("%POSTDIVCLASS%", "hide")
            pageHTML = pageHTML.Replace("%POST FORM MODE%", "reply")
            pageHTML = pageHTML.Replace("%POSTING RULES%", "")
        Else
            pageHTML = pageHTML.Replace("%POSTING MODE%", postingModeReplyHtml)
            pageHTML = pageHTML.Replace("%POSTDIVCLASS%", "")
            pageHTML = pageHTML.Replace("%POST FORM MODE%", "thread")
            pageHTML = pageHTML.Replace("%POSTING RULES%", postingRulesHTML)
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

        pageHTML = pageHTML.Replace("%THREAD COUNT%", "")

        pageHTML = pageHTML.Replace("%EXTRA META%", "<META HTTP-EQUIV=""pragma"" CONTENT=""no-cache"">")

        pageHTML = pageHTML.Replace("%RETURN BUTTON DESKTOP%", DesktopReturnButtonHTML.Replace("%P%", WebRoot & pageHandlerLink & ".aspx"))
        pageHTML = pageHTML.Replace("%RETURN BUTTON MOBILE%", MobileReturnButtonHTML.Replace("%P%", WebRoot & pageHandlerLink & ".aspx"))

        '####################################### BODY PROCESSING CODE #######################################
        Dim body As New StringBuilder
        Dim para As New HTMLParameters()

        para.isCurrentThread = Not po.archived

        para.replyButton = False
        para.isTrailPost = False

        body.Append("<div class=""thread"" id=""t" & tid & """>")
        body.Append(GetThreadHTML(tid, para))
        body.Append("</div><hr />")

        pageHTML = pageHTML.Replace("%BODY%", body.ToString)

        pageHTML = pageHTML.Replace("%PAGES LIST%", "")

        Return pageHTML
    End Function

#End Region

#Region "Panel Functions"

    Public Function CountTotalPost() As Integer
        Dim command As String = "SELECT Count(ID) As T FROM board"
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
        Dim i As Integer = 0
        While query.Reader.Read
            i = CInt(ConvertNoNull(query.Reader(0)))
        End While
        query.Reader.Close()
        query.Connection.Close()
        Return i
    End Function

    Public Function CountTotalFiles() As Integer
        Dim command As String = "SELECT DISTINCT md5 FROM files"
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
        Dim i As Integer = 0
        While query.Reader.Read
            i += 1
        End While
        query.Reader.Close()
        query.Connection.Close()
        Return i
    End Function

    Public Function CountTotalUsers() As Integer
        Dim command As String = "SELECT DISTINCT IP FROM board"
        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
        Dim i As Integer = 0
        While query.Reader.Read
            i += 1
        End While
        query.Reader.Close()
        query.Connection.Close()
        Return i
    End Function

    Public Function GetChanbVersion() As String
        ' Return "0.5.0.153"
        Dim a As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly()
        Return a.FullName.Replace(", Culture=neutral, PublicKeyToken=null", "")
    End Function

#End Region

#Region "Files database processing"

    Private Sub AddFileToDatabase(ByVal wpi As WPostImage, ByVal postID As Integer, Optional ByRef connection As DbConnection = Nothing)
        Dim command As DbCommand
        If connection Is Nothing Then
            command = DatabaseEngine.GenerateDbCommand
        Else
            command = DatabaseEngine.GenerateDbCommand(connection)
        End If
        '; SELECT ID FROM files WHERE (postID = @postID) AND (md5 = @md5)

        command.CommandText = "INSERT INTO files (postID, chanbname, realname, size, md5, extension, mimetype, dimension) " & _
                            " VALUES  (@postID, @chanbname, @realname, @size, @md5, @extension, @mimetype, @dimension)"


        command.Parameters.Add(MakeParameter("@postID", postID, Data.DbType.Int32))
        command.Parameters.Add(MakeParameter("@chanbname", wpi.ChanbName, Data.DbType.String))
        command.Parameters.Add(MakeParameter("@realname", wpi.RealName, Data.DbType.String))
        command.Parameters.Add(MakeParameter("@size", wpi.Size, Data.DbType.Int64))
        command.Parameters.Add(MakeParameter("@md5", wpi.MD5, Data.DbType.String))
        command.Parameters.Add(MakeParameter("@extension", wpi.Extension, Data.DbType.String))
        command.Parameters.Add(MakeParameter("@mimetype", wpi.MimeType, Data.DbType.String))
        command.Parameters.Add(MakeParameter("@dimension", wpi.Dimensions, Data.DbType.String))

        If connection Is Nothing Then
            DatabaseEngine.ExecuteNonQuery(command)
        Else
            DatabaseEngine.ExecuteNonQuery(command, connection)
        End If


        'Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
        'Dim id As Integer = 0
        'While query.Reader.Read
        '    id = CInt(query.Reader(0))
        'End While
        'query.Reader.Close()
        'query.Connection.Close()
        'Return id
    End Sub

    Public Function GetFileDataByMD5(ByVal md5 As String, Optional ByVal connection As DbConnection = Nothing) As WPostImage
        Dim wpi As New WPostImage

        Dim command As DbCommand

        If connection Is Nothing Then
            command = GenerateDbCommand()
        Else
            command = GenerateDbCommand(connection)
        End If

        command.CommandText = "SELECT ID, postID, chanbname, realname, size, extension, mimetype, dimension FROM files WHERE (md5 = @md5)"

        command.Parameters.Add(MakeParameter("@md5", md5, Data.DbType.String))

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)

        While query.Reader.Read
            If IsDBNull(query.Reader(0)) Then
                wpi = dummyWImage
            Else
                wpi.PostID = CInt(query.Reader(1))
                wpi.ChanbName = CStr(query.Reader(2))
                wpi.RealName = CStr(query.Reader(3))
                wpi.Size = CLng(query.Reader(4))
                wpi.Extension = CStr(query.Reader(5)).ToUpper
                wpi.MimeType = CStr(query.Reader(6))
                wpi.Dimensions = CStr(query.Reader(7))
                wpi.MD5 = md5
            End If
        End While
        query.Reader.Close()
        If connection Is Nothing Then query.Connection.Close()
        Return wpi
    End Function

    Public Function GetFileDataByChanbName(ByVal chanbName As String) As WPostImage
        Dim wpi As New WPostImage

        Dim command As DbCommand = GenerateDbCommand()

        Select Case dbType
            Case "mssql"
                command.CommandText = "SELECT TOP 1 ID, postID, md5, realname, size, extension, mimetype, dimension FROM files WHERE (chanbname LIKE @chanbname)"
            Case "mysql"
                command.CommandText = "SELECT ID, postID, md5, realname, size, extension, mimetype, dimension FROM files WHERE (chanbname LIKE @chanbname) LIMIT 0,1"
            Case Else
                If isInstalled Then
                    Throw New Exception(dbTypeInvalid)
                Else
                    Return dummyWImage
                End If
        End Select

        command.Parameters.Add(MakeParameter("@chanbname", chanbName, Data.DbType.String))

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)

        While query.Reader.Read
            If IsDBNull(query.Reader(0)) Then
                wpi = dummyWImage
            Else
                wpi.PostID = CInt(query.Reader(1))
                wpi.MD5 = CStr(query.Reader(2))
                wpi.RealName = CStr(query.Reader(3))
                wpi.Size = CLng(query.Reader(4))
                wpi.Extension = CStr(query.Reader(5)).ToUpper
                wpi.MimeType = CStr(query.Reader(6))
                wpi.Dimensions = CStr(query.Reader(7))
                wpi.ChanbName = chanbName
            End If
        End While
        query.Reader.Close()
        query.Connection.Close()
        Return wpi
    End Function

    Public Function FileExistInDB(ByVal md5 As String, Optional ByVal connection As DbConnection = Nothing) As Boolean
        Dim wpi As New WPostImage

        Dim command As DbCommand
        If connection Is Nothing Then
            command = GenerateDbCommand()
        Else
            command = GenerateDbCommand(connection)
        End If

        command.CommandText = "SELECT ID FROM files WHERE (md5 = @md5)"

        command.Parameters.Add(MakeParameter("@md5", md5, Data.DbType.String))

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
        Dim p As Boolean = False

        While query.Reader.Read
            p = Not IsDBNull(query.Reader(0))
        End While

        query.Reader.Close()
        If connection Is Nothing Then query.Connection.Close()

        Return p
    End Function

    Public Function FileExistInDB(ByVal md5 As String, ByVal ExcludedPost As Integer) As Boolean
        Dim wpi As New WPostImage

        Dim command As DbCommand = GenerateDbCommand("SELECT ID FROM files WHERE (md5 = @md5) AND (postId <> @exP)")

        command.Parameters.Add(MakeParameter("@md5", md5, Data.DbType.String))
        command.Parameters.Add(MakeParameter("@exP", ExcludedPost, Data.DbType.Int32))

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)
        Dim p As Boolean = False

        While query.Reader.Read
            p = Not IsDBNull(query.Reader(0))
        End While

        query.Reader.Close()
        query.Connection.Close()

        Return p
    End Function

    Public Function GetPostFiles(ByVal postId As Integer, Optional ByVal connection As DbConnection = Nothing) As WPostImage()
        Dim command As DbCommand
        If connection Is Nothing Then
            command = DatabaseEngine.GenerateDbCommand
        Else
            command = DatabaseEngine.GenerateDbCommand(connection)
        End If

        command.CommandText = "SELECT ID, chanbname, realname, size, md5, extension, mimetype, dimension FROM files WHERE (postID = @postID)"

        command.Parameters.Add(MakeParameter("@postID", postId, Data.DbType.Int32))

        Dim il As New List(Of WPostImage)

        Dim query As ChanbQuery = DatabaseEngine.ExecuteQueryReader(command)

        While query.Reader.Read
            Dim wpi As New WPostImage
            wpi.PostID = postId
            wpi.ChanbName = CStr(query.Reader(1))
            wpi.RealName = CStr(query.Reader(2))
            wpi.Size = CLng(query.Reader(3))
            wpi.MD5 = CStr(query.Reader(4))
            wpi.Extension = CStr(query.Reader(5)).ToUpper
            wpi.MimeType = CStr(query.Reader(6))
            wpi.Dimensions = CStr(query.Reader(7))
            il.Add(wpi)
        End While

        query.Reader.Close()
        If connection Is Nothing Then query.Connection.Close()

        Return il.ToArray
        il.Clear()
    End Function


#End Region

End Module