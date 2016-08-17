(function () {
    var $sideBarAndWrapper = $("#sidebar, #wrapper");
    var $icon = $("#sidebar-toggle i.fa");

    $("#sidebar-toggle").on("click", function() {
        $sideBarAndWrapper.toggleClass("hide-sidebar");
        if ($sideBarAndWrapper.hasClass("hide-sidebar")) {
            $icon.removeClass("fa-angle-left");
            $icon.addClass("fa-angle-right");
        } else {
            $icon.addClass("fa-angle-left");
            $icon.removeClass("fa-angle-right");
        }
    });
    
})();
