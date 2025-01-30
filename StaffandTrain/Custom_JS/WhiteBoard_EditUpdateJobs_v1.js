$(document).ready(function () {

    $("#btnsave").click(function () {
        $(".validateTips").html("");
        if ($("#txtNotes").val().trim() == "") {
            $('#txtNotes').nextAll(".validateTips:first").html("Please enter notes");
            return false;
		}
		if (CKEDITOR.instances.sampleEditor.document.getBody().getChild(0).getText().trim().length == 0) {
			$('#txtNotes').nextAll(".validateTips:first").html("Please enter notes");
			return false;
		}

    });


	

});


$(function () {

	$('.summernote').summernote({
		height: 400,                 // set editor height
		minHeight: 350,             // set minimum height of editor
		maxHeight: null,             // set maximum  height of editor
        toolbar: [
            ['style', ['bold', 'italic', 'underline', 'clear']], // style options
            ['fontsize', ['fontsize']], // font size options
            ['fontname', ['fontname']], // font name options
            ['color', ['color']],       // font color option
            ['height', ['height']],     // line height options
            ['table', ['table']],       // table options
            ['insert', ['link', 'picture', 'hr']], // insert options
            ['para', ['ul', 'ol', 'paragraph']], // paragraph options
            ['view', ['fullscreen', 'codeview']], // view options
            ['help', ['help']],         // help option
            ['misc', ['print']]         // print option
        ],
		onblur: function (e) {
			$('#txtNotes').html($('.summernote').code());
		},

	});
});