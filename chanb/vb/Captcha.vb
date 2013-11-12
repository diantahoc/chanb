Public Interface Captcha

    ReadOnly Property FormHTML() As String

    ReadOnly Property ServiceName() As String

    ReadOnly Property ShortServiceName() As String

    Function Verify(ByVal data As CaptchaData) As CaptchaResponse

    ReadOnly Property QrHTML() As String


End Interface

Public Class CaptchaResponse

    Public Correct As Boolean = False
    Public ErrorMessage As String = ""

End Class

Public Class CaptchaData

    Public SolverIP As String

    ''' <summary>
    ''' The captcha session challenge
    ''' </summary>
    ''' <remarks></remarks>
    Public CaptchaChallenge As String

    Public SolverResponse As String

End Class