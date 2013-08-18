<%@ Import Namespace = "chanb" %>
<%@ Import Namespace = "chanb.GlobalFunctions" %>
<%@ Import Namespace = "chanb.GlobalVariables" %>
<%@ Page Language="VB" %>
<%  
    If isInstalled Then
        Response.StatusCode = 403
        Response.Write("Forbidden")
        Response.End()
    Else
        Session("admin") = CStr(True)
    End If
    
    %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Chanb Installer</title>
    <link rel="Stylesheet" href='yotsubab.css' />
    <script src="scripts.js" type="text/javascript" language="javascript"></script>
    <script type="text/javascript" src="js/jquery.min.js"></script>
    <script type="text/javascript" src="js/jquery-ui.min.js"></script>
</head>
<body>
<form action="adminaction.aspx" method="post" enctype="multipart/form-data">
<div align="center">

         <h5>
           <span style="text-align: center ">Thank you for choosing channel board software.</span>
           <br />
           <span style="text-align :center "><a href="https://github.com/diantahoc/chanb" target="_blank">Channel board</a> is free software, meaning you can modify it as you like.</span>
           <br />
           <span style="text-align :center ">The full legal terms can be found <a href="http://www.gnu.org/licenses/old-licenses/gpl-2.0.html" target="_blank">here</a></span>         
         </h5>
<br />
<br />



<div class='whitebox'>      
           <h2 style="text-align: center !important;">
           <span style=" text-align: center !important; ">Configure database</span>
           <br />
           
           </h2>
           
           <span>- Enter database server type:</span> 
           
           <select id="dbType" name="db" onchange="if (document.getElementById('dbType').value == 'mssql') { $('#mssqlExample').removeClass('hide'); $('#mysqlExample').addClass('hide'); } else {$('#mssqlExample').addClass('hide'); $('#mysqlExample').removeClass('hide'); };updateAttrb('testdbConnection','href','adminaction.aspx?action=testdbconnection&db='+  document.getElementById('dbType').value  + '&dbconnectionstring=' + document.getElementById('dbconnectionstring').value )">
           
           <option value="mssql">MS SQL</option>
           
           <option value="mysql">MySQL</option>
           
           </select>
           
           <br />
           <span>- Enter database connection string:</span> 
           
           <input size="30px" id="dbconnectionstring" type="text" name="dbconnectionstring" onchange="updateAttrb('testdbConnection','href','adminaction.aspx?action=testdbconnection&db='+  document.getElementById('dbType').value  + '&dbconnectionstring=' + document.getElementById('dbconnectionstring').value )" class="form-text" />
           
           <a target="_blank" id='testdbConnection' href="#" >Test the connection</a>
           
           <br />
           
           <div id='mssqlExample'>
           <span>- An MSSQL connection string should look like</span> 
           <br />
           <blockquote style="background: #EEEEEE; max-width: 600px;">
           Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;
           </blockquote>
           <span>Or</span>
           <blockquote style="background: #EEEEEE; max-width: 600px;">
           Data Source=Server IP Adresse,Server Port;Network Library=DBMSSOCN;Initial Catalog=myDataBase;User ID=myUsername;Password=myPassword;
           </blockquote>
           <a href='http://www.connectionstrings.com/sql-server-2008' target='_blank'>More info here</a>
           </div>
           
           <div id='mysqlExample'>
           <span>- A MySQL connection string should look like</span> 
           <br />
           <blockquote style="background: #EEEEEE; max-width: 600px;">
           Server=myServerAddress;Port=3306;Database=myDataBase;Uid=myUsername;Pwd=myPassword;
           </blockquote>
           <a href='http://www.connectionstrings.com/mysql' target='_blank'>More info here</a>
           </div>
                  
           <script>$('#mssqlExample').removeClass('hide'); $('#mysqlExample').addClass('hide');</script>
</div> 
<br />
<br />

<div class='whitebox'>           
            <h2 style="text-align: center !important;">
           <span style="text-align: center !important;">Configure database structure</span>
           <br />
           </h2>
           

           <span>- Click this button to <input type="submit" name="action" value="automatically configure the database structure"/></span>
         
           <br />
           <br />
           
           <span>- Or upload your script.</span> <input name="customdbfile" type="file" class="form-text" /> <input type="submit" name="action" value="Upload and run script"/>
            
</div>

<br />
<br />



<div class='whitebox'>  
            <h2 style="text-align: center !important;">
           <span style="text-align: center !important; ">Server configuration</span>
           <br />
           </h2>     
           <span style="font-size:medium !important;">(Note: configuring this section is optional.)</span>
           
           <br />
           <br />
           
           <span>- Enter the physical path for file storage:</span> 
  
           <input value="<% Response.Write(StorageFolder) %>" type="text" name="StorageFolder" class="form-text" size="30px" />
           
           <br />
           <span style="font-size:smaller !important;">(Note: The storage folder should be accessible from the web)</span> 
         
           <br />
           <br />
         
           <span>- Enter the web path for file storage:</span> 
            
           <input value="<% Response.Write(StorageFolderWeb) %>" type="text" name="StorageFolderWeb" size="30px" class="form-text" />
           
           <br />
           <span style="font-size:smaller !important;">(Note: Enter the full web path, not a relative link.)</span> 
         
</div>

<br />
<br />



<div class='whitebox'>  
           
            <h2 style="text-align: center !important;">
           <span style="text-align: center !important; ">Board configuration</span>
           <br />
           </h2>     
           <span style="font-size:medium !important;">(Note: configuring this section is also optional, but it's recommended to edit the configurations.)</span>
           
           <br />
           <br />
           
           <span>- Board Title:</span>
           <input value="<% Response.Write(BoardTitle) %>" type="text" name="BoardTitle" size="30px" class="form-text" /> 
           <br /><br />
           
           <span>- Board description:</span>
           <input value="<% Response.Write(BoardDesc) %>" type="text" name="BoardDesc" size="30px" class="form-text" /> 
           <br /><br />
           
           <span>- Time between post requests:</span>
           <input value="<% Response.Write(TimeBetweenRequestes) %>" type="text" name="TimeBetweenRequestes" size="5px" class="form-text" /> <span> in seconds.</span>
           <br /><span style="font-size:smaller !important;">(for flood detection, 0 to disable)</span>
           <br /><br />
           
           <span>- Maximum file size:</span>
           <input value="<% Response.Write(MaximumFileSize / 1024) %>" type="text" name="MaximumFileSize" size="10px" class="form-text" /> <span> in KB.</span>
           <br /><span style="font-size:smaller !important;">(1 MB = 1024 KB)</span>
           <br /><br />
           
           <span>- Thread count per page:</span>
           <input value="<% Response.Write(ThreadPerPage) %>" type="text" name="ThreadPerPage" size="5px" class="form-text" />
           <br /><br />
           
           <span>- Maximum pages count:</span>
           <input value="<% Response.Write(MaximumPages) %>" type="text" name="MaximumPages" size="5px" class="form-text" />
           <br /><br />
           
           <span>- Trail posts count:</span>
           <input value="<% Response.Write(TrailPosts) %>" type="text" name="TrailPosts" size="5px" class="form-text" />
           <br /><br />
           
           <span>- Bump limit:</span>
           <input value="<% Response.Write(BumpLimit) %>" type="text" name="BumpLimit" size="5px" class="form-text" />
           <br /><br />
           
           <span>- Enable user ID:</span>
           <select name="EnableUserID">
           <option>True</option>
           <option>False</option>
           </select> 
           <br /><br />

           <span>- Enable integrated archive:</span>
           <select name="EnableArchive">
           <option>True</option>
           <option>False</option>
           </select> 
           <br /><br />
           
           <span>- Enable Captcha:</span>
           <select name="EnableCaptcha">
           <option>True</option>
           <option>False</option>
           </select> 
           <br /><br />
           
           <span>- Captcha difficultly level:</span>
           <select id='CaptchaLevel' name="CaptchaLevel" onchange="refreshcaptcha(document.getElementById('CaptchaLevel').value)">
           <option value="1">Easy</option>
           <option value="2">Normal</option>
           <option value="5">Hard</option>
           <option value="3">Complex</option>
           <option value="4">Super Complex</option>
           </select>
           <span>Preview:</span><img alt='captcha preview' id='captchaImage' src='captcha.aspx?l=1' /> 
           <br /><br />
               
           <span>- Automatically delete files:</span>
           <select name="DeleteFiles">
           <option>True</option>
           <option>False</option>
           </select> 
           <br /><br />
           
           <span>- Allow duplicates files:</span>
           <select id="AllowDuplicatesFiles" name="AllowDuplicatesFiles" onchange="if (document.getElementById('AllowDuplicatesFiles').value == 'True' ) { document.getElementById('SmartLinkDuplicateImages').removeAttribute('disabled') } else { updateAttrb('SmartLinkDuplicateImages','disabled','disabled') } ">
           <option>True</option>
           <option>False</option>
           </select> 
           <br /><br />
            
           <span>- Link duplicate images to the same file:</span>
           <select id='SmartLinkDuplicateImages' name="SmartLinkDuplicateImages">
           <option>True</option>
           <option>False</option>
           </select>
           <br /><span style="font-size:smaller !important;">(avoid storing duplicate file)</span>
           <br /><br />
            
</div>

<br />
<br />



<div class='whitebox'>  
  <h2 style="text-align: center !important;">
           <span style="text-align: center !important; ">Administration credentials</span>
           <br />
           </h2>             
           <span>- Admin user name:</span>
           <input type="text" name="adminname" size="30px" class="form-text" /> 
           <br /><br />
           
           <span>- Admin password:</span>
           <input type="text" name="adminpass" size="30px" class="form-text" /> 
           <br /><br />
           
</div>

<br />
<br />


<input type="submit" value="Install" name="action" />
</div>
    </form>
</body>
</html>
