Imports Microsoft.VisualBasic.FileIO
Imports System.IO

''' <summary>
''' A class that represent a simple dictionary / setting storing system.
''' Written by diantahoc
''' Project: Novelli Torrent
''' </summary>
''' <remarks></remarks>
Public Class ValuesStore

    Dim _name As String = ""
    Dim dic As New Dictionary(Of String, String)
    Dim Seperator As Char = CChar("╝")

    Sub New(ByVal name As String)
        If FileSystem.FileExists(name & ".dic") = False Then
            File.Create(name & ".dic").Close()
        Else
            LoadData(name & ".dic")
        End If
        _name = name & ".dic"
    End Sub

    Private Sub LoadData(ByVal path As String)
        Dim data As String() = IO.File.ReadAllLines(path)
        For i As Integer = 0 To data.Length - 1 Step 1
            Try
                Dim sp As String() = data(i).Split(Seperator)
                dic.Add(sp(0).Trim, sp(1))
            Catch ex As Exception
            End Try
        Next
        data = Nothing
    End Sub

    Public Function GetKey(ByVal KeyName As String) As String
        If dic.Keys.Contains(KeyName) Then
            Return dic(KeyName)
        Else
            Return String.Empty
        End If
    End Function

    Function GetKeyList() As String()
        Dim illist As New List(Of String)
        For Each x In dic.Keys
            illist.Add(x)
        Next
        Return illist.ToArray
    End Function

    ''' <summary>
    ''' Update an existing key or add a new key.
    ''' </summary>
    Sub AddKey(ByVal KeyName As String, ByVal value As String)
        If dic.Keys.Contains(KeyName) Then
            dic.Item(KeyName) = value
        Else
            dic.Add(KeyName, value)
        End If
    End Sub

    Sub Save()
        Dim il As New List(Of String)
        For Each x In dic
            il.Add(x.Key & Seperator & x.Value)
        Next
        IO.File.WriteAllLines(_name, il.ToArray)
        il = Nothing
    End Sub

    Function KeyExist(ByVal name As String) As Boolean
        Return dic.Keys.Contains(name)
    End Function

End Class

'Public Class ValuesStore

'    Dim _name As String = ""
'    Dim il As New List(Of String)

'    Sub New(ByVal name As String)
'        If FileSystem.FileExists(name & ".as") = False Then
'            File.Create(name & ".as").Close()
'        Else
'            il.AddRange(File.ReadAllLines(name & ".as"))
'        End If
'        _name = name & ".as"
'    End Sub

'    Function GetKey(ByVal name As String) As Object
'        For Each x In il
'            If x.Split(CChar("╝")).ElementAt(0).ToLower = name.ToLower Then
'                Return x.Split(CChar("╝")).ElementAt(1)
'            End If
'        Next
'        Return Nothing
'    End Function

'    Function GetKeyList() As String()
'        Dim illist As New List(Of String)
'        For Each x In il
'            If x.Contains(CChar("╝")) Then illist.Add(x.Split(CChar("╝")).ElementAt(0))
'        Next
'        Return illist.toarray
'    End Function

'    ''' <summary>
'    ''' Update an existing key or add a new key.
'    ''' </summary>
'    ''' <param name="name"></param>
'    ''' <param name="value"></param>
'    ''' <remarks></remarks>
'    ''' 
'    Sub AddKey(ByVal name As String, ByVal value As String)
'        Dim copy As New List(Of String)
'        copy.AddRange(il.ToArray)
'        For Each x In copy
'            If x.Split(CChar("╝")).ElementAt(0).ToLower = name.ToLower Then
'                il.Item(copy.IndexOf(x)) = name.ToLower & "╝" & value
'                Exit Sub
'            End If
'        Next
'        copy.Clear()
'        il.Add(name & "╝" & value)
'    End Sub

'    Sub Save()
'        File.WriteAllLines(_name, il.ToArray)
'    End Sub

'    Function KeyExist(ByVal name As String) As Boolean
'        Dim b As Boolean = False
'        For Each x In il
'            If x.Split(CChar("╝")).ElementAt(0).ToLower = name.ToLower Then b = True
'        Next
'        Return b
'    End Function

'End Class