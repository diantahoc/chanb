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
    Public MaximumPages As Integer = 15
    Public AllowDuplicatesFiles As Boolean = False
    Public SmartLinkDuplicateImages As Boolean = True
    Public TrailPosts As Integer = 3
    Public EnableUserID As Boolean = True
    Public BumpLimit As Integer = 250
    Public ResizeMethode As Integer = 1

    'Server related
    Public STORAGEFOLDER As String = "PHYSICAL PATH TO\dtin" ' Make sure that the directory is write-able first
    Public StoragefolderWEB As String = "http://HOSTNAME/dtin/" ' The storage folder must be accessible from the web

    'Physical path , not relative, to template files.
    Public ReadOnly postTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\postTemplate.txt")
    Public ReadOnly imageTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\imageTemplate.txt")
    Public ReadOnly opPostTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\opPostTemplate.txt")
    Public ReadOnly threadTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\threadTemplate.txt")
    Public ReadOnly rotatorTemplate As String = IO.File.ReadAllText("PHYSICAL PATH TO\RotatorTemplate.txt")
    Public ReadOnly idHtml As String = "<span class='posteruid id_%UID%'>(ID: <span class='hand' title='Highlight posts by this ID'>%UID%</span>)</span>"
    Public replyButtonHTML As String = "&nbsp;<span>[<a href='%POST LINK%' class='replylink'>Reply</a>]</span>"
    Public ReadOnly noscriptItemHTML As String = "<a href='%IMAGE SRC%' target='_blank'>%FILE NAME%<br/><img src='%THUMB_LINK%' style='width: 100px; height: 100px' /></a><br/>"
    Public ReadOnly modMenu As String = "<a href='modaction.aspx?action=banpost&postid=%ID%'>Ban</a>,&nbsp;<a href='modaction.aspx?action=delpost&id=%ID%'>Delete</a>,&nbsp;<a href='modaction.aspx?action=tgsticky&id=%ID%'>Toggle sticky</a>,&nbsp;<a href='modaction.aspx?action=tglock&id=%ID%'>Toggle lock.</a>"

End Module
