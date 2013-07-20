Public Module Language

    Public ReadOnly nameStr As String = "Name"
    Public ReadOnly subjectStr As String = "Subject"
    Public ReadOnly emailStr As String = "Email"
    Public ReadOnly commentStr As String = "Comment"
    Public ReadOnly passwordStr As String = "Password"
    Public ReadOnly verificationStr As String = "Verification"
    Public ReadOnly newThreadStr As String = "New thread"
    Public ReadOnly filesStr As String = "Files(s)"
    Public ReadOnly replyStr As String = "Reply"
    Public ReadOnly deleteStr As String = "Delete"
    Public ReadOnly reportStr As String = "Report"
    Public ReadOnly fileOnlyStr As String = "File only"
    Public ReadOnly addEachFileInNewPostStr As String = "Add each file to a seperate post"
    Public ReadOnly AnonNameStr As String = "Anonymous"
    Public ReadOnly stickyStr As String = "Sticky"
    Public ReadOnly lockedStr As String = "Locked"
    Public ReadOnly prevStr As String = "Previous"
    Public ReadOnly nextStr As String = "Next"
    Public ReadOnly forPostDelStr As String = "For post deletion."
    Public ReadOnly postingModstr As String = "Posting mode: Reply"
    Public ReadOnly returnStr As String = "Return"
    Public ReadOnly addAnotherFStr As String = "Add another file"
    Public ReadOnly countFilesStr As String = "Count files"

    Public summaryPandIStr As String = "%P% post(s) and %I% image reply(ies) omitted."
    Public summaryPonlyStr As String = "%P% post(s) omitted."
    Public summaryIonlyStr As String = "%I% image reply(ies) omitted."

    Public summaryClickToViewStr As String = " Click <a href=""%POSTLINK%"">here</a> to view."

    Public ReadOnly banuserStr As String = "Ban user"
    Public modSilentBanStr As String = "Silent ban"
    Public ReadOnly deletePostStr As String = "Delete post"
    Public ReadOnly tgstickStr As String = "Toggle sticky"
    Public ReadOnly tglockStr As String = "Toggle Lock"
    Public ReadOnly EditpostStr As String = "Edit post"
    Public modBlurImageStr As String = "Blur image"
    Public ReadOnly higlightPostByThisIDStr As String = "Higlight posts by this ID"
    Public ReadOnly TopStr As String = "Top"
    Public ReadOnly CatalogStr As String = "Catalog"
    Public ReadOnly RefreshStr As String = "Refresh"
    Public ReadOnly BottomStr As String = "Bottom"
    Public ReadOnly CaptchaChallengeStr As String = "CAPTCHA Challenge"
    Public ReadOnly ArchiveStr As String = "Archive"
    Public ReadOnly ForbiddenStr As String = "Fobidden"
    Public ReadOnly sucessStr As String = "Success"
    Public ReadOnly reportReasonStr As String = "Report reason:"
    Public invalidIdStr As String = "Invalid ID"

    Public errorStr As String = "Error"

    Public ReadOnly ImageRequired As String = "You cannot start a new thread without an image."
    Public ReadOnly SuccessfulPostString As String = "Post Succesful !"
    Public ReadOnly FileToBig As String = "The provided file %NAME% is larger than the allowed limit %L%"
    Public ReadOnly FloodDetected As String = "Flood detected, please allow % seconds before posting, reporting and deleting."
    Public ReadOnly CannotDeletePostBadPassword As String = "Cannot delete post % , bad password"
    Public ReadOnly PostDeletedSuccess As String = "Post number % has been deleted"
    Public ReadOnly NoPostWasSelected As String = "No post was selected"
    Public ReadOnly ReportedSucess As String = "Reported post number %"
    Public ReadOnly BadOrNoImage As String = "No image or image contains errors."
    Public ReadOnly BannedMessage As String = "You are banned."
    Public ReadOnly lockedMessage As String = "Thread is locked."
    Public ReadOnly noBlankpost As String = "Blank post are not allowed."
    Public ReadOnly duplicateFile As String = "Duplicate image detected."
    Public ReadOnly UnsupportedFile As String = "Unsupported file type."
    Public ReadOnly arhivedMessage As String = "This thread has 404'ed and it is in the archive now."
    Public ReadOnly invalidPostmodestr As String = "Invalid posting mode."
    Public ReadOnly wrongCaptcha As String = "You have mistyped the CAPTCHA. Please try again."
    Public ReadOnly archiveNoticeStr As String = "This is the archive. Click <a href='" & WebRoot & "default.aspx'>here</a> to go back to the main page."
    Public ReadOnly noVideoSupportStr As String = "Your browser does not support html5 video"

    Public ReadOnly banMsgStr As String = "User was banned for this post."
    Public ReadOnly commentToolong As String = "<br/><span class='summary'>Comment is too long. Click <a href='%POSTLINK%' target='_blank'>here</a> to view</span>"


    Public ReadOnly nameAlreadyUsed As String = "The name '%' is already taken, you cannot post using it"

    Public ReadOnly modRequired As String = "Moderator privilege is required to access this page."
    Public ReadOnly modNoPower As String = "You have no power to do that."
    Public ReadOnly modBannedPosterStr As String = "Banned the poster of %."
    Public ReadOnly modRequetComplete As String = "Request complete."
    Public ReadOnly modLoginSucess As String = "Login succeeded"
    Public ReadOnly modLoginFailed As String = "Login failed"
    Public modSelectBanReason As String = "Select a reason for this ban"

    '  Public ReadOnly currentThreadsCountStr As String = "Currently there are % thread(s)."
    Public dbTypeNotSet As String = "Database type is not set"
    Public dbTypeInvalid As String = "Invalid database type"

    Public installerDbTypeNotSpecified As String = "Database type not specified"
    Public installerDbConnectionStringNotSpecified As String = "Database connection string is not specified"
    Public errorOccuredStr As String = "An error has been occured, error message is: %"
    Public isntallerConnectionEstablishedSucess As String = "% connection was sucessfully established."


    Public rotatorImagesStr As String = "image(s)" '%LANG rotatorImagesStr%
    Public rotatorfirstStr As String = "First" '%LANG rotatorfirstStr%
    Public rotatorlastStr As String = "Last" '%LANG rotatorlastStr%
    Public rotatorPrevStr As String = "Previous" '%LANG rotatorPrevStr%
    Public rotatorNextStr As String = "Next" '%LANG rotatornextStr%






    Public banReasonStr As String = "Reason"
    Public bannedOnStr As String = "Banned on"
    Public banIpStr As String = "Your IP was"
    Public banPostNoStr As String = "The post you were banned for"
    Public banExpiryStr As String = "Expiration date"





#Region "Arabic"

    'Public ReadOnly nameStr As String = "الاسم"
    'Public ReadOnly subjectStr As String = "الموضوع"
    'Public ReadOnly emailStr As String = "البريد الالكتروني"
    'Public ReadOnly commentStr As String = "التعليق"
    'Public ReadOnly passwordStr As String = "كلمة السر"
    'Public ReadOnly verificationStr As String = "التحقق"
    'Public ReadOnly newThreadStr As String = "موضوع جديد"
    'Public ReadOnly filesStr As String = "الملفات"
    'Public ReadOnly replyStr As String = "رد"
    'Public ReadOnly deleteStr As String = "حذف"
    'Public ReadOnly reportStr As String = "تبليغ"
    'Public ReadOnly fileOnlyStr As String = "الملف فقط"
    'Public ReadOnly addEachFileInNewPostStr As String = "اضف كل ملف الى رد جديد"
    'Public ReadOnly AnonNameStr As String = "مجهول"
    'Public ReadOnly stickyStr As String = "معلقة"
    'Public ReadOnly lockedStr As String = "مغلق"
    'Public ReadOnly prevStr As String = "السابق"
    'Public ReadOnly nextStr As String = "التالي"
    'Public ReadOnly forPostDelStr As String = "من أجل حذف التعليق لاحقا"
    'Public ReadOnly postingModstr As String = "وضع الارسال : رد"
    'Public ReadOnly returnStr As String = "الرجوع"
    'Public ReadOnly addAnotherFStr As String = "اضافة ملف جديد"
    'Public ReadOnly countFilesStr As String = "قم بعدّ الملفات"

    'Public ReadOnly p1str As String = "% رد"
    'Public ReadOnly andStr As String = " و "
    'Public ReadOnly p2str As String = "% فيه صورة"
    'Public ReadOnly omittedStr As String = " مخبأة. اضغط <a href='%POSTLINK%' target='_blank' class='replylink'>هنا</a> للمشاهدة."

    'Public ReadOnly banuserStr As String = "حظر المستخدم"
    'Public ReadOnly deletePostStr As String = "محو التعليق"
    'Public ReadOnly tgstickStr As String = "التبديل بين معلق أو غير معلق"
    'Public ReadOnly tglockStr As String = "التبديل بين مغلق أو غير مغلق"
    'Public ReadOnly EditpostStr As String = "تعديل الرد"
    'Public ReadOnly PosterIdStr As String = "معرف صاحب الرد"
    'Public ReadOnly TopStr As String = "الى فوق"
    'Public ReadOnly CatalogStr As String = "الفهرست"
    'Public ReadOnly RefreshStr As String = "تحديث"
    'Public ReadOnly BottomStr As String = "الى الاسفل"
    'Public ReadOnly CaptchaChallengeStr As String = "تحدّي CAPTCHA"
    'Public ReadOnly ArchiveStr As String = "الأرشيف"
    'Public ReadOnly ForbiddenStr As String = "ممنوع"
    'Public ReadOnly sucessStr As String = "تم بنجاح"

    'Public invalidIdStr As String = "معرف غير صالح"

    'Public errorStr As String = "خطأ"

    'Public ReadOnly ImageRequired As String = "لا يمكنك انشاء موضوع جديد من دون صورة"
    'Public ReadOnly SuccessfulPostString As String = "تم الارسال بنجاح"
    'Public ReadOnly FileToBig As String = "الملف %NAME% أكبر من الحجم المسموح به %L%"
    'Public ReadOnly FloodDetected As String = "Flood detected, please allow % seconds before posting, reporting and deleting."
    'Public ReadOnly CannotDeletePostBadPassword As String = "Cannot delete post % , bad password"
    'Public ReadOnly PostDeletedSuccess As String = "Post number % has been deleted"
    'Public ReadOnly NoPostWasSelected As String = "No post was selected"
    'Public ReadOnly ReportedSucess As String = "Reported post number %"
    'Public ReadOnly BadOrNoImage As String = "No image or image contains errors."
    'Public ReadOnly BannedMessage As String = "You are banned from posting."
    'Public ReadOnly lockedMessage As String = "Thread is locked."
    'Public ReadOnly noBlankpost As String = "Blank post are not allowed."
    'Public ReadOnly duplicateFile As String = "Duplicate image detected."
    'Public ReadOnly UnsupportedFile As String = "Unsupported file type."
    'Public ReadOnly arhivedMessage As String = "This thread has 404'ed and it is in the archive now."
    'Public ReadOnly invalidPostmodestr As String = "Invalid posting mode."
    'Public ReadOnly wrongCaptcha As String = "You have mistyped the CAPTCHA. Please try again."
    'Public ReadOnly archiveNoticeStr As String = "This is the archive. Click <a href='" & WebRoot & "default.aspx'>here</a> to go back to the main page."
    'Public ReadOnly noVideoSupportStr As String = "Your browser does not support html5 video"

    'Public ReadOnly banMsgStr As String = "User was banned for this post."
    'Public ReadOnly commentToolong As String = "<br/><span class='summary'>Comment is too long. Click <a href='%POSTLINK%' target='_blank'>here</a> to view</span>"


    'Public ReadOnly nameAlreadyUsed As String = "The name '%' is already taken, you cannot post using it"

    'Public ReadOnly modRequired As String = "Moderator privilege is required to access this page."
    'Public ReadOnly modNoPower As String = "You have no power to do that."
    'Public ReadOnly modBannedPosterStr As String = "Banned the poster of %."
    'Public ReadOnly modRequetComplete As String = "Request complete."
    'Public ReadOnly modLoginSucess As String = "Login sucessful"
    ''  Public ReadOnly currentThreadsCountStr As String = "Currently there are % thread(s)."
    'Public dbTypeNotSet As String = "Database type is not set"
    'Public dbTypeInvalid As String = "Invalid database type"

    'Public installerDbTypeNotSpecified As String = "Database type not specified"
    'Public installerDbConnectionStringNotSpecified As String = "Database connection string is not specified"
    'Public errorOccuredStr As String = "An error has been occured, error message is: %"
    'Public isntallerConnectionEstablishedSucess As String = "% connection was sucessfully established."
#End Region

End Module
