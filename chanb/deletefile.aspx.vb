Public Partial Class deletefile
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session.Add("chanb", "chanb") ' To prevent session destroy   
        If chanb.isInstalled Then
            chanb.GlobalFunctions.GenerateDeleteFilePage(Context)
        Else
            Response.Redirect("installer.aspx")
        End If
    End Sub

End Class