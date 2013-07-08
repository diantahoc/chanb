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
#If TripCodeSupport Then
     If _name.Contains("#") And Not _name.Contains("##") Then _name = _name.Replace(GetTrip(_name), "!" & ComputeTrip(GetTrip(_name)))
#End If
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

#If TripCodeSupport Then
    Private Function GetTrip(ByVal s As String) As String
        Dim hashtagPostion As Integer
        Dim doubleHashtags As Integer

        For i As Integer = 0 To s.Length - 1 Step 1
            If s(i) = "#" Then
                hashtagPostion = i
            End If
            Try
                If i < s.Length - 1 Then
                    If s(i) = "#" And s(i + 1) = "#" Then doubleHashtags = i Else doubleHashtags = s.Length - 1
                End If
            Catch ex As Exception
                doubleHashtags = s.Length - 1
            End Try

        Next

        Dim tripcode As New StringBuilder

        For i As Integer = hashtagPostion To doubleHashtags Step 1
            tripcode.Append(s(i))
        Next
        Dim tr As String = tripcode.ToString
        Return tr
    End Function

    Private normalSalt As Char() = "./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray
    'Private hiddenSalt As Char() = MD5(normalSalt.ToString).ToCharArray

    Private Function ComputeTrip(ByVal tripc As String) As String
        Return UnixCrypt.Crypt(tripc.Substring(1, 2), tripc).Substring(3)
    End Function
#End If

End Class
