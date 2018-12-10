function Mask() {
    this.maskList = ".metadata-container-masklist";
    this.activeMask = null;

    this.initialize();
}

Mask.prototype.initialize = function () {
    var me = this;
    $(document).on("click", this.maskList + " li", function (e) { me.changeMask(me, e); });
    $(document).on("click", "#btn-modal-metadata-ok", function () { $('.modal').modal('hide'); });

    $(document).on("keyup", ".metadata-container-indexfields input[type=text]", function (e) {
        me.loadDynKwl(me, $(e.target));      
    });
    $(document).on("click", ".metadata-container-indexfields .fa-list-ul", function (e) {
        var $target = $(e.target).prev();
        me.loadDynKwl(me, $target);
    });

    $(document).on("click", ".dynkwl li", function (e) {
        var $target = $(e.target);
        me.fillInputFromDynKwl(me, $target);
    });
};

Mask.prototype.changeMask = function (me, e) {
    var $target = $(e.target);
    var maskname = $target.text().toLowerCase();
    $target.addClass("selected").siblings().removeClass("selected");

    me.setActiveMask(maskname);

    var fields = viewport.config.fields[maskname];

    $(".table-metadata tbody").find("tr:not('.index-default')").remove();

    $.each(fields, function (i, field) {
        var linetype = field.linetype;
        var input = '<div><input type="text" data-linekey="' + field.linekey + '"/><i class="fas fa-list-ul"></i></div>';
        if (linetype == 1)
            input = '<input type="checkbox" data-linekey="' + field.linekey + '"/>';
        else if (linetype == 2)
            input = '<input type="date" data-linekey="' + field.linekey + '"/>';
        $(".table-metadata tbody").append('<tr class="index-custom"><td>' + field.linebez + '</td><td colspan="3">'+input+'</td></tr>')
    });
};

Mask.prototype.loadDynKwl = function (me, $target) {
    var fieldName = $target.data("linekey");
    var searchVal = $target.val();

    $.ajax({
        url: "/Archive/LoadDynKwl",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: { fieldName: fieldName, searchVal: searchVal },
        success: function (listEntries) {
            var html = [];

            $.each(listEntries, function (i, entry) {
                var li = '<li>' + entry + '</li>';

                html.push(li);
            });

            $(".dynkwl").remove();
            $target.parent().append("<ul class='dynkwl'>" + html.join('') + "</ul>");
        }
    });
};

Mask.prototype.fillInputFromDynKwl = function (me, $target) {
    var val = $target.text();

    $target.parent().siblings("input").val(val);
    $target.parent().remove();
};

Mask.prototype.setMaskFields = function (fields) {
    var me = this;

    $.each(fields, function (i, field) {
        var key = field.fieldKey;
        if (field.lineType == 0)
            $("input[data-linekey='" + key + "']").val(field.fieldData);
        else if (field.lineType == 2)
            $("input[data-linekey='" + key + "']").val(moment(field.fieldData).format("DD.MM.YYYY"));      
    });
};

Mask.prototype.getMaskFields = function () {
    var me = this;

    var fieldKeys = {};

    var fields = $("input[data-linekey]");
    $.each(fields, function (i, field) {
        var key = $(field).data("linekey");
        var val = $(field).val();
        fieldKeys[key] = val;
    });

    return fieldKeys;
};

Mask.prototype.setActiveMask = function (maskname) {
    var mask = $.grep(viewport.config.masks, function (v) {
        return v.maskname.toLowerCase() === maskname.toLowerCase();
    })[0];
    this.activeMask = mask;
};