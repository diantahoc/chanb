Public Partial Class modEditpost
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("chanb") = "chanb"
        If CBool(Session("mod")) Then
            GenerateModEditPostPage(Context)
        Else
            Response.StatusCode = 403
            Response.End()
        End If
    End Sub

End Class