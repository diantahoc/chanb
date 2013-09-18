Chanb
=====

Chanb (Channel Board) is an ASP.NET Image board written in Visual Basic.
Licensed legally under the GPLv2

Live preview
=============

http://www.chanb.somee.com

The website gets cleaned each while.

Current features
========

-Administration related:
* Installer.
* Moderator powers customization.

-Board related:
* Auto updater.
* Automatic set dumper (with file counting support).
* Captcha.
* Catalog.
* Duplicate image checking support.
* Integrated archive.
* Markdown support.
* Mobile support.
* Multilingual support.
* Multiple files per post.
* Poster ID support.
* Smart Linking of duplicate images to the same real file, therefore reducing the consumed disk space.
* Support the following tags: 
	* `code` for code highlighting
	* `spoiler` for spoilers
	* `md` for markdown
	* `q` for quote

-File support:
* JPEG, PNG, APNG, GIF, BMP support.
* PDF Support (proper thumbnail generation require TallComponent PDFRasterizer library, not enabled by default).
* SVG documents support.
* Video (WEBM) and audio (MP3, AAC) support.
* You can add more file support by using plugins.

-Server related:
* Dual MS SQL or MySQL support. (Each one alone, not together)
* Static mode (Means threads are saved to HTML files).

Planned features (Todo)
=================

* Add LaTeX support.
* Administration panel.
* Online language adder and editor.
* Read and write JSON API (partially done).

How to install
==============

* Copy the binary files to your application root, under a directory called `bin`. 

* Make sure you copy the required dlls, and the templates files under a directory called `templates`, in the same dir as `chanb.dll`.

* Make a folder `langs` in the same directory as `chanb.dll`. Put the `en-US.dic` file in that directory (you can find it in the `languages` folder).

* Copy all the `.aspx` files to your application root.

* Point your browser to your website to begin installation.

* Enjoy.

Note: Currently, you can change settings by editing the `data.dic` file which is automatically made in the same folder as `chanb.dll`.

Bugs reports
============

Please file an issue when you find a bug, or you have a general complain about channel board.