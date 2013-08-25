Partial Public Class _panel
    Inherits System.Web.UI.Page
    Public ChanbVersion As String = GetChanbVersion()
    Public TotalPostCount As Integer = CountTotalPost()
    Public TotalUsers As Integer = CountTotalUsers()
    Public TotalFiles As Integer = CountTotalFiles()
    Public reportBoxContent As String = ""
    Public wb As String = WebRoot
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("chanb") = "chanb"
        '  Session("admin") = CStr(True)

        'lets fill the report box
        Dim repo As New Text.StringBuilder
        repo.Append("<table><tr><td>ID</td><td>Reported on</td><td>Reporter IP</td><td>Reported Post no.</td><td>Reason</td></tr></table>")
        repo.Append("<table id='reportsdata'>")
        Dim reports As Report() = GetReports(0, 10)
        For i As Integer = 0 To reports.Length - 1 Step 1
            repo.Append("<tr><td>%ID%</td><td>%DATE%</td><td>%REPORTERIP%</td><td>%POST%</td><td>%COMMENT%</td></tr>" _
                        .Replace("%ID%", CStr(reports(i).ReportID)) _
                        .Replace("%DATE%", GlobalFunctions.GetTimeString(reports(i).Time)) _
                        .Replace("%REPORTERIP%", reports(i).ReporterIP) _
                        .Replace("%POST%", CStr(reports(i).PostID)) _
                        .Replace("%COMMENT%", reports(i).ReportComment))
        Next
        repo.Append("</table>")
        reportBoxContent = repo.ToString
        reports = Nothing
    End Sub



End Class