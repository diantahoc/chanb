<%@ Import Namespace="chanb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace = "chanb.Language" %>

<%
    If Not isInstalled Then
        
        Select Case Request.Item("action")
            Case "testdbconnection"
                Try
                    Dim sq As New Data.SqlClient.SqlConnection(Request.Item("dbconnectionstring"))
                    sq.Open()
                    sq.Close()
                    Response.Write(FormatHTMLMessage("OK", "SQL connection was sucessfully established.", "", "8888", False))
                Catch ex As Exception
           
                    Response.Write(FormatHTMLMessage("Error", "SQL connection error: " & ex.Message, "", "8888", True))
                End Try
            Case "Upload and run script"
                
                If Request.Item("dbconnectionstring") = "" Then
                    Response.Write(FormatHTMLMessage("Error", "Invalid connection string specified", "", "8888", True))
                    Response.End()
                End If
                
                Try
                    Dim tempfilename As String = Request.PhysicalApplicationPath & "\sqlt"
                    Request.Files("customdbfile").SaveAs(tempfilename)
                    Dim sq As New Data.SqlClient.SqlConnection(Request.Item("dbconnectionstring"))
                    sq.Open()
                    Dim q As New Data.SqlClient.SqlCommand(IO.File.ReadAllText(tempfilename), sq)
                    q.ExecuteNonQuery()
                    sq.Close()
                    IO.File.Delete(tempfilename)
                    Response.Write("File executed sucessfully")
                Catch ex As Exception
                    Response.Write("Error occured: " & ex.Message)
                End Try
            Case "automatically configure the database structure"
                
                If Request.Item("dbconnectionstring") = "" Then
                    Response.Write("Invalid connection string specified")
                    Response.End()
                End If
                
                
                Dim chanbsqlfile As String = Request.PhysicalApplicationPath & "\chanbsql.sql"
                If FileIO.FileSystem.FileExists(chanbsqlfile) = False Then
                    Response.Write("The chanbsql.sql file was not found. Please upload it to your application root, or upload the script manually")
                Else
                    Try
                        Dim tempfilename As String = Request.PhysicalApplicationPath & "\sqlt"
                        Request.Files("customdbfile").SaveAs(tempfilename)
                        Dim sq As New Data.SqlClient.SqlConnection(Request.Item("dbconnectionstring"))
                        sq.Open()
                        Dim q As New Data.SqlClient.SqlCommand(IO.File.ReadAllText(chanbsqlfile), sq)
                        q.ExecuteNonQuery()
                        sq.Close()
                        IO.File.Delete(tempfilename)
                        Response.Write("Database updated sucessfully")
                    Catch ex As Exception
                        Response.Write("Error occured: " & ex.Message)
                    End Try
                End If

            Case "Install"
              
                
                Dim dbdotTextfile As String = Request.PhysicalApplicationPath & "\bin\db.txt"
               
                Dim sq As New Data.SqlClient.SqlConnection(Request.Item("dbconnectionstring"))
               
                Try
                    'Check for legit connection string 
                    sq.Open()
                    IO.File.WriteAllText(dbdotTextfile, Request.Item("dbconnectionstring"))
                Catch ex As Exception
                    Response.Write("Invalid connection string or database error, message is : " & ex.Message)
                    Response.End()
                End Try

                'Check for structured table
                
                Dim tablestocheck As String() = {"board", "bans", "mods", "reports", "lockedT", "ioqueue"}
                
                Dim res As Integer = 1
                
                For Each Table In tablestocheck
                    Dim query As New Data.SqlClient.SqlCommand("IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE (TABLE_NAME = '" & Table & "')  )) SELECT 1 AS res ELSE SELECT 0 AS res", sq)
                
                    Dim reader As Data.SqlClient.SqlDataReader = query.ExecuteReader

                    While reader.Read
                        res = res * CInt(reader(0)) ' If one table don't exist, res = 0, since the sql command will return 0.
                    End While
                    reader.Close()
                Next
                
                
                If res = 0 Then
                    'Database need configuring.
                    Dim chanbsqlfile As String = Request.PhysicalApplicationPath & "\chanbsql.sql"
                    If FileIO.FileSystem.FileExists(chanbsqlfile) = False Then
                        Response.Write("The chanbsql.sql file was not found. Please upload it to your application root, or upload and excute the script manually")
                        Response.End()
                    Else
                        Try
                            Dim q As New Data.SqlClient.SqlCommand(IO.File.ReadAllText(chanbsqlfile), sq)
                           
                            q.ExecuteNonQuery()
                        Catch ex As Exception
                            Response.Write("Error occured while updating database structure: " & ex.Message)
                            Response.End()
                        End Try
                    End If
                End If
                sq.Close()
               
                'Now that we have checked that the database is ok, along with the connection string is stored, lets update the board settings.
                'But first, we check if there is admin creds.
                
                If Request.Item("adminname") = "" Or Request.Item("adminpass") = "" Then
                    Response.Write("Invalid admin credentials")
                    Response.End()
                Else
                    NewMod(Request.Item("adminname"), Request.Item("adminpass"), "admin")
                End If
                
                Dim di As New DataInitializer
                
                If FileIO.FileSystem.DirectoryExists(Request.Item("StorageFolder")) = False Then FileIO.FileSystem.CreateDirectory(Request.Item("StorageFolder"))
                
                Dim StorageFolderName As String = FileIO.FileSystem.GetDirectoryInfo(Request.Item("StorageFolder")).Name
                
                di.UpdateSetting("StorageFolderName", StorageFolderName)
                
                di.UpdateSetting("PhysicalStorageFolderPath", Request.Item("StorageFolder"))
                
                di.UpdateSetting("WebStorageFolderPath", Request.Item("StorageFolderWeb"))
                
                di.UpdateSetting("boardtitle", Request.Item("BoardTitle"))
                
                di.UpdateSetting("boarddesc", Request.Item("BoardDesc"))
                
                di.UpdateSetting("fi", CInt(Request.Item("TimeBetweenRequestes")))
                
                di.UpdateSetting("mfs", CInt(Request.Item("MaximumFileSize")) * 1024)
                
                di.UpdateSetting("tpp", CInt(Request.Item("ThreadPerPage")))
                
                di.UpdateSetting("maxpages", CInt(Request.Item("MaximumPages")))
                
                di.UpdateSetting("TrailPosts", CInt(Request.Item("TrailPosts")))
                
                di.UpdateSetting("BumpLimit", CInt(Request.Item("BumpLimit")))
                
                di.UpdateSetting("uid", CBool(Request.Item("EnableUserID")))
                
                di.UpdateSetting("EnableArchive", CBool(Request.Item("EnableArchive")))
                
                di.UpdateSetting("TransmitRealFileName", CBool(Request.Item("transmitRealFileName")))
                
                di.UpdateSetting("EnableCaptcha", CBool(Request.Item("EnableCaptcha")))
                
                di.UpdateSetting("CaptchaLevel", CInt(Request.Item("CaptchaLevel")))
                
                di.UpdateSetting("DeleteFiles", CBool(Request.Item("DeleteFiles")))
                
                di.UpdateSetting("allowdups", CBool(Request.Item("AllowDuplicatesFiles")))
                
                If Not Request.Item("SmartLinkDuplicateImages") = "" Then
                    di.UpdateSetting("smartlinkdups", CBool(Request.Item("SmartLinkDuplicateImages")))
                End If
                
                'Finally, mark the board as installed.
                
                di.UpdateSetting("isinstalled", True)
                
                Response.Write("Installation was sucessful. Click <a href='default.aspx'>here</a> to open your new board.")
                
            Case Else
                Response.Write("Admin privelige is required.")
        End Select
        
    Else ' Process normal admin actions
        
        If CBool(Session("admin")) = False Then
            Response.Write("Admin privelige is required.")
        Else
            Select Case Request.Item("action")
                Case "updatesettings"
                    Try
                        Dim stmanager As New DataInitializer()
                        stmanager.UpdateSetting(Request.Item("stname"), Request.Item("stvalue"))
                        Response.Write("OK")
                    Catch ex As Exception
                        Response.Write(ex.Message)
                    End Try
                Case "newmod"
                    Try
                        NewMod(Request.Item("newmodname"), Request.Item("newmodpass"), Request.Item("newmodpowers"))
                        Response.Write("OK")
                    Catch ex As Exception
                        Response.Write(ex.Message)
                    End Try
                Case "deletemod"
                Case "updatemod"
                Case "changepass"
                Case "permadelpost"
                Case "stickythread"
                Case "lockthread"
                Case "editpost"
                Case "editimage"
            End Select
        End If
    End If
    
    
   
    
%>