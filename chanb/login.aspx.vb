Partial Public Class _login
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("chanb") = "chanb"
        If (Not Request.Item("name") = "") And (Not Request.Item("pass") = "") Then
            Dim lgi As chanb.LoginInfo = GetLoginInfo(Request.Item("name"), Request.Item("pass"))
            If lgi.LogInValid Then
                Select Case lgi.AccountType
                    Case chanb.LoginInfo.ChanbAccountType.Administrator
                        Session("admin") = CStr(True)
                    Case chanb.LoginInfo.ChanbAccountType.Moderator
                        Session("mod") = CStr(True)
                        Session("modname") = Request.Item("name")
                End Select
                Session("credpower") = lgi.Powers
                Session("credmenu") = GetModeratorHTMLMenu(lgi.Powers) ' I don't know if it is a good idea to store html in the session, but I don't see a reason to not do it.
                Response.Redirect(WebRoot & "default.aspx", False)
            Else
                Response.Write(FormatHTMLMessage(Language.ForbiddenStr, Language.modLoginFailed, "", "8888", False))
                Response.End()
            End If
        End If
    End Sub

End Class