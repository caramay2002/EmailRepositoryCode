
    $(document).ready(function () {


        $("#btnsave").click(function () {

            $(".validateTips").html("");
            if ($("#txttemplatename").val().trim() == "") {
                $('#txttemplatename').nextAll(".validateTips:first").html("Enter Template Name");
                return false;
            }
            if ($("#txtSubject").val().trim() == "") {
                $('#txtSubject').nextAll(".validateTips:first").html("Enter Subject");
                return false;
            }
            if (CKEDITOR.instances.sampleEditor.document.getBody().getChild(0).getText().trim().length == 0) {
                $('#sampleEditor').nextAll(".validateTips:first").html("Enter Template Body");
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

    });
});

