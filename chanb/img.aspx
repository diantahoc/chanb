<%@ Import Namespace = "chanb" %>
<%@ Page Language="VB" %>
<%
    ' img.aspx
    ' usage: 
    ' 1 - img.aspx?md5= image md5 , return image data with real image name.
    ' 2 - img.aspx?cn= chanb name & rn = real name, return image data with the specified real name.
    Session("chanb") = "chanb"
    If chanb.isInstalled Then
        ImgPageHandler.Process(Context)
    Else
        Response.StatusCode = 404
        Response.Write("404")
    End If
 %>