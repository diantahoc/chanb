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

    var $active = $(".active." + id );
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

    var $last = $("#rot" + id  +  " div:last-child");
  

    $last.removeClass('notactive');
    $last.addClass('active');
    
 }

function updatemode(f) {

    document.getElementById("ROD").value = f;
}

function extension() {

    var postdiv = document.getElementById("postdiv");
    postdiv.setAttribute("style", "border-style: dashed; border-width: thin; clip: rect(0px, auto, auto, auto); display:inline-block; ");
    $("#postdiv").draggable();

}