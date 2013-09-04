function quote(id) {
    if (qr_isHidden) {
        qr("show");
    }
    insert(document.getElementById("qr_comment"), ">>" + id + "\n");
}

function insert(textarea, text) {
    if (!textarea) return;

    if (textarea.createTextRange && textarea.caretPos) {
        var caretPos = textarea.caretPos;
        caretPos.text = caretPos.text.charAt(caretPos.text.length - 1) == " " ? text + " " : text;
    } else if (textarea.setSelectionRange) {
        var start = textarea.selectionStart;
        var end = textarea.selectionEnd;
        textarea.value = textarea.value.substr(0, start) + text + textarea.value.substr(end);
        textarea.setSelectionRange(start + text.length, start + text.length);
    } else {
        textarea.value += text + " ";
    }
    textarea.focus();
}

function addUf(id) {
    var cont = document.getElementById(id);
    
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
    delbutton.setAttribute("class", "buttonBlue bbf");

    var br = document.createElement("br");
    br.setAttribute("id", "br" + idstr);

    cont.appendChild(br);
    cont.appendChild(divTag);
    cont.appendChild(delbutton);

}

function delUf(id) {

    var parent = document.getElementById("delb"+id).parentElement;
    
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

function refreshcaptcha(level) {
    if (level == null) {
        var cap = document.getElementById("captchaImage");
        cap.setAttribute("src", webroot + "captcha.aspx?" + Math.random().toString(10));
    } else {
        var cap = document.getElementById("captchaImage");
        cap.setAttribute("src", webroot + "captcha.aspx?l=" + level + "&y=" + Math.random().toString(10));
    }
    document.getElementById("usercaptcha").value = "";
}

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



function handle_pbox(e) {
    switch (e.type) {
        case "mouseover":
            e.target.setAttribute("type", "text");
            break;
        case "mouseout":
            e.target.setAttribute("type", "password");
            break;
        default:
            break;
    }
}

function show_hide_pbox() {
    var items = document.getElementsByTagName("input");
    for (a = 0; a < items.length; a++) {
        if (items[a].type == "password") {
            items[a].addEventListener('mouseover', handle_pbox, false);
            items[a].addEventListener('mouseout', handle_pbox, false);       
        }
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

var qr_isHidden = false;
function qr(command) {
    var qbox = $("#qrbox");
    switch (command) {
        case "hide":
            if (!qr_isHidden) {
                qbox.hide();
                qr_isHidden = true;
            }
            qr("clear");
            break;
        case "show":
            if (qr_isHidden) {
                qbox.show();
                qr_isHidden = false;
            }
            break;
        case "clear":
            var oaw = ["qr_name", "qr_email", "qr_subject", "qr_comment"];
            for (i = 0; i < oaw.length; i++) {
                document.getElementById(oaw[i]).value = "";
            }
            var qr_fil = document.getElementById("qr_files");
            var gee = qr_fil.children;
            for (i = 0; i < gee.length; i++) {
                if (!(gee[i].getAttribute("name") == "ufile")) {
                    qr_fil.removeChild(gee[i]);
                }
            }

            break;
        case "submit":
            qr_send(document.getElementById("qr_form"));
            break;
        default:
            break;
    }
}
function qr_init() {
    var qbox = $("#qrbox");
    qbox.draggable({ appendTo: "#qrtitle", containment: "document" });
    qbox.removeClass("hide");
    qbox.hide();
    qr_isHidden = true;
    $("#progress-block").hide();
}

var qr_isSending = false;

function qr_send(qrForm) {
    if (qr_isSending) {
        return null;
    }
    qr_isSending = true;
    
    //UI Stuffs
    if (qr_hasFiles()) { $("#progress-block").slideDown(); }
   
    
    var req = new XMLHttpRequest();
    req.open('POST', webroot + "api.aspx", true);
    req.withCredentials = true;
    req.upload.onprogress = function(e) { qr_updateProgress(e); }

    req.onerror = function() { req = null; qr_enable_controls(); }

    req.onloadend = function() {
        if (req.getResponseHeader("Content-Type").split(";")[0] == "application/json") {

            var a = JSON.parse(req.responseText);


            switch (a.ResponseType) {


                case -1: //undefined
                    break;
                case 0: //info
                    break;
                case 1: //error
                    var e = "";
                    switch (a.ErrorType) {
                        case -1:
                            e = "Undefined";
                            break;
                        case 0:
                            e = "Captcha";
                            break;
                        case 1:
                            e = "FileSize";
                            break;
                        case 2:
                            e = "BlankPost";
                            break;
                        case 3:
                            e = "Spam";
                            break;
                        case 4:
                            e = "ImpersonationProtection";
                            break;
                        case 5:
                            e = "FileRequired";
                            break;
                        case 6:
                            e = "Banned";
                            break;
                        case 7:
                            e = "ServerError";
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
                    break;
                case 3: //reply
                    break;
                default:
                    break;
            }

             console.log(a);


        } else {
            console.log("resp: " + req.responseType);
            console.log(req.responseText);
        }

        req = null;
        //ui stuffs
        $("#progress-block").slideUp();
        qr_enable_controls();
        //force new update
        noNewRepliesIncrem = -1;
        qr("clear");
        //allow new quick reply submissions.
        qr_isSending = false;




    }
   
    var formdata = new FormData(qrForm);
    req.send(formdata);
    qr_disable_controls();
}

function qr_display_message(text, level) {
    //

    document.getElementById("qr_message_content").textContent = text;
    var e = document.getElementById("qr_message");
    switch (level) {
        case "error":
            e.setAttribute("style", "style=\"background-color: #FF0000!important; color:#FFFFFF\"");break;
        case "info":
            e.setAttribute("style", "style=\"background-color: #00FF00!important; color:#FFFFFF\""); break;
        default:
            e.setAttribute("style", "style=\"background-color: #0000FF!important; color:#FFFFFF\""); break;
    }
    e.setAttribute("class", "");
 }

function qr_updateProgress(e) {
    if (e.loaded >= e.total) {
        document.getElementById("download_perc").textContent = "100%";
        document.getElementById("download_progress").setAttribute("style","width: 100%");      
    }
    else {
        var pro = (0 | (e.loaded / e.total * 100)) + '%';
        document.getElementById("download_perc").textContent = pro;
        document.getElementById("download_progress").setAttribute("style", "width: " + pro); 
    }
}

var qr_controls = ["qr_name", "qr_email", "qr_subject", "qr_comment", "qr_send", "qr_reset"]

function qr_disable_controls() {
    for (i = 0; i < qr_controls.length; i++) {
        document.getElementById(qr_controls[i]).setAttribute("disabled", "disabled");
     }
 }

 function qr_enable_controls() {
     for (i = 0; i < qr_controls.length; i++) {
         document.getElementById(qr_controls[i]).removeAttribute("disabled");
     }
 }

 function qr_hasFiles() {

     var qrF = document.getElementById("qr_files");
     if (qrF.children.length = 0) { return false; } else {

     var fileFound = false;
         for (i = 0; i < qrF.children.length; i++) {
             if (! (qrF.children[i].value == "")) {fileFound = true; }
          }
          return fileFound;
      }

  }

//var c_pl= new Array();
//  function lp(a) {
//      if (c_pl.indexOf(a) == -1) {
//          c_pl.push(a);
//          var script = document.createElement('script');
//          script.type = 'text/javascript';
//          script.src = webroot + 'js/pr/lang-' + a +'.js';
//          document.getElementsByTagName('head')[0].appendChild(script);   
//      }
//}
//   