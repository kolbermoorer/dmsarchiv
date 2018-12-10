function IntrayView() {
    this.METADATA_TITLE = "Verschlagwortung für neues Dokument";

    this.name = "Intray";
    this.table = $("#tbl-intray-files");
    this.dropzone = new Dropzone();
    this.files = [];
    this.multiselect = false;
    
}

IntrayView.prototype.initialize = function () {
    var me = this;

    me.reload();
    me.dropzone.initialize();

    $(document).on("click", "#btn-modal-metadata-ok", function () { me.insertIntoArchive(me); });
    $(document).on("click", "#tbl-intray-files tbody tr", function (e) { me.showDocument(me, e); });
};


/**
 * View spezific recalculation of height
 */
IntrayView.prototype.recalculateHeight = function () {
};

IntrayView.prototype.showDocument = function (me, e) {
    me.multiselect = e.ctrlKey;
    $(e.target).closest("tr").addClass("selected");

    if (me.multiselect) {
        viewport.selectedItems = me.getSelectedFiles();
    }
    else {
        $(e.target).closest("tr").siblings().removeClass("selected");
        var selectedFile = me.getSelectedFile();

        viewport.showDocument(-1, selectedFile.type, selectedFile.filePath);
    }

    
};


IntrayView.prototype.reload = function (selectLastFile = false) {
    var me = this;
    $.ajax({
        url: "/Intray/GetFiles",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: {},
        success: function (files) {
            console.log(files);
            me.files = files;

            var html = [];

            $.each(files, function (i) {
                var img = '<img src="data:image/png;base64, ' + this.iconBase64 + '"/>';
                var selected = ((i == files.length - 1) && selectLastFile) ? "selected" : "";

                html.push('<tr data-index="'+i+'" class="'+selected+'"><td>' + img + '</td><td>[' + this.fileName + ']</td><td>' + moment(this.creationDate).format("DD.MM.YYYY") + '</td><td></td><td>' + this.fileName + '</td></tr>');
            });

            me.table.find("tbody").html(html.join(''));

            var tblTemp = me.table;
            me.table.remove();
            $(".left_panel").append(tblTemp);
            me.table = tblTemp;

            if (files.length) {
                me.table.tablesorter({
                    sortList: [[1, 0]],
                    headers: { 0: { sorter: false } }
                });
            }

            if (selectLastFile)
                me.getSelectedFiles();
        },
        error: function (e) {
            console.error(e);
        }
    });
};

IntrayView.prototype.fillMetadata = function () {
    var container = $("#modal-metadata");
    var selFile = this.getSelectedFile();

    container.find(".title").text(this.METADATA_TITLE);
    $("#input-objshort").val(selFile.name);

    $("input[data-linekey='EDITOR']").val(viewport.user.name);
    $("input[data-linekey='VERSION']").val(1);
    $("input[data-linekey='DOCUMENTDATE']").val(moment().format('YYYY-MM-DD'));
    $("input[data-linekey='CREATIONDATE']").val(moment().format('YYYY-MM-DD'));

    $(".metadata-container-masklist li:contains('Chaosablage')").addClass("selected");
};

IntrayView.prototype.insertIntoArchive = function (me) {
    if (viewport.name != me.name)
        return;

    var file = me.getSelectedFile();
    var metadata = viewport.mask.activeMask;
    var fields = viewport.mask.getMaskFields();

    metadata.fields = fields;

    file.metadata = metadata;
    file.name = fields["NAME"];
    file.documentDate = fields["DOCUMENTDATE"];
    file.creationDate = fields["CREATIONDATE"];
    file.version = fields["VERSION"];
    file.editor = fields["EDITOR"];

    delete file["iconBase64"];

    console.log(file);

    $.ajax({
        url: "/Intray/InsertIntoArchive",
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(file),
        success: function (id) {
            //$('.modal').modal('hide');
            //$("#button-goToLastElement").removeClass("disabled");
            me.reload();
            window.location.href = "/Archive/" + id;
        }
    });
};

IntrayView.prototype.getSelectedFiles = function () {
    var me = this;
    
    var row = $(this.table.find("tr.selected"));

    if (row.length == 1)
        viewport.selectedItems = [];

    $.each(row, function (i, item) {
        var index = $(item).data("index");
        var selectedItem = me.files[index];

        var alreadySelected = viewport.selectedItems.includes(selectedItem);
        if (alreadySelected == false) {
            viewport.selectedItems.push(selectedItem);
        }
            
    });
    return viewport.selectedItems;
};

IntrayView.prototype.getSelectedFile = function () {
    return this.getSelectedFiles()[0];
};

IntrayView.prototype.getSelectedMaskName = function () {
    var maskname = $(".metadata-container-masklist li.selected").text();
    return maskname;
};

IntrayView.prototype.getDocumentName = function () {
    var name = $("#input-objshort").val();
    return name;
};