<%@ Page Language="VB" %>
<%
    If chanb.isInstalled Then
        Response.Write(chanb.GlobalFunctions.GeneratePageHTML(True, Session, Request, Response))
    Else
        Response.Redirect("installer.aspx")
    End If
%>