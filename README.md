Chanb
=====

Chanb (Channel Board) is an ASP.NET Image board written in Visual Basic. 
Licensed legally under the GPLv2

Live preview
=============

http://www.chanb.somee.com

The website gets cleaned each while.

Current Features
========

* Multiple files per post .
* Automatic set dumper (with file counting support).
* SVG documents support.
* Duplicate image checking support.
* Smart Linking of duplicate images to the same real file, therefore reducing the consumed disk space.
* Poster ID support.
* PDF Support (proper thumbnail generation require TallComponent PDFRasterizer library, not enabled by default).
* 4chan-like interface, and mobile support.
* Moderator powers customization.
* Code and spolier tags support.
* Markdown support.
* Integrated archive.

There will be more of course.

Planned features
==============

* Catalog
* Multilanguage support.
* Admin panel
* Installer
* Online language adder and editor.
* Video (FLV, WEBM) and audio (MP3, FLAC)

How to install
==============

Copy the binary files to your application root, under a directory called bin. Make sure you copy the required dlls, and the templates files.

Make a "db.txt" file in the same directory as chanb.dll, write inside of it the SQL connection string.

Make sure you prepare the database with the SQL template provided. Note: The provided template drop all existing items before updating the database.

And copy all the .aspx files to your application root.

Make a "images" folder in your application root.

Currently, you can change settings by editing the data.as file which is automatically made in the same folder as chanb.dll .

Enjoy
