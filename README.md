Chanb
=====

Chanb (Channel Board) is an ASP.NET Image board written in C#.

Licensed legally under the GPLv2

As of 2/14/2014, chanb was re-written in C#.

Live preview
=============

http://www.chanb.somee.com/csharp

The website gets cleaned each while.

Current features
========

Features that need re-writing were present in the VB.NET version of chanb, but they need to be re-written in C#.

-Administration related:
* Installer. [Need re-write]
* Moderator powers customization. [Need re-write]

-Board related:
* Auto updater.
* Automatic set dumper (with file counting support).
* Captcha. [Need re-write]
* Catalog. [Need re-write]
* Duplicate image checking support.
* Integrated archive. [Need re-write]
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
* PDF Support (thumbnail generation require Ghost-Script).
* SVG documents support. [Need re-write]
* Video (WEBM) and audio (MP3, AAC) support.
* You can add more file support by using plugins. [Need re-write]
* File optimization using `jpegtran` for JPG files, `optipng` for PNG files, and `mkclean` for WEBM files. 

-Server related:
* Dual MS SQL or MySQL support. (Each one alone, not together)
* Static mode (Means threads are saved to HTML files). [Dropped in the C# version]

Mono support
--------------

Chanb has been tested and confirmed to work under the XSP4 Mono ASP.NET server, running on a Raspberry Pi.

Planned features (Todo)
=================

* Add LaTeX support.
* Administration panel.
* Read and write JSON API (partially done). [Need re-write]

How to install
==============

The following is a step by step instruction to install the `VB.NET` version of chanb:

* Copy the binary files to your application root, under a directory called `bin`. 

* Make sure you copy the required dlls, and the templates files under a directory called `templates`, in the same dir as `chanb.dll`.

* Make a folder `langs` in the same directory as `chanb.dll`. Put the `en-US.dic` file in that directory (you can find it in the `languages` folder).

* Copy all the `.aspx` files to your application root.

* Point your browser to your website to begin installation.

Note: Currently, you can change settings by editing the `data.dic` file which is automatically made in the same folder as `chanb.dll`.


Bugs reports
============

Please file an issue when you find a bug, or you have a general complain about channel board.