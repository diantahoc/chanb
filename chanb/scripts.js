function quote(id) {
    insert(document.getElementById("commentfield"), ">>" + id + "\n");
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





//function initQR() { 

///*<div id="qrbox" class="qr">
//    <div id="qrtitle" class="qrtitle">
//		<span class="qrmove">Quick reply</span><a style="float:right;" class="form-button form-button-red" href="javascript:qr('hide')">X</a>
//	</div>
//    <div id="qrbody" class="qrbody">
//        <input class="form-text" id="qrname" type="text" style="max-width: 135px" />
//        <input class="form-text" id="qremail" type="text" style="max-width: 135px" />
//        <input class="form-text" id="qrsubject" type="text" style="max-width: 135px" />
//        
//        <br />
//        
//        <textarea id="qrtext" cols="50" rows="10" class="form-textarea"></textarea>
//        
//        <br />
//        
//        <a class="form-button" onclick="qr('sumbit')">Send</a>
//	</div>
//</div>*/


//    var qrbox = document.createElement("div");
//    qrbox.setAttribute("id", "qrbox");
//    qrbox.setAttribute("class", "qr");

//    var qrtitle = document.createElement("div");

//    qrtitle.setAttribute("id", "qrtitle");
//    qrtitle.setAttribute("class", "qrtitle");

//    var spanqrmove = document.createElement("span");
//    spanqrmove.setAttribute("class", "qrmove");
//    spanqrmove.text = "Quick Reply";

//    var qrhideB = document.createElement("a");
//    qrhideB.setAttribute("class", "form-button form-button-red");
//    qrhideB.setAttribute("style", "float:right;");
//    qrhideB.setAttribute("href", "javascript:qr('hide')");
//    qrhideB.text = "X";

//    qrtitle.appendChild(spanqrmove);
//    qrtitle.appendChild(qrhideB);

//    var br = document.createElement("br");
//    
//    var qrbody = document.createElement("div");
//    qrbody.setAttribute("id", "qrbody");
//    qrbody.setAttribute("class", "qrbody");

//    var qrname = document.createElement("input");
//    qrname.setAttribute("id", "qrname");
//    qrname.setAttribute("class", "form-text");
//    qrname.setAttribute("type", "text");
//    qrname.setAttribute("style", "max-width: 135px");
//    qrbody.appendChild(qrname);
//    
//    var qremail = document.createElement("input");
//    qremail.setAttribute("id", "qremail");
//    qremail.setAttribute("class", "form-text");
//    qremail.setAttribute("type", "text");
//    qremail.setAttribute("style", "max-width: 135px");
//    qrbody.appendChild(qremail);
//    
//    var qrsubject = document.createElement("input");
//    qrsubject.setAttribute("id", "qrsubject");
//    qrsubject.setAttribute("class", "form-text");
//    qrsubject.setAttribute("type", "text");
//    qrsubject.setAttribute("style", "max-width: 135px");
//    qrbody.appendChild(qrsubject);

//    qrbody.appendChild(br);
//    
//    
//    var qrtext = document.createElement("textarea");
//    qrtext.setAttribute("id", "qrtext");
//    qrtext.setAttribute("cols", "50");
//    qrtext.setAttribute("rows", "10");
//    qrtext.setAttribute("class", "form-textarea");
//    qrbody.appendChild(qrtext);
//    
//    qrbody.appendChild(br);

//    var qrsendB = document.createElement("a");
//    qrsendB.setAttribute("class", "form-button");
//    qrsendB.setAttribute("href", "javascript:qr('send')");
//    qrsendB.text = "Reply";
//    qrbody.appendChild(qrsendB);
//    
//    qrbox.appendChild(qrtitle);
//    qrbox.appendChild(qrbody);

//    var body = $(".thread")[0];
//    
//    body.appendChild(qrbox);
//    
//    $("#qrbox").draggable({ handle: "#qrtitle" });
//}

//function qr(command) { }


