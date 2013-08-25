Public Class Report

    Private _ip As String
    Private _ReportID As Integer

    Public ReadOnly Property ReporterIP() As String
        Get
            Return _ip
        End Get
    End Property

    Public PostID As Integer
    Public Time As Date
    Public ReportComment As String

    Public ReadOnly Property ReportID() As Integer
        Get
            Return _ReportID
        End Get
    End Property

    Sub New(ByVal IP As String, ByVal id As Integer)
        _ip = IP : _ReportID = id
    End Sub

End Class