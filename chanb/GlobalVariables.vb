Public Module GlobalVariables

    'Board related
    Public BoardTitle As String = "ChanBoard"
    Public BoardLetter As String = "c"
    Public BoardDesc As String = "VB.NET Imageboard"

    'Server related
    Public STORAGEFOLDER As String = "%PhysicalPath%\dtin" ' Make sure that the directory is write-able first
    Public StoragefolderWEB As String = "/dtin/" ' the STORAGEFOLDER must be accessible from the web.
    Public MaximumFileSize As Long = 15 * 1024 * 1024 ' 15 MB


    'Physical path , not relative, to template files.
    Public ReadOnly postTemplate As String = IO.File.ReadAllText("%PhysicalPath%\postTemplate.txt")
    Public ReadOnly imageTemplate As String = IO.File.ReadAllText("%PhysicalPath%\imageTemplate.txt")

End Module
