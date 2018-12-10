function SearchView() {
    this.table = $("#tbl-search-files");
    this.container = $(".search-container");
    this.files = [];
}

SearchView.prototype.initialize = function () {
    var me = this;
    $(document).on("keyup", ".input-search", function (e) { if (e.keyCode == 13) me.executeSearch(); });
    $(document).on("click", ".search-container .fa-search:not(.fa-times-circle)", function () { me.executeSearch(); });
    $(document).on("click", ".search-container .fa-times-circle", function () { me.emptySearch(); });
    $(document).on("click", "#tbl-search-files tr", function (e) { me.showDocument(me, e) });
};

SearchView.prototype.executeSearch = function () {
    var me = this;

    var searchVal = $(".input-search").val();

    this.table.addClass("hidden");
    $(".search-wait-animation").removeClass("hidden");
    me.toggleSearchIcon(true);

    $.ajax({
        url: "/Search/Execute",
        contentType: 'application/json',
        type: "GET",
        data: {
            val: searchVal
        },
        success: function (files) {

            me.files = files;

            viewport.fillItemList(files, me.table);

            me.table.removeClass("hidden");
            $(".search-wait-animation").addClass("hidden");
        },
        error: function () {
            //TODO: add error handling
        }
    });
};

SearchView.prototype.emptySearch = function () {
    var me = this;
    me.table.addClass("hidden");
    me.toggleSearchIcon();
};

SearchView.prototype.toggleSearchIcon = function (show) {
    var me = this;

    me.container.find(".search-bar .fas").toggleClass("fa-times-circle", show);
};

SearchView.prototype.showDocument = function (me, e) {

    var target = $(e.target).closest("tr");
    var index = target.data("index");

    target.addClass("selected").siblings().removeClass("selected");

    var selectedItem = me.files[index];
    viewport.selectedItems = [selectedItem];

    if (viewport.docViewer.isFolder(selectedItem))
        viewport.showDocumentList(selectedItem.id);
    else
        viewport.showDocument(selectedItem.id, selectedItem.type);
};


SearchView.prototype.recalculateHeight = function () {
    var h = $(".panel-left").height() - $(".search-container").height();
    var prev = parseInt($(".search-hits-list-container").css("max-height"), 10); //.replace("px", "");

    $(".search-hits-list-container").css("max-height", prev + h);
};

