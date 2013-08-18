﻿Public Partial Class _default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session.Add("chanb", "chanb") ' To prevent session destroy   
        If chanb.isInstalled Then
            Response.Write(chanb.GlobalFunctions.GeneratePageHTML(False, Session, Request, Response))
        Else
            Response.Redirect("installer.aspx")
        End If
    End Sub

End Class