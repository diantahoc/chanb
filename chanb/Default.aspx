<%@ Import Namespace="chanb" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.Language" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
<%  Session.Add("chanb", "chanb") ' To prevent session destroy  %>
<title><% Response.Write(BoardTitle)%></title>
<link rel="Stylesheet" href='yotsubab.css' />
<link rel="Stylesheet" href="mobile.css" />
<script src="scripts.js" type="text/javascript" language="javascript"></script>
<script type="text/javascript" src="jquery-1.4.2.min.js"></script>
</head> 
<body>
<div class="boardBanner"> 
<div class="boardTitle"><% Response.Write(BoardTitle)%></div>
<div class="boardSubtitle"><% Response.Write(BoardDesc)%></div>
 <% If Not Request.Item("id") = "" Then Response.Write("<div class='postingMode desktop'><span>" & postingModstr & "</span><a href='./'>[" & returnStr & "]</a></div><br/>")%>
</div>
<div class="postdiv" align="center">
<form id="delform" name="delform" enctype="multipart/form-data" action="post.aspx" method="post" title="<% Response.Write(newthreadStr) %>">
<input type="hidden" name="mode" value="<% If Not request.item("id") = "" Then Response.write("reply") Else Response.write("thread") %>" />
<input type="hidden" name="threadid" value="<% Response.Write(Request.Item("id")) %>" />
<table>
<tbody>
<tr>
<th><% Response.Write(NAMEString)%></th>
<td><input type="text" size="25" name="postername" /></td>
</tr><tr>
<th><% Response.Write(EMAILString)%></th>
<td>
<input name="email" size="25" type="text">
</td>
</tr><tr>
<th><% Response.Write(SUBJECTString)%></th>
<td><input style="float:left;" name="subject" size="25" type="text"/>
<input accesskey="s" style="margin-left:2px;" name="post" value="<% If Not Request.Item("id") = "" then Response.write(replyStr) else Response.Write(newthreadStr) %>" type="submit"/></td>
</tr><tr>
<th><% Response.Write(COMMENTString)%></th>
<td><textarea name="comment" id="commentfield" rows="5" cols="35"></textarea></td>
</tr><tr>
<th><% Response.Write(filesStr)%></th>
<td>
    <div id="files" >
    <input type="file" name="ufile" class="file" maxlength="<% response.write(maximumfilesize / 1024) %>" id="file1" />
    </div>  
    <%If Not Request.Item("id") = "" Then Response.Write("<input type='checkbox' name='finp' value='yes'>" & eachfileInNewpost & "</input><br/><input type='checkbox' name='countf' value='yes'>" & countFiles & "</input><br/><input class='button' type='button'  value='" & addAnotherF & "'  onclick='createUf();' />")%> 
</td>   
</tr><tr>
<th><% Response.Write(PASSWORDString)%></th>
<td><input name="password" size="12" autocomplete="off" type="text" value="<% Response.Write(GetSessionPassword(Request.Cookies, Session)) %>"/>
<span>(<% Response.Write(forPD)%>)</span>
</td>
</tr>
</tbody>
</table>
</form>
<ul class="rules" >
<li><span>Maximum file size is <% Response.Write((MaximumFileSize / 1024 / 1024) & " MB")%>.</span></li>
<li><span>Supported file types are : JPG, JPEG, BMP, PNG, SVG, and PDF.</span></li>
<li><span>Blank posts are not allowed.</span></li>
<li><span>Spoilers are suported under the [spolier][/spolier] tags.</span></li>
<li><span>You may highlight your code by using the [code][/code] tags. The [lang][/lang] tags are required in order to properly highlight your code. See a <a href="faq.aspx#codetags">list</a> of supported languages.</span></li>
<li><span>This website is for demonstrating a live preview of ChanB imageboard. Expect your post to be deleted at any time.</span></li>
<li><span>All times are UTC.</span></li>
<li><span><% Response.Write("Currently there is " & CStr(GetThreadsCount(False)) & " thread(s).")%></span></li>
</ul>
</div>
<div class="navLinks navLinksBot desktop">[<a href="./"><% Response.Write(returnStr)%></a>] [<a href="catalog.aspx"><% Response.Write(catalogstr)%></a>] [<a href="#bottom"><% Response.Write(bottomstr)%></a>] </div>
<hr />
<form name="deletation" action="post.aspx" enctype="application/x-www-form-urlencoded" method="get">
<div class="board">
<div id="top"></div>
 <%    
     
     Dim para As New HTMLParameters()
     para.IsModerator = CBool(Session("mod"))
     para.ModeratorPowers = CStr(Session("modpowers"))
     para.modMenu = CStr(Session("modmenu"))
     
     Dim validID As Boolean = False  
     Try
         Dim i = CInt(Request.Item("id"))
         validID = True
     Catch ex As Exception
         validID = False
     End Try
     
     
     If Not (Request.Item("id") = "") And validID Then
         
        
         
         'Display a thread and children posts 
         Dim opID As Integer = CInt(Request.Item("id"))
         opID = Math.Abs(opID)
         Dim po As WPost = FetchPostData(opID)
        
         If po.type Is Nothing Then
             Response.Redirect("default.aspx")
         End If
        
         If po.archived = True Then
             Response.Redirect("archive.aspx?id=" & po.PostID)
         End If
         
         ' Check if it is a reply or a thread , 0 = thread, 1 = reply
         ' If it is a reply, redirect to parent thread.
         If po.type = 1 Then Response.Redirect("default.aspx?id=" & po.parent & "#p" & po.PostID)
         
         'Write OP Post 
         para.replyButton = False
         para.isTrailPost = False
         Response.Write(GetOPPostHTML(opID, para))
         'Write replies, if any.  
         Response.Write(GetRepliesHTML(opID, para))
                 
     Else
         
         'Display a list of current threads
         Dim startIndex As Integer = 0
         para.replyButton = True
         para.isTrailPost = True
         If Not (Request.Item("startindex") = "") Then startIndex = CInt(Request.Item("startindex")) * (ThreadPerPage)
         For Each x In GetThreads(startIndex, ThreadPerPage - 1 + startIndex, False, False)
             Response.Write(GetThreadHTML(x, para, TrailPosts))
         Next
         
     End If
     
     
%>

</div>
<div class="navLinks mobile" style="margin-top: 10px;">
<span class="mobileib button"><a href="./"><% Response.Write(returnStr)%></a></span>
<span class="mobileib button"><a href="#top"><% Response.Write(topsrt)%></a></span>
<span class="mobileib button"><a href="#bottom_r" id="refresh_bottom" onmouseup="javascript:refresh();"><% Response.Write(refreshStr)%></a></span></div>

<div class="navLinks navLinksBot desktop">[<a href="./"><% Response.Write(returnStr)%></a>] [<a href="catalog.aspx"><% Response.Write(catalogstr)%></a>] [<a href="#top"><% Response.Write(topsrt)%></a>] </div>
<br />
<div style="float: right;">
<div class="deleteform desktop">
<input type="text" name="deletePass" value="<% Response.Write(GetSessionPassword(Request.Cookies, Session)) %>" />
<input type="submit" name="mode" value="<% Response.Write(deleteStr) %>" />
<input type="submit" name="mode" value="<% Response.Write(reportStr) %>" /></div>
</div>
</form>
<div class="pagelist desktop">
<%
    Dim threadCount As Integer = GetThreadsCount(False)
    Dim pagesCount As Double = threadCount / ThreadPerPage  
    If pagesCount > (Fix(pagesCount)) Then
        pagesCount = Fix(pagesCount) + 1
    End If  
    Dim startIndexA As Integer
    Try
        startIndexA = CInt(Request.Item("startindex"))
    Catch ex As Exception
        startIndexA = 0
    End Try 
    If startIndexA = 0 Then
        Response.Write("<div class='prev'><span>" & prevStr & "</span></div>")
    Else
        Response.Write("<div class='prev'><form action='default.aspx'><input name='startindex' type='hidden' value='" & startIndexA - 1 & "' /><input value='" & prevStr & "' type='submit'/></form></div>")
    End If   
    Response.Write("<div class='pages'>")
    For i As Integer = 0 To (pagesCount - 1) Step 1
        If i = startIndexA Then
            Response.Write("[<strong><a href='?startindex=" & i & "'>" & i + 1 & "</a></strong>]")
        Else
            Response.Write("[<a href='?startindex=" & i & "'>" & i + 1 & "</a>]")
        End If
   
    Next 
    Response.Write("</div>")   
    If startIndexA = pagesCount - 1 Then ' last page
        Response.Write("<div class='next'><span>" & nextStr & "</span></div>")
    Else
        Response.Write("<div class='next'><form action='default.aspx'><input name='startindex' type='hidden' value='" & startIndexA + 1 & "' /><input value='" & nextStr & "' type='submit'/></form></div>")
    End If
%>
</div>
<div id="bottom"></div>
<div id="absbot" class="absBotText"><% Response.Write(footerText)%></span></div>
</body>
</html>
