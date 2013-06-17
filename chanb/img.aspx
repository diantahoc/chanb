<%@ Import Namespace = "chanb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Page Language="VB" %>
<%
    ' img.aspx
    ' usage: 
    ' 1 - img.aspx?md5= image md5 , return image data with real image name.
    ' 2 - img.aspx?cn= chanb name & rn = real name, return image data with the specified real name.
       
    If Request.Item("md5") = "" Then
    
        If Request.Item("cn") = "" Or Request.Item("rn") = "" Then ' cn is 'chanb name', rn is 'real name'
            
            Response.StatusCode = 404
            Response.Write("404")
            Response.End()
        Else
            
            Dim ChanbName As String = Request.Item("cn")
            Dim RealName As String = Request.Item("rn")
            
            If FileIO.FileSystem.FileExists(StorageFolder & "\" & ChanbName) Then
                Response.Clear()
                Response.ClearContent()
                Response.ClearHeaders()
                Response.BufferOutput = True
                
                Dim f As IO.FileInfo = FileIO.FileSystem.GetFileInfo(StorageFolder & "\" & ChanbName)
     
                Dim fileextension As String = ChanbName.Split(CChar(".")).ElementAt(ChanbName.Split(CChar(".")).Length - 1).ToUpper
                
                Select Case fileextension
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
                    Case "WEBM"
                        Response.ContentType = "video/webm"
                    Case Else
                        Response.ContentType = "application/octet-stream"
                End Select
                
                Response.AppendHeader("Content-Disposition", "attachment; filename=""" & RealName & """")
            
                Response.AppendHeader("Content-Length", f.Length)
                
                
                Response.WriteFile(StorageFolder & "\" & ChanbName)
                Response.End()
                
            Else
                
                Response.StatusCode = 404
                Response.Write("404")
                Response.End()
            End If
            
        End If
        
        
    Else
 
        Dim md5 As String = Request.Item("md5")
        If ImageExist(md5) Then
                        
            Dim ImageData As WPostImage = GetImageDataByMD5(md5)
            
            If FileIO.FileSystem.FileExists(StorageFolder & "\" & ImageData.ChanbName) Then
                Response.Clear()
                Response.ClearContent()
                Response.ClearHeaders()
                Response.BufferOutput = True
                
                Dim f As IO.FileInfo = FileIO.FileSystem.GetFileInfo(StorageFolder & "\" & ImageData.ChanbName)


     
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
                    Case "WEBM"
                        Response.ContentType = "video/webm"
                    Case Else
                        Response.ContentType = "application/octet-stream"
                End Select
                
                Response.AppendHeader("Content-Disposition", "inline; filename=""" & ImageData.RealName & """")
            
                Response.AppendHeader("Content-Length", f.Length)

                Response.WriteFile(StorageFolder & "\" & ImageData.ChanbName)
            
                Response.End()
            Else
                Response.StatusCode = 404
                Response.Write("404")
                Response.End()
            End If
         
        Else
            Response.StatusCode = 404
            Response.Write("404")
            Response.End()
        End If
    End If
    
    
 %>