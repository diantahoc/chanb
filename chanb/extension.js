//Channel Board JavaScript extension.
function extension_init() {
    $(document).ready(function() {
    window.setInterval(ext_timer, 1000);
});
    backlink();
    snas();
    wdLinks();
    beautifiesNames();
    format_page_title();
    initImageAE();
    if (prettyPrint) { prettyPrint(); }
    quotePreview();
}

var isUpdating = false;
var noNewRepliesIncrem = 20;
var delay = 0;

var pagetitle = document.title;

function ext_timer() {
    //thread updater stuffs
    if ((!isUpdating) && is_a_thread_opened()) {
        noNewRepliesIncrem -= 1
        if (noNewRepliesIncrem < 0) {
            noNewRepliesIncrem = delay + 20;
            if (noNewRepliesIncrem > 100) {noNewRepliesIncrem = 100; }
            updateThread();
            displayMessage("Update request sent");
        }
        displayMessage("Updating in " + noNewRepliesIncrem + " sec");
    }
}

function displayMessage(s) {
    document.getElementById("extension_log").textContent = s;//console.log(s);
 }

function updateThread() {
    var threadID = $(".thread:first").attr("id").replace("t", "");
    fetchnewreplies(threadID);
    check_for_deleted_posts(threadID);
}

function check_for_deleted_posts(threadID) {
    $.get(webroot + "api.aspx", {
        mode: "getthreadposts",
        tid: threadID
    }, function(data) {
        var posts = $(".post.reply");
        for (i = 0; i < posts.length; i++) {
            var currentPostID = parseInt(posts[i].id.slice(1));
            if (data.indexOf(currentPostID) == -1) {
                //post is deleted.
                if (document.getElementById("del" + currentPostID.toString()) == undefined) {
                    var deletedSpan = document.createElement("span");
                    deletedSpan.textContent = "[404]";
                    deletedSpan.setAttribute("style", "color: red");
                    deletedSpan.setAttribute("id", "del" + currentPostID.toString());
                    
                    posts[i].children[1].appendChild(deletedSpan);
                }
              
            }

        }
    }, "json");


}

function fetchnewreplies(threadID) {
    var lastpostID = $(".postContainer:last").attr("id").replace("pc", "");
    $.get(webroot + "api.aspx", {
        mode: "fetchrepliesafter",
        tid: threadID,
        lp: lastpostID
    }, function(data) { format_reply_div(data); if (data.length == 0) { delay += 15; displayMessage("No new replies"); } else { delay = 0; displayMessage("New replies added"); } }, "json");
}

function format_page_title() {
    if (is_a_thread_opened()) {
        var threadID = $(".thread:first").attr("id").replace("t", "");
        var subjectText = document.getElementById("pi" + threadID).getElementsByClassName("subject")[0].textContent;
        var commentText = document.getElementById("m" + threadID).textContent;
        if (subjectText == "") {
            //no subject, check the comment
            if (!(commentText == "")) {
                //if no comment, no need to modifie page title
                pagetitle = commentText;
            }
        } else {
              pagetitle = subjectText;
        }
    }
    document.title = pagetitle;
}

function is_a_thread_opened() {
    //if (document.getElementsByName("threadid")[0].value == "") { return false; } else {return true;}
    return (  ! document.getElementsByName("threadid")[0].value == "")
 }



function format_reply_div(jsondata) {
    isUpdating = true;
    for (i = 0; i < jsondata.length; i++){
        var q = replyTemplate;
        var postData = jsondata[i];

        q = repl(q, "ID", postData.ID);
        q = repl(q, "MODPANEL", modpanel);

        q = repl(q, "SUBJECT", postData.Subject);
        q = repl(q, "POST LINK", get_post_link(postData.ID));

        var postTime = new Date(postData.Time.toString());
        q = repl(q, "DATE UTC UNIX", postTime.getTime());
        q = repl(q, "DATE UTC TEXT", fts(postTime));

        // poster ID
        if (postData.posterId == "") {
            q = repl(q, "UID", "");
            q = repl(q, "UIDS", "");
        }
        else {
            q = repl(q, "UID", repl(uihs, "UID", postData.posterId));
            q = repl(q, "UIDS", postData.posterId);
        }

        //name and email
        if (postData.Email == "") {
            q = repl(q, "NAMESPAN", "<span class=\"name\">%NAME%</span>");
        } else {
            q = repl(q, "NAMESPAN", "<a href=\"mailto:%EMAIL%\" class=\"useremail\"><span class=\"name\">%NAME%</span></a>");
            q = repl(q, "EMAIL", postData.Email);
        }
        q = repl(q, "NAME", postData.Name);

        // files
        if (postData.HasFiles) {
            q = repl(q, "IMAGES", format_post_files(postData.Files, postData.ID));
        }
        else { q = repl(q, "IMAGES", ""); }

        q = repl(q, "POST TEXT", postData.Comment);
        var threadDiv = $("#t" + postData.ParentThread.toString());
        threadDiv.append(q);
        //process the new posts.
        wdLinks();
        beautifiesNames();
        backlink();
        if (prettyPrint) { prettyPrint(); }
        initImageAE();
    }
    isUpdating = false;
}

function get_post_link(id) {
    var q = document.URL.toString();
    return q.split("#")[0] + "#p" + id.toString();
}

function format_post_files(files, id) {
    if (files.length > 1) {
        //Multiple files        
        var items = new StringBuilder();

        var isNext = false;

        var rotatorHTML = FilesRotatorTemplate;

        var wpi, item, extension;

        var i = 0;
        
        for (; i < files.length; i++) {

            if (i > files.length) { break; }
            
             wpi = files[i];

          //  item = "";
            extension = wpi.Extension.toLowerCase();
            switch (extension) {
                case "jpg":
                case "jpeg":
                case "png":
                case "bmp":
                case "gif":
                case "apng":
                case "svg":
                case "pdf":
                    item = get_image_html(wpi);
                    break;
                case "webm":
                    item = get_video_html(wpi);
                    break;
                case "mp3":
                case "ogg":
                    item = get_audio_html(wpi);
                    break;
                default:
                    if (additionalFiles.indexOf(extension) >= 0) {
                        var func = eval("get_" + extension + "_html");
                        if (func) {
                            item = func(wpi);
                            break;
                        }
                    } else {
                        break;
                    }
            } //switch block
            wpi = null;
            
            //Mark the first item as active
            if (!isNext) {
                item = repl(item, "AN", "active");
                isNext = true;
            } else {
                item = repl(item, "AN", "notactive");
            }
            
            item = repl(item, "filec", "");
            items.append(item);
            item = null;

        } //for block
        
        wpi = null;
        item = null;
       
        rotatorHTML = repl(rotatorHTML, "ID", id);
        rotatorHTML = repl(rotatorHTML, "IMAGECOUNT", files.length.toString());     
        return repl(rotatorHTML, "ITEMS", items.toString());
       
        items = null;
   
    } else {

        //Single file

        var wpi = files[0];
        var item = "";
        var extension = wpi.Extension.toLowerCase();
        
        switch (extension) {
            case "jpg":
            case "jpeg":
            case "png":
            case "bmp":
            case "gif":
            case "apng":
            case "svg":
            case "pdf":
                item = get_image_html(wpi);
                break;
            case "webm":
                item = get_video_html(wpi);
                break;
            case "mp3":
            case "ogg":
                item = get_audio_html(wpi);
                break;
            default:
                if (additionalFiles.indexOf(extension) >= 0) {
                    var func = eval("get_" + extension + "_html");
                    if (!(func == null)) {
                        item = func(wpi);
                        break;
                    }
                } else {
                    break;
                }
        } //switch block

        item = repl(item, "filec", "file");
        item = repl(item, "AN", "");
        return item;

    } //file count block
}

function get_image_html(file) {
    var r = repl(ImageTemplate, "ID", file.PostID);
    r = repl(r, "FILE NAME", file.RealName);
    r = repl(r, "IMAGE TEXT DL", webroot + "img.aspx?cn=" + file.ChanbName + "&rn=" + file.RealName);
    r = repl(r, "IMAGE DL", file.ImageWebPath);
    r = repl(r, "IMAGE SRC", file.ImageWebPath);
    r = repl(r, "FILE SIZE", format_size_string(file.Size));
    r = repl(r, "IMAGE SIZE", file.Dimensions);
    r = repl(r, "IMAGE MD5", file.MD5);
    r = repl(r, "THUMB_LINK", file.ImageThumbailWebPath);
    r = repl(r, "IMAGE EXT", file.Extension);
    r = repl(r, "Search Engine Links", get_selinks(file.ImageThumbailWebPath));
    file = null;
    return r;
}

function get_audio_html(file) {
    var r = AudioItemTemplate;
    r = repl(r, "ID", file.PostID);
    r = repl(r, "FILE NAME", file.RealName);
    r = repl(r, "FILE SIZE", format_size_string(file.Size));
    r = repl(r, "IMAGE TEXT DL", webroot + "img.aspx?cn=" + file.ChanbName + "&rn=" + file.RealName);
    r = repl(r, "LINK", file.ImageWebPath);
    r = repl(r, "IMAGE MD5", file.MD5);
    r = repl(r, "IMAGE EXT", file.Extension);
    r = repl(r, "NO AUDIO SUPPORT", "Your browser does not support audio tags.")
    if (file.Extension == "MP3") {
        r = repl(r, "EXT", "mpeg")
    }
    if (file.Extension == "OGG") {
        r = repl(r, "EXT", "ogg")
    }
    return r;
}

function get_video_html(file) {
    var r = VideoItemTemplate;
    r = repl(r, "ID", file.PostID);
    r = repl(r, "FILE NAME", file.RealName);
    r = repl(r, "FILE SIZE", format_size_string(file.Size));
    r = repl(r, "IMAGE TEXT DL", webroot + "img.aspx?cn=" + file.ChanbName + "&rn=" + file.RealName);
    r = repl(r, "VIDEO LINK", file.ImageWebPath);
    r = repl(r, "IMAGE MD5", file.MD5);
    r = repl(r, "IMAGE EXT", file.Extension);
    r = repl(r, "NO VIDEO SUPPORT", "Your browser does not support video tags.")
    if (file.Extension == "WEBM") {
        r = repl(r, "EXT", "webm")
    }
    return r;
}

function format_size_string(s) { 
    var kb = (s / 1024).toFixed(0);
    var mb = (s / 1048576).toFixed(0); ;
    var gb = (s / 1073741824).toFixed(0); ;
    if (kb == 0) {
        return s.toString() + " B";
    } else if (kb > 0 && mb == 0) {
        return kb.toString() + " KB";
    } else if (mb > 0 && gb == 0) {
        return mb.toString() + " MB";
    } else if (gb > 0) {
        return gb.toString() + " GB";
    } else {
    return s.toString();
    }
}

var selectedId = "";

function higlightID(id) {
    if (selectedId == id) {
        var items = $(".post." + selectedId);
        for (i = 0; i < items.length; i++) {
             $("#" + items[i].id).removeClass("highlight");
        }
        selectedId = "";
    } else {
    var allitems = $(".post");
        //hide all
        for (i = 0; i < allitems.length; i++) {
            $("#" + allitems[i].id).removeClass("highlight");;
        }
        //highlight only posts with that id
        var items = $(".post." + id);
        for (i = 0; i < items.length; i++) {
            $("#" + items[i].id).addClass("highlight");
        }
        selectedId = id;
    }
}

function backlink() {
    var i, j, ii, jj, tid, bl, qb, t, form, backlinks, linklist, replies;
 
    form = document.getElementById("delfrm");

    if (!(replies = form.getElementsByClassName('reply'))) {
        return;
    }

    for (i = 0, j = replies.length; i < j; ++i) {
        if (!(backlinks = replies[i].getElementsByClassName('backlink'))) {
            continue;
        }
        linklist = {};
        for (ii = 0, jj = backlinks.length; ii < jj; ++ii) {
            tid = backlinks[ii].getAttribute('href').split(/#/);
            if (!(t = document.getElementById(tid[1]))) {
                continue;
            }
            //			if (t.tagName == 'DIV') {
            //				backlinks[ii].textContent = '>>OP';
            //			}
            if (linklist[tid[1]]) {
                continue;
            }
            var bl = null;
            var blID = "blq" + tid + replies[i].id;
            if (document.getElementById(blID) == undefined) {
                bl = document.createElement('a');
                //bl.className = 'backlink';
                bl.setAttribute("id", blID);
                bl.setAttribute("class", "backlonk");
                bl.href = '#' + replies[i].id;
                bl.textContent = '->' + replies[i].id.slice(1);
            } else {continue;}
            if (!(qb = t.children[1].getElementsByClassName('nameBlock')[0])) {
                continue;
               // linklist[tid[1]] = true;   
              //  qb = document.createElement('div');
             //   qb.className = 'quoted-by';
              //  qb.textContent = '';
              //  qb.appendChild(bl);
               // t.insertBefore(qb, t.getElementsByTagName('blockquote')[0]);
            }
            else {
                linklist[tid[1]] = true;
                qb.appendChild(document.createTextNode(' '));
                qb.appendChild(bl);
            }
        }
    }
}

//Shorten long file names.
function beautifiesName(anch) {
    var fullnameAnchor = $(anch);
    if (fullnameAnchor.text().length > 20) {
        fullnameAnchor.removeClass("fn");
        var shortnameAnchor = document.createElement("a");
        var realId = fullnameAnchor.attr("id").toString().substr(3).toString();
        //setup the short name anchor
        shortnameAnchor.setAttribute("href", fullnameAnchor.attr("href").toString());
        shortnameAnchor.setAttribute("id", "fsn" + realId);
        //shortnameAnchor.setAttribute("class", "fn");
        shortnameAnchor.text = fullnameAnchor.text().substr(0, 17) + "...";
        //hide the full name 
        fullnameAnchor.addClass("hide");
        //get the span parent
        var parent = fullnameAnchor.parent();
        //add the shortNameAnchor       
        parent.append(shortnameAnchor);
        //add mouse handlers
        $(parent).mouseover(function() { showFullName(realId); });
        $(parent).mouseout(function() { showShortName(realId); });
    }
}

function beautifiesNames() {
    var items = $(".fn");
    for (i = 0; i < items.length; i++) {
        beautifiesName(items[i])
    }
}

//Do misc stuffs such as name/email/password saving.
function snas(){
    $("#pname").attr("value", getCookie("postername"));
    $("#pemail").attr("value", getCookie("posteremail"));
    if (getCookie("posterpass") == "") { setCookie("posterpass", $("#formps").attr("value"), 3); } else { $("#formps").attr("value", getCookie("posterpass")); $("#formdelP").attr("value", getCookie("posterpass")); }
}

//process window links (WDLinks)
function wdLinks(){
    var WDLitems = $(".wdlink");
    for (i = 0; i < WDLitems.length; i++) {
        var anchor = $(WDLitems[i]);
        var link = anchor.attr("href").toString();
        anchor.attr("href", "javascript:openWindow('" + link + "','" + anchor.text() + "')");
        anchor.removeAttr("target");
        anchor.removeClass("wdlink");
    }
}

function openWindow(link, title) {
    var width = 750;
    var height = 250;
    var left = (screen.width - width) / 2;
    var top = (screen.height - height) / 2;
    var params = 'width=' + width + ', height=' + height + ', top=' + top + ', left=' + left + ', directories=no, location=no, menubar=no, resizable=no, scrollbars=no, status=yes, toolbar=no';
    newwin = window.open(link, title, params);
    if (window.focus) {
        newwin.focus()
    }
}

//function openNav() {

//    if ($("#effecktNav").length == 0) {
//        var nav = document.createElement("nav");
//        nav.setAttribute("id", "effecktNav");
//        nav.setAttribute("class", "effeckt-off-screen-nav effeckt-off-screen-nav-right-squeeze effeckt-off-screen-nav-show");
//        document.appendChild(nav);
//        openNav();
//    } else { 
//        //
//    
//    }
//}

function get_selinks(v) {
    var sb = new StringBuilder();
    for (i = 0; i < selinks.length; i++) {
        sb.append(repl(selinks[i], "THUMB_LINK", v));
        sb.append("&nbsp;");
    }
    return sb.toString();
    sb = null;
}

//format time string
// d = datetime object
function fts(d) { if (d) { return d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + (d.getDate() + 1) + " " + d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds() } else { return "" } }

//replace a string inside a string
//a = string to process
//b = chanb keyword
//c = value to replace
function repl(a, b, c) {var re = new RegExp("%" + b + "%", "g");  return a.replace(re, c)}

//------------- Image inline expansion region --------------------
function initImageAE() {

    var thumbs = document.getElementsByClassName("fileThumb");
    var isImage = /\.(jpe?g|a?png|gif|bmp)$/
    for (i = 0; i < thumbs.length; i++) {
        if (!(thumbs[i].hasAttribute("href"))) {continue;}
        
        if (isImage.test(thumbs[i].getAttribute("href").toString())) {
            var item = thumbs[i];
            var imgItem = item.children[0];
            var imgItemID = imgItem.getAttribute("id").toString();

            var newBigImage = document.createElement("img");
            
            newBigImage.setAttribute("id", "big" + imgItemID);
            newBigImage.setAttribute("asrc", item.href);
            newBigImage.setAttribute("class", "hide");
            
            item.appendChild(newBigImage);
        
            //item.setAttribute("href","javascript:showFull('" + imgItemID + "' )");
          
            item.addEventListener('click', showFull, false);
            item.removeAttribute("target");
           
        
        }
    }
    
 }

 function showFull(e) {
     e.preventDefault();

     var id = e.target.getAttribute("id").toString().replace("big","");

     var ab = document.getElementById("big" + id);
     
     var thumbImg = document.getElementById(id);

     if (ab.getAttribute("asrc") == "") {
         //hide or show
         $(ab).toggleClass("hide");
         $(thumbImg).toggleClass("hide");

     } else {

         ab.setAttribute("src", ab.getAttribute("asrc"));
         ab.setAttribute("asrc", "");

         $(ab).removeClass("hide");
         $(thumbImg).addClass("hide");
     }
 
 }
// -----------------------------------------------------------

// ----------- Quote Preview Region---------------------------

 function quotePreview() {
     var quotes = document.forms.delfrm.getElementsByClassName('backlink');
     for (i = 0, j = quotes.length; i < j; i++) {
         quotes[i].addEventListener('mousemove', display_post_preview, false);
         quotes[i].addEventListener('mouseout', remove_post_preview, false);
     }
     var quotes = document.forms.delfrm.getElementsByClassName('backlonk');
     for (i = 0, j = quotes.length; i < j; i++) {
         quotes[i].addEventListener('mousemove', display_post_preview, false);
         quotes[i].addEventListener('mouseout', remove_post_preview, false);
         quotes[i].addEventListener('click', embed_post, false);
     }
     
 }

 function embed_post(e) {
     e.preventDefault();
     var postID = e.target.getAttribute('href').split('#')[1];
     var parentID = __get_parentID(e);
     
     var cn = document.getElementById("qp" + postID + parentID);
     var t = document.getElementById(parentID);

     if (t && cn) {
         if (cn.getAttribute("isinserted") == "yep") {
             cn.setAttribute("isinserted", "");
         t.removeChild(cn);
        }else{
         cn.setAttribute("class", "qpInserted");
         cn.setAttribute("isinserted", "yep");
         t.insertBefore(cn, t.getElementsByTagName("blockquote")[0]);
       }
     }
  }

function display_post_preview(e) {
    e.preventDefault();
     var postID = e.target.getAttribute('href').split('#')[1];

     var parentID = __get_parentID(e);

     var qpID = "qp" + postID + parentID;
     if (document.getElementById(qpID))
     {
         document.getElementById(qpID).setAttribute("style", get_relative_mouse_postion(e));
     }
     else 
     {
         var replyBox = $("#" + postID);
         replyBox.addClass("qphl");

         var newObject = replyBox.clone();
       //  console.log(newObject);

         newObject[0].setAttribute("id", qpID);

         newObject[0].setAttribute("class", "qp");
         newObject[0].setAttribute("style", get_relative_mouse_postion(e));

         //dash the corresponding backlink
       
         var blk = newObject[0].getElementsByTagName("a");

         for (i = 0; i < blk.length; i++) {

             if (blk[i].getAttribute('href').indexOf(parentID) >=0) {
                 blk[i].setAttribute("style", "text-decoration: overline underline;");
             }
         
         }

         document.getElementsByTagName("body")[0].appendChild(newObject[0]);
     }

 }

 function get_relative_mouse_postion(e) {
     var left = e.clientX;
     var top = e.clientY;  
     return "left: " + left + "px; top: " + top + "px;"
 }

 function __get_parentID(e) {
     switch (e.target.getAttribute("class")) {
         case "backlink":
             return e.target.parentElement.parentElement.getAttribute("id");
             break;
         case "backlonk":
             return e.target.parentElement.parentElement.parentElement.getAttribute("id");
             break;
         default:
             return "";
             break;
     }
 }

 function remove_post_preview(e) {
     e.preventDefault();
     var postID = e.target.getAttribute('href').split('#')[1];
     var parentID = __get_parentID(e);
     
     var cn = document.getElementById("qp" + postID+ parentID);
     if (cn) 
     {
         if (cn.getAttribute("isinserted") == "yep")
          {
          } 
         else 
         {
             document.getElementsByTagName("body")[0].removeChild(cn);
         }
     }
     $("#" + postID).removeClass("qphl");
 }

 
//------------------------------------------------------


// StringBuilder class
// Initializes a new instance of the StringBuilder class
// and appends the given value if supplied
function StringBuilder(value) {
    this.strings = new Array("");
    this.append(value);
}

// Appends the given value to the end of this instance.
StringBuilder.prototype.append = function(value) {
    if (value) {
        this.strings.push(value);
    }
}

// Clears the string buffer
StringBuilder.prototype.clear = function() {
    this.strings.length = 1;
}

// Converts this instance to a String.
StringBuilder.prototype.toString = function() {
    return this.strings.join("");
}