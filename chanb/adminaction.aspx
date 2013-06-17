﻿<%@ Import Namespace="chanb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Import Namespace = "chanb.Language" %>
<%@ Page Language="VB" Strict="true" %>

<%
    If Not isInstalled Then
        
        InstallActions.ProcessCommand(Request, Response)
        
    Else ' Process normal admin actions, since chanb is marked as installed.
        
        If CBool(Session("admin")) = False Then
            Response.Write(FormatHTMLMessage(ForbiddenStr, "Admin privelige is required.", "", "8888", True))
        Else
            Select Case Request.Item("action")
                Case "updatesettings"
                    Try
                        Dim stmanager As New DataInitializer()
                        stmanager.UpdateSetting(Request.Item("stname"), Request.Item("stvalue"))
                        Response.Write("OK")
                    Catch ex As Exception
                        Response.Write(ex.Message)
                    End Try
                Case "newmod"
                    Try
                        NewMod(Request.Item("newmodname"), Request.Item("newmodpass"), Request.Item("newmodpowers"))
                        Response.Write("OK")
                    Catch ex As Exception
                        Response.Write(ex.Message)
                    End Try
                Case "deletemod"
                Case "updatemod"
                Case "changepass"
                Case "permadelpost"
                Case "stickythread"
                Case "lockthread"
                Case "editpost"
                Case "editimage"
            End Select
        End If
    End If
    
    
   
    
%>