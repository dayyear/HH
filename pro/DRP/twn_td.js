$(document).ready(function () {

    $("input[type=submit], input[type=reset], input[type=button], a.btn").button();
    datepicker();
    var resultTemplate = Handlebars.compile($("#resultTemplate").html());

    var rows = parseInt($("[name='rows']").val());
    var page = parseInt($("[name='page']").val());
    
    $.ajax({ type: "POST",
        url: "twn_td.aspx?VERB=SELECT&r=" + new Date().getTime(),
        data: $("form").serialize(),
        success: function (response) {
            if (response.success) {
                var sn = (page - 1) * rows;
                var tbody = "";
                $.each(response.items, function () { this.sn = ++sn; tbody += resultTemplate(this); });
                $("#datagrid tbody").append(tbody).children("tr:odd").addClass("odd");

                if (page <= 1) {
                    $("#首页, #上一页").attr("disabled", "disabled");
                    $("#首页, #上一页").button("disable");
                }
                else {
                    $("#首页, #上一页").removeAttr("disabled");
                    $("#首页, #上一页").button("enable");
                }
                if (page * rows >= response.total) {
                    $("#下一页, #尾页").attr("disabled", "disabled");
                    $("#下一页, #尾页").button("disable");
                }
                else {
                    $("#下一页, #尾页").removeAttr("disabled");
                    $("#下一页, #尾页").button("enable");
                }


                $("#total").text(response.total);
                $("#page").text(page);
                $("#totalpage").text(Math.ceil(response.total / rows));

            }
            else alert(response.message);
        }
    }); //ajax

    $("#首页").click(function (e) {
        e.preventDefault();
        $("[name='page']").val(1);
        $("form").submit();
    });
    $("#上一页").click(function (e) {
        e.preventDefault();
        $("[name='page']").val(page - 1);
        $("form").submit();
    });
    $("#下一页").click(function (e) {
        e.preventDefault();
        $("[name='page']").val(page + 1);
        $("form").submit();
    });
    $("#尾页").click(function (e) {
        e.preventDefault();
        $("[name='page']").val(Math.ceil(parseInt($("#total").text()) / rows));
        $("form").submit();
    });

});
