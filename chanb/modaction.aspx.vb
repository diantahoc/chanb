Partial Public Class _modaction
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim isMod As Boolean = False
        If Not GetCookie(Context, "mod") = "" Then isMod = CBool(GetCookie(Context, "mod"))

        If isMod Then
            Dim id As Integer = 0
            Try
                id = CInt(Request.Item("id"))
            Catch ex As Exception
                Response.Write(FormatHTMLMessage(errorStr, invalidIdStr, "", "4444", True))
                Response.End()
            End Try
            Dim powers As String() = GetCookie(Context, "credpower").Split(CChar("-"))
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
                        BanPosterByPost(id, silentBan, GetCookie(Context, "modname"), Request.Item("reason"))
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
                        If Request.Item("new_data") Is Nothing Or Request.Item("new_data") = String.Empty Then
                            Response.Redirect(WebRoot & "modEditpost.aspx?id=" & id)
                        Else
                            Dim newData As String = Request.Item("new_data")
                            GlobalFunctions.UpdatePostText(id, newData, False)
                            Response.Write(FormatHTMLMessage(sucessStr, modRequetComplete, "", "8888", False))
                        End If
                    End If
                Case Else
                    Response.StatusCode = 403
                    Response.End()
            End Select
        Else
            Response.StatusCode = 403
            Response.Write(FormatHTMLMessage(ForbiddenStr, modRequired, "", "8888", True))
        End If
    End Sub

End Class