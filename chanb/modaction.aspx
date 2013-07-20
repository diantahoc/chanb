<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace = "chanb.Misc" %>
<%@ Import Namespace = "chanb.Language" %>
<%@ Page Language="VB" %>

    <%  
        
    If Session.Item("mod") Is "" Or Session.Item("mod") Is Nothing Then Session("mod") = CStr(False)
   
        If CBool(Session("mod")) = False Then
            Response.StatusCode = 403
            Response.Write(FormatHTMLMessage(ForbiddenStr, modRequired, "", "8888", True))
        Else
            Dim powers As String() = CStr(Session("credpower")).Split(CChar("-"))
            Dim id As Integer = 0
            Try
                id = CInt(Request.Item("id"))
            Catch ex As Exception
                Response.Write(FormatHTMLMessage(errorStr, invalidIdStr, "", "4444", True))
                Response.End()
            End Try
            Select Case Request.Item("action")
                Case "banpost"
                    If powers(0) = "0" Then
                        Response.Write(FormatHTMLMessage(ForbiddenStr, modNoPower, "", "8888", True))
                    Else
                        Dim silentBan As Boolean = False
                        Try
                            silentBan = CBool(Request.Item("sib"))
                        Catch ex As Exception
                            silentBan = False
                        End Try
                        BanPosterByPost(id, silentBan, Session("modname"), Request.Item("reason"))
                        Response.Write(FormatHTMLMessage(sucessStr, modBannedPosterStr.Replace("%", CStr(id)), "", "5555", False))
                    End If
                Case "delpost"
                    If powers(1) = "0" Then
                        Response.Write(FormatHTMLMessage(ForbiddenStr, modNoPower, "", "8888", True))
                    Else
                        PrunePost(id, AutoDeleteFiles)
                        Response.Write(FormatHTMLMessage(sucessStr, PostDeletedSuccess.Replace("%", CStr(id)), "", "88888", False))
                    End If
                Case "tgsticky"
                    If powers(2) = "0" Then
                        Response.Write(FormatHTMLMessage(ForbiddenStr, modNoPower, "", "8888", True))
                    Else
                        ToggleSticky(id)
                        Response.Write(FormatHTMLMessage(sucessStr, modRequetComplete, "", "8888", False))
                    End If
                Case "tglock"
                    If powers(3) = "0" Then
                        Response.Write(FormatHTMLMessage(ForbiddenStr, modNoPower, "", "8888", True))
                    Else
                        ToggleLock(id)
                        Response.Write(FormatHTMLMessage(sucessStr, modRequetComplete, "", "8888", False))
                    End If
                Case "editpost"
                    If powers(4) = "0" Then
                        Response.Write(FormatHTMLMessage(ForbiddenStr, modNoPower, "", "8888", True))
                    Else
                        'nod
                    End If
                Case Else
                    Response.Write(forbiddenPage)
            End Select
        End If
    %>