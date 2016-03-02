$(document).ready(function () {

    var model = {
        id: "Id",
        fields: {
            Id: { type: "number", nullable: false, editable:false, validation: { required: true } },
            UserName: { type: "string", nullable: false, validation: { required: true } },
            UpdatedOn: { type: "Date", nullable: true, editable: false },
            CreatedOn: { type: "Date", nullable: false, editable: false }
        }
    };

    var ds = new kendo.data.DataSource({
        schema: {
            model: model
        },
        transport: {
            read: function (e) {
                lis.kendo.data.read({ url: "/users" }).response(e);
            },
            create: function (e) {
                lis.kendo.data.create({ url: "/user", data: e.data }).response(e);
            },
            update: function (e) {
                lis.kendo.data.update({ url: "/user", data: e.data }).response(e);
            },
            destroy: function (e) {
                lis.kendo.data.destroy({ url: "/user", data: e.data }).response(e);
            },
            parameterMap: function (data, type) {
                debugger;
                return data;
            }
        }
    });

    var columns = [
        { field: "UserName", title: "User Name" },
        { field: "FirstName", title: "First Name" },
        { field: "LastName", title: "Last Name" },
        { field: "UpdatedOn", title: "Updated" },
        { field: "CreatedOn", title: "Created" },
        { command: [ { name: "change", template: '<a class="k-button k-button-icon k-grid-change-password"><span class="k-icon k-i-unlock k-password" title="Change Password">P</span></a>' } ] }
    ];

    var grid = $("#grid-users").lisGrid({
        dataSource: ds,
        columns: columns,
        toolbar: ["create", "save", "cancel"],
        editable: {
            mode: "inline",
            createAt: "bottom",
            destroy: true
        },
        pageable: {
            refresh: true
        }
    }).data("kendoGrid");

    grid.bind("dataBound", function (e) {
        e.sender.element.find(".k-grid-change-password").on("click", function (e) {
            var row = lis.kendo.grid.getRow(e.target);
            var item = grid.dataItem(row);
            $("#window-change-password").remove();
            var template = kendo.template($("#window-change-password-template").html());
            var $win = $('<div id="window-change-password></div>').kendoWindow({
                title: "Change Password",
                width: "400px",
                height: "400px",
                visible: false,
                iframe: false,
                actions: [ "close", "minimize", "maximize" ],
                content: {
                    template: template(item)
                }
            });
            
            var win = $win.data("kendoWindow");

            $win.find(".k-primary").on("click", function (e) {
                
            });
            $win.find(".k-grid-cancel").on("click", function (e) {
                win.close();
            });
        });
    });

});