jQuery(document).ready(function () {
    jQuery('tr').on({
        mouseenter: function () {
            jQuery(this).find('.btn-group').fadeTo('fast', 1)
        },
        mouseleave: function () {
            jQuery(this).find('.btn-group').fadeTo('fast', 0);
        }
    })
});