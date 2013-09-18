Partial Public Class _img
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' img.aspx
        ' usage: 
        ' 1 - img.aspx?md5= image md5 , return image data with real image name.
        ' 2 - img.aspx?cn= chanb name & rn = real name, return image data with the specified real name.
        Session("chanb") = "chanb"
        If chanb.isInstalled Then
            With Context
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

                            .Response.AppendHeader("Content-Disposition", "attachment; filename=""" & RealName.Replace("""", "") & """")

                            .Response.AppendHeader("Content-Length", CStr(f.Length))

                            .Response.WriteFile(StorageFolder & "\" & ChanbName)

                            .Response.End()
                        Else
                            c404(.Response)
                        End If
                    End If
                Else

                    Dim md5 As String = .Request.Item("md5")
                    If FileExistInDB(md5) Then

                        Dim ImageData As WPostImage = GetFileDataByMD5(md5)

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
        Else
            Response.StatusCode = 404
            Response.Write("404")
        End If
    End Sub

    Private Sub c404(ByRef response As HttpResponse)
        response.StatusCode = 404
        response.Write("404")
        response.End()
    End Sub

End Class