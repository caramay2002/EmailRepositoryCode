
//$(window).unload(function () {
//    alert("In")
//});

//$(document).on('click', '.details', function () {
//    
//    $.mobile.navigate("#second", {
//         transition: "slide"
//    });
//});

//$(document).ready(function () {
$(function () {
    ClearSearchLocalStorage();
    var listid = $("#divlistid").attr('data-listid');

    var BizTValTxt = localStorage.biztypeVal;
    var CityCirValTxt = localStorage.CitycircleVal;
    var TitalValTxt = localStorage.titleVal;
    var NameValTxt = localStorage.nameVal;
    var NotsValTxt = localStorage.NotesVal;
    var EmailValTxt = localStorage.EmailVal;
    if (BizTValTxt || CityCirValTxt || TitalValTxt || NameValTxt || NotsValTxt || EmailValTxt)
    {
        $('#ddlbiztype').val(localStorage.biztypeVal);

        $("#ddlcitycircle").val(localStorage.CitycircleVal);
        $("#ddltitle").val(localStorage.titleVal);
        $("#txtname").val(localStorage.nameVal);
        $("#txtnotes").val(localStorage.NotesVal);
        $("#txtemail").val(localStorage.emailVal);

        //var RetList = getUrlVars();

        $.ajax({
            type: "POST",
            url: "/ProspectViewList/GetCompanyList",
            async: false,
            data: { listid: localStorage.ListIdVal, citycircle: localStorage.CitycircleVal, biztype: localStorage.biztypeVal, title: localStorage.titleVal, name: localStorage.nameVal, Notes: localStorage.NotesVal, email: localStorage.EmailVal },
            beforeSend: function () { showLoader(); },


            success: function (result) {
                var IsRoleAdmin = $('#IsRoleAdmin').text();
                var table = '<table id="WebGrid" class="table table-striped table-bordered">';
                //table += '<tr><th>@Resource.FamilyMemberName</th><th>@Resource.SocialId</th><th>@Resource.Relation</th><th>@Resource.Email</th><th>@Resource.Phone</th></tr>';

                table += '<th>Biz Type</th>'
                table += '<th>CompanyName</th>'
                table += '<th>User Actions</th>'

                for (var i = 0; i < result.length; i++) {

                    var companyId = result[i].companyid;
                    var priority = result[i].priority;
                    var target = result[i].target;

                    if (priority == true) {
                        table += '<tr class=Str_blue>';
                    }
                    else if (target == true) {
                        table += '<tr class = Str_pink>';
                    }
                    else {
                        table += '<tr>'
                    }

                    table += '<td>' + result[i].biztype + '</td>';
                   
                        table += '<td><a id=' + result[i].companyid + ' href=/ProspectViewCompany/Index?Compid=' + result[i].companyid + ' class=details> ' + result[i].name + '</a></td>';
                    
                    if (IsRoleAdmin == "True") {
                        table += '<td><a id=' + result[i].companyid + ' href=/ProspectViewList/SaveCompany?companyidedit=' + result[i].companyid + ' class="details AnyAction"> <i class="fa fa-pencil-square-o"></i></a>  <a href=/ProspectViewList/DeleteCompany?companyidedit=' + result[i].companyid + '&listid=' + result[i].listid + ' class="btn Delete AnyAction"><i class="fa fa-trash-o"></i></a></td>';
                    }
                    else {
                        table += '<td><a id=' + result[i].companyid + ' href=/ProspectViewList/SaveCompany?companyidedit=' + result[i].companyid + ' class="details AnyAction"> <i class="fa fa-pencil-square-o"></i></a></td>';
                    }
                    //table += '<td>' + result[i].priority + '</td>';
                    //table += '<td>' + result[i].target + '</td>';
                    table += '</tr>';
                }

                table += '</table>';
                $("#tblpros").html(table);
                hideLoader();
                //$('#example').DataTable({
                //});
                // $("#divLoading").removeClass('show');;
            },

            error: function (error) {
            },

        });
    }
    else
    {
        getcompanylist(listid);

    }

    $("#lnksearch").click(function () {

        showLoader();
        var listid = $("#divlistid").attr('data-listid');

        getcompanylist(listid);
    });
    $("#ddlbiztypecon").change(function () {
        $(".loading").show();

        $(".alert-success").hide();
        $(".validateTips").text("");
        var queryattr = $("#lnkconexport").attr('href');
        if ($("#ddlbiztypecon").val() != "") {

            queryattr = "/ProspectViewList/exporttocontactexcel?listid=" + $("#divlistid").attr('data-listid') + "&biztype=" + $("#ddlbiztypecon").val();
            $("#lnkconexport").attr('href', queryattr);
            setTimeout(function () { $(".loading").hide(); }, 5000);

        }
        else {
            $(".loading").hide();
        }


    });




    //var vars = [], hash;
    //var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    //for (var i = 0; i < hashes.length; i++) {
    //    hash = hashes[i].split('=');
    //    listid = hash[1];

    //}
    if (listid) {
        $("#lnkcreate").attr('href', '/ProspectViewList/SaveCompany?compid=' + listid);

    }







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
function ClearSearchLocalStorage() {
    localStorage.CitycircleVal = "";
    localStorage.biztypeVal = "";
    localStorage.titleVal = "";
    localStorage.nameVal = "";
    localStorage.NotesVal = "";
    localStorage.ListIdVal = "";
}
function getcompanylist(listid) {
    //
    if ($('#IsAuthenticated1').text() == 'True') {
        ClearSearchLocalStorage();
        var citycircle = $("#ddlcitycircle").val();
        var biztype = $("#ddlbiztype").val();
        var title = $("#ddltitle").val();
        var name = $("#txtname").val().trim();
        var Notes = $("#txtnotes").val().trim();
        var Email = $("#txtemail").val().trim();


        localStorage.CitycircleVal = $("#ddlcitycircle").val();
        localStorage.biztypeVal = $("#ddlbiztype").val();
        localStorage.titleVal = $("#ddltitle").val();
        localStorage.nameVal = $("#txtname").val().trim();
        localStorage.NotesVal = $("#txtnotes").val().trim();
        localStorage.emailVal = $("#txtemail").val().trim();
        localStorage.ListIdVal = listid;
        $.ajax({
            type: "POST",
            url: "/ProspectViewList/GetCompanyList",
            async: false,
            data: { listid: listid, citycircle: citycircle, biztype: biztype, title: title, name: name, Notes: Notes, Email: Email },
            beforeSend: function () { showLoader(); },


            success: function (result) {
                var IsRoleAdmin = $('#IsRoleAdmin').text();
                var table = '<table id="WebGrid" class="table table-striped table-bordered">';
                //table += '<tr><th>@Resource.FamilyMemberName</th><th>@Resource.SocialId</th><th>@Resource.Relation</th><th>@Resource.Email</th><th>@Resource.Phone</th></tr>';

                table += '<th>Biz Type</th>'
                table += '<th>CompanyName</th>'
                table += '<th>User Actions</th>'

                for (var i = 0; i < result.length; i++)
                {

                    var companyId = result[i].companyid;
                    var priority = result[i].priority;
                    var target = result[i].target;

                    if (priority == true)
                    {
                        table += '<tr class=Str_blue>';
                    }
                    else if (target == true)
                    {
                        table += '<tr class = Str_pink>';
                    }
                    else {
                        table += '<tr>'
                    }

                    table += '<td>' + result[i].biztype + '</td>';
                    table += '<td><a id=' + result[i].companyid + ' href=/ProspectViewCompany/Index?Compid=' + result[i].companyid + ' class=details> ' + result[i].name + '</a></td>';
                    if (IsRoleAdmin == "True")
                    {
                        var all_parameters = result[i].listid + ',' + result[i].companyid + ',"' + result[i].name + '"';
                        table += '<td>';
                        table += '<a id=' + result[i].companyid + ' href=/ProspectViewList/SaveCompany?companyidedit=' + result[i].companyid + ' class="details AnyAction"> <i class="fa fa-pencil-square-o"></i></a>';
                        table += "<a class='btn MoveList' onclick='showpopup_Move(" + all_parameters + ")' href='javascript:void(0)' title='Move company and their contacts with notes to another list'><i class='fa fa-exchange'></i></a>";
                        table += "<a class='btn Delete AnyAction' onclick='showpopup_Delete(" + all_parameters + ")' ><i class='fa fa-trash-o'></i></a>";
                        table += "</td>";
                    }
                    else
                    {
                        table += '<td><a id=' + result[i].companyid + ' href=/ProspectViewList/SaveCompany?companyidedit=' + result[i].companyid + ' class="details AnyAction"> <i class="fa fa-pencil-square-o"></i></a></td>';
                    }
                    //table += '<td>' + result[i].priority + '</td>';
                    //table += '<td>' + result[i].target + '</td>';
                    table += '</tr>';
                }

                table += '</table>';
                $("#tblpros").html(table);
                hideLoader();
                //$('#example').DataTable({
                //});
                // $("#divLoading").removeClass('show');;
            },

            error: function (error)
            {
            },

        });
    }
    else {
        window.location.href = "/Login/Index";
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
function showpopup_Delete(listid, companyid, companyName) {
    $("#hdnDeleteListId").val(listid);
    $("#hdnDeleteCompanyId").val(companyid);
    $("#deleteCompanyName").html(companyName);
    $("#DeleteConfirmModal").modal();
}

function showpopup_Delete_Confirm() {
    $("#DeleteConfirmModal").modal('hide');
    var url = "/ProspectViewList/DeleteCompany?companyidedit=" + $("#hdnDeleteCompanyId").val() + '&listid=' + $("#hdnDeleteListId").val();
    window.location.replace(url);
}
function showpopup_Delete_Cancel() {
    $("#DeleteConfirmModal").modal('hide');
}




// Functionality for Moving one list's companies and their contacts with notes to another list [START]
function showpopup_Move(listid, companyid, companyName) {
    $("#hdnlistid").val("");
    $("#hdncompanyid").val("");

    $("#ddlOtherList").val("");
    $("#divMessage_CompanyName").text("");
    $("#divMessage_ddlOtherList").text("");

    $.ajax({
        type: "GET",
        url: "/ProspectViewList/GetDataForMoveFunctionality",
        data: { ListID: listid },
        async: false,
        success: function (result) {
            $("#hdnlistid").val(listid);
            $("#hdncompanyid").val(companyid);
            $("#CompanyName").val(companyName);

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

function moveCompanyToAnotherList() {
    var moveFrom_ListID = parseInt($("#hdnlistid").val());
    var companyID = parseInt($("#hdncompanyid").val());
    var moveTo_ListID = parseInt($("#ddlOtherList").val());
    
    if (!(moveFrom_ListID > 0)) {
        $("#divMessage_ListId").text("List Id is empty. Please try again.");
        $("#divMessage_ListId").show();
        return false;
    }
    else {
        $("#divMessage_ListId").text("");
        $("#divMessage_ListId").hide();
    }

    if (!(companyID > 0)) {
        $("#divMessage_CompanyName").text("Company Id is empty. Please try again.");
        $("#divMessage_CompanyName").show();
        return false;
    }
    else {
        $("#divMessage_CompanyName").text("");
        $("#divMessage_CompanyName").hide();
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
        url: "/ProspectViewList/MoveCompanyToAnotherList",
        data: { ListID_from: moveFrom_ListID, CompanyId: companyID, ListID_to: moveTo_ListID },
        async: false,
        success: function (result) {
            if (result === true) {
                $("#MoveListDataModal").modal('hide');
                getcompanylist(moveFrom_ListID);
                $('#divmsg').text('Company moved successfully.');
                $('#divmsg').show();
                $('#div_error_msg').hide();
                //$("#divmsg").fadeOut(5000);
            }
            else {
                $("#MoveListDataModal").modal('hide');
                getcompanylist(moveFrom_ListID);
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


$(document).ready(function () {


    var Companyid = $("#hiddencmpnyid").html(); 
    
   
    if (Companyid != "" && Companyid != null) {
        //alert(Companyid);
        $("#" + Companyid).focus();
    }


    //if (localStorage.scrollPosition) {
    //    
    //    //alert("if=" + localStorage.getItem("scrollPosition"))
    //    $(window).scrollTop(localStorage.getItem("scrollPosition"))
    //    localStorage.setItem("scrollPosition", null);

    //}
    //else {
    //    //alert("else=" + localStorage.getItem("scrollPosition"));
    //}

    $('.AnyAction').click(function () {
        
        var tempScrollTop = $(window).scrollTop();
        localStorage.setItem("scrollPosition", tempScrollTop);
        var s = localStorage.getItem("scrollPosition")
    });



});


//$(document).ready(function () {
//    
//    if (window.history && history.pushState) {
//        addEventListener('load', function () {
//            history.pushState(null, null, null); // creates new history entry with same URL
//            addEventListener('popstate', function () {
          
//            });
//        });
//    }
//})




















