var viewport;

$(window).on('resize', function () {
    viewport.recalculateHeight();
});

$(window).click(function (e) {
    var $target = $(e.target);

    $(".dynkwl").remove(); 
});

$(window).keydown(function (e) {
    var keyCode = e.keyCode || e.which;

    if (keyCode == 115) { // F4
        e.preventDefault();
        //showModal("metadata", true);
        viewport.openMetadata();
    }
});   

$(window).mouseup(function () {
    if ($(".document-rendering").length)
        $(".splitter-helper").addClass("hidden");
});

$(function () {
    viewport = new Viewport();
    viewport.initialize();

    $(window).trigger("resize");

    /* TABS */

    $(document).on("click", ".menu-bar .nav-tabs li a", function (e) {
        e.stopImmediatePropagation();
        $(".menu-bar .nav-tabs li").removeClass("active");
        $(this).parent().addClass("active");
    });

    $(document).on("click", ".menu-bar .nav-tabs li", function (e) {
        $(this).children().trigger("click");
    });

    $(document).on("click", ".navbar-buttons .tab", function (e) {
        var link = $(e.target).children("a").eq(0).attr("href");
        window.location.href = link;
    });
    
});

function convertDocToHtml() {
    $.ajax({
        url: "/Archive/ConvertDocToHtml",
        type: 'GET',
        dataType: 'json',
        async: false,
        contentType: 'application/json',
        data: {},
        success: function (html) {
            console.log(html);
        }
    });
}




function setContextMenu() {
    $.contextMenu({
        selector: '.jstree-anchor',
        callback: function (key, options) {
            var selected = $('.jstree-clicked').map(function () { console.log(this); return this.text; }).get();
            if (key == "elo") {
                var id = $('.jstree-clicked').parent().attr("objid");
                window.open("elodms://" + id, "_self");
            }
            else if (key == "copy") {
                copyToClipboard(selected.join(";"));
            }

        },
        items: {
            "elo": {
                name: "In ELO öffnen", icon: "edit", disabled: function (key, opt) {
                    return $('.jstree-clicked').length != 1;
                }
            },
            "copy": { name: "Kopieren", icon: "copy" },
            "sep1": "---------",
            "quit": {
                name: "Schließen", icon: function () {
                    return 'context-menu-icon context-menu-icon-quit';
                }
            }
        }
    });
}