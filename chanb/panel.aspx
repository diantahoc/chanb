<%@ Import Namespace = "chanb" %>
<%@ Page Language="vb" %>
<%  
    Session("chanb") = "chanb"
    PanelModule.InitPanel(Request, Session, Response)
%>