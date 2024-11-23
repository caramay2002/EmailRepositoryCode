$(document).ready(function () {
    var queryattr = $("#lnkconexport").attr('href');

    queryattr = queryattr + "&biztype=" + $("#ddlbiztypecon").val();
    $("#lnkconexport").attr('href', '/ProspectViewListClient/exporttocontactexcel?listid=' + $("#lbllistid").text() + '&biztype=' + $("#ddlbiztypecon").val());
    $("#ddlbiztypecon").change(function () {
        $(".loading").show();
        $(".validateTips").text("");
        var queryattr = $("#lnkconexport").attr('href');
        if ($("#ddlbiztypecon").val() != "") {
            queryattr = queryattr + "&biztype=" + $("#ddlbiztypecon").val();
            //$("#lnkconexport").attr('href', queryattr);
            $("#lnkconexport").attr('href', '/ProspectViewListClient/exporttocontactexcel?listid=' + $("#lbllistid").text()  + '&biztype=' + $("#ddlbiztypecon").val());
            setTimeout(function () { $(".loading").hide(); }, 5000);
        }
        else {
            $(".loading").hide();
        }

    });
    var listid = $("#divlistid").attr('data-listid');
    //var vars = [], hash;
    //var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    //for (var i = 0; i < hashes.length; i++) {
    //    hash = hashes[i].split('=');
    //    listid = hash[1];

    //}
    if (listid != "") {
        $("#lnkcreate").attr('href', '/ProspectViewListClient/SaveCompany?compid=' + listid);

    }
    getcompanylist(listid);
    $("#lnksearch").click(function () {
        showLoader();
        
        var listid = $("#divlistid").attr('data-listid');
        getcompanylist(listid);
    });
    //$("#lnkconexport").click(function () {

    //    if ($("#ddlbiztypecon").val() == "") {
    //        $('#ddlbiztypecon').nextAll(".validateTips:first").html("Select BizType");
    //        return false;
    //    }
    //    else {
    //        return true;
    //    }
    //});

});

function getcompanylist(listid) {
    var citycircle = $("#ddlcitycircle").val();
    var biztype = $("#ddlbiztype").val();
    var title = $("#ddltitle").val();
    var name = $("#txtname").val().trim();
    var Notes = $("#txtnotes").val().trim();
    if ($('#IsAuthenticated').text() == 'True') {
        $.ajax({
            type: "POST",
            url: "/ProspectViewListClient/GetCompanyList",
            async: false,
            data: { listid: listid, citycircle: citycircle, biztype: biztype, title: title, name: name, Notes: Notes },
            beforeSend: function () { showLoader(); },
            success: function (msg) {
                $("#tblpros").html(msg);
                hideLoader();

            },
            error: function (error) {
            }
        });
    }
    else {
        window.location.href = "\Login\Index";
    }
}

function showLoader() {
    $(".loading").show()

}

function hideLoader() {
    setTimeout(function () {
        $(".loading").hide();
    }, 1000);
}