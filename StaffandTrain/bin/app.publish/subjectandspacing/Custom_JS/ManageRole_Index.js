function showpopup() {
    $(".alert").hide();
    $(".validateTips").text("");
    $("#divcreaterole").show();
    $("#divUpdaterole").hide();
    $("#divrole").text("");
    $("#oldrolename").hide();
    $("#myModalLabel").text("Create Role");
    $("#txtrolename").val("");
    $('#FileManageModal').modal('show');
}

function showpopupedit(role) {
    $(".alert").hide();
    $(".validateTips").text("");
    $("#myModalLabel").text("Update Role");
    $("#txtoldrolename").val(role);
    $("#divrole").text(role);
    $("#oldrolename").show();
    $("#divcreaterole").hide();
    $("#divUpdaterole").show();
    $("#txtrolename").val("");
    $('#FileManageModal').modal('show');
}

$(document).on("click", "a[name='btnAddFile']", function (e) {
    $("#divmsg").text("");
    $(".validateTips").text("");
    if ($("#txtrolename").val().trim() == "") {
        $('#txtrolename').nextAll(".validateTips:first").html("Enter Role Name");
        $('#FileManageModal').modal('show');
        return false;
    }
    var RoleName = $("#txtrolename").val();
    if ($('#IsAuthenticated').text() == 'True') {
        $.ajax({
            type: "POST",
            url: "/ManageRoles/Save_Role",
            data: { RoleName: RoleName },
            dataType: 'json',


            success: function (msg) {
                
                if (msg == "Success") {
                    getupdatedroles();
                    $('#FileManageModal').modal('hide');
                }
                else {
                    $('#FileManageModal').modal('show');
                    // $("#divmsg").text(msg);
                    $('#txtrolename').nextAll(".validateTips:first").html(msg);
                }


            },
            error: function (error) {

            }
        });
    }
    else {
        window.location.href = "\Login\Index";
    }


});

$(document).on("click", "a[name='btnupdaterole']", function (e) {
    
    $("#divmsg").text("");
    $(".validateTips").text("");
    if ($("#txtrolename").val().trim() == "") {
        $('#txtrolename').nextAll(".validateTips:first").html("Enter Role Name");
        $('#FileManageModal').modal('show');
        return false;
    }

    var RoleName = $("#txtrolename").val();
    var OldRoleName = $("#txtoldrolename").val();
    if ($('#IsAuthenticated').text() == 'True') {
        $.ajax({
            type: "POST",
            url: "/ManageRoles/Update_Role",
            data: { RoleName: RoleName, OldRoleName: OldRoleName },
            dataType: 'json',


            success: function (msg) {
                if (msg == "Success") {
                    getupdatedroles();
                    $('#FileManageModal').modal('hide');
                }
                else {
                    $('#FileManageModal').modal('show');
                    // $("#divmsg").text(msg);
                    $('#txtrolename').nextAll(".validateTips:first").html(msg);
                }


            },
            error: function (error) {

            }
        });
    }
    else {
        window.location.href = "\Login\Index";
    }

});


function getupdatedroles() {
    $.ajax({
        type: "POST",
        url: "/ManageRoles/updatedroles",

        async: false,
        success: function (msg) {
            if (msg != "") {
                $('#tblroles').html(msg);
            }
        },
        error: function (error) {
        }
    });
}