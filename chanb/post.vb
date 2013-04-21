Public Class WPost

    Public _imageP As String
    Public comment As String
    Public ip As String
    Public subject As String
    Public email As String
    Public name As String
    Public password As String
    Public time As Date
    Public PostID As Long
    Public type As String
    Public parent As Int32

    Sub New(ByVal _id As Long)
        PostID = _id
    End Sub



End Class
