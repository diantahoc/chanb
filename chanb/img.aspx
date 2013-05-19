<%@ Import Namespace = "chanb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>

<%

    If Request.Item("md5") = "" Then
        Response.StatusCode = 404
        Response.Write("Not found")
        Response.End()
    Else
 
        Dim md5 As String = Request.Item("md5")
        If ImageExist(md5) Then
                        
            Dim ImageData As WPostImage = GetImageDataByMD5(md5)
            
            If FileIO.FileSystem.FileExists(STORAGEFOLDER & "\" & ImageData.ChanbName) Then
                Response.Clear()
                Response.ClearContent()
                Response.ClearHeaders()
                Response.BufferOutput = True
                
                Dim f As IO.FileInfo = FileIO.FileSystem.GetFileInfo(STORAGEFOLDER & "\" & ImageData.ChanbName)

                Response.AppendHeader("Content-Disposition", "inline; filename=" & ImageData.RealName)
            
                Response.AddHeader("Content-Length", f.Length)
     
                Select Case ImageData.Extension
                    Case "JPG"
                        Response.ContentType = "image/jpeg"
                    Case "JPEG"
                        Response.ContentType = "image/jpeg"
                    Case "BMP"
                        Response.ContentType = "image/bmp"
                    Case "PNG"
                        Response.ContentType = "image/png"
                    Case "SVG"
                        Response.ContentType = "image/svg+xml"
                    Case "PDF"
                        Response.ContentType = "application/acrobat"
                    Case Else
                        Response.ContentType = "application/octet-stream"
                End Select
            

                Response.WriteFile(STORAGEFOLDER & "\" & ImageData.ChanbName)
            
                Response.End()
            Else
                Response.StatusCode = 404
                Response.Write("Not found")
                Response.End()
            End If
         
        Else
            Response.StatusCode = 404
            Response.Write("Not found")
            Response.End()
        End If
    End If
    
    
 %>