Partial Public Class _adminaction
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("chanb") = "chanb"
        ' Session("admin") = CStr(True)
        If Not isInstalled Then
            InstallActions.ProcessCommand(Request, Response)
        Else ' Process normal admin actions, since chanb is marked as installed.

            If CBool(Session("admin")) = False Then
                Response.Write(FormatHTMLMessage(ForbiddenStr, "Admin privelige is required.", "", "8888", True))
            Else
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
                    Case "deletemod"
                    Case "updatemod"
                    Case "changepass"
                    Case "permadelpost"
                    Case "stickythread"
                    Case "lockthread"
                    Case "editpost"
                    Case "editimage"
                    Case "getreportoffset" 'start , end
                        Response.ContentType = "application/json"
                        Response.ContentEncoding = Text.UTF8Encoding.UTF8
                        Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(GetReports(CInt(Request.Item("st")), CInt(Request.Item("ed"))), Newtonsoft.Json.Formatting.Indented))
                End Select
            End If
        End If


    End Sub

End Class