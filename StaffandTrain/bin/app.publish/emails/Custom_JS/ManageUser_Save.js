$(document).ready(function () {
    var dataroles = $("#hdnrolesaved").val().split(",");
    $("#ddlroles").val(dataroles).trigger("chosen:updated");

    $("#txtname").blur(function () {
        checkusename();
    });

    $("#btnsave").click(function () {
        $(".validateTips").html("");
        if ($("#txtname").val().trim() == "") {
            $('#txtname').nextAll(".validateTips:first").html("Enter UserName");
            return false;
        }
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

        if ($("#txtuserid").val().trim() == "") {
            if ($("#txtpassword").val().trim() == "") {
                $('#txtpassword').nextAll(".validateTips:first").html("Enter Password");
                return false;
            }
            if ($("#txtpassword").val().length < 5) {
                $('#txtpassword').nextAll(".validateTips:first").html("Password length should contain minimum 5 characters");
                return false;
            }

            if ($("#txtconpass").val().trim() == "") {
                $('#txtconpass').nextAll(".validateTips:first").html("Enter Confirm Password");
                return false;
            }
        }
        else {
            if ($("#txtpassword").val() != "") {
                if ($("#txtpassword").val().length < 5) {
                    $('#txtpassword').nextAll(".validateTips:first").html("Password length should contain minimum 5 characters");
                    return false;
                }

                if ($("#txtconpass").val().trim() == "") {
                    $('#txtconpass').nextAll(".validateTips:first").html("Enter Confirm Password");
                    return false;
                }
            }
        }
        if ($("#txtconpass").val().trim() != "") {
            if ($("#txtconpass").val().trim() != $("#txtpassword").val().trim()) {
                $('#txtconpass').nextAll(".validateTips:first").html("Password and Confirm password does not match");
                return false;
            }

        }
        if ($("#ddlroles").val() == "" || $("#ddlroles").val() == null) {
            $('#ddlroles').nextAll(".validateTips:first").html('Select Role');
            chkvalidation = 0;
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

function checkusename() {
    var Username = $("#txtname").val().trim();
    var action = "";
    var userid = ""
    if ($("#txtuserid").val().trim() == "") {
        action = "Insert";
    }
    else {
        action = "Update";
        userid = $("#txtuserid").val().trim();
    }
    if (Username != "") {
        $.ajax({
            url: '/ManageUser/checkuser',
            type: 'POST',
            data: { Username: Username, action: action, userid: userid },
            async: false,
            success: function (response) {

                if (response == 0) {
                    $('#txtname').nextAll(".validateTips:first").html("");
                    $('#btnsave').removeAttr("disabled", "disabled");
                }
                else {
                    $('#txtname').nextAll(".validateTips:first").html('User already exist');
                    $('#btnsave').attr("disabled", "disabled");

                    //alert("Please verify yourself by click on link on verification mail to continue");
                }



            }
        });
    }
}