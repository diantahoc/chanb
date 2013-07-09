function quote(id) {
    document.getElementById("commentfield").value += ">>" + id + "\n";
}

function createUf() {
    var divTag = document.createElement("input");

    var idstr = "d" + Math.random().toString(10);

    divTag.setAttribute("name", idstr);
    divTag.setAttribute("id", "uf" + idstr);
    divTag.setAttribute("type", "file");

    var delbutton = document.createElement("input");

    delbutton.setAttribute("onclick", "javascript:delUf('" + idstr + "')");
    delbutton.setAttribute("type", "button");
    delbutton.setAttribute("value", "X");
    delbutton.setAttribute("id", "delb" + idstr);


    var br = document.createElement("br");
    br.setAttribute("id", "br" + idstr);

    document.getElementById("files").appendChild(br);
    document.getElementById("files").appendChild(divTag);
    document.getElementById("files").appendChild(delbutton);

}

function delUf(id) {

    var parent = document.getElementById("files");
    var a = document.getElementById("uf" + id);
    var b = document.getElementById("delb" + id);
    var c = document.getElementById("br" + id);

    parent.removeChild(a);
    parent.removeChild(b);
    parent.removeChild(c);
}


function goleft(id) {

    var $active = $(".active." + id);
    var $next = $active.prev().length ? $active.prev() : $("." + id + ".focusitem:last");
    $active.removeClass('active');
    $active.addClass('notactive');
    $next.addClass('active');
    $next.removeClass('notactive');
}

function goright(id) {

    var $active = $(".active." + id);
    var $next = $active.next().length ? $active.next() : $("." + id + ".focusitem:first");
    $active.removeClass('active');
    $active.addClass('notactive');
    $next.addClass('active');
    $next.removeClass('notactive');
}

function gofirst(id) {

    var $active = $(".active." + id);
    $active.removeClass('active');
    $active.addClass('notactive');
    var $last = $("#rot" + id + " div:first-child");
    $last.removeClass('notactive');
    $last.addClass('active');
}

function golast(id) {

    var $active = $(".active." + id);
    $active.removeClass('active');
    $active.addClass('notactive');
    var $last = $("#rot" + id + " div:last-child");
    $last.removeClass('notactive');
    $last.addClass('active');
}

function timer() {

    $(document).ready(function() {
        window.setInterval("fetchnewreplies()", 15000);
    });
}

function fetchnewreplies() {

    var $threadDiv = $(".thread:first");
    var threadID = $threadDiv.attr('id');
    var lastpostID = $(".postContainer:last").attr('id');
    $.get(
        "api.aspx", {
            mode: 'fetchrepliesafter',
            tid: threadID,
            lp: lastpostID
        },
        function(data) {

            //process here.
            if (!data.toString().length == 0) {
                $threadDiv.append(data);
            }
        }
    );

}

function refreshcaptcha(level) {

    if (level == null) {
        var cap = document.getElementById("captchaImage");
        cap.setAttribute("src", "/captcha.aspx?" + Math.random().toString(10));
    } else {
        var cap = document.getElementById("captchaImage");
        cap.setAttribute("src", "/captcha.aspx?l=" + level + "&y=" + Math.random().toString(10));
    }
    document.getElementById("usercaptcha").value = "";
}

//function focusRCB() {
//    document.getElementById('refreshcaptchabutton').setAttribute('src', '/res/refresh-high.png');
//}

//function unfocusRCB() {
//    document.getElementById('refreshcaptchabutton').setAttribute('src', '/res/refresh.png');
//}

function updateAttrb(id, name, value) {
    document.getElementById(id).setAttribute(name, value);
}

function getAttrb(id, name) {
    return document.getElementById(id).getAttribute(name).toString();
}

function showFullName(id) {
    $("#fsn" + id).addClass("hide");
    $("#ffn" + id).removeClass("hide");
}

function showShortName(id) {
    $("#ffn" + id).addClass("hide");
    $("#fsn" + id).removeClass("hide");
}

function showPassword(id) {
    document.getElementById(id).setAttribute("type", "text");
}

function hidePassword(id) {
    document.getElementById(id).setAttribute("type", "password");
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

function checkforNSFWImages() {

    var items = $(".cimage");

    for (i = 0; i < items.length; i++) {

        var id = items[i].getAttribute('id').toString()

        var nude = new getNudeObject();

        nude.load(id);

        nude.scan();

    }

}

function getNudeObject() {


    var nude = new (function() {
        // private var definition
        var imageID;
        var canvasId = Math.random().toString(10);
        var canvas = null,
            ctx = null,
            resultFn = null,
        // private functions
            initCanvas = function() {
                canvas = document.createElement("canvas");
                // the canvas should not be visible
                canvas.style.display = "none";
                canvas.setAttribute("id", canvasId);
                var b = document.getElementsByTagName("body")[0];
                b.appendChild(canvas);
                ctx = document.getElementById(canvasId).getContext("2d");
            },
            loadImageById = function(id) {
                // get the image
                imageID = id;
                var img = document.getElementById(id);
                // apply the width and height to the canvas element
                document.getElementById(canvasId).width = img.width;
                document.getElementById(canvasId).height = img.height;
                // reset the result function
                resultFn = null;
                // draw the image into the canvas element
                ctx.drawImage(img, 0, 0);

            },
            loadImageByElement = function(element) {
                // apply width and height to the canvas element
                // make sure you set width and height at the element
                canvas.width = element.width;
                canvas.height = element.height;
                // reset result function
                resultFn = null;
                // draw the image/video element into the canvas
                ctx.drawImage(element, 0, 0);
            },
            scanImage = function() {
                // get the image data
                var image = ctx.getImageData(0, 0, canvas.width, canvas.height),
                    imageData = image.data;

                var myWorker = new Worker('js/worker.nude.js'),
                    message = [imageData, document.getElementById(canvasId).width, document.getElementById(canvasId).height];
                myWorker.postMessage(message);
                myWorker.onmessage = function(event) {
                    resultHandler(event.data);
                }
            },
            cleanup = function() {
                // destroy the image canvas
                var b = document.getElementsByTagName("body")[0];
                b.removeChild(document.getElementById(canvasId));
                canvas = null;
            },
        // the result handler will be executed when the analysing process is done
        // the result contains true (it is nude) or false (it is not nude)
        // if the user passed an result function to the scan function, the result function will be executed
            resultHandler = function(result) {

                if (resultFn) {
                    resultFn(result);
                } else {
                    if (result) {
                        //  Pixastic.process(image, "blurfast", { amount: 1.0 });
                        $("#" + imageID).addClass("blur");
                    }
                }
                cleanup();
            }
        // public interface
        return {
            init: function() {
                initCanvas();
                // if web worker are not supported, append the noworker script
                if (!!!window.Worker) {
                    document.write(unescape("%3Cscript src='js/noworker.nude.js' type='text/javascript'%3E%3C/script%3E"));
                }

            },
            load: function(param) {
                if (typeof (param) == "string") {
                    loadImageById(param);
                } else {
                    loadImageByElement(param);
                }
            },
            scan: function(fn) {
                if (arguments.length > 0 && typeof (arguments[0]) == "function") {
                    resultFn = fn;
                }
                scanImage();
            }
        };
    })();
    nude.init();
    return nude;
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

function snas() {
    $("#pname").attr("value", getCookie("postername"));
    $("#pemail").attr("value", getCookie("posteremail"));
    if (getCookie("posterpass") == "") { setCookie("posterpass", $("#formps").attr("value"), 3); } else { $("#formps").attr("value", getCookie("posterpass")); $("#formdelP").attr("value", getCookie("posterpass")); }

    var WDLitems = $(".wdlink");
    for (i = 0; i < WDLitems.length; i++) {
        wdLink($(WDLitems[i]));
     }
}

function getCookie(name) {
    with (document.cookie) {
        var regexp = new RegExp("(^|;\\s+)" + name + "=(.*?)(;|$)");
        var hit = regexp.exec(document.cookie);
        if (hit && hit.length > 2) return decodeURIComponent(hit[2]);
        else return '';
    }
}

function setCookie(c_name, value, exdays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
    document.cookie = c_name + "=" + c_value;
}

function beforePost() {
    setCookie("postername", $("#pname").val(), 3);
    setCookie("posterpass", $("#formps").val(), 3);
    setCookie("posteremail", $("#pemail").val(), 3);
}


function wdLink(anchor) {
    var link = anchor.attr("href").toString();
    var newlink = "javascript:openWindow('" + link + "','" + anchor.text() + "')";

    
    anchor.attr("href", newlink);
    anchor.removeAttr("target");
    
}

function openWindow(link,title) { 
    var width  = 750;
	var height = 250;
	var left   = (screen.width  - width)/2;
	var top    = (screen.height - height)/2;
	var params = 'width='+width+', height='+height+', top='+top+', left='+left+', directories=no, location=no, menubar=no, resizable=no, scrollbars=no, status=yes, toolbar=no';
	newwin=window.open(link,title, params);
	if (window.focus) {
		newwin.focus()
	}
}
