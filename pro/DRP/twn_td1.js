
$(document).ready(function () {
    $("input[type=submit], input[type=reset], input[type=button], a.btn").button();
    datepicker();
    var BJXXTemplate = Handlebars.compile($("#BJXXTemplate").html());

    var VERB = $("[name='VERB']").val();

    if (VERB.toUpperCase() !== "INSERT") {
        var ID = $("[name='ID']").val();
        if (!ID) {
            alert("[团队]不能为空");
            window.opener = null; window.open('', '_self'); window.close();
        }

        $.ajax({ type: "POST",
            url: "twn_td.aspx?VERB=SELECT&r=" + new Date().getTime(),
            data: { ID: ID },
            success: function (response) {
                if (response.success) {
                    var item = response.items[0];
                    if (!item) {
                        alert("[团队]不存在：" + ID);
                        window.opener = null; window.open('', '_self'); window.close();
                    }

                    $("form").loadJSON(item);
                    $("#BJSL").val(item.BJ.length);
                    var tbody = "";
                    $.each(item.BJ, function (i) { this.sn = i + 1; tbody += BJXXTemplate(this); });
                    $("#BJXX").empty().append(tbody);

                    if (VERB.toUpperCase() === "SELECT") {
                        $("[type='text']").each(function () {
                            $(this).replaceWith($('<span/>').text($(this).val()));
                        });
                        $("select").each(function () {
                            $(this).replaceWith($('<span/>').text($(this).find("option:selected").text()));
                        });
                        $("img.ui-datepicker-trigger").hide();
                        $("[type='file']").hide();

                        if (item.ZT !== "1") {
                            $("#预定").button({ disabled: true }); //.find("span").text("已关闭");
                        }
                        if (parseInt(item.SYRS) <= 0) {
                            $("#预定").button({ disabled: true }).find("span").text("已订满");
                        }
                    }
                }
                else {
                    alert("错误: " + response.message);
                    window.opener = null; window.open('', '_self'); window.close();
                }
            }
        }); //ajax
    } //if (VERB.toUpperCase() !== "INSERT") {


    $("input[type='file']").live("change", function () {
        $("#loading").show();
        $this = $(this);
        $prev = $this.prev();
        $.ajaxFileUpload({
            url: "twn_td_fj.aspx",
            secureuri: false,
            dataType: 'json',
            fileElementId: $this.attr("ID"),
            success: function (response, status) {
                $("#loading").hide();
                if (response.success) {
                    $prev.val(response.message);
                }
                else alert("上传失败：" + response.message);
            }
        });
    });

    $("#BJSL").live("blur", function () {
        $this = $(this);

        // 报价中已有数量
        var YYSL = $("#BJXX tr").length;

        // 整数判断
        var BJSL = parseInt($this.val());
        if (isNaN(BJSL)) $this.val(YYSL);
        else $this.val(BJSL);


        // 添加报价信息
        if (BJSL > YYSL) {
            for (var sn = $("#BJXX tr").length + 1; sn <= BJSL; sn++) {
                $("#BJXX").append(BJXXTemplate({ sn: sn }));
            }
        }

        // 删除报价信息
        if (BJSL < YYSL) {
            for (var sn = 1; sn <= YYSL - BJSL; sn++) {
                $("#BJXX tr").eq(-1).remove();
            }
        }
    });


});

function save(VERB) {
    $(".toolbar a").button("disable");
    $.ajax({ type: "POST",
        url: "twn_td.aspx?r=" + new Date().getTime() + "&VERB=" + VERB,
        data: $("form").serialize(),
        success: function (response) {
            if (response.success) {
                alert("操作成功");
                if (VERB === "COPY")
                    window.location.href = "twn_td1.aspx?VERB=UPDATE&ID=" + response.message;
                else
                    window.location.href = "twn_td1.aspx?VERB=SELECT&ID=" + response.message;
            }
            else {
                alert(response.message);
                $(".toolbar a").button("enable");
            }
        }
    });
} //save