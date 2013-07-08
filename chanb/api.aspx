<%@ Import Namespace = "chanb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace = "chanb.JSONApi" %>
<%@ Page Language="VB" %>

<%

    Try
        Select Case Request.Item("mode")
            Case "fetchrepliesafter" ' tid = thread id , lp = last post 
                Dim threadid As Integer = CInt(Request.Item("tid").Replace("t", ""))
                Dim lastpost As Integer = CInt(Request.Item("lp").Replace("pc", ""))
                Dim pa As New HTMLParameters
                pa.isCurrentThread = True
                pa.isTrailPost = False
                pa.IsModerator = CBool(Session("mod"))
                pa.ModeratorPowers = CStr(Session("modpowers"))
                pa.modMenu = CStr(Session("modmenu"))
                pa.replyButton = False
                Response.Write(WpostsToJsonList(GetThreadRepliesAfter(threadid, lastpost, False), pa))
            Case "getfileslist" ' tid = thread id , ft = file types jpg,bmp, list ... , ar = boolean include archived
                Dim tid As Integer = CInt(Request.Item("tid"))
                Dim allowedfiles As New List(Of String)
                For Each x In Request.Item("ft").Split(CChar(","))
                    allowedfiles.Add(x)
                Next
                Response.Write(GetFileList(tid, allowedfiles.ToArray, CBool(Request.Item("ar"))))
            Case "getthreadpagenumber" 'tid = thread id
                Dim i As Integer = CInt(Request.Item("tid"))
                Response.Write(GetThreadPageNumber(i))
            Case Else
                Response.Write("Invalid mode")
        End Select
        
    Catch ex As Exception
        Response.Write("Server or syntax error")
    End Try
    


  %>