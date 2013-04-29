function quote(id) {

    document.getElementById("commentfield").value += ">>" + id;
}


function createUf() {
    var divTag = document.createElement("input");
    divTag.setAttribute("name", "d" + Math.random().toString(10));

    divTag.setAttribute("type", "file");
    document.getElementById("files").appendChild(divTag);

    var br = document.createElement("br");
    document.getElementById("files").appendChild(br);
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