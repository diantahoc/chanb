<%@ Import Namespace = "chanb" %>
<%@ Page Language="vb" %>
<%  
    Session("chanb") = "chanb"
    'PanelModule.InitPanel(Request, Session, Response)
%>

<html>
<head>
<base target="_blank">
<meta charset="utf-8">
<title>Channel Board Panel</title>
<link href="css/panel/normalize.css" type="text/css" rel="stylesheet" media="screen">
<link href="css/panel/styles.css" type="text/css" rel="stylesheet" media="screen">
<script>
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
</script>
</head>
<body>
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
						<li>version<span class="statsBoxSection-value"><% Response.Write(GlobalFunctions.GetChanbVersion)%></span></li>
						<li>total posts<span class="statsBoxSection-value"><% Response.Write(GlobalFunctions.CountTotalPost)%></span></li>
						<li>total users<span class="statsBoxSection-value"><% Response.Write(GlobalFunctions.CountTotalUsers)%></span></li>
						<li>total files<span class="statsBoxSection-value"><% Response.Write(GlobalFunctions.CountTotalFiles)%></span></li>
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
          reports
          </div>  
	</div>

</div>
<div style="display: none;" class="loading">
	<img src="res/panel/loading.png">
</div>
</body>
</html>