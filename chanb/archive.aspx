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
<div class='postingMode'><span>This is the archive. Click <a href="default.aspx">here</a> to go back to the main page</span></div> 
</div>
<div class="postdiv" align="center">
<ul class="rules" >
<li><span>Please report any violating post.</span></li>
</ul>
</div>
<hr />
<form name="deletation" action="post.aspx" enctype="application/x-www-form-urlencoded" method="get">
<div class="board">

 <%    
     
     Dim para As New HTMLParameters()
     para.IsModerator = CBool(Session("mod"))
     para.ModeratorPowers = CStr(Session("modpowers"))
     para.modMenu = CStr(Session("modmenu"))
     para.isCurrentThread = False
     
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
             Response.StatusCode = 404
             Response.End()
         End If
        
         If po.archived = True Then
            
             ' Check if it is a reply or a thread , 0 = thread, 1 = reply
             ' If it is a reply, redirect to parent thread.
             If po.type = 1 Then Response.Redirect("archive.aspx?id=" & po.parent & "#p" & po.PostID)
         
             'Write OP Post 
             para.replyButton = False
             para.isTrailPost = False
             
             Response.Write("<div class='thread' id='t" & opID & "'>")
             Response.Write(GetThreadHTML(opID, para))
             Response.Write("</div><hr ></hr>")
         
         Else
             
             Response.Redirect("default.aspx?id=" & po.PostID)
             
         End If
         
     Else
         
         'Display a list of archived threads
         Dim startIndex As Integer = 0
         para.replyButton = True
         para.isTrailPost = True
         If Not (Request.Item("startindex") = "") Then startIndex = CInt(Request.Item("startindex")) * (ThreadPerPage)
         For Each x In GetThreads(startIndex, ThreadPerPage - 1 + startIndex, False, True)
             Response.Write(GetStreamThreadHTML(x, para, TrailPosts))
         Next
         
     End If
     
     
%>

</div>
<div style="float: right;">
<div class="deleteform desktop">
<input type="submit" name="mode" value="<% Response.Write(reportStr) %>" /></div>
</div>
</form>
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
            Response.Write("<div><a class='form-button' href='archive.aspx?startindex=" & CStr(startIndexA - 1) & "'>" & prevStr & "</a></div>")
        End If
        Response.Write("<div class='pages'>")
        For i As Integer = 0 To (pagesCount - 1) Step 1
            If i = startIndexA Then
                Response.Write("[<strong><a href='archive.aspx?startindex=" & i & "'>" & i + 1 & "</a></strong>]")
            Else
                Response.Write("[<a href='archive.aspx?startindex=" & i & "'>" & i + 1 & "</a>]")
            End If
   
        Next
        Response.Write("</div>")
        If startIndexA = pagesCount - 1 Then ' last page
            Response.Write("<div class='next'><a class='form-button-disabled'>" & nextStr & "</a></div>")
        Else
            Response.Write("<div><a class='form-button' href='archive.aspx?startindex=" & CStr(startIndexA + 1) & "'>" & nextStr & "</a></div>")
        End If
    End If
    
    Response.Write("</div>")
    %>

<div id="bottom"></div>
<div id="absbot" class="absBotText"><% Response.Write(footerText)%></span></div>
</body>
</html>
