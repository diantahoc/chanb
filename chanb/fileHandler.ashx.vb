Imports System.Web
Imports System.Web.Services

Public Class fileHandler
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            Dim url As String = context.Request.Url.LocalPath
            Dim ChanbName As String = url.Split(CChar("/")).ElementAt(url.Split(CChar("/")).Length - 1)
            With context
                If FileIO.FileSystem.FileExists(StorageFolder & "\" & ChanbName) Then
                    .Response.Clear()
                    .Response.ClearContent()
                    .Response.ClearHeaders()
                    .Response.BufferOutput = True

                    Dim f As IO.FileInfo = FileIO.FileSystem.GetFileInfo(StorageFolder & "\" & ChanbName)

                    Dim wpi As WPostImage = GetFileDataByChanbName(ChanbName)
                    Dim fileextension As String = ChanbName.Split(CChar(".")).ElementAt(ChanbName.Split(CChar(".")).Length - 1).ToUpper

                    If wpi._isDummy Then

                        .Response.AppendHeader("Content-Disposition", "inline; filename=""" & ChanbName & """")
                        .Response.ContentType = GetMimeType(fileextension)
                        .Response.WriteFile(StorageFolder & "\" & ChanbName)
                        .Response.End()

                    Else
                        .Response.ContentType = GetMimeType(fileextension)

                        .Response.AppendHeader("Content-Disposition", "inline; filename=""" & wpi.RealName & """")

                        .Response.AppendHeader("Content-Length", CStr(f.Length))

                        .Response.WriteFile(StorageFolder & "\" & ChanbName)

                        .Response.End()
                    End If

                Else
                    c404(.Response)
                End If
            End With
        Catch ex As Exception
            c404(context.Response)
        End Try      
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return True
        End Get
    End Property

    Private Sub c404(ByRef response As HttpResponse)
        response.StatusCode = 404
        response.End()
    End Sub
End Class