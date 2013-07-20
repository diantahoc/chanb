<%@ Page Language="VB" %>
<%
    Session.Add("chanb", "chanb") ' To prevent session destroy   
    If chanb.isInstalled Then
        If Session("mod") = True Then
            Response.Write(chanb.GlobalFunctions.GenerateModSBR(Context))
        Else
            Response.StatusCode = 403
            Response.Write(chanb.forbiddenPage)
        End If
    Else
        Response.Redirect("installer.aspx")
    End If
%>