(function () {
    jQuery('#btnRight').click(function (e) {
        var selectedOpts = jQuery('#technologies option:selected');
        if (selectedOpts.length === 0) {
            alert("Nothing to move.");
            e.preventDefault();
        }
        jQuery('#myTechnologies').append(jQuery(selectedOpts).clone());
        jQuery(selectedOpts).remove();
        e.preventDefault();
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
    });
}(jQuery));
