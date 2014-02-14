<%@ Page Language="vb"  AutoEventWireup="false" CodeBehind="panel.aspx.vb" Inherits="chanb._panel"  %>
<html>
<head>
<base target="_blank">
<meta charset="utf-8">
<title>Channel Board Panel</title>
<link href="css/panel/normalize.css" type="text/css" rel="stylesheet" media="screen">
<link href="css/panel/styles.css" type="text/css" rel="stylesheet" media="screen">
<script src="<% Response.Write(wb) %>dscripts.aspx"></script>
<script src="<% Response.Write(wb) %>js/jquery.min.js"></script>
<script>
    var reportOffsetStart = 0;
    var reportOffsetEnd = 10;
    
    
    function switcht(name) {
        var k = document.getElementsByClassName("mainContent");
        for (i = 0; i < k.length; i++) {
            k[i].setAttribute("style", "display: none;");
        }
        document.getElementById(name).setAttribute("style", "display: block;");
        //hide the activelink
        document.getElementsByClassName("active")[0].setAttribute("class", "");
        document.getElementById(name+"A").setAttribute("class", "active");
    }

    function reportsPrev() {
        reportOffsetEnd -= 10;
        reportOffsetStart -= 10;
        if (reportOffsetStart < 0) { reportOffsetStart = 0 }
        if (reportOffsetEnd < 0) { reportOffsetStart = 0; reportOffsetEnd = 10 }
        updateReportBox();
    }

    function reportsNext() {
        reportOffsetEnd += 10;
        reportOffsetStart += 10;
        if (reportOffsetStart < 0) { reportOffsetStart = 0 }
        if (reportOffsetEnd < 0) {reportOffsetStart = 0; reportOffsetEnd = 10 }
        
        updateReportBox();
    }

    function updateReportBox() {

        $.get(webroot + "adminaction.aspx", {
            action: "getreportoffset",
            st: reportOffsetStart,
            ed: reportOffsetEnd
        }, function(data) {
            var table = document.getElementById("reportsdata");



            //let's clear the table
            var rol = table.rows.length;
            for (i = 0; i < rol; i++) {
                table.deleteRow(i);
            }


            for (i = 0; i < data.length; i++) {

                var r = document.createElement("tr");
                var it = "<tr><td>%ID%</td><td>%DATE%</td><td>%REPORTERIP%</td><td>%POST%</td><td>%COMMENT%</td></tr>";
                it = repl(it, "ID", data[i].ReportID);
                it = repl(it, "DATE", fts(data[i].Time));
                it = repl(it, "REPORTERIP", data[i].ReporterIP);
                it = repl(it, "POST", data[i].PostID);
                it = repl(it, "COMMENT", data[i].ReportComment);


                r.innerHTML = it;

                table.appendChild(r);

            }

        }, "json");

    }
    
    function fts(d) { if (!(d == null)) { return d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + (d.getDate() + 1) + " " + d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds() } else { return "" } }

    function repl(a, b, c) { var d = new RegExp("%" + b + "%", "g"); return a.replace(d, c); }

</script>
<style>
.mainContent{
text-align: center;
}
</style>
</head>
<body>
<noscript>please enable javascript</noscript>
<div class="persistentHeader">
	<div class="headerContainer">
		<div class="navAndBlurb">
			<ul class="nav">
				<li class="nav-category"><a id="homeA" href="#" target="_self" onclick="switcht('home')" class="active">Home</a></li>
				<li class="nav-separator"></li>
				<li class="nav-category"><a id="configA" href="#" target="_self" onclick="switcht('config')" class="">Configuration</a></li>
                <li class="nav-separator"></li>
				<li class="nav-category"><a id="reportsA" href="#" target="_self" onclick="switcht('reports')" class="">Reports</a></li>
			</ul>
			<div class="blurb">
				<span>Channel Board administration panel</span><br/>
				<a href="https://github.com/diantahoc/chanb">GitHub Page</a>
			</div>
		</div>
	</div>
</div>
<div class="outerContainer">
	<div class="sidebar">
		<div class="statsBox">
			<div class="statsBoxSection">
				<div class="statsBoxSection-header">
					<img class="icon" src="res/panel/icon-vitalStats.png" height="16" width="28">Statistics
				</div>
				<div class="statsBoxSection-content">
					<ul id="vital_stats">
						<li>version<span class="statsBoxSection-value"><% Response.Write(ChanbVersion)%></span></li>
						<li>total posts<span class="statsBoxSection-value"><% Response.Write(TotalPostCount)%></span></li>
						<li>total users<span class="statsBoxSection-value"><% Response.Write(TotalUsers)%></span></li>
						<li>total files<span class="statsBoxSection-value"><% Response.Write(TotalFiles)%></span></li>
					</ul>
				</div>
			</div>
		</div>
	</div>
	<div style="display: block;" id="home" class="mainContent">
          <div style="display: inline-block">
          home
          </div>    
	</div>

	<div style="display: none;" id="config" class="mainContent">
          <div style="display: inline-block">
          config
          </div>  
	</div>

    <div style="display: none;" id="reports" class="mainContent">
          <div style="display: inline-block">
         <% Response.Write(reportBoxContent)%>
         <br />
         <div align="right">
         <a onclick="reportsPrev()" ><<</a>
         <a onclick="reportsNext()" >>></a>
         
         </div>  
	</div>

</div>
<div style="display: none;" class="loading">
	<img src="res/panel/loading.png">
</div>
</body>
</html>