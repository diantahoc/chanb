Partial Public Class _archive
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session.Add("chanb", "chanb")
        If chanb.isInstalled Then
            Response.Write(chanb.GlobalFunctions.GeneratePageHTML(True, Session, Request, Response))
        Else
            Response.Redirect("installer.aspx")
        End If
    End Sub

End Class