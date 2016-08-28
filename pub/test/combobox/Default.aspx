<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="pub_test_combobox_Default" %>

<!DOCTYPE html>
<!--[if lt IE 7]><html class="ie6"><![endif]-->
<!--[if IE 7]><html class="ie7"><![endif]-->
<!--[if IE 8]><html class="ie8"><![endif]-->
<!--[if IE 9]><html class="ie9"><![endif]-->
<!--[if (gt IE 9)|!(IE)]><!--><html><!--<![endif]-->
<head runat="server">
    <title>combobox</title>
    <link href="../../themes/cupertino/jquery-ui.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .noTitleStuff .ui-dialog-titlebar {display:none}
        .noTitleStuff .ui-dialog-content {margin:0;padding:0;}
        .noTitleStuff ul{margin:0;padding:0;list-style:none;}
    </style>

    <script src="../../js/jquery-1.8.3.min.js" type="text/javascript"></script>
    <script src="../../js/jquery-ui.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function() {
            var availableTags = [
                "ActionScript",
                "AppleScript",
                "Asp",
                "BASIC",
                "C",
                "C++",
                "Clojure",
                "COBOL",
                "ColdFusion",
                "Erlang",
                "Fortran",
                "Groovy",
                "Haskell",
                "Java",
                "JavaScript",
                "Lisp",
                "Perl",
                "PHP",
                "Python",
                "Ruby",
                "Scala",
                "Scheme"
            ];

            function split(val) {
                return val.split(/,\s*/);
            }

            function extractLast(term) {
                return split(term).pop();
            }

            $("#country")
                // don't navigate away from the field on tab when selecting an item
                .bind("keydown", function(event) {
                    if (event.keyCode === $.ui.keyCode.TAB &&
                        $(this).autocomplete("instance").menu.active) {
                        event.preventDefault();
                    }
                })
                .autocomplete({
                    minLength: 0,
                    autoFocus: true,
                    source: function(request, response) {
                        // delegate back to autocomplete, but extract the last term
                        response($.ui.autocomplete.filter(
                            availableTags, extractLast(request.term)));
                    },
                    focus: function() {
                        // prevent value inserted on focus
                        //return false;
                    },
                    select: function(event, ui) {
                        var terms = split(this.value);
                        // remove the current input
                        terms.pop();
                        // add the selected item
                        terms.push(ui.item.value);
                        // add placeholder to get the comma-and-space at the end
                        terms.push("");
                        this.value = terms.join(", ");
                        return false;
                    }
                });
        });
    </script>
</head>
<body>
    <h1>combobox</h1>
    <div class="combobox">
        <input type="text" name="country" id="country"/>
        <div class="ui-helper-hidden">
            
                <div><input type="checkbox" id="country_CHN" name="country_CHN"/><label for="country_CHN">中国</label></div>
                <div><input type="checkbox" id="country_USA" name="country_USA"/><label for="country_USA">美国</label></div>
            
        </div>
    </div>
</body>
</html>
