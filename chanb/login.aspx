﻿<%@ Page Language="vb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.Misc" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%  
    If (Not Request.Item("modname") = "") AndAlso (Not Request.Item("modpass") = "") Then
        If IsModLoginValid(Request.Item("modname"), Request.Item("modpass")) Then
            Session("mod") = CStr(True)
            Session("modpowers") = GetModPowers(Request.Item("modname"))
            Session("modmenu") = GetModeratorHTMLMenu("%ID%", Session("modpowers"))     
            Response.Write(FormatHTMLMessage(chanb.Language.modLoginSucess, chanb.Language.modLoginSucess, "default.aspx", "2", False))
        End If
    End If
    
  
    
        %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Mod login</title>
</head>
<body>
    <form id="form1" action="login.aspx">
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