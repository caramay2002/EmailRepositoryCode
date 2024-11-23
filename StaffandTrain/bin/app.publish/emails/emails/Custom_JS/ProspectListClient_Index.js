$(document).ready(function () {


    $(document).on("click", "a[name='btnAddFile']", function (e) {
        $("#divmsg").text("");
        $(".validateTips").text("");
        if ($("#txtlistname").val().trim() == "") {
            $('#txtlistname').nextAll(".validateTips:first").html("Enter Prospect Name");
            $('#FileManageModal').modal('show');
            return false;
        }
        var ListName = $("#txtlistname").val();
        var restricted = $('input[name=Restricted]:checked').val();
        if ($('#IsAuthenticated').text() == 'True') {
            $.ajax({
                type: "POST",
                url: "/ProspectListsClient/Save_List",
                data: { ListName: ListName, restricted: restricted },
                dataType: 'json',
                success: function (msg) {
                    
                    if (msg == "Success") {
                        getupdatedlist();
                        $('#FileManageModal').modal('hide');
                        $('#divmsg').text('Record Inserted');
                        $('#divmsg').show();
                    }
                    else {
                        $('#FileManageModal').modal('show');
                        // $("#divmsg").text(msg);
                        $('#txtlistname').nextAll(".validateTips:first").html(msg);
                        $('#divmsg').text('');
                        $('#divmsg').hide();
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
        if ($("#txtlistname").val().trim() == "") {
            $('#txtlistname').nextAll(".validateTips:first").html("Enter Prospect Name");
            $('#FileManageModal').modal('show');
            return false;
        }

        var ListName = $("#txtlistname").val();
        var restricted = $('input[name=Restricted]:checked').val()
        var Listid = $("#hdnlistid").val();
        if ($('#IsAuthenticated').text() == 'True') {
            $.ajax({
                type: "POST",
                url: "/ProspectListsClient/Update_List",
                data: { ListName: ListName, restricted: restricted, Listid: Listid },
                dataType: 'json',


                success: function (msg) {
                    if (msg == "Success") {
                        getupdatedlist();
                        $('#FileManageModal').modal('hide');
                        $('#divmsg').text('Record Updated');
                        $('#divmsg').show();
                    }
                    else {
                        $('#FileManageModal').modal('show');
                        // $("#divmsg").text(msg);
                        $('#txtlistname').nextAll(".validateTips:first").html(msg);
                        $('#divmsg').text('');
                        $('#divmsg').hide();
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


});


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