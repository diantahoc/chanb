Public Partial Class dscripts
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        WriteScriptDocument(Context)
    End Sub

    Private Sub WriteScriptDocument(ByVal context As HttpContext)
        With context
            .Response.Cache.SetExpires(DateTime.Now.AddDays(3))
            .Response.ContentType = "application/javascript"
            .Response.ContentEncoding = Text.Encoding.UTF8

            Dim sb As New Text.StringBuilder

            sb.Append(MakeScriptVariable("replyTemplate", ReplyPostTemplate _
                                                .Replace("%LANG reportStr%", reportStr) _
                                                .Replace("%LANG deleteStr%", deleteStr) _
                                                .Replace("%ROOT%", WebRoot)))

            If Not context.Session("credmenu") Is Nothing Then sb.Append(MakeScriptVariable("modpanel", CStr(context.Session("credmenu")))) Else sb.Append(MakeScriptVariable("modpanel", ""))

            sb.Append(MakeScriptVariable("uihs", GlobalVariables.UserIDHtmlSPAN))

            sb.Append(MakeScriptVariable("FilesRotatorTemplate", FilesRotatorTemplate))

            sb.Append(MakeScriptVariable("ImageTemplate", ImageTemplate))

            sb.Append(MakeScriptVariable("webroot", WebRoot))

            sb.Append(MakeScriptVariable("VideoItemTemplate", VideoItemTemplate))

            sb.Append(MakeScriptVariable("AudioItemTemplate", AudioItemTemplate))

            sb.Append("var selinks = " & JSONApi.GetSELinks & ";")


            .Response.Write(sb.ToString)
        End With
    End Sub

    Private Function MakeScriptVariable(ByVal name As String, ByVal content As String) As String
        Return "var " & name & " = """ & content.Replace("""", "\""") & """;"
    End Function

End Class