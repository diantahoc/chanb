function quote(idd, tid) {
    qr("open",tid);
    insert(gid("qr_comment"), ">>" + idd + "\n");
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

function getUfCount(a) {
    var filesInputs = gid(a).getElementsByTagName("input");
    //check if the browser support the files attribute for <file> input
    if (filesInputs[0].files) {
        var sum = 0;
        for (i = 0; i < filesInputs.length; i++) {
            if (filesInputs[i].files) {
                for (f = 0; f < filesInputs[i].files.length; f++) {
                    if (filesInputs[i].files[f]) {
                        sum += filesInputs[i].files[f].size;
                    };
                } 
            }
        }
        return sum;
    }
    else {
        return -3;
    }
}

function addUf(id) {
    var sum = getUfCount(id);
    if (sum == -3) {
        //browser does not support files attribute for <file> input. Assume all files have equal size defined by maxFileLength
        var maximum_file_per_post = (maxHttpLength / maxFileLength).toFixed(0);
        if (gid(id).getElementsByTagName("input").length > maximum_file_per_post) {
            return;
        }
    }
    else {
        if (sum > maxHttpLength) {
            return;
        }
    }
    
    var cont = gid(id);
    
    var inputElem = document.createElement("input");
    inputElem.setAttribute("name", "file" + Math.random().toString(10));
    inputElem.setAttribute("type", "file");
    
    var delbutton = document.createElement("input");
    delbutton.setAttribute("type", "button");
    delbutton.setAttribute("value", "X");
    delbutton.setAttribute("class", "buttonBlue bbf");

    var br = document.createElement("br");

    delbutton.onclick = function() {
        cont.removeChild(inputElem);
        cont.removeChild(delbutton);
        cont.removeChild(br);
    };
 
    cont.appendChild(br);
    cont.appendChild(inputElem);
    cont.appendChild(delbutton);
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
    $(".active." + id).removeClass('active').addClass('notactive');
    $("#rot" + id + " div:last-child").removeClass('notactive').addClass('active');
}

function refreshcaptcha(level, id) {
    var iad = "captchaImage";
    if (id != null) { iad = id }
    if (level == null) {
        var cap = gid(iad);
        cap.setAttribute("src", webroot + "captcha.aspx?" + Math.random().toString(10));
    } else {
    var cap = gid(iad);
        cap.setAttribute("src", webroot + "captcha.aspx?l=" + level + "&y=" + Math.random().toString(10));
    }
    gid("usercaptcha").value = "";
}

/*function updateAttrb(id, name, value) {
    document.getElementById(id).setAttribute(name, value);
}

function getAttrb(id, name) {
    return document.getElementById(id).getAttribute(name).toString();
}*/

function handle_pbox(e) {
    switch (e.type) {
        case "mouseover":
            e.target.setAttribute("type", "text");
            break;
        case "mouseout":
            e.target.setAttribute("type", "password");
            break;
        default:
            e.target.setAttribute("type", "password");
            break;
    }
}

function show_hide_pbox() {
    var items = tags("input");
    for (a = 0; a < items.length; a++) {
        if (items[a].type == "password") {
            items[a].onmouseover = handle_pbox;
            items[a].onmouseout = handle_pbox;}
    }
}

function switch_css(css_title) {
      var i, link_tag;
      for (i = 0, link_tag = tags("link"); i < link_tag.length; i++) {
          if ((link_tag[i].rel.indexOf("stylesheet") != -1) &&
      link_tag[i].title) {
              link_tag[i].disabled = true;
              if (link_tag[i].title == css_title) {
                  link_tag[i].disabled = false;
              }
          }
          SM.SetItem("selected_style", css_title);
      }
}

function loadStyle() {
      var css_title = SM.GetItem("selected_style");
      if (css_title.length > 0) {
          switch_css(css_title);
      }
  }

function gid(s){return document.getElementById(s);}
function clas(a) {return document.getElementsByClassName(a)}
function tags(a) { return document.getElementsByTagName(a) }

//Settings manager.
var SM = new SettingsManager();

function SettingsManager() {
    this.getCookie = function(name) {
        with (document.cookie) {
            var regexp = new RegExp("(^|;\\s+)" + name + "=(.*?)(;|$)");
            var hit = regexp.exec(document.cookie);
            if (hit && hit.length > 2) return decodeURIComponent(hit[2]);
            else return "";
        } 
    }
    this.setCookie = function(c_name, value, exdays) {
        var exdate = new Date();
        exdate.setDate(exdate.getDate() + exdays);
        var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
        document.cookie = c_name + "=" + c_value;
    }
    this.GetItem = function(name) {
        if (window.localstorage) {
            return window.localstorage.getItem(name);
        }
        else {
            if (this.getCookie(name) != '') {
                return this.getCookie(name);
            }
            else {
                return "";
            }
        }
    }
    this.SetItem = function(name, value) {
        if (window.localstorage) {
            return window.localstorage.setItem(name, value);
        }
        else {
            return this.setCookie(name, value, 15);
        }
    }
}