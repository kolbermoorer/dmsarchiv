/**
 * Creates a new container for the active page. It holds the different view pages depending on the active view name
 */
function Viewport() {
    this.name = null;
    this.dms = null;
    this.view = null;
    this.docViewer = null;
    this.mask = new Mask();
    this.user = new User();
    this.links = $(".navbar-links a");
    this.sections = {
        "picker":   $(".section-picker"),
        "document": $(".section-document"),
        "feed":     $(".section-feed")
    };
    this.config = null;
    this.metadataForm = $("#modal-metadata");
    
    this.id = null;

    this.selectedItems = [];
}

/**
 * Initialize the view container and sets all necessary components of the view
 */
Viewport.prototype.initialize = function () {
    var me = this;

    me.dms = new DMS();
    me.dms.initialize();

    me.getConfig();
    me.setActivePage();
    me.setActiveMenuTab();

    switch (me.name) {
        case "Archive":
            me.view = new ArchiveView();
            break;
        case "Search":
            me.view = new SearchView();
            break;
        case "Intray":
            me.view = new IntrayView();
            break;
        default:
            throw "Ansicht existiert nicht";
    }

    me.splitter = me.setSplitter();
    me.view.initialize();

    me.docViewer = new DocumentViewer();
    me.docViewer.initialize();

    $(".datepicker").datepicker({
        showOtherMonths: true,
        selectOtherMonths: true,
        dateFormat: "dd.mm.yy" 
    });
    $(document).on("click", ".fa-calendar-alt", function (e) { $(e.target).prev().datepicker("show"); });

    me.selectedItems = [];
};

Viewport.prototype.setActiveMenuTab = function () {
    var me = this;

    var tab = $("a[data-link='" + me.name + "']");
    if(!tab.length)
        tab = $("a[data-link='Default']");

    var id = tab.attr("href");
    $(id).addClass("in active show");
    tab.parent().addClass("active");
};

/**
 * Set the name of the active view
 */
Viewport.prototype.setActivePage = function () {
    var me = this;

    $.each(me.links, function (i, link) {
        var href = $(link).prop("href");
        if (window.location.href.search(href) > -1) {
            $(link).parent().addClass("active");
            me.name = $(link).data("name");
        }
    });

    var url = window.location.pathname;
    var id = url.substring(url.lastIndexOf('/') + 1);
    if (id !== me.name)
        me.id = id;
};

/**
 * Initialize the splitter of the view. Right side is always the document viewer, left side depends on the active view.
 */
Viewport.prototype.setSplitter = function () {
    var me = this;

    var splitter = $('.panel-container').height(200).split({
        orientation: 'vertical',
        position: me.name == "Archive" ? "30%" : "35%",
        onDrag: function (event) {
            if ($(".document-rendering").length)
                $(".splitter-helper").removeClass("hidden");
        }
    });
    return splitter;
};

/**
 * Get the config of the DMS system (mask names, line numbers, etc.) - loaded only once at the start of the website and cached afterwards
 */
Viewport.prototype.getConfig = function () {
    var me = this;

    $.get("/View/GetConfig", function (config) {
        me.config = config;

        me.fillMaskList();
    });
};


/**
 * Recalculate the height of the different sections in the viewport
 */
Viewport.prototype.recalculateHeight = function () {
    $(".panel-container").height($("body").height() - $("nav").height() - $(".tabs").height() - 2);
    $(".document-container").height($("body").height() - $("nav").height() - $(".document-menu-bar").height() - $(".tabs").height() - 2);

    this.view.recalculateHeight();
};

Viewport.prototype.getSection = function (section) {
    return this.sections[section];
};

/* Show the document in the viewer */

Viewport.prototype.showDocument = function (id, type, path = null) {
    var me = this;

    var params = $.param({
        id: id,
        type: type,
        path: path
    });

    $(".items-list-table").addClass("hidden");
    $(".document-container").html('<embed id="document-rendering" src="/View/LoadDocument?' + params + '" width="100%" height="100%"/>');
};

Viewport.prototype.showDocumentList = function (id) {
    var me = this;

    $("#document-rendering").remove();
    me.getItems(id);
};

Viewport.prototype.getItems = function (id) {
    var me = this;

    $.ajax({
        url: "/Archive/GetItemsByParentId",
        async: false,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: { id: id },
        success: function (items) {
            me.fillItemList(items);
        }
    });
};

Viewport.prototype.fillItemList = function (files, scope = $(".items-list-table")) {
    var me = this;

    scope.removeClass("hidden");

    var html = [];

    $.each(files, function (i) {
        var img = '<img src="' + this.icon + '"/>';
        html.push('<tr data-index="' + i + '"><td><div>' + img + '<span class="name-index">' + this.name + '</span></div><div class="second-row"><span class="editor-index">Thomas</span><span class="date-index">' + moment(this.objcreatedate).format("DD.MM.YYYY") + '</span><span class="mask-index">' + this.objmask + '</span></div></td></tr>');
    });

    scope.find("tbody").html(html.join(''));
    scope.closest(".document-container").addClass("list");
};



/**
 * Fill the masks in the metadata mask list
 */
Viewport.prototype.fillMaskList = function () {
    var me = this;
    var $ul = $(".metadata-container-masklist ul");
    $.each(me.config.masks, function() {
        $ul.append('<li>' + this.maskname + '</li>').data("fields", me.config.fields);

        if (this.maskname == "Chaosablage")
            me.mask.activeMask = this;
    });
};


Viewport.prototype.openMetadata = function (exists = true) {
    if(exists)
        this.view.fillMetadata();
    $("#modal-metadata").modal("show").draggable();
};

Viewport.prototype.showNotifyMessage = function (message, type = "success") {
    $.notify({
        message: message
    }, {
            type: type,
            placement: {
                from: "top",
                align: "center"
            },
            delay: 3000
    });
};