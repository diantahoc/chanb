//Channel Board JavaScript extension.
//Global Variables
var isUpdating = false;
var updIn = 20;
var delay = 0;
var pagetitle = document.title;
var qr_isSending = false;
var qr_isHidden = false;
var qr_active_thread = -1;
var qr_mode = "thread";
var qr_threads;

function extension_init() {
    load_settings();
    show_hide_pbox(); //todo: move this call to scripts.js
    init_file_rotator();
    backlink();
    snas();
    wdLinks();
    beautifiesNames();
    format_page_title();
    initImageAE();
    if (prettyPrint)
    {
        prettyPrint();
    }
    //quotePreview();
    window.setInterval(ext_timer, 1000);
}

function init_file_rotator() {
    var rotators = document.getElementsByClassName("rotator");
    for (i = 0; i < rotators.length; i++)
    {
        rotators[i].children[1].setAttribute("class", "");   
    }
}

function ext_timer() {
    //thread updater stuffs
    if ((!isUpdating) && is_a_thread_opened())
    {
        updIn -= 1
        if (updIn < 0)
        {
            updIn = delay + 20;
            if (updIn > 100)
            {
                updIn = 100;
            }
            updateThread();
            displayMessage("Update request sent");
        }
        displayMessage("Updating in " + updIn + " sec");
    }
}

function displayMessage(s) {
    gid("extension_log").textContent = s; //console.log(s);
}

function updateThread() {
    var threadID = $(".thread:first").attr("id").replace("t", "");
    fetchnewreplies(threadID);
    check_for_deleted_posts(threadID);
}

function check_for_deleted_posts(threadID) {
    $.get(webroot + "api.aspx",
    {
        mode: "getthreadposts",
        tid: threadID
    }, function (data) {
        var posts = $(".post.reply");
        for (i = 0; i < posts.length; i++)
        {
            var currentPostID = parseInt(posts[i].id.slice(1));
            if (data.indexOf(currentPostID) == -1)
            {
                //post is deleted.
                if (document.getElementById("del" + currentPostID.toString()) == undefined)
                {
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

function forceUpdate() {
    updIn = -1;
}

function fetchnewreplies(threadID) {
    var lastpostID = $(".postContainer:last").attr("id").replace("pc", "");
    $.get(webroot + "api.aspx",
    {
        mode: "fetchrepliesafter",
        tid: threadID,
        lp: lastpostID
    }, function (data) {
        format_reply_div(data);
        if (data.length == 0)
        {
            delay += 15;
            displayMessage("No new replies");
        }
        else
        {
            delay = 0;
            displayMessage("New replies added");
        }
    }, "json");
}

function format_page_title() {
    if (is_a_thread_opened())
    {
        var threadID = $(".thread:first").attr("id").replace("t", "");
        var subjectText = document.getElementById("pi" + threadID).getElementsByClassName("subject")[0].textContent;
        var commentText = document.getElementById("m" + threadID).textContent;
        if (subjectText == "")
        {
            //no subject, check the comment
            if (commentText !== "")
            {
                //if no comment, no need to modify page title
                pagetitle = commentText;
            }
        }
        else
        {
            pagetitle = subjectText;
        }
    }
    document.title = pagetitle;
}

function is_a_thread_opened() {
    return (document.getElementsByName("threadid")[0].value !== "");
}

function get_thread_id() {
    try
    {
        if (is_a_thread_opened())
        {
            return parseInt(document.getElementsByClassName("thread")[0].getAttribute("id").replace("t", ""));
        }
        else
        {
            return -1;
        }
    }
    catch (e)
    {
        return -1;
    }
}

function format_reply_div(jsondata) {
    isUpdating = true;
    for (i = 0; i < jsondata.length; i++)
    {
        var q = replyTemplate;
        var postData = jsondata[i];
        q = repl(q, "ID", postData.ID);
        q = repl(q, "TID", postData.ParentThread);
        q = repl(q, "MODPANEL", modpanel);
        q = repl(q, "SUBJECT", postData.Subject);
        q = repl(q, "POST LINK", get_post_link(postData.ID));
        var postTime = new Date(postData.Time.toString());
        q = repl(q, "DATE UTC UNIX", postTime.getTime());
        q = repl(q, "DATE UTC TEXT", fts(postTime));
        // poster ID
        if (postData.posterId == "")
        {
            q = repl(q, "UID", "");
            q = repl(q, "UIDS", "");
        }
        else
        {
            q = repl(q, "UID", repl(uihs, "UID", postData.posterId));
            q = repl(q, "UIDS", postData.posterId);
        }
        //name and email
        if (postData.Email == "")
        {
            q = repl(q, "NAMESPAN", "<span class=\"name\">%NAME%</span>");
        }
        else
        {
            q = repl(q, "NAMESPAN", "<a href=\"mailto:%EMAIL%\" class=\"useremail\"><span class=\"name\">%NAME%</span></a>");
            q = repl(q, "EMAIL", postData.Email);
        }
        q = repl(q, "NAME", postData.Name);
        // files
        if (postData.HasFiles)
        {
            q = repl(q, "IMAGES", format_post_files(postData.Files, postData.ID));
            q = repl(q, "DFILESMENU", repl(dfiles_template, "ID", postData.ID));
        }
        else
        {
            q = repl(q, "IMAGES", "");
            q = repl(q, "DFILESMENU", "");
        }
        q = repl(q, "POST TEXT", postData.Comment);
        var threadDiv = $("#t" + postData.ParentThread.toString());
        threadDiv.append(q);
        //process the new posts.
        wdLinks();
        beautifiesNames();
        backlink();
        if (prettyPrint)
        {
            prettyPrint();
        }
        initImageAE();
    }
    isUpdating = false;
}

function get_post_link(id) {
    var q = document.URL.toString();
    return q.split("#")[0] + "#p" + id.toString();
}

function format_post_files(files, id) {
    if (files.length > 1)
    {
        //Multiple files        
        var items = new StringBuilder();
        var isNext = false;
        var rotatorHTML = FilesRotatorTemplate;
        var wpi, item, extension;
        var i = 0;
        for (; i < files.length; i++)
        {
            if (i > files.length)
            {
                break;
            }
            wpi = files[i];
            //  item = "";
            extension = wpi.Extension.toLowerCase();
            switch (extension)
            {
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
                    if (additionalFiles.indexOf(extension) >= 0)
                    {
                        var func = eval("get_" + extension + "_html");
                        if (func)
                        {
                            item = func(wpi);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
            } //switch block
            wpi = null;
            //Mark the first item as active
            if (!isNext)
            {
                item = repl(item, "AN", "active");
                isNext = true;
            }
            else
            {
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
    }
    else
    {
        //Single file
        var wpi = files[0];
        var item = "";
        var extension = wpi.Extension.toLowerCase();
        switch (extension)
        {
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
                if (additionalFiles.indexOf(extension) >= 0)
                {
                    var func = eval("get_" + extension + "_html");
                    if (!(func == null))
                    {
                        item = func(wpi);
                        break;
                    }
                }
                else
                {
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
    if (file.Extension == "MP3")
    {
        r = repl(r, "EXT", "mpeg")
    }
    if (file.Extension == "OGG")
    {
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
    if (file.Extension == "WEBM")
    {
        r = repl(r, "EXT", "webm")
    }
    return r;
}

function format_size_string(s) {
    var kb = (s / 1024).toFixed(0);
    var mb = (s / 1048576).toFixed(0); ;
    var gb = (s / 1073741824).toFixed(0); ;
    if (kb == 0)
    {
        return s.toString() + " B";
    }
    else if (kb > 0 && mb == 0)
    {
        return kb.toString() + " KB";
    }
    else if (mb > 0 && gb == 0)
    {
        return mb.toString() + " MB";
    }
    else if (gb > 0)
    {
        return gb.toString() + " GB";
    }
    else
    {
        return s.toString();
    }
}
var selectedId = "";

function higlightID(id) {
    if (selectedId == id)
    {
        var items = $(".post." + selectedId);
        for (i = 0; i < items.length; i++)
        {
            $("#" + items[i].id).removeClass("highlight");
        }
        selectedId = "";
    }
    else
    {
        var allitems = $(".post");
        //hide all
        for (i = 0; i < allitems.length; i++)
        {
            $("#" + allitems[i].id).removeClass("highlight"); ;
        }
        //highlight only posts with that id
        var items = $(".post." + id);
        for (i = 0; i < items.length; i++)
        {
            $("#" + items[i].id).addClass("highlight");
        }
        selectedId = id;
    }
}

function backlink() {
    var i, j, ii, jj, tid, bl, qb, t, form, backlinks, linklist, replies;
    form = document.getElementsByClassName("board")[0];
    if (!(replies = form.getElementsByClassName('reply')))
    {
        return;
    }
    for (i = 0, j = replies.length; i < j; ++i)
    {
        if (!(backlinks = replies[i].getElementsByClassName('backlink')))
        {
            continue;
        }
        linklist = {};
        for (ii = 0, jj = backlinks.length; ii < jj; ++ii)
        {
            tid = backlinks[ii].getAttribute('href').split(/#/);
            if (!(t = document.getElementById(tid[1])))
            {
                continue;
            }
            //			if (t.tagName == 'DIV') {
            //				backlinks[ii].textContent = '>>OP';
            //			}
            if (linklist[tid[1]])
            {
                continue;
            }
            var bl = null;
            var blID = "blq" + tid + replies[i].id;
            if (document.getElementById(blID) == undefined)
            {
                bl = document.createElement('a');
                //bl.className = 'backlink';
                bl.setAttribute("id", blID);
                bl.setAttribute("class", "backlonk");
                bl.href = '#' + replies[i].id;
                bl.textContent = '->' + replies[i].id.slice(1);
            }
            else
            {
                continue;
            }
            if (!(qb = t.children[1].getElementsByClassName('nameBlock')[0]))
            {
                continue;
                // linklist[tid[1]] = true;   
                //  qb = document.createElement('div');
                //   qb.className = 'quoted-by';
                //  qb.textContent = '';
                //  qb.appendChild(bl);
                // t.insertBefore(qb, t.getElementsByTagName('blockquote')[0]);
            }
            else
            {
                linklist[tid[1]] = true;
                qb.appendChild(document.createTextNode(' '));
                qb.appendChild(bl);
            }
        }
    }
}
//Shorten long file names.
//=================================================================
function beautifiesNames() {
    var items = $(".fn");
    for (i = 0; i < items.length; i++)
    {
        bName(items[i])
    }
}

function bName(element) {
    var link = element.href;
    var text = element.textContent;
    if (text.length > 20)
    {
        element.addEventListener("mouseover", function (event) {
            var shortName = document.createElement("a");
            shortName.setAttribute("href", link);
            shortName.textContent = text;
            shortName.addEventListener("mouseout", function (event) {
                shortName.parentElement.removeChild(shortName);
                element.style.display = "";
            }, false);
            element.parentElement.insertBefore(shortName, element);
            element.style.display = "none";
        }, false);
        element.setAttribute("class", "");
        element.textContent = element.textContent.substr(0, 17) + "...";
    }
}
//=================================================================
//Do misc stuffs such as name/email/password saving.
function snas() {
    $("#pname").attr("value", SM.GetItem("postername"));
    $("#pemail").attr("value", SM.GetItem("posteremail"));
    if (SM.GetItem("posterpass") == "")
    {
        SM.SetItem("posterpass", $("#formps").attr("value"), 3);
    }
    else
    {
        $("#formps").attr("value", SM.GetItem("posterpass"));
        $("#formdelP").attr("value", SM.GetItem("posterpass"));
    }
}

function beforePost() {
    SM.SetItem("postername", $("#pname").val(), 3);
    if ($("#formps").val().length > 0)
    {
        SM.GetItem("posterpass", $("#formps").val(), 3)
    };
    SM.GetItem("posteremail", $("#pemail").val(), 3);
}
//process window links (WDLinks)
function wdLinks() {
    var WDLitems = $(".wdlink");
    for (i = 0; i < WDLitems.length; i++)
    {
        var anchor = $(WDLitems[i]);
        var link = anchor.attr("href").toString();
        anchor.attr("href", "javascript:openWindow('" + link + "','" + anchor.text() + "')");
        anchor.removeAttr("target");
        anchor.removeClass("wdlink");
    }
}

function openWindow(link, title) {
    var width = 800;
    var height = 400;
    if (SM.GetItem("modal_dialogs") == "true")
    {
        var left = (screen.width - width) / 2;
        var top = (screen.height - height) / 3;
        //dim the page
        var dim = document.createElement("div");
        dim.setAttribute("id", "dimbox");
        dim.setAttribute("title", title);


        var d = document.createElement("div");

        d.setAttribute("id", "modal_dialog");
        d.setAttribute("style", "top: " + top + "px ;left:" + left + "px; width: " + width + "px;height: " + height + "px;");


        var ifra = document.createElement("iframe");
        ifra.setAttribute("src", link);
        ifra.setAttribute("frameborder", "0");
        ifra.setAttribute("style", "width:100%!important; height: 100%!important");


        d.appendChild(ifra);
        dim.onclick = function () { document.body.removeChild(dim); document.body.removeChild(d); };


        document.body.insertBefore(dim, document.body.children[0]);

        document.body.insertBefore(d, document.body.children[0]);

    }
    else
    {
        var left = (screen.width - width) / 2;
        var top = (screen.height - height) / 2;
        var params = 'width=' + width + ', height=' + height + ', top=' + top + ', left=' + left + ', directories=no, location=no, menubar=no, resizable=no, scrollbars=yes, status=yes, toolbar=no';
        newwin = window.open(link, title, params);
        if (window.focus)
        {
            newwin.focus()
        }
    }
}

function get_selinks(v) {
    var sb = new StringBuilder();
    for (i = 0; i < selinks.length; i++)
    {
        sb.append(repl(selinks[i], "THUMB_LINK", v));
        sb.append("&nbsp;");
    }
    return sb.toString();
    sb = null;
}
//format time string
// d = datetime object
function fts(d) {
    if (d)
    {
        return d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + (d.getDate() + 1) + " " + d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
    }
    else
    {
        return "";
    }
}
//replace a string inside a string
//a = string to process
//b = chanb keyword
//c = value to replace
function repl(a, b, c) {
    var re = new RegExp("%" + b + "%", "g");
    return a.replace(re, c)
}
//------------- Image inline expansion region --------------------
function initImageAE() {
    var thumbs = document.getElementsByClassName("fileThumb");
    var isImage = /\.(jpe?g|a?png|gif|bmp)$/
    for (i = 0; i < thumbs.length; i++)
    {
        if (!(thumbs[i].hasAttribute("href")))
        {
            continue;
        }
        if (isImage.test(thumbs[i].getAttribute("href").toString()))
        {
            if (!thumbs[i].hasAttribute("processed"))
            {
                fim(thumbs[i], thumbs[i].href);
                thumbs[i].removeAttribute("target");
                thumbs[i].setAttribute("processed", "");
            }
        }
    }
}

function fim(element, link) {
    element.addEventListener("click", function (event) {

        event.preventDefault();
        if (SM.GetItem("modal_images") == "true")
        {

            //dim the page
            var dim = document.createElement("div");
            dim.setAttribute("id", "dimbox");

            var d = document.createElement("img");

            d.setAttribute("id", "modal_dialog");
            d.setAttribute("style", "left: 33%; max-width: 80%;max-height: 80%;");

            d.setAttribute("src", link);

            dim.onclick = function () { document.body.removeChild(dim); document.body.removeChild(d); };


            document.body.insertBefore(dim, document.body.children[0]);

            document.body.insertBefore(d, document.body.children[0]);

        }
        else
        {
            var href = document.createElement("a");
            href.setAttribute("href", link);
            var fimg = document.createElement("img");
            fimg.setAttribute("src", link);
            fimg.setAttribute("style", "max-width:75%;max-height:75%");
            fimg.onclick = function (event) {
                event.preventDefault();
                this.parentNode.removeChild(this);
                element.style.display = "";
                element.parentElement.removeChild(href);
            };

            href.appendChild(fimg);
            element.parentElement.insertBefore(href, element);
            element.style.display = "none";
        }
    }, false);

}
// ---------------End of inline image expansion region -----------

// ---------------- Quote Preview Region -------------------------
/*function quotePreview() {
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
}
else {
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
if (document.getElementById(qpID)) {
document.getElementById(qpID).setAttribute("style", get_relative_mouse_postion(e));
}
else {
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
if (blk[i].getAttribute('href').indexOf(parentID) >= 0) {
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
var cn = document.getElementById("qp" + postID + parentID);
if (cn) {
if (cn.getAttribute("isinserted") == "yep")
{ }
else {
document.getElementsByTagName("body")[0].removeChild(cn);
}
}
$("#" + postID).removeClass("qphl");
}*/
//------------------------------------------------------

//===========================Quick Reply box==============================
function qr(command, param) {
    var qbox = $("#qrbox");
    switch (command)
    {
        case "hide":
            if (!qr_isHidden)
            {
                qbox.hide();
                qr_isHidden = true;
            }
            qr("clear");
            qr("clear");
            qr("auto_detect_mode");
            break;
        case "show":
            if (qr_isHidden)
            {
                qbox.show();
                qr_isHidden = false;
            }
            qr("clear");
            qr("update_ui");
            break;
        case "clear":
            var oaw = ["qr_name", "qr_email", "qr_subject", "qr_comment"];
            for (i = 0; i < oaw.length; i++)
            {
                gid(oaw[i]).value = "";
                gid(oaw[i]).textContent = "";
            }
            var qr_fil = gid("qr_files");
            var gee = qr_fil.children;
            for (i = 1; i < gee.length; i++)
            {
                qr_fil.removeChild(gee[i]);
            }
            //clear the selected file
            gee[0].value = "";
            $('#qr_message').addClass('hide');
            break;
        case "submit":
            //prepare a few things first
            switch (qr_mode)
            {
                case "thread":
                    $("#qr_mode").attr("value", "thread");
                    break;
                case "reply":
                    if (qr_active_thread >= 1)
                    {
                        $("#qr_mode").attr("value", "reply");
                        $("#qr_form_tid").attr("value", qr_active_thread);
                    }
                    else
                    {
                        $("#qr_mode").attr("value", "thread");
                    }
                    break;
                default:
                    $("#qr_mode").attr("value", "thread");
                    break;
            }
            qr("update_ui");
            qr_send(gid("qr_form"));
            break;
        case "open": //open the qr with an active thread selected.
            if (param != null)
            {
                qr("set_active_thread", param);
                qr("show");
            }
            qr("update_ui");
            break;
        case "set_active_thread": // change mode to reply and set active thread id, since we always reply to the active thread id
            if (param != null)
            {
                qr_active_thread = param;
                qr("set_mode", "reply");
            }
            qr("update_ui");
            break;
        case "set_mode": // reply or new thread
            if (param != null)
            {
                qr_mode = param;
            }
            qr("update_ui");
            break;
        case "load_data": //load current threads ids from the catalog
            $.get(webroot + "api.aspx",
        {
            mode: "getcatalogids"
        }, function (data) {
            if (data.length > 0)
            {
                qr_threads = null;
                qr_threads = new Array();
                for (i = 0; i < data.length; i++)
                {
                    qr_threads.push(data[i]);
                }
            }
        }, "json");
            break;
        case "update_ui":
            var s = "";
            if (qr_mode == "thread")
            {
                s = "(new thread)";
            }
            if (qr_mode == "reply")
            {
                s = "(reply to " + qr_active_thread + ")";
            }
            gid("qr_title_text").textContent = "Quick reply " + s;
            break;
        case "auto_detect_mode":
            if (is_a_thread_opened())
            {
                qr_mode = "reply";
                qr_active_thread = get_thread_id();
            }
            else
            {
                qr_mode = "thread";
                qr_active_thread = -1;
            }
            break;
        default:
            break;
    }
}

function qr_init() {
    var qbox = $("#qrbox");
    qbox.draggable(
    {
        appendTo: "#qrtitle",
        containment: "document"
    });
    qbox.removeClass("hide");
    qbox.hide();
    qr_isHidden = true;
    $("#progress-block").hide();
    qr("auto_detect_mode");
    qr("update_ui");
    qr("load_data");
}

function qr_send(qrForm) {
    if (qr_isSending)
    {
        return null;
    }
    qr_isSending = true;
    //UI Stuffs
    if (qr_hasFiles())
    {
        $("#progress-block").slideDown();
    }
    var req = new XMLHttpRequest();
    req.open("POST", webroot + "api.aspx", true);
    req.withCredentials = true;
    req.upload.onprogress = function (e) {
        qr_updateProgress(e);
    }
    req.onerror = function () {
        req = null;
        qr_enable_controls();
    }
    req.onloadend = function () {
        if (req.getResponseHeader("Content-Type").split(";")[0] == "application/json")
        {
            var a = JSON.parse(req.responseText);
            switch (a.ResponseType)
            {
                case -1: //undefined
                    break;
                case 0: //info
                    break;
                case 1: //error
                    var e = "";
                    switch (a.ErrorType)
                    {
                        case -1:
                            e = "Undefined";
                            break;
                        case 0:
                            e = "Captcha";
                            break;
                        case 1:
                            e = "FileSize [" + a.ErrorMessage + "]";
                            break;
                        case 2:
                            e = "BlankPost";
                            break;
                        case 3:
                            e = "Spam";
                            break;
                        case 4:
                            e = "ImpersonationProtection [" + a.ErrorMessage + "]";
                            break;
                        case 5:
                            e = "FileRequired";
                            break;
                        case 6:
                            e = "Banned";
                            break;
                        case 7:
                            e = "ServerError [" + a.ErrorMessage + "]";
                            break;
                        case 8:
                            e = "InvalidRequest";
                            break;
                        default:
                            e = "Undefined";
                            break;
                    }
                    qr_display_message("Cannot post: " + e, "error");
                    break;
                case 2: //newthread
                    open_newtab_thread(a.ThreadID);
                    break;
                case 3: //reply
                    /* if get_current_tid == e.ThreadID then update else open new tab */
                    if (get_thread_id() == a.ThreadID)
                    {
                        forceUpdate();
                    }
                    else
                    {
                        open_newtab_thread(a.ThreadID, a.PostID);
                    }
                    break;
                default:
                    break;
            }
            //console.log(a);
        }
        else
        {
            console.log("resp: " + req.responseType);
            console.log(req.responseText);
        }
        req = null;
        qr("clear");
        //ui stuffs
        $("#progress-block").slideUp();
        qr_enable_controls();
        //force new update
        forceUpdate();
        //allow new quick reply submissions.
        qr_isSending = false;
    }
    var formdata = new FormData(qrForm);
    req.send(formdata);
    qr_disable_controls();
}

function open_newtab_thread(tid, postid) {
    var url = (postid != null) ? webroot + "?id=" + tid + "#p" + postid : webroot + "?id=" + tid;
    window.open(url, "_blank");
}

function qr_display_message(text, level) {
    alert(text);
    //document.getElementById("qr_message_content").textContent = text;
    //    var e = gid("qr_message");
    //    switch (level) {
    //        case "error":
    //            e.setAttribute("style", "style=\"background-color: #FF0000!important; color:#FFFFFF\"");
    //            break;
    //        case "info":
    //            e.setAttribute("style", "style=\"background-color: #00FF00!important; color:#FFFFFF\"");
    //            break;
    //        default:
    //            e.setAttribute("style", "style=\"background-color: #0000FF!important; color:#FFFFFF\"");
    //            break;
    //    }
    //    e.setAttribute("class", "");
}

function qr_updateProgress(e) {
    if (e.loaded >= e.total)
    {
        gid("download_perc").textContent = "100 %";
        gid("download_progress").setAttribute("style", "width: 100%");
    }
    else
    {
        var pro = (0 | (e.loaded / e.total * 100)) + " %";
        gid("download_perc").textContent = pro;
        gid("download_progress").setAttribute("style", "width: " + pro);
    }
}

var qr_controls = ["qr_name", "qr_email", "qr_subject", "qr_comment", "qr_send", "qr_reset"]

function qr_disable_controls() {
    for (i = 0; i < qr_controls.length; i++)
    {
        gid(qr_controls[i]).setAttribute("disabled", "disabled");
    }
}

function qr_enable_controls() {
    for (i = 0; i < qr_controls.length; i++)
    {
        gid(qr_controls[i]).removeAttribute("disabled");
    }
}

function qr_hasFiles() {
    var qrF = gid("qr_files");
    if (qrF.children.length = 0)
    {
        return false;
    }
    else
    {
        var fileFound = false;
        for (i = 0; i < qrF.children.length; i++)
        {
            if (!(qrF.children[i].value == ""))
            {
                fileFound = true;
            }
        }
        return fileFound;
    }
}
//===============================================================
// StringBuilder class
// Initializes a new instance of the StringBuilder class
// and appends the given value if supplied
function StringBuilder(value) {
    this.strings = new Array("");
    this.append(value);
}
// Appends the given value to the end of this instance.
StringBuilder.prototype.append = function (value) {
    if (value)
    {
        this.strings.push(value);
    }
}
// Clears the string buffer
StringBuilder.prototype.clear = function () {
    this.strings.length = 1;
}
// Converts this instance to a String.
StringBuilder.prototype.toString = function () {
    return this.strings.join("");
}

function load_settings() {

    if (SM)
    {
        check_setting("modal_dialogs", "true");
        check_setting("modal_images", "true");
        check_setting("qr", "true");
    }

    function check_setting(name, defaultvalue) { if (SM.GetItem(name) == "") { SM.SetItem(name, defaultvalue); } }

}
 
