Public Partial Class _error
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With Context
            Try
                'Dim lastError As Exception = Server.GetLastError().GetBaseException
                'Dim errorPageHTML As String = ""



                .Response.Write((errorPageTemplate.Replace("%EM%", CStr(Session("EM"))) _
                                             .Replace("%ES%", CStr(Session("ES"))) _
                                             .Replace("%EST%", CStr(Session("EST"))) _
                                             .Replace("%EH%", CStr(Session("EH")))))

                '.Response.End()
            Catch ex As Exception
                .Response.Write(My.Resources.eq.Replace("%EM%", "Error occured while generating error page") _
                                            .Replace("%ES%", ex.Message) _
                                            .Replace("%EST%", ex.StackTrace) _
                                            .Replace("%EH%", String.Empty))
                .Response.End()
            End Try
        End With
    End Sub

End Class