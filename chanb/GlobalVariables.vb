Public Module GlobalVariables

    Dim DI As New DataInitializer

#Region "Board configuration"

    Public BoardTitle As String = DI.BoardTitle
    Public BoardLetter As String = DI.BoardLetter
    Public BoardDesc As String = DI.BoardDescription
    Public TimeBetweenRequestes As Integer = DI.FloodInterval  ' in seconds
    Public footerText As String = DI.FooterText
    Public MaximumFileSize As Long = DI.MaximumFileSize
    Public DeleteFiles As Boolean = DI.AutoDeleteFiles
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

#End Region

#Region "Server configuration"

    Public StorageFolder As String = DI.PhysicalStorageFolderPath
    Public StoragefolderWEB As String = DI.WebStorageFolderPath
    Public isInstalled As Boolean = DI.isInstalled

#End Region

#Region "HTML Templates"


    Public ReadOnly postTemplate As String = DI.ReplyPostTemplate
    Public ReadOnly imageTemplate As String = DI.ImageTemplate
    Public ReadOnly opPostTemplate As String = DI.OPPostTemplate
    Public ReadOnly threadTemplate As String = DI.ThreadTemplate
    Public ReadOnly rotatorTemplate As String = DI.ImageRotatorTemplate

    Public ReadOnly uidHtml As String = "<span class='posteruid id_%UID%'>(ID: <span class='hand' title='%PO%.'>%UID%</span>)</span>".Replace("%PO%", posterIdstr)
    Public ReadOnly replyButtonHTML As String = "&nbsp;<span>[<a href='%POST LINK%' target='_blank' class='replylink'>%RE%</a>]</span>".Replace("%RE%", replyStr)
    Public ReadOnly noscriptItemHTML As String = "<a href='%IMAGE DL%' target='_blank'>%FILE NAME%<br/><img src='%THUMB_LINK%' style='width: 100px; height: 100px' /></a><br/>"
    Public ReadOnly modMenu As String() = {"<option value='banpost'>LANG</option>".Replace("LANG", banStr), "<option value='delpost'>LANG</option>".Replace("LANG", deletepostStr), "<option value='tgsticky'>LANG</option>".Replace("LANG", tgstickstr), "<option value='tglock'>LANG</option>".Replace("LANG", tglockstr), "<option value='editpost'>LANG</option>".Replace("LANG", editpostStr)}
    Public ReadOnly modMenuNoscript As String() = {"<a href='modaction.aspx?action=banpost&id=%ID%'>LANG</a>".Replace("LANG", banStr), "<a href='modaction.aspx?action=delpost&id=%ID%'>LANG</a>".Replace("LANG", deletepostStr), "<a href='modaction.aspx?action=tgsticky&id=%ID%'>LANG</a>".Replace("LANG", tgstickstr), "<a href='modaction.aspx?action=tglock&id=%ID%'>LANG</a>".Replace("LANG", tglockstr), "<a href='modaction.aspx?action=editpost&id=%ID%'>LANG</a>".Replace("LANG", editpostStr)}
    Public ReadOnly bannedMessageHTML As String = "<br><strong style=''color: red;''>%MES%</strong>".Replace("%MES%", banMsgStr)
    Public ReadOnly GenericMessageTemplate As String = "<html xmlns='http://www.w3.org/1999/xhtml' xml:lang='en' lang='en'><head><title>%MSG TITLE%</title><link rel='Stylesheet' href='yotsubab.css' /><meta HTTP-EQUIV='REFRESH' content='%REDIRECT DELAY%; url=%REDIRECT URL%'></head><body><div align='center'><span style='color: #66CCFF; font-size: xx-large'>%MESSAGE TEXT%</span></div></body></html>"
#End Region

End Module

