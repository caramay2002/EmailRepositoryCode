$(document).ready(function () {

    // Maintain page scroll state 
    $(document).scroll(function () {
        localStorage['page'] = document.URL;
        localStorage['scrollTop'] = $(document).scrollTop();
    });
   

    getlist();

    
    $(document).delegate('.Edit', 'click', function () {
        
            ;
            var par = $(this).parent().parent(); //tr

            $('#example tr:not(:first)').each(function () {

                var htmldata = "";
                var td1 = $(this).children("td:nth-child(1)");
                var td2 = $(this).children("td:nth-child(2)");
                //var tdEmail = par.children("td:nth-child(3)");
                //var tdButtons = par.children("td:nth-child(4)");
                var Data = "test";
                var htmldata = "";
                if (td1.children('input').length > 0) {

                    htmldata = htmldata + "<label id='lblid'>" + td1.children('#lblid').text() + "</label><br />";
                    htmldata = htmldata + "Name: " + td1.children('#txtName').val() + "<br />";
                    htmldata = htmldata + "Title: " + td1.children('#ddltitle').val() + "<br />";
                    htmldata = htmldata + "WPhone: " + td1.children('#txtWphone').val() + "<br />";
                    htmldata = htmldata + "CPhone: " + td1.children('#txtCPhone').val() + "<br />";
                    htmldata = htmldata + "Email: <a href='mailto:" + td1.children('#txtEmail').val() + "'>" + td1.children('#txtEmail').val() + "</a><br />";
                    htmldata = htmldata + "LinkedIn: <a target='_blank'  href=" + td1.children('#txtLinkedIn').val() + ">" + td1.children('#txtLinkedIn').val() + "</a><br />";
                    td1.html(htmldata);
                    td2.html(td2.children('#txtcombinednotes').val());

                }


            });


            var contactid = $(this).attr("data-contactid");
            //var tdName = par.children("td:nth-child(1)");

            var tdName = par.children("td:nth-child(1)").children("span");

            var tdPhone = par.children("td:nth-child(2)");
            if ($('#IsAuthenticated').text() == 'True') {
                $.ajax({
                    type: "POST",
                    url: "/ProspectViewCompany/GetContactDetails",
                    async: false,
                    data: { contactid: contactid },
                    success: function (msg) {
                        ;
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
                        Html = Html + "<label class='lbl_edit'>Email :</label><input type='text' id='txtEmail'  class='edit_input' value='" + msg.data.contactemail + "'/><div id='' class='validateTips'></div><br />";
                        Html = Html + "<label class='lbl_edit'>LinkedIn :</label><input type='text' id='txtLinkedIn'  class='edit_input' value='" + msg.data.linkedinprofileurl + "'/><div id='' class='validateTips'></div>";
                        //Html = Html + "<label>Actions :</label><a class='btn Edit'><i class='fa fa-pencil-square-o'></i></a></div>";
                        tdName.html(Html);
                        //tdPhone.html("<textarea rows='8' cols='50'  id='txtcombinednotes'>" + msg.data.combinednotes + "</textarea><br /> <a href='javascript:void(0);' id ='btnupdatcon' data-contactid=" + msg.data.contactid + " name ='btnupdatcon'  class='btn btn-success'> Update</a> <a href='javascript:void(0);' id ='btncancel' data-contactid=" + msg.data.contactid + " name ='btncancelupdate'  class='btn btn-success'> Cancel</a>");
                        tdPhone.html("<textarea rows='8' cols='50'  id='txtcombinednotes' style='padding: 10px; word-break: break-word; width:100%'>" + msg.data.combinednotes + "</textarea><br /> <a href='javascript:void(0);' id ='btnupdatcon' data-contactid=" + msg.data.contactid + " name ='btnupdatcon'  class='btn btn-success AnyAction'> Update</a> <a href='javascript:void(0);' id ='btncancel' data-contactid=" + msg.data.contactid + " name ='btncancelupdate'  class='btn btn-success'> Cancel</a>");
                    },
                    error: function (error) {
                    }
                });

            }
            else {
                window.location.href = "\Login\Index";
            }



            //$("#txtId").focus();
       
    });
   
 //  $(".Edit").bind("click", Edit);

    $("#lnkclear").click(function () {
        $("#txtname").val("");
        $("#txtsearchnotes").val("");
    });

    $("#lnksearch").click(function () {     
        getlist();
    });

    $("#tblcon").on("click", ".conresume", function () {
   // $(".conresume").click(function () {
      
        
        if ($('#IsAuthenticated').text() == 'True') {
            $(".validateTips").html("");
            var contactid = $(this).attr("data-resume");
            $.ajax({
                type: "POST",
                url: "/ProspectViewCompany/GetConResume",
                async: false,
                data: { contactid: contactid },
                success: function (msg) {
                    ;
                    $('#FileManageModal').modal('show');
                    $("#txtcontactid").val(contactid);
                    $("#txtresumes").val(msg);

                },
                error: function (error) {
                    ;
                }
            });
        }
        else {
            window.location.href = "\Login\Index";
        }
    });

    $("#tblcon").on("click", ".conmove", function () {
        
   // $(".conmove").click(function () {
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
                url: "/ProspectViewCompany/MoveContact",
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

    //$(document).on("click", "a[name='btnupdate']", function (e) {
    //    $(".validateTips").html("");
    //    if ($("#txtresumes").val() == "") {
    //        $('#txtresumes').nextAll(".validateTips:first").html("Enter Resumes");
    //        return false;
    //    }
    //    var contactid = $("#txtcontactid").val();
    //    var resumes = $("#txtresumes").val();
    //    if ($('#IsAuthenticated').text() == 'True') {
    //        $.ajax({
    //            type: "POST",
    //            url: "/ProspectViewCompany/UpdateConResume",
    //            async: false,
    //            data: { contactid: contactid, resumes: resumes },
    //            success: function (msg) {

    //                $('#FileManageModal').modal('hide');
    //                $("#txtcontactid").val("");
    //                $("#txtresumes").val("");

    //            },
    //            error: function (error) {
    //            }
    //        });
    //    }
    //    else {
    //        window.location.href = "\Login\Index";
    //    }
    //});
    $(document).on("click", "a[name='btnupdate']", function (e) {
        debugger

        var allowedFiles = [".doc", ".docx", ".pdf"];
        var fileUpload = $("#FileUpload1");
        var lblError = $("#lblError");
        var regex = new RegExp("([a-zA-Z0-9\s_\\.\-:])+(" + allowedFiles.join('|') + ")$");
        if (!regex.test(fileUpload.val().toLowerCase())) {
            lblError.html("Please upload files having extensions: <b>" + allowedFiles.join(', ') + "</b> only.");
            return false;
        }
        lblError.html('');
     
        $(".validateTips").html("");
        if ($("#txtresumes").val() === "") {
            //$('#txtresumes').nextAll(".validateTips:first").html("Enter Resumes");
           // $('#FileUpload1').nextAll(".validateTips:first").html("pdf file is required");
            //return false;
        }

        var contactid = $("#txtcontactid").val();
        var resumes = $("#txtresumes").val();


        // Create FormData object
        var SaveData = new FormData();

        var fileUpload = $("#FileUpload1").get(0);

        var files = fileUpload.files;
        for (var i = 0; i < files.length; i++) {
            SaveData.append(files[i].name, files[i]);
        }

        SaveData.append('contactid', contactid);
        SaveData.append('resumes', resumes);

        if ($('#IsAuthenticated').text() === 'True') {

            $.ajax({
                url: "/ProspectViewCompany/UpdateConResumeFile",
                type: 'POST',
                data: SaveData,
                cache: false,
                contentType: false,
                processData: false,
                success: function (file) {
                    debugger
                    $('#FileManageModal').modal('hide');
                    $("#txtcontactid").val("");
                    $("#txtresumes").val("");
                    $("FileUpload1").val("");
                    getlist();
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
        
        //$(".validateTips").html("");
        //if ($("#txtresumes").val() == "") {
        //    $('#txtresumes').nextAll(".validateTips:first").html("Enter Resumes");
        //    return false;
        //}
        if ($('#IsAuthenticated').text() == 'True') {
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

            $.ajax({
                type: "POST",
                url: "/ProspectViewCompany/UpdateContact",
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
            var td1 = $(this).children("td:nth-child(1)").children("span");
            var td2 = $(this).children("td:nth-child(2)");
            //var tdEmail = par.children("td:nth-child(3)");
            //var tdButtons = par.children("td:nth-child(4)");
            var Data = "test";
            var htmldata = "";
            if (td1.children('input').length > 0) {
                htmldata = htmldata + "<label id='lblid'>" + td1.children('#lblid').text() + "</label><br />";
                htmldata = htmldata + "Name: " + td1.children('#txtName').val() + "<br />";
                htmldata = htmldata + "Title: " + td1.children('#ddltitle').val() + "<br />";
                htmldata = htmldata + "WPhone: " + td1.children('#txtWphone').val() + "<br />";
                htmldata = htmldata + "CPhone: " + td1.children('#txtCPhone').val() + "<br />";
                htmldata = htmldata + "Email: <a href='mailto:" + td1.children('#txtEmail').val() + "'>" + td1.children('#txtEmail').val() + "</a><br />";
                htmldata = htmldata + "LinkedIn: <a target='_blank' href=" + td1.children('#txtLinkedIn').val() + ">" + td1.children('#txtLinkedIn').val() + "</a><br />";
                td1.html(htmldata);
                td2.html("<pre>" + td2.children('#txtcombinednotes').val() + "</pre>");

            }


        });

    });

    if (localStorage.scrollPosition) {
       
        $(window).scrollTop(localStorage.getItem("scrollPosition"))
        localStorage.setItem("scrollPosition", null);

    }
    else {
        //alert("else=" + localStorage.getItem("scrollPosition"));
    }

    $('.AnyAction').click(function () {
        
        var tempScrollTop = $(window).scrollTop();
        localStorage.setItem("scrollPosition", tempScrollTop);
        var s = localStorage.getItem("scrollPosition")

    });
});



function getlist() {

    debugger
    if ($('#IsAuthenticated').text() == 'True') {
        var name = $("#txtname").val().trim();
        var Notes = $("#txtsearchnotes").val().trim();
        var listid = $("#hdnlistid").val();
        var companyid = $("#hdncompid").val();
        var emailaddes = $('#ConEmail').text();
        $.ajax({
            type: "POST",
            url: "/ProspectViewCompany/Getlist",
            async: false,
            data: { name: name, Notes: Notes, listid: listid, companyid: companyid, emailaddes: emailaddes },
            success: function (msg) {
                var top = localStorage['scrollTop'];
                if (localStorage['page'] == document.URL) {
                    $(document).scrollTop(localStorage['scrollTop']);
                }
                setTimeout(() => {
                    if (localStorage['page'] == document.URL) {
                        $(document).scrollTop(top);
                    }
                }, 500);
                
                $("#tblcon").html(msg);
                $(".loading").hide();
            },
            error: function (error) {
            }
        });
    }
    else {
        window.location.href = "\Login\Index";
    }
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
        var Data = "test";
        var htmldata = "";
        if (td1.children('input').length > 0) {

            htmldata = htmldata + "<label id='lblid'>" + td1.children('#lblid').text() + "</label><br />";
            htmldata = htmldata + "Name: " + td1.children('#txtName').val() + "<br />";
            htmldata = htmldata + "Title: " + td1.children('#ddltitle').val() + "<br />";
            htmldata = htmldata + "WPhone: " + td1.children('#txtWphone').val() + "<br />";
            htmldata = htmldata + "CPhone: " + td1.children('#txtCPhone').val() + "<br />";
            htmldata = htmldata + "Email: <a href='mailto:" + td1.children('#txtEmail').val() + "'>" + td1.children('#txtEmail').val() + "</a><br />";
            htmldata = htmldata + "LinkedIn: <a target='_blank'  href=" + td1.children('#txtLinkedIn').val() + ">" + td1.children('#txtLinkedIn').val() + "</a><br />";
            td1.html(htmldata);
            td2.html(td2.children('#txtcombinednotes').val());

        }


    });


    var contactid = $(this).attr("data-contactid");
    //var tdName = par.children("td:nth-child(1)");
    
    var tdName = par.children("td:nth-child(1)").children("span");

    var tdPhone = par.children("td:nth-child(2)");
    if ($('#IsAuthenticated').text() == 'True') {
        $.ajax({
            type: "POST",
            url: "/ProspectViewCompany/GetContactDetails",
            async: false,
            data: { contactid: contactid },
            success: function (msg) {
                ;
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
                Html = Html + "<label class='lbl_edit'>Email :</label><input type='text' id='txtEmail'  class='edit_input' value='" + msg.data.contactemail + "'/><div id='' class='validateTips'></div><br />";
                Html = Html + "<label class='lbl_edit'>LinkedIn :</label><input type='text' id='txtLinkedIn'  class='edit_input' value='" + msg.data.linkedinprofileurl + "'/><div id='' class='validateTips'></div>";
                //Html = Html + "<label>Actions :</label><a class='btn Edit'><i class='fa fa-pencil-square-o'></i></a></div>";
                tdName.html(Html);
                //tdPhone.html("<textarea rows='8' cols='50'  id='txtcombinednotes'>" + msg.data.combinednotes + "</textarea><br /> <a href='javascript:void(0);' id ='btnupdatcon' data-contactid=" + msg.data.contactid + " name ='btnupdatcon'  class='btn btn-success'> Update</a> <a href='javascript:void(0);' id ='btncancel' data-contactid=" + msg.data.contactid + " name ='btncancelupdate'  class='btn btn-success'> Cancel</a>");
                tdPhone.html("<textarea rows='8' cols='50'  id='txtcombinednotes' style='padding: 10px; word-break: break-word; width:100%'>" + msg.data.combinednotes + "</textarea><br /> <a href='javascript:void(0);' id ='btnupdatcon' data-contactid=" + msg.data.contactid + " name ='btnupdatcon'  class='btn btn-success AnyAction'> Update</a> <a href='javascript:void(0);' id ='btncancel' data-contactid=" + msg.data.contactid + " name ='btncancelupdate'  class='btn btn-success'> Cancel</a>");
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

$(document).ready(function () {
    
    if (window.history && history.pushState) {
        addEventListener('load', function () {
            history.pushState(null, null, null); // creates new history entry with same URL
            addEventListener('popstate', function () {
                //var stayOnPage = confirm("Would you like to save this draft?");

                
                var listId = $('#hdnlistid').val();
                var compid = $('#hdncompid').val();
                //alert("listId"+listId)
               // alert("compid" +compid)
                window.location = '/ProspectViewList/Index?listid='+listId+"&cmpnyid=" + compid
                //if (!stayOnPage) {
                //    history.back()
                //} else {
                //    history.pushState(null, null, null);
                //}
               
            });
        });
    }
})




