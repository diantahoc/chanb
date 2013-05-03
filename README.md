chanb
=====

chanb is an ASP.NET Image board written in visual basic. Licensed legally under the GPLv2

Live preview
=============

http://www.chanb.somee.com

The website gets cleaned each while.

Features
========

* Multiple files per post 
* PDF Support (not yet)
* 4chan-like interface
* Catalog (not yet)
* Multilanguage support (not complete)

Yet there will be more of course.

How to install
==============

Open the project in visual studio, and open the GlobalFunctions file. Change the 'SQLConnectionString' variable to your
server connection string. After that, open the GlobalVariables file, and adjust it. Instructions are commented inside of it. 
When you have finished adjusting the code, compile it, and upload all .aspx files to your application root. Upload the bin 
directory in the same directory as the .aspx files. Make sure you also make a folder to store files, and make that folder 
accessible from the web. Don't forget to upload the templates files.
