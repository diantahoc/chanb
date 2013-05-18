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
<script type="text/javascript" src="js/jquery.min.js"></script>
<script type="text/javascript" src="js/jquery-ui.min.js"></script>
</head> 
<body>
<div class="boardBanner"> 
<div class="boardTitle"><% Response.Write(BoardTitle)%></div>
<div class="boardSubtitle"><% Response.Write(BoardDesc)%></div>
</div>

<div id="postdiv" align="center">
 <% If Not Request.Item("id") = "" Then Response.Write("<br /><div class='postingMode desktop'><span>" & postingModstr & "</span></div>")%>
<form id="delform" name="delform" enctype="multipart/form-data" action="post.aspx" method="post" title="<% Response.Write(newthreadStr) %>">
<input type="hidden" name="mode" value="<% If Not request.item("id") = "" Then Response.write("reply") Else Response.write("thread") %>" />
<input type="hidden" name="threadid" value="<% Response.Write(Request.Item("id")) %>" />

<table>
<tbody>
<tr>
<th><% Response.Write(NAMEString)%></th>
<td><input class="form-text" type="text" size="30" name="postername" /></td>
</tr><tr>
<th><% Response.Write(EMAILString)%></th>
<td>
<input class="form-text"  name="email" size="30" type="text"/>
</td>
</tr><tr>
<th><% Response.Write(SUBJECTString)%></th>
<td><input class="form-text" style="float:left;" name="subject" size="30" type="text"/>

<a class="form-button" onclick="$(this).closest('form').submit()"><% If Not Request.Item("id") = "" then Response.write(replyStr) else Response.Write(newthreadStr) %></a>

<noscript>
<input style="margin-left:2px;" name="post" value="<% If Not Request.Item("id") = "" then Response.write(replyStr) else Response.Write(newthreadStr) %>" type="submit"/>
</noscript>
</td>
</tr><tr>
<th><% Response.Write(COMMENTString)%></th>
<td><textarea class="form-textarea" name="comment" id="commentfield" rows="5" cols="35"></textarea></td>
</tr><tr>
<th><% Response.Write(filesStr)%></th>
<td>
    <div id="files" >
    <input type="file" name="ufile" class="file" maxlength="<% response.write(maximumfilesize / 1024) %>" id="file1" />
    </div>  
    <%If Not Request.Item("id") = "" Then Response.Write("<input type='checkbox' name='finp' value='yes'>" & eachfileInNewpost & "</input><br/><input type='checkbox' name='countf' value='yes'>" & countFiles & "</input><br/><a class='form-button' onclick='createUf();' >" & addAnotherF & "</a>")%> 
</td>   
</tr><tr>
<th><% Response.Write(PASSWORDString)%></th>
<td><input class="form-text" name="password" size="12" autocomplete="off" type="text" value="<% Response.Write(GetSessionPassword(Request.Cookies, Session)) %>"/>
<span>(<% Response.Write(forPD)%>)</span>
</td>
</tr>
</tbody>
</table>
</form>
</div>

<div id="rulesdiv" align="center">
<ul class="rules" >
<li><span>Maximum file size is <% Response.Write(FormatSizeString(MaximumFileSize))%>.</span></li>
<li><span>Supported file types are : JPG, JPEG, BMP, PNG, SVG, and PDF.</span></li>
<li><span>Blank posts are not allowed.</span></li>
<li><span>Spoilers are suported under the [spolier][/spolier] tags.</span></li>
<li><span>You may highlight your code by using the [code][/code] tags. The [lang][/lang] tags are required in order to properly highlight your code. See a <a href="faq.aspx#codetags">list</a> of supported languages.</span></li>
<li><span>This website is for demonstrating a live preview of ChanB imageboard. Expect your post to be deleted at any time.</span></li>
<li><span>All times are UTC.</span></li>
<li><span><% Response.Write("Currently there is " & CStr(GetThreadsCount(False)) & " thread(s).")%></span></li>
</ul>
</div>


<div class="navLinks navLinksBot desktop">
<% If Not Request.Item("id") = "" Then Response.Write("[<a href='./'>" & returnStr & "</a>]") %>
[<a href="catalog.aspx"><% Response.Write(catalogstr)%></a>] 
[<a href="#bottom"><% Response.Write(bottomstr)%></a>] </div>

<div class="navLinks mobile" style="margin-top: 10px;">
<% If Not Request.Item("id") = "" Then Response.Write("<span class='mobileib button'><a href='./'>" & returnStr & "</a></span>") %>
<span class="mobileib button"><a href="#bottom"><% Response.Write(bottomstr)%></a></span>
<span class="mobileib button"><a href="catalog.aspx"><% Response.Write(catalogstr)%></a></span>
<span class="mobileib button"><a href="javascript:document.location.reload();" id="refresh_bottom"><% Response.Write(refreshStr)%></a></span>
</div>
<hr />
<div id="top"></div>
<form id="delfrm" name="deletation" action="post.aspx" enctype="application/x-www-form-urlencoded" method="get">


<div class="board">

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

<div style="float: right;">
<div class="deleteform desktop">
<input type="text" class="form-text" name="deletePass" value="<% Response.Write(GetSessionPassword(Request.Cookies, Session)) %>" />
<input type="hidden" value="" id="ROD" name="mode" />

<a class="form-button" onclick="updatemode('<% Response.Write(deleteStr) %>');$(this).closest('form').submit();"><% Response.Write(deleteStr) %></a>
<a class="form-button" onclick="updatemode('<% Response.Write(reportStr) %>');$(this).closest('form').submit();"><% Response.Write(reportStr) %></a>

<noscript>
<input type="submit" name="mode" value="<% Response.Write(deleteStr) %>" />
<input type="submit" name="mode" value="<% Response.Write(reportStr) %>" />
</noscript>
</div>
</div>
</form>

<div class="navLinks mobile" style="margin-top: 10px;">

<% If Not Request.Item("id") = "" Then Response.Write("<span class='mobileib button'><a href='./'>" & returnStr & "</a></span>") %>
<span class="mobileib button"><a href="#top"><% Response.Write(topsrt)%></a></span>
<span class="mobileib button"><a href="catalog.aspx"><% Response.Write(catalogstr)%></a></span>
<span class="mobileib button"><a href="javascript:document.location.reload();" id="refresh_bottom"><% Response.Write(refreshStr)%></a></span>
</div>

<div class="navLinks navLinksBot desktop">
<% If Not Request.Item("id") = "" Then Response.Write("[<a href='./'>" & returnStr & "</a>]") %>
[<a href="catalog.aspx"><% Response.Write(catalogstr)%></a>] 
[<a href="#top"><% Response.Write(topsrt)%></a>] 
</div>

<br />

<%  If Request.Item("id") = "" Then
        
        Response.Write("<div class='pagelist desktop'>")
        
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
            Response.Write("<div class='prev'><a class='form-button-disabled'>" & prevStr & "</a></div>")
        Else
            Response.Write("<div><a class='form-button' href='default.aspx?startindex=" & CStr(startIndexA - 1) & "'>" & prevStr & "</a></div>")
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
            Response.Write("<div class='next'><a class='form-button-disabled'>" & nextStr & "</a></div>")
        Else
            Response.Write("<div><a class='form-button' href='default.aspx?startindex=" & CStr(startIndexA + 1) & "'>" & nextStr & "</a></div>")
        End If
    End If
    
    Response.Write("</div>")
    %>
<div id="bottom"></div>
<div id="absbot" class="absBotText"><% Response.Write(footerText)%></span></div>
</body>
</html>
