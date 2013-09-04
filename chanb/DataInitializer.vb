Public Class DataInitializer

    Dim chanbDLLROOT As String = ""
    Dim boardDataFileDB As String = ""

    Dim DataDB As ValuesStore

    Sub New()
        chanbDLLROOT = chanb.My.Request.PhysicalApplicationPath & "\bin\"
        boardDataFileDB = chanbDLLROOT & "data"
        DataDB = New ValuesStore(boardDataFileDB)
    End Sub

#Region "Templates files"

    Public ReadOnly Property OPPostTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\opposttemplate.html")
        End Get
    End Property

    Public ReadOnly Property ReplyPostTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\posttemplate.html")
        End Get
    End Property

    Public ReadOnly Property ThreadTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\threadtemplate.html")
        End Get
    End Property

    Public ReadOnly Property ImageTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\imagetemplate.html")
        End Get
    End Property

    Public ReadOnly Property FilesRotatorTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\rotatortemplate.html")
        End Get
    End Property

    Public ReadOnly Property FullPageTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\fullPageTemplate.html")
            '  Return IO.File.ReadAllText("C:\Users\Istan\Documents\Visual Studio 2008\Projects\tinyboardasp\tinyboardasp\fullPageTemplate.html")
        End Get
    End Property

    Public ReadOnly Property CatalogItemTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\catalogItemTemplate.html")
        End Get
    End Property

    Public ReadOnly Property VideoItemTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\videoTemplate.html")
        End Get
    End Property

    Public ReadOnly Property AudioItemTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\audiofileTemplate.html")
        End Get
    End Property

    Public ReadOnly Property ReportPageTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\reportPage.html")
        End Get
    End Property

    Public ReadOnly Property DeletePostPageTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\deletepostPage.html")
        End Get
    End Property

    Public ReadOnly Property ModSBRTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\modSBR.html")
        End Get
    End Property

    Public ReadOnly Property editPostPageTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\editPostPageTemplate.html")
        End Get
    End Property

    Public ReadOnly Property BanPageTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\bannedPageTemplate.html")
        End Get
    End Property

    Public ReadOnly Property ErrorPageTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "templates\errorPageTemplate.html")
        End Get
    End Property

#End Region

    Public Property BoardTitle() As String
        Get
            If DataDB.KeyExist("BoardTitle") = False Then
                DataDB.AddKey("BoardTitle", "Channel Board")
                DataDB.Save()
                Return "Channel Board"
            Else
                Return CStr(DataDB.GetKey("BoardTitle"))
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("BoardTitle", value)
            DataDB.Save()
        End Set
    End Property

    Public Property BoardLetter() As String
        Get
            If DataDB.KeyExist("BoardLetter") = False Then
                DataDB.AddKey("BoardLetter", "c")
                DataDB.Save()
                Return "c"
            Else
                Return CStr(DataDB.GetKey("BoardLetter"))
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("BoardLetter", value)
            DataDB.Save()
        End Set
    End Property

    Public Property BoardDescription() As String
        Get
            If DataDB.KeyExist("BoardDescription") = False Then
                DataDB.AddKey("BoardDescription", "ASP.NET Image board")
                DataDB.Save()
                Return "ASP.NET Image board"
            Else
                Return CStr(DataDB.GetKey("BoardDescription"))
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("BoardDescription", value)
            DataDB.Save()
        End Set
    End Property

    Public Property FloodInterval() As Integer
        Get
            If DataDB.KeyExist("FloodInterval") = False Then
                DataDB.AddKey("FloodInterval", "10")
                DataDB.Save()
                Return 10
            Else
                Return CInt(DataDB.GetKey("FloodInterval"))
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("FloodInterval", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property FooterText() As String
        Get
            If DataDB.KeyExist("FooterText") = False Then
                DataDB.AddKey("FooterText", "<a href='https://github.com/diantahoc/chanb' target='_blank'>ChanB</a> ASP.NET board.")
                DataDB.Save()
                Return "<a href='https://github.com/diantahoc/chanb' target='_blank'>ChanB</a> ASP.NET board."
            Else
                Return CStr(DataDB.GetKey("FooterText"))
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("FooterText", value)
            DataDB.Save()
        End Set
    End Property

    Public Property MaximumFileSize() As Long
        Get
            If DataDB.KeyExist("MaximumFileSize") = False Then
                DataDB.AddKey("MaximumFileSize", CStr(15 * 1024 * 1024))
                DataDB.Save()
                Return 15 * 1024 * 1024
            Else
                Return CLng(DataDB.GetKey("MaximumFileSize"))
            End If
        End Get
        Set(ByVal value As Long)
            DataDB.AddKey("MaximumFileSize", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property AutoDeleteFiles() As Boolean
        Get
            If DataDB.KeyExist("AutoDeleteFiles") = False Then
                DataDB.AddKey("AutoDeleteFiles", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("AutoDeleteFiles"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("AutoDeleteFiles", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property ThreadPerPage() As Integer
        Get
            If DataDB.KeyExist("ThreadPerPage") = False Then
                DataDB.AddKey("ThreadPerPage", "10")
                DataDB.Save()
                Return 10
            Else
                Return CInt(DataDB.GetKey("ThreadPerPage"))
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("ThreadPerPage", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property MaximumPages() As Integer
        Get
            If DataDB.KeyExist("MaximumPages") = False Then
                DataDB.AddKey("MaximumPages", "10")
                DataDB.Save()
                Return 10
            Else
                Return CInt(DataDB.GetKey("MaximumPages"))
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("MaximumPages", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property AllowDuplicatesFiles() As Boolean
        Get
            If DataDB.KeyExist("AllowDuplicatesFiles") = False Then
                DataDB.AddKey("AllowDuplicatesFiles", CStr(False))
                DataDB.Save()
                Return False
            Else
                Return CBool(DataDB.GetKey("AllowDuplicatesFiles"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("AllowDuplicatesFiles", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property SmartLinkDuplicateImages() As Boolean
        Get
            If DataDB.KeyExist("SmartLinkDuplicateImages") = False Then
                DataDB.AddKey("SmartLinkDuplicateImages", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("SmartLinkDuplicateImages"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("SmartLinkDuplicateImages", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property EnableUserID() As Boolean
        Get
            If DataDB.KeyExist("EnableUserID") = False Then
                DataDB.AddKey("EnableUserID", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("EnableUserID"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnableUserID", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property TrailPostsCount() As Integer
        Get
            If DataDB.KeyExist("TrailPostsCount") = False Then
                DataDB.AddKey("TrailPostsCount", "4")
                DataDB.Save()
                Return 4
            Else
                Return CInt(DataDB.GetKey("TrailPostsCount"))
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("TrailPostsCount", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property BumpLimit() As Integer
        Get
            If DataDB.KeyExist("BumpLimit") = False Then
                DataDB.AddKey("BumpLimit", "250")
                DataDB.Save()
                Return 250
            Else
                Return CInt(DataDB.GetKey("BumpLimit"))
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("BumpLimit", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property ResizeMethod() As Integer
        Get
            If DataDB.KeyExist("ResizeMethod") = False Then
                DataDB.AddKey("ResizeMethod", "1")
                DataDB.Save()
                Return 1
            Else
                Return CInt(DataDB.GetKey("ResizeMethod"))
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("ResizeMethod", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property StorageFolderName() As String
        Get
            If DataDB.KeyExist("StorageFolderName") = False Then
                DataDB.AddKey("StorageFolderName", "images")
                DataDB.Save()
                Return "images"
            Else
                Return CStr(DataDB.GetKey("StorageFolderName"))
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("StorageFolderName", value)
            DataDB.Save()
        End Set
    End Property

    Public Property PhysicalStorageFolderPath() As String
        Get
            If DataDB.KeyExist("PhysicalStorageFolderPath") = False Then
                Return chanb.My.Request.PhysicalApplicationPath & "\" & StorageFolderName
            Else
                Return CStr(DataDB.GetKey("PhysicalStorageFolderPath"))
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("PhysicalStorageFolderPath", value)
            DataDB.Save()
        End Set
    End Property

    Public Property WebStorageFolderPath() As String
        Get
            If DataDB.KeyExist("WebStorageFolderPath") = False Then
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
            Else
                Return CStr(DataDB.GetKey("WebStorageFolderPath"))
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("WebStorageFolderPath", value)
            DataDB.Save()
        End Set
    End Property

    Public Property isInstalled() As Boolean
        Get
            If DataDB.KeyExist("isInstalled") = False Then
                DataDB.AddKey("isInstalled", CStr(False))
                DataDB.Save()
                Return False
            Else
                Return CBool(DataDB.GetKey("isInstalled"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("isInstalled", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property DefaultModPowers() As String
        Get
            If DataDB.KeyExist("DefaultModPowers") = False Then
                DataDB.AddKey("DefaultModPowers", "1-1-1-1-0")
                DataDB.Save()
                Return "1-1-1-1-0"
            Else
                Return CStr(DataDB.GetKey("DefaultModPowers"))
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("DefaultModPowers", value)
            DataDB.Save()
        End Set
    End Property

    Public Property EnableArchive() As Boolean
        Get
            If DataDB.KeyExist("EnableArchive") = False Then
                DataDB.AddKey("EnableArchive", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("EnableArchive"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnableArchive", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property EnableSmilies() As Boolean
        Get
            If DataDB.KeyExist("EnableSmilies") = False Then
                DataDB.AddKey("EnableSmilies", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("EnableSmilies"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnableSmilies", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property EnableCaptcha() As Boolean
        Get
            If DataDB.KeyExist("EnableCaptcha") = False Then
                DataDB.AddKey("EnableCaptcha", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("EnableCaptcha"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnableCaptcha", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property CaptchaLevel() As Integer
        Get
            If DataDB.KeyExist("CaptchaLevel") = False Then
                DataDB.AddKey("CaptchaLevel", "5")
                DataDB.Save()
                Return 5
            Else
                Return CInt(DataDB.GetKey("CaptchaLevel"))
            End If
        End Get
        Set(ByVal value As Integer)
            DataDB.AddKey("CaptchaLevel", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property RemoveEXIFData() As Boolean
        Get
            If DataDB.KeyExist("RemoveEXIFData") = False Then
                DataDB.AddKey("RemoveEXIFData", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("RemoveEXIFData"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("RemoveEXIFData", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property StaticMode() As Boolean
        Get
            If DataDB.KeyExist("StaticMode") = False Then
                DataDB.AddKey("StaticMode", CStr(False))
                DataDB.Save()
                Return False
            Else
                Return CBool(DataDB.GetKey("StaticMode"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("StaticMode", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property EnableImpresonationProtection() As Boolean
        Get
            If DataDB.KeyExist("EnableImpresonationProtection") = False Then
                DataDB.AddKey("EnableImpresonationProtection", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("EnableImpresonationProtection"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("EnableImpresonationProtection", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property DatabaseType() As String
        Get
            If DataDB.KeyExist("dbType") = False Then
                If isInstalled = True Then
                    Throw New Exception(dbTypeNotSet)
                Else
                    Return ""
                End If
            Else
                Return CStr(DataDB.GetKey("dbType"))
            End If
        End Get
        Set(ByVal value As String)
            DataDB.AddKey("dbType", value)
            DataDB.Save()
        End Set
    End Property

    Public Property ConvertArchivedThreadToHTML() As Boolean
        Get
            If DataDB.KeyExist("ConvertArchivedThreadToHTML") = False Then
                DataDB.AddKey("ConvertArchivedThreadToHTML", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("ConvertArchivedThreadToHTML"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("ConvertArchivedThreadToHTML", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property CheckExifOrientation() As Boolean
        Get
            If DataDB.KeyExist("CheckExifOrientation") = False Then
                DataDB.AddKey("CheckExifOrientation", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("CheckExifOrientation"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("CheckExifOrientation", CStr(value))
            DataDB.Save()
        End Set
    End Property

    Public Property ShowThreadRepliesCount() As Boolean
        Get
            If DataDB.KeyExist("ShowThreadRepliesCount") = False Then
                DataDB.AddKey("ShowThreadRepliesCount", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("ShowThreadRepliesCount"))
            End If
        End Get
        Set(ByVal value As Boolean)
            DataDB.AddKey("ShowThreadRepliesCount", CStr(value))
            DataDB.Save()
        End Set
    End Property

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

