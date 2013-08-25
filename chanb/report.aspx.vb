Partial Public Class _report
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("chanb") = "chanb" ' To prevent session destroy   
        If chanb.isInstalled Then
            Response.Write(chanb.GlobalFunctions.GenerateReportPage(Context))
        Else
            Response.Redirect("installer.aspx")
        End If
    End Sub

End Class