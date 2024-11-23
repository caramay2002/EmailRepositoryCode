$(document).ready(function () {
    $("#btnsave").click(function () {
        $(".validateTips").html("");
        if ($("#txtemail").val().trim() == "") {
            $('#txtemail').nextAll(".validateTips:first").html("Enter Email");
            return false;
        }
        if ($("#txtemail").val().trim() != "") {
            if (!validateEmail($("#txtemail").val().trim())) {
                $('#txtemail').nextAll(".validateTips:first").html("Invalid Email");
                return false;
            }

        }

        if ($("#txtpassword").val().trim() == "") {
            $('#txtpassword').nextAll(".validateTips:first").html("Enter Password");
            return false;
        }

        if ($("#txtpassword").val() != "") {
            if ($("#txtpassword").val().length < 5) {
                $('#txtpassword').nextAll(".validateTips:first").html("Password length should contain minimum 5 characters");
                return false;
            }
        }

    });
});
function validateEmail(Email) {
    var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (!filter.test(Email)) {
        return false;
    }
    else {
        return true;
    }
}
