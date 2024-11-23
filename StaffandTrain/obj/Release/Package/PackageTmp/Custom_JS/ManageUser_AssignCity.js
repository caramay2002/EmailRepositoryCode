$("#chkselectall").click(function () {
    $(".chkin").prop('checked', $(this).prop('checked'));
});

$(".chkin").click(function () {
    
    var totalcnt = $(".chkin").length;
    var totalselectedcnt = $('input[name="chkin"]:checked').length;
    if (totalcnt == totalselectedcnt) {
        $("#chkselectall").prop('checked', true);
    }
    else {
        $("#chkselectall").prop('checked', false);
    }

});

$(document).ready(function () {

    $("#btnupdate").click(function () {
        var ids = "";
        var listids = "";
        $("#example tr").each(function () {
            ;

            var row = $.trim($(this).find("td:nth-child(2)").text());
            if (row != '') {
                if ($(this).find("td:nth-child(1)").children('.chkin').is(':checked')) {
                    var expids = $(this).find("td:nth-child(1)").children('.chkin').attr('data-listid');
                    if (ids == "") {
                        ids = expids
                    }
                    else {
                        ids = ids + "," + expids
                    }
                }
            }
        });
        var Username = $("#hdnuser").val();
        $.ajax({
            type: "POST",
            url: "/ManageUser/UpdateProspective",
            async: false,
            data: { Username: Username, listids: ids },
            success: function (msg) {
                if (msg != "" && msg != "Error") {
                    console.log(msg);
                    //  $("#Ddlcity").val("").trigger("chosen:updated");
                    $("#divmsg").text(msg);
                    $("#divmsg").show();
                }
            },
            error: function (error) {
            }
        });
    });
});