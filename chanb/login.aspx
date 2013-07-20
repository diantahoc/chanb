<%@ Page Language="vb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.Misc" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<%  
    If (Not Request.Item("name") = "") And (Not Request.Item("pass") = "") Then      
        Dim lgi As chanb.LoginInfo = GetLoginInfo(Request.Item("name"), Request.Item("pass"))
        
        If lgi.LogInValid Then
            Select Case lgi.AccountType
                Case chanb.LoginInfo.ChanbAccountType.Administrator
                    Session("admin") = CStr(True)
                Case chanb.LoginInfo.ChanbAccountType.Moderator
                    Session("mod") = CStr(True)
                    Session("modname") = Request.Item("name")
            End Select
            Session("credpower") = lgi.Powers
            Session("credmenu") = GetModeratorHTMLMenu(lgi.Powers) ' I don't know if it is a good idea to store html in the session, but I don't see a reason to not do it.
            Response.Write(FormatHTMLMessage(chanb.Language.modLoginSucess, chanb.Language.modLoginSucess, "default.aspx", "2", False))
        Else
            Response.Write(FormatHTMLMessage(chanb.Language.ForbiddenStr, chanb.Language.modLoginFailed, "", "8888", False))
        End If
    End If
%>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Channel Board login</title>
    <link rel="Stylesheet" href="sysadmin.css" />
</head>
<body>
    <form action="login.aspx">
    <div align="center">
    <div class="whitebox">
           <span > Name:</span> <input type="text" name="name" />
          <br />
           <span > Password:</span> <input type="password" name="pass" /> 
           <br />
           <input type="submit" value="ok" /> 
     
    </div></div>
    </form>
</body>
</html>
