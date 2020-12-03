//extend string format
$.extend(String.prototype, {
    padLeft: function (max, charPad) {
        return this.length < max ? (charPad + this).padLeft(max, charPad) : this;
    },
    padRight: function (max, charPad) {
        return this.length < max ? (this + charPad).padRight(max, charPad) : this;
    },
    setMask: function (startPos, length) {
        var chars = this.split('');
        return $.grep(chars, function (obj, idx) {
            return idx < startPos;
        }).join('') + ''.padLeft(length, '*') + $.grep(chars, function (obj, idx) {
            return idx >= startPos + length;
        }).join('');
    },
    /* 
    函數：字串格式化
    作者：Steven Tsai
    說明：將指定字串中以 {} 標示的內容依照指定參數順序進行取代
    範例："the {0} jumps over the {1}".format("quick brown fox", "lazy dog")
    */
    format: function () {
        var str = this;
        for (var i = 0; i < arguments.length; i++) {
            str = str.replace(new RegExp("\\{" + i + "\\}", "gm"), arguments[i]);
        }
        return str;
    }
});
//set orgs tree dialog open
function openOrgTreeDialog(id) {
    var orgTree = $('#orgTree_' + id).data('kendoTreeView');
    orgTree.select($());
    chkTreeNodes(orgTree.dataSource.view(), false);
    setChkTreeNodes(id);
    $('#dlgOrgTree_' + id).data('kendoDialog').open();
    return false;
}
//set treeview checked object
function setChkTreeNodes(id) {
    var orgTree = $('#orgTree_' + id).data('kendoTreeView');
    $.each($('#' + id).data('kendoMultiSelect').value(), function () {
        if (orgTree.dataSource.get(this))
            if ($('#orgTree_' + id).data('hasCheckbox'))
                orgTree.dataItem(orgTree.findByText(orgTree.dataSource.get(this).ORG_DISPLAY)).set('checked', true);
            else
                orgTree.select(orgTree.findByText(orgTree.dataSource.get(this).ORG_DISPLAY));
    });
}
//get treeview checked object
function getChkTreeNodes(nodes, aryChecked) {
    $.each(nodes, function () {
        if (this.checked)
            aryChecked.push(this);
        if (this.hasChildren)
            getChkTreeNodes(this.children.view(), aryChecked);
    });
}
//set treeview checkbox checked/unchecked
function chkTreeNodes(nodes, isChecked) {
    $.each(nodes, function () {
        this.set('checked', isChecked);
        if (this.hasChildren)
            chkTreeNodes(this.children.view(), isChecked);
    });
}
//javascript htmldecode by xssprevent
function htmlDecode(source) {
    return $('<div />').html($('<div />').html(source).text()).text();
}
function sanitizeHtml(source) {
    source = htmlDecode(htmlDecode(source));
    return $('<div />').append($.parseHTML(source)).each(function (node) {
        $.each(node.attributes, function () {
            var attrName = this.name;
            var attrValue = this.value;
            if (attrName.indexOf('on') === 0 || attrValue.indexOf('javascript:') === 0)
                $(node).removeAttr(attrName);
        });
    });
} 
