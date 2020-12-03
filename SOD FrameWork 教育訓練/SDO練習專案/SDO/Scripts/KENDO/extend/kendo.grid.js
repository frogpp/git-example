var gridConfig = {
    //autoBind: false,
    encoded: false,
    pageable: {
        pageSizes: ['All', 5, 10, 20, 50, 100]
    },
    page: function (e) {
        console.log(e.page);
        console.log(e.sender.dataSource.page() + ',' + e.sender.dataSource.pageSize());
    },
    resizable: true,
    sortable: false,
    filterable: false,
    dataBinding: function (e) {
        record = (e.sender.dataSource.page() - 1) * e.sender.dataSource.pageSize();
        var pager = [e.sender.dataSource.page(), e.sender.dataSource.pageSize()];

        // 設定 session 或 cookie 的 name 和 value
        var sessionName = pageConfig.cookiePath + 'pager';
        var sessionData = pager.join(',')

        // 判斷是否支援 sessionStorage 不支援改用 cookie
        if (typeof (Storage) !== "undefined") {
            window.sessionStorage[sessionName] = sessionData;
        } else {
            $.cookie(sessionName, sessionData);
        }

        //$.post(pageConfig.rootPath + 'System/Cookie', {
        //    'key': pageConfig.cookiePath + 'pager',
        //    'value': pager.join(',')
        //}, function (result) {
        //});
    },
    dataBound: function (e) {
        $.each(this.columns, function (i, o) {
            if (!o.title && !o.selectable) {
                e.sender.autoFitColumn(i);
                if (o.command) {
                    $.each(o.command, function () {
                        if ($.trim(this.text).length === 0)
                            $(e.sender.tbody).find('.' + this.iconClass.split(/\s+/).join('.')).attr('title', this.title);
                    });
                }
            }
        });
        if (!$(e.sender.table).closest('td').hasClass('k-detail-cell'))
            $(e.sender.tbody).kendoTooltip({
                filter: '[title]'
            });
    }
};
var extendGrid = kendo.ui.Grid.extend({
    initOptions: function () {
        var cols = $.grep(this.getOptions().columns, function (obj) {
            return !(obj.template && !$.isFunction(obj.template) && obj.template.indexOf('++record') >= 0) && !obj.selectable;
        });
        cols.unshift({
            template: '#= ++record #.',
            attributes: {
                style: 'text-align:right;'
            }
        });
        var options = $.extend({}, {
            columns: cols
        }, gridConfig);
        return options;
    },
    initSource: function (gridData, keyCol) {
        var dfd = $.Deferred(),
            dataSource = new kendo.data.DataSource({});

        // 設定 session 或 cookie 的 name 
        var sessionName = pageConfig.cookiePath + 'pager';
        var sessionValues;

        // 判斷是否支援 sessionStorage 不支援改用 cookie
        if (typeof (Storage) !== "undefined") {
            sessionValues = window.sessionStorage[sessionName];
        } else {
            sessionValues = $.cookie(sessionName);
        }

        var cookiePager = [1, 20];
        if ($.trim(sessionValues).length > 0)
            cookiePager = sessionValues.split(',');
        if ($.isArray(gridData)) {
            dataSource = new kendo.data.DataSource({
                data: gridData,
                page: $.isNumeric(cookiePager[0]) && parseInt(cookiePager[1]) < gridData.length ? parseInt(cookiePager[0]) : 1,
                pageSize: $.isNumeric(cookiePager[1]) ? parseInt(cookiePager[1]) : 20,
                schema: {
                    model: {
                        id: keyCol ? keyCol : 'uid'
                    }
                }
            });
        }
        else if ('url' in gridData) {
            dataSource = new kendo.data.DataSource({
                transport: {
                    read: {
                        type: 'POST',
                        dataType: 'json',
                        url: gridData.url,
                        data: gridData.data
                    }
                },
                serverPaging: true,
                page: parseInt(cookiePager[0]),
                pageSize: parseInt(cookiePager[1]),
                schema: {
                    total: 'total',
                    data: 'gridData',
                    model: {
                        id: keyCol ? keyCol : 'uid'
                    }
                }
            });
        }
        dfd.resolve(dataSource)


        //$.post(pageConfig.rootPath + 'System/Cookie', {
        //    'key': pageConfig.cookiePath + 'pager'
        //}, function (result) {
        //    var cookiePager = [1, 20];
        //    if ($.trim(result).length > 0)
        //        cookiePager = result.split(',');
        //    if ($.isArray(gridData)) {
        //        dataSource = new kendo.data.DataSource({
        //            data: gridData,
        //            page: $.isNumeric(cookiePager[0]) ? parseInt(cookiePager[0]) : 1,
        //            pageSize: $.isNumeric(cookiePager[1]) ? parseInt(cookiePager[1]) : 20,
        //            schema: {
        //                model: {
        //                    id: keyCol ? keyCol : 'uid'
        //                }
        //            }
        //        });
        //    }
        //    else if ('url' in gridData) {
        //        dataSource = new kendo.data.DataSource({
        //            transport: {
        //                read: {
        //                    type: 'POST',
        //                    dataType: 'json',
        //                    url: gridData.url,
        //                    data: gridData.data
        //                }
        //            },
        //            serverPaging: true,
        //            page: parseInt(cookiePager[0]),
        //            pageSize: parseInt(cookiePager[1]),
        //            schema: {
        //                total: 'total',
        //                data: 'gridData',
        //                model: {
        //                    id: keyCol ? keyCol : 'uid'
        //                }
        //            }
        //        });
        //    }
        //    dfd.resolve(dataSource)
        //});
        return dfd.promise();
    },
    //build custom grid
    genGrid: function (gridData) {
        var grid = this;
        if (grid.dataSource._total == 0)
            grid.setOptions(grid.initOptions());
        $.when(grid.initSource(gridData)).then(function (result) {
            grid.setDataSource(result);
        });
        return grid;
    },
    //build multi select checkbox grid
    genChkboxGrid: function (gridData, keyCol) {
        var grid = this,
            exOpts = grid.initOptions();
        exOpts.columns.splice(1, 0, {
            selectable: true,
            width: 24
        });
        grid.setOptions(exOpts);
        $.when(grid.initSource(gridData)).then(function (result) {
            grid.setDataSource(result);
            grid.setOptions({
                persistSelection: true
            });
        });
        return grid;
    },
    //get grid data with selected
    getSelect: function (idx) {
        var grid = this, result = [];
        $.each(this.selectedKeyNames(), function () {
            result.push(grid.dataSource.get(this));
        });
        return result;
    }
});
kendo.ui.plugin(extendGrid);