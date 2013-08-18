Imports System.Web.SessionState

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application is started
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
        CronJob()
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs
        With Context
            Try
                Dim lastError As Exception = Server.GetLastError().GetBaseException()
                Dim errorPageHTML As String = ""
                errorPageHTML = errorPageTemplate.Replace("%EM%", lastError.Message) _
                                             .Replace("%ES%", lastError.Source) _
                                             .Replace("%EST%", lastError.StackTrace) _
                                             .Replace("%EH%", lastError.HelpLink)
                .Response.Write(errorPageHTML)
            Catch ex As Exception
                .Response.Write(My.Resources.eq.Replace("%EM%", "Error occured while generating error page") _
                                               .Replace("%ES%", ex.Message) _
                                               .Replace("%EST%", ex.StackTrace) _
                                               .Replace("%EH%", String.Empty))
                .Response.End()
            End Try
        End With
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub

    Private Sub CronJob()
        Dim requestCount As Integer = 0
        If FileIO.FileSystem.FileExists(requestCountF) Then
            Try
                requestCount = CInt(IO.File.ReadAllText(requestCountF))
                requestCount += 1
                IO.File.WriteAllText(requestCountF, CStr(requestCount))
            Catch ex As Exception
            End Try
        Else
            IO.File.WriteAllText(requestCountF, "1")
        End If

        If requestCount > 100 Then
            'Do stuffs
            UpdateBanFile()

            'reset request count
            IO.File.WriteAllText(requestCountF, "0")
        End If

    End Sub

End Class