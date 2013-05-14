Public Class OPData

    Dim _Comment As String
    Dim _name As String
    Dim _password As String
    Dim _subject As String
    Dim _email As String

    Public Property Comment() As String
        Get
            Return _Comment
        End Get
        Set(ByVal value As String)
            _Comment = value
        End Set
    End Property

    Public Property name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            If value.Length > 1000 Then
                _name = value.Remove(999)
            Else
                _name = value
            End If
        End Set
    End Property

    Public Property email() As String
        Get
            Return _email
        End Get
        Set(ByVal value As String)
            If value.Length > 1000 Then
                _email = value.Remove(999)
            Else
                _email = value
            End If
        End Set
    End Property

    Public Property subject() As String
        Get
            Return _subject
        End Get
        Set(ByVal value As String)
            If value.Length > 1000 Then
                _subject = value.Remove(999)
            Else
                _subject = value
            End If
        End Set
    End Property

    Public Property password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            If value.Length > 1000 Then
                _password = value.Remove(999)
            Else
                _password = value
            End If
        End Set
    End Property

    Public time As Date
    Public imageName As String
    Public IP As String
    Public UserAgent As String

End Class
