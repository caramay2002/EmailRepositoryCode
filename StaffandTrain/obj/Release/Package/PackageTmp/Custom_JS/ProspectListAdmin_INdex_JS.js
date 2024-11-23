

$(document).ready(function () {
    $(".select2").select2({
        placeholder: "--Select Company--"
    });

    //$('#example').DataTable({

    //});
    ClearSearchLocalStorage();
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

        $.ajax({
            type: "POST",
            url: "/ProspectListsAdmin/Save_List",
            data: { ListName: ListName, restricted: restricted },
            dataType: 'json',
            success: function (msg) {

                if (msg == "Success") {
                    getupdatedlist();
                    $('#FileManageModal').modal('hide');
                    $('#divmsg').text('Record Inserted');
                    $('#divmsg').show();
                }
                else if (msg == 'Session Expired') {
                    window.location.href = "/Login/Index";
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
        $.ajax({
            type: "POST",
            url: "/ProspectListsAdmin/Update_List",
            data: { ListName: ListName, restricted: restricted, Listid: Listid },
            dataType: 'json',


            success: function (msg) {
                if (msg == "Success") {
                    getupdatedlist();
                    $('#FileManageModal').modal('hide');
                    $('#divmsg').text('Record Updated');
                    $('#divmsg').show();
                }
                else if (msg == 'Session Expired') {
                    window.location.href = "/Login/Index";
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

    });


});

function ClearSearchLocalStorage() {
    localStorage.CitycircleVal = "";
    localStorage.biztypeVal = "";
    localStorage.titleVal = "";
    localStorage.nameVal = "";
    localStorage.NotesVal = "";
    localStorage.ListIdVal = "";
}
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
        url: "/ProspectListsAdmin/updatedlist",

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


var listid = $("#hiddenlistid").html();

if (listid != "" && listid != null) {
    $("#" + listid).focus();
}

// Functionality for Moving one list's companies and their contacts with notes to another list [START]
function showpopup_Move(listid, listName) {
    $("#hdnlistid").val("");
    $("#ddlOtherList").val("");
    $("#ddlCompaniesInList").children().remove();
    $("#ddlCompaniesInList").select2("val", "");
    $("#divMessage_ListName").text("");
    $("#divMessage_ddlCompaniesInList").text("");
    $("#divMessage_ddlOtherList").text("");

    $.ajax({
        type: "GET",
        url: "/ProspectListsAdmin/GetDataForMoveFunctionality",
        data: { ListID: listid },
        async: false,
        success: function (result) {
            $("#hdnlistid").val(listid);
            $("#ListName").val(listName);

            $.each(result.ListCompany, function (data, value) {
                $("#ddlCompaniesInList").append($("<option></option>").val(value.companyid).html(value.name));
            });

            $.each(result.listName, function (data, value) {
                $("#ddlOtherList").append($("<option></option>").val(value.listid).html(value.listname));
            });

            $("#MoveListDataModal").modal();
        },
        error: function (error) {
            console.log(error);
        }
    });
}

function moveListDataToAnotherList() {
    var moveFrom_ListID = parseInt($("#hdnlistid").val());
    var moveTo_ListID = parseInt($("#ddlOtherList").val());

    var all_company_ids = [];
    var company_ids = $("#ddlCompaniesInList").val();
    $.each(company_ids, function (index, value) {
        if (value !== "") {
            all_company_ids.push(value);
        }
    });

    all_company_ids = all_company_ids.join(",").toString();

    if (!(moveFrom_ListID > 0)) {
        $("#divMessage_ListName").text("List Id is empty. Please try again.");
        $("#divMessage_ListName").show();
        return false;
    }
    else {
        $("#divMessage_ListName").text("");
        $("#divMessage_ListName").hide();
    }

    if (all_company_ids === "") {
        $("#divMessage_ddlCompaniesInList").text("Please select the company(s) you want to move from dropdown list.");
        $("#divMessage_ddlCompaniesInList").show();
        return false;
    }
    else {
        $("#divMessage_ddlCompaniesInList").text("");
        $("#divMessage_ddlCompaniesInList").hide();
    }

    if (!(moveTo_ListID > 0)) {
        $("#divMessage_ddlOtherList").text("Please select the list from dropdown list.");
        $("#divMessage_ddlOtherList").show();
        return false;
    }
    else {
        $("#divMessage_ddlOtherList").text("");
        $("#divMessage_ddlOtherList").hide();
    }

    $.ajax({
        type: "POST",
        url: "/ProspectListsAdmin/MoveListDataToAnotherList",
        data: { ListID_from: moveFrom_ListID, companyIds: all_company_ids, ListID_to: moveTo_ListID },
        async: false,
        success: function (result) {
            debugger;
            if (result === true) {
                $("#MoveListDataModal").modal('hide');
                getupdatedlist();
                $('#divmsg').text('Companies moved successfully.');
                $('#divmsg').show();
                $('#div_error_msg').hide();
                //$("#divmsg").fadeOut(5000);
            }
            else {
                $("#MoveListDataModal").modal('hide');
                getupdatedlist();
                $('#div_error_msg').text('Something went wrong. Please try again.');
                $('#div_error_msg').show();
                $('#divmsg').hide();
                //$("#divmsg").fadeOut(5000);
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
}
// Functionality for Moving one list's companies and their contacts with notes to another list [END]