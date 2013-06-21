Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient

Public Module InstallActions

    Public Sub ProcessCommand(ByVal Request As HttpRequest, ByVal Response As HttpResponse)

        Select Case Request.Item("action")
            Case "testdbconnection"

                'Check if the dbType is specified
                If Request.Item("db") = "" Then
                    Response.Write(FormatHTMLMessage(errorStr, installerDbTypeNotSpecified, "", "8888", True))
                    Response.End()
                End If

                'Check if the database connection string is specified
                If Request.Item("dbconnectionstring") = "" Then
                    Response.Write(FormatHTMLMessage(errorStr, installerDbConnectionStringNotSpecified, "", "8888", True))
                    Response.End()
                End If

                'Now test the connection

                Try

                    Select Case Request.Item("db").ToLower

                        Case "mssql"
                            Dim sq As New Data.SqlClient.SqlConnection(Request.Item("dbconnectionstring"))
                            sq.Open()
                            sq.Close()
                            Response.Write(FormatHTMLMessage(sucessStr, isntallerConnectionEstablishedSucess.Replace("%", "SQL"), "", "8888", False))
                        Case "mysql"
                            Dim sq As New MySql.Data.MySqlClient.MySqlConnection(Request.Item("dbconnectionstring"))
                            sq.Open()
                            sq.Close()
                            Response.Write(FormatHTMLMessage(sucessStr, isntallerConnectionEstablishedSucess.Replace("%", "MySQL"), "", "8888", False))
                        Case Else

                            Response.Write(FormatHTMLMessage(errorStr, dbTypeInvalid, "", "8888", True))
                            Response.End()
                    End Select

                Catch ex As Exception

                    Response.Write(FormatHTMLMessage(errorStr, errorOccuredStr.Replace("%", ex.Message), "", "8888", True))
                End Try

            Case "Upload and run script"

                'Check if the dbType is specified
                If Request.Item("db") = "" Then
                    Response.Write(FormatHTMLMessage(errorStr, installerDbTypeNotSpecified, "", "8888", True))
                    Response.End()
                End If

                'Check if the database connection string is specified
                If Request.Item("dbconnectionstring") = "" Then
                    Response.Write(FormatHTMLMessage(errorStr, installerDbConnectionStringNotSpecified, "", "8888", True))
                    Response.End()
                End If

                'We try to upload the sql script and excute it 
                Try

                    Dim tempfilename As String = Request.PhysicalApplicationPath & "\sqlt"
                    Request.Files("customdbfile").SaveAs(tempfilename) ' Save the file so we can read it later with IO.File.ReadAllText

                    Select Case Request.Item("db").ToLower

                        Case "mssql"

                            Dim sq As New Data.SqlClient.SqlConnection(Request.Item("dbconnectionstring"))
                            sq.Open()
                            Dim q As New Data.SqlClient.SqlCommand(IO.File.ReadAllText(tempfilename), sq)
                            q.ExecuteNonQuery()
                            sq.Close()
                            IO.File.Delete(tempfilename)

                            Response.Write(FormatHTMLMessage("OK", "File executed sucessfully", "", "8887", False))


                        Case "mysql"

                            Dim sq As New MySql.Data.MySqlClient.MySqlConnection(Request.Item("dbconnectionstring"))
                            sq.Open()
                            Dim q As New MySql.Data.MySqlClient.MySqlCommand(IO.File.ReadAllText(tempfilename), sq)
                            q.ExecuteNonQuery()
                            sq.Close()
                            IO.File.Delete(tempfilename)

                            Response.Write(FormatHTMLMessage("OK", "File executed sucessfully", "", "8887", False))

                        Case Else

                            Response.Write(FormatHTMLMessage(errorStr, dbTypeInvalid, "", "8888", True))
                            Response.End()
                    End Select

                Catch ex As Exception
                    Response.Write(FormatHTMLMessage(errorStr, errorOccuredStr.Replace("%", ex.Message), "", "8888", True))
                End Try


            Case "automatically configure the database structure"

                'Check if the dbType is specified
                If Request.Item("db") = "" Then
                    Response.Write(FormatHTMLMessage(errorStr, installerDbTypeNotSpecified, "", "8888", True))
                    Response.End()
                End If

                'Check if the database connection string is specified
                If Request.Item("dbconnectionstring") = "" Then
                    Response.Write(FormatHTMLMessage(errorStr, installerDbConnectionStringNotSpecified, "", "8888", True))
                    Response.End()
                End If


                Dim chanbsqlfile As String = Request.PhysicalApplicationPath & "\chanbsql.sql"
                Dim chanbmySQLfile As String = Request.PhysicalApplicationPath & "\chanbmysql.sql"

                Try
                    Select Case Request.Item("db").ToLower

                        Case "mssql"

                            If FileIO.FileSystem.FileExists(chanbsqlfile) = False Then
                                Response.Write("The chanbsql.sql file was not found. Please upload it to your application root, or upload the script manually")
                            Else
                                Dim sq As New Data.SqlClient.SqlConnection(Request.Item("dbconnectionstring"))
                                sq.Open()
                                Dim q As New Data.SqlClient.SqlCommand(IO.File.ReadAllText(chanbsqlfile), sq)
                                q.ExecuteNonQuery()
                                sq.Close()
                                Response.Write(FormatHTMLMessage("OK", "Database updated sucessfully", "", "8887", False))

                            End If

                        Case "mysql"

                            If FileIO.FileSystem.FileExists(chanbmySQLfile) = False Then
                                Response.Write("The chanbmysql.sql file was not found. Please upload it to your application root, or upload the script manually")
                            Else
                                Dim sq As New MySql.Data.MySqlClient.MySqlConnection(Request.Item("dbconnectionstring"))
                                sq.Open()
                                Dim q As New MySql.Data.MySqlClient.MySqlCommand(IO.File.ReadAllText(chanbmySQLfile), sq)
                                q.ExecuteNonQuery()
                                sq.Close()
                                Response.Write(FormatHTMLMessage("OK", "Database updated sucessfully", "", "8887", False))
                            End If

                        Case Else

                            Response.Write(FormatHTMLMessage(errorStr, dbTypeInvalid, "", "8888", True))
                            Response.End()

                    End Select

                Catch ex As Exception
                    Response.Write(FormatHTMLMessage(errorStr, errorOccuredStr.Replace("%", ex.Message), "", "8888", True))
                End Try

            Case "Install"

                'Check if the dbType is specified
                If Request.Item("db") = "" Then
                    Response.Write(FormatHTMLMessage(errorStr, installerDbTypeNotSpecified, "", "8888", True))
                    Response.End()
                End If

                'Check if the database connection string is specified
                If Request.Item("dbconnectionstring") = "" Then
                    Response.Write(FormatHTMLMessage(errorStr, installerDbConnectionStringNotSpecified, "", "8888", True))
                    Response.End()
                End If

                'The db.txt file is where chanb store the database connection string.
                Dim dbdotTextfile As String = Request.PhysicalApplicationPath & "\bin\db.txt"

                'List of tables that need to be in the database.
                'Dim tablestocheck As String() = {"board", "bans", "mods", "reports", "lockedT", "ioqueue"}

                Select Case Request.Item("db").ToLower
                    Case "mssql"
                        Dim sq As New Data.SqlClient.SqlConnection(Request.Item("dbconnectionstring"))

                        Try
                            'Check for legit connection string 
                            sq.Open()
                            'If a connection is established without errors, we save the connection string.
                            IO.File.WriteAllText(dbdotTextfile, Request.Item("dbconnectionstring"))
                        Catch ex As Exception
                            Response.Write("Invalid connection string or database error, message is : " & ex.Message)
                            Response.End()
                        End Try

                        ''Check for missing tables in the database. One missing table means we need to reconfigure the database structure.

                        'Dim res As Integer = 1

                        'For Each Table As String In tablestocheck
                        '    'MS SQL
                        '    Dim query As New Data.SqlClient.SqlCommand("IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE (TABLE_NAME = '" & Table & "')  )) SELECT 1 AS res ELSE SELECT 0 AS res", sq)

                        '    Dim reader As Data.SqlClient.SqlDataReader = query.ExecuteReader

                        '    While reader.Read
                        '        res = res * CInt(reader(0)) ' If one table don't exist, res = 0, since the sql command will return 0.
                        '    End While
                        '    reader.Close()
                        'Next



                        'If res = 0 Then 'Database need configuring.

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

                        'End If

                        sq.Close()

                    Case "mysql"


                        Dim sq As New MySql.Data.MySqlClient.MySqlConnection(Request.Item("dbconnectionstring"))

                        Try
                            'Check for legit connection string 
                            sq.Open()
                            'If a connection is established without errors, we save the connection string.
                            IO.File.WriteAllText(dbdotTextfile, Request.Item("dbconnectionstring"))
                        Catch ex As Exception
                            Response.Write("Invalid connection string or database error, message is : " & ex.Message)
                            Response.End()
                        End Try

                        ''Check for missing tables in the database. One missing table means we need to reconfigure the database structure.

                        'Dim res As Integer = 1

                        'For Each Table As String In tablestocheck
                        '    'MySQL
                        '    Dim query As New MySql.Data.MySqlClient.MySqlCommand("SELECT TABLE_SCHEMA FROM INFORMATION_SCHEMA.TABLES WHERE ( TABLE_NAME = '" & Table & "' ) AND (TABLE_SCHEMA = '" & sq.Database & "')", sq)

                        '    Dim reader As MySql.Data.MySqlClient.MySqlDataReader = query.ExecuteReader

                        '    While reader.Read
                        '        If Not CStr(reader(0)) = sq.Database Then res = 0
                        '    End While
                        '    reader.Close()
                        'Next


                        'If res = 0 Then 'Database need configuring.

                        Dim chanbmySQLfile As String = Request.PhysicalApplicationPath & "\chanbmysql.sql"
                        If FileIO.FileSystem.FileExists(chanbmySQLfile) = False Then
                            Response.Write("The chanbmysql.sql file was not found. Please upload it to your application root, or upload and excute the script manually")
                            Response.End()
                        Else

                            Try
                                Dim q As New MySql.Data.MySqlClient.MySqlCommand(IO.File.ReadAllText(chanbmySQLfile), sq)
                                q.ExecuteNonQuery()
                            Catch ex As Exception
                                Response.Write("Error occured while updating database structure: " & ex.Message)
                                Response.End()
                            End Try
                        End If

                        'End If

                        sq.Close()
                    Case Else

                        Response.Write(FormatHTMLMessage(errorStr, dbTypeInvalid, "", "8888", True))
                        Response.End()

                End Select

                'Now that we have checked that the database is ok, along with the connection string is stored, lets update the board settings.
                'But first, we check if there is admin creds.

                If Request.Item("adminname") = "" Or Request.Item("adminpass") = "" Then
                    Response.Write(FormatHTMLMessage(errorStr, "Invalid admin credentials", "", "8888", True))
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

                di.UpdateSetting("fi", (Request.Item("TimeBetweenRequestes")))

                di.UpdateSetting("mfs", CInt(Request.Item("MaximumFileSize")) * 1024)

                di.UpdateSetting("tpp", CInt(Request.Item("ThreadPerPage")))

                di.UpdateSetting("maxpages", CInt(Request.Item("MaximumPages")))

                di.UpdateSetting("TrailPosts", CInt(Request.Item("TrailPosts")))

                di.UpdateSetting("BumpLimit", CInt(Request.Item("BumpLimit")))

                di.UpdateSetting("uid", CBool(Request.Item("EnableUserID")))

                di.UpdateSetting("EnableArchive", CBool(Request.Item("EnableArchive")))

                di.UpdateSetting("EnableCaptcha", CBool(Request.Item("EnableCaptcha")))

                di.UpdateSetting("CaptchaLevel", CInt(Request.Item("CaptchaLevel")))

                di.UpdateSetting("DeleteFiles", CBool(Request.Item("DeleteFiles")))

                di.UpdateSetting("allowdups", CBool(Request.Item("AllowDuplicatesFiles")))

                di.UpdateSetting("dbType", Request.Item("db"))

                If Not Request.Item("SmartLinkDuplicateImages") = "" Then
                    di.UpdateSetting("smartlinkdups", CBool(Request.Item("SmartLinkDuplicateImages")))
                End If

                'Finally, mark the board as installed.

                di.UpdateSetting("isinstalled", True)

                Response.Write(FormatHTMLMessage(sucessStr, "Installation was sucessful. Click <a href='default.aspx'>here</a> to open your new board.", "default.aspx", "15", False))


            Case Else
                Response.Write(FormatHTMLMessage(errorStr, "Invalid install command.", "", "8888", True))
        End Select
    End Sub

End Module
