Public Partial Class modEditpost
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("chanb") = "chanb"
        Dim isMod As Boolean = False
        If Not GetCookie(Context, "mod") = "" Then isMod = CBool(GetCookie(Context, "mod"))
        Dim isAdmin As Boolean = False
        If Not GetCookie(Context, "admin") = "" Then isAdmin = CBool(GetCookie(Context, "admin"))

        If isMod Or isAdmin Then
            GenerateModEditPostPage(Context)
        Else
            Response.StatusCode = 403
            Response.Write("403")
            Response.End()
        End If
    End Sub

End Class