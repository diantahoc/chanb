<%@ Import Namespace = "chanb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace = "chanb.JSONApi" %>

<%

    Select Case Request.Item("mode")
        Case "update"
            Dim threadid As Integer = CInt(Request.Item("tid").Replace("t", ""))
            Dim lastpost As Integer = CInt(Request.Item("lp").Replace("p", ""))
            Dim pa As New HTMLParameters
            pa.isCurrentThread = True
            pa.isTrailPost = False
            pa.IsModerator = CBool(Session("mod"))
            pa.ModeratorPowers = CStr(Session("modpowers"))
            pa.modMenu = CStr(Session("modmenu"))
            pa.replyButton = False  
            Response.Write(WpostsToJsonList(GetThreadRepliesAfter(threadid, lastpost, False), pa))
        Case Else
            Response.Write("Invalid mode")
    End Select

  %>