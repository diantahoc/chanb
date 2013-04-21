function quote(id) {

    document.getElementById("commentfield").value += ">>" + id;
}


function createUf() {
    var divTag = document.createElement("input");
    divTag.setAttribute("name", "d" + Math.random().toString(10));

    divTag.setAttribute("type", "file");

    document.getElementsByName("form").item(0).appendChild(divTag);

    var br = document.createElement("br");

    document.getElementsByName("form").item(0).appendChild(br);
} 