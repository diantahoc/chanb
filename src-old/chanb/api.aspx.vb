Partial Public Class _api
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("chanb") = "chanb"
        Try
            Select Case Request.Item("mode")
                Case "fetchrepliesafter" ' tid = thread id , lp = last post 
                    Dim threadid As Integer = CInt(Request.Item("tid"))
                    Dim lastpost As Integer = CInt(Request.Item("lp"))
                    Dim pa As New HTMLParameters
                    pa.isCurrentThread = True
                    pa.isTrailPost = False
                    pa.isModerator = CBool(Session("mod"))
                    pa.isAdmin = CBool(Session("admin"))
                    pa.CredPowers = CStr(Session("credpower"))

                    pa.replyButton = False

                    Response.ContentType = "application/json"
                    Response.ContentEncoding = Text.Encoding.UTF8
                    Response.Write(WpostsToJsonList(GetThreadRepliesAfter(threadid, lastpost, False), pa))

                Case "getfileslist" ' tid = thread id , ft = file types jpg,bmp,list ...
                    Dim tid As Integer = CInt(Request.Item("tid"))
                    Dim allowedfiles As New List(Of String)
                    For Each x In Request.Item("ft").Split(CChar(","))
                        allowedfiles.Add(x)
                    Next
                    Response.ContentType = "application/json"
                    Response.ContentEncoding = Text.Encoding.UTF8
                    Response.Write(GetFileList(tid, allowedfiles.ToArray, False))
                Case "getthreadpagenumber" 'tid = thread id
                    Dim i As Integer = CInt(Request.Item("tid"))
                    Response.ContentType = "application/javascript"
                    Response.Write(GetThreadPageNumber(i))
                Case "getthreadposts" ' tid = thread id
                    Dim i As Integer = CInt(Request.Item("tid"))
                    Response.ContentType = "application/json"
                    Response.ContentEncoding = Text.Encoding.UTF8
                    Response.Write(GetThreadPosts(i))
                Case "getcatalog"
                    Response.ContentType = "application/json"
                    Response.ContentEncoding = Text.Encoding.UTF8
                    Response.Write(JSON_GetCatalog())
                Case "getcatalogids"
                    Response.ContentType = "application/json"
                    Response.ContentEncoding = Text.Encoding.UTF8
                    Response.Write(JSON_GetCatalog_ID_Only())

                    'get threads id for a specified page
                Case "getpagethreadsids" ' pn = page number, the first page is 1
                    Dim i As Integer = Math.Abs(CInt(Request.Item("pn")) - 1)
                    Response.ContentType = "application/json"
                    Response.ContentEncoding = Text.Encoding.UTF8
                    Response.Write(JSON_GetPageThreads_ID_Only(i))

                    'get threads data for a specified page
                Case "getpagethreads" ' pn = page number, the first page is 1
                    Dim i As Integer = Math.Abs(CInt(Request.Item("pn")) - 1)
                    Response.ContentType = "application/json"
                    Response.ContentEncoding = Text.Encoding.UTF8
                    Response.Write(JSON_GetPageThreads(i))

                Case "qrpost"
                    Response.ContentType = "application/json"
                    Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(ProcessPostAPI(Context), Newtonsoft.Json.Formatting.None))
                Case "checkcaptcha"
                    Response.ContentType = "application/javascript"
                    If EnableCaptcha Then
                        Dim check As String = Request.Item("check")
                        If check = "" Then
                            Response.Write("false")
                        Else
                            If GetCookie(Context, "captcha") = check Then
                                Response.Write("true")
                            Else
                                Response.Write("false")
                            End If
                        End If
                    Else
                        Response.Write("false")
                    End If
              
                Case Else
                    Response.StatusCode = 400
                    Response.Write("Invalid mode")
            End Select
        Catch ex As Exception
            Response.StatusCode = 500
            Response.Write("Server or syntax error" & vbNewLine & ex.Message)
        End Try
    End Sub

End Class