<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace="chanb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.Language" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title><% Response.Write(BoardTitle)%></title>
    <link rel="Stylesheet" href=<% Response.Write("'" & GetUserSelectedStyle(Session) & ".css'")%> />
    </head> 
<body>
    <div class="main">
        
    <div class="menu"><% Response.Write("Page generated in " & Request.CurrentExecutionFilePath)%></div>
    
    <div class="Bdesc">
    <label class="headertext"><% Response.Write(BoardTitle & " - " & BoardDesc)%></label>
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
    
    
    <div class="threadstream">
    <form name="deletation">
    
   <!--- <div class="postContainer"><img alt="" src="" /><label class="text">Subject</label><br /><label class='text'><a href='mailto:sage'></a>Name</label></div> -->
 
    <%    
        
        
        Dim startIndex As Integer = 0
        If Not (Request.Item("startPos") = "") Then startIndex = Request.Item("startPos")
        
        For Each x In GetThreads(startIndex, 10)
            Dim po As WPost = FetchPOSTData(x)
            Response.Write("<div class='postContainer'><a href='dispatcher.aspx?id=" & po.PostID & "'><img class='image' alt='' src='" & GetImageWEBPATHRE(po._imageP) & "' /></a><label class='text'>No." & po.PostID & "  " & po.time & "</label><label class='text'>" & po.subject & "</label><br /><label class='text'><a href='mailto:" & po.email & "'></a>" & po.name & "</label><br/><label>" & po.comment & "</label></div>")
        Next
%>

    </form>
    </div>
    
    </div>
</body>
</html>
