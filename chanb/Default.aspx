<%@ Import Namespace="chanb" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.Language" %>


<%  Session("SS") = "yotsubab"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title><% Response.Write(BoardTitle)%></title>
    <link rel="Stylesheet" href=<% Response.Write("'" & GetUserSelectedStyle(Session) & ".css'")%> />
    </head> 
<body>
   
   <div class="boardBanner"> 
   <div class="boardTitle"><% Response.Write(BoardTitle)%></div>
   <div class="boardSubtitle"><% Response.Write(BoardDesc)%></div>
   </div>
    
<div class="postdiv" align="center">
    
    <form name="form" action="post.aspx" method="post" enctype="multipart/form-data" title="New thread">
    
    <input type="hidden" name="mode" value="thread" />
    
    <label class="text"><% Response.Write(NAMEString)%></label>
    <input type="text" class="inputfield" name="postername" />
    <br class="br" />
    <label class="text"><% Response.Write(SUBJECTString)%></label>
    <input type="text" class="inputfield" name="subject" />
    <br class="br" />
    <label class="text"><% Response.Write(EMAILString)%></label>
    <input type="text" class="inputfield" name="email" />
    <br class="br" />
    <label class="text"><% Response.Write(COMMENTString)%></label>
    <textarea cols="13" rows="10" class="texto" name="comment" ></textarea>
    <br class="br" />
    <label class="text"><% Response.Write(PASSWORDString)%></label>
    <input type="text" class="inputfield" name="password" value="<% Response.Write(GetSessionPassword(Session)) %>" />
    <br class="br" />  
    <input type="file" name="ufile" class="file" maxlength="<% response.write(maximumfilesize / 1024) %>" id="file" />
    <br class="br" />
    <input type="submit" value="submit" />
    </form>
    
    <label class="text">Maximum file size is <% Response.Write((MaximumFileSize / 1024 / 1024) & " MB")%></label>
    
    </div>
    
<form name="deletation" action="post.aspx" enctype="application/x-www-form-urlencoded" method="get">
<div class="board">
 <%    
     Dim startIndex As Integer = 0
        If Not (Request.Item("startPos") = "") Then startIndex = Request.Item("startPos")
        
        For Each x In GetThreads(startIndex, 10)
            Response.Write(GetThreadHTML(x))
        Next
%>
</div>

<div style="float: right;">
<div class="deleteform desktop">
<input type="text" name="deletePass" value="<% Response.Write(GetSessionPassword(Session)) %>" />
<input type="submit" name="mode" value="delete" />
<input type="submit" name="mode" value="report" /></div>
</div>
</form>
<div id="bottom"></div>
</body>
</html>
