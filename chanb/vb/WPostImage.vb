Public Class WPostImage
    Private _cn As String
    Private _iwp As String
    Private _twp As String

    Public Property ChanbName() As String
        Get
            Return _cn
        End Get
        Set(ByVal value As String)
            _cn = value
            _iwp = GetImageWEBPATH(value)
            _twp = GetImageWEBPATHRE(value)
        End Set
    End Property

    Public ReadOnly Property ImageWebPath() As String
        Get
            Return _iwp
        End Get
    End Property

    Public ReadOnly Property ImageThumbailWebPath() As String
        Get
            Return _twp
        End Get
    End Property

    Public Size As Long
    Public RealName As String
    Public Dimensions As String
    Public MD5 As String
    Public Extension As String
    Public PostID As Integer
    Public MimeType As String
    Public _isDummy As Boolean = False
End Class
