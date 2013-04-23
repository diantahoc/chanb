<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace="chanb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.Language" %>

<%  Session("SS") = "yotsubab"%>

<%

    If Request.Item("id") = "" Then
        Response.Redirect("default.aspx")
    End If
    
 %>


<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title><% Response.Write(BoardTitle)%></title>
    <link rel="Stylesheet" href='<% Response.Write("" & GetUserSelectedStyle(Session) & ".css")%>' />
    <script src="scripts.js" type="text/javascript" language="javascript"></script>
</head> 
<body>

   <div class="boardBanner"> 
   <div class="boardTitle"><% Response.Write(BoardTitle)%></div>
   <div class="boardSubtitle"><%Response.Write(BoardDesc)%></div>
   </div>

   <div class="postdiv" align="center">
    
    <form id="replyform" name="form" action="post.aspx" method="post" enctype="multipart/form-data" title="New thread">
    
    <input type="hidden" name="mode" value="reply" />
    <input type="hidden" name="threadid" value="<% Response.Write(Request.Item("id")) %>" />
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
    <textarea id="commentfield" cols="13" rows="10" class="texto" name="comment"></textarea>
    <br class="br" />
    <label class="text"><% Response.Write(PASSWORDString)%></label>
    <input type="text" class="inputfield" name="password" value="<% Response.Write(GetSessionPassword(Session)) %>" />
    <br class="br" />  
    <input type="file" name="ufile" class="file" maxlength="<% response.write(maximumfilesize / 1024) %>" id="file" />

<input class="button" type="button"  value="Add another file"  onclick="createUf();" />  
    <br class="br" />
    <input type="submit" value="submit" />
    </form>
    
    <label class="text">Maximum file size is <% Response.Write((MaximumFileSize / 1024 / 1024) & " MB")%></label>
    
    </div>
    
 <div class="replystream">
    <form name="deletation" action="post.aspx" enctype="application/x-www-form-urlencoded" method="get">
    
    <% 
        Dim opID As Integer = Request.Item("id")
        Dim po As WPost = FetchPostData(opID)
        
        If po.type Is Nothing Then
            Response.Redirect("default.aspx")
        End If
        
        ' Check if it is a reply or a thread , 0 = thread, 1 = reply
        ' If it is a reply, redirect to parent thread.
        If po.type = 1 Then Response.Redirect("dispatcher.aspx?id=" & po.parent & "#p" & po.PostID)
    
        'Write OP Post        
        Response.Write(GetOPPostHTML(opID, False))   
        'Write replies, if any.  
        Response.Write(GetRepliesHTML(opID))           
%>   

<div style="float: right;">
<div class="deleteform desktop">
<input type="text" name="deletePass" value="<% Response.Write(GetSessionPassword(Session)) %>" />
<input type="submit" name="mode" value="delete" />
<input type="submit" name="mode" value="report" /></div>
</div>
    </form>
    </div>
    
</body>
</html>
