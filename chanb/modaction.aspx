<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>

    <%  
        
    If Session.Item("mod") Is "" Or Session.Item("mod") Is Nothing Then Session("mod") = CStr(False)
   
    If CBool (Session ("mod")) = False then
    Response.Write("Moderator privilege is required to access this page.")
    Else
            Dim powers As String() = CStr(Session("modpowers")).Split(CChar("-"))
            Select Case Request.Item("action")
                Case "banpost"
                    If powers(0) = "0" Then
                        Response.Write("You have no power to do that")
                    Else
                        BanPosterByPost(CInt(Request.Item("id")))
                        Response.Write("Banned the poster of " & Request.Item("id"))
                    End If
                Case "delpost"
                    If powers(1) = "0" Then
                        Response.Write("You have no power to do that")
                    Else
                        PrunePost(Request.Item("id"), deleteFiles)
                        Response.Write("Deleted post " & Request.Item("id"))
                    End If
                Case "tgsticky"
                    If powers(2) = "0" Then
                        Response.Write("You have no power to do that")
                    Else
                        ToggleSticky(CInt(Request.Item("id")))
                        Response.Write("Request complete")
                    End If
                Case "tglock"
                    If powers(3) = "0" Then
                        Response.Write("You have no power to do that")
                    Else
                        ToggleLock(CInt(Request.Item("id")))
                        Response.Write("Request complete")
                    End If
                Case "editpost"
                    If powers(4) = "0" Then
                        Response.Write("You have no power to do that")
                    Else
                        'nod
                    End If
                Case Else
                    Response.Write("invalid action")
            End Select
    End If
    %>