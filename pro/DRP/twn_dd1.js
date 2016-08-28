
$(document).ready(function () {
    $("input[type=submit], input[type=reset], input[type=button], a.btn").button();
    //$(".box").tabs();
    datepicker();
    //var XLXXTemplate = Handlebars.compile($("#XLXXTemplate").html());
    var FYJSTemplate = Handlebars.compile($("#FYJSTemplate").html());
    var YKMDTemplate = Handlebars.compile($("#YKMDTemplate").html());

    var VERB = $("[name='VERB']").val();

    if (VERB.toUpperCase() === "INSERT") {

        var TD_ID = $("[name='TD_ID']").val();
        if (!TD_ID) {
            alert("[团队]不能为空");
            window.opener = null; window.open('', '_self'); window.close();
        }

        $.ajax({ type: "POST",
            url: "twn_td.aspx?VERB=SELECT&r=" + new Date().getTime(),
            data: { ID: TD_ID },
            success: function (response) {
                if (response.success) {
                    var item = response.items[0];
                    if (!item) {
                        alert("[团队]不存在：" + TD_ID);
                        window.opener = null; window.open('', '_self'); window.close();
                    }

                    $("form").loadJSON(item);

                    var tbody = "";
                    $.each(item.BJ, function (i) { this.sn = i + 1; tbody += FYJSTemplate(this); });
                    $("#FYJS tbody").empty().append(tbody);

                    $(".RS,.RS_DFC").val(0);
                    $(".FYXJ,.FYHJ,.FLXJ").text("0.00");

                    $("#XLXX").find("[type='text']").each(function () {
                        $(this).replaceWith($('<span/>').text($(this).val()));
                    });
                    $("#XLXX").find("select").each(function () {
                        $(this).replaceWith($('<span/>').text($(this).find("option:selected").text()));
                    });
                    $("[name='DFC']").replaceWith($("<span id='DFC'/>").text($("[name='DFC']").val()));
                    $("img.ui-datepicker-trigger").hide();

                }
                else {
                    alert("错误: " + response.message);
                    window.opener = null; window.open('', '_self'); window.close();
                }
            }
        }); //ajax
    } //INSERT
    else {
        var ID = $("[name='ID']").val();
        if (!ID) {
            alert("[订单]不能为空");
            window.opener = null; window.open('', '_self'); window.close();
        }

        $.ajax({ type: "POST",
            url: "twn_dd.aspx?VERB=SELECT&r=" + new Date().getTime(),
            data: { ID: ID },
            success: function (response) {
                if (response.success) {

                    var item = response.items[0];
                    if (!item) {
                        alert("[订单]不存在：" + ID);
                        window.opener = null; window.open('', '_self'); window.close();
                    }
                    $("form").loadJSON(item);

                    var tbody = "";
                    $.each(item.RS, function (i) { this.sn = i + 1; tbody += FYJSTemplate(this); });
                    $("#FYJS tbody").empty().append(tbody);

                    tbody = "";
                    $.each(item.MD, function (i) {
                        this.sn = i + 1;
                        tbody += YKMDTemplate(this);
                    });
                    $("#YKMD tbody").empty().append(tbody);
                    $.each($("#YKMD tbody tr"), function (i) { $(this).loadJSON(item.MD[i]) });
                    datepicker();

                    $("#XLXX, #YDXX, #FYJS").find("[type='text']").each(function () {
                        $(this).replaceWith($('<span/>').text($(this).val()));
                    });
                    $("#XLXX, #YDXX, #FYJS").find("select.flat").each(function () {
                        $(this).replaceWith($('<span/>').text($(this).find("option:selected").text()));
                    });

                    $("[name='TD_DFC']").replaceWith($("<span id='DFC'/>").text($("[name='TD_DFC']").val()));
                    $("#XLXX img.ui-datepicker-trigger").hide();

                    // 计算费用合计
                    //var FYHJ = 0.0;
                    //$(".FYXJ").each(function () { FYHJ += parseFloat($(this).text()); });
                    //$("#FYHJ").text(FYHJ.toFixed(2));
                }
                else {
                    alert("错误: " + response.message);
                    window.opener = null; window.open('', '_self'); window.close();
                }

            }
        }); //ajax
    }




    $(".RS,.RS_DFC").live("blur", function () {
        var $this = $(this);

        // 整数判断
        var i = parseInt($this.val());
        if (isNaN(i)) $this.val(0);
        else $this.val(Math.abs(i));

        // 判断单房差人数是否小于人数
        var $tr = $this.closest("tr");
        if (parseInt($tr.find(".RS").val()) < parseInt($tr.find(".RS_DFC").val())) {
            if ($this.hasClass("RS")) $this.val($tr.find(".RS_DFC").val());
            else $this.val($tr.find(".RS").val());
        }

        // 计算总人数
        var ZRS = 0;
        $(".RS").each(function () { ZRS += parseInt($(this).val()); });

        // 名单中已有人数
        var MDRS = $("#YKMD tbody tr").length;

        // 添加游客名单
        if (ZRS > MDRS) {
            for (var sn = $("#YKMD tbody tr").length + 1; sn <= ZRS; sn++) {
                $("#YKMD tbody").append(YKMDTemplate({ sn: sn }));
            }
            datepicker();
        }

        // 删除游客名单
        if (ZRS < MDRS) {
            for (var sn = 1; sn <= MDRS - ZRS; sn++) {
                $("#YKMD tbody tr").eq(-1).remove();
            }
        }

        // 计算费用小记
        var $tr = $this.closest("tr");
        var FYXJ = $tr.find(".MSJ").text() * $tr.find(".RS").val() + $("#DFC").text() * $tr.find(".RS_DFC").val();
        var FLXJ = $tr.find(".FL").text() * $tr.find(".RS").val();
        $tr.find(".FYXJ").text(FYXJ.toFixed(2));
        $tr.find(".FLXJ").text(FLXJ.toFixed(2));

        // 计算费用合计
        var FYHJ = 0.0;
        var FLHJ = 0.0;
        $(".FYXJ").each(function () {
            FYHJ += parseFloat($(this).text());
        });
        $(".FLXJ").each(function () {
            FLHJ += parseFloat($(this).text());
        });
        $("#FYHJ").text(FYHJ.toFixed(2));
        $("#JSJG").text((FYHJ - FLHJ).toFixed(2));
    });

});

function save() {
    $.ajax({ type: "POST",
        url: "twn_dd.aspx?r=" + new Date().getTime(),
        data: $("form").serialize(),
        success: function (response) {
            if (response.success) {
                alert("保存成功");
                window.location.href = "twn_dd1.aspx?VERB=UPDATE&ID=" + response.message;
            }
            else {
                alert(response.message);
            }
        }
    });
} //save

function cancel() {
    $.ajax({ type: "POST",
        url: "twn_dd.aspx?r=" + new Date().getTime() + "&ZT=0",
        data: $("form").serialize(),
        success: function (response) {
            if (response.success) {
                alert("保存成功");
                window.location.href = "twn_dd1.aspx?VERB=UPDATE&ID=" + response.message;
            }
            else {
                alert(response.message);
            }
        }
    });
} //cancel