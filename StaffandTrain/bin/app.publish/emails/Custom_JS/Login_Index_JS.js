 
        $(document).ready(function () { 
            $("#btnsave").click(function () {
                $(".validateTips").html("");
                if ($("#txtusername").val().trim() == "") {
                    $('#txtusername').nextAll(".validateTips:first").html("Enter UserName");
                    return false;
                }
                if ($("#txtpassword").val().trim() == "") {
                    $('#txtpassword').nextAll(".validateTips:first").html("Enter Password");
                    return false;
                }

            });
        });

