$(document).ready(function () {
    $('#example').DataTable({

    });
    $("#btnupdate").click(function () {
        var listids = $("#Ddlcity").val();
        var Username = $("#hdnuser").val();
        if ($('#IsAuthenticated').text() == 'True') {
            $.ajax({
                type: "POST",
                url: "/ManageUser/UpdateProspective",
                async: false,
                data: { Username: Username, listids: listids },
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
        }
        else {
            window.location.href = "\Login\Index";
        }

    });

});

function showpopup(UserName) {
    //$(".alert").hide();
    //$(".validateTips").text("");
    //$("#divcreaterole").show();
    //$("#divUpdaterole").hide();
    //$("#divrole").text("");
    //$("#oldrolename").hide();
    $("#Ddlcity").val("").trigger("chosen:updated");
    $("#myModalLabel").text(UserName);
    $("#hdnuser").val(UserName);
    $("#divmsg").hide();
    $("#Ddlcity").val("");
    $('#FileManageModal').modal('show');

}