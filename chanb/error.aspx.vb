Public Partial Class _error
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With Context
            Try
                Dim lastError As Exception = Server.GetLastError().GetBaseException
                Dim errorPageHTML As String = ""
                errorPageHTML = errorPageTemplate.Replace("%EM%", lastError.Message) _
                                             .Replace("%ES%", lastError.Source) _
                                             .Replace("%EST%", lastError.StackTrace) _
                                             .Replace("%EH%", lastError.HelpLink)
                .Response.Write(errorPageHTML)
            Catch ex As Exception
                Dim errorPageHTML As String = My.Resources.eq
                errorPageHTML = errorPageHTML.Replace("%EM%", "Error occured while generating error page") _
                                             .Replace("%ES%", ex.Message) _
                                             .Replace("%EST%", ex.StackTrace) _
                                             .Replace("%EH%", String.Empty)
                .Response.Write(errorPageHTML)
                .Response.End()
            End Try
        End With
    End Sub

End Class