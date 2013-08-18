Partial Public Class _modSelectBanReason
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session.Add("chanb", "chanb") ' To prevent session destroy   
        If chanb.isInstalled Then
            If CBool(Session("mod").ToString()) Then
                Response.Write(chanb.GlobalFunctions.GenerateModSBR(Context))
            Else
                Response.StatusCode = 403
                Response.Write(chanb.forbiddenPage)
            End If
        Else
            Response.Redirect("installer.aspx")
        End If
    End Sub

End Class