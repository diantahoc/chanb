Partial Public Class _modSelectBanReason
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session.Add("chanb", "chanb") ' To prevent session destroy   
        If chanb.isInstalled Then
            Dim isMod As Boolean = False
            If Not GetCookie(Context, "mod") = "" Then isMod = CBool(GetCookie(Context, "mod"))
            Dim isAdmin As Boolean = False
            If Not GetCookie(Context, "admin") = "" Then isAdmin = CBool(GetCookie(Context, "admin"))

            If isAdmin Or isMod Then
                Response.Write(chanb.GlobalFunctions.GenerateModSBR(Context))
            Else
                Response.StatusCode = 403
                Response.End()
            End If
        Else
            Response.Redirect(WebRoot & "installer.aspx")
        End If
    End Sub

End Class