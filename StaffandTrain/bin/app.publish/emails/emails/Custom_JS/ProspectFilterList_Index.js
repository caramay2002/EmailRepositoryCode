$(document).ready(function () {
    // getlist();
    $("#lnksearch").click(function () {
        
        getlist();
    });

    $("#lnkclear").click(function () {
        $("#ddlcitycircle").val("");
        $("#ddlbiztype").val("");
        $("#ddltitle").val("Select Title").trigger("chosen:updated");
        $("#txtname").val("");
        $("#txtemaillst").val("")
        $("#lblcount").text("");
    });



});
function getlist() {
    var citycircle = $("#ddlcitycircle").val();
    var biztype = $("#ddlbiztype").val();
    var name = $("#txtname").val().trim();
    var title = "";
    if ($("#ddltitle").val() != null) {
        var arr = $("#ddltitle").val();
        title = (arr.join(", "));
    }

    $.ajax({
        type: "POST",
        url: "/ProspectFilterList/Prospectlistsearch",
        async: false,
        data: { CityCircle: citycircle, Biztype: biztype, TitleStandard: title, search: name },
        success: function (msg) {
            var emaildata = "";

            for (var i = 0; i < msg.data.length; i++) {

                emaildata = emaildata + msg.data[i] + "\n";
                if (i != 0) {
                    if (Math.ceil(i % 50) === 0) {

                        emaildata = emaildata + "--------" + "\n"
                    }
                }

            }
            $("#txtemaillst").val(emaildata);
            if (msg.str == "0") {
                $("#txtemaillst").val("No record found");

            }
            $("#lblcount").text("Total Count :- " + msg.str);

        },
        error: function (error) {
        }
    });
}


