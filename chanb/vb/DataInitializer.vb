Public Class DataInitializer

    Dim boardDataFileDB As String = ""

    Dim DataDB As ValuesStore

    Sub New()
        boardDataFileDB = CombinePath3(chanb.My.Request.PhysicalApplicationPath, "bin", "data")
        DataDB = New ValuesStore(boardDataFileDB)
    End Sub


#Region "Templates files"

    Public ReadOnly Property OPPostTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "op_post.html"))
        End Get
    End Property

    Public ReadOnly Property ReplyPostTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "reply_post.html"))
        End Get
    End Property

    Public ReadOnly Property ThreadTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "thread.html"))
        End Get
    End Property

    Public ReadOnly Property ImageTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "image_file.html"))
        End Get
    End Property

    Public ReadOnly Property FilesRotatorTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "files_rotator.html"))
        End Get
    End Property

    Public ReadOnly Property FullPageTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "full_page.html"))
        End Get
    End Property

    'Public ReadOnly Property CatalogItemTemplate() As String
    '    Get
    '        Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "catalog_item.html"))
    '    End Get
    'End Property

    Public ReadOnly Property VideoItemTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "video_file.html"))
        End Get
    End Property

    Public ReadOnly Property AudioItemTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "audio_file.html"))
        End Get
    End Property

    Public ReadOnly Property ReportPageTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "report_page.html"))
        End Get
    End Property

    Public ReadOnly Property DeletePostPageTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "delete_post_page.html"))
        End Get
    End Property

    Public ReadOnly Property ModSBRTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "modSBR.html"))
        End Get
    End Property

    Public ReadOnly Property editPostPageTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "edit_post_page.html"))
        End Get
    End Property

    Public ReadOnly Property BanPageTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "banned_page.html"))

        End Get
    End Property

    Public ReadOnly Property DeletePostFileTemplate() As String
        Get
            Return IO.File.ReadAllText(CombinePath3(GlobalVariables.ApplicationDllRoot, "templates", "delete_file_page.html"))
        End Get
    End Property

#End Region

    Public Property BoardTitle() As String
        Get
            If DataDB.KeyExist("BoardTitle") Then
                Return DataDB.GetKey("BoardTitle")
            Else
                DataDB.AddKey("BoardTitle", "Channel Board")
                DataDB.Save()
                Return "Channel Board"
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("BoardTitle", value)
            DataDB.Save()
        End Set
    End Property

    'Public Property BoardLetter() As String
    '    Get
    '        If DataDB.KeyExist("BoardLetter") = False Then
    '            DataDB.AddKey("BoardLetter", "c")
    '            DataDB.Save()
    '            Return "c"
    '        Else
    '            Return CStr(DataDB.GetKey("BoardLetter"))
    '        End If
    '    End Get
    '    Set(ByVal value As String)
    '        DataDB.AddKey("BoardLetter", value)
    '        DataDB.Save()
    '    End Set
    'End Property

    Public Property BoardDescription() As String
        Get
            If DataDB.KeyExist("BoardDescription") Then
                Return CStr(DataDB.GetKey("BoardDescription"))    
            Else
                DataDB.AddKey("BoardDescription", "ASP.NET Image board")
                DataDB.Save()
                Return "ASP.NET Image board"
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("BoardDescription", value)
            DataDB.Save()
        End Set
    End Property

    Public Property FloodInterval() As Integer
        Get
            If DataDB.KeyExist("FloodInterval") Then
                Return CInt(DataDB.GetKey("FloodInterval"))
            Else
                DataDB.AddKey("FloodInterval", "10")
                DataDB.Save()
                Return 10
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("FloodInterval", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property FooterText() As String
        Get
            If DataDB.KeyExist("FooterText") Then
                Return CStr(DataDB.GetKey("FooterText"))
            Else
                DataDB.AddKey("FooterText", "<a href='https://github.com/diantahoc/chanb' target='_blank'>ChanB</a> ASP.NET board.")
                DataDB.Save()
                Return "<a href='https://github.com/diantahoc/chanb' target='_blank'>ChanB</a> ASP.NET board."
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("FooterText", value)
            DataDB.Save()
        End Set
    End Property

    Public Property MaximumFileSize() As Long
        Get
            If DataDB.KeyExist("MaximumFileSize") Then
                Return CLng(DataDB.GetKey("MaximumFileSize"))
            Else
                DataDB.AddKey("MaximumFileSize", "4194304")
                DataDB.Save()
                Return 4194304
            End If
        End Get
        Set(ByVal value As Long)
            DataDB.AddKey("MaximumFileSize", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property AutoDeleteFiles() As Boolean
        Get
            If DataDB.KeyExist("AutoDeleteFiles") Then
                Return CBool(DataDB.GetKey("AutoDeleteFiles"))
            Else
                DataDB.AddKey("AutoDeleteFiles", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("AutoDeleteFiles", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property ThreadPerPage() As Integer
        Get
            If DataDB.KeyExist("ThreadPerPage") Then
                Return CInt(DataDB.GetKey("ThreadPerPage"))
            Else
                DataDB.AddKey("ThreadPerPage", "10")
                DataDB.Save()
                Return 10
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("ThreadPerPage", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property MaximumPages() As Integer
        Get
            If DataDB.KeyExist("MaximumPages") Then
                Return CInt(DataDB.GetKey("MaximumPages"))
            Else
                DataDB.AddKey("MaximumPages", "10")
                DataDB.Save()
                Return 10
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("MaximumPages", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property AllowDuplicatesFiles() As Boolean
        Get
            If DataDB.KeyExist("AllowDuplicatesFiles") Then
                Return CBool(DataDB.GetKey("AllowDuplicatesFiles"))
            Else
                DataDB.AddKey("AllowDuplicatesFiles", "False")
                DataDB.Save()
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("AllowDuplicatesFiles", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property SmartLinkDuplicateImages() As Boolean
        Get
            If DataDB.KeyExist("SmartLinkDuplicateImages") Then
                Return CBool(DataDB.GetKey("SmartLinkDuplicateImages"))
            Else
                DataDB.AddKey("SmartLinkDuplicateImages", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("SmartLinkDuplicateImages", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property EnableUserID() As Boolean
        Get
            If DataDB.KeyExist("EnableUserID") Then
                Return CBool(DataDB.GetKey("EnableUserID"))
            Else
                DataDB.AddKey("EnableUserID", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnableUserID", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property TrailPostsCount() As Integer
        Get
            If DataDB.KeyExist("TrailPostsCount") Then
                Return CInt(DataDB.GetKey("TrailPostsCount"))
            Else
                DataDB.AddKey("TrailPostsCount", "4")
                DataDB.Save()
                Return 4
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("TrailPostsCount", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property BumpLimit() As Integer
        Get
            If DataDB.KeyExist("BumpLimit") Then
                Return CInt(DataDB.GetKey("BumpLimit"))
            Else
                DataDB.AddKey("BumpLimit", "210")
                DataDB.Save()
                Return 210
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("BumpLimit", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property ResizeMethod() As Integer
        Get
            If DataDB.KeyExist("ResizeMethod") Then
                Return CInt(DataDB.GetKey("ResizeMethod"))
            Else
                DataDB.AddKey("ResizeMethod", "1")
                DataDB.Save()
                Return 1
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("ResizeMethod", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property StorageFolderName() As String
        Get
            If DataDB.KeyExist("StorageFolderName") Then
                Return CStr(DataDB.GetKey("StorageFolderName"))
            Else
                DataDB.AddKey("StorageFolderName", "images")
                DataDB.Save()
                Return "images"
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("StorageFolderName", value)
            DataDB.Save()
        End Set
    End Property

    Public Property PhysicalStorageFolderPath() As String
        Get
            If DataDB.KeyExist("PhysicalStorageFolderPath") Then
                Return CStr(DataDB.GetKey("PhysicalStorageFolderPath"))
            Else
                Return CombinePath(chanb.My.Request.PhysicalApplicationPath, StorageFolderName)
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("PhysicalStorageFolderPath", value)
            DataDB.Save()
        End Set
    End Property

    Public Property WebStorageFolderPath() As String
        Get
            If DataDB.KeyExist("WebStorageFolderPath") Then
                Return CStr(DataDB.GetKey("WebStorageFolderPath"))
            Else
                Dim prefix As String = ""
                If chanb.My.Request.ServerVariables("HTTPS") = "ON" Then
                    prefix = "https://"
                Else
                    prefix = "http://"
                End If
                If chanb.My.Request.ApplicationPath = "/" Then
                    Return prefix & chanb.My.Request.ServerVariables("HTTP_HOST") & chanb.My.Request.ApplicationPath & StorageFolderName & "/"
                Else
                    Return prefix & chanb.My.Request.ServerVariables("HTTP_HOST") & chanb.My.Request.ApplicationPath & "/" & StorageFolderName & "/"
                End If
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("WebStorageFolderPath", value)
            DataDB.Save()
        End Set
    End Property

    Public Property isInstalled() As Boolean
        Get
            If DataDB.KeyExist("isInstalled") Then
                Return CBool(DataDB.GetKey("isInstalled"))
            Else
                DataDB.AddKey("isInstalled", "False")
                DataDB.Save()
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("isInstalled", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property DefaultModPowers() As String
        Get
            If DataDB.KeyExist("DefaultModPowers") Then
                Return CStr(DataDB.GetKey("DefaultModPowers"))
            Else
                DataDB.AddKey("DefaultModPowers", "1-1-1-1-0")
                DataDB.Save()
                Return "1-1-1-1-0"
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("DefaultModPowers", value)
            DataDB.Save()
        End Set
    End Property

    Public Property EnableArchive() As Boolean
        Get
            If DataDB.KeyExist("EnableArchive") Then
                Return CBool(DataDB.GetKey("EnableArchive"))
            Else
                DataDB.AddKey("EnableArchive", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnableArchive", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property EnableSmilies() As Boolean
        Get
            If DataDB.KeyExist("EnableSmilies") Then
                Return CBool(DataDB.GetKey("EnableSmilies"))
            Else
                DataDB.AddKey("EnableSmilies", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnableSmilies", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property EnableCaptcha() As Boolean
        Get
            If DataDB.KeyExist("EnableCaptcha") Then
                Return CBool(DataDB.GetKey("EnableCaptcha"))
            Else
                DataDB.AddKey("EnableCaptcha", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnableCaptcha", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property CaptchaLevel() As Integer
        Get
            If DataDB.KeyExist("CaptchaLevel") Then
                Return CInt(DataDB.GetKey("CaptchaLevel"))
            Else
                DataDB.AddKey("CaptchaLevel", "5")
                DataDB.Save()
                Return 5
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("CaptchaLevel", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property RemoveEXIFData() As Boolean
        Get
            If DataDB.KeyExist("RemoveEXIFData") Then
                Return CBool(DataDB.GetKey("RemoveEXIFData"))
            Else
                DataDB.AddKey("RemoveEXIFData", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("RemoveEXIFData", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property StaticMode() As Boolean
        Get
            If DataDB.KeyExist("StaticMode") Then
                Return CBool(DataDB.GetKey("StaticMode"))   
            Else
                DataDB.AddKey("StaticMode", "False")
                DataDB.Save()
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("StaticMode", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property EnableImpresonationProtection() As Boolean
        Get
            If DataDB.KeyExist("EnableImpresonationProtection") Then
                Return CBool(DataDB.GetKey("EnableImpresonationProtection"))
            Else
                DataDB.AddKey("EnableImpresonationProtection", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnableImpresonationProtection", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property DatabaseType() As String
        Get
            If DataDB.KeyExist("dbType") Then
                Return CStr(DataDB.GetKey("dbType")) 
            Else
                If isInstalled Then
                    Throw New Exception(dbTypeNotSet)
                Else
                    Return ""
                End If
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("dbType", value)
            DataDB.Save()
        End Set
    End Property

    Public Property ConvertArchivedThreadToHTML() As Boolean
        Get
            If DataDB.KeyExist("ConvertArchivedThreadToHTML") Then
                Return CBool(DataDB.GetKey("ConvertArchivedThreadToHTML"))
            Else
                DataDB.AddKey("ConvertArchivedThreadToHTML", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("ConvertArchivedThreadToHTML", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property CheckExifOrientation() As Boolean
        Get
            If DataDB.KeyExist("CheckExifOrientation") Then
                Return CBool(DataDB.GetKey("CheckExifOrientation"))
            Else
                DataDB.AddKey("CheckExifOrientation", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("CheckExifOrientation", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property ShowThreadRepliesCount() As Boolean
        Get
            If DataDB.KeyExist("ShowThreadRepliesCount") Then
                Return CBool(DataDB.GetKey("ShowThreadRepliesCount"))
            Else
                DataDB.AddKey("ShowThreadRepliesCount", "True")
                DataDB.Save()
                Return True
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("ShowThreadRepliesCount", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property ShowPostSuccessfulMessage() As Boolean
        Get
            If DataDB.KeyExist("ShowPostSuccessfulMessage") Then
                Return CBool(DataDB.GetKey("ShowPostSuccessfulMessage"))
            Else
                DataDB.AddKey("ShowPostSuccessfulMessage", "False")
                DataDB.Save()
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("ShowPostSuccessfulMessage", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property EnchancedThumbGeneration() As Boolean
        Get
            If DataDB.KeyExist("EnchancedThumbGeneration") Then
                Return CBool(DataDB.GetKey("EnchancedThumbGeneration"))
            Else
                DataDB.AddKey("EnchancedThumbGeneration", "False")
                DataDB.Save()
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnchancedThumbGeneration", CStr(value))
            DataDB.Save()
        End Set
    End Property

    'Public Property GoogleRecaptchaID() As String
    '    Get
    '        If DataDB.KeyExist("GoogleRecaptchaID") = False Then
    '            DataDB.AddKey("GoogleRecaptchaID", "")
    '            DataDB.Save()
    '            Return ""
    '        Else
    '            Return CStr(DataDB.GetKey("GoogleRecaptchaID"))
    '        End If
    '    End Get
    '    Set(ByVal value As String)
    '        DataDB.AddKey("GoogleRecaptchaID", value)
    '        DataDB.Save()
    '    End Set
    'End Property


    Public Sub UpdateSetting(ByVal name As String, ByVal newData As String)
        DataDB.AddKey(name, newData)
        DataDB.Save()
    End Sub

    'Public Sub UpdateSetting(ByVal name As String, ByVal newData As Integer)
    '    DataDB.AddKey(name, CStr(newData))
    '    DataDB.Save()
    'End Sub

    'Public Sub UpdateSetting(ByVal name As String, ByVal newData As Boolean)
    '    DataDB.AddKey(name, CStr(newData))
    '    DataDB.Save()
    'End Sub


End Class

