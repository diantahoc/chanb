Public Module GlobalVariables

    'Board related
    Public BoardTitle As String = "ChanBoard"
    Public BoardLetter As String = "c"
    Public BoardDesc As String = "VB.NET Imageboard"
    Public TimeBetweenRequestes As Integer = 15 ' in seconds

    'Server related
    Public STORAGEFOLDER As String = "PHYSICAL PATH TO\dtin" ' Make sure that the directory is write-able first
    Public StoragefolderWEB As String = "/dtin/" ' The STORAGE FOLDER MUST BE ACCESSIBLE FROM THE WEB
    Public MaximumFileSize As Long = 15 * 1024 * 1024 ' 15 MB

    Public replyButtonHTML As String = "&nbsp;<span>[<a href='%POST LINK%' class='replylink'>Reply</a>]</span>"

    'Physical path , not relative, to template files.
    Public ReadOnly postTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\postTemplate.txt")
    Public ReadOnly imageTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\imageTemplate.txt")
    Public ReadOnly opPostTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\opPostTemplate.txt")
    Public ReadOnly threadTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\threadTemplate.txt")


End Module
