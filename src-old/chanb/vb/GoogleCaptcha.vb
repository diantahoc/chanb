Public Class GoogleCaptcha
    Implements Captcha

    Dim _privateKey As String = ""
    Dim _publicKey As String = ""

    Public ReadOnly Property FormHTML() As String Implements Captcha.FormHTML
        Get
            Return "<div><script>var RecaptchaOptions={theme:'clean'};</script><script type=""text/javascript"" src=""%proto%://www.google.com/recaptcha/api/challenge?k=%your_public_key%""></script><noscript><iframe src=""%proto%://www.google.com/recaptcha/api/noscript?k=%your_public_key%"" height=""300"" width=""500"" frameborder=""0""></iframe><br><textarea name=""recaptcha_challenge_field"" rows=""3"" cols=""40""></textarea><input type=""hidden"" name=""recaptcha_response_field"" value=""manual_challenge""></noscript></div>".Replace("%proto%", HttpProtocolString).Replace("%your_public_key%", _publicKey)
        End Get
    End Property

    Public ReadOnly Property QrHTML() As String Implements Captcha.QrHTML
        Get
            Return "<iframe src=""%proto%://www.google.com/recaptcha/api/noscript?hl=en&k=%your_public_key%"" frameborder=""0""></iframe><br><textarea name=""recaptcha_challenge_field"" rows=""3"" cols=""40""></textarea><input type=""hidden"" name=""recaptcha_response_field"" value=""manual_challenge"">".Replace("%proto%", HttpProtocolString).Replace("%your_public_key%", _publicKey)
            '  Return "<div><div id=""qr_div_recaptcha""></div> <script type=""text/javascript"" src=""%proto%://www.google.com/recaptcha/api/js/recaptcha_ajax.js""></script><script>Recaptcha.create(""%your_public_key%"",""qr_div_recaptcha"",{theme:""red"",callback:Recaptcha.focus_response_field});</script></div>".Replace("%proto%", HttpProtocolString).Replace("%your_public_key%", _publicKey)
        End Get
    End Property

    Public ReadOnly Property ServiceName() As String Implements Captcha.ServiceName
        Get
            Return "Google reCAPTCHA"
        End Get
    End Property

    Public ReadOnly Property ShortServiceName() As String Implements Captcha.ShortServiceName
        Get
            Return "google"
        End Get
    End Property

    Public Function Verify(ByVal data As CaptchaData) As CaptchaResponse Implements Captcha.Verify
        Dim re As New CaptchaResponse()

        If data.CaptchaChallenge.Length = 0 Or data.SolverResponse.Length = 0 Then
            re.Correct = False
            re.ErrorMessage = "Unsolved captcha"
            Return re
        End If


        Dim req As New Net.WebClient()
        req.Encoding = Text.Encoding.UTF8

        'privatekey (required) 	Your private key
        'remoteip (required) 	The IP address of the user who solved the CAPTCHA.
        'challenge (required) 	The value of "recaptcha_challenge_field" sent via the form
        'response (required) 	The value of "recaptcha_response_field" sent via the form

        Dim postData As New StringBuilder

        req.Headers(Net.HttpRequestHeader.ContentType) = "application/x-www-form-urlencoded"


        postData.AppendFormat("{0}={1}", "privatekey", _privateKey)

        postData.AppendFormat("&{0}={1}", "remoteip", data.SolverIP)

        postData.AppendFormat("&{0}={1}", "challenge", data.CaptchaChallenge)

        postData.AppendFormat("&{0}={1}", "response", data.SolverResponse)


        Dim response As String = ""

        Try
            response = req.UploadString("http://www.google.com/recaptcha/api/verify", "POST", postData.ToString())

            Dim d As String() = response.Split(CChar(vbLf))

            Select Case d(0).ToLower()
                Case "true"
                    re.Correct = True
                    re.ErrorMessage = ""
                Case "false"
                    re.Correct = False
                    re.ErrorMessage = format_google_reCa_response(d(1))
                Case Else
                    re.Correct = False
                    re.ErrorMessage = "Invalid reCAPTCHA API response {0}".Replace("0", response)
            End Select

        Catch ex As Exception
            re.Correct = False
            re.ErrorMessage = "Cannot verify this captcha because WebClient error {0} ".Replace("0", ex.Message) & vbNewLine & " Inner Exception {0}".Replace("0", If(ex.InnerException Is Nothing, "None", ex.InnerException.Message))
        End Try

        req.Dispose()

        Return re
    End Function

    Private Function format_google_reCa_response(ByVal a As String) As String
        Select Case a.ToLower()
            Case "invalid-site-private-key"
                Return "reCAPTCHA private key error."

            Case "invalid-request-cookie"
                Return "The challenge parameter of the verify script was incorrect."

            Case "incorrect-captcha-sol"
                Return "The reCAPTCHA solution was incorrect."

            Case "captcha-timeout"
                Return "This reCAPTCHA has expired."

            Case Else
                Return "Unkown reCAPTCHA error message {0}.".Replace("0", a)
        End Select
    End Function

    Sub New(ByVal privateKey As String, ByVal publicKey As String)
        If Not privateKey.Length = 40 Then
            Throw New ArgumentException("Invalid private key length")
        Else
            _privateKey = privateKey
        End If

        If publicKey.Length = 0 Then
            Throw New ArgumentException("Invalid public key length")
        Else
            _publicKey = publicKey
        End If

    End Sub

End Class