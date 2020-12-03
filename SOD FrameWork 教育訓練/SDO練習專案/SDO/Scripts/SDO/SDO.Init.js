$(window).on('beforeunload', function () {
    if (top.dlgProgress) {
        top.dlgProgress.open();
        setTimeout(function () {
            if ($.active === 0)
                top.dlgProgress.close();
        }, 5000);
    }
});
$(document)
    .ajaxStart(function () {
        if (top.dlgProgress)
            top.dlgProgress.open();
    })
    .ajaxStop(function () {
        if (top.resetCountdown)
            top.resetCountdown();
        if (top.dlgProgress)
            top.dlgProgress.close();
    });
$.ajaxPrefilter(function (options, originalOptions, jqXHR) {
    if (!options.beforeSend && options.type.toLowerCase() === 'post') {
        var data = originalOptions.data === undefined ? {} : originalOptions.data;
        if (originalOptions.data !== undefined) {
            if (Object.prototype.toString.call(originalOptions.data) === '[object String]')
                return;
        }
        else
            data = {};
        options.data = $.param($.extend(data, {
            __RequestVerificationToken: top.$('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val()
        }));
    }
});
$.validator.setDefaults({
    ignore: ''
});
$(function () {
    kendo.culture(pageConfig.i18N);
    if (top.resetCountdown)
        top.resetCountdown();
    $.each($('input[type=text],input[type=email],input[type=password],textarea'), function () {
        if (!$(this).data('kendoDateTimePicker') && !$(this).data('kendoDatePicker'))
            $(this).addClass('k-textbox');
    });
    $.each($('input[type=submit],input[type=button],button'), function () {
        if (!$(this).data('kendoButton'))
            $(this).kendoButton();
    });
    $.each($('select'), function () {
        if (!$(this).data('kendoDropDownList') && !$(this).data('kendoListBox') && !$(this).data('kendoMultiSelect'))
            $(this).kendoDropDownList({
                autoWidth: true
            });
    });
    if (top.dlgProgress)
        top.dlgProgress.close();
});

