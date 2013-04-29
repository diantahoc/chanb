<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%  
    If Session.Item("mod") Is "" Or Session.Item("mod") Is Nothing Then Session("mod") = CStr(False)
   
    If CBool (Session ("mod")) = False then
    Response.Write("Moderator privilege is required to access this page.")
    Else 
     Select Case Request.Item("action")
        Case "banpost"
            BanPosterByPost(CInt(Request.Item("postid")))
                Response.Write("Banned the poster of " & Request.Item("postid"))
            Case "delpost"
                DeletePost(Request.Item("id"), True)
                Response.Write("Delete post " & Request.Item("id"))
        Case Else
            Response.Write("invalid action")
    End Select
    End If  
        
   
%>