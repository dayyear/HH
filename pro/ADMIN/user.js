$(document).ready(function () {
    $(".btn").button();

    $("form.condition").submit(function (e) {
        e.preventDefault();
        $("table.dataTable")
            .data("condition", $(this).find(":input").filter(function () { return $.trim(this.value).length > 0 }).serialize())
            .data("page", 1)
            .data("rows", parseInt($("#rows").val()))
            .trigger("refresh");
    });

    $("table.dataTable").on("refresh", function () {
        var condition = $(this).data("condition");
        var page = $(this).data("page");
        var rows = $(this).data("rows");

        $(".loading").show();
        var start = new Date();
        $.post("userJson.ashx?verb=select&rows=" + rows + "&page=" + page, condition, function (resp) {
            $(".loading").hide();
            if (!resp.success) {
                alert(resp.message);
                return;
            }

            /* 1. table */
            var template = $("#template").html();
            Mustache.parse(template);
            var tbody = [];
            $.each(resp.items, function (i, item) {
                var tr = $("<tr>").append(Mustache.render(template, item)).data("item", item);
                tbody.push(tr);
            });
            $(".dataTable tbody").empty().append(tbody).children("tr:even").addClass("odd").end().children("tr:odd").addClass("even").end().selectable({ cancel: "span" });

            /* 2. page */
            $(".dataTablePagination").pagination(resp.total, { items_per_page: rows, num_edge_entries: 1, num_display_entries: 7, load_first_page: false, current_page: page - 1, next_text: "下页", prev_text: "上页",
                callback: function (page_index) {
                    $("table.dataTable").data("page", page_index + 1).trigger("refresh");
                    return false;
                }
            });

            /* 3. info */
            $(".dataTableInfo").text(sprintf("显示第 %d 至第 %d 项结果，共 %d 项，用时 %f 秒", (resp.total > 0 ? 1 : 0) + (page - 1) * rows, resp.items.length + (page - 1) * rows, resp.total, ((new Date()) - start) / 1000));
        }); //post
    });

    $("table.dataTable").on("click", "a.edit", function (e) {
        e.preventDefault();
        var item = $(this).closest("tr").data("item");
        var $dialog = $("#dialog-edit");
        $dialog.dialog({
            modal: true,
            title: "用户",
            width: 350,
            open: function () { $dialog.find("[name]").each(function () { $(this).val(item[$(this).attr("name")]); }); },
            buttons: {
                "保存": function () {
                    $(".loading").show();
                    $.post("userJson.ashx?verb=update", $dialog.find("form").serialize(), function (resp) {
                        $(".loading").hide();
                        if (!resp.success) {
                            alert(resp.message);
                            return;
                        }
                        $dialog.dialog("close");
                        $("table.dataTable").trigger("refresh");
                    }); //post
                },
                "密码重置": function () {
                    if (!confirm("你确定要密码重置吗？"))
                        return;
                    $(".loading").show();
                    $.post("userJson.ashx?verb=reset", $dialog.find("form").serialize(), function (resp) {
                        $(".loading").hide();
                        if (!resp.success) {
                            alert(resp.message);
                            return;
                        }
                        alert("密码已重置为: 111111");
                    });
                },
                "删除": function () {
                    if (!confirm("你确定要删除吗？删除之后将无法恢复！"))
                        return;
                    $(".loading").show();
                    $.post("userJson.ashx?verb=delete", $dialog.find("form").serialize(), function (resp) {
                        $(".loading").hide();
                        if (!resp.success) {
                            alert(resp.message);
                            return;
                        }
                        $dialog.dialog("close");
                        $("table.dataTable").trigger("refresh");
                    });
                },
                "取消": function () {
                    $dialog.dialog("close");
                }
            }
        }); //dialog
    });

    $("input.new").click(function (e) {
        e.preventDefault();
        var $dialog = $("#dialog-new");
        $dialog.dialog({
            modal: true,
            title: "用户",
            open: function () { $dialog.find("[name]").val(""); },
            buttons: {
                "保存": function () {
                    $(".loading").show();
                    $.post("userJson.ashx?verb=insert", $dialog.find("form").serialize(), function (resp) {
                        $(".loading").hide();
                        if (!resp.success) {
                            alert(resp.message);
                            return;
                        }
                        $dialog.dialog("close");
                        $("table.dataTable").trigger("refresh");
                    });
                },
                "取消": function () {
                    $dialog.dialog("close");
                }
            }
        }); //dialog
    });

    $("form.condition").submit();

}); //ready

