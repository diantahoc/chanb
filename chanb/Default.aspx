<%
    Session.Add("chanb", "chanb") ' To prevent session destroy   
    If Not chanb.isInstalled Then
        Response.Redirect("installer.aspx")
    End If 
    Response.Write(chanb.GlobalFunctions.GeneratePageHTML(False, Session, Request, Response))
%>