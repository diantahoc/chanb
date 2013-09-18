Partial Public Class _adminaction
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("chanb") = "chanb"
        If Not isInstalled Then
            InstallActions.ProcessCommand(Request, Response)
        Else
            ' Process normal admin actions, since chanb is marked as installed.
            Dim isAdmin As Boolean = False
            If Not GetCookie(Context, "admin") = "" Then isAdmin = CBool(GetCookie(Context, "admin"))
            If isAdmin Then
                Select Case Request.Item("action")
                    Case "updatesettings"
                        Try
                            Dim stmanager As New DataInitializer()
                            stmanager.UpdateSetting(Request.Item("stname"), Request.Item("stvalue"))
                            Response.Write("OK")
                        Catch ex As Exception
                            Response.Write(ex.Message)
                        End Try
                    Case "newmod"
                        Try
                            NewMod(Request.Item("newmodname"), Request.Item("newmodpass"), Request.Item("newmodpowers"))
                            Response.Write("OK")
                        Catch ex As Exception
                            Response.Write(ex.Message)
                        End Try
                    Case "purgeall"
                        PermaDeleteAllPosts(True)
                    Case "deletemod"
                        Dim modName As String = Request.Item("modname")
                        If modName = "" Then
                            Response.Write("Bad mod name")
                            Response.End()
                        ElseIf modName = GetCookie(Context, "aname") Then
                            Response.Write("Cannot delete the administrator")
                            Response.End()
                        Else
                            DeleteMod(modName)
                            Response.Write("ok")
                            Response.End()
                        End If
                    Case "updatemod"
                        'mod name
                        'powers or password
                        'new value
                        Dim modName As String = Request.Item("modname")
                        If modName = "" Then
                            Response.Write("Bad mod name")
                        ElseIf modName = GetCookie(Context, "aname") Then
                            Response.Write("Cannot delete the administrator")
                            Response.End()
                        Else
                            Dim action As Integer = CInt(Request.Item("upd"))
                            If action <= 0 Then
                                Response.Write("Bad update action")
                                Response.End()
                            Else
                                Dim newValue As String = Request.Item("newval")
                                If newValue = "" Then
                                    Response.Write("Bad new value")
                                Else
                                    Try
                                        UpdateMod(modName, action, newValue)
                                        Response.Write("update ok")
                                        Response.End()
                                    Catch ex As Exception
                                        Response.Write(ex.Message)
                                        Response.End()
                                    End Try
                                End If
                            End If
                        End If
                    Case "banpostercc"
                        Dim id As Integer = CInt(Request.Item("id"))
                        If id <= 0 Then
                            Response.Write("bad id")
                        Else
                            Dim isSilent, reason, perm, days, canbrowse As String

                            isSilent = Request.Item("silent")
                            reason = Request.Item("reason") 'can be null
                            perm = Request.Item("perm")
                            days = Request.Item("days")
                            canbrowse = Request.Item("canbrowse")

                            If isSilent = "" Then
                                Response.Write("Missing silent parameter")
                                Response.End()
                            Else
                                Dim sil As Boolean = False
                                If isSilent = "yes" Then
                                    sil = True
                                End If
                                Dim permb As Boolean = False
                                If perm = "yes" Then
                                    permb = True
                                End If
                                Dim daysC As Integer = -1
                                Try
                                    daysC = CInt(days)
                                Catch ex As Exception
                                    daysC = -1
                                End Try
                                If daysC <= 0 And Not permb Then
                                    Response.Write("Need ban length")
                                    Response.End()
                                Else
                                    Dim canBr As Boolean = True
                                    If canbrowse = "no" Then
                                        canBr = False
                                    End If

                                    BanPosterByPost_CR(id, sil, reason, GetCookie(Context, "aname"), permb, Now + New TimeSpan(daysC, 0, 0, 0), canBr)
                                    Response.Write("ok")
                                End If
                            End If
                        End If
                    Case "banpost"
                        Dim id As Integer = CInt(Request.Item("id"))
                        If id <= 0 Then
                            Response.Write("bad id")
                        Else
                            Dim silentBan As Boolean = False
                            Try
                                silentBan = CBool(Request.Item("sib"))
                            Catch ex As Exception
                                silentBan = False
                            End Try
                            BanPosterByPost(id, silentBan, GetCookie(Context, "aname"), Request.Item("reason"))
                            Response.Write(FormatHTMLMessage(sucessStr, modBannedPosterStr.Replace("%", CStr(id)), "", "5555", False))
                        End If
                    Case "changepass" 'for the admin
                        Dim newPass As String = Request.Item("newpass")
                        If newPass = "" Or newPass.Length < 8 Then
                            Response.Write("Insecure password, try again")
                        Else
                            UpdateMod(GetCookie(Context, "aname"), 1, newPass)
                            Response.Write("ok")
                        End If
                    Case "permadelpost"
                        Dim id As Integer = CInt(Request.Item("id"))
                        If id <= 0 Then
                            Response.Write("bad id")
                        Else
                            PrunePost(id, True, True)
                            Response.Write("ok")
                        End If
                        'delete post from db even if archive is enabled
                    Case "tgsticky"
                        Dim id As Integer = CInt(Request.Item("id"))
                        If id <= 0 Then
                            Response.Write("bad id")
                        Else
                            ToggleSticky(id)
                            Response.Write("ok")
                        End If
                    Case "tglock"
                        Dim id As Integer = CInt(Request.Item("id"))
                        If id <= 0 Then
                            Response.Write("bad id")
                        Else
                            ToggleLock(id)
                            Response.Write("ok")
                        End If
                    Case "editpost"
                        Dim id As Integer = CInt(Request.Item("id"))
                        If id <= 0 Then
                            Response.Write("bad id")
                        Else
                            If Request.Item("new_data") Is Nothing Or Request.Item("new_data") = String.Empty Then
                                Response.Redirect(WebRoot & "modEditpost.aspx?shw=yes&id=" & id)
                            Else
                                Dim newData As String = Request.Item("new_data")
                                GlobalFunctions.UpdatePostText(id, newData, True)
                                Response.Write("ok")
                            End If
                        End If
                    Case "editimage"
                        'replace file with new one
                    Case "getreportoffset" 'start , end
                        Response.ContentType = "application/json"
                        Response.ContentEncoding = Text.UTF8Encoding.UTF8
                        Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(GetReports(CInt(Request.Item("st")), CInt(Request.Item("ed"))), Newtonsoft.Json.Formatting.Indented))
                End Select
            Else
                Response.StatusCode = 403
                Response.Write(FormatHTMLMessage(ForbiddenStr, "Admin privelige is required.", "", "8888", True))
            End If

        End If


    End Sub

End Class