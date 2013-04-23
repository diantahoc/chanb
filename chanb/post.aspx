<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.Language" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>

<% 
    If Session.Item("lastpost") = "" Then
        Session.Item("lastpost") = Now.ToString
        Response.Write(ProcessPost(Request, Session))
       
    Else
        
        Dim i As Date = Date.Parse(Session.Item("lastpost"))
    
        If (Now - i).TotalSeconds < TimeBetweenRequestes Then
            Response.Write(FloodDetected)
        Else
            Response.Write(ProcessPost(Request, Session))
            Session.Item("lastpost") = Now.ToString
        End If
        
    End If
 %>