function checkforNSFWImages() {
    var items = $(".cimage");
    for (i = 0; i < items.length; i++) {
        var id = items[i].getAttribute('id').toString()
        var nude = getNudeObject();
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

                var myWorker = new Worker( webroot + 'js/worker.nude.js'),
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
