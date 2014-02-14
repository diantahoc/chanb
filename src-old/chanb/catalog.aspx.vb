Partial Public Class _catalog
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("chanb") = "chanb"
        Response.Write(chanb.GenerateCatalogPage(Request, Session))
    End Sub

End Class