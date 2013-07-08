Public Module ImgPageHandler

    Public Sub Process(ByVal context As HttpContext)
        With context
            If .Request.Item("md5") = "" Then

                If .Request.Item("cn") = "" Or .Request.Item("rn") = "" Then ' cn is 'chanb name', rn is 'real name'
                    c404(.Response)
                Else

                    Dim ChanbName As String = .Request.Item("cn")
                    Dim RealName As String = .Request.Item("rn")

                    If FileIO.FileSystem.FileExists(StorageFolder & "\" & ChanbName) Then
                        .Response.Clear()
                        .Response.ClearContent()
                        .Response.ClearHeaders()
                        .Response.BufferOutput = True

                        Dim f As IO.FileInfo = FileIO.FileSystem.GetFileInfo(StorageFolder & "\" & ChanbName)

                        Dim fileextension As String = ChanbName.Split(CChar(".")).ElementAt(ChanbName.Split(CChar(".")).Length - 1).ToUpper

                        .Response.ContentType = GetMimeType(fileextension)

                        .Response.AppendHeader("Content-Disposition", "attachment; filename=""" & RealName & """")

                        .Response.AppendHeader("Content-Length", CStr(f.Length))

                        .Response.WriteFile(StorageFolder & "\" & ChanbName)

                        .Response.End()
                    Else
                        c404(.Response)
                    End If
                End If
            Else

                Dim md5 As String = .Request.Item("md5")
                If ImageExist(md5) Then

                    Dim ImageData As WPostImage = GetImageDataByMD5(md5)

                    If FileIO.FileSystem.FileExists(StorageFolder & "\" & ImageData.ChanbName) Then
                        .Response.Clear()
                        .Response.ClearContent()
                        .Response.ClearHeaders()
                        .Response.BufferOutput = True

                        Dim f As IO.FileInfo = FileIO.FileSystem.GetFileInfo(StorageFolder & "\" & ImageData.ChanbName)

                        .Response.ContentType = GetMimeType(ImageData.Extension)

                        .Response.AppendHeader("Content-Disposition", "inline; filename=""" & ImageData.RealName & """")

                        .Response.AppendHeader("Content-Length", CStr(f.Length))

                        .Response.WriteFile(StorageFolder & "\" & ImageData.ChanbName)

                        .Response.End()
                    Else
                        c404(.Response)
                    End If

                Else
                    c404(.Response)
                End If
            End If
        End With
    End Sub

    Private Function GetMimeType(ByVal fileExtension As String) As String
        Select Case fileExtension.ToUpper
            Case "JPG"
                Return "image/jpeg"
            Case "JPEG"
                Return "image/jpeg"
            Case "BMP"
                Return "image/bmp"
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

    Private Sub c404(ByRef response As HttpResponse)
        response.StatusCode = 404
        response.Write("404")
        response.End()
    End Sub


End Module
