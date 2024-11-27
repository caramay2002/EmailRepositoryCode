
    $(document).ready(function () {
        $("#btnsave").click(function () {
            $(".validateTips").html("");
            if ($("#txtpassword").val().trim() == "") {
                $('#txtpassword').nextAll(".validateTips:first").html("Enter Password");
                return false;
            }
            if ($("#txtconpassword").val().trim() == "") {
                $('#txtconpassword').nextAll(".validateTips:first").html("Enter Confirm Password");
                return false;
            }
            if ($("#txtconpassword").val().trim() != "") {
                if ($("#txtconpassword").val().trim() != $("#txtpassword").val().trim()) {

                    $('#txtconpassword').nextAll(".validateTips:first").html("Password & Confirm Password does not match" );
                    return false;
                } 

            }




        });
    });

