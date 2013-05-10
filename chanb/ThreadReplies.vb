Public Class ThreadReplies
    Public TextReplies As Integer
    Public ImageReplies As Integer

    Public ReadOnly Property TotalReplies() As Integer
        Get
            Return TextReplies + ImageReplies
        End Get
    End Property

End Class
