var app = {
    form: null,
    page: 1,
    rows: 10,
    total: 0,
    items: [],
    success: false,
    message: null,
    template: null
};


$(document).ready(function () {

    $("input[type=submit], input[type=reset], input[type=button]").button();

    $('.datepicker').datepicker({
        dateFormat: "yy-mm-dd",
        changeMonth: true,
        changeYear: true,
        showButtonPanel: true,
        monthNamesShort: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
        dayNamesMin: ["日", "一", "二", "三", "四", "五", "六"],
        currentText: '今天',
        closeText: '关闭',
        yearRange: "c-100:c+10",
        showOn: "button",
        buttonImage: "../../pub/img/datebox_arrow.png",
        buttonImageOnly: true,
        buttonText: "选择日期"
    });

    app.template = Handlebars.compile($("#resultTemplate").html());

    $("#condition form").submit(function (e) {
        e.preventDefault();
        app.form = $("#condition form").serialize();
        app.page = 1;
        app.rows = parseInt($("#rows").val());
        save();
    });

    $("#condition form").trigger("submit");

    $("#首页").click(function (e) {
        e.preventDefault();
        app.page = 1;
        save();
    });
    $("#上一页").click(function (e) {
        e.preventDefault();
        app.page--;
        save();
    });
    $("#下一页").click(function (e) {
        e.preventDefault();
        app.page++;
        save();
    });
    $("#尾页").click(function (e) {
        e.preventDefault();
        app.page = Math.ceil(app.total / app.rows);
        save();
    });

    $("#datagrid tbody tr").live("mouseenter", function () {
        //$("#datagrid tbody").on("mouseenter", "tr", function () {
        $(this).addClass("selecting");
    });
    $("#datagrid tbody tr").live("mouseleave", function () {
        //$("#datagrid tbody").on("mouseleave", "tr", function () {
        $(this).removeClass("selecting");
    });
    $("#datagrid tbody tr").live("click", function () {
        //$("#datagrid tbody").on("click", "tr", function () {
        $(this).addClass("selected");
        $(this).siblings().removeClass("selected");
    });

    $("#change-password").click(function (e) {
        e.preventDefault();
        $("#dialog-change-password input[type='reset']").trigger("click");
        $("#dialog-change-password").dialog({
            modal: true,
            width: 300,
            title: "修改密码",
            buttons: {
                "确定": function () {
                    $.ajax({
                        type: "POST",
                        url: "change-password.ashx?r=" + new Date().getTime(),
                        data: $("#dialog-change-password form").serialize(),
                        success: function (response) {
                            if (response.success) {
                                $("#dialog-change-password").dialog("close");
                                alert("修改密码成功");
                                save();
                            }
                            else alert("修改密码失败：" + response.message);
                        }
                    });
                },
                "取消": function () { $("#dialog-change-password").dialog("close"); }
            }
        });
    });

    $("#新增").click(function (e) {
        e.preventDefault();
        $("#dialog-insert input[type='reset']").trigger("click");
        $("#dialog-insert").dialog({
            modal: true,
            width: 385,
            title: "新增",
            buttons: {
                "保存并继续": function () {
                    $("#loading").show();
                    $.ajax({
                        type: "POST",
                        url: "customerJson.ashx?verb=insert&r=" + new Date().getTime(),
                        data: $("#dialog-insert form").serialize(),
                        success: function (response) {
                            $("#loading").hide();
                            if (response.success) {
                                //$("#dialog-insert").dialog("close");
                                save();
                                $("#dialog-insert").find("[name='XM'],[name='XB'],[name='CSRQ'],[name='LXFS']").val("");
                            }
                            else { alert("保存失败：" + response.message); }
                        }
                    });
                },
                "取消": function () { $("#dialog-insert").dialog("close"); }
            }
        });
    });
    $("#datagrid tbody tr").live("dblclick", function () {
        //$("#datagrid tbody").on("dblclick", "tr", function () {
        $("#编辑").trigger("click");
    });

    $("#编辑").click(function (e) {
        e.preventDefault();
        var item_index = $("#datagrid tbody tr").index($(".selected"));
        var item = app.items[item_index];
        var $dialog = $("#dialog-update");
        var $form = $dialog.find("form.text");
        if (item) {
            $dialog.dialog({
                modal: true,
                resizable: false,
                width: 630,
                title: "编辑",
                open: function (event, ui) {
                    $form.find("input,select").val("");
                    $form.loadJSON(item);
                    $("#img-photo").attr("src", "CustomerGetImage.ashx?FIELD=PHOTO&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-pass1").attr("src", "CustomerGetImage.ashx?FIELD=PASS1&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-idcard1").attr("src", "CustomerGetImage.ashx?FIELD=IDCARD1&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-idcard2").attr("src", "CustomerGetImage.ashx?FIELD=IDCARD2&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-birth").attr("src", "CustomerGetImage.ashx?FIELD=BIRTH&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-consent").attr("src", "CustomerGetImage.ashx?FIELD=CONSENT&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-household1").attr("src", "CustomerGetImage.ashx?FIELD=HOUSEHOLD1&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-household2").attr("src", "CustomerGetImage.ashx?FIELD=HOUSEHOLD2&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-household3").attr("src", "CustomerGetImage.ashx?FIELD=HOUSEHOLD3&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-household4").attr("src", "CustomerGetImage.ashx?FIELD=HOUSEHOLD4&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-other1").attr("src", "CustomerGetImage.ashx?FIELD=OTHER1&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("#img-other2").attr("src", "CustomerGetImage.ashx?FIELD=OTHER2&ID=" + item.ID + "&r=" + new Date().getTime());
                    $("form.image input[name='ID']").val(item.ID);
                },
                close: function (event, ui) {
                    $("#img-photo").attr("src", "");
                    $("#img-pass1").attr("src", "");
                    $("#img-idcard1").attr("src", "");
                    $("#img-idcard2").attr("src", "");
                    $("#img-birth").attr("src", "");
                    $("#img-consent").attr("src", "");
                    $("#img-household1").attr("src", "");
                    $("#img-household2").attr("src", "");
                    $("#img-household3").attr("src", "");
                    $("#img-household4").attr("src", "");
                    $("#img-other1").attr("src", "");
                    $("#img-other2").attr("src", "");
                    $("form.image input[name='ID']").val("");
                    $("#loading").hide();
                },
                buttons: {
                    "保存": function () {
                        if ($.trim($form.find("[name='ZJHM']").val()).length > 0) {
                            $form.find("[name='BZJD']").val("已收证");
                            $("#dialog-update input[name='CXSJ']").val("");
                        }
                        $("#loading").show();
                        $.ajax({
                            type: "POST",
                            url: "Customer.ashx?verb=update&ID=" + item.ID + "&r=" + new Date().getTime(),
                            data: $form.serialize(),
                            success: function (response) {
                                $("#loading").hide();
                                if (response.success) {
                                    $dialog.dialog("close");
                                    save();
                                }
                                else { alert("保存失败：" + response.message); }
                            }
                        });
                    },
                    "取消": function () { $dialog.dialog("close"); }
                }//buttons
            }); //dialog

            $dialog.find("div.buttonpane input[type='button']").each(function (index, element) {
                $(this).unbind();
                $(this).click(function (e) {
                    e.preventDefault();
                    $dialog.next().find("button:eq(" + index + ")").trigger("click");
                });
            });


        }
        else alert("请选择一条记录");
    });



    $("#进度查询").click(function (e) {
        e.preventDefault();
        if ($.trim($("#dialog-update input[name='ZJHM']").val()).length > 0) {
            $("#dialog-update input[name='BZJD']").val("已收证");
            $("#dialog-update input[name='CXSJ']").val("");
        } else {
            //$("#进度查询").button("disable");
            $("#loading").show();
            $("#dialog-update input[name='BZJD']").val("");
            $.ajax({
                type: "POST",
                async: "true",
                url: "BZJDCX.ashx?r=" + new Date().getTime(),
                data: $("#dialog-update form").serialize(),
                success: function (response) {
                    $("#loading").hide();
                    //$("#进度查询").button("enable");
                    if (response.success) {
                        $("#dialog-update input[name='BZJD']").val(response.message);
                        $("#dialog-update input[name='CXSJ']").val((new Date()).format("yyyy-MM-dd hh:mm"));
                    } else alert("办证进度查询失败：" + response.message);
                }
            });
        }
    });
    $("#dialog-update form.image img").click(function (e) {
        e.preventDefault();
        window.open($(this).attr("src"));
    });

    $("#dialog-update form.image input[type='file']").live("change", function () {
        //$("#dialog-update form.image").on("change", "input[type='file']", function () {
        var $form = $(this).closest("form");
        $("#loading").show();
        $.ajaxFileUpload({
            url: 'CustomerUploadImage.ashx?' + $form.serialize(),
            secureuri: false,
            dataType: 'json',
            fileElementId: $(this).attr("ID"),
            success: function (response, status) {
                $("#loading").hide();
                if (response.success) {
                    $form.find("img").attr("src", getRootPath() + response.message);
                    var FIELD = $form.find("[name='FIELD']").val();
                    $form.siblings("form.text").find("[name='" + FIELD + "']").val(response.message);
                }
                else alert("上传失败：" + response.message);
            }
        });
    });

    $("#dialog-update form.image button").click(function (e) {
        e.preventDefault();
        var $form = $(this).closest("form");

        $form.find("img").attr("src", getRootPath() + "/pub/img/noimg.png");
        var FIELD = $form.find("[name='FIELD']").val();
        $form.siblings("form.text").find("[name='" + FIELD + "']").val("/pub/img/noimg.png");
    });

    $("#删除").click(function (e) {
        e.preventDefault();
        var item_index = $("#datagrid tbody tr").index($(".selected"));
        var item = app.items[item_index];
        if (item) {
            $("#dialog-delete").dialog({
                modal: true,
                title: "警告",
                buttons: {
                    "删除": function () {
                        $.ajax({
                            type: "POST",
                            url: "Customer.ashx?verb=delete&ID=" + item.ID + "&r=" + new Date().getTime(),
                            success: function (response) {
                                if (response.success) {
                                    $("#dialog-delete").dialog("close");
                                    //alert("删除成功");
                                    save();
                                }
                                else alert("删除失败：" + response.message);
                            }
                        });
                    },
                    "取消": function () { $("#dialog-delete").dialog("close"); }
                }
            });
        }
        else alert("请选择一条记录");
    });


    $("#预约办证名单打印").click(function (e) {
        e.preventDefault();
        $("#report1 input[type='reset']").trigger("click");
        $("#report1").dialog({
            modal: true,
            width: 420,
            title: "预约办证名单打印",
            buttons: {
                "确定": function () {
                    $("#report1 form").submit();
                },
                "取消": function () { $("#report1").dialog("close"); }
            }
        });
    });

    $("#团队名单打印").click(function (e) {
        e.preventDefault();
        $("#report2 input[type='reset']").trigger("click");
        $("#report2").dialog({
            modal: true,
            width: 420,
            title: "团队名单打印",
            buttons: {
                "确定": function () {
                    $("#report2 form").submit();
                },
                "取消": function () { $("#report2").dialog("close"); }
            }
        });
    });

    $("#团队电子材料打包").click(function (e) {
        e.preventDefault();
        $("#zip input[type='reset']").trigger("click");
        $("#zip").dialog({
            modal: true,
            width: 420,
            title: "团队电子材料打包",
            buttons: {
                /*"一人一夹打包": function () {
                    $("#zip input[name='TYPE']").val("A");
                    $("#zip form").submit();
                },
                "一人一图打包": function () {
                    $("#zip input[name='TYPE']").val("B");
                    $("#zip form").submit();
                },*/
                "打包下载": function () {
                    $("#zip input[name='TYPE']").val("C");
                    $("#zip form").submit();
                },
                "取消": function () { $("#zip").dialog("close"); }
            }
        });
    });

});

function save() {
    $.ajax({ type: "POST", data: app.form,
        url: "customerJson.ashx?verb=select&page=" + app.page + "&rows=" + app.rows + "&r=" + new Date().getTime(),
        success: function (response) {
            app.success = response.success;
            app.message = response.message;
            app.total = parseInt(response.total);
            app.items = response.items;
            var sn = (app.page - 1) * app.rows;
            $.each(app.items, function () { this.sn = ++sn; });
            updateApp();
        },
        beforeSend: function () {
            $("#loading").show();
            $("#result").hide();
        },
        complete: function () {
            $("#loading").hide();
        }
    });
}

function updateApp() {
    $("#datagrid tbody").empty();
    if (!app.success) {
        alert(app.message);
    }
    else if (app.total === 0) {
        alert("未找到记录");
    }
    else {
        var tbody = "";
        $.each(app.items, function () { tbody += app.template(this); });
        $("#datagrid tbody").append(tbody).children("tr:odd").addClass("odd");

        if (app.page <= 1) {
            $("#首页, #上一页").attr("disabled", "disabled");
            $("#首页, #上一页").button("disable");
        }
        else {
            $("#首页, #上一页").removeAttr("disabled");
            $("#首页, #上一页").button("enable");
        }
        if (app.page * app.rows >= app.total) {
            $("#下一页, #尾页").attr("disabled", "disabled");
            $("#下一页, #尾页").button("disable");
        }
        else {
            $("#下一页, #尾页").removeAttr("disabled");
            $("#下一页, #尾页").button("enable");
        }
        $("#page").val(app.page);
        $("#total").text(app.total);
        $("#totalpage").text(Math.ceil(app.total / app.rows));

        $("#result").show();
    }
}