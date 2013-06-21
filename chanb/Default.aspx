<%@ Page Language="VB" %>
<%
    Session.Add("chanb", "chanb") ' To prevent session destroy   
    If chanb.isInstalled Then
        Response.Write(chanb.GlobalFunctions.GeneratePageHTML(False, Session, Request, Response))
    Else
        Response.Redirect("installer.aspx")
    End If
%>