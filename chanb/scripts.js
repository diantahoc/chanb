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