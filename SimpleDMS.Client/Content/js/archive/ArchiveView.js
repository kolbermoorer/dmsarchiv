/**
 * Creates a new view for the archive
 */
function ArchiveView() {
    this.tree = null;
    this.dropzone = new Dropzone();
    this.METADATA_TITLE = "Details zu Dokument";
    this.addNewFolder = false;
}

/**
 * Initialize the archive view. 
 * Create a new tree object that holds the archive tree.
 */
ArchiveView.prototype.initialize = function () {
    var me = this;

    me.tree = new Tree();
    me.tree.initialize();

    me.dropzone.initialize();

    $(document).on("click", "#btn-modal-metadata-ok", function () { me.checkMetadataOkClick(me); });
};

/**
 * View spezific recalculation of height
 */
ArchiveView.prototype.recalculateHeight = function () {
};

ArchiveView.prototype.checkMetadataOkClick = function (me) {
    if (me.addNewFolder) {
        me.addNewFolder(me);
    }
    else {
        me.moveDocument();
    }
};

ArchiveView.prototype.addNewFolder = function (me) {
    var selectedNode = me.tree.selectedNode;
    var objshort = $("#input-objshort").val();

    $.ajax({
        url: "/Archive/AddNewFolder",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: { selectedNode: selectedNode.li_attr.id, objshort: objshort, type: selectedNode.parents.length },
        success: function (id) {
            window.location.href = "/Archive/" + id;
            me.addNewFolder = false;
        }
    }); 
};

ArchiveView.prototype.fillMetadata = function () {
    var me = this;

    var selectedNode = viewport.view.tree.selectedNode;

    $.ajax({
        url: "/Archive/GetMaskAndIndexFields",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: { id: selectedNode.li_attr.id },
        success: function (fields) {
            var maskno = fields.length == 0 ? 0 : fields[0].maskNo;

            var mask = $.grep(viewport.config.masks, function (v) {
                return v.maskno === maskno;
            })[0];
            var maskname = mask.maskname;
            viewport.mask.activeMask = mask;

            $(".metadata-container-masklist li:contains('" + maskname + "')").addClass("selected").trigger("click");

            viewport.mask.setMaskFields(fields);

            var container = $("#modal-metadata");
            container.find(".title").text(me.METADATA_TITLE);
            $("#input-objshort").val(selectedNode.text);
        }
    });
};

ArchiveView.prototype.moveDocument = function () {
    var mask = viewport.mask.activeMask;
    var fieldKeys = viewport.mask.getMaskFields();
    var path = pad(mask.maskno, 2) + " " + mask.maskname + "¶" + mask.maskindex;

    $.each(fieldKeys, function (key, val) {
        path = path.replace(key, val);
    });

    mask.maskindex = path;
    mask.fields = fieldKeys;

    var id = viewport.view.tree.selectedNode.li_attr.id;

    $.ajax({
        url: "/Archive/MoveDocument",
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(
            {
                id: id,
                metadata: mask
            }),
        success: function (ret) {
            window.location.href = "/Archive/" + id;
        }
    });
};

function pad(str, max) {
    str = str.toString();
    return str.length < max ? pad("0" + str, max) : str;
}