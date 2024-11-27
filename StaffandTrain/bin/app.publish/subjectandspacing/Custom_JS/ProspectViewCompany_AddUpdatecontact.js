function showpopup() {
    $(".alert").hide();
    $(".validateTips").text("");
    $("#divcreaterole").show();
    $("#divUpdaterole").hide();
    $("#divrole").text("");
    $("#myModalLabel").text("Create Prospect");
    $("#txtlistname").val("");
    $("input[name='Restricted'][value='Yes']").prop('checked', true);
    $('#FileManageModal').modal('show');
}

function showpopupedit(lsitname, listid, restricted) {
    $(".alert").hide();
    $(".validateTips").text("");
    $("#myModalLabel").text("Update Prospect");
    $("#txtlistname").val(lsitname);
    $("#divcreaterole").hide();
    $("#divUpdaterole").show();
    $("#hdnlistid").val(listid);
    var value = "No";
    if (restricted == "1") {
        value = "Yes";
    }
    else { value = "No" }

    $("input[name='Restricted'][value='" + value + "']").prop('checked', true);
    $('#FileManageModal').modal('show');
}
function getupdatedlist() {
    $.ajax({
        type: "POST",
        url: "/ProspectListsClient/updatedlist",
        async: false,
        success: function (msg) {
            if (msg != "") {
                $('#tblprospect').html(msg);

                //$('#example').DataTable({

                //});
            }

        },
        error: function (error) {
        }
    });
}


function validateEmail(Email) {
    var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (!filter.test(Email)) {
        return false;
    }
    else {
        return true;
    }
}