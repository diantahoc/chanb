<%@ Import Namespace="chanb" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.Language" %>
<%  Session("SS") = "yotsubab"%>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title><% Response.Write(BoardTitle)%></title>
<link rel="Stylesheet" href='yotsubab.css' />
<script src="scripts.js" type="text/javascript" language="javascript"></script>
</head> 
<body>
<div class="boardBanner"> 
<div class="boardTitle"><% Response.Write(BoardTitle)%></div>
<div class="boardSubtitle"><% Response.Write(BoardDesc)%></div>
</div>
<hr />
<div id="board">

<table>
<tr>
<td>
<div id="globalrules" style="float: left">
<span>Rules:</span>
<ul>
<il><span>• Do not post illegal material.</span></il><br />
<il><span>• Do not spam the site.</span></il><br />
</div>
</td>

<td>&nbsp;</td>

<td>

<div id="codetags" style="float: right">
<span>Code tags are useful to highlight syntax, rendering your code a lot easier to read. But you need to specify a language in the [lang][/lang] tags to properly highlight your code.</span>
<br />
<span>An example code tags using the C++ language should be like this</span>
<pre>
[code]
[lang]cpp[/lang]
//actuall code here
[/code]
</pre>

<span>This is a list of supported language identifiers (language id's are case-sensitive):</span>
<br />
<ul>
<il><span>• cpp for C++</span></il><br />
<il><span>• c for C</span></il><br />
<il><span>• vb or vbdotnet for VB.NET</span></il><br />
<il><span>• csharp for C#</span></il><br />
<il><span>• fsharp for F#</span></il><br />
<il><span>• java for Java</span></il><br />
<il><span>• js or javascript for Javascript</span></il><br />
<il><span>• php for PHP</span></il><br />
<il><span>• xml for XML</span></il><br />
<il><span>• html for HTML</span></il><br />
<il><span>• ps for PowerShell</span></il><br />
<il><span>• sql for SQL</span></il><br />
<il><span>• typescript for TypeScript</span></il><br />
<il><span>• css for CSS</span></il><br />
<il><span>• asax, ashx, aspx, aspxcs, aspxvb for ASAX, ASHX, ASPX, ASP.NET C#, ASP.NET VB.NET respectively.</span></il><br />
</ul>

<br />
<span>If you don't specify  a language, your code is treated as C/C++.</span>

</div>
</td>
</tr>
</table>





</div>


<div id="bottom"></div>
<div id="absbot" class="absBotText"><% Response.Write(footerText)%></span></div>
</body>
</html>
