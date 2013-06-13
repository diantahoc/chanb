Imports System.Data
Imports mysql.data
Imports MySql.Data.MySqlClient

Public Module Misc

    Public Function FormatHTMLMessage(ByVal title As String, ByVal msg As String, ByVal redirectpage As String, ByVal timeout As String, ByVal critical As Boolean) As String
        Dim messageTemplate As String = GenericMessageTemplate
        messageTemplate = messageTemplate.Replace("%MSG TITLE%", title.ElementAt(0).ToString.ToUpper() & title.Remove(0, 1).ToLower())
        messageTemplate = messageTemplate.Replace("%REDIRECT URL%", redirectpage)
        messageTemplate = messageTemplate.Replace("%REDIRECT DELAY%", timeout)
        messageTemplate = messageTemplate.Replace("%MESSAGE TEXT%", msg)
        If critical Then
            messageTemplate = messageTemplate.Replace("%C%", "DD0000")
        Else
            messageTemplate = messageTemplate.Replace("%C%", "66CCFF")

        End If
        Return messageTemplate
    End Function


    Function MYSQLTEST() As String
        Dim i As New MySqlConnection("server=127.0.0.1;user=root;database=traviantest;port=3306;password=545731;")
        i.Open()
        Dim cmd As MySqlCommand = New MySqlCommand("SELECT wref FROM s1_odata", i)
        Dim reader As MySqlDataReader = cmd.ExecuteReader
        Dim sb As New StringBuilder

        While reader.Read
            sb.Append(CStr(reader(0)))
            sb.Append("<br/>")
        End While
        i.Close()
        Return sb.ToString

    End Function

End Module
