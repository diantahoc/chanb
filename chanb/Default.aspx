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
    <script src="scripts.js" type="text/javascript" language="javascript"></script>
    <script type="text/javascript" src="jquery-1.4.2.min.js"></script>
    </head> 
<body>
 
   <div class="boardBanner"> 
   <div class="boardTitle"><% Response.Write(BoardTitle)%></div>
   <div class="boardSubtitle"><% Response.Write(BoardDesc)%></div>
 <% If Not Request.Item("id") = "" Then Response.Write("<div class='postingMode desktop'><span>Posting mode: Reply</span><a href='/'>[Return]</a></div><br/>")%>
   </div>
    
<div class="postdiv" align="center">
<form name="form" enctype="multipart/form-data" action="post.aspx" method="post" title="New thread">
<input type="hidden" name="mode" value="<% If not request.item("id") = "" then Response.write("reply") else Response.write("thread") %>" />
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
<td><input style="float:left;" name="subject" size="25" type="text">
<input accesskey="s" style="margin-left:2px;" name="post" value="<% If Not Request.Item("id") = "" then Response.write("Reply") else Response.Write("New Topic") %>" type="submit"></td>
</tr><tr>
<th><% Response.Write(COMMENTString)%></th>
<td><textarea name="comment" id="commentfield" rows="5" cols="35"></textarea></td>
</tr><tr>

<th>File(s)</th>
<td>
    <div id="files" >
    <input type="file" name="ufile" class="file" maxlength="<% response.write(maximumfilesize / 1024) %>" id="file1" />
    </div>  
    <%If Not Request.Item("id") = "" Then Response.Write("<input class='button' type='button'  value='Add another file'  onclick='createUf();' />")%> 
</td>   
</tr><tr>
<th><% Response.Write(PASSWORDString)%></th>
<td><input name="password" size="12" autocomplete="off" type="text" value="<% Response.Write(GetSessionPassword(Session)) %>">
<span>(For post deletion.)</span>
</td>
</tr>
</tbody>
</table>
</form>
<span>Maximum file size is <% Response.Write((MaximumFileSize / 1024 / 1024) & " MB")%></span>   
</div> 
 
<form name="deletation" action="post.aspx" enctype="application/x-www-form-urlencoded" method="get">
<div class="board">
 <%    
     
     If Session.Item("mod") Is "" Or Session.Item("mod") Is Nothing Then Session("mod") = CStr(False)
     
     Dim validID As Boolean = False
     
     Try
         Dim i = CInt(Request.Item("id"))
         validID = True
     Catch ex As Exception
         validID = False
     End Try
     
     
     If Not (Request.Item("id") = "") And validID Then
         
         'Display thread or post thread      
         Dim opID As Integer = Request.Item("id")
         Dim po As WPost = FetchPostData(opID)
        
         If po.type Is Nothing Then
             Response.Redirect("default.aspx")
         End If
        
         ' Check if it is a reply or a thread , 0 = thread, 1 = reply
         ' If it is a reply, redirect to parent thread.
         If po.type = 1 Then Response.Redirect("default.aspx?id=" & po.parent & "#p" & po.PostID)
    
         'Write OP Post        
         Response.Write(GetOPPostHTML(opID, False, CBool(Session("mod"))))
         'Write replies, if any.  
         Response.Write(GetRepliesHTML(opID, CBool(Session("mod"))))
                 
     Else
         
         'Display a list of current threads
         Dim startIndex As Integer = 0
         If Not (Request.Item("startindex") = "") Then startIndex = Request.Item("startindex")
        
         For Each x In GetThreads(startIndex, ThreadPerPage)
             Response.Write(GetThreadHTML(x, CBool(Session("mod")), 3))
         Next
         
     End If
     
   
%>
</div>



<div style="float: right;">
<div class="deleteform desktop">
<input type="text" name="deletePass" value="<% Response.Write(GetSessionPassword(Session)) %>" />
<input type="submit" name="mode" value="delete" />
<input type="submit" name="mode" value="report" /></div>
</div>
</form>


<div class="pagelist desktop">
<%

    Dim threadCount As Integer = GetThreadsCount()
    
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
        Response.Write("<div class='prev'><span>Previous</span></div>")
    Else
        Response.Write("<div class='prev'><form action='default.aspx'><input name='startindex' type='hidden' value='" & startIndexA - 1 & "' /><input value='Previous' type='submit'/></form></div>")
    End If
    
    Response.Write("<div class='pages'>")
    
    '[<strong><a href="">0</a></strong>]
    '[<a href="1">1</a>]

    For i As Integer = 0 To (pagesCount - 1) Step 1
        If i = startIndexA Then
            Response.Write("[<strong><a href='?startindex=" & i & "'>" & i + 1 & "</a></strong>]")
        Else
            Response.Write("[<a href='?startindex=" & i & "'>" & i + 1 & "</a>]")
        End If
   
    Next
   
    Response.Write("</div>")
    
    If startIndexA = pagesCount - 1 Then ' last page
        Response.Write("<div class='next'><span>Next</span></div>")
    Else
        Response.Write("<div class='next'><form action='default.aspx'><input name='startindex' type='hidden' value='" & startIndexA + 1 & "' /><input value='Next' type='submit'/></form></div>")
    End If
%>
</div>
<div id="bottom"></div>

</body>
</html>
