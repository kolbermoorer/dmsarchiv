function adjustFixedHeaderWidth() {
    $.each($(".fixed-headers tr:first-child td"), function () {
        var w = $(this).width();
        var index = $(this).index();

        $(".fixed-headers th").eq(index).width(w);
    });

    var h = $(".fixed-headers thead").height();

    $(".fixed-headers tbody").css("margin-top", h);
}