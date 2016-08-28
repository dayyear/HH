function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

Date.prototype.format = function (format) //author: meizz
{
    var o = {
        "M+": this.getMonth() + 1, //month
        "d+": this.getDate(),    //day
        "h+": this.getHours(),   //hour
        "m+": this.getMinutes(), //minute
        "s+": this.getSeconds(), //second
        "q+": Math.floor((this.getMonth() + 3) / 3),  //quarter
        "S": this.getMilliseconds() //millisecond
    }
    if (/(y+)/.test(format)) format = format.replace(RegExp.$1,
    (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o) if (new RegExp("(" + k + ")").test(format))
        format = format.replace(RegExp.$1,
      RegExp.$1.length == 1 ? o[k] :
        ("00" + o[k]).substr(("" + o[k]).length));
    return format;
};

//js获取网站根路径(站点及虚拟目录)，获得网站的根目录或虚拟目录的根地址      
function getRootPath() {
    var strFullPath = window.document.location.href;
    var strPath = window.document.location.pathname;
    var pos = strFullPath.indexOf(strPath);
    var prePath = strFullPath.substring(0, pos);
    var postPath = strPath.substring(0, strPath.substr(1).indexOf('/') + 1);
    return (prePath + postPath);
}

function datepicker() {
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
}

/**
* http://papermashup.com/read-url-get-variables-withjavascript/
* For Url http://papermashup.com/index.php?id=123&page=home
* All we’d need to do to get the parameters id and page are to call this:
*   var first = getUrlVars()["id"];
*   var second = getUrlVars()["page"];
*   alert(first);
*   alert(second);
*/
function getUrlVars() {
    var vars = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi, function (m, key, value) {
        vars[key] = value;
    });
    return vars;
}