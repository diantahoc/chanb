Imports System.Security.Cryptography
Imports System.IO

Friend Module Misc

    Private _salt As Byte() = Encoding.ASCII.GetBytes(GlobalFunctions.dbi.ConnectionString)

    Public Function FormatHTMLMessage(ByVal title As String, ByVal msg As String, ByVal redirectpage As String, ByVal timeout As String, ByVal critical As Boolean) As String
        Dim messageTemplate As String = GenericMessageTemplate
        If Not title.Length < 1 Then messageTemplate = messageTemplate.Replace("%MSG TITLE%", title.ElementAt(0).ToString.ToUpper() & title.Remove(0, 1).ToLower()) Else messageTemplate = messageTemplate.Replace("%MSG TITLE%", "")
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

    Public Sub SetCookie(ByRef Context As HttpContext, ByVal name As String, ByVal data As String)
        If Context.Request.Cookies(name) Is Nothing Then
            Context.Response.Cookies.Add(New HttpCookie(name, EncryptStringAES(data, name)))
        Else
            Context.Request.Cookies.Remove(name)
            Context.Response.Cookies.Add(New HttpCookie(name, EncryptStringAES(data, name)))
        End If
    End Sub

    Public Sub RemoveCookie(ByRef Context As HttpContext, ByVal name As String)
        Context.Request.Cookies.Remove(name)
    End Sub

    Public Function GetCookie(ByRef Context As HttpContext, ByVal name As String) As String
        If Context.Request.Cookies(name) Is Nothing Then
            Return ""
        Else
            Dim c As HttpCookie = Context.Request.Cookies(name)
            Return DecryptStringAES(c.Value, name)
        End If
    End Function

    ''' <summary>
    ''' Encrypt the given string using AES.  The string can be decrypted using 
    ''' DecryptStringAES().  The sharedSecret parameters must match.
    ''' </summary>
    ''' <param name="plainText">The text to encrypt.</param>
    ''' <param name="sharedSecret">A password used to generate a key for encryption.</param>
    Private Function EncryptStringAES(ByVal plainText As String, ByVal sharedSecret As String) As String
        If String.IsNullOrEmpty(plainText) Then
            Throw New ArgumentNullException("plainText")
        End If
        If String.IsNullOrEmpty(sharedSecret) Then
            Throw New ArgumentNullException("sharedSecret")
        End If

        Dim outStr As String = Nothing
        ' Encrypted string to return
        Dim aesAlg As RijndaelManaged = Nothing
        ' RijndaelManaged object used to encrypt the data.
        Try
            ' generate the key from the shared secret and the salt
            Dim key As New Rfc2898DeriveBytes(sharedSecret, _salt)

            ' Create a RijndaelManaged object
            aesAlg = New RijndaelManaged()
            aesAlg.Key = key.GetBytes(CInt(aesAlg.KeySize / 8))

            ' Create a decryptor to perform the stream transform.
            Dim encryptor As ICryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)

            ' Create the streams used for encryption.
            Using msEncrypt As New MemoryStream()
                ' prepend the IV
                msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, 4)
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length)
                Using csEncrypt As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
                    Using swEncrypt As New StreamWriter(csEncrypt)
                        'Write all data to the stream.
                        swEncrypt.Write(plainText)
                    End Using
                End Using
                outStr = Convert.ToBase64String(msEncrypt.ToArray())
            End Using
        Finally
            ' Clear the RijndaelManaged object.
            If aesAlg IsNot Nothing Then
                aesAlg.Clear()
            End If
        End Try

        ' Return the encrypted bytes from the memory stream.
        Return outStr
    End Function

    ''' <summary>
    ''' Decrypt the given string.  Assumes the string was encrypted using 
    ''' EncryptStringAES(), using an identical sharedSecret.
    ''' </summary>
    ''' <param name="cipherText">The text to decrypt.</param>
    ''' <param name="sharedSecret">A password used to generate a key for decryption.</param>
    Private Function DecryptStringAES(ByVal cipherText As String, ByVal sharedSecret As String) As String
        If String.IsNullOrEmpty(cipherText) Then
            Throw New ArgumentNullException("cipherText")
        End If
        If String.IsNullOrEmpty(sharedSecret) Then
            Throw New ArgumentNullException("sharedSecret")
        End If

        ' Declare the RijndaelManaged object
        ' used to decrypt the data.
        Dim aesAlg As RijndaelManaged = Nothing

        ' Declare the string used to hold
        ' the decrypted text.
        Dim plaintext As String = Nothing

        Try
            ' generate the key from the shared secret and the salt
            Dim key As New Rfc2898DeriveBytes(sharedSecret, _salt)

            ' Create the streams used for decryption.                
            Dim bytes As Byte() = Convert.FromBase64String(cipherText)
            Using msDecrypt As New MemoryStream(bytes)
                ' Create a RijndaelManaged object
                ' with the specified key and IV.
                aesAlg = New RijndaelManaged()
                aesAlg.Key = key.GetBytes(CInt(aesAlg.KeySize / 8))
                ' Get the initialization vector from the encrypted stream
                aesAlg.IV = ReadByteArray(msDecrypt)
                ' Create a decrytor to perform the stream transform.
                Dim decryptor As ICryptoTransform = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV)
                Using csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)
                    Using srDecrypt As New StreamReader(csDecrypt)

                        ' Read the decrypted bytes from the decrypting stream
                        ' and place them in a string.
                        plaintext = srDecrypt.ReadToEnd()
                    End Using
                End Using
            End Using
        Finally
            ' Clear the RijndaelManaged object.
            If aesAlg IsNot Nothing Then
                aesAlg.Clear()
            End If
        End Try

        Return plaintext
    End Function

    Private Function ReadByteArray(ByVal s As Stream) As Byte()
        Dim rawLength As Byte() = New Byte(4 - 1) {}
        If s.Read(rawLength, 0, rawLength.Length) <> rawLength.Length Then
            Throw New SystemException("Stream did not contain properly formatted byte array")
        End If

        Dim buffer As Byte() = New Byte(BitConverter.ToInt32(rawLength, 0) - 1) {}
        If s.Read(buffer, 0, buffer.Length) <> buffer.Length Then
            Throw New SystemException("Did not read byte array properly")
        End If

        Return buffer
    End Function

    Public Class Validators

        Public Shared Function ValidateWEBM(ByVal fs As IO.Stream) As Boolean
            Return ValidateFS(fs, "1A 45 DF A3 01")
            '00 00 00 00 00 00 1F 42 86 81 01 42 F7 81 01 42 F2 81 04 42 F3 81 08 42 82 84 77 65 62 6D 42 87 81 02 42
        End Function

        Public Shared Function ValidateOGG(ByVal fs As IO.Stream) As Boolean
            Return ValidateFS(fs, "4F 67 67 53")
        End Function

        Public Shared Function ValidateMP3(ByVal fs As IO.Stream) As Boolean
            Return ValidateFS(fs, "49 44 33")
        End Function

        Private Shared Function ValidateFS(ByVal fs As IO.Stream, ByVal sig As String) As Boolean
            Dim signature As String() = sig.Split(CChar(" "))
            Dim signLength As Integer = signature.Length

            If fs.Length < signLength Then
                Return False
            Else

                Try
                    Dim buff(signLength) As Byte

                    fs.Seek(0, SeekOrigin.Begin)

                    fs.Read(buff, 0, signLength - 1)

                    Dim boolResult As Boolean = True

                    For i As Integer = 0 To signLength - 1 Step 1
                        Dim byS As Byte = CType("&H" & signature(i), Byte)
                        If Not (byS = buff(i)) Then boolResult = False
                    Next

                    Return boolResult

                Catch ex As Exception
                    Return False
                End Try

            End If
        End Function

        ''' <summary>
        ''' Experimental, not meant to be used.
        ''' </summary>
        ''' <param name="fs"></param>
        ''' <param name="sign"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function FSContainSign(ByVal fs As IO.Stream, ByVal sign As String) As Boolean

            Dim buff(CInt(fs.Length)) As Byte

            fs.Seek(0, SeekOrigin.Begin)

            fs.Read(buff, 0, CInt(fs.Length - 1))


            Dim sb As New StringBuilder()

            For Each x In buff
                sb.Append(x.ToString("X"))
                sb.Append(" ")
            Next

            buff = Nothing

            Return sb.ToString().Contains(sign.ToUpper())
        End Function

    End Class

End Module

