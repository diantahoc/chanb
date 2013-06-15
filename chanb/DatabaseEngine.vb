Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports System.Data.Common

Public Module DatabaseEngine

    Dim dbi As New DBInitializer

    Sub New()
        If Not (dbType = "mysql" Or dbType = "mssql") Then Throw New Exception(dbTypeInvalid)
    End Sub

    ''' <summary>
    ''' Execute text query.
    ''' </summary>
    ''' <param name="QueryText"></param>
    ''' <returns>Number of rows affected</returns>
    ''' <remarks></remarks>
    Public Function ExecuteNonQuery(ByVal QueryText As String) As Integer
        Select Case dbType
            Case "mssql"
                Dim i As New SqlConnection(dbi.ConnectionString)
                Dim command As New SqlCommand(QueryText, i)
                i.Open()
                Dim d As Integer = command.ExecuteNonQuery
                i.Close()
                Return d
            Case "mysql"
                Dim i As New MySqlConnection(dbi.ConnectionString)
                Dim command As New MySqlCommand(QueryText, i)
                i.Open()
                Dim d As Integer = command.ExecuteNonQuery
                i.Close()
                Return d
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Execute DbCommand query.
    ''' </summary>
    ''' <param name="Query"></param>
    ''' <returns>Number of rows affected</returns>
    ''' <remarks></remarks>
    Public Function ExecuteNonQuery(ByVal Query As DbCommand) As Integer
        Select Case dbType
            Case "mssql"
                Dim i As New SqlConnection(dbi.ConnectionString)
                i.Open()
                Query.Connection = i
                Dim d As Integer = Query.ExecuteNonQuery
                i.Close()
                Return d
            Case "mysql"
                Dim i As New MySqlConnection(dbi.ConnectionString)
                i.Open()
                Query.Connection = i
                Query.Prepare()
                Dim d As Integer = Query.ExecuteNonQuery
                i.Close()
                Return d
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Execute text query with an existing connection.
    ''' </summary>
    ''' <param name="QueryText"></param>
    ''' <returns>Number of rows affected</returns>
    ''' <remarks></remarks>
    Public Function ExecuteNonQuery(ByVal QueryText As String, ByVal i As DbConnection) As Integer
        Select Case dbType
            Case "mssql"
                Dim command As New SqlCommand(QueryText, CType(i, SqlConnection))
                Dim d As Integer = command.ExecuteNonQuery
                Return d
            Case "mysql"
                Dim command As New MySqlCommand(QueryText, CType(i, MySqlConnection))
                Dim d As Integer = command.ExecuteNonQuery
                Return d
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Execute DbCommand query with an existing connection.
    ''' </summary>
    ''' <param name="Query"></param>
    ''' <returns>Number of rows affected</returns>
    ''' <remarks></remarks>
    Public Function ExecuteNonQuery(ByVal Query As DbCommand, ByVal i As DbConnection) As Integer
        Select Case dbType
            Case "mssql"   
                Query.Connection = i
                Dim d As Integer = Query.ExecuteNonQuery
                Return d
            Case "mysql"
                Query.Connection = i
                Dim d As Integer = Query.ExecuteNonQuery
                Return d
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Execute text query.
    ''' </summary>
    ''' <param name="QueryText"></param>
    ''' <returns>ChanbQuery</returns>
    ''' <remarks></remarks>
    Public Function ExecuteQueryReader(ByVal QueryText As String) As ChanbQuery
        Select Case dbType
            Case "mssql"
                Dim i As New SqlConnection(dbi.ConnectionString)
                Dim command As New SqlCommand(QueryText, i)
                i.Open()
                Dim reader As SqlDataReader = command.ExecuteReader
                Dim c As New ChanbQuery
                c.Connection = i
                c.Reader = reader
                Return c
            Case "mysql"
                Dim i As New MySqlConnection(dbi.ConnectionString)
                Dim command As New MySqlCommand(QueryText, i)
                i.Open()
                Dim reader As MySqlDataReader = command.ExecuteReader
                Dim c As New ChanbQuery
                c.Connection = i
                c.Reader = reader
                Return c
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Execute DbCommand query.
    ''' </summary>
    ''' <param name="Query"></param>
    ''' <returns>ChanbQuery</returns>
    ''' <remarks></remarks>
    Public Function ExecuteQueryReader(ByVal Query As DbCommand) As ChanbQuery
        Select Case dbType
            Case "mssql"
                Dim i As New SqlConnection(dbi.ConnectionString)
                Query.Connection = i
                i.Open()
                Dim reader As SqlDataReader = CType(Query.ExecuteReader, SqlDataReader)
                Dim c As New ChanbQuery
                c.Connection = i
                c.Reader = reader
                Return c
            Case "mysql"
                Dim i As New MySqlConnection(dbi.ConnectionString)
                Query.Connection = i
                i.Open()
                Dim reader As MySqlDataReader = CType(Query.ExecuteReader, MySqlDataReader)
                Dim c As New ChanbQuery
                c.Connection = i
                c.Reader = reader
                Return c
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Execute text query with an existing connection.
    ''' </summary>
    ''' <param name="QueryText"></param>
    ''' <param name="i"></param>
    ''' <returns>ChanbQuery</returns>
    ''' <remarks></remarks>
    Public Function ExecuteQueryReader(ByVal QueryText As String, ByVal i As DbConnection) As ChanbQuery
        Select Case dbType
            Case "mssql"
                Dim command As New SqlCommand(QueryText, CType(i, SqlConnection))
                Dim reader As SqlDataReader = command.ExecuteReader
                Dim c As New ChanbQuery
                c.Connection = i
                c.Reader = reader
                Return c
            Case "mysql"
                Dim command As New MySqlCommand(QueryText, CType(i, MySqlConnection))
                Dim reader As MySqlDataReader = command.ExecuteReader
                Dim c As New ChanbQuery
                c.Connection = i
                c.Reader = reader
                Return c
            Case Else
                Return Nothing
        End Select
    End Function

    ''' <summary>
    ''' Execute DbCommand query with an existing connection.
    ''' </summary>
    ''' <param name="Query"></param>
    ''' <param name="i"></param>
    ''' <returns>ChanbQuery</returns>
    ''' <remarks></remarks>
    Public Function ExecuteQueryReader(ByVal Query As DbCommand, ByVal i As DbConnection) As ChanbQuery
        Select Case dbType
            Case "mssql"
                Query.Connection = i
                Dim reader As SqlDataReader = CType(Query.ExecuteReader, SqlDataReader)
                Dim c As New ChanbQuery
                c.Connection = i
                c.Reader = reader
                Return c
            Case "mysql"
                Query.Connection = i
                Dim reader As MySqlDataReader = CType(Query.ExecuteReader, MySqlDataReader)
                Dim c As New ChanbQuery
                c.Connection = i
                c.Reader = reader
                Return c
            Case Else          
                Return Nothing
        End Select
    End Function

    Public Function GenerateDbCommand() As DbCommand
        Select Case dbType
            Case "mssql"
                Dim dbConnection As New SqlConnection(dbi.ConnectionString)
                Dim dbC As New SqlCommand()
                dbConnection.Open()
                dbC.Connection = dbConnection
                Return dbC
            Case "mysql"
                Dim dbConnection As New MySqlConnection(dbi.ConnectionString)
                Dim dbC As New MySqlCommand()
                dbConnection.Open()
                dbC.Connection = dbConnection
                Return dbC
            Case Else
                Return Nothing
        End Select
    End Function

    Public Function MakeParameter(ByVal name As String, ByVal value As Object, ByVal type As DbType) As DbParameter
        Select Case dbType
            Case "mssql"
                Dim p As New SqlParameter(name, type)
                p.Value = value
                Return p
            Case "mysql"
                Dim p As New MySqlParameter(name, value)
                Select Case type
                    Case Data.DbType.String
                        p.MySqlDbType = MySqlDbType.Text
                    Case Data.DbType.DateTime
                        p.MySqlDbType = MySqlDbType.DateTime
                    Case Data.DbType.Int32
                        p.MySqlDbType = MySqlDbType.Int32
                    Case Data.DbType.AnsiString
                        p.MySqlDbType = MySqlDbType.Text
                End Select
                Return p
            Case Else
               Return Nothing
        End Select
    End Function


End Module

Public Class ChanbQuery

    Public Reader As IDataReader
    Public Connection As DbConnection

End Class