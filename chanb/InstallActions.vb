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

                'Check if there is admin creds.
                If Request.Item("adminname") = "" Or Request.Item("adminpass") = "" Then
                    Response.Write(FormatHTMLMessage(errorStr, "Invalid admin credentials", "", "8888", True))
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

                        Try
                            Dim q As New Data.SqlClient.SqlCommand(My.Resources.mssqlDatabaseSetup, sq)
                            q.ExecuteNonQuery()
                        Catch ex As Exception
                            Response.Write("Error occured while updating database structure: " & ex.Message)
                            Response.End()
                        End Try
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

                        Try
                            Dim q As New MySql.Data.MySqlClient.MySqlCommand(My.Resources.mysqlDatabaseSetup, sq)
                            q.ExecuteNonQuery()
                        Catch ex As Exception
                            Response.Write("Error occured while updating database structure: " & ex.Message)
                            Response.End()
                        End Try


                        'End If

                        sq.Close()
                    Case Else

                        Response.Write(FormatHTMLMessage(errorStr, dbTypeInvalid, "", "8888", True))
                        Response.End()

                End Select


                Dim di As New DataInitializer

                If FileIO.FileSystem.DirectoryExists(Request.Item("StorageFolder")) = False Then FileIO.FileSystem.CreateDirectory(Request.Item("StorageFolder"))

                Dim StorageFolderName As String = FileIO.FileSystem.GetDirectoryInfo(Request.Item("StorageFolder")).Name

                di.StorageFolderName = StorageFolderName

                di.PhysicalStorageFolderPath = Request.Item("StorageFolder")

                di.WebStorageFolderPath = Request.Item("StorageFolderWeb")

                di.BoardTitle = Request.Item("BoardTitle")

                di.BoardDescription = Request.Item("BoardDesc")

                di.FloodInterval = CInt(Request.Item("TimeBetweenRequestes"))

                di.MaximumFileSize = CLng(CInt(Request.Item("MaximumFileSize")) * 1024)

                di.ThreadPerPage = CInt(Request.Item("ThreadPerPage"))

                di.MaximumPages = CInt(Request.Item("MaximumPages"))

                di.TrailPostsCount = CInt(Request.Item("TrailPosts"))

                di.BumpLimit = CInt(Request.Item("BumpLimit"))

                di.EnableUserID = CBool(Request.Item("EnableUserID"))

                di.EnableArchive = CBool(Request.Item("EnableArchive"))

                di.EnableCaptcha = CBool(Request.Item("EnableCaptcha"))
               
                di.CaptchaLevel = CInt(Request.Item("CaptchaLevel"))

                di.AutoDeleteFiles = CBool(Request.Item("DeleteFiles"))

                di.AllowDuplicatesFiles = CBool(Request.Item("AllowDuplicatesFiles"))

                di.DatabaseType = Request.Item("db")

                If Not Request.Item("SmartLinkDuplicateImages") = "" Then
                    di.SmartLinkDuplicateImages = CBool(Request.Item("SmartLinkDuplicateImages"))
                End If

                'Make the admin
                Select Case Request.Item("db").ToLower()
                    Case "mssql"
                        Dim i As New SqlClient.SqlConnection(Request.Item("dbconnectionstring"))
                        i.Open()
                        Dim command As New SqlClient.SqlCommand
                        command.Connection = i
                        command.CommandText = "INSERT INTO mods (username, password, power) VALUES (@username, @pass, @powers)"

                        Dim nameA As New SqlParameter("@username", Data.DbType.String)
                        nameA.Value = Request.Item("adminname")

                        Dim passA As New SqlParameter("@pass", Data.DbType.String)
                        passA.Value = MD5(Request.Item("adminpass"))

                        Dim powerP As New SqlParameter("@powers", Data.DbType.String)
                        powerP.Value = "admin"


                        command.Parameters.Add(nameA)
                        command.Parameters.Add(passA)
                        command.Parameters.Add(powerP)

                        command.ExecuteNonQuery()
                        i.Close()
                    Case "mysql"
                        Dim i As New MySqlConnection(Request.Item("dbconnectionstring"))
                        i.Open()
                        Dim command As New MySqlCommand()
                        command.Connection = i
                        command.CommandText = "INSERT INTO mods (username, password, power) VALUES (@username, @pass, @powers)"

                        Dim nameA As New MySqlParameter("@username", MySqlDbType.Text)
                        nameA.Value = Request.Item("adminname")

                        Dim passA As New MySqlParameter("@pass", MySqlDbType.Text)
                        passA.Value = MD5(Request.Item("adminpass"))

                        Dim powerP As New MySqlParameter("@powers", MySqlDbType.Text)
                        powerP.Value = "admin"


                        command.Parameters.Add(nameA)
                        command.Parameters.Add(passA)
                        command.Parameters.Add(powerP)

                        command.ExecuteNonQuery()
                        i.Close()
                End Select

                'Finally, mark the board as installed.

                di.isInstalled = True

                Response.Write(FormatHTMLMessage(sucessStr, "Installation was sucessful. Click <a href='%O%default.aspx'>here</a> to open your new board.", "%O%default.aspx", "5", False).Replace("%O%", WebRoot))

            Case Else
                Response.Write(FormatHTMLMessage(errorStr, "Invalid install command.", "", "8888", True))
        End Select
    End Sub

End Module
