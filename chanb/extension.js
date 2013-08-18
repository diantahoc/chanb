//Channel Board JavaScript extension.
function extension_init() {
    $(document).ready(function() {
    window.setInterval("ext_timer()", 1000);
});
    backlink();
    snas();
    wdLinks();
    beautifiesNames();
    format_page_title();
    initImageAE();
}

var isCurrentlyFetchingReplies = false;
var noNewRepliesIncrem = 20;
var delay = 0;

var pagetitle = document.title;

function ext_timer() {
    //thread updater stuffs
    if ((!isCurrentlyFetchingReplies) && is_a_thread_opened()) {
        noNewRepliesIncrem -= 1
        if (noNewRepliesIncrem < 0) {
            noNewRepliesIncrem = delay + 20;
            fetchnewreplies();
            console.log("Update request sent");
        }
       console.log("Updating in " + noNewRepliesIncrem + " sec");
    }
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

function fetchnewreplies() {
    isCurrentlyFetchingReplies = true;
    var $threadDiv = $(".thread:first");
    var threadID = $threadDiv.attr("id").replace("t", "");
    var lastpostID = $(".postContainer:last").attr("id").replace("pc", "");
    $.get(webroot + "api.aspx", {
        mode: "fetchrepliesafter",
        tid: threadID,
        lp: lastpostID
    }, function(data) { format_reply_div(data); if (data.length == 0) { delay += 15; console.log("No new replies"); } else { delay = 0; console.log("Thread updated."); } }, "json");
    isCurrentlyFetchingReplies = false;
}

function format_reply_div(jsondata) {

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
        //process wdlinks
        wdLinks();
    }
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

        for (i = 0; i < files.length; i++) {

            var wpi = files[i];

            var item = "";

            switch (wpi.Extension.toLowerCase()) {
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
                    break;
            } //switch block

            //Mark the first item as active
            if (!isNext) {
                item = repl(item, "AN", "active");
            } else {
                item = repl(item, "AN", "notactive");
            }
            item = repl(item, "filec", "");
            items.append(item);

            isNext = true;
        } //for block

        rotatorHTML = repl(rotatorHTML, "ID", id);
        rotatorHTML = repl(rotatorHTML, "IMAGECOUNT", files.length);
        rotatorHTML = repl(rotatorHTML, "ITEMS", items.toString());
        rotatorHTML = repl(rotatorHTML, "NOS", ""); //no need for no script items, since if this code was executing, JavaScript is enabled.
        return rotatorHTML;
    } else {

        //Single file

        var wpi = files[0];
        var item = "";

        switch (wpi.Extension.toLowerCase()) {
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
                break;
        } //switch block

        item = repl(item, "filec", "file");
        item = repl(item, "AN", "");
        return item;

    } //file count block
}

function get_image_html(file) {
    var r = ImageTemplate;
    r = repl(r, "ID", file.PostID);
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
            var item = $("#" + items[i].getAttribute('id').toString());
            item.removeClass("highlight");
        }
        selectedId = "";
    } else {
        var allitems = $(".post");
        for (i = 0; i < allitems.length; i++) {
            var item = $("#" + allitems[i].getAttribute('id').toString());
            item.removeClass("highlight");
        }
        var items = $(".post." + id);
        for (i = 0; i < items.length; i++) {
            var item = $("#" + items[i].getAttribute('id').toString());
            item.addClass("highlight");
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
            bl = document.createElement('a');
            bl.className = 'backlink';
            bl.href = '#' + replies[i].id;
            bl.textContent = '>>' + replies[i].id.slice(1);
            if (!(qb = t.getElementsByClassName('quoted-by')[0])) {
                linklist[tid[1]] = true;
                qb = document.createElement('div');
                qb.className = 'quoted-by';
                qb.textContent = '';
                qb.appendChild(bl);
                t.insertBefore(qb, t.getElementsByTagName('blockquote')[0]);
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
        var shortnameAnchor = document.createElement("a");
        var realId = fullnameAnchor.attr("id").toString().substr(3).toString();
        //setup the short name anchor
        shortnameAnchor.setAttribute("href", fullnameAnchor.attr("href").toString());
        shortnameAnchor.setAttribute("id", "fsn" + realId);
        shortnameAnchor.setAttribute("class", "fn");
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
        //var newlink = "javascript:openWindow('" + link + "','" + anchor.text() + "')";
        //anchor.attr("href", newlink);
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

function openNav() {

    if ($("#effecktNav").length == 0) {
        var nav = document.createElement("nav");
        nav.setAttribute("id", "effecktNav");
        nav.setAttribute("class", "effeckt-off-screen-nav effeckt-off-screen-nav-right-squeeze effeckt-off-screen-nav-show");
        document.appendChild(nav);
        openNav();
    } else { 
        //
    
    }
}

function get_selinks(v) {
    var sb = new StringBuilder();
    for (i = 0; i < selinks.length; i++) {
        sb.append(repl(selinks[i], "THUMB_LINK", v));
        sb.append("&nbsp;");
    }
    return sb.toString();
}

//format time string
// d = datetime object
function fts(d) { if (!(d == null)) { return d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + (d.getDate() + 1) + " " + d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds() } else { return "" } }

//replace a string inside a string
//a = string to process
//b = chanb keyword
//c = value to replace
function repl(a, b, c) { var d = new RegExp("%" + b + "%", "g"); return a.replace(d, c); }

function initImageAE() {

    var thumbs = document.getElementsByClassName("fileThumb");
    var isImage = /\.(jpe?g|a?png|gif|svg|bmp)$/
    for (i = 0; i < thumbs.length; i++) {
        if (isImage.test(thumbs[i].getAttribute("href").toString())) {
            var item = thumbs[i];
            var imgItem = item.children[0];
            var imgItemID = imgItem.getAttribute("id").toString();

            var newBigImage = document.createElement("img");
            
            newBigImage.setAttribute("id", "big" + imgItemID);
            newBigImage.setAttribute("asrc", item.href);
            newBigImage.setAttribute("class", "hide");
            
            item.appendChild(newBigImage);
        
            item.setAttribute("href","javascript:showFull('" + imgItemID + "' )");
            item.removeAttribute("target");
           
        
        }
    }
    
 }

 function showFull(id) {
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