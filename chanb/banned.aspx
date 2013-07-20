<%@ Page Language="vb"%>
<% 
    Session.Add("chanb", "chanb") ' To prevent session destroy 
    If chanb.isInstalled Then
        Response.Write(chanb.GlobalFunctions.GenerateBanPage(Context))
    Else
        Response.Redirect("installer.aspx")
    End If
%>