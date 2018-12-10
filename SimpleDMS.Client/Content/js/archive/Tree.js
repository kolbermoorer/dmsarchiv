function Tree(scope) {
    this.$tree = $("#" + scope);
    this.selectedNode = null;
    this.level = null;
    this.pathArr = null;
}

Tree.prototype.initialize = function () {
    var me = this;

    $('#jstree_dmsarchiv')
        .on('hover_node.jstree', function (e, data) {
        })
        .on('select_node.jstree', function (e, data) {
            var selectedNode = data.node;
            var id = selectedNode.li_attr.objid;
            var type = selectedNode.li_attr.type;

            selectedNode.li_attr.id = id;
            selectedNode.li_attr.name = selectedNode.text;

            me.selectedNode = selectedNode;

            viewport.selectedItems = [selectedNode.li_attr];

            $(".document-title").text(selectedNode.text);

            me.level = selectedNode.parents.length;

            $("#button-addFolder").removeClass("disabled");
            $("#document-rendering").remove();

            if (viewport.docViewer.isFolder(selectedNode.li_attr)) {
                viewport.showDocumentList(id);
            }
            else {
                viewport.showDocument(id, type);
            }
        })
        .on('after_close.jstree', function (e, data) {
            data.instance.close_all(data.node.id);
            return false;
        })
        .jstree({
            core: {
                data: {
                    url: function (node) {
                        return '/Archive/GetItemsByParentId';
                    },
                    data: function (node) {
                        return {
                            'id': (node.li_attr || {"objid" : "0"}).objid }
                    }
                }
            },
            plugins: ["state", "wholerow", "sort"],
            sort: function (a, b) {
                a1 = this.get_node(a);
                b1 = this.get_node(b);
                return (a1.text > b1.text) ? 1 : -1;
                /*if (a1.icon == b1.icon) {
                    return (a1.text > b1.text) ? 1 : -1;
                } else {
                    return (a1.icon > b1.icon) ? 1 : -1;
                }*/
            }
        });

    if (viewport.id != null) {
        me.openTreePath(viewport.id);
    }
};

Tree.prototype.createTableRow = function () {
    var $li = $("<li><table><tbody><tr></tr></tbody></table></li>");
};

/**
 * Opens a path in tree by full path as array of item ids
 * @param {any} id  item id of the entity that should be opened
 */
Tree.prototype.openTreePath = function (id) {
    var me = this;
    $(".jstree").jstree('close_all');

    $.ajax({
        url: "/Archive/GetPathAsArray",
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json',
        data: { id: id },
        success: function (pathArr) {
            pathArr.push(id);
            me.pathArr = pathArr;
            me.openTreeNode();
        }
    });
};


Tree.prototype.openTreeNode = function () {
    var me = this;
    var id = me.pathArr.shift();

    if (id == "0")
        id = "1";

   $(".jstree").jstree("open_node", $('.jstree-node[objid="' + id + '"]'));

    if (me.pathArr.length) {
        var nextId = me.pathArr[0];
        var waitTimer = setTimeout(function () {
            var nodeExists = $('.jstree-node[objid="' + nextId + '"]');

            if (nodeExists) {
                clearTimeout(waitTimer);
                me.openTreeNode();
            }
        }, 100);
    }
    else {
        $('.jstree').jstree("deselect_all");
        $(".jstree").jstree("select_node", $('.jstree-node[guid="' + id + '"]'));
    }

    
};
