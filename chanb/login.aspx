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
            Response.End()
        Else
            Response.Write(FormatHTMLMessage(chanb.Language.ForbiddenStr, chanb.Language.modLoginFailed, "", "8888", False))
            Response.End()
        End If
    End If
%>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>Channel Board Login</title>
<style>
html {
  background-color: #E6E6E6;
  font-size: 10pt;
  margin: 50px 40px 20px 40px;
  text-align: center;
}
body {
  margin: auto;
  max-width: 540px;
  min-width: 200px;
}
#pc {
  margin: auto;
  padding-bottom: 20px;
  max-width: 540px;
  min-width: 200px;
  border: 0px solid #AAA;
  color: #444;
  background-color: #f9f9f9;
  border: 1px solid #AAA;
  border-bottom: 1px solid #888;
  border-radius: 3px;
  box-shadow: 0px 2px 2px #AAA;
}
#pc #titleText {
  color: #666;
  margin: 0;
  padding-top: 25px;
  padding-bottom: 25px;
  font-weight: normal;
  font-size: 1.5em;
  border-bottom: 1px solid #EEE;
  border-radius: 3px 3px 0 0;
  background-position: center 20px;
}
#content {
  padding: 0 24px 24px;
  text-align: start;
}
#sdesc > p {
  white-space: pre-wrap;
}
.button {
  -moz-appearance: none;
  background-image: linear-gradient(#ededed, #ededed 38%, #dedede);
  border: 1px solid rgba(0, 0, 0, 0.25);
  color: #444;
  text-shadow: 0 1px 0 rgb(240, 240, 240);
  border-radius: 2px;
  box-shadow: 0 1px 0 rgba(0, 0, 0, 0.08), inset 0 1px 2px rgba(255, 255, 255, 0.75);
  margin: 0px 5px;
  min-width: 4em;
  min-height: 34px;
  -moz-padding-start: 9px;
  -moz-padding-end: 8px;
  font-family: 'Segoe UI', Arial, 'Microsoft Yahei', Simsun, sans-serif;
}
.button:hover {
  background-image: linear-gradient(#f0f0f0, #f0f0f0 38%, #e0e0e0);
  border: 1px solid rgba(0, 0, 0, 0.3);
  color: #000;
  box-shadow: 0 1px 0 rgba(0, 0, 0, 0.12), inset 0 1px 2px rgba(255, 255, 255, 0.95);
}
.button:active {
  background-image: linear-gradient(#e7e7e7, #e7e7e7 38%, #d7d7d7);
  border: 1px solid rgba(0, 0, 0, 0.3);
  color: #444;
  box-shadow: none;
}
</style>
</head>
<body dir="ltr">
<div id="pc">
	<div id="title">
		<h1 id="titleText">Login</h1>
	</div>
	<div id="content">
		<div id="sdesc">
			<p id="sdescText">
				Enter your user name and password
			</p>
		</div>
		<div id="ldesc">
			<form action="login.aspx">
				<span> Name:</span><input type="text" name="name"/>
				<br/>
				<span> Password:</span><input type="password" name="pass"/>
				<br/>
				<input class="button" type="submit" value="Login"/>
			</form>
		</div>
	</div>
</div>
</body>
</html>