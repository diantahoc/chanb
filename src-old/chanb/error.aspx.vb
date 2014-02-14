Public Partial Class _error
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        With Context
            Try
                Dim r As Exception = Nothing

                Dim aq As Exception = Server.GetLastError()
                If aq IsNot Nothing Then
                    r = Server.GetLastError().GetBaseException()
                End If

                'Dim errorPageHTML As String = ""

                If r IsNot Nothing Then
                    .Response.Write((My.Resources.eq.Replace("%EM%", r.Message) _
                                         .Replace("%ES%", r.Source) _
                                         .Replace("%EST%", r.StackTrace) _
                                         .Replace("%EH%", r.HelpLink)))
                Else
                    .Response.Write((My.Resources.eq.Replace("%EM%", CStr(Session("EM"))) _
                                         .Replace("%ES%", CStr(Session("ES"))) _
                                         .Replace("%EST%", CStr(Session("EST"))) _
                                         .Replace("%EH%", CStr(Session("EH")))))
                End If

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