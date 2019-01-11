(function () {
    refresh();
    jQuery('#btnRight').click(function (e) {
        var selectedOpts = jQuery('#technologies option:selected');
        if (selectedOpts.length === 0) {
            alert("Nothing to move.");
            e.preventDefault();
        }
        jQuery('#myTechnologies').append(jQuery(selectedOpts).clone());
        jQuery(selectedOpts).remove();
        e.preventDefault();
        refresh();
    });
    jQuery('#btnAllRight').click(function (e) {
        var selectedOpts = jQuery('#technologies option');
        if (selectedOpts.length === 0) {
            alert("Nothing to move.");
            e.preventDefault();
        }
        jQuery('#myTechnologies').append(jQuery(selectedOpts).clone());
        jQuery(selectedOpts).remove();
        e.preventDefault();
        refresh();
    });
    jQuery('#btnLeft').click(function (e) {
        var selectedOpts = jQuery('#myTechnologies option:selected');
        if (selectedOpts.length === 0) {
            alert("Nothing to move.");
            e.preventDefault();
        }
        jQuery('#technologies').append(jQuery(selectedOpts).clone());
        jQuery(selectedOpts).remove();
        e.preventDefault();
        refresh();
    });
    jQuery('#btnAllLeft').click(function (e) {
        var selectedOpts = jQuery('#myTechnologies option');
        if (selectedOpts.length === 0) {
            alert("Nothing to move.");
            e.preventDefault();
        }
        jQuery('#technologies').append(jQuery(selectedOpts).clone());
        jQuery(selectedOpts).remove();
        e.preventDefault();
        refresh();
    });
}(jQuery));

function refresh() {
    var list = document.getElementById("myTechnologies");
    for (var i = 0; i < list.options.length; i++) {
        list.options[i].selected = true;
    }
}