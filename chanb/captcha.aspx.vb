Imports System.Drawing
Imports System.Drawing.Text

Partial Public Class _captcha
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not EnableCaptcha Then
            Response.StatusCode = 404
            Response.Write("captcha is disabled")
            Response.End()
        End If

        Dim isAdmin As Boolean = False
        If Not GetCookie(Context, "admin") = "" Then isAdmin = CBool(GetCookie(Context, "admin"))

        Dim r As New Random()

        Dim captchaString As String = MD5(Session.SessionID & CStr(r.Next(1, 1000)))

        Dim captchaImageSize As New Size(150, 30)
        Dim fontSize As Integer = captchaImageSize.Height
        Dim maximumChars As Integer = CInt((captchaImageSize.Width / fontSize))

        Dim cl As Integer = CaptchaLevel

        If isAdmin Then
            Dim customLevel As Integer = -1
            Try
                customLevel = CInt(Request.Item("l"))
            Catch ex As Exception
                customLevel = -1
            End Try

            If Not customLevel <= 0 Then
                cl = customLevel
            End If
        End If

        'Declare common stuffs
        Dim bi As New Bitmap(captchaImageSize.Width, captchaImageSize.Height)

        Dim g As Graphics = Graphics.FromImage(bi)

        g.Clear(Color.White)

        g.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias

        Dim f As New Font(FontFamily.GenericMonospace, fontSize, FontStyle.Regular, GraphicsUnit.Pixel)

        captchaString = New String(CType(captchaString, Char()), 0, maximumChars + 1)

        SetCookie(Context, "captcha", captchaString)

        Dim mem As New IO.MemoryStream()

        Select Case cl
            Case 1

                Dim stepping As Integer = CInt(Fix(bi.Width / captchaString.Length))
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

                bi.Save(mem, Imaging.ImageFormat.Gif)

            Case 2
                'the same as case = 1, but add black spots.        

                Dim stepping As Integer = CInt(Fix(bi.Width / captchaString.Length))
                Dim x As Integer = 0
                Dim h As Boolean = False
                For Each c In captchaString
                    If h Then
                        g.RotateTransform(r.Next(0, 3))




                    Else
                        g.RotateTransform(-r.Next(0, 3))
                    End If
                    g.SetClip(New RectangleF(x, 0, stepping, bi.Height))

                    'Black spots
                    If h Then
                        g.Clear(Color.Black)
                        g.DrawString(c, f, Brushes.White, x, 0 + r.Next(0, 5))
                    Else
                        g.DrawString(c, f, Brushes.Black, x, 0 + r.Next(0, 5))
                    End If



                    g.ResetClip()
                    x += stepping
                    h = Not h
                Next

                'h = False
                'For x = 0 To bi.Width - 1 Step CInt(Fix(bi.Width / 10))
                '    If h Then
                '        For i As Integer = x To CInt(x + Fix(bi.Width / 10) - 1) Step 1

                '            For y As Integer = 0 To bi.Height - 1 Step 1

                '                bi.SetPixel(i, y, Color.FromArgb(Not bi.GetPixel(i, y).R, Not bi.GetPixel(i, y).G, Not bi.GetPixel(i, y).B))

                '            Next


                '        Next
                '    End If
                '    h = Not h
                'Next


                bi.Save(mem, Imaging.ImageFormat.Gif)
            Case 3 ' Same as 2 with interlaced spots

                Dim stepping As Integer = CInt(Fix(bi.Width / captchaString.Length))
                Dim x As Integer = 0
                Dim h As Boolean = False
                For Each c In captchaString
                    If h Then
                        g.RotateTransform(r.Next(0, 3))
                    Else
                        g.RotateTransform(-r.Next(0, 3))
                    End If
                    g.SetClip(New RectangleF(x, 0, stepping, bi.Height))
                    If h Then
                        g.Clear(Color.Black)
                        g.DrawString(c, f, Brushes.White, x, 0 + r.Next(0, 5))
                    Else
                        g.DrawString(c, f, Brushes.Black, x, 0 + r.Next(0, 5))
                    End If
                    ' g.DrawString(c, f, Brushes.Black, x, 0 + r.Next(0, 5))
                    g.ResetClip()
                    x += stepping
                    h = Not h
                Next

                h = False

                'For x = 0 To bi.Width - 1 Step CInt(Fix(bi.Width / 10))
                '    If h Then
                '        For i As Integer = x To CInt(x + Fix(bi.Width / 10) - 1) Step 1
                '            For y As Integer = 0 To bi.Height - 1 Step 1
                '                bi.SetPixel(i, y, Color.FromArgb(Not bi.GetPixel(i, y).R, Not bi.GetPixel(i, y).G, Not bi.GetPixel(i, y).B))
                '            Next
                '        Next
                '    End If
                '    h = Not h
                'Next

                For x = 0 To bi.Height - 1 Step CInt(Fix(bi.Height / 2))
                    If h Then
                        For i As Integer = x To CInt(x + Fix(bi.Height / 2) - 1) Step 1
                            For y As Integer = 0 To bi.Width - 1 Step 1
                                bi.SetPixel(y, i, Color.FromArgb(Not bi.GetPixel(y, i).R, Not bi.GetPixel(y, i).G, Not bi.GetPixel(y, i).B))
                            Next
                        Next
                    End If
                    h = Not h
                Next

                Dim pe As New Drawing.Pen(Color.GreenYellow, r.Next(1, 3))
                For i As Integer = 0 To bi.Height - 1 Step 5

                    g.RotateTransform(r.Next(0, 2))
                    g.DrawLine(pe, 0, i, bi.Width - 1, i)

                Next
                pe.Dispose()
                bi.Save(mem, Imaging.ImageFormat.Gif)

            Case 4

                Dim bgimage As Drawing.Image

                Dim filelist As IO.FileInfo() = FileIO.FileSystem.GetDirectoryInfo(StorageFolder).GetFiles("*.jpg")

                If filelist.Length > 1 Then
                    bgimage = Drawing.Image.FromFile(filelist.ElementAt(r.Next(0, filelist.Length - 1)).FullName)
                Else
                    bgimage = New Bitmap(150, 150)
                    Dim gb As Graphics = Graphics.FromImage(bgimage)
                    gb.Clear(Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)))
                    gb.Dispose()
                End If

                g.DrawImageUnscaled(bgimage, 0, 0)
                bgimage.Dispose()

                Dim stepping As Integer = CInt(Fix(bi.Width / captchaString.Length))
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
                For x = 0 To bi.Width - 1 Step CInt(Fix(bi.Width / 10))
                    If h Then
                        For i As Integer = x To CInt(x + Fix(bi.Width / 10) - 1) Step 1

                            For y As Integer = 0 To bi.Height - 1 Step 1
                                bi.SetPixel(i, y, Color.FromArgb(Not bi.GetPixel(i, y).R, Not bi.GetPixel(i, y).G, Not bi.GetPixel(i, y).B))
                            Next
                        Next
                    End If
                    h = Not h
                Next

                For x = 0 To bi.Height - 1 Step CInt(Fix(bi.Height / 2))
                    If h Then
                        For i As Integer = x To CInt(x + Fix(bi.Height / 2) - 1) Step 1
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



                bi.Save(mem, Imaging.ImageFormat.Gif)

            Case 5


                For i As Integer = 0 To bi.Height - 1 Step 5
                    Dim pe As New Drawing.Pen(Color.Black, r.Next(1, 3))
                    g.RotateTransform(r.Next(0, 1))
                    g.DrawLine(pe, 0, i, bi.Width - 1, i)
                    pe.Dispose()
                Next


                'Proper text drawing method.
                Dim stepping As Integer = CInt(Fix(bi.Width / captchaString.Length))
                Dim x As Integer = 0
                Dim h As Boolean = False

                For Each c In captchaString
                    g.SetClip(New RectangleF(x, 0, stepping, bi.Height))
                    If h Then
                        g.RotateTransform(r.Next(0, 3))
                    Else
                        g.RotateTransform(-r.Next(0, 3))
                    End If
                    g.DrawString(c, f, Brushes.Black, x, 0 + r.Next(0, 5))
                    g.ResetClip()
                    x += stepping
                    h = Not h
                Next




                bi.Save(mem, Imaging.ImageFormat.Gif)

            Case Else
                Response.StatusCode = 404
                Response.Write("404")
                Response.End()
        End Select

        bi.Dispose()
        g.Dispose()

        Response.BufferOutput = True
        Response.Expires = 0

        Response.ContentType = "image/gif"

        Response.OutputStream.Write(mem.GetBuffer, 0, CInt(mem.Length))


        Response.End()

    End Sub

End Class