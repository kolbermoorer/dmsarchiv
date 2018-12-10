
/**
 * Creates a new document viewer object to hold the selected document from archive, search or intray view 
 * @param {string} docType - type of the selected document (e.g. pdf, png, tiff, ...)
 */
function DocumentViewer(docType) {
    var me = this;
    this.docType = docType;
}

DocumentViewer.prototype.initialize = function () {

};

DocumentViewer.prototype.loadDocumentInViewer = function () {
    var me = this;

    $.ajax({
        url: "/Document/ShowInWebViewer",
        type: 'GET',
        dataType: 'json',
        async: false,
        contentType: 'text',
        data: {},
        success: function (html) {
            getDocument();
        }
    });
};

DocumentViewer.prototype.getItemsByParentId = function () {
    $.ajax({
        url: "/Archive/GetItemsByParentId",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: { id: selectedNode.id },
        success: function (nodes) {
            $.each(nodes, function () {
                $(".list-container").append(createTableRow(this));
            });
        }
    });
};

/**
 * Check if an item is an folder or document
 * @param {object} item Selected item in view
 * @return {bool} if the item is a folder this function returns true
 */
DocumentViewer.prototype.isFolder = function (item) {
    return item.type < 254 || item.type == 9999;
};


