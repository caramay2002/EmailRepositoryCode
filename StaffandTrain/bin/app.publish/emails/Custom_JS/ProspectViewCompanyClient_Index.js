$(document).ready(function () {

    getlist();
    $(".Edit").bind("click", Edit);
    $("#lnkclear").click(function () {
        $("#txtname").val("");
        $("#txtsearchnotes").val("");
    });

    $("#lnksearch").click(function () {
        getlist();
    });
    $(".conresume").click(function () {
        $(".validateTips").html("");
        var contactid = $(this).attr("data-resume");
        if ($('#IsAuthenticated').text() == 'True') {
            $.ajax({
                type: "POST",
                url: "/ProspectViewCompanyClient/GetConResume",
                async: false,
                data: { contactid: contactid },
                success: function (msg) {
                    $('#FileManageModal').modal('show');
                    $("#txtcontactid").val(contactid);
                    $("#txtresumes").val(msg);

                },
                error: function (error) {
                }
            });
        }
        else {
            window.location.href = "\Login\Index";
        }

    });
    $(".conmove").click(function () {
        $(".validateTips").html("");
        $('#FileManageModalmover').modal('show');
        $("#Ddlcomp").val("");
        $("#divmover").text($("#tdcompname").text().trim());
        $("#txtcontactidmove").val($(this).attr("data-resume"))
    });
    $(document).on("click", "a[name='btnmove']", function (e) {
        $(".validateTips").html("");
        if ($("#Ddlcomp").val() == "") {
            $('#Ddlcomp').nextAll(".validateTips:first").html("Select Company");
            return false;
        }
        if ($('#IsAuthenticated').text() == 'True') {
            var contactid = $("#txtcontactidmove").val();
            var companyid = $("#Ddlcomp").val();
            var companyidold = $("#hdncompid").val();
            $.ajax({
                type: "POST",
                url: "/ProspectViewCompanyClient/MoveContact",
                async: false,
                data: { contactid: contactid, companyid: companyid, companyidold: companyidold },
                success: function (msg) {

                    $('#FileManageModalmover').modal('hide');
                    $("#Ddlcomp").val("");
                    if (msg != "") {
                        window.location.href = msg;
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



    $(document).on("click", "a[name='btnupdate']", function (e) {
        $(".validateTips").html("");
        if ($("#txtresumes").val() == "") {
            $('#txtresumes').nextAll(".validateTips:first").html("Enter Resumes");
            return false;
        }
        if ($('#IsAuthenticated').text() == 'True') {
            var contactid = $("#txtcontactid").val();
            var resumes = $("#txtresumes").val();
            $.ajax({
                type: "POST",
                url: "/ProspectViewCompanyClient/UpdateConResume",
                async: false,
                data: { contactid: contactid, resumes: resumes },
                success: function (msg) {

                    $('#FileManageModal').modal('hide');
                    $("#txtcontactid").val("");
                    $("#txtresumes").val("");

                },
                error: function (error) {
                }
            });
        }
        else {
            window.location.href = "\Login\Index";
        }




    });


    $(document).on("click", "a[name='btnupdatcon']", function (e) {
        var contactid = $(this).attr("data-contactid");
        $(".validateTips").html("");
        if ($("#txtEmail").val().trim() != "") {
            if (!validateEmail($("#txtEmail").val().trim())) {
                $('#txtEmail').nextAll(".validateTips:first").html("Invalid Email");
                return false;
            }

        }
        if ($("#txtLinkedIn").val().trim() != "") {
            var url = new RegExp('^(http|https|www)://.*$');
            if (!url.test($("#txtLinkedIn").val())) {
                $('#txtLinkedIn').nextAll(".validateTips:first").html("Enter Valid Url");
                return false;
            }


        }


        var ContactDetails = {
            'contactid': contactid,
            'contactfullname': $("#txtName").val(),
            'titlestandard': $("#ddltitle").val(),
            'contactphone': $("#txtWphone").val(),
            'contactcellphone': $("#txtCPhone").val(),
            'contactemail': $("#txtEmail").val(),
            'linkedinprofileurl': $("#txtLinkedIn").val(),
            'combinednotes': $("#txtcombinednotes").val()
        }
        if ($('#IsAuthenticated').text() == 'True') {
            $.ajax({
                type: "POST",
                url: "/ProspectViewCompanyClient/UpdateContact",
                async: false,
                data: ContactDetails,
                success: function (msg) {

                    if (msg == "Success") {
                        location.reload();
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


    $(document).on("click", "a[name='btncancelupdate']", function (e) {
        $('#example tr:not(:first)').each(function () {
            var htmldata = "";
            var td1 = $(this).children("td:nth-child(1)");
            var td2 = $(this).children("td:nth-child(2)");
            //var tdEmail = par.children("td:nth-child(3)");
            //var tdButtons = par.children("td:nth-child(4)");
            var htmldata = "";
            if (td1.children('input').length > 0) {
                htmldata = htmldata + "<label id='lblid'>" + td1.children('#lblid').text() + "</label><br />";
                htmldata = htmldata + "Name :" + td1.children('#txtName').val() + "<br />";
                htmldata = htmldata + "Title :" + td1.children('#ddltitle').val() + "<br />";
                htmldata = htmldata + "WPhone :" + td1.children('#txtWphone').val() + "<br />";
                htmldata = htmldata + "CPhone :" + td1.children('#txtCPhone').val() + "<br />";
                htmldata = htmldata + "Email : <a href='mailto:" + td1.children('#txtEmail').val() + "'>" + td1.children('#txtEmail').val() + "</a><br />";
                htmldata = htmldata + "LinkedIn :<a target='_blank' href=" + td1.children('#txtLinkedIn').val() + ">" + td1.children('#txtLinkedIn').val() + "</a><br />";
                td1.html(htmldata);
                td2.html("<pre>" + td2.children('#txtcombinednotes').val() + "</pre>");

            }
        });

    });


    function getlist() {

        var name = $("#txtname").val().trim();
        var Notes = $("#txtsearchnotes").val().trim();
        var listid = $("#hdnlistid").val();
        var companyid = $("#hdncompid").val();
        var emailaddes = $('#ConEmail').text();

        $.ajax({
            type: "POST",
            url: "/ProspectViewCompanyClient/Getlist",
            async: false,
            data: { name: name, Notes: Notes, listid: listid, companyid: companyid, emailaddes: emailaddes },
            success: function (msg) {

                $("#tblcon").html(msg);


            },
            error: function (error) {
            }
        });
    }

    function Edit() {
        ;
        var par = $(this).parent().parent(); //tr

        $('#example tr:not(:first)').each(function () {

            var htmldata = "";
            var td1 = $(this).children("td:nth-child(1)");
            var td2 = $(this).children("td:nth-child(2)");
            //var tdEmail = par.children("td:nth-child(3)");
            //var tdButtons = par.children("td:nth-child(4)");
            var htmldata = "";
            if (td1.children('input').length > 0) {

                htmldata = htmldata + "<label id='lblid'>" + td1.children('#lblid').text() + "</label><br />";
                htmldata = htmldata + "Name :" + td1.children('#txtName').val() + "<br />";
                htmldata = htmldata + "Title :" + td1.children('#ddltitle').val() + "<br />";
                htmldata = htmldata + "WPhone :" + td1.children('#txtWphone').val() + "<br />";
                htmldata = htmldata + "CPhone :" + td1.children('#txtCPhone').val() + "<br />";
                htmldata = htmldata + "Email : <a href='mailto:" + td1.children('#txtEmail').val() + "'>" + td1.children('#txtEmail').val() + "</a><br />";
                htmldata = htmldata + "LinkedIn :<a target='_blank' href=" + td1.children('#txtLinkedIn').val() + ">" + td1.children('#txtLinkedIn').val() + "</a><br />";
                td1.html(htmldata);
                td2.html(td2.children('#txtcombinednotes').val());

            }


        });


        var contactid = $(this).attr("data-contactid");
        var tdName = par.children("td:nth-child(1)");
        var tdPhone = par.children("td:nth-child(2)");
        if ($('#IsAuthenticated').text() == 'True') {
            $.ajax({
                type: "POST",
                url: "/ProspectViewCompanyClient/GetContactDetails",
                async: false,
                data: { contactid: contactid },
                success: function (msg) {

                    var drptile = "<select id='ddltitle'><option value=''>Select</option>";
                    $(msg.Titlelist).each(function (index) {
                        if (msg.data.titlestandard == msg.Titlelist[index].Value) {
                            drptile = drptile + "<option value='" + msg.Titlelist[index].Value + "' selected>" + msg.Titlelist[index].Text + " </option>";
                        }
                        else {
                            drptile = drptile + "<option value='" + msg.Titlelist[index].Value + "'>" + msg.Titlelist[index].Text + " </option>";
                        }
                    });
                    drptile = drptile + "</select>"
                    var Html = "";
                    Html = Html + " <label id='lblid'>ID :" + msg.data.contactid + "</label><br /> ";
                    Html = Html + "<label class='lbl_edit'>Name :</label><input type='text' id='txtName'  class='edit_input'   value='" + msg.data.contactfullname + "'/>  <br />";
                    Html = Html + "<label class='lbl_edit'>Title :</label>" + drptile + "<br />";
                    Html = Html + "<label class='lbl_edit'>WPhone :</label><input type='text' id='txtWphone'  class='edit_input' value='" + msg.data.contactphone + "'/><br />";
                    Html = Html + "<label class='lbl_edit'>CPhone :</label><input type='text' id='txtCPhone'  class='edit_input' value='" + msg.data.contactcellphone + "'/><br />";
                    Html = Html + "<label class='lbl_edit'>Email :</label><input type='text' id='txtEmail'  class='edit_input' value='" + msg.data.contactemail + "'/><div id='' class='validateTips'></div>";
                    Html = Html + "<label class='lbl_edit'>LinkedIn :</label><input type='text' id='txtLinkedIn'  class='edit_input' value='" + msg.data.linkedinprofileurl + "'/>  <div id='' class='validateTips'></div>";
                    tdName.html(Html);
                    tdPhone.html("<textarea rows='8' cols='50'  id='txtcombinednotes'>" + msg.data.combinednotes + "</textarea><br /> <a href='javascript:void(0);' id ='btnupdatcon' data-contactid=" + msg.data.contactid + " name ='btnupdatcon'  class='btn btn-success'> Update</a> <a href='javascript:void(0);' id ='btncancel' data-contactid=" + msg.data.contactid + " name ='btncancelupdate'  class='btn btn-success'> Cancel</a>");
                },
                error: function (error) {
                }
            });
        }
        else {
            window.location.href = "\Login\Index";
        }



        //$("#txtId").focus();
    };

    function validateEmail(Email) {
        var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
        if (!filter.test(Email)) {
            return false;
        }
        else {
            return true;
        }
    }





});