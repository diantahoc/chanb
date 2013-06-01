Public Module GlobalVariables

    Dim DI As New DataInitializer

    Sub New()
        StorageFolder = DI.PhysicalStorageFolderPath
        StorageFolderThumbs = DI.PhysicalStorageFolderPath & "\thumbs"

        If FileIO.FileSystem.DirectoryExists(StorageFolder) = False Then FileIO.FileSystem.CreateDirectory(StorageFolder)
        If FileIO.FileSystem.DirectoryExists(StorageFolderThumbs) = False Then FileIO.FileSystem.CreateDirectory(StorageFolderThumbs)

        If FileIO.FileSystem.DirectoryExists(ThreadStorageFolder) = False Then FileIO.FileSystem.CreateDirectory(ThreadStorageFolder)
        If FileIO.FileSystem.DirectoryExists(ArchivedTStorageFolder) = False Then FileIO.FileSystem.CreateDirectory(ArchivedTStorageFolder)

    End Sub

#Region "Board configuration"

    Public BoardTitle As String = DI.BoardTitle
    Public BoardLetter As String = DI.BoardLetter
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
    Public transmitRealFileName As Boolean = DI.TransmitRealFileName
    Public EnableSmilies As Boolean = DI.EnableSmilies
    Public CaptchaLevel As Integer = DI.CaptchaLevel
    Public EnableCaptcha As Boolean = DI.EnableCaptcha
    Public RemoveEXIFData As Boolean = DI.RemoveEXIFData

    Public StaticHTML As Boolean = DI.StaticMode

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

#End Region

#Region "Server configuration"

    Public StorageFolder As String = DI.PhysicalStorageFolderPath
    Public StorageFolderThumbs As String = DI.PhysicalStorageFolderPath & "\thumbs"
    Public StoragefolderWEB As String = DI.WebStorageFolderPath
    Public isInstalled As Boolean = DI.isInstalled

    Public ThreadStorageFolder As String = chanb.My.Request.PhysicalApplicationPath & "\thread"
    Public ThreadHTMLWebPath As String = WebRoot & "thread/"

    Public ArchivedTStorageFolder As String = chanb.My.Request.PhysicalApplicationPath & "\archive"
    Public ArchivedTHTMLWebPath As String = WebRoot & "archive/"


#End Region

#Region "HTML Templates"


    Public ReadOnly ReplyPostTemplate As String = DI.ReplyPostTemplate
    Public ReadOnly ImageTemplate As String = DI.ImageTemplate
    Public ReadOnly OPPostTemplate As String = DI.OPPostTemplate
    Public ReadOnly ThreadTemplate As String = DI.ThreadTemplate
    Public ReadOnly ImageRotatorTemplate As String = DI.ImageRotatorTemplate
    Public ReadOnly FullPageTemplate As String = DI.FullPageTemplate
    Public ReadOnly CatalogItemTemplate As String = DI.CatalogItemTemplate

    Public ReadOnly UserIDHtmlSPAN As String = "<span class='posteruid id_%UID%'>(ID: <span class='hand' title='%PO%.'>%UID%</span>)</span>".Replace("%PO%", posterIdstr)
    Public ReadOnly replyButtonHTML As String = "&nbsp;<span>[<a href='%POST LINK%' target='_blank' class='replylink'>%RE%</a>]</span>".Replace("%RE%", replyStr)
    Public ReadOnly noscriptItemHTML As String = "<a href='%IMAGE DL%' target='_blank'>%FILE NAME%<br/><img src='%THUMB_LINK%' style='width: 100px; height: 100px' /></a><br/>"
    Public ReadOnly modMenu As String() = {"<option value='banpost'>LANG</option>".Replace("LANG", banStr), "<option value='delpost'>LANG</option>".Replace("LANG", deletepostStr), "<option value='tgsticky'>LANG</option>".Replace("LANG", tgstickstr), "<option value='tglock'>LANG</option>".Replace("LANG", tglockstr), "<option value='editpost'>LANG</option>".Replace("LANG", editpostStr)}
    Public ReadOnly modMenuNoscript As String() = {"<a href='modaction.aspx?action=banpost&id=%ID%'>LANG</a>".Replace("LANG", banStr), "<a href='modaction.aspx?action=delpost&id=%ID%'>LANG</a>".Replace("LANG", deletepostStr), "<a href='modaction.aspx?action=tgsticky&id=%ID%'>LANG</a>".Replace("LANG", tgstickstr), "<a href='modaction.aspx?action=tglock&id=%ID%'>LANG</a>".Replace("LANG", tglockstr), "<a href='modaction.aspx?action=editpost&id=%ID%'>LANG</a>".Replace("LANG", editpostStr)}
    Public ReadOnly bannedMessageHTML As String = "<br><strong style=''color: red;''>%MES%</strong>".Replace("%MES%", banMsgStr)
    Public ReadOnly GenericMessageTemplate As String = "<html xmlns='http://www.w3.org/1999/xhtml' xml:lang='en' lang='en'><head><title>%MSG TITLE%</title><link rel='Stylesheet' href='yotsubab.css' /><meta HTTP-EQUIV='REFRESH' content='%REDIRECT DELAY%; url=%REDIRECT URL%'></head><body><div align='center'><span style='color: #%C%; font-size: xx-large'>%MESSAGE TEXT%</span></div></body></html>"

    Public ReadOnly postingModeReplyHtml As String = "<div class='postingMode desktop'><span>" & postingModstr & "</span></div>"
    Public ReadOnly archiveNotice As String = "<div class='postingMode'><span>" & archiveNoticeStr & "</span></div>"

    Public ReadOnly captchaTableEntryHtml As String = "<tr><th>" & EnterCaptchastr & "</th><td><img alt='" & captchaChallengeStr & "' id='captchaImage' src='" & WebRoot & "captcha.aspx' /><a onclick='refreshcaptcha();'><img alt='refresh' style='cursor:pointer; min-height:30px; min-width:30px;' src='" & WebRoot & "res/refresh.png' id='refreshcaptchabutton' onmousemove='focusRCB();' onmouseout='unfocusRCB();' /></a><br /><input id='usercaptcha' autocomplete='off' class='form-text' type='text' size ='30' name='usercaptcha' /></td></tr>"
    Public ReadOnly addNewFileButtonHTML As String = "<input type='checkbox' name='finp' value='yes'>" & eachfileInNewpost & "</input><br/><input type='checkbox' name='countf' value='yes'>" & countFiles & "</input><br/><a class='form-button' onclick='createUf();' >" & addAnotherF & "</a>"

    Public ReadOnly postingRulesHTML As String = "<li><span>Blank posts are not allowed.</span></li><li><span>Spoilers are suported under the [spoiler][/spoiler] tags.</span></li><li><span>You may highlight your code by using the [code][/code] tags. The [lang][/lang] tags are required in order to properly highlight your code. See a <a href='faq.aspx#codetags'>list</a> of supported languages.</span></li>"
    Public ReadOnly threadCountHTMLli As String = "<li><span>Currently there is % thread(s).</span></li>"

    Public ReadOnly DesktopReturnButtonHTML As String = "[<a href='%P%'>" & returnStr & "</a>]"
    Public ReadOnly MobileReturnButtonHTML As String = "<span class='mobileib button'><a href='%P%'>" & returnStr & "</a></span>"

    Public ReadOnly searchEngineLinkList As String() = {"<a href='http://iqdb.org/?url=%THUMB_LINK%' target='_blank'>iqdb</a>", "<a href='http://www.google.com/searchbyimage?image_url=%THUMB_LINK%' target='_blank'>google</a>", "<a href='http://saucenao.com/search.php?db=999&url=%THUMB_LINK%' target='_blank'>saucenao</a>"}



#End Region

End Module

