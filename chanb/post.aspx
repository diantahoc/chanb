<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ validateRequest=false %>
<%  Response.Write(ProcessPost(Request, Session))%>