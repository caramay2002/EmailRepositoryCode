$(function () {

 //   $('.summernote').summernote({
 //       height: 400,                 // set editor height
 //       minHeight: 350,             // set minimum height of editor
	//	maxHeight: null,             // set maximum  height of editor
 //       onblur: function (e) {
 //           $('#message').html($('.summernote').code());
 //       },
 //       callbacks: {
 //           onImageUpload: function (image) {
 //               //uploadImage(image[0]);
 //               uploadImage(image);
 //           }
 //       }
	//});

	$('.summernote').summernote({
		width: 710,
		//height: 350,
		minHeight: 350,
		maxHeight: null,
		focus: true,
		toolbar: true,
		toolbar: [
			['style', ['bold', 'italic', 'underline', 'clear']],
			['fontsize', ['fontsize']],
			//['fontname', ['fontname']],
			//['color', ['color']],
			['height', ['height']],
			['table', ['table']],
			['insert', ['link', 'picture', 'hr']],
			['fontname', ['fontname']],
			['para', ['ul', 'ol', 'paragraph']],
			['view', ['fullscreen', 'codeview']],
			['help', ['help']],
			['misc', ['print']]
		],
		onblur: function (e) {
			$('#message').html($('.summernote').code());
		},
		callbacks: {
			onImageUpload: function (image) {
				//uploadImage(image[0]);
				uploadImage(image);
			}
		}

	});
	
});

function funcMyHtml() {
	// This is to get the batch values and then update to the hidden field
	var dpval = $("#SelectBunchValue").val();
	$("#HiddenBatchVal").val(dpval);

    //document.getElementById("txtTemphidden").value = $('.summernote').summernote('code');
}

$(document).ready(function () {

    $("#divcount").hide();

    $("#ddlcitycircle").val("");
    //Dropdownlist Selectedchange event
    $("#ddlcitycircle").change(function () {
        $("#divcount").hide();
        bindbiztype();
    });
    $("#ddltemplate").change(function () {
        // $("#divcount").hide();
        bindtemplatetype();
    });
    $("#ddlbiztype").change(function () {
        $("#divcount").hide();
    });
    $("#ddltitle").change(function () {
        if ($("#ddltitle").val() != "") {
            getcount();
        }
    });

    $("#btnsave").click(function () {
        $(".validateTips").html("");
        if ($("#ddlcitycircle").val() === "") {
            $('#ddlcitycircle').nextAll(".validateTips:first").html("Select City Circle");
            return false;
        }
        if ($("#ddltitle").val() === "") {
            $('#ddltitle').nextAll(".validateTips:first").html("Select Title");
            return false;
        }
        if ($("#ddltemplate").val() === "") {
            $('#ddltemplate').nextAll(".validateTips:first").html("Select Email Template");
            return false;
        }
        if ($('#SelectBunchValue').val() === "" || $('#SelectBunchValue').val() === null) {
            $('#SelectBunchValue').nextAll(".validateTips:first").html("Email batch can not be empty.");
            return false;
        }
        if ($("#txtsubject").val().trim() === "") {
            $('#txtsubject').nextAll(".validateTips:first").html("Enter Subject");
            return false;
        }
        if ($("#txtservername").val().trim() === "") {
            $('#txtservername').nextAll(".validateTips:first").html("Enter ServerName");
            return false;
        }
        if ($("#txtemail").val().trim() == "") {
            $('#txtemail').nextAll(".validateTips:first").html("Enter Email");
            return false;
        }
        if ($("#txtpassword").val().trim() == "") {
            $('#txtpassword').nextAll(".validateTips:first").html("Enter Password");
            return false;
        }
        if (!validateEmail($("#txtemail").val())) {
            $('#txtemail').nextAll(".validateTips:first").html("InValid Email");
            return false;
        }
    });
});

function validateEmail(Email) {
    //var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    //if (!filter.test(Email)) {
    //    return false;
    //}
    //else {
    //    return true;
    //}
	if (!isValidEmailAddress(Email)) {
		return false;
	} else {
		return true;
	}
}

function isValidEmailAddress(Email) {

	var pattern = /^([a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+(\.[a-z\d!#$%&'*+\-\/=?^_`{|}~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]+)*|"((([ \t]*\r\n)?[ \t]+)?([\x01-\x08\x0b\x0c\x0e-\x1f\x7f\x21\x23-\x5b\x5d-\x7e\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|\\[\x01-\x09\x0b\x0c\x0d-\x7f\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))*(([ \t]*\r\n)?[ \t]+)?")@(([a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\d\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.)+([a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]|[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF][a-z\d\-._~\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]*[a-z\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])\.?$/i;
	return pattern.test(Email);
}

function bindbiztype() {
    $("#ddlbiztype").empty();
    $("#ddlbiztype").append('<option value="">Select</option>');
    if ($('#IsAuthenticated').text() == 'True') {
        $.ajax({
            type: 'POST',
            url: "/EmailSendFrom/Getbiztype",//'@Url.Action("Getbiztype")', // we are calling json method
            dataType: 'json',
            data: { id: $("#ddlcitycircle").val() },
            success: function (biztype) {
                $.each(biztype, function (i, biztype) {
                    $("#ddlbiztype").append('<option value="' + biztype.Value + '">' +
                        biztype.Text + '</option>');

                });
            },
            error: function (ex) {

            }
        });
    }
    else {
        window.location.href = "\Login\Index";
    }
    return false;
}

function bindtemplatetype() {

    if ($('#IsAuthenticated').text() == 'True') {
        $.ajax({
            type: 'POST',
            url: "/EmailSendFrom/GetTemplatedata",//'@Url.Action("GetTemplatedata")', // we are calling json method
            dataType: 'json',
            data: { id: $("#ddltemplate").val() },
            success: function (data) {
                if (data != "LoginRequired") {
                    $("#txtsubject").val(data.Subject);

                    //$("#txtsubject").attr("disabled", "disabled");
                    $("#txtsubject").attr("Readonly", "True"); // ST
                    //CKEDITOR.instances['sampleEditor'].setData(data.EmailBody);

                    var editor = CKEDITOR.instances.CKEditorContent;
                    editor.setData(data.EmailBody);
                }
                else {
                    window.location.href = "/Login/Index";
                }
            },
            error: function (ex) {

            }
        });
    }
    else {
        window.location.href = "\Login\Index";
    }
    return false;
}

function getcount() {
    var prospectListvalue = $("#ddlcitycircle").val();
    var BizzTYpe = $("#ddlbiztype").val();
	var titleStandard = $("#ddltitle").val();

	$('#SelectBunchValue option').remove();  // remove the batch drop down values 

    if ($('#IsAuthenticated').text() == 'True') {
        $.ajax({
            type: 'POST',
            url: "/EmailSendFrom/Getcount",//'@Url.Action("Getcount")', // we are calling json method
            dataType: 'json',
            data: { prospectListvalue: prospectListvalue, BizzTYpe: BizzTYpe, titleStandard: titleStandard },
            success: function (data) {
				// To bind dynamic values in batch dropdown
                if (data.Count > 0) {
                    //$('#BunchSelectingDropdownDiv').css('display', 'block');
                    const BatchLimit = data.BatchLimit
                    const EmailCount = data.Count
                    var EmailbatchCount = EmailCount / BatchLimit;
                    if ((EmailbatchCount - Math.floor(EmailbatchCount)) !== 0) {
                        EmailbatchCount = EmailbatchCount + 1;
                    }
                    else {
                        EmailbatchCount = EmailbatchCount + 1;
                    }

                    var count = 0;
                    for (var i = 1; i < EmailbatchCount; i++) {
                        var set_count = 0;
                        if (i === 1 && parseInt(EmailCount) > BatchLimit) {
                            count = parseInt(EmailCount) - BatchLimit;
                            set_count = BatchLimit;
                        }
                        else if (i === 1 && parseInt(EmailCount) < BatchLimit) {
                            set_count = parseInt(EmailCount);
                        }
                        else {
                            if (count > BatchLimit) {
                                count = count - BatchLimit;
                                set_count = BatchLimit;
                            }
                            else {
                                set_count = count;
                            }
                        }

                        $('#SelectBunchValue').append('<option data="' + set_count + '" name="' + i + '" value="' + 'Batch-' + i + '">' + 'Batch-' + i + '</option>');
                    }

                    if (parseInt(EmailCount) > BatchLimit) {
                        $('#HiddenBatchEmailCount').val(BatchLimit);
                        $("#divcnt").text("Total mail to be send: " + BatchLimit);
                        $("#divcount").show();
                    }
                    else {
                        $('#HiddenBatchEmailCount').val(parseInt(EmailCount));
                        $("#divcnt").text("Total mail to be send: " + parseInt(EmailCount));
                        $("#divcount").show();
                    }
                }
                else {
                    $('#HiddenBatchEmailCount').val(parseInt(EmailCount));
                    $("#divcnt").text("Total mail to be send: " + EmailCount);
                    $("#divcount").show();
                }
            },
            error: function (ex) {

            }
        });
        return false;
    }
    else {
        window.location.href = "\Login\Index";
    }
}

function uploadImage(image) {

    var Formdata = new FormData();
    Formdata.append("image", image[0]);

    xhr = new XMLHttpRequest();

    xhr.open('POST', '/EmailSendFrom/UploadFile', true);
    xhr.onreadystatechange = function (response) {
        if (xhr.readyState === 4 && response.currentTarget.status == "200") {
            
            var image = $('<img>').attr('src', '../ImagesEmail/' + response.currentTarget.responseText.toString().replace(/"/g, ""));
            var Imagepath = '../ImagesEmail/' + response.currentTarget.responseText.toString().replace(/"/g, "");
            $("#hiddenImageName").val(response.currentTarget.responseText.toString().replace(/"/g, ""));
            $('.summernote').summernote("insertNode", image[0])          
           // response.currentTarget.responseText
        }
    };
    xhr.send(Formdata);

    //$.ajax({
    //    type: 'POST',
    //    url: "/EmailSendFrom/UploadFile",//'@Url.Action("Getcount")', // we are calling json method
    //    dataType: 'json',
    //    data: Formdata,
    //    success: function (data) {
    //        alert("in")
    //        var image = $('<img>').attr('src', '~/ImagesEmail/' + url);
    //       $("#hiddenImageName").val(url);
    //        $('.summernote').summernote("insertNode", image[0]);
    //    },
    //    error: function (ex) {
    //        alert(ex + "else")
    //    }
    //});



    //$.ajax({
    //    //url: '/EmailSendFrom/UploadSummerNoteFile',
    //    url: "/EmailSendFrom/UploadFile", //'@Url.Action("UploadFile")',
    //    //cache: false,
    //    //contentType: false,
    //    //processData: false,
    //    type: "POST",
    //    data: data,
    //    async: false,
    //    //success: function (data) {
    //    //    alert(data)
    //    //    var image = $('<img>').attr('src', 'data:' + file.type + ';base64,' + data);
    //    //    $('.summernote').summernote("insertNode", image[0]);
    //    //},
    //    //error: function (data) {
    //    //    alert("error")
    //    //    console.log(data);
    //    //}
    //    success: function (url) {
    //        var image = $('<img>').attr('src', '~/ImagesEmail/' + url);
    //        $("#hiddenImageName").val(url);
    //        $('.summernote').summernote("insertNode", image[0]);
    //    },
    //    error: function (data) {
    //        console.log(data);
    //    }
    //});
}

$("#SelectBunchValue").on("change", function () {
    var batchNumber = this.value.split("-")[1];

    if (batchNumber) {
        var row_id = parseInt(batchNumber) - 1;
        var count = $($("#SelectBunchValue").children("option")[row_id]).attr("data");

        $('#HiddenBatchEmailCount').val(count);
        $("#divcnt").text("Total mail to be send: " + count);
        $("#divcount").show();
    }
});

$("#ddlcitycircle").on("change", function () {
    $('#ddltitle').val("");
    $('#SelectBunchValue option').remove();  // remove the batch drop down values 
    $('#HiddenBatchEmailCount').val("");
    $("#divcnt").text("");
    $("#divcount").hide();
});

$("#ddlbiztype").on("change", function () {
    $('#ddltitle').val("");
    $('#SelectBunchValue option').remove();  // remove the batch drop down values 
    $('#HiddenBatchEmailCount').val("");
    $("#divcnt").text("");
    $("#divcount").hide();
});

$("#ddltitle").on("change", function () {
    if (this.value === "" || this.value === null) {
        $('#SelectBunchValue option').remove();  // remove the batch drop down values 
        $('#HiddenBatchEmailCount').val("");
        $("#divcnt").text("");
        $("#divcount").hide();
    }
});