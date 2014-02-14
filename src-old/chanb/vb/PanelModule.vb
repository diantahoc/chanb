Public Module PanelModule

    Public Sub InitPanel(ByVal Request As HttpRequest, ByVal Session As HttpSessionState, ByVal Response As HttpResponse)
        If Not (CBool(Session("mod")) Or CBool(Session("admin"))) Then
            Response.StatusCode = 403
            Response.Write(ForbiddenStr)
            Response.End()
        End If

        If CBool(Session("mod")) Then


            Dim tem As String = PreparePanelTemplate()

            tem = tem.Replace("%BODY%", getModPanel)
            Response.Write(tem)
        End If
    End Sub

    Private Function getModPanel() As String

        Return IO.File.ReadAllText("C:\Users\Istan\Documents\Visual Studio 2008\Projects\tinyboardasp\tinyboardasp\modPanel.html")
    End Function

    Private Function PreparePanelTemplate() As String
        Dim s As String = IO.File.ReadAllText("C:\Users\Istan\Documents\Visual Studio 2008\Projects\tinyboardasp\tinyboardasp\panelTemplate.html")
        s = s.Replace("%TITLE%", "Channel Board Panel")
        s = s.Replace("%ROOT%", WebRoot)
        Return s
    End Function

End Module
