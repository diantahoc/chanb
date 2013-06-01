<%  Session("chanb") = "chanb"
    Response.Write(chanb.GenerateCatalogPage(Request, Session))%>