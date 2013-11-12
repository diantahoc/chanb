Imports Microsoft.VisualBasic.FileIO

Friend Module GlobalVariables

    Private DI As New DataInitializer()

    Sub New()
        StorageFolder = DI.PhysicalStorageFolderPath
        StorageFolderThumbs = CombinePath(DI.PhysicalStorageFolderPath, "thumbs")

        CheckDir(StorageFolder)
        CheckDir(StorageFolderThumbs)
        CheckDir(ThreadStorageFolder)
        CheckDir(ArchivedTStorageFolder)
        CheckDir(dataFileDir)

        If StaticHTML Then ConvertArchivedThreadToHTML = True

        InitVariables()
    End Sub

#Region "Board configuration"

    Public BoardTitle As String = DI.BoardTitle
    'Public BoardLetter As String = DI.BoardLetter
    Public BoardDesc As String = DI.BoardDescription
    Public TimeBetweenRequestes As Integer = DI.FloodInterval  ' in seconds
    Public footerText As String = DI.FooterText
    Public MaximumFileSize As Long = DI.MaximumFileSize
    Public AutoDeleteFiles As Boolean = DI.AutoDeleteFiles
    Public ThreadPerPage As Integer = DI.ThreadPerPage
    Public MaximumPages As Integer = DI.MaximumPages
    Public AllowDuplicatesFiles As Boolean = DI.AllowDuplicatesFiles
    Public SmartLinkDuplicateImages As Boolean = DI.SmartLinkDuplicateImages
    Public TrailPosts As Integer = DI.TrailPostsCount
    Public EnableUserID As Boolean = DI.EnableUserID
    Public BumpLimit As Integer = DI.BumpLimit
    Public ResizeMethode As Integer = DI.ResizeMethod
    Public EnableArchive As Boolean = DI.EnableArchive
    Public DefaultModPowers As String = DI.DefaultModPowers  ' In that order: Ban, Delete, Toggle sticky, Toggle Locked, Edit post.
    Public EnableSmilies As Boolean = DI.EnableSmilies
    Public CaptchaLevel As Integer = DI.CaptchaLevel
    Public EnableCaptcha As Boolean = DI.EnableCaptcha
    Public RemoveEXIFData As Boolean = DI.RemoveEXIFData
    Public StaticHTML As Boolean = DI.StaticMode
    Public EnableImpresonationProtection As Boolean = DI.EnableImpresonationProtection
    Public ConvertArchivedThreadToHTML As Boolean = DI.ConvertArchivedThreadToHTML
    Public CheckEXIFOrientation As Boolean = DI.CheckExifOrientation
    Public ShowThreadRepliesCount As Boolean = DI.ShowThreadRepliesCount
    Public EnchancedThumbGeneration As Boolean = DI.EnchancedThumbGeneration
    Public ShowPostSuccessfulMessage As Boolean = DI.ShowPostSuccessfulMessage

#End Region

#Region "Server configuration"

    'Physical path values
    Public StorageFolder As String = DI.PhysicalStorageFolderPath
    Public StorageFolderThumbs As String = CombinePath(DI.PhysicalStorageFolderPath, "thumbs")
    Public ThreadStorageFolder As String = CombinePath(ApplicationDllRoot, "thread")
    Public ArchivedTStorageFolder As String = CombinePath(ApplicationDllRoot, "archive")

    'Misc files path
    Public banFile As String = CombinePath(dataFileDir, "bans")
    Public requestCountF As String = CombinePath(dataFileDir, "req.txt")


    'Web paths Values
    'WebRoot always end with /, and also web paths
    Public ArchivedTHTMLWebPath As String = WebRoot & "archive/"
    Public ThreadHTMLWebPath As String = WebRoot & "thread/"
    Public StoragefolderWEB As String = DI.WebStorageFolderPath



#End Region

#Region "Public Properties"
    Public ReadOnly Property dbType() As String
        Get
            Return DI.DatabaseType
        End Get
    End Property

    Friend ReadOnly Property dataFileDir() As String
        Get
            Return CombinePath(CombinePath(ApplicationDllRoot, "bin"), "datafiles")
        End Get
    End Property

    Friend ReadOnly Property chanbDir() As String
        Get
            Return CombinePath(ApplicationDllRoot, "bin")
        End Get
    End Property

    Public ReadOnly Property ApplicationDllRoot() As String
        Get
            Return chanb.My.Request.PhysicalApplicationPath
        End Get
    End Property

    Public ReadOnly Property isInstalled() As Boolean
        Get
            Return DI.isInstalled
        End Get
    End Property

    ''' <summary>
    ''' Get application web root.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>With a trailing slash /</remarks>
    Public ReadOnly Property WebRoot() As String
        Get
            Dim prefix As String = ""
            If chanb.My.Request.ServerVariables("HTTPS") = "ON" Then
                prefix = "https://"
            Else
                prefix = "http://"
            End If
            If chanb.My.Request.ApplicationPath = "/" Then
                Return prefix & chanb.My.Request.ServerVariables("HTTP_HOST") & chanb.My.Request.ApplicationPath
            Else
                Return prefix & chanb.My.Request.ServerVariables("HTTP_HOST") & chanb.My.Request.ApplicationPath & "/"
            End If
        End Get
    End Property

    Public ReadOnly Property HttpProtocolString() As String
        Get
            If chanb.My.Request.ServerVariables("HTTPS") = "ON" Then
                Return "https"
            Else
                Return "http"
            End If
        End Get
    End Property
#End Region

#Region "HTML Templates"

    'Note: New line removal should be a responsibilty of the site admin.
    Public ReadOnly ReplyPostTemplate As String = DI.ReplyPostTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly ImageTemplate As String = DI.ImageTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly OPPostTemplate As String = DI.OPPostTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly ThreadTemplate As String = DI.ThreadTemplate '.Replace(vbNewLine, String.Empty)

    Public ReadOnly FilesRotatorTemplate As String = DI.FilesRotatorTemplate _
    .Replace("%LANG rotatorImagesStr%", rotatorImagesStr) _
    .Replace("%LANG rotatorfirstStr%", rotatorfirstStr) _
    .Replace("%LANG rotatorlastStr%", rotatorlastStr) _
    .Replace("%LANG rotatorprevStr%", rotatorPrevStr) _
    .Replace("%LANG rotatornextStr%", rotatorNextStr) _
    .Replace("%ROOT%", WebRoot)


    Public ReadOnly FullPageTemplate As String = DI.FullPageTemplate '.Replace(vbNewLine, String.Empty)
    ' Public ReadOnly CatalogItemTemplate As String = DI.CatalogItemTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly VideoItemTemplate As String = DI.VideoItemTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly AudioItemTemplate As String = DI.AudioItemTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly ReportPageTemplate As String = DI.ReportPageTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly DeletePostPageTemplate As String = DI.DeletePostPageTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly modSBRPageTemplate As String = DI.ModSBRTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly BanPageTemplate As String = DI.BanPageTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly editPostPageTemplate As String = DI.editPostPageTemplate '.Replace(vbNewLine, String.Empty)
    Public ReadOnly DeletePostFileTemplate As String = DI.DeletePostFileTemplate '.Replace(vbNewLine, String.Empty)

    Public ReadOnly UserIDHtmlSPAN As String = "<span class='posteruid id_%UID%'>(ID: <span class='hand' onclick=""higlightID('%UID%')"" title='%PO%.'>%UID%</span>)</span>".Replace("%PO%", higlightPostByThisIDStr)

    Public ReadOnly replyButtonHTML As String = "&nbsp;<span>[<a href='%POST LINK%' target='_blank' class='replylink'>%RE%</a>]</span>".Replace("%RE%", replyStr)

    Public ReadOnly noscriptItemHTML As String = "<a href='%IMAGE DL%' target='_blank'>%FILE NAME%<br/><img src='%THUMB_LINK%' style='width: 100px; height: 100px' /></a><br/>"

    Public ReadOnly noscriptVideoHTML As String = "<a href='%VIDEO LINK%' target='_blank'>%FILE NAME%</a>"
    Public ReadOnly noscriptAudioHTML As String = "<a href='%LINK%' target='_blank'>%FILE NAME%</a>"


    Private _modLi As String = "<li><a class=""wdlink"" href=""%WEBROOT%modaction.aspx?action=%ACTION%&id=%ID%"" target=""_blank"">$</a></li>".Replace("%WEBROOT%", WebRoot)

    Public modMenuItems As String() = {("<li><a class=""wdlink"" href=""%WEBROOT%modSelectBanReason.aspx?id=%ID%&sib=false"" target=""_blank"">$</a></li>".Replace("$", banuserStr) & _
                                       "<li><a class=""wdlink"" href=""%WEBROOT%modSelectBanReason.aspx?id=%ID%&sib=true"" target=""_blank"">$</a></li>".Replace("$", banuserStr & " (" & modSilentBanStr & ")")).Replace("%WEBROOT%", WebRoot), _
                                       _modLi.Replace("%ACTION%", "delpost").Replace("$", deletePostStr), _
                                       _modLi.Replace("%ACTION%", "tgsticky").Replace("$", tgstickStr), _
                                       _modLi.Replace("%ACTION%", "tglock").Replace("$", tglockStr), _
                                       _modLi.Replace("%ACTION%", "editpost").Replace("$", EditpostStr)}

    Private _adminLi As String = "<li><a class=""wdlink"" href=""%WEBROOT%adminaction.aspx?action=%ACTION%&id=%ID%"" target=""_blank"">$</a></li>".Replace("%WEBROOT%", WebRoot)

    Public adminMenuItems As String() = {("<li><a class=""wdlink"" href=""%WEBROOT%modSelectBanReason.aspx?id=%ID%&sib=false"" target=""_blank"">$</a></li>".Replace("$", banuserStr) & _
                                       "<li><a class=""wdlink"" href=""%WEBROOT%modSelectBanReason.aspx?id=%ID%&sib=true"" target=""_blank"">$</a></li>".Replace("$", banuserStr & " (" & modSilentBanStr & ")")).Replace("%WEBROOT%", WebRoot), _
                                       _adminLi.Replace("%ACTION%", "permadelpost").Replace("$", "Perma delete"), _
                                       _adminLi.Replace("%ACTION%", "tgsticky").Replace("$", tgstickStr), _
                                       _adminLi.Replace("%ACTION%", "tglock").Replace("$", tglockStr), _
                                       _adminLi.Replace("%ACTION%", "editpost").Replace("$", EditpostStr)}


    Public ReadOnly bannedMessageHTML As String = "<br/><strong style=""color: red;"">%MES%</strong>".Replace("%MES%", banMsgStr)

    Public ReadOnly postingModeReplyHtml As String = "<div class=""postingMode desktop""><span>" & postingModstr & "</span></div>"
    Public ReadOnly archiveNotice As String = "<div class=""postingMode""><span>" & archiveNoticeStr & "</span></div>"

    '  Public ReadOnly captchaTableEntryHtml As String = "<tr><th>" & verificationStr & "</th><td><img alt='" & CaptchaChallengeStr & "' id='captchaImage' src=""%ROOT%captcha.aspx"" /><a onclick='refreshcaptcha();'><img alt=""refresh"" src=""%ROOT%res/refresh.png"" /></a><br /><input id='usercaptcha' autocomplete='off' class='form-text' type='text' size ='30' name='usercaptcha' /></td></tr>".Replace("%ROOT%", WebRoot)
    ' Public ReadOnly captchaTableEntryHtml_qr As String = "<tr><span>" & verificationStr & "</span> <img alt='" & CaptchaChallengeStr & "' id='captchaImage_qr' src=""%ROOT%captcha.aspx"" /><a onclick='refreshcaptcha(null, 'captchaImage_qr');'><img alt=""refresh"" src=""%ROOT%res/refresh.png"" /></a><br /><input id='usercaptcha' autocomplete='off' class='form-text' type='text' size ='30' name='usercaptcha' /></tr>".Replace("%ROOT%", WebRoot)

    Public ReadOnly addNewFileButtonHTML As String = "<input id=""fr_finp"" type=""checkbox"" name=""finp"" value=""yes"" /><label for=""fr_finp"">" & addEachFileInNewPostStr & "</label><br/><input type=""checkbox"" name=""countf"" id=""fr_cfiles"" value=""yes"" /><label for=""fr_cfiles"">" & Language.countFilesStr & "</label><br/><input type='button' onclick=""addUf('files')"" class='button' value='" & addAnotherFStr & "'/>"

    Public postingRulesHTML As String = "<li><span>Blank posts are not allowed.</span></li><li>Please <a href=""%ROOT%faq.aspx"" target=""_blank"">visit</a> the FAQ page for info about tags supported.</li>".Replace("%ROOT%", WebRoot)
    Public ReadOnly threadCountHTMLli As String = "<li><span>Currently there is % thread(s).</span></li>"

    Public ReadOnly DesktopReturnButtonHTML As String = "<a class=""buttonBlue"" href=""%P%"">" & returnStr & "</a>"
    Public ReadOnly MobileReturnButtonHTML As String = "<span class=""mobileib buttonm""><a href=""%P%"">" & returnStr & "</a></span>"

    Public searchEngineLinkList As String() = {"<a href=""http://www.google.com/searchbyimage?image_url=%THUMB_LINK%"" target=""_blank"">google</a>"}

    Public ReadOnly reportReasons As String() = {"rulev$Rule violation", "illm$Illegal material", "spam$SPAM/Abuse", "nsfw$Nudity on SFW board"}

    Public modPostName As String = "<span style=""color: blue"">Moderator</span>"
    Public adminPostName As String = "<span style=""color: red"">Administrator</span>"

    ' Public ReadOnly forbiddenPage As String = "<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN""> <html> <head> <title>403 Forbidden</title> </head> <body bottommargin=""0"" leftmargin=""0"" marginheight=""0"" marginwidth=""0"" rightmargin=""0"" topmargin=""0"" bgcolor=""#000000""> <table height=""75%"" width=""100%"" align=""center"" cellspacing=""0"" cellpadding=""0"" border=""0""> <tr> <td bgcolor=""#cccccc"" height=""67%"" width=""15%"">&nbsp;</td> <td bgcolor=""#ffff00"" height=""67%"" width=""14%"">&nbsp;</td> <td bgcolor=""#00ffff"" height=""67%"" width=""14%"">&nbsp;</td> <td bgcolor=""#00ff00"" height=""67%"" width=""14%"">&nbsp;</td> <td bgcolor=""#ff00ff"" height=""67%"" width=""14%"">&nbsp;</td> <td bgcolor=""#ff0000"" height=""67%"" width=""14%"">&nbsp;</td> <td bgcolor=""#0000ff"" height=""67%"" width=""15%"">&nbsp;</td> </tr> <tr> <td bgcolor=""#0000ff"" height=""8%"" width=""15%"">&nbsp;</td> <td bgcolor=""#131313"" height=""8%"" width=""14%"">&nbsp;</td> <td bgcolor=""#ff00ff"" height=""8%"" width=""14%"">&nbsp;</td> <td bgcolor=""#131313"" height=""8%"" width=""14%"">&nbsp;</td> <td bgcolor=""#00ffff"" height=""8%"" width=""14%"">&nbsp;</td> <td bgcolor=""#131313"" height=""8%"" width=""14%"">&nbsp;</td> <td bgcolor=""#cccccc"" height=""8%"" width=""15%"">&nbsp;</td> </tr> </table> <table height=""25%"" width=""100%"" align=""center"" cellspacing=""0"" cellpadding=""5"" border=""0""> <tr> <td bgcolor=""#083e59"" height=""25%"" width=""18%"">&nbsp;</td> <td bgcolor=""#ffffff"" height=""25%"" width=""18%"">&nbsp;</td> <td bgcolor=""#3a007e"" height=""25%"" width=""18%"">&nbsp;</td> <td bgcolor=""#131313"" height=""25%"" width=""20%"" align=""center"" valign=""middle"" style=""line-height:1.25""><font face=""verdana, san-serif"" color=""#00ff00""><font size=""+3"">403</font><br>Forbidden<br><font size=""-1"">STOP RIGHT THERE</font></font></td><td bgcolor=""#262626"" height=""25%"" width=""26%"" align=""right"" valign=""bottom""></td></tr></table></body></html>"

    Public ReadOnly replyCountSpan As String = "<span>&nbsp;(<b>%REPLY COUNT%</b><span> " & replyStr & ".)</span></span>"
    Public ReadOnly deleteFilesHTMLMenuItem As String = "<li><a class=""wdlink"" href=""%ROOT%deletefile.aspx?mode=report&id=%ID%"" target=""_blank"">Delete files</a></li>".Replace("%ROOT%", WebRoot)

    'A ban reason should be in this format
    ' internalname$autoperm$length$canview$Localised ban reason.
    ' length: in days
    ' autoperm: yes / no
    ' canview: yes / no
    Public modBanReasons As String() = {formatbanreason("trolling", "no", "6", "no", "Trolling/Abuse"), _
                                        formatbanreason("spam", "no", "15", "yes", "Spamming"), _
                                        formatbanreason("nsfw", "no", "5", "yes", "NSFW on SFW board"), _
                                        formatbanreason("cp", "yes", "8", "no", "Child pornography"), _
                                        formatbanreason("security", "yes", "8", "no", "Impersonating the moderator/administrator"), _
                                        formatbanreason("postquality", "no", "3", "yes", "Shitposting")}

    Public dummyWImage As WPostImage = generateDummyImage()
    Private Function generateDummyImage() As WPostImage
        Dim wpi As New WPostImage
        wpi.RealName = "404"
        wpi.Size = 1
        wpi.MD5 = "null"
        wpi.Extension = "JPG"
        wpi.Dimensions = "250x250"
        wpi.ChanbName = "notfound.jpg"
        wpi._isDummy = True
        Return wpi
    End Function

    Private Function formatbanreason(ByVal internalname As String, ByVal perm As String, ByVal length As String, ByVal canview As String, ByVal lang As String) As String
        Return internalname & "$" & perm & "$" & length & "$" & canview & "$" & lang
    End Function

    Private Sub InitVariables()
        'Search engine links
        If FileSystem.FileExists(CombinePath(dataFileDir, "SELinks.txt")) Then
            Dim il As New List(Of String)
            For Each line As String In IO.File.ReadAllLines(CombinePath(dataFileDir, "SELinks.txt"))
                If Not (line.StartsWith("#") Or line.Trim() = String.Empty) Then il.Add(line)
            Next
            searchEngineLinkList = il.ToArray()
            il.Clear()
        End If
        'Ban file override
        If FileSystem.FileExists(CombinePath(dataFileDir, "BanReasons.txt")) Then
            Dim il As New List(Of String)
            For Each line As String In IO.File.ReadAllLines(CombinePath(dataFileDir, "BanReasons.txt"))
                If Not (line.StartsWith("#") Or line.Trim() = String.Empty) Then il.Add(line)
            Next
            modBanReasons = il.ToArray()
            il.Clear()
        End If
        'Posting rules
        If FileSystem.FileExists(CombinePath(dataFileDir, "postRules.txt")) Then
            Dim il As New StringBuilder
            For Each line As String In IO.File.ReadAllLines(CombinePath(dataFileDir, "postRules.txt"))
                If Not (line.StartsWith("#") Or line.Trim() = String.Empty) Then il.Append("<li>" & line.Replace("%ROOT%", WebRoot) & "</li>")
            Next
            postingRulesHTML = il.ToString()
        End If
    End Sub

    Private Sub CheckDir(ByVal path As String)
        If Not FileSystem.DirectoryExists(path) Then FileSystem.CreateDirectory(path)
    End Sub

    Friend Function CombinePath(ByVal a As String, ByVal b As String) As String
        Return IO.Path.Combine(a, b)
    End Function

    Friend Function CombinePath3(ByVal a As String, ByVal b As String, ByVal c As String) As String
        Return IO.Path.Combine(IO.Path.Combine(a, b), c)
    End Function

#End Region

End Module

