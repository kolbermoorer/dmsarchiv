function DMS() {

}

DMS.prototype.initialize = function () {
};

DMS.prototype.removeMetadata = function () {
    //TODO
};

DMS.prototype.addDocumentToFulltext = function () {
    viewport.showNotifyMessage("Aufnahme in den Volltext gestartet", "info");
    $.ajax({
        url: "/Archive/AddDocumentToFulltext",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: { id: viewport.selectedItems[0].id, name: viewport.selectedItems[0].name },
        success: function (success) {
            console.log("success");
            viewport.showNotifyMessage("Dokument erfolgreich in den Volltext aufgenommen");
        }
    });
};

DMS.prototype.removeDocumentFromFulltext = function () {
    viewport.showNotifyMessage("Entfernen aus dem Volltext gestartet", "info");
    $.ajax({
        url: "/Archive/RemoveDocumentFromFulltext",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: { id: viewport.selectedItems[0].id },
        success: function (success) {
            viewport.showNotifyMessage("Dokument erfolgreich aus dem Volltext entfernt");
        }
    });
};

DMS.prototype.mergeDocuments = function () {
    var me = this;
    var selectedItems = viewport.selectedItems;
    var files = selectedItems.map(a => a.FullPath);

    $.ajax({
        url: "/Archive/MergeDocuments",
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(files),
        success: function (success) {
            if (success) {
                viewport.showNotifyMessage("Dokument erfolgreich zusammengeführt");
                viewport.view.reload(true);
            }
            else {
                viewport.showNotifyMessage("Fehler beim zusammenführen der Dokumente", "danger");
            }
        }
    });
};

DMS.prototype.splitDocuments = function () {
    var me = this;
    var selectedItems = viewport.selectedItems;
    var files = selectedItems.map(a => a.FullPath);

    $.ajax({
        url: "/Archive/SplitDocuments",
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(files),
        success: function (success) {
            if (success) {
                viewport.showNotifyMessage("Dokument erfolgreich getrennt");
                viewport.view.reload(true);
            }
            else {
                viewport.showNotifyMessage("Fehler beim trennen der Dokumente", "danger");
            }
        }
    });
};

DMS.prototype.deleteDocuments = function () {
    var me = this;
    var selectedItems = viewport.selectedItems;
    var files = selectedItems.map(a => a.filePath);

    $.ajax({
        url: "/Archive/DeleteDocuments",
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(files),
        success: function (success) {
            if (success) {
                viewport.showNotifyMessage("Dokumente erfolgreich gelöscht");
                viewport.view.reload();
            }
            else {
                viewport.showNotifyMessage("Fehler beim Löschen der Dokumente", "danger");
            }
        }
    });
};