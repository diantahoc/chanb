<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace = "chanb.Language" %>

<%
    If CBool(Session("admin")) = False Then
        Response.Write("Admin privelige is required.")
    Else
        Select Case Request.Item("action")
            Case "updatesettings"
            Case "newmod"
            Case "deletemod"
            Case "updatemod"
            Case "changepass"
            Case "permadelpost"
            Case "stickythread"
            Case "lockthread"
            Case "editpost"
            Case "editimage"
        End Select
    End If

%>