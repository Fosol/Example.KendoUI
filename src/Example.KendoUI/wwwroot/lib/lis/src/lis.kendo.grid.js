lis.kendo.grid = (function (options) {
    var _this = {};
    _this.getRow = function (element) {
        return $(element).closest("tr");
    }
    _this.getRowIndex = function (grid, row) {
        if (!row.prev().is("tr")) row = lis.kendo.grid.getRow(row);
        return $("tr", grid.tbody).index(row);
    }
    _this.getColumnIndex = function (row, cell) {
        if (!row.prev().is("tr")) row = lis.kendo.grid.getRow(row);
        if (!cell.prev().is("td")) cell = $(cell).closest("td");
        return $("td", row).index(cell);
    }
    return _this;
})();

$.fn.lisGrid = function (options) {
    options = $.extend({
        selectable: true,
        scrollable: false
    }, options);
    options.editable = $.extend({
        confirmation: false,
        mode: "inline",
        createAt: "bottom",
        update: true,
        destroy: true
    }, options.editable);
    var editable = options.editable;

    var command_column = new lis.Linq(options.columns).firstOrDefault(function (e) { return e.command !== undefined; })
    if (editable) {
        if (!command_column) {
            command_column = { command: [] };
            options.columns.push(command_column);
        }
        var commands = new lis.Linq(command_column.command);
        var edit = commands.firstOrDefault(function (e) { return e.name === "edit"; });
        if (options.editable.update && !edit) {
            edit = { name: "edit" };
            command_column.command.push(edit);
        }
        if (edit) {
            edit.template = '<a class="k-button k-button-icon k-grid-action-edit"><span class="k-icon k-edit" title="Edit">#=text#</span></a>';
        }
        var destroy = commands.firstOrDefault(function (e) { return e.name === "destroy"; });
        if (options.editable.destroy && !destroy) {
            destroy = { name: "destroy" };
            command_column.command.push(destroy);
        }
        if (destroy) {
            destroy.template = '<a class="k-button k-button-icon k-grid-action-delete"><span class="k-icon k-delete" title="Delete">#=text#</span></a>';
        }
        command_column.width = (command_column.command.length * 48) + "px";
    }
    $.extend({}, options.columns)
    var $grid = $(this).kendoGrid(options);
    var grid = $grid.data("kendoGrid");

    // Hook up events to the grid data.
    grid.bind("dataBound", function (e) {
        e.sender.element.find(".k-grid-action-edit").on("click", function (e) {
            var row = lis.kendo.grid.getRow(e.target);
            grid.editRow(row);
        });
        e.sender.element.find(".k-grid-action-delete").on("click", function (e) {
            var row = lis.kendo.grid.getRow(e.target);
            grid.select(row);
            var item = grid.dataItem(row);
            $("#window-confirm").remove();
            var html = $('<div class="k-edit-form-container"></div>')
                .append('<div class="cell"><span>Do you want to delete "#:UserName#"?</span></div>')
                .append('<div class="k-edit-buttons k-state-default"><button class="k-button k-button-icontext k-primary" data-confirm-yes><span class="k-icon k-i-tick" title="Yes"></span>Yes</button>'
                + '<button class="k-button k-button-icontext k-grid-cancel" data-confirm-cancel><span class="k-icon k-i-cancel" title="No"></span>No</button></div>');
            var template = kendo.template(html.wrap("div").parent().html());
            var $win = $('<div id="window-confirm"></div>').kendoWindow({
                title: "Delete",
                width: "400px",
                height: "200px",
                visible: false,
                iframe: false,
                content: {
                    template: template(item)
                }
            });
            var win = $win.data("kendoWindow");

            $win.find("[data-confirm-yes]").on("click", function (e) {
                grid.removeRow(row);
                win.close();
            });
            $win.find("[data-confirm-cancel]").on("click", function (e) {
                win.close();
            });

            win.center().open();
        });
    });

    grid.bind("edit", function (e) {
        if (editable) {
            var row = lis.kendo.grid.getRow(e.container);
            var col = $("td", row).last();
            col.html("");
            var btn_update = $('<a class="k-button k-button-icon k-grid-action-update"><span class="k-icon k-update" title="Update">Update</span></a>').appendTo(col);
            var btn_cancel = $('<a class="k-button k-button-icon k-grid-action-cancel"><span class="k-icon k-cancel" title="Cancel">Cancel</span></a>').appendTo(col);
            btn_update.on("click", function (e) {
                grid.saveRow(row);
            });
            btn_cancel.on("click", function (e) {
                grid.cancelRow(row);
                grid.refresh();
            });
        }
    });

    // Add change event to datasource
    if (options.dataSource) {
        options.dataSource.bind("change", function (e) {
            var has_changes = e.sender.hasChanges();
            $grid.find(".k-grid-cancel-changes").toggle(has_changes);
            $grid.find(".k-grid-save-changes").toggle(has_changes);
            if (e.action) {

            }
        });
    }

    return $grid;
};