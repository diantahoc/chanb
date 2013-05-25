Public Class DataInitializer

    Dim chanbDLLROOT As String = ""
    Dim boardDataFileDB As String = ""

    Dim DataDB As ValuesStore

    Sub New()
        chanbDLLROOT = chanb.My.Request.PhysicalApplicationPath & "\bin\"
        boardDataFileDB = chanbDLLROOT & "data"
        DataDB = New ValuesStore(boardDataFileDB)
    End Sub

    Public ReadOnly Property OPPostTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "opposttemplate.txt")
        End Get
    End Property

    Public ReadOnly Property ReplyPostTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "posttemplate.txt")
        End Get
    End Property

    Public ReadOnly Property ThreadTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "threadtemplate.txt")
        End Get
    End Property

    Public ReadOnly Property ImageTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "imagetemplate.txt")
        End Get
    End Property

    Public ReadOnly Property ImageRotatorTemplate() As String
        Get
            Return IO.File.ReadAllText(chanbDLLROOT & "rotatortemplate.txt")
        End Get
    End Property

    Public ReadOnly Property BoardTitle() As String
        Get
            If DataDB.KeyExist("boardtitle") = False Then
                DataDB.AddKey("boardtitle", "Channel Board")
                DataDB.Save()
                Return "Channel Board"
            Else
                Return CStr(DataDB.GetKey("boardtitle"))
            End If
        End Get
    End Property

    Public ReadOnly Property BoardLetter() As String
        Get
            If DataDB.KeyExist("boardletter") = False Then
                DataDB.AddKey("boardletter", "c")
                DataDB.Save()
                Return "c"
            Else
                Return CStr(DataDB.GetKey("boardletter"))
            End If
        End Get
    End Property

    Public ReadOnly Property BoardDescription() As String
        Get
            If DataDB.KeyExist("boarddesc") = False Then
                DataDB.AddKey("boarddesc", "ASP.NET Image board")
                DataDB.Save()
                Return "ASP.NET Image board"
            Else
                Return CStr(DataDB.GetKey("boarddesc"))
            End If
        End Get
    End Property

    Public ReadOnly Property FloodInterval() As Integer
        Get
            If DataDB.KeyExist("fi") = False Then
                DataDB.AddKey("fi", "10")
                DataDB.Save()
                Return 10
            Else
                Return CInt(DataDB.GetKey("fi"))
            End If
        End Get
    End Property

    Public ReadOnly Property FooterText() As String
        Get
            If DataDB.KeyExist("FooterText") = False Then
                DataDB.AddKey("FooterText", "<a href='https://github.com/diantahoc/chanb' target='_blank'>ChanB</a> ASP.NET board.")
                DataDB.Save()
                Return "<a href='https://github.com/diantahoc/chanb' target='_blank'>ChanB</a> ASP.NET board."
            Else
                Return CStr(DataDB.GetKey("FooterText"))
            End If
        End Get
    End Property

    Public ReadOnly Property MaximumFileSize() As Long
        Get
            If DataDB.KeyExist("mfs") = False Then
                DataDB.AddKey("mfs", CStr(15 * 1024 * 1024))
                DataDB.Save()
                Return 15 * 1024 * 1024
            Else
                Return CLng(DataDB.GetKey("mfs"))
            End If
        End Get
    End Property

    Public ReadOnly Property AutoDeleteFiles() As Boolean
        Get
            If DataDB.KeyExist("DeleteFiles") = False Then
                DataDB.AddKey("DeleteFiles", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("DeleteFiles"))
            End If
        End Get
    End Property

    Public ReadOnly Property ThreadPerPage() As Integer
        Get
            If DataDB.KeyExist("tpp") = False Then
                DataDB.AddKey("tpp", "10")
                DataDB.Save()
                Return 10
            Else
                Return CInt(DataDB.GetKey("tpp"))
            End If
        End Get
    End Property

    Public ReadOnly Property MaximumPages() As Integer
        Get
            If DataDB.KeyExist("maxpages") = False Then
                DataDB.AddKey("maxpages", "15")
                DataDB.Save()
                Return 15
            Else
                Return CInt(DataDB.GetKey("maxpages"))
            End If
        End Get
    End Property

    Public ReadOnly Property AllowDuplicatesFiles() As Boolean
        Get
            If DataDB.KeyExist("allowdups") = False Then
                DataDB.AddKey("allowdups", CStr(False))
                DataDB.Save()
                Return False
            Else
                Return CBool(DataDB.GetKey("allowdups"))
            End If
        End Get
    End Property

    Public ReadOnly Property SmartLinkDuplicateImages() As Boolean
        Get
            If DataDB.KeyExist("smartlinkdups") = False Then
                DataDB.AddKey("smartlinkdups", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("smartlinkdups"))
            End If
        End Get
    End Property

    Public ReadOnly Property EnableUserID() As Boolean
        Get
            If DataDB.KeyExist("uid") = False Then
                DataDB.AddKey("uid", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("uid"))
            End If
        End Get
    End Property

    Public ReadOnly Property TrailPostsCount() As Integer
        Get
            If DataDB.KeyExist("TrailPosts") = False Then
                DataDB.AddKey("TrailPosts", "4")
                DataDB.Save()
                Return 4
            Else
                Return CInt(DataDB.GetKey("TrailPosts"))
            End If
        End Get
    End Property

    Public ReadOnly Property BumpLimit() As Integer
        Get
            If DataDB.KeyExist("BumpLimit") = False Then
                DataDB.AddKey("BumpLimit", "250")
                DataDB.Save()
                Return 250
            Else
                Return CInt(DataDB.GetKey("BumpLimit"))
            End If
        End Get
    End Property

    Public ReadOnly Property ResizeMethod() As Integer
        Get
            If DataDB.KeyExist("ResizeMethod") = False Then
                DataDB.AddKey("ResizeMethod", "1")
                DataDB.Save()
                Return 1
            Else
                Return CInt(DataDB.GetKey("ResizeMethod"))
            End If
        End Get
    End Property

    Public ReadOnly Property StorageFolderName() As String
        Get
            If DataDB.KeyExist("StorageFolderName") = False Then
                DataDB.AddKey("StorageFolderName", "images")
                DataDB.Save()
                Return "images"
            Else
                Return CStr(DataDB.GetKey("StorageFolderName"))
            End If
        End Get
    End Property

    Public ReadOnly Property PhysicalStorageFolderPath() As String
        Get
            If DataDB.KeyExist("PhysicalStorageFolderPath") = False Then
                Return chanb.My.Request.PhysicalApplicationPath & "\" & StorageFolderName
            Else
                Return CStr(DataDB.GetKey("PhysicalStorageFolderPath"))
            End If
        End Get
    End Property

    Public ReadOnly Property WebStorageFolderPath() As String
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
    End Property

    Public ReadOnly Property isInstalled() As Boolean
        Get
            If DataDB.KeyExist("isinstalled") = False Then
                DataDB.AddKey("isinstalled", CStr(False))
                DataDB.Save()
                Return False
            Else
                Return CBool(DataDB.GetKey("isinstalled"))
            End If
        End Get
    End Property

    Public ReadOnly Property DefaultModPowers() As String
        Get
            If DataDB.KeyExist("DefaultModPowers") = False Then
                DataDB.AddKey("DefaultModPowers", "1-1-1-1-0")
                DataDB.Save()
                Return "1-1-1-1-0"
            Else
                Return CStr(DataDB.GetKey("DefaultModPowers"))
            End If
        End Get
    End Property

    Public ReadOnly Property EnableArchive() As Boolean
        Get
            If DataDB.KeyExist("EnableArchive") = False Then
                DataDB.AddKey("EnableArchive", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("EnableArchive"))
            End If
        End Get
    End Property

    Public ReadOnly Property TransmitRealFileName() As Boolean
        Get
            If DataDB.KeyExist("TransmitRealFileName") = False Then
                DataDB.AddKey("TransmitRealFileName", CStr(False))
                DataDB.Save()
                Return False
            Else
                Return CBool(DataDB.GetKey("TransmitRealFileName"))
            End If
        End Get
    End Property

    Public ReadOnly Property EnableSmilies() As Boolean
        Get
            If DataDB.KeyExist("EnableSmilies") = False Then
                DataDB.AddKey("EnableSmilies", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("EnableSmilies"))
            End If
        End Get
    End Property

    Public ReadOnly Property EnableCaptcha() As Boolean
        Get
            If DataDB.KeyExist("EnableCaptcha") = False Then
                DataDB.AddKey("EnableCaptcha", CStr(True))
                DataDB.Save()
                Return True
            Else
                Return CBool(DataDB.GetKey("EnableCaptcha"))
            End If
        End Get
    End Property

    Public ReadOnly Property CaptchaLevel() As Integer
        Get
            If DataDB.KeyExist("CaptchaLevel") = False Then
                DataDB.AddKey("CaptchaLevel", "5")
                DataDB.Save()
                Return 5
            Else
                Return CInt(DataDB.GetKey("CaptchaLevel"))
            End If
        End Get
    End Property

    Public Sub UpdateSetting(ByVal name As String, ByVal newData As String)
        DataDB.AddKey(name, newData)
        DataDB.Save()
    End Sub

End Class

