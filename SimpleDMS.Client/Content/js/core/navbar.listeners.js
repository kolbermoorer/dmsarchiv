$(document).on("click", "#button-addFolder", function () {
    viewport.openMetadata(false);
});

$(document).on("click", "#button-goToLastElement", function () {
    var guid = viewport.view.selectedItem[0].guid;
    window.location.href = "/Archive/" + guid;
});

$(document).on("click", "#button-openMetadata", function () {
    viewport.openMetadata();
});

$(document).on("click", "#button-removeMetadata", function () {
    //TODO
});

$(document).on("click", "#button-addFulltext", function () {
    viewport.dms.addDocumentToFulltext();
});

$(document).on("click", "#button-removeFulltext", function () {
    viewport.dms.removeDocumentFromFulltext();
});

/* INTRAY */

$(document).on("click", "#button-addFile", function () {
    viewport.view.dropzone.zone.find('input').trigger("click");
});


$(document).on("click", "#button-mergeDocuments", function () {
    viewport.dms.mergeDocuments();
});


$(document).on("click", "#button-splitDocuments", function () {
    viewport.dms.splitDocuments();
});

$(document).on("click", "#button-deleteDocuments", function () {
    viewport.dms.deleteDocuments();
});

/* FULLSCREEN */

var isFullscreen = false;

document.addEventListener("fullscreenchange", onFullScreenChange, false);
document.addEventListener("webkitfullscreenchange", onFullScreenChange, false);
document.addEventListener("mozfullscreenchange", onFullScreenChange, false);

function onFullScreenChange() {
    isFullscreen = !isFullscreen;
}

//fullscreen Button
$(document).on('click', '.toggleFullscreen', function () {
    var elem = document.documentElement;
    if (isFullscreen) {
        cancelFullScreen(document);
    } else {
        requestFullScreen(elem);
    }

    return false;
});

function cancelFullScreen(el) {
    var requestMethod = el.cancelFullScreen || el.webkitExitFullscreen || el.mozCancelFullScreen || el.exitFullscreen || el.msExitFullscreen;
    if (requestMethod) { // cancel full screen.
        requestMethod.call(el);
    } else if (typeof window.ActiveXObject !== "undefined") { // Older IE.
        var wscript = new ActiveXObject("WScript.Shell");
        if (wscript !== null) {
            wscript.SendKeys("{F11}");
        }
    }
}

function requestFullScreen(el) {
    // Supports most browsers and their versions.
    var requestMethod = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullscreen;

    if (requestMethod) { // Native full screen.
        requestMethod.call(el);
    } else if (typeof window.ActiveXObject !== "undefined") { // Older IE.
        var wscript = new ActiveXObject("WScript.Shell");
        if (wscript !== null) {
            wscript.SendKeys("{F11}");
        }
    }
    return false
}

/** 
 * detect IE
 * returns version of IE or false, if browser is not Internet Explorer
 */
function detectIE() {
    var ua = window.navigator.userAgent;

    var msie = ua.indexOf('MSIE ');
    if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }

    var trident = ua.indexOf('Trident/');
    if (trident > 0) {
        // IE 11 => return version number
        var rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }

    var edge = ua.indexOf('Edge/');
    if (edge > 0) {
        // Edge (IE 12+) => return version number
        return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
    }

    // other browser
    return false;
}