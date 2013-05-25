Imports Microsoft.VisualBasic.FileIO
Imports System.IO

''' <summary>
''' A class that represent a simple dictionary / data storing system.
''' Written by diantahoc
''' Project: Novelli Torrent
''' </summary>
''' <remarks></remarks>
Public Class ValuesStore

    Dim _name As String = ""
    Dim il As New List(Of String)

    Sub New(ByVal name As String)
        If FileSystem.FileExists(name & ".as") = False Then
            File.Create(name & ".as").Close()
        Else
            il.AddRange(File.ReadAllLines(name & ".as"))
        End If
        _name = name & ".as"
    End Sub

    Function GetKey(ByVal name As String) As Object
        For Each x In il
            If x.Split(CChar("╝")).ElementAt(0).ToLower = name.ToLower Then
                Return x.Split(CChar("╝")).ElementAt(1)
            End If
        Next
        Return Nothing
    End Function

    Function GetKeyList() As String()
        Dim illist As New List(Of String)
        For Each x In il
            If x.Contains(CChar("╝")) Then illist.Add(x.Split(CChar("╝")).ElementAt(0))
        Next
        Return illist.toarray
    End Function

    ''' <summary>
    ''' Update an existing key or add a new key.
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="value"></param>
    ''' <remarks></remarks>
    ''' 
    Sub AddKey(ByVal name As String, ByVal value As String)
        Dim copy As New List(Of String)
        copy.AddRange(il.ToArray)
        For Each x In copy
            If x.Split(CChar("╝")).ElementAt(0).ToLower = name.ToLower Then
                il.Item(copy.IndexOf(x)) = name.ToLower & "╝" & value
                Exit Sub
            End If
        Next
        copy.Clear()
        il.Add(name & "╝" & value)
    End Sub

    Sub Save()
        File.WriteAllLines(_name, il.ToArray)
    End Sub

    Function KeyExist(ByVal name As String) As Boolean
        Dim b As Boolean = False
        For Each x In il
            If x.Split(CChar("╝")).ElementAt(0).ToLower = name.ToLower Then b = True
        Next
        Return b
    End Function

End Class
