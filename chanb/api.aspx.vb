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
                    pa.CredMenu = CStr(Session("credmenu"))
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
                Case "qrpost"
                    Response.ContentType = "application/json"
                    Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(ProcessPostAPI(Context), Newtonsoft.Json.Formatting.None))
                Case Else
                    Response.Write("Invalid mode")
            End Select
        Catch ex As Exception
            Response.Write("Server or syntax error" & vbNewLine & ex.Message)
        End Try
    End Sub

End Class