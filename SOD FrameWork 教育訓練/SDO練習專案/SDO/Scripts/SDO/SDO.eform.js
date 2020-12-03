$.fn.extend({
    resetValid: function () {
        this.removeData('validator');
        this.removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(this);
    },
    resetForm: function () {
        this.get(0).reset();
    },
    cacheForm: function () {
        var dfd = $.Deferred(),
            _form = this;

        // 設定 session 或 cookie 的 name 和 value
        var sessionName = pageConfig.cookiePath + _form.attr('id');
        var sessionData = JSON.stringify($.grep(_form.serializeArray(), function (o) {
            return o.name !== '__RequestVerificationToken';
        }));
        // 判斷是否支援 sessionStorage 不支援改用 cookie
        if (typeof (Storage) !== "undefined") {
            window.sessionStorage[sessionName] = sessionData;
        } else {
            $.cookie(sessionName, sessionData);
        }
        //$.post(pageConfig.rootPath + 'System/Cookie', {
        //    'key': pageConfig.cookiePath + _form.attr('id'),
        //    'value': JSON.stringify($.grep(_form.serializeArray(), function (o) {
        //        return o.name !== '__RequestVerificationToken';
        //    }))
        //}, function (result) {
        //    //
        //    });
        dfd.resolve();
        return dfd.promise();
    },
    restoreForm: function () {
        var dfd = $.Deferred(),
            _form = this;

        // 設定 session 或 cookie 的 name 
        var sessionName = pageConfig.cookiePath + _form.attr('id');
        var sessionValues;

        // 判斷是否支援 sessionStorage 不支援改用 cookie
        if (typeof (Storage) !== "undefined") {
            //window.sessionStorage.removeItem(sessionName);
            sessionValues = window.sessionStorage[sessionName];
        } else {
            sessionValues = $.cookie(sessionName);
        }
        if ($.trim(sessionValues).length > 0)
            $.when(_form.deserializeForm(sessionValues)).then(function () {
                dfd.resolve();
            });
        else
            dfd.resolve();

        //$.post(pageConfig.rootPath + 'System/Cookie', {
        //    'key': pageConfig.cookiePath + _form.attr('id')
        //}, function (result) {
        //    if ($.trim(result).length > 0)
        //        $.when(_form.deserializeForm(result)).then(function () {
        //            dfd.resolve();
        //        });
        //    else
        //        dfd.resolve();
        //});
        return dfd.promise();
    },
    deserializeForm: function (formData) {
        var dfd = $.Deferred();
        if (formData && !$.isArray(formData))
            formData = JSON.parse(formData);
        if (formData && $.isArray(formData)) {
            var _form = this;
            $.each($.unique($.map(formData, function (o) {
                return o.name;
            })), function (i, name) {
                var field = $('#' + name, _form),
                    fieldVal = $.map($.grep(formData, function (o) {
                        return o.name === name;
                    }), function (o) {
                        return o.value;
                    });
                if (field && name !== '__RequestVerificationToken') {
                    if (field.data('kendoDropDownList'))
                        field.data("kendoDropDownList").value(fieldVal);
                    else if (field.data('kendoMultiSelect')) {
                        if (parseInt(field.data('FIELD_EXTTYPE')) === 12) {
                            $.post(pageConfig.rootPath + 'EmpOrg/QueryMultiOrgs', { 'orgIds': fieldVal }, function (result) {
                                field.data('kendoMultiSelect').setDataSource(new kendo.data.DataSource({
                                    data: result
                                }));
                                field.data('kendoMultiSelect').value($.map(result, function (o) {
                                    return o.ORG_ID;
                                }));
                            });
                        }
                        else
                            field.data('kendoMultiSelect').value(fieldVal);
                    }
                    else if (field.data('kendoListBox')) {
                        if (parseInt(field.data('FIELD_EXTTYPE')) === 13) {
                            $.post(pageConfig.rootPath + 'EmpUser/QueryUsers', { 'userIds': fieldVal }, function (result) {
                                field.data('kendoListBox').setDataSource(new kendo.data.DataSource({
                                    data: result
                                }));
                            });
                        }
                        else {
                            $.each(fieldVal, function (i, o) {
                                var idx = $.inArray(o, $.map(field.data('kendoListBox').dataItems(), function (o) {
                                    return o.VALUE;
                                }));
                                if (idx >= 0)
                                    field.data('kendoListBox').select(field.data('kendoListBox').items().eq(idx));
                            });
                        }
                    }
                    else if (field.data('kendoEditor'))
                        field.data("kendoEditor").value(fieldVal);
                    else if (field.data("kendoNumericTextBox"))
                        field.data("kendoNumericTextBox").value(fieldVal);
                    else if (field.data("kendoSlider"))
                        field.data("kendoSlider").value(fieldVal);
                    else {
                        if ($('input[name="' + name + '"]').length > 1) {
                            $.each($('input[name="' + name + '"]'), function () {
                                switch ($(this).attr('type').toLowerCase()) {
                                    case 'checkbox':
                                    case 'radio':
                                        $(this).attr('checked', $.inArray($(this).val(), fieldVal) > -1);
                                        break;
                                    default:
                                        break;
                                }
                            });
                        }
                        else {
                            field.val(fieldVal);
                            switch (parseInt(field.data('FIELD_EXTTYPE'))) {
                                case 22:
                                    field.nextAll('.twdatetime').remove();
                                    $.get(pageConfig.rootPath + '/Home/_TWDatetime', $.extend(JSON.parse(field.data('FIELD_OPTION')), { 'ID': name }), function (result) {
                                        $('<div />').addClass('twdatetime').append($(result)).insertAfter(field);
                                    });
                                    break;
                                case 4:
                                    alert(fieldVal)
                                    field.nextAll('.fileuploader').remove();
                                    $.post(pageConfig.rootPath + '/Upload/GetUploads', { 'fileNames': fieldVal }, function (result) {
                                        $.get(pageConfig.rootPath + '/Home/_FileUploader', $.extend(JSON.parse(field.data('FIELD_OPTION')), { 'ID': name }), function (result) {
                                            $('<div />').addClass('fileuploader').append($(result)).insertAfter(field);
                                        });
                                    });
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            });
        }
        var chkActive = setInterval(function () {
            if ($.active === 0) {
                clearInterval(chkActive);
                setTimeout(function () {
                    dfd.resolve();
                }, 10);
            }
        }, 100);
        return dfd.promise();
    },
    buildForm: function (_readonly) {
        var _form = this, _fieldTable = $('<table />').appendTo(this), _dataFills, _fields;
        this.readonly = function () {
            //全域遮罩
            $('<div />').css('background-color', 'gray').css('position', 'absolute').css('opacity', 0.3).css('z-index', 9).width(parseInt(_form.width())).height(parseInt(_form.height() + 10)).insertBefore(_form);
            //獨立控件處理
            //$.each(_fields, function (i, field) {
            //    switch (parseInt(field.FIELD_TYPE)) {
            //        case 1: // 下拉選單
            //            switch (parseInt(field.FIELD_EXTTYPE)) {
            //                case 11: //多選
            //                    if ($('#' + field.FIELD_ID, _form).data('kendoMultiSelect')) {
            //                        $('#' + field.FIELD_ID, _form).data('kendoMultiSelect').readonly();
            //                        $('span', $('#' + field.FIELD_ID, _form).data('kendoMultiSelect').tagList).css('color', 'gray');
            //                    }
            //                    break;
            //                case 12: //組織選單
            //                    if ($('#' + field.FIELD_ID, _form).data('kendoMultiSelect')) {
            //                        $('#' + field.FIELD_ID, _form).data('kendoMultiSelect').readonly();
            //                        $('span', $('#' + field.FIELD_ID, _form).data('kendoMultiSelect').tagList).css('color', 'gray');
            //                    }
            //                    if ($('button', $('#' + field.FIELD_ID, _form).closest('td')).data('kendoButton'))
            //                        $('button', $('#' + field.FIELD_ID, _form).closest('td')).data('kendoButton').enable(false);
            //                    break;
            //                case 13: //人員選單
            //                    if ($('#orgUsersDdl_' + field.FIELD_ID, _form).data('kendoDropDownList')) {
            //                        $('#orgUsersDdl_' + field.FIELD_ID, _form).data('kendoDropDownList').readonly();
            //                        $('#orgUsersDdl_' + field.FIELD_ID, _form).data('kendoDropDownList').span.css('color', 'gray');
            //                    }
            //                    if ($('#orgUsersSrc_' + field.FIELD_ID, _form).data('kendoListBox'))
            //                        $('#orgUsersSrc_' + field.FIELD_ID, _form).data('kendoListBox').enable('.k-item', false);
            //                    if ($('#' + field.FIELD_ID, _form).data('kendoListBox'))
            //                        $('#' + field.FIELD_ID, _form).data('kendoListBox').enable('.k-item', false)
            //                    break;
            //                case 14: //Checkbox
            //                case 15: //Radio
            //                    $('input[name="' + field.FIELD_ID + '"]', _form).attr('disabled', 'disabled');
            //                    break;
            //                default:
            //                    if ($('#' + field.FIELD_ID, _form).data('kendoDropDownList')) {
            //                        $('#' + field.FIELD_ID, _form).data('kendoDropDownList').readonly();
            //                        $('#' + field.FIELD_ID, _form).data('kendoDropDownList').span.css('color', 'gray');
            //                    }
            //                    break;
            //            }
            //            break;
            //        case 2: //文字輸入框 textbox
            //            switch (parseInt(field.FIELD_EXTTYPE)) {
            //                case 21: //數字
            //                    if ($('#' + field.FIELD_ID, _form).data('kendoNumericTextBox'))
            //                        $('#' + field.FIELD_ID, _form).data('kendoNumericTextBox').readonly();
            //                    break;
            //                case 22: //日期
            //                    if ($('#_' + field.FIELD_ID, _form).data('kendoDatePicker'))
            //                        $('#_' + field.FIELD_ID, _form).data('kendoDatePicker').readonly();
            //                    if ($('#_' + field.FIELD_ID, _form).data('kendoDateTimePicker'))
            //                        $('#_' + field.FIELD_ID, _form).data('kendoDateTimePicker').readonly();
            //                    break;
            //                case 23: //格式化文字
            //                    if ($('#' + field.FIELD_ID, _form).data('kendoMaskedTextBox'))
            //                        $('#' + field.FIELD_ID, _form).data('kendoMaskedTextBox').readonly();
            //                    break;
            //                default: //一般文字
            //                    $('#' + field.FIELD_ID, _form).attr('readonly', true);
            //                    break;
            //            }
            //            break;
            //        case 3: //文字輸入框 textarea
            //            switch (parseInt(field.FIELD_EXTTYPE)) {
            //                case 31:
            //                case 32:
            //                    var editor = $('#' + field.FIELD_ID, _form).closest('table');
            //                    if ($('#' + field.FIELD_ID, _form).data('kendoEditor'))
            //                        $('<div />').css('background-color', 'gray').css('position', 'absolute').css('opacity', 0.1).css('z-index', 9).width(parseInt(editor.width())).height(parseInt(editor.height() + 10)).insertBefore(editor);
            //                    break;
            //                default:
            //                    $('#' + field.FIELD_ID, _form).off('focus').off('blur');
            //                    $('#' + field.FIELD_ID, _form).attr('readonly', true);
            //                    break;
            //            }
            //            break;
            //        case 4: //檔案上傳
            //            if ($('#uploader_' + field.FIELD_ID, _form).data('kendoUpload'))
            //                $('#uploader_' + field.FIELD_ID, _form).data('kendoUpload').disable();
            //            break;
            //        default:
            //            break;
            //    }
            //});
            return this;
        };
        this.resetForm = function () {
            if (_dataFills && _dataFills.length > 0)
                _form.deserializeForm(_dataFills);
            else
                _form.get(0).reset();
        };
        this.saveForm = function () {
            this.saveFiles = function () {
                var dfd = $.Deferred(),
                    fields = $.grep(_fields, function (o) {
                        return parseInt(o.FIELD_TYPE) === 4;
                    });
                if (fields.length > 0)
                    $.each(fields, function (i, o) {
                        var field = $('#' + o.FIELD_ID, _form);
                        if ($.trim(field.val()).split(',').length > 0)
                            $.post(pageConfig.rootPath + 'Upload/SaveUploads', { 'fileNames': field.val().split(',') }, function (result) {
                                field.val(result);
                            });
                    });
                else
                    dfd.resolve();
                var chkActive = setInterval(function () {
                    if ($.active === 0) {
                        clearInterval(chkActive);
                        setTimeout(function () {
                            dfd.resolve();
                        }, 10);
                    }
                }, 100);
                return dfd.promise();
            };
            _form.resetValid();
            if (_form.valid()) {
                $.when(this.saveFiles()).then(function () {
                    $('#FILL_DATA', _form).val(JSON.stringify($.grep(_form.serializeArray(), function (o) {
                        return $.inArray(o.name, $.map(_fields, function (o) {
                            return o.FIELD_ID.toString();
                        })) > -1 || o.name === 'FORM_ID';
                    })));
                    _form.submit();
                });
            }
        };
        this.getDataFill = function () {
            var dfd = $.Deferred();
            if ($.trim($('#FILL_ID', _form).val()).length > 0)
                $.post(pageConfig.rootPath + 'eForm/QueryDataFills', { 'fillId': $('#FILL_ID', _form).val() }, function (result) {
                    _dataFills = JSON.parse(htmlDecode(result));
                    if (_dataFills.length > 0)
                        $('#FORM_ID', _form).val($.map($.grep(_dataFills, function (o) {
                            return o.name === 'FORM_ID';
                        }), function (o) {
                            return o.value;
                        }).join(''));
                    dfd.resolve();
                }).fail(function () {
                    alert(123);
                    dfd.resolve();
                });
            else
                dfd.resolve();
            return dfd.promise();
        };
        this.initForm = function () {
            $.fn.extend({
                genField: function (field) {
                    $('<th />').text(field.FIELD_NAME).prepend(field.FIELD_ISFILL ? '*' : '').appendTo(this);
                    var ctrl, ctrlContainer = $('<td />').attr('colspan', field.ISSPLITTER ? 1 : 3).appendTo(this);
                    switch (parseInt(field.FIELD_TYPE)) {
                        case 0: //純文字顯示
                            ctrlContainer.text(field.FIELD_OPTION);
                            break;
                        case 1: // 下拉選單
                            ctrl = $('<select />').data('FIELD_EXTTYPE', parseInt(field.FIELD_EXTTYPE)).attr('id', field.FIELD_ID).attr('name', field.FIELD_ID).attr('data-val', 'true').appendTo(ctrlContainer);
                            switch (parseInt(field.FIELD_EXTTYPE)) {
                                case 11: //多選
                                    ctrl.kendoMultiSelect({
                                        dataSource: JSON.parse(htmlDecode(field.FIELD_OPTION)),
                                        dataTextField: 'Text',
                                        dataValueField: 'Value',
                                        autoClose: false,
                                        placeholder: '請選擇...'
                                    });
                                    break;
                                case 12: //組織選單
                                    $.get(pageConfig.rootPath + '/Home/_OrgSelector', $.extend(JSON.parse(htmlDecode(field.FIELD_OPTION)), { 'ID': field.FIELD_ID }), function (result) {
                                        $(result).insertAfter(ctrl);
                                    });
                                    break;
                                case 13: //人員選單
                                    ctrl.attr('multiple', 'multiple');
                                    $.get(pageConfig.rootPath + '/Home/_OrgUserSelector', { 'ID': field.FIELD_ID }, function (result) {
                                        $(result).insertAfter(ctrl);
                                    });
                                    break;
                                case 14: //Checkbox
                                case 15: //Radio
                                    ctrl.remove();
                                    $.get(pageConfig.rootPath + '/Home/' + (parseInt(field.FIELD_EXTTYPE) === 14 ? '_CheckboxList' : '_RadioList'), { 'ID': field.FIELD_ID, 'VALUE': htmlDecode(field.FIELD_OPTION) }, function (result) {
                                        $(result).prependTo(ctrlContainer);
                                    });
                                    break;
                                default:
                                    ctrl.kendoDropDownList({
                                        dataSource: JSON.parse(htmlDecode(field.FIELD_OPTION)),
                                        dataTextField: 'Text',
                                        dataValueField: 'Value',
                                        optionLabel: '請選擇...'
                                    });
                                    break;
                            }
                            break;
                        case 2: //文字輸入框 textbox
                            ctrl = $('<input type="text" />').data('FIELD_EXTTYPE', parseInt(field.FIELD_EXTTYPE)).attr('id', field.FIELD_ID).attr('name', field.FIELD_ID).attr('data-val', 'true').appendTo(ctrlContainer);
                            switch (parseInt(field.FIELD_EXTTYPE)) {
                                case 21: //數字
                                    ctrl.kendoNumericTextBox(JSON.parse(htmlDecode(field.FIELD_OPTION)));
                                    break;
                                case 22: //日期
                                    ctrl.data('FIELD_OPTION', htmlDecode(field.FIELD_OPTION));
                                    $.get(pageConfig.rootPath + '/Home/_TWDatetime', $.extend(JSON.parse(htmlDecode(field.FIELD_OPTION)), { 'ID': field.FIELD_ID }), function (result) {
                                        $('<div />').addClass('twdatetime').append($(result)).insertAfter(ctrl);
                                    });
                                    break;
                                case 23: //格式化文字
                                    ctrl.kendoMaskedTextBox(JSON.parse(htmlDecode(field.FIELD_OPTION)));
                                    break;
                                case 24: //數字slider
                                    ctrl.css('min-width', '480px').css('width', '100%');
                                    ctrl.kendoSlider(JSON.parse(htmlDecode(field.FIELD_OPTION)));
                                    break;
                                default: //一般文字
                                    if (parseInt(field.FIELD_SIZE) > 0)
                                        ctrl.attr('data-val-length-min', 0).attr('data-val-length-max', parseInt(field.FIELD_SIZE)).attr('data-val-length', '#{field_name} 欄位超出長度。'.replace(/#\{field_name\}/g, field.FIELD_NAME));
                                    ctrl.addClass('k-textbox');
                                    break;
                            }
                            break;
                        case 3: //文字輸入框 textarea
                            ctrl = $('<textarea />').data('FIELD_EXTTYPE', parseInt(field.FIELD_EXTTYPE)).attr('id', field.FIELD_ID).attr('name', field.FIELD_ID).attr('rows', 5).css('width', '100%').attr('data-val', 'true').appendTo(ctrlContainer);
                            setTimeout(function () {
                                switch (parseInt(field.FIELD_EXTTYPE)) {
                                    case 31:
                                        ctrl.kendoEditor();
                                        break;
                                    case 32:
                                        ctrl.kendoEditor({
                                            resizable: true,
                                            tools: parseInt(field.FIELD_EXTTYPE) === 31 ? [] : ['bold', 'italic', 'underline', 'strikethrough', 'justifyLeft', 'justifyCenter', 'justifyRight', 'justifyFull', 'insertUnorderedList', 'insertOrderedList', 'indent', 'outdent', 'createLink', 'unlink', 'insertImage', 'insertFile', 'subscript', 'superscript', 'tableWizard', 'createTable', 'addRowAbove', 'addRowBelow', 'addColumnLeft', 'addColumnRight', 'deleteRow', 'deleteColumn', 'viewHtml', 'formatting', 'cleanFormatting', 'fontName', 'fontSize', 'foreColor', 'backColor', 'print']
                                        });
                                        break;
                                    default:
                                        ctrl.on('focus', function () {
                                            $(this).height(parseInt($(this).height() * 3));
                                        }).on('blur', function () {
                                            $(this).height(parseInt($(this).height() / 3));
                                        });
                                        ctrl.css('min-height', '50px').addClass('k-textbox');
                                        break;
                                }
                            }, 10);
                            break;
                        case 4: //檔案上傳
                            ctrl = $('<input type="text" />').data('FIELD_EXTTYPE', parseInt(field.FIELD_TYPE)).data('FIELD_OPTION', htmlDecode(field.FIELD_OPTION)).attr('id', field.FIELD_ID).attr('name', field.FIELD_ID).attr('data-val', 'true').appendTo(ctrlContainer);
                            $.get(pageConfig.rootPath + '/Home/_FileUploader', $.extend(JSON.parse(htmlDecode(field.FIELD_OPTION)), { 'ID': field.FIELD_ID }), function (result) {
                                $('<div />').addClass('fileuploader').append($(result)).insertAfter(ctrl);
                            });
                            break;
                        default:
                            ctrlContainer.text('');
                            break;
                    }
                    if (ctrl && field.FIELD_ISFILL)
                        ctrl.attr('data-val-required', '#{field_name} 欄位是必要項。'.replace(/#\{field_name\}/g, field.FIELD_NAME));
                    $('<span />').addClass('field-validation-valid').attr('data-valmsg-replace', true).attr('data-valmsg-for', field.FIELD_ID).appendTo(ctrlContainer);
                }
            });
            var dfd = $.Deferred();
            $.post(pageConfig.rootPath + 'eForm/QueryFields', { 'eFormId': $('#FORM_ID', _form).val() }, function (result) {
                var row, colCnt = 0;
                _fields = result;
                $.each(_fields, function (i, o) {
                    if (colCnt === 0)
                        row = $('<tr />').appendTo(_fieldTable);
                    row.genField(o);
                    if (o.ISSPLITTER)
                        colCnt = colCnt === 0 ? colCnt + 1 : 0;
                });
            });
            var chkActive = setInterval(function () {
                if ($.active === 0) {
                    clearInterval(chkActive);
                    setTimeout(function () {
                        dfd.resolve();
                    }, 10);
                }
            }, 100);
            return dfd.promise();
        };
        $.when(this.getDataFill()).then(function () {
            return _form.initForm();
        }).then(function () {
            if (_dataFills && _dataFills.length > 0)
                return _form.deserializeForm(_dataFills);
        }).then(function () {
            if (_readonly)
                _form.readonly();
        });
        return this;
    }
});

