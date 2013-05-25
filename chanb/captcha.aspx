<%@ Import Namespace="chanb" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%
    '0 for disabled, 1 for simple, 2 for complex, 3 for super complex, 4 for complex with random background
    
    If EnableCaptcha = False Then
        Response.StatusCode = 404
        Response.Write("captcha is disabled")
        Response.End()
    End If
    
    Dim r As New Random
    
    Dim captchaString As String = MD5(Session.SessionID & CStr(r.Next(1, 1000)))
    
    Dim captchaImageSize As New Size(150, 30)
    Dim fontSize As Integer = captchaImageSize.Height
    Dim maximumChars As Integer = (captchaImageSize.Width / fontSize)
    
    Dim cl As Integer
    
    If Request.Item("l") = "" Then
        cl = CaptchaLevel
    Else
        If CBool(Session("admin")) = True Then
            Try
                cl = CInt(Request.Item("l"))
            Catch ex As Exception
                cl = CaptchaLevel
            End Try
        Else
            cl = CaptchaLevel
        End If
    End If
    
    Select Case cl
        Case 0
            Response.StatusCode = 404
            Response.Write("captcha is disabled")
            Response.End()
        Case 1
              
            
            Dim bi As New Bitmap(captchaImageSize.Width, captchaImageSize.Height)
            
            Dim g As Graphics = Graphics.FromImage(bi)
            
            g.Clear(Color.White)
            
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            
            Dim f As New Font(FontFamily.GenericMonospace, fontSize, FontStyle.Regular, GraphicsUnit.Pixel)
           
            captchaString = New String(captchaString, 0, maximumChars + 1)
            
            Session("captcha") = captchaString
            
            Dim stepping As Integer = Fix(bi.Width / captchaString.Length)
            Dim x As Integer = 0
            Dim h As Boolean = False
            For Each c In captchaString
                If h Then
                    g.RotateTransform(r.Next(0, 3))
                Else
                    g.RotateTransform(-r.Next(0, 3))
                End If
                g.SetClip(New RectangleF(x, 0, stepping, bi.Height))
                g.DrawString(c, f, Brushes.Black, x, 0 + r.Next(0, 5))
                g.ResetClip()
                x += stepping
                h = Not h
            Next
            
            Dim mem As New IO.MemoryStream
            
            bi.Save(mem, Imaging.ImageFormat.Png)
            
            bi.Dispose()
            g.Dispose()
            
            Response.Clear()
            Response.ClearContent()
            Response.ClearHeaders()
            Response.BufferOutput = True
            Response.Expires = 0
         
            Response.ContentType = "image/png"
            
            Response.OutputStream.Write(mem.GetBuffer, 0, mem.Length)
            
            Response.Flush()
            
            Response.End()
            
        Case 2
            'the same as case = 1, but add black spots.        
            
            Dim bi As New Bitmap(captchaImageSize.Width, captchaImageSize.Height)
            
            Dim g As Graphics = Graphics.FromImage(bi)
            
            g.Clear(Color.White)
            
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            
            Dim f As New Font(FontFamily.GenericMonospace, fontSize, FontStyle.Regular, GraphicsUnit.Pixel)
           
            captchaString = New String(captchaString, 0, maximumChars + 1)
            
            Session("captcha") = captchaString
            
            Dim stepping As Integer = Fix(bi.Width / captchaString.Length)
            Dim x As Integer = 0
            Dim h As Boolean = False
            For Each c In captchaString
                If h Then
                    g.RotateTransform(r.Next(0, 3))
                Else
                    g.RotateTransform(-r.Next(0, 3))
                End If
                g.SetClip(New RectangleF(x, 0, stepping, bi.Height))
                g.DrawString(c, f, Brushes.Black, x, 0 + r.Next(0, 5))
                g.ResetClip()
                x += stepping
                h = Not h
            Next
            
            h = False
            For x = 0 To bi.Width - 1 Step Fix(bi.Width / 10)
                If h Then
                    For i As Integer = x To x + Fix(bi.Width / 10) - 1 Step 1
                    
                        For y As Integer = 0 To bi.Height - 1 Step 1
                      
                            bi.SetPixel(i, y, Color.FromArgb(Not bi.GetPixel(i, y).R, Not bi.GetPixel(i, y).G, Not bi.GetPixel(i, y).B))
                     
                        Next
                    
                    
                    Next
                End If
                h = Not h
            Next
                       
            Dim mem As New IO.MemoryStream
            
            bi.Save(mem, Imaging.ImageFormat.Png)
            
            bi.Dispose()
            g.Dispose()
            
            Response.Clear()
            Response.ClearContent()
            Response.ClearHeaders()
            Response.BufferOutput = True
            Response.Expires = 0
         
            Response.ContentType = "image/png"
            
            Response.OutputStream.Write(mem.GetBuffer, 0, mem.Length)
            
            Response.Flush()
            
            Response.End()
            
        Case 3
            
            Dim bi As New Bitmap(captchaImageSize.Width, captchaImageSize.Height)
            
            Dim g As Graphics = Graphics.FromImage(bi)
            
            g.Clear(Color.White)
            
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            
            Dim f As New Font(FontFamily.GenericMonospace, fontSize, FontStyle.Regular, GraphicsUnit.Pixel)
           
            captchaString = New String(captchaString, 0, maximumChars + 1)
            
            Session("captcha") = captchaString
            
            Dim stepping As Integer = Fix(bi.Width / captchaString.Length)
            Dim x As Integer = 0
            Dim h As Boolean = False
            For Each c In captchaString
                If h Then
                    g.RotateTransform(r.Next(0, 3))
                Else
                    g.RotateTransform(-r.Next(0, 3))
                End If
                g.SetClip(New RectangleF(x, 0, stepping, bi.Height))
                g.DrawString(c, f, Brushes.Black, x, 0 + r.Next(0, 5))
                g.ResetClip()
                x += stepping
                h = Not h
            Next
            
            h = False
            For x = 0 To bi.Width - 1 Step Fix(bi.Width / 10)
                If h Then
                    For i As Integer = x To x + Fix(bi.Width / 10) - 1 Step 1
                    
                        For y As Integer = 0 To bi.Height - 1 Step 1
                      
                            bi.SetPixel(i, y, Color.FromArgb(Not bi.GetPixel(i, y).R, Not bi.GetPixel(i, y).G, Not bi.GetPixel(i, y).B))
                     
                        Next
                    
                    
                    Next
                End If
                h = Not h
            Next
            
            For x = 0 To bi.Height - 1 Step Fix(bi.Height / 2)
                If h Then
                    For i As Integer = x To x + Fix(bi.Height / 2) - 1 Step 1

                        For y As Integer = 0 To bi.Width - 1 Step 1

                            bi.SetPixel(y, i, Color.FromArgb(Not bi.GetPixel(y, i).R, Not bi.GetPixel(y, i).G, Not bi.GetPixel(y, i).B))

                        Next


                    Next
                End If
                h = Not h
            Next
              
            For i As Integer = 0 To bi.Height - 1 Step 5
                Dim pe As New Drawing.Pen(Color.Black, r.Next(1, 3))
                g.RotateTransform(r.Next(0, 1))
                g.DrawLine(pe, 0, i, bi.Width - 1, i)
                pe.Dispose()
            Next
            
            Dim mem As New IO.MemoryStream
            
            bi.Save(mem, Imaging.ImageFormat.Png)
            
            bi.Dispose()
            g.Dispose()
            
            Response.Clear()
            Response.ClearContent()
            Response.ClearHeaders()
            Response.BufferOutput = True
            Response.Expires = 0
         
            Response.ContentType = "image/png"
            
            Response.OutputStream.Write(mem.GetBuffer, 0, mem.Length)
            
            Response.Flush()
            
            Response.End()
            
        Case 4
            
            Dim bi As New Bitmap(captchaImageSize.Width, captchaImageSize.Height)
            
            Dim bgimage As Drawing.Image
            
            Dim filelist As IO.FileInfo() = FileIO.FileSystem.GetDirectoryInfo(STORAGEFOLDER).GetFiles("*.jpg")
            
            bgimage = Drawing.Image.FromFile(filelist.ElementAt(r.Next(0, filelist.Length - 1)).FullName)
            
           
            Dim g As Graphics = Graphics.FromImage(bi)
            
           
            g.DrawImageUnscaled(bgimage, 0, 0)
            bgimage.Dispose()
            
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            
            Dim f As New Font(FontFamily.GenericMonospace, fontSize, FontStyle.Regular, GraphicsUnit.Pixel)
           
            captchaString = New String(captchaString, 0, maximumChars + 1)
            
            Session("captcha") = captchaString
          
            Dim stepping As Integer = Fix(bi.Width / captchaString.Length)
            Dim x As Integer = 0
            Dim h As Boolean = False
            For Each c In captchaString
                If h Then
                    g.RotateTransform(r.Next(0, 3))
                Else
                    g.RotateTransform(-r.Next(0, 3))
                End If
                g.SetClip(New RectangleF(x, 0, stepping, bi.Height))
                g.DrawString(c, f, Brushes.Black, x, 0 + r.Next(0, 5))
                g.ResetClip()
                x += stepping
                h = Not h
            Next
           
            ' 
            
            h = False
            For x = 0 To bi.Width - 1 Step Fix(bi.Width / 10)
                If h Then
                    For i As Integer = x To x + Fix(bi.Width / 10) - 1 Step 1
                    
                        For y As Integer = 0 To bi.Height - 1 Step 1
                            bi.SetPixel(i, y, Color.FromArgb(Not bi.GetPixel(i, y).R, Not bi.GetPixel(i, y).G, Not bi.GetPixel(i, y).B))
                        Next
                    Next
                End If
                h = Not h
            Next
            
            For x = 0 To bi.Height - 1 Step Fix(bi.Height / 2)
                If h Then
                    For i As Integer = x To x + Fix(bi.Height / 2) - 1 Step 1
                        For y As Integer = 0 To bi.Width - 1 Step 1
                            bi.SetPixel(y, i, Color.FromArgb(Not bi.GetPixel(y, i).R, Not bi.GetPixel(y, i).G, Not bi.GetPixel(y, i).B))
                        Next
                    Next
                End If
                h = Not h
            Next
            
            For i As Integer = 0 To bi.Height - 1 Step 5
                Dim pe As New Drawing.Pen(Color.Black, r.Next(1, 3))
                g.RotateTransform(r.Next(0, 1))
                g.DrawLine(pe, 0, i, bi.Width - 1, i)
                pe.Dispose()
            Next
            
            Dim mem As New IO.MemoryStream
            
            bi.Save(mem, Imaging.ImageFormat.Png)
            
            bi.Dispose()
            g.Dispose()
            
            Response.Clear()
            Response.ClearContent()
            Response.ClearHeaders()
            Response.BufferOutput = True
            Response.Expires = 0
         
            Response.ContentType = "image/png"
            
            Response.OutputStream.Write(mem.GetBuffer, 0, mem.Length)
            
            Response.Flush()
            
            Response.End()
            
        Case 5
            Dim bi As New Bitmap(captchaImageSize.Width, captchaImageSize.Height)
            
            Dim g As Graphics = Graphics.FromImage(bi)
            
            g.Clear(Color.White)
            
            g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
            
            Dim f As New Font(FontFamily.GenericMonospace, fontSize, FontStyle.Regular, GraphicsUnit.Pixel)
           
            captchaString = New String(captchaString, 0, maximumChars + 1)
            
            Session("captcha") = captchaString
            
          
            'Proper text drawing method.
            Dim stepping As Integer = Fix(bi.Width / captchaString.Length)
            Dim x As Integer = 0
            Dim h As Boolean = False
            For Each c In captchaString
                If h Then
                    g.RotateTransform(r.Next(0, 3))
                Else
                    g.RotateTransform(-r.Next(0, 3))
                End If
                g.SetClip(New RectangleF(x, 0, stepping, bi.Height))
                g.DrawString(c, f, Brushes.Black, x, 0 + r.Next(0, 5))
                g.ResetClip()
                x += stepping
                h = Not h
            Next
            
            
            For i As Integer = 0 To bi.Height - 1 Step 5
                Dim pe As New Drawing.Pen(Color.Black, r.Next(1, 3))
                g.RotateTransform(r.Next(0, 1))
                g.DrawLine(pe, 0, i, bi.Width - 1, i)
                pe.Dispose()
            Next
                     
            Dim mem As New IO.MemoryStream
            
            bi.Save(mem, Imaging.ImageFormat.Jpeg)
            
            bi.Dispose()
            g.Dispose()
            
            Response.Clear()
            Response.ClearContent()
            Response.ClearHeaders()
            Response.BufferOutput = True
            Response.Expires = 0
         
            Response.ContentType = "image/jpeg"
            Response.OutputStream.Write(mem.GetBuffer, 0, mem.Length)
            Response.Flush()
            Response.End()
        Case Else
            Response.StatusCode = 404
            Response.Write("404")
            Response.End()
    End Select
%>