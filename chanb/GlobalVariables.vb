Public Module GlobalVariables

    'Board related
    Public BoardTitle As String = "ChanBoard"
    Public BoardLetter As String = "c"
    Public BoardDesc As String = "VB.NET Imageboard"
    Public TimeBetweenRequestes As Integer = 15 ' in seconds
    Public footerText As String = "<a href='https://github.com/diantahoc/chanb' target='_blank'>ChanB</a> ASP.NET board."
    Public MaximumFileSize As Long = 15 * 1024 * 1024 ' 15 MB
    Public DeleteFiles As Boolean = True
    Public ThreadPerPage As Integer = 10
    Public AllowDuplicatesFiles As Boolean = False
    Public SmartLinkDuplicateImages As Boolean = True

    'Server related
    Public STORAGEFOLDER As String = "PHYSICAL PATH TO\dtin" ' Make sure that the directory is write-able first
    Public StoragefolderWEB As String = "http://HOSTNAME/dtin/" ' The storage folder must be accessible from the web

    Public replyButtonHTML As String = "&nbsp;<span>[<a href='%POST LINK%' class='replylink'>Reply</a>]</span>"

    'Physical path , not relative, to template files.
    Public ReadOnly postTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\postTemplate.txt")
    Public ReadOnly imageTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\imageTemplate.txt")
    Public ReadOnly opPostTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\opPostTemplate.txt")
    Public ReadOnly threadTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\threadTemplate.txt")
    Public ReadOnly rotatorTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\RotatorTemplate.txt")

End Module
