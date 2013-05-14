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
    Public MaximumPostLenght As Integer = 0

#End Region

#Region "Server configuration"

    Public STORAGEFOLDER As String = DI.PhysicalStorageFolderPath
    Public StoragefolderWEB As String = DI.WebStorageFolderPath

#End Region

#Region "HTML Templates"


    Public ReadOnly postTemplate As String = DI.ReplyPostTemplate
    Public ReadOnly imageTemplate As String = DI.ImageTemplate
    Public ReadOnly opPostTemplate As String = DI.OPPostTemplate
    Public ReadOnly threadTemplate As String = DI.ThreadTemplate
    Public ReadOnly rotatorTemplate As String = DI.ImageRotatorTemplate

    Public ReadOnly idHtml As String = "<span class='posteruid id_%UID%'>(ID: <span class='hand' title='%PO%.'>%UID%</span>)</span>".Replace("%PO%", posterIdstr)
    Public ReadOnly replyButtonHTML As String = "&nbsp;<span>[<a href='%POST LINK%' target='_blank' class='replylink'>%RE%</a>]</span>".Replace("%RE%", replyStr)
    Public ReadOnly noscriptItemHTML As String = "<a href='%IMAGE SRC%' target='_blank'>%FILE NAME%<br/><img src='%THUMB_LINK%' style='width: 100px; height: 100px' /></a><br/>"
    Public ReadOnly modMenu As String() = {"<a href='modaction.aspx?action=banpost&id=%ID%'>LANG</a>".Replace("LANG", banStr), "<a href='modaction.aspx?action=delpost&id=%ID%'>LANG</a>".Replace("LANG", deletepostStr), "<a href='modaction.aspx?action=tgsticky&id=%ID%'>LANG</a>".Replace("LANG", tgstickstr), "<a href='modaction.aspx?action=tglock&id=%ID%'>LANG</a>".Replace("LANG", tglockstr), "<a href='modaction.aspx?action=editpost&id=%ID%'>LANG</a>".Replace("LANG", editpostStr)}
    Public ReadOnly bannedMessageHTML As String = "<br><strong style=''color: red;''>%MES%</strong>".Replace("%MES%", banMsgStr)

#End Region

End Module

