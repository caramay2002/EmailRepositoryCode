$(document).ready(function () {



    $("#btnsave").click(function () {
        $(".validateTips").html("");
        if ($("#txtname").val().trim() == "") {
            $('#txtname').nextAll(".validateTips:first").html("Enter Name");
            return false;
        }
        //if ($("#txtemail").val().trim() == "") {
        //    $('#txtemail').nextAll(".validateTips:first").html("Enter Email");
        //    return false;
        //}
        if ($("#txtemail").val().trim() != "") {
            if (!validateEmail($("#txtemail").val().trim())) {
                $('#txtemail').nextAll(".validateTips:first").html("Invalid Email");
                return false;
            }

        }
        if ($("#txtlinkedinurl").val().trim() != "") {

            var url = new RegExp('^(http|https|www)://.*$');

            if (!url.test($("#txtlinkedinurl").val())) {
                $('#txtlinkedinurl').nextAll(".validateTips:first").html("Enter Valid Url");
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
