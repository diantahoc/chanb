Friend Module Language

    Dim langFile As String = dataFileDir & "boardLanguage.conf"

    Public Property BoardLanguage() As String
        Get
            If FileIO.FileSystem.FileExists(langFile) Then
                Return IO.File.ReadAllText(langFile)
            Else
                IO.File.WriteAllText(langFile, "en-US")
                Return "en-US"
            End If
        End Get
        Set(ByVal value As String)
            IO.File.WriteAllText(langFile, value)
        End Set
    End Property


    Private langDic As New LangEngine(BoardLanguage)

    Public ReadOnly nameStr As String = langDic.Retrive("nameStr")
    Public ReadOnly subjectStr As String = langDic.Retrive("subjectStr")
    Public ReadOnly emailStr As String = langDic.Retrive("emailStr")
    Public ReadOnly commentStr As String = langDic.Retrive("commentStr")
    Public ReadOnly passwordStr As String = langDic.Retrive("passwordStr")
    Public ReadOnly verificationStr As String = langDic.Retrive("verificationStr")
    Public ReadOnly newThreadStr As String = langDic.Retrive("newThreadStr")
    Public ReadOnly filesStr As String = langDic.Retrive("filesStr")
    Public ReadOnly replyStr As String = langDic.Retrive("replyStr")
    Public ReadOnly deleteStr As String = langDic.Retrive("deleteStr")
    Public ReadOnly reportStr As String = langDic.Retrive("reportStr")
    Public ReadOnly fileOnlyStr As String = langDic.Retrive("fileOnlyStr")
    Public ReadOnly addEachFileInNewPostStr As String = langDic.Retrive("addEachFileInNewPostStr")
    Public ReadOnly AnonNameStr As String = langDic.Retrive("AnonNameStr")
    Public ReadOnly stickyStr As String = langDic.Retrive("stickyStr")
    Public ReadOnly lockedStr As String = langDic.Retrive("lockedStr")
    Public ReadOnly prevStr As String = langDic.Retrive("prevStr")
    Public ReadOnly nextStr As String = langDic.Retrive("nextStr")
    Public ReadOnly forPostDelStr As String = langDic.Retrive("forPostDelStr")
    Public ReadOnly postingModstr As String = langDic.Retrive("postingModstr")
    Public ReadOnly returnStr As String = langDic.Retrive("returnStr")
    Public ReadOnly addAnotherFStr As String = langDic.Retrive("addAnotherFStr")
    Public ReadOnly countFilesStr As String = langDic.Retrive("countFilesStr")

    Public ReadOnly summaryPandIStr As String = langDic.Retrive("summaryPandIStr")
    Public ReadOnly summaryPonlyStr As String = langDic.Retrive("summaryPonlyStr")
    Public ReadOnly summaryIonlyStr As String = langDic.Retrive("summaryIonlyStr")

    Public ReadOnly summaryClickToViewStr As String = langDic.Retrive("summaryClickToViewStr")

    Public ReadOnly banuserStr As String = langDic.Retrive("banuserStr")
    Public ReadOnly modSilentBanStr As String = langDic.Retrive("modSilentBanStr")
    Public ReadOnly deletePostStr As String = langDic.Retrive("deletePostStr")
    Public ReadOnly tgstickStr As String = langDic.Retrive("tgstickStr")
    Public ReadOnly tglockStr As String = langDic.Retrive("tglockStr")
    Public ReadOnly EditpostStr As String = langDic.Retrive("EditpostStr")
    Public ReadOnly higlightPostByThisIDStr As String = langDic.Retrive("higlightPostByThisIDStr")
    Public ReadOnly TopStr As String = langDic.Retrive("TopStr")
    Public ReadOnly CatalogStr As String = langDic.Retrive("CatalogStr")
    Public ReadOnly RefreshStr As String = langDic.Retrive("RefreshStr")
    Public ReadOnly BottomStr As String = langDic.Retrive("BottomStr")
    Public ReadOnly CaptchaChallengeStr As String = langDic.Retrive("CaptchaChallengeStr")
    Public ReadOnly ArchiveStr As String = langDic.Retrive("ArchiveStr")
    Public ReadOnly ForbiddenStr As String = langDic.Retrive("ForbiddenStr")
    Public ReadOnly sucessStr As String = langDic.Retrive("sucessStr")
    Public ReadOnly reportReasonStr As String = langDic.Retrive("reportReasonStr")
    Public ReadOnly invalidIdStr As String = langDic.Retrive("invalidIdStr")
    Public ReadOnly thread404Str As String = langDic.Retrive("thread404Str")

    Public ReadOnly errorStr As String = langDic.Retrive("errorStr")

    Public ReadOnly ImageRequired As String = langDic.Retrive("ImageRequired")
    Public ReadOnly SuccessfulPostString As String = langDic.Retrive("SuccessfulPostString")
    Public ReadOnly FileToBig As String = langDic.Retrive("FileToBig")
    Public ReadOnly FloodDetected As String = langDic.Retrive("FloodDetected")
    Public ReadOnly CannotDeletePostBadPassword As String = langDic.Retrive("CannotDeletePostBadPassword")
    Public ReadOnly PostDeletedSuccess As String = langDic.Retrive("PostDeletedSuccess")
    Public ReadOnly NoPostWasSelected As String = langDic.Retrive("NoPostWasSelected")
    Public ReadOnly ReportedSucess As String = langDic.Retrive("ReportedSucess")
    Public ReadOnly BadOrNoImage As String = langDic.Retrive("BadOrNoImage")
    Public ReadOnly BannedMessage As String = langDic.Retrive("BannedMessage")
    Public ReadOnly lockedMessage As String = langDic.Retrive("lockedMessage")
    Public ReadOnly noBlankpost As String = langDic.Retrive("noBlankpost")
    Public ReadOnly duplicateFile As String = langDic.Retrive("duplicateFile")
    Public ReadOnly UnsupportedFile As String = langDic.Retrive("UnsupportedFile")
    Public ReadOnly arhivedMessage As String = langDic.Retrive("arhivedMessage")
    Public ReadOnly invalidPostmodestr As String = langDic.Retrive("invalidPostmodestr")
    Public ReadOnly wrongCaptcha As String = langDic.Retrive("wrongCaptcha")
    Public ReadOnly archiveNoticeStr As String = langDic.Retrive("archiveNoticeStr").Replace("%WR%", WebRoot)
    Public ReadOnly noVideoSupportStr As String = langDic.Retrive("noVideoSupportStr")

    Public ReadOnly banMsgStr As String = langDic.Retrive("banMsgStr")
    Public ReadOnly commentToolong As String = langDic.Retrive("commentToolong")


    Public ReadOnly nameAlreadyUsed As String = langDic.Retrive("nameAlreadyUsed")

    Public ReadOnly modRequired As String = langDic.Retrive("modRequired")
    Public ReadOnly modNoPower As String = langDic.Retrive("modNoPower")
    Public ReadOnly modBannedPosterStr As String = langDic.Retrive("modBannedPosterStr")
    Public ReadOnly modRequetComplete As String = langDic.Retrive("modRequetComplete")
    Public ReadOnly modLoginSucess As String = langDic.Retrive("modLoginSucess")
    Public ReadOnly modLoginFailed As String = langDic.Retrive("modLoginFailed")
    Public ReadOnly modSelectBanReason As String = langDic.Retrive("modSelectBanReason")

    Public ReadOnly dbTypeNotSet As String = langDic.Retrive("dbTypeNotSet")
    Public ReadOnly dbTypeInvalid As String = langDic.Retrive("dbTypeInvalid")

    Public ReadOnly installerDbTypeNotSpecified As String = langDic.Retrive("installerDbTypeNotSpecified")
    Public ReadOnly installerDbConnectionStringNotSpecified As String = langDic.Retrive("installerDbConnectionStringNotSpecified")
    Public ReadOnly errorOccuredStr As String = langDic.Retrive("errorOccuredStr")
    Public ReadOnly isntallerConnectionEstablishedSucess As String = langDic.Retrive("isntallerConnectionEstablishedSucess")


    Public ReadOnly rotatorImagesStr As String = langDic.Retrive("rotatorImagesStr")
    Public ReadOnly rotatorfirstStr As String = langDic.Retrive("rotatorfirstStr")
    Public ReadOnly rotatorlastStr As String = langDic.Retrive("rotatorlastStr")
    Public ReadOnly rotatorPrevStr As String = langDic.Retrive("rotatorPrevStr")
    Public ReadOnly rotatorNextStr As String = langDic.Retrive("rotatorNextStr")


    Public ReadOnly banReasonStr As String = langDic.Retrive("banReasonStr")
    Public ReadOnly bannedOnStr As String = langDic.Retrive("bannedOnStr")
    Public ReadOnly banIpStr As String = langDic.Retrive("banIpStr")
    Public ReadOnly banPostNoStr As String = langDic.Retrive("banPostNoStr")
    Public ReadOnly banExpiryStr As String = langDic.Retrive("banExpiryStr")

End Module
