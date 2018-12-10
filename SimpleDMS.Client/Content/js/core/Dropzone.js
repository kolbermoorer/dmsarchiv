function Dropzone() {
    this.zone = $("#dropzone");
    this.section = viewport.getSection("picker");
}

Dropzone.prototype.initialize = function () {
    var me = this;
    this.section.on({
        dragenter: function () {
            console.log(dropzone);
            $(dropzone).find("input").css("width", me.section.width()).css("height", me.section.height());
            $(dropzone).css("width", me.section.width()).css("height", me.section.height()).removeClass("hidden");
            
        },
        dragleave: function (e) {
            if ($(e.relatedTarget).hasClass("panel-left"))
                $(dropzone).removeClass("hidden");
            else if ($(e.relatedTarget).parents(".panel-left").length == 0)
                $(dropzone).addClass("hidden");
        }
    });

    this.zone.find('input').on('change', function (e) { me.uploadFile(me); });
};


Dropzone.prototype.uploadFile = function (me) {
    me.zone.addClass("hidden");
    var formData = new FormData($('form')[0]);
    formData.append("page", viewport.name);

    $.ajax({
        url: '/Intray/UploadFile',
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        type: 'POST',
        success: function (data, textStatus, jqXHR) {
            viewport.view.reload();
        }
    });
};