Imports chanb.GlobalVariables

Partial Public Class _installer
    Inherits System.Web.UI.Page
    Public StorageFolder As String = GlobalVariables.StorageFolder
    Public StorageFolderWeb As String = GlobalVariables.StoragefolderWEB
    Public BoardTitle As String = GlobalVariables.BoardTitle
    Public BoardDesc As String = GlobalVariables.BoardDesc
    Public TimeBetweenRequestes As String = CStr(GlobalVariables.TimeBetweenRequestes)
    Public MaximumFileSize As Long = GlobalVariables.MaximumFileSize
    Public ThreadPerPage As Integer = GlobalVariables.ThreadPerPage
    Public MaximumPages As Integer = GlobalVariables.MaximumPages
    Public TrailPosts As Integer = GlobalVariables.TrailPosts
    Public BumpLimit As Integer = GlobalVariables.BumpLimit

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If isInstalled Then
            Response.StatusCode = 403
            Response.Write("Forbidden")
            Response.End()
        Else
            Session("admin") = CStr(True)
        End If
    End Sub
End Class