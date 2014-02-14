Public Class ChanbCaptcha
    Implements Captcha

    Public ReadOnly Property FormHTML() As String Implements Captcha.FormHTML
        Get
            Return ""
        End Get
    End Property

    Public ReadOnly Property QrHTML() As String Implements Captcha.QrHTML
        Get
            Return Me.FormHTML()
        End Get
    End Property

    Public ReadOnly Property ServiceName() As String Implements Captcha.ServiceName
        Get
            Return "Channel board Internal Captcha"
        End Get
    End Property

    Public ReadOnly Property ShortServiceName() As String Implements Captcha.ShortServiceName
        Get
            Return "chanb"
        End Get
    End Property

    Public Function Verify(ByVal data As CaptchaData) As CaptchaResponse Implements Captcha.Verify
        Dim re As New CaptchaResponse

        Return re
    End Function
End Class
