<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="mod.aspx.vb" Inherits="chanb._mod" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%  
    If (Not Request.Item("modname") = "") AndAlso (Not Request.Item("modpass") = "") Then
        If IsModLoginValid(Request.Item("modname"), Request.Item("modpass")) Then
            Session("mod") = CStr(True)
            Response.Write("Login successful")
            Response.Redirect("default.aspx")
        End If
    End If
        %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Mod login</title>
</head>
<body>
    <form id="form1" action="mod.aspx">
    <div>
           <span > Name:</span> <input type="text" name="modname" /> 
          <br />
           <span > Password:</span> <input type="text" name="modpass" /> 
           <br />
           <input type="submit" value="ok" /> 
     
    </div>
    </form>
</body>
</html>
