Public Partial Class modEditpost
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        GenerateModEditPostPage(Context)
    End Sub

End Class