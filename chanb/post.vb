Public Class WPost
    'Public _imageP As String = ""
    Public FileCount As Integer = 0
    Public files As WPostImage() = {}
    Public HasFile As Boolean = False
    Public comment As String = ""
    Public ip As String = ""
    Public subject As String = ""
    Public email As String = ""
    Public name As String = ""
    Public password As String = ""
    Public time As Date
    Public PostID As Integer
    Public type As PostType = PostType.Unknown
    Public parent As Integer
    Public ua As String = ""
    Public posterID As String = ""
    Public isSticky As Boolean
    Public locked As Boolean
    Public archived As Boolean
    Sub New(ByVal _id As Integer)
        PostID = _id
    End Sub

    Public Enum PostType As Integer

        Unknown = -1
        Thread = 0
        Reply = 1

    End Enum
End Class

