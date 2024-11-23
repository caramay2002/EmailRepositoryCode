$(document).ready(function () {
    //$('#example').DataTable({

    //});
});
function showpopupedit(WBId, WhiteBoardName) {

    $(".validateTips").text("");
    $("#hdnwbid").val(WBId);
    $("#txtwbname").val(WhiteBoardName);
    $('#FileManageModal').modal('show');
}

$("#btnupdate").click(function () {
    $(".validateTips").text("");
    if ($("#txtwbname").val().trim() == "") {
        $('#txtwbname').nextAll(".validateTips:first").html("Enter White Board Name");
        $('#FileManageModal').modal('show');
        return false;
    }
    else {
        return true;
    }
});