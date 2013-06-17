<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@  Page Language="VB" validateRequest=false %>
<%  Session("chanb") = "chanb"
    Response.Write(ProcessPost(Request, Session))
%>