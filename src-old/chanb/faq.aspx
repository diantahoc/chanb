<%@ Page Language="VB"  AutoEventWireup="false" CodeBehind="faq.aspx.vb" Inherits="chanb._faq" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<title>Channel Board</title>
<link rel="Stylesheet" href='yotsubab.css' />
<script src="scripts.js" type="text/javascript" language="javascript"></script>
</head> 
<body>
<div class="boardBanner"> 
<div class="boardTitle">Channel Board</div>
<div class="boardSubtitle">---</div>
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

<span>This is a list of supported language identifiers:</span>
<br />
<ul>
<il><span>• scheme,scm for Scheme</span></il><br />
<il><span>• lisp,lsp for Lisp and derivatives.</span></il><br />
<il><span>• apollo,agc,aea for Apollo</span></il><br />
<il><span>• vb,vbs,vb.net for Visual Basic</span></il><br />
<il><span>• scala for Scala</span></il><br />
<il><span>• pascal for Pascal</span></il><br />
<il><span>• go for Go</span></il><br />
<il><span>• sql for SQL</span></il><br />
<il><span>• c,cpp,cplusplus for C/C++</span></il><br />
<il><span>• css for CSS</span></il><br />
<il><span>• clojure,clj for Clojure</span></il><br />
<il><span>• haskel,hs for Haskel</span></il><br />
<il><span>• erlang,erl for Erlang</span></il><br />
<il><span>• lua for Lua</span></il><br />
<il><span>•  F# for F#</span></il><br />
</ul>
<br />
<span>If you don't specify  a language, prettify will guess it.</span>
</div>
</td>
</tr>
</table>





</div>


<div id="bottom"></div>
<div id="absbot" class="absBotText"><% Response.Write(foo)%></span></div>
</body>
</html>
