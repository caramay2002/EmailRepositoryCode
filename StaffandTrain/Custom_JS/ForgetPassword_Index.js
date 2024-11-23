
    $(document).ready(function () {
        $("#btnsave").click(function () {
            $(".validateTips").html("");
            if ($("#txtusername").val().trim() == "") {
                $('#txtusername').nextAll(".validateTips:first").html("Enter UserName");
                return false;
            }

        });
    });
